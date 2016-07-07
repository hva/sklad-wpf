using Warehouse.Wpf.Infrastructure.Interfaces;

namespace Warehouse.Wpf.Infrastructure
{
    public class ApplicationSettings : IApplicationSettings
    {
        public ApplicationSettings(string baseAddress)
        {
            Endpoint = baseAddress;
        }

        public string Endpoint { get; }
    }
}
