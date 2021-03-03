using SharedUtils.Scripts.Configurations;

namespace ClientsUtils.Scripts.Configurations
{
    public abstract class StandartClientConfiguration : StandartConfiguration
    {
        public string GetIpAddress(string defaultIpAddress)
        {
            return GetValue<string>("NETWORKING", "ip_address", defaultIpAddress);
        }
    }
}