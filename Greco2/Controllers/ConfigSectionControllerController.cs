using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Greco2.Controllers
{
    public class ConfigSectionControllerController : Controller
    {
        public class ConfigurationSectionController : ConfigurationSection
        {
            #region variables

            public string LDAPServer { get; set; }
            public string LDAPPath { get; set; }
            public string LDAPPort { get; set; }
            public string LDAPUser { get; set; }
            public string LDAPPassword { get; set; }
            public string LDAPMinPwsLength { get; set; }

            public string LDAPUsersDefaultAttrs { get; set; }
            public string LDAPUsersLdapFilter { get; set; }
            public string LDAPUsersRolesAttribute { get; set; }
            public string LDAPUsersNamingAttribute { get; set; }
            public string LDAPUsersBaseDN { get; set; }
            public string LDAPRolesBaseDN { get; set; }
            public string LDAPRolesLDAPFilter { get; set; }
            public string LDAPRolesAppAttribute { get; set; }
            public string LDAPRolesSysAttribute { get; set; }
            public string LDAPRolesDefaultAttrs { get; set; }
            public string LDAPRolesDescAttribute { get; set; }
            public string LDAPRolesNamingAttribute { get; set; }

            #endregion

            #region Constructores

            public ConfigurationSectionController()
            {
                var secureAppSettings = ConfigurationManager.GetSection("secureAppSettings") as NameValueCollection;
                LDAPServer = secureAppSettings["LDAPServer"];
                LDAPPath = secureAppSettings["LDAPPath"];
                LDAPPort = secureAppSettings["LDAPPort"];
                LDAPUser = secureAppSettings["LDAPUser"];
                LDAPPassword = secureAppSettings["LDAPPassword"];
                LDAPUsersDefaultAttrs = secureAppSettings["LDAPUsersDefaultAttrs"];
                LDAPUsersLdapFilter = secureAppSettings["LDAPUsersLdapFilter"];
                LDAPUsersRolesAttribute = secureAppSettings["LDAPUsersRolesAttribute"];
                LDAPUsersNamingAttribute = secureAppSettings["LDAPUsersNamingAttribute"];
                LDAPUsersBaseDN = secureAppSettings["LDAPUsersBaseDN"];
                LDAPRolesBaseDN = secureAppSettings["LDAPRolesBaseDN"];
                LDAPRolesLDAPFilter = secureAppSettings["LDAPRolesLDAPFilter"];
                LDAPRolesAppAttribute = secureAppSettings["LDAPRolesAppAttribute"];
                LDAPRolesSysAttribute = secureAppSettings["LDAPRolesSysAttribute"];
                LDAPRolesDefaultAttrs = secureAppSettings["LDAPRolesDefaultAttrs"];
                LDAPRolesDescAttribute = secureAppSettings["LDAPRolesDescAttribute"];
                LDAPRolesNamingAttribute = secureAppSettings["LDAPRolesNamingAttribute"];
                //LDAPServer = ConfigurationManager.AppSettings.Get("LDAPServer");
                //LDAPPath = ConfigurationManager.AppSettings.Get("LDAPPath");
                //LDAPPort = ConfigurationManager.AppSettings.Get("LDAPPort");
                //LDAPUser = ConfigurationManager.AppSettings.Get("LDAPUser");
                //LDAPPassword = ConfigurationManager.AppSettings.Get("LDAPPassword");
                //LDAPUsersDefaultAttrs = ConfigurationManager.AppSettings.Get("LDAPUsersDefaultAttrs");
                //LDAPUsersLdapFilter = ConfigurationManager.AppSettings.Get("LDAPUsersLdapFilter");
                //LDAPUsersRolesAttribute = ConfigurationManager.AppSettings.Get("LDAPUsersRolesAttribute");
                //LDAPUsersNamingAttribute = ConfigurationManager.AppSettings.Get("LDAPUsersNamingAttribute");
                //LDAPUsersBaseDN = ConfigurationManager.AppSettings.Get("LDAPUsersBaseDN");
                //LDAPRolesBaseDN = ConfigurationManager.AppSettings.Get("LDAPRolesBaseDN");
                //LDAPRolesLDAPFilter = ConfigurationManager.AppSettings.Get("LDAPRolesLDAPFilter");
                //LDAPRolesAppAttribute = ConfigurationManager.AppSettings.Get("LDAPRolesAppAttribute");
                //LDAPRolesSysAttribute = ConfigurationManager.AppSettings.Get("LDAPRolesSysAttribute");
                //LDAPRolesDefaultAttrs = ConfigurationManager.AppSettings.Get("LDAPRolesDefaultAttrs");
                //LDAPRolesDescAttribute = ConfigurationManager.AppSettings.Get("LDAPRolesDescAttribute");
                //LDAPRolesNamingAttribute = ConfigurationManager.AppSettings.Get("LDAPRolesNamingAttribute");
            }

            #endregion
        }

    }
}