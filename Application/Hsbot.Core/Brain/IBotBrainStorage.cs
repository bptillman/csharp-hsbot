using System.Threading.Tasks;

namespace Hsbot.Core.Brain
{
    public interface IBotBrainStorage<T>
        where T : IBotBrain
    {
        Task<T> Load();
        Task Save(T brain);
    }
}