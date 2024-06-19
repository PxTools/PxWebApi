namespace PxWeb.Models.Api2
{
    public interface IApplicationState
    {
        public bool MarkedForShutdown { get; set; }
    }

    public class ApplicationState : IApplicationState
    {
        public bool MarkedForShutdown { get; set; } = false;
    }

}
