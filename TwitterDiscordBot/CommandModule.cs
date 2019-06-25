using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

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
        [Alias("post")]
        [Summary("Tweets something to Twitter. You must first link your twitter account. Supports .png and .jpg image attatchments.")]
        public async Task TweetAsync([Remainder] string message)
        {
            if (message.Length > 280)
            {
                await ReplyAsync("Tweets must be under 280 characters in length.");
            }
            else
            {
                string attatchmentURL = null;
                if (Context.Message.Attachments.Any() == true)
                {
                    IUserMessage _message = Context.Message;
                    var attachment = _message.Attachments.First();

                    attatchmentURL = attachment.Url;
                }
                await ReplyAsync(await TwitterService.PostMessage(Context.Message.Author.Id, message, attatchmentURL));
            }
        }

        [Command("login")]
        [Summary("Logs a Twitter user into the bot.")]
        public async Task LoginAsync()
        {
            string URL = await TwitterService.GetAuthURL(Context.Message.Author.Id);
            IDMChannel DMChannel = await Context.Message.Author.GetOrCreateDMChannelAsync();
            await DMChannel.SendMessageAsync($"Go to {URL} and follow the instructions on the wbesite and get the PIN number. Once you are done, reply to this message with the command \"!pin [PIN NUMBER]\". Do not run the command in a public channel, as that could allow someone else to control your Twitter Account.");
        }

        [Command("pin")]
        [Summary("Retreives the Twitter authentication PIN to complete the login process. Do not run this command in a public channel, as it puts your Twitter account at risk.")]
        public async Task PinAsync(string PIN)
        {
            await ReplyAsync(await TwitterService.Authenticate(PIN, Context.Message.Author.Id));
        }

        [Command("feed")]
        [Alias("timeline")]
        [Summary("Displays the last 7 posts on your timeline.")]
        public async Task FeedAsync(int NumberOfPosts)
        {
            if(NumberOfPosts <= 0 || NumberOfPosts > 8)
            {
                await ReplyAsync("The number of posts you choose must be in between 1 and 8, inclusive.");
            }
            else
            {
                await ReplyAsync(await TwitterService.GetFeed(Context.Message.Author.Id, NumberOfPosts));
            }
        }

        [Command("follow")]
        [Summary("Follows another user")]
        public async Task FollowAsync(string username)
        {
            await ReplyAsync(await TwitterService.Follow(Context.Message.Author.Id, username));
        }

        [Command("help")]
        [Summary("Displays all available commands")]
        public async Task HelpAsync()
        {
            await ReplyAsync("**!ping** -- Pings the bot.\n**!login** -- Logs a Twitter user into the bot.\n**!pin** -- Retreives the Twitter authentication PIN to complete the login process. Do not run this command in a public channel, as it puts your Twitter account at risk. Parameters: <Authenication PIN>\n**!tweet** -- Tweets something to Twitter. You must first link your twitter account. Supports .png and .jpg image attatchments. Parameters: <Message>\n**!feed** -- Displays the last 7 posts on your timeline. Parameters: <Number of Tweets>");
        }
    }
}
