using System;
using System.Reflection;
using System.Security.Principal;
using System.Web;
using System.Web.Configuration;

namespace WindowsFormsAuth.WebApp.Infra.Auth
{
    public class MixedFormsWindowsAuthModule : IHttpModule
    {
        public const string EnableFormsAuthServerVariableName = "FormsAuth_Enable";
        private System.Web.Security.FormsAuthenticationModule originalFormsAuthenticationModule;
        private bool formsAuthenticationEnabled;
        private MethodInfo originalFormsAuthenticationModuleOnEnter;
        private MethodInfo originalFormsAuthenticationModuleOnLeave;
        private FormsAuthConfigurationSection config;

        public void Init(HttpApplication app)
        {
            originalFormsAuthenticationModule = new System.Web.Security.FormsAuthenticationModule();
            var t = originalFormsAuthenticationModule.GetType();
            originalFormsAuthenticationModuleOnEnter = t.GetMethod("OnEnter", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(Object), typeof(EventArgs) }, null);
            originalFormsAuthenticationModuleOnLeave = t.GetMethod("OnLeave", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(Object), typeof(EventArgs) }, null);
            if (originalFormsAuthenticationModuleOnEnter == null || originalFormsAuthenticationModuleOnLeave == null)
                throw new Exception("Unable to get all required FormsAuthenticationModule entrypoints using reflection.");
            app.AuthenticateRequest += (source, e) =>
            {
                formsAuthenticationEnabled = false;
                var context = ((HttpApplication)source).Context;
                config = WebConfigurationManager.GetSection(FormsAuthConfigurationSection.ConfigurationSectionName, context.Request.Path) as FormsAuthConfigurationSection;
                if (config == null || !config.FormsAuthenticationEnabled)
                    return;
                if (!IsFormsAuthEnabled(context))
                    return;
                formsAuthenticationEnabled = true;
                originalFormsAuthenticationModuleOnEnter.Invoke(originalFormsAuthenticationModule, new [] { source, e });
            };
            app.PostAuthenticateRequest += (source, e) =>
            {
                var context = ((HttpApplication)source).Context;
                if (!formsAuthenticationEnabled && context.User == null && config != null && config.WindowsAuthenticationEnabled)
                {
                    var iisIdentity = context.Request.LogonUserIdentity;
                    if (iisIdentity != null)
                        context.User = iisIdentity.IsAnonymous ? new WindowsPrincipal(WindowsIdentity.GetAnonymous()) : new WindowsPrincipal(iisIdentity);
                }
            };
            app.EndRequest += (source, e) =>
            {
                if (!formsAuthenticationEnabled)
                    return;
                originalFormsAuthenticationModuleOnLeave.Invoke(originalFormsAuthenticationModule, new [] { source, e });
            };
        }

        public static bool IsFormsAuthEnabled(HttpContext context)
        {
            var enabledSV = context.Request.ServerVariables[EnableFormsAuthServerVariableName];
            return string.IsNullOrWhiteSpace(enabledSV) || enabledSV.Equals("true", StringComparison.InvariantCultureIgnoreCase);
        }

        public void Dispose()
        {
            if (originalFormsAuthenticationModule != null)
                originalFormsAuthenticationModule.Dispose();                
        }
    }
}
