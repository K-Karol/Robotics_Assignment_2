namespace KarolK72.LegoAssignment.Library
{
    public interface IEV3CommunicationService : IDisposable
    {
        Task Connect(string url, int port);
        Task Disconnect();
    }
}