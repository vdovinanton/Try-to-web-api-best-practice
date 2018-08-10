using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace WebApplicationExercise.Utils
{
    public class Settings
    {
        private Settings() { }

        #region Singleton
        private static readonly Lazy<Settings> _instance = new Lazy<Settings>(() => new Settings());
        public static Settings Instance => _instance.Value;
        #endregion

        public string CustomerName => ConfigurationManager.AppSettings["CustomerBanned"];

        public string CurrenctCurrency => ConfigurationManager.AppSettings["CurrentCurrency"];
    }
}