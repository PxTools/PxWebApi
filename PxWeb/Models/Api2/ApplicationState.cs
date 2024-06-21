namespace PxWeb.Models.Api2
{
    public interface IApplicationState
    {
        public bool InMaintanceMode { get; set; }
    }

    public class ApplicationState : IApplicationState
    {
        public bool InMaintanceMode { get; set; } = false;
    }

}
