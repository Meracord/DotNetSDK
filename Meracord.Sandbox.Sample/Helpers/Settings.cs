using System.Configuration;

namespace Meracord.Sandbox.Helpers
{
    public class Settings
    {
        public static string BaseServiceAddress { get { return ConfigurationManager.AppSettings["BaseServiceAddress"]; }}
        public static string UserId { get { return ConfigurationManager.AppSettings["UserId"]; }}
        public static string Password { get { return ConfigurationManager.AppSettings["Password"]; }}
        public static string GroupNumber { get { return ConfigurationManager.AppSettings["GroupNumber"]; }}
        public static string DocumentPath { get { return ConfigurationManager.AppSettings["DocumentPath"]; }}
    }
}
