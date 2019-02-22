using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;

namespace CAS.myUtilities
{
    public sealed class MyConfig
    {
        private static volatile MyConfig instance;
        private static readonly object syncRoot = new object();

        readonly string configFile = System.Reflection.Assembly.GetExecutingAssembly().Location;

        //Constructor
        public MyConfig() { }

        //Properties

        //Methods
        public static MyConfig Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new MyConfig();
                            
                        }
                    }
                }
                return instance;
            }
        }

        public enum Key
        {
            Basislayer = 1,
            Block = 2,
            Outputfile = 3
        }

        public string GetAppSettingString(string key)
        {
            string value;
           
            //Zurückgeben der dem Key zugehörigen Value
            try
            {
                //Laden der AppSettings
                Configuration config = ConfigurationManager.OpenExeConfiguration(configFile);

                value = config.AppSettings.Settings[key].Value;
            }
            catch
            {
                value = String.Empty;
            }

            return value;
        }

        public bool GetAppSettingBool(string key)
        {
            bool value;

            //Zurückgeben der dem Key zugehörigen Value
            try
            {
                //Laden der AppSettings
                Configuration config = ConfigurationManager.OpenExeConfiguration(configFile);

                value = Convert.ToBoolean(config.AppSettings.Settings[key].Value);
            }
            catch
            {
                value = false;
            }

            return value;
        }

        public int GetAppSettingInt(string key)
        {
            int value;

            //Zurückgeben der dem Key zugehörigen Value
            try
            {
                //Laden der AppSettings
                Configuration config = ConfigurationManager.OpenExeConfiguration(configFile);

                value = Convert.ToInt32(config.AppSettings.Settings[key].Value);
            }
            catch
            {
                value = 0;
            }

            return value;
        }

        public void SetAppSetting(string key, string value)
        {
            //Laden der AppSettings
            Configuration config = ConfigurationManager.OpenExeConfiguration(configFile);
            //Überprüfen ob Key existiert
            if (config.AppSettings.Settings[key] != null)
            {
                //Key existiert. Löschen des Keys zum "überschreiben"
                config.AppSettings.Settings.Remove(key);
            }
            //Anlegen eines neuen KeyValue-Paars
            config.AppSettings.Settings.Add(key, value);
            //Speichern der aktualisierten AppSettings
            config.Save(ConfigurationSaveMode.Modified);
        }
    }
}
