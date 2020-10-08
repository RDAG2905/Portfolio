using Greco2.Model;
using Greco2.Models.Log;
using Identicum.Directory.Ldap;

using System;
using System.Collections.Generic;
using System.DirectoryServices;

using System.Linq;
using System.Web;
using System.Web.Mvc;
using static Greco2.Controllers.ConfigSectionControllerController;

namespace Greco2.Controllers
{
    //public class MembershipControllerController : Controller
    //{

    //}

    public class ADMembership
    {
        public ConfigurationSectionController Config = new ConfigurationSectionController();

        public object AuthenticationTypes { get; private set; }

        public bool LoginUser(string user, string pwd)
        {
            bool result = false;
            string data = "";
            try
            {
                LdapServer ldapServer = new LdapServer(SetDataConfiguration());
                result = ldapServer.AuthenticateUser(user, pwd);
            }
            catch (Exception ex)
            {
                data = ex.Message;
                using (NuevoDbContext db = new NuevoDbContext())
                {
                    var log = new LogErrorDto();
                    log.Fecha = DateTime.Now;
                    log.Error = data;
                    db.Add(log);
                    db.SaveChanges();
                }
            }
            return result;
        }

        public List<string> GetUserRoles(string user, string aplication, string system)
        {
            string error = "";
            List<string> listResult = new List<string>();
            try
            {
                string app = "*";
                string sys = system;
                LdapServer ldapServer = new LdapServer(SetDataConfiguration());
                listResult = ldapServer.GetUserRoles(user, app, sys);

                //var itenes = "No trajo ningún DnRol";
                //foreach (var item in listResult) {
                //    itenes = item + " <--> "; 
                //}
                //using (NuevoDbContext db = new NuevoDbContext())
                //{
                //    var log = new LogErrorDto();
                //    log.Fecha = DateTime.Now;
                //    log.Error = itenes;
                //    log.UserId = user;
                //    //log.ErrorDetallado = itenes;
                //    db.Add(log);
                //    db.SaveChanges();
                //}

            }
            catch (Exception ex)
            {
                error = ex.Message;
                using (NuevoDbContext db = new NuevoDbContext())
                {
                    var log = new LogErrorDto();
                    log.Fecha = DateTime.Now;
                    log.Error = error;
                    db.Add(log);
                    db.SaveChanges();
                }
            }
            return listResult;
        }

        public Dictionary<string, string> SetDataConfiguration()
        {
            Dictionary<string, string> ldapConfiguration = new Dictionary<string, string>();
            ldapConfiguration.Add(LdapConfiguration.LDAP_SERVER, Config.LDAPServer.ToString());
            ldapConfiguration.Add(LdapConfiguration.LDAP_PORT, Config.LDAPPort.ToString()); //puerto seguro 636 o 389
            ldapConfiguration.Add(LdapConfiguration.SERVICE_ACCOUNTDN, Config.LDAPUser.ToString());
            ldapConfiguration.Add(LdapConfiguration.SERVICE_ACCOUNTPWD, Config.LDAPPassword.ToString());
            ldapConfiguration.Add(LdapConfiguration.USERS_DEFAULTATTRS, Config.LDAPUsersDefaultAttrs.ToString());
            ldapConfiguration.Add(LdapConfiguration.USERS_LDAP_FILTER, Config.LDAPUsersLdapFilter.ToString());
            ldapConfiguration.Add(LdapConfiguration.USERS_ROLES_ATTRIBUTE, Config.LDAPUsersRolesAttribute.ToString());
            ldapConfiguration.Add(LdapConfiguration.USERS_NAMING_ATRIBUTE, Config.LDAPUsersNamingAttribute.ToString());
            ldapConfiguration.Add(LdapConfiguration.USERS_BASEDN, Config.LDAPUsersBaseDN.ToString());
            ldapConfiguration.Add(LdapConfiguration.ROLES_BASEDN, Config.LDAPRolesBaseDN.ToString());
            ldapConfiguration.Add(LdapConfiguration.ROLES_LDAP_FILTER, Config.LDAPRolesLDAPFilter.ToString());
            ldapConfiguration.Add(LdapConfiguration.ROLES_APP_ATTRIBUTE, Config.LDAPRolesAppAttribute.ToString());
            ldapConfiguration.Add(LdapConfiguration.ROLES_SYS_ATTRIBUTE, Config.LDAPRolesSysAttribute.ToString());
            ldapConfiguration.Add(LdapConfiguration.ROLES_DEFAULTATTRS, Config.LDAPRolesDefaultAttrs.ToString());
            ldapConfiguration.Add(LdapConfiguration.ROLES_DESC_ATTRIBUTE, Config.LDAPRolesDescAttribute.ToString());
            ldapConfiguration.Add(LdapConfiguration.ROLES_NAMING_ATTRIBUTE, Config.LDAPRolesNamingAttribute.ToString());

            return ldapConfiguration;
        }

