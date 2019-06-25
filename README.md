# TwitterCord-Discord-Bot
A Discord bot made for Discord Hack Week that allows Discord Users to post Tweets and view their feed from Discord.

You can add this bot to your server with this link: https://discordapp.com/api/oauth2/authorize?client_id=592698765538885632&permissions=67584&scope=bot

# Commands
**!ping** -- Pings the bot.

**!login** -- Logs a Twitter user into the bot.

**!pin** -- Retreives the Twitter authentication PIN to complete the login process. Do not run this command in a public channel, as it puts your Twitter account at risk. Parameters: <Authenication PIN>

**!tweet** -- Tweets something to Twitter. You must first link your twitter account. Supports most image, video, and GIF attatchments. Parameters: <Message>

**!feed** -- Displays the last 7 posts on your timeline. Parameters: <Number of Tweets>

**!follow** -- Follows another user. Parameters: <Username>

# Notes
* Do not expect consistant uptime for the bot. I will be away for a bit and won't be able to keep the bot running.
* This bot wasn't extensively tested, and as such may have bugs. There are a couple of known bugs regarding tweets, but they can usually be fixed by using the command again.
