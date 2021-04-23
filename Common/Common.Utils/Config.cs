using System;

namespace Common.Utils
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

        public static IAppLogger Logger { get; private set; }
        public static Func<string, string> GetAppSettings { get; private set; }
        public static EnvironmentType Environment { get; private set; }
    }
}
