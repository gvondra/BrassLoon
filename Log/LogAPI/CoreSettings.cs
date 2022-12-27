using BrassLoon.CommonCore;

namespace LogAPI
{
    public class CoreSettings : BrassLoon.CommonAPI.CoreSettings, ISettings
    {
        public CoreSettings(Settings settings)
            : base(settings)
        { }

    }
}
