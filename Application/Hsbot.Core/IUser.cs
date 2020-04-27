namespace Hsbot.Core
{
    public interface IUser
    {
        string Id { get; }
        string Email { get; }
        string FullName { get; }
        bool IsEmployee { get; }
    }
}
