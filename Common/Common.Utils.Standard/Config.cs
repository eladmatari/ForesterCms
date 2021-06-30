using Common.Utils.Standard;
using System;

namespace Common.Utils.Standard
{
    public class Config
    {
        public static void Init(Func<string, string> getAppSettings, IAppLogger logger)
        {
            if (getAppSettings == null)
                throw new ArgumentNullException(nameof(getAppSettings));

            GetAppSettings = getAppSettings;

            EnvironmentType environment;
            Enum.TryParse(GetAppSettings("Envirinment"), out environment);
            Environment = environment;
            Logger = logger;
        }

        public static readonly DateTime CreateDate = DateTime.Now;
        public static IAppLogger Logger { get; private set; }
        public static Func<string, string> GetAppSettings { get; private set; }
        public static EnvironmentType Environment { get; private set; }
        public static string MailServer { get { return GetAppSettings("MailServer"); } }

        public static bool IsProxy { get { return GetAppSettings("IsProxy") == "1"; } }
    }
}
