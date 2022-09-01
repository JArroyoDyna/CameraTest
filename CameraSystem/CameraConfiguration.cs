using System;
using System.Configuration;
using System.Reflection;

namespace CameraSystem
{
    public class CameraConfiguration
    {
        public static void setConfigValue(string setting, string value)
        {
            Assembly cAssembly = Assembly.GetExecutingAssembly();
            string exePath = cAssembly.ManifestModule.Assembly.Location;
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(exePath);
                config.AppSettings.Settings.Remove(setting);
                config.AppSettings.Settings.Add(setting, value);
                config.Save(ConfigurationSaveMode.Modified);
            }
            catch (Exception ex)
            {
                //Logging.WriteException(ex);
                //throw;
            }
        }
        public static string getConfigValue(string setting)
        {
            Assembly cAssembly = Assembly.GetExecutingAssembly();
            string exePath = cAssembly.ManifestModule.Assembly.Location;
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(exePath);
                return config.AppSettings.Settings[setting].Value;
            }
            catch (Exception ex)
            {
                //Logging.WriteException(ex);
                return "";
                //throw;
            }
        }
    }
}
