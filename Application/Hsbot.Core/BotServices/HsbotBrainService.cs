using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Hsbot.Core.Brain;

namespace Hsbot.Core.BotServices
{
    public sealed class HsbotBrainService : IBotBrain, IBotService, IDisposable
    {
        private readonly IBotBrainStorage<HsbotBrain> _botBrainStorage;
        private readonly IHsbotLog _log;
        private HsbotBrain _brain;
        private bool _persistenceEnabled;

        private IDisposable _brainChangedEventSubscription;

        public int StartupOrder => 0;

        public HsbotBrainService(IBotBrainStorage<HsbotBrain> botBrainStorage, IHsbotLog log)
        {
            _botBrainStorage = botBrainStorage;
            _log = log;
        }

        public async Task Start(BotServiceContext context)
        {
            if (_brain != null)
            {
                throw new InvalidOperationException("Service is already started");
            }

            try
            {
                _log.Info("Initializing brain");

                _brain = await _botBrainStorage.Load();
                _brainChangedEventSubscription = _brain.BrainChanged
                    .Select(SaveBrain)
                    .Window(1) //ensure we only run 1 call to save brain method at a given time
                    .Concat()
                    .Subscribe();

                _persistenceEnabled = true;

                _log.Info("Brain loaded from storage successfully");
            }

            catch (Exception e)
            {
                _log.Error("Error loading brain - falling back to an in-memory brain without persistence.");
                _log.Error("Brain load exception: {0}", e);

                _brain = new HsbotBrain();
            }
        }

        private async Task SaveBrain(HsbotBrain brain)
        {
            try
            {
                if (_persistenceEnabled)
                {
                    _log.Info("Saving brain to storage");
                    await _botBrainStorage.Save(brain);
                    _log.Info("Brain saved successfully");
                }
            }

            catch (Exception e)
            {
                _log.Error("Failed to save brain to storage: {0}", e);
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
            _log.Debug($"Saving new value to brain for Key={key}");
            _brain.SetItem(key, value);

            return _persistenceEnabled
                ? PersistenceState.Persisted
                : PersistenceState.InMemoryOnly;
        }

        public void Dispose()
        {
            _brainChangedEventSubscription?.Dispose();
        }
    }
}