using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace WebApplicationExercise.Utils
{
    public class Settings
    {
        #region Singleton
        private static readonly Lazy<Settings> _instance = new Lazy<Settings>(() => new Settings());
        public static Settings Instance => _instance.Value;
        #endregion

        private Settings() { }

        public string CustomerName => ConfigurationManager.AppSettings["CustomerException"];
    }
}