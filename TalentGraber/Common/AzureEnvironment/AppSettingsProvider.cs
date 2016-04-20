using Microsoft.Azure;

namespace Macrodeek.MacrodeekCommon
{
    public class AppSettingsProvider
    {
        public static string GetSetting(string Name)
        {
            string connectionString = CloudConfigurationManager.GetSetting(Name);

            return connectionString;
        }
    }
}