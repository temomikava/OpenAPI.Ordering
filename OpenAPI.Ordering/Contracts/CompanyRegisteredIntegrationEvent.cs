using SharedKernel;

namespace IntegrationEvents
{
    public class CompanyRegisteredIntegrationEvent : BaseIntegrationEvent
    {
        public CompanyRegisteredIntegrationEvent(string name, string apikey, string apiSecret)
        {
            Name = name;
            ApiKey = apikey;
            ApiSecret = apiSecret;
        }
        public string Name { get; set; }
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
    }
}
