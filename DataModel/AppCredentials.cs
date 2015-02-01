using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
using System.Threading.Tasks;

using Windows.Storage;
using Windows.Security.Credentials;

using GitMe.Common.OctokitWrapper;

namespace GitMe.Data
{
    class PasswordHelper
    {
        public abstract class InitializePasswordVault
        {
            public static void Initialize()
            {
                Task.Factory.StartNew(() => { InitializePasswordVault.LoadPasswordVault(); });
            }

            private static void LoadPasswordVault()
            {
                // any call to the password vault will load the vault
                PasswordVault vault = new PasswordVault();
                vault.RetrieveAll();
            }
        }
    }

    public interface IAppCredentials
    {
        string Login { get; set; }
        string Password { get; set; }
        void RemoveCredentials(string login);
        Task<bool> IsValid();
    }

    public sealed class AppCredentials : IAppCredentials
    {
        private static readonly AppCredentials _appCredentials = new AppCredentials();
        //private static object syncRoot = new Object();

        private static readonly ApplicationDataContainer _roamingSettings = ApplicationData.Current.RoamingSettings;
        private static readonly PasswordVault _vault = InitializeVault();

        private static string _login = "";
        private static string _password = "";

        private AppCredentials() { }



        // TODO http://msdn.microsoft.com/en-us/library/ff650316.aspx
        public static AppCredentials Builder
        {
            get
            {
                _appCredentials.Login = InitializeLogin();
                if (_appCredentials.Login != "")
                {
                    PasswordCredential cred = _vault.Retrieve("gitme", _appCredentials.Login);
                    if (cred != null)
                    {
                        _appCredentials.Password = cred.Password;
                    }
                    else
                    {
                        _appCredentials.Password = "";
                    }
                }
                else
                {
                    _appCredentials.Password = "";
                }
                return _appCredentials;
            }
        }

        private static PasswordVault InitializeVault()
        {
            PasswordHelper.InitializePasswordVault.Initialize();
            return new PasswordVault();
        }

        private static string InitializeLogin()
        {
            object __login;
            if (_roamingSettings.Values.TryGetValue("login", out __login))
                return (string)__login;
            else
                return "";
        }

        public async Task<bool> IsValid()
        {
            //if (_login != "" && _password != "")
            try 
            {
                var cred = new GitHubWrapper(_appCredentials.Login, _appCredentials.Password);
                return await cred.TestCredentials();
            }
            catch
            {
                return false;
            }
        }
        
        public string Login { get { return _login; } set { _roamingSettings.Values["login"] = value; _login = value; } }
        
        public string Password 
        {
            get { return _password; } 
            set 
            {
                if (_login == null || _login == "" || value == null || value == "")
                {
                    _login = "";
                    _password = "";
                }
                else
                {
                    PasswordCredential cred = new PasswordCredential()
                    {
                        Resource = "gitme",
                        UserName = _login,
                        Password = value
                    };
                    _vault.Add(cred);
                    _password = value;
                }    
            } 
        }

        public void RemoveCredentials(string login)
        {
            PasswordCredential cred = _vault.Retrieve("gitme", login);
            _vault.Remove(cred);
            _roamingSettings.Values["login"] = "";
            _appCredentials.Login = "";
            _appCredentials.Password = "";
        }

    }
}
