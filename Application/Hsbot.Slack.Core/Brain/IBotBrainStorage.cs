namespace Hsbot.Slack.Core.Brain
{
    public interface IBotBrainStorage<T>
        where T : IBotBrain
    {
        T Load(string address);
        void Save(string address, T brain);
    }
}