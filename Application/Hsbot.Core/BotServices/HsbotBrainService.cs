using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Hsbot.Core.Brain;
using Microsoft.Extensions.Logging;

namespace Hsbot.Core.BotServices
{
    public sealed class HsbotBrainService : IBotBrain, IBotService, IDisposable
    {
        private readonly IBotBrainStorage<InMemoryBrain> _botBrainStorage;
        private readonly ILogger<HsbotBrainService> _log;
        private InMemoryBrain _brain;
        private bool _persistenceEnabled;

        private IDisposable _brainChangedEventSubscription;

        public HsbotBrainService(IBotBrainStorage<InMemoryBrain> botBrainStorage, ILogger<HsbotBrainService> log)
        {
            _botBrainStorage = botBrainStorage;
            _log = log;
        }

        public static readonly int StartupOrder = BotStartupOrder.First;

        public int GetStartupOrder()
        {
            return StartupOrder;
        }

        public async Task Start(BotServiceContext context)
        {
            if (_brain != null)
            {
                throw new InvalidOperationException("Service is already started");
            }

            try
            {
                _log.LogInformation("Initializing brain");

                _brain = await _botBrainStorage.Load();
                _brainChangedEventSubscription = _brain.BrainChanged
                    .Select(SaveBrain)
                    .Window(1) //ensure we only run 1 call to save brain method at a given time
                    .Concat()
                    .Subscribe();

                _persistenceEnabled = true;

                _log.LogInformation("Brain loaded from storage successfully");
            }

            catch (Exception e)
            {
                _log.LogError("Error loading brain - falling back to an in-memory brain without persistence.");
                _log.LogError("Brain load exception: {0}", e);

                _brain = new InMemoryBrain();
            }
        }

        private async Task SaveBrain(InMemoryBrain brain)
        {
            try
            {
                if (_persistenceEnabled)
                {
                    _log.LogInformation("Saving brain to storage");
                    await _botBrainStorage.Save(brain);
                    _log.LogInformation("Brain saved successfully");
                }
            }

            catch (Exception e)
            {
                _log.LogError("Failed to save brain to storage: {0}", e);
            }
        }

        public async Task Stop()
        {
            await SaveBrain(_brain);
        }

        public ICollection<string> Keys => _brain.Keys;

        public T GetItem<T>(string key) where T : class
        {
            return _brain.GetItem<T>(key);
        }

        public PersistenceState SetItem<T>(string key, T value) where T : class
        {
            _log.LogDebug($"Saving new value to brain for Key={key}");
            _brain.SetItem(key, value);

            return _persistenceEnabled
                ? PersistenceState.Persisted
                : PersistenceState.InMemoryOnly;
        }

        public string BrainDump()
        {
            return _brain.BrainDump();
        }

        public void Dispose()
        {
            _brainChangedEventSubscription?.Dispose();
        }
    }
}