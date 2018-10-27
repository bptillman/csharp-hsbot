namespace Hsbot.Core.Brain
{
    public interface IBotBrainSerializer<T>
        where T: IBotBrain
    {
        T Deserialize(string serializedBrain);
        string Serialize(T brain);
    }
}