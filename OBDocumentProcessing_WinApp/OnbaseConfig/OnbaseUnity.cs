using Hyland.Unity;
using OBDocumentProcessing_WinApp.Model;
using OBDocumentProcessing_WinApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OBDocumentProcessing_WinApp.OnbaseConfig
{
    public static class OnbaseUnity
    {
        private static UnityConfig _unityConfig;
        private static Application OBApp;
        public static Application _OBApp
        {
            get { return OBApp; }
            set { OBApp = value; }
        }


        public static string erro;

        public static async Task<OnbaseUser> AuthOnbase(LoginViewModel login)
        {
            _unityConfig = new UnityConfig();
            DomainAuthenticationProperties onbaseProp = Hyland.Unity.Application.CreateDomainAuthenticationProperties(_unityConfig.Service, _unityConfig.DataSource);
            try
            {
                onbaseProp.Username = login.Matricula;
                onbaseProp.Password = login.Senha;
                onbaseProp.Domain = "IMG_PETROS";
                onbaseProp.IsDisconnectEnabled = false;
                OBApp = Hyland.Unity.Application.Connect(onbaseProp);

                return new OnbaseUser
                {
                    RealName = OBApp.CurrentUser.RealName,
                    Username = OBApp.CurrentUser.Name,
                    Email = OBApp.CurrentUser.EmailAddress,
                    SessionID = OBApp.SessionID,
                    UserGroupList = OBApp.CurrentUser.GetUserGroups()
                };
            }
            catch (InvalidLoginException ex)
            {
                throw new Exception("The credentials entered are invalid." + ex.Message.ToString());
            }
            catch (UserAccountLockedException ex)
            {
                throw new Exception("The user account is locked." + ex.Message.ToString());
            }
            catch (AuthenticationFailedException ex)
            {
                throw new Exception("Authentication failed." + ex.Message.ToString());
            }
            catch (MaxConcurrentLicensesException ex)
            {
                throw new Exception("All licenses are currently in use, please try again later." + ex.Message.ToString());
            }
            catch (NamedLicenseNotAvailableException ex)
            {
                throw new Exception("Your license is not availble, please insure you are logged out of other OnBase clients." + ex.Message.ToString());
            }
            catch (SystemLockedOutException ex)
            {
                throw new Exception("The system is currently locked, please try back later." + ex.Message.ToString());
            }
            catch (UnityAPIException ex)
            {
                throw new Exception("There was an unhandled exception with the Unity API." + ex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("There was an unhandled exception." + ex.Message.ToString());
            }
        }

        public static bool connectOnbaseSessionID(string sessionID)
        {
            try
            {
                SessionIDAuthenticationProperties onbaseProp = Hyland.Unity.Application.CreateSessionIDAuthenticationProperties(_unityConfig.Service, sessionID, false);
                OBApp = Hyland.Unity.Application.Connect(onbaseProp);

                return true;
            }
            catch (InvalidLoginException ex)
            {
                erro = "The credentials entered are invalid." + ex.StackTrace.ToString();
                return false;
            }
            catch (UserAccountLockedException ex)
            {
                erro = "The user account is locked." + ex.StackTrace.ToString();
                return false;
            }
            catch (AuthenticationFailedException ex)
            {
                erro = "Authentication failed." + ex.StackTrace.ToString();
                return false;
            }
            catch (MaxConcurrentLicensesException ex)
            {
                erro = "All licenses are currently in use, please try again later." + ex.StackTrace.ToString();
                return false;
            }
            //catch (NamedLicenseNotAvailableException ex)
            //{
            //    erro = "Your license is not availble, please insure you are logged out of other OnBase clients."  + ex.StackTrace.ToString();
            //    return false;
            //}
            catch (SystemLockedOutException ex)
            {
                erro = "The system is currently locked, please try back later." + ex.StackTrace.ToString();
                return false;
            }
            catch (UnityAPIException ex)
            {
                erro = "There was an unhandled exception with the Unity API." + ex.StackTrace.ToString();
                return false;
            }
            catch (Exception ex)
            {
                erro = "There was an unhandled exception." + ex.StackTrace.ToString();
                return false;
            }

        }

        public static void disconnectOnbase()
        {
            try
            {
                OBApp.Disconnect();
            }
            catch (UnityAPIException uae)
            {
                throw new Exception("Unity API Exception Occured: " + uae.Message);

            }
            catch (Exception ex)
            {
                throw new Exception("Exception Occured: " + ex.Message);
            }
        }

        public static bool IsConnected()
        {
            if (OBApp != null)
                return OBApp.IsConnected;
            else
                return false;
        }

        

        /// <summary>
        /// Criação de palavras chaves para usar em processos externos a classe OnbaseUnity
        /// </summary>
        /// <param name="keywordName"></param>
        /// <param name="keywordValue"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static Keyword CreateKeyword_Ext(string keywordName, string keywordValue)
        {
            try
            {
                KeywordType keytype = OBApp.Core.KeywordTypes.Find(keywordName);
                if (keytype != null)
                {
                    Keyword keyword = CreateKeyword(keytype, keywordValue);
                    return keyword;
                }
                else
                {
                    throw new Exception($"Keyword: {keywordName} não localizada.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro: {ex.Message}");
            }
        }

        public static Keyword CreateKeyword(KeywordType Keytype, string Value)
        {
            Keyword key = null;
            switch (Keytype.DataType)
            {
                case KeywordDataType.Currency:
                case KeywordDataType.Numeric20:
                    decimal decVal = decimal.Parse(Value);
                    key = Keytype.CreateKeyword(decVal);
                    break;
                case KeywordDataType.Date:
                case KeywordDataType.DateTime:
                    DateTime dateVal = DateTime.Parse(Value);
                    key = Keytype.CreateKeyword(dateVal);
                    break;
                case KeywordDataType.FloatingPoint:
                    double dblVal = double.Parse(Value);
                    key = Keytype.CreateKeyword(dblVal);
                    break;
                case KeywordDataType.Numeric9:
                    long lngVal = long.Parse(Value);
                    key = Keytype.CreateKeyword(lngVal);
                    break;
                default:
                    key = Keytype.CreateKeyword(Value);
                    break;
            }
            return key;
        }

    }
}
