namespace Hsbot.Core.Connection
{
    public interface IChatUser
    {
        string Id { get; }
        string Email { get; }
        string DisplayName { get; }
        string FullName { get; }
        bool IsEmployee { get; }
    }
}
