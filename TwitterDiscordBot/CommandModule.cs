using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;

namespace TwitterDiscordBot
{
    public class CommandModule : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        [Summary("Pings the bot.")]
        public async Task PingAsync()
        {
            await ReplyAsync($"<@{Context.User.Id}> pong!");
        }

        [Command("tweet")]
        [Summary("Tweets something.")]
        public async Task TweetAsync([Remainder] string message)
        {
            TwitterService.PostMessage(message);
        }

        [Command("login")]
        [Summary("Logs a twitter user into the bot.")]
        public async Task LoginAsync(string username)
        {
            string returnUsername;

            if (username.StartsWith("@"))
            {
                returnUsername = "";

                int place = 0;
                foreach (char character in username)
                {
                    if (place != 0)
                    {
                        returnUsername += character;
                    }
                    place++;
                }
            }
            else
            {
                returnUsername = username;
            }
        }
    }
}
