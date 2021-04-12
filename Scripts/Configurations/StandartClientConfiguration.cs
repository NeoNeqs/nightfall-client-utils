using Godot;

using SharedUtils.Configurations;

namespace ClientsUtils.Configurations
{
    public abstract class StandartClientConfiguration<T> : StandartConfiguration<T> where T : Node
    {
        public string GetIpAddress(string defaultIpAddress)
        {
            return GetValue<string>("NETWORKING", "ip_address", defaultIpAddress);
        }
    }
}