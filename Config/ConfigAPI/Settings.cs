using BrassLoon.CommonAPI;

namespace ConfigAPI
{
    public class Settings : CommonApiSettings
    {
        public bool UseMongoDb { get; }
        public string DatabaseName { get; }
    }
}