        // No compilan los tipos de autenticación de esta linea
        //System.DirectoryServices.AuthenticationTypes AuthTypes = AuthenticationTypes.Signing | AuthenticationTypes.Sealing | AuthenticationTypes.Secure;

        //public string SetPassword(string distinguishedName, string pw)
        //{
        //    const long ADS_OPTION_PASSWORD_PORTNUMBER = 6;
        //    const long ADS_OPTION_PASSWORD_METHOD = 7;
        //    const int ADS_PASSWORD_ENCODE_REQUIRE_SSL = 0;
        //    const int ADS_PASSWORD_ENCODE_CLEAR = 1;
        //    string strPort = "389";
        //    int intPort = Int32.Parse(strPort);
        //    System.DirectoryServices.AuthenticationTypes AuthTypes = AuthenticationTypes.Signing | AuthenticationTypes.Sealing | AuthenticationTypes.Secure;
        //    DirectoryEntry objUser = new DirectoryEntry("LDAP://" + distinguishedName, null, null, AuthTypes);
        //    try
        //    {
        //        objUser.Invoke("SetOption", new object[] { ADS_OPTION_PASSWORD_PORTNUMBER, intPort });
        //        objUser.Invoke("SetOption", new object[] { ADS_OPTION_PASSWORD_METHOD, ADS_PASSWORD_ENCODE_CLEAR });
        //        objUser.Invoke("SetPassword", new object[] { pw });
        //    }
        //    catch (Exception e)
        //    {
        //        return e.Message;
        //    }
        //    return "true";
        //}


        //public DirectoryEntry GetUserDetails(string user, string currentPws)
        //{
        //    string strUser = "DESA" + @"\" + user;
        //    try
        //    {
        //        AuthenticationTypes AuthTypes = AuthenticationTypes.Signing | AuthenticationTypes.Sealing | AuthenticationTypes.Secure;
        //        DirectoryEntry entry = new DirectoryEntry("LDAP://" + Config.LDAPServer, user, currentPws, AuthTypes);
        //        DirectorySearcher search = new DirectorySearcher(entry);
        //        string strFilter = "(SAMAccountName=" + user + ")";

        //        search.Filter = strFilter;
        //        SearchResult result = search.FindOne();
        //        DirectoryEntry objUser = result.GetDirectoryEntry();
        //        return objUser;
        //    }
        //    catch (Exception exception)
        //    {
        //        return null;
        //    }

        //}

        //public bool ChangePassword(string user, string currentPwd, string newPwd)
        //{
        //    const long ADS_OPTION_PASSWORD_PORTNUMBER = 6;
        //    const long ADS_OPTION_PASSWORD_METHOD = 7;
        //    const int ADS_PASSWORD_ENCODE_REQUIRE_SSL = 0;
        //    const int ADS_PASSWORD_ENCODE_CLEAR = 1;
        //    bool result = true;
        //    string strPort = Config.LDAPPort;
        //    int intPort = Int32.Parse(strPort);
        //    DirectoryEntry objUser;
        //    string strUser = "DESA" + @"\" + user;
        //    try
        //    {
        //        objUser = GetUserDetails(user, currentPwd);
        //        objUser.Invoke("SetOption", new object[] { ADS_OPTION_PASSWORD_PORTNUMBER, intPort });
        //        objUser.Invoke("SetOption", new object[] { ADS_OPTION_PASSWORD_METHOD, ADS_PASSWORD_ENCODE_CLEAR });
        //        objUser.Invoke("ChangePassword", new object[] { currentPwd, newPwd });
        //        objUser.CommitChanges();

        //        result = true;
        //    }
        //    catch (Exception exception)
        //    {
        //        result = false;
        //    }
        //    return result;
        //}
    }

}