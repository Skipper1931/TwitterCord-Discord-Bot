using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
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
        [Alias("post")]
        [Summary("Tweets something to Twitter. You must first link your twitter account.")]
        public async Task TweetAsync([Remainder] string message)
        {
            await ReplyAsync($"Tweeted! {await TwitterService.PostMessage(Context.Message.Author.Id, message)}");
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
    }
}
