using System.Threading.Tasks;

namespace BrassLoon.CommonCore
{
    public class SaveSettings : TransactionHandler, ISaveSettings
    {
        private readonly ISettings _settings;

        public SaveSettings(ISettings settings)
            : base(settings)
        {
            _settings = settings;
        }

        public Task<string> GetDatabaseName() => _settings.GetDatabaseName();
    }
}
