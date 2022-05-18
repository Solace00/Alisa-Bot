using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BotUpdate.Modules
{
    public class Fun : ModuleBase<SocketCommandContext>
    {
        private readonly ILogger<Fun> _logger;

        public Fun(ILogger<Fun> logger)
            => _logger = logger;

        [Command("meme")]
        [Alias("reddit")]
        public async Task MemeAsync(string subreddit = null)
        {
            var client = new HttpClient();
            var result = await client.GetStringAsync($"https://reddit.com/r/{subreddit ?? "Animemes"}/random.json?limit=1");
            if (!result.StartsWith("["))
            {
                await ReplyAsync("This Subreddit doesn't exist!");
                return;
            }
            JArray arr = JArray.Parse(result);
            JObject post = JObject.Parse(arr[0]["data"]["children"][0]["data"].ToString());

            var builder = new EmbedBuilder()
                .WithImageUrl(post["url"].ToString())
                .WithColor(new Color(255, 215, 0))
                .WithTitle(post["title"].ToString())
                .WithUrl("https://reddit.com" + post["permalink"].ToString())
                .WithFooter($"🗨 {post["num_comments"]} ⬆️ {post["ups"]}");

            var embed = builder.Build();
            await ReplyAsync(null, false, embed);
        }
    }
}
