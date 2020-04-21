using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hsbot.Core.Brain;

namespace Hsbot.Core.BotServices
{
    public sealed class HsbotBrainService : IBotBrain, IBotService
    {
        private readonly IBotBrainStorage<HsbotBrain> _botBrainStorage;
        private readonly IHsbotLog _log;
        private HsbotBrain _brain;

        public int StartupOrder => 0;

        public HsbotBrainService(IBotBrainStorage<HsbotBrain> botBrainStorage, IHsbotLog log)
        {
            _botBrainStorage = botBrainStorage;
            _log = log;
        }

        public async Task Start(BotServiceContext context)
        {
            _log.Info("Initializing brain");
            if (_brain != null)
            {
                _log.Info("Brain already initialized, skipping");
                return;
            }

            try
            {
                _brain = await _botBrainStorage.Load();
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
            _log.Debug("Received brain change event - saving to storage");
            try
            {
                await _botBrainStorage.Save(brain);
                _log.Debug("Received brain change event - brain saved successfully");
            }

            catch (Exception e)
            {
                _log.Error("Failed to save brain to storage: {0}", e);
            }
        }

        public async Task Stop()
        {
            _log.Info("Saving brain to storage");
            await _botBrainStorage.Save(_brain);
        }

        public ICollection<string> Keys => _brain.Keys;

        public T GetItem<T>(string key) where T : class
        {
            return _brain.GetItem<T>(key);
        }

        public void SetItem<T>(string key, T value) where T : class
        {
            _brain.SetItem(key, value);
        }
    }
}