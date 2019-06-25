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
using Tweetinvi.Parameters;
using System.Net;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;

namespace TwitterDiscordBot
{
    public static class TwitterService
    {
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

        public static async Task<string> PostMessage(ulong userID, string message, string attatchmentURL = null)
        {
            if (!Credentials.Keys.Contains(userID))
            {
                return "You need to link your account first.";
            }
            else if (attatchmentURL != null)
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.DownloadFile(attatchmentURL, @"image" + userID);
                }

                byte[] tempImage = File.ReadAllBytes(@"image" + userID);
                var attatchment = Upload.UploadBinary(tempImage);

                PublishTweetOptionalParameters parameters = new PublishTweetOptionalParameters();
                parameters.Medias = new List<IMedia> { attatchment };
                File.Delete(@"image" + userID);
                return $"Tweeted! {Credentials[userID].PublishTweet(message, parameters).Url}";
            }
            else
            {
                return $"Tweeted! {Credentials[userID].PublishTweet(message).Url}";
            }
        }

        public static async Task<string> GetFeed(ulong userID, int amount)
        {
            if (!Credentials.Keys.Contains(userID))
            {
                return "You need to link your account first.";
            }
            IEnumerable<ITweet> _feed = await Credentials[userID].GetHomeTimelineAsync(amount);
            string feed = "";

            foreach (ITweet post in _feed)
            {
                feed += $"**{post.CreatedBy.UserDTO.Name}:** {post.Text}\n";
            }

            return feed;
        }

        public static async Task<string> Follow(ulong userID, string username)
        {
            try
            {
                if (!Credentials.Keys.Contains(userID))
                {
                    return "You need to link your account first.";

                }

                var user = Credentials[userID];

                var relationshipDetails = user.GetRelationshipWith(username);

                if (relationshipDetails.Following == true)
                {
                    return "You are already following this account.";
                }
                else
                {
                    await user.FollowUserAsync(username);
                    return $"Successfully followed {username}";
                }
            }
            catch (System.NullReferenceException)
            {
                return "Invalid username";
            }
            
        }
    }

}
