using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Credentials;
using TwitterDiscordBot.Properties;
using Tweetinvi.Models;

namespace TwitterDiscordBot
{
    public static class TwitterService
    {
        private class AccessPair
        {
            public readonly string accessToken;
            public readonly string accessSecrert;
        }

        private static Dictionary<string,AccessPair> Credentials = new Dictionary<string,AccessPair>();
        // Discord Username, Token/Secret Pair

        public static void PostMessage(string discordUsername, string message)
        {
            AccessPair _credentials = Credentials[discordUsername];

            var userCredentials = Auth.CreateCredentials(Resources.API_key, Resources.API_secret_key, _credentials.accessToken, _credentials.accessSecrert);
            var user = User.GetAuthenticatedUser(userCredentials);

            var tweet = user.PublishTweet(message);
        }


    }

}
