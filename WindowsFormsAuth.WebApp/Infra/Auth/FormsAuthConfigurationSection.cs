using System.Configuration;

namespace WindowsFormsAuth.WebApp.Infra.Auth
{
    public class FormsAuthConfigurationSection : ConfigurationSection
    {
        public const string ConfigurationSectionName = "mixedFormsWindowsAuth";
        private static readonly ConfigurationPropertyCollection properties;
        private static readonly ConfigurationProperty propFormsAuthenticationEnabled = new ConfigurationProperty("formsAuthenticationEnabled", typeof(bool), true);
        private static readonly ConfigurationProperty propWindowsAuthenticationEnabled = new ConfigurationProperty("windowsAuthenticationEnabled", typeof(bool), true);

        static FormsAuthConfigurationSection()
        {
            properties = new ConfigurationPropertyCollection { propFormsAuthenticationEnabled, propWindowsAuthenticationEnabled };
        }

        protected override ConfigurationPropertyCollection Properties { get { return properties; } }

        [ConfigurationProperty("formsAuthenticationEnabled", DefaultValue = true)]
        public bool FormsAuthenticationEnabled
        {
            get { return (bool)base[propFormsAuthenticationEnabled]; }
            set { base[propFormsAuthenticationEnabled] = value; }
        }

        [ConfigurationProperty("windowsAuthenticationEnabled", DefaultValue = true)]
        public bool WindowsAuthenticationEnabled
        { 
            get { return (bool)base[propWindowsAuthenticationEnabled]; }
            set { base[propWindowsAuthenticationEnabled] = value; }
        }

    }
}
