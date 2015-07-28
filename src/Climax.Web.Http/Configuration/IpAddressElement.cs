using System.Configuration;

namespace Climax.Web.Http.Configuration
{
    public class IpAddressElement : ConfigurationElement
    {
        [ConfigurationProperty("address", IsKey = true, IsRequired = true)]
        public string Address
        {
            get { return (string)this["address"]; }
            set { this["address"] = value; }
        }

        [ConfigurationProperty("denied", IsRequired = false)]
        public bool Denied
        {
            get { return (bool)this["denied"]; }
            set { this["denied"] = value; }
        }
    }
}