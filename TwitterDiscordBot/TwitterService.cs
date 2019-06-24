using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Credentials;
using TwitterDiscordBot.Properties;
using Tweetinvi.Models;
using System.Net.Http;
using System.Security.Cryptography;
using System.Diagnostics;


namespace TwitterDiscordBot
{
    public static class TwitterService
    {


        private static readonly HttpClient client = new HttpClient();

        private const string callbackURL = "http://www.skipper1931.com/oauthcallback";

        private static Dictionary<ulong, IAuthenticatedUser> Credentials = new Dictionary<ulong, IAuthenticatedUser>();
        // Discord User ID, Twitter Account Credentials

        private static Dictionary<ulong, IAuthenticationContext> AuthenticationStates = new Dictionary<ulong, IAuthenticationContext>();
        // Discord User ID, Twitter Authentication State

        public static async Task<string> GetAuthURL(ulong userID)
        {
            var appCredentials = new TwitterCredentials(Resources.API_key, Resources.API_secret_key);
            var authenticationContext = AuthFlow.InitAuthentication(appCredentials);

            AuthenticationStates[userID] = authenticationContext;

            return authenticationContext.AuthorizationURL;
        }

        public static async Task<string> Authenticate(string pinCode, ulong userID)
        {
            if (!AuthenticationStates.Keys.Contains(userID))
            {
                return "You must first run \"!login\" before running this command.";
            }
            else if (AuthenticationStates[userID] == null)
            {
                return "Your account has already been linked. If you'd like to link another one, start with the \"!login\" command.";
            }
            else
            {
                var _creds = AuthFlow.CreateCredentialsFromVerifierCode(pinCode, AuthenticationStates[userID]);
                AuthenticationStates[userID] = null;
                var userCredentials = Auth.CreateCredentials(Resources.API_key, Resources.API_secret_key, _creds.AccessToken, _creds.AccessTokenSecret);
                Credentials[userID] = User.GetAuthenticatedUser(userCredentials);
                return $"Twitter Account {Credentials[userID].UserDTO.ScreenName} linked!";
            }
        }

        public static async Task<string> PostMessage(ulong userID, string message)
        {
            return Credentials[userID].PublishTweet(message).Url;
        }

    }

}
