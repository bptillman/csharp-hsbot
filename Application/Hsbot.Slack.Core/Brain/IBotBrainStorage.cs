using System.Threading.Tasks;

namespace Hsbot.Slack.Core.Brain
{
    public interface IBotBrainStorage<T>
        where T : IBotBrain
    {
        Task<T> Load();
        Task Save(T brain);
    }
}