using BotUpdate.Utilities;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotUpdate.Modules
{
    public class Basic : ModuleBase<SocketCommandContext>
    {
        private readonly ILogger<Basic> _logger;
        private readonly Images _images;

        public Basic(ILogger<Basic> logger, Images image)
        {
            _logger = logger;
            _images = image;
        }

        [Command("ping")]
        public async Task PingAsync()
        {
            await ReplyAsync("Pong!");
        }

        [Command("echo")]
        public async Task EchoAsync([Remainder] string text)
        {
            await ReplyAsync(text);
        }

        [Command("math")]
        public async Task MathAsync([Remainder] string math)
        {
            var dt = new DataTable();
            var result = dt.Compute(math, null);

            await ReplyAsync($"Result: {result}");
        }

        [Command("info")]
        public async Task InfoAsync(SocketGuildUser user = null)
        {
            if (user == null)
            {
                var builder = new EmbedBuilder()
                    .WithThumbnailUrl(Context.User.GetAvatarUrl() ?? Context.User.GetDefaultAvatarUrl())
                    .WithDescription(Context.User.Mention)
                    .WithColor(new Color(255, 215, 0))
                    .AddField("User_ID:", Context.User.Id, true)
                    .AddField("Joined:", (Context.User as SocketGuildUser).JoinedAt.Value.ToString("dd/MM/yyyy"), true)
                    .AddField("Registered:", Context.User.CreatedAt.ToString("dd/MM/yyyy"), true)
                    .AddField("Roles:", string.Join(" ", (Context.User as SocketGuildUser).Roles.Select(x => x.Mention)))
                    .WithCurrentTimestamp();

                var embed = builder.Build();
                await ReplyAsync(null, false, embed);
            }
            else
            {
                var builder = new EmbedBuilder()
                    .WithThumbnailUrl(user.GetAvatarUrl() ?? Context.User.GetDefaultAvatarUrl())
                    .WithDescription(user.Mention)
                    .WithColor(new Color(255, 215, 0))
                    .AddField("User_ID:", user.Id, true)
                    .AddField("Joined:", user.JoinedAt.Value.ToString("dd/MM/yyyy"), true)
                    .AddField("Registered:", user.CreatedAt.ToString("dd/MM/yyyy"), true)
                    .AddField("Roles:", string.Join(" ", user.Roles.Select(x => x.Mention)))
                    .WithCurrentTimestamp();

                var embed = builder.Build();
                await ReplyAsync(null, false, embed);
            }
        }

        [Command("server")]
        public async Task ServerAsync()
        {
            var builder = new EmbedBuilder()
                .WithThumbnailUrl(Context.Guild.IconUrl)
                .WithTitle($"{Context.Guild.Name} Information")
                .WithColor(new Color(255, 215, 0))
                .AddField("Owner:", (Context.Guild as SocketGuild).Owner, true)
                .AddField("Channel Categories:", (Context.Guild as SocketGuild).CategoryChannels, true)   //FIX
                .AddField("Text Channels:", (Context.Guild as SocketGuild).TextChannels, true)            //FIX
                .AddField("Voice Channels:", (Context.Guild as SocketGuild).VoiceChannels, true)          //FIX
                .AddField("Members:", (Context.Guild as SocketGuild).MemberCount, true)
                .AddField("Roles:", (Context.Guild as SocketGuild).Roles, true);                          //FIX
            //    .WithFooter("ID:", Context.Guild.Id, true)
            //    .WithFooter("Created on:", Context.Guild.CreatedAt.ToString("dd/MM/yyyy"));

            var embed = builder.Build();
            await ReplyAsync(null, false, embed);
        }

        [Command("image", RunMode = RunMode.Async)]
        public async Task ImageAsync()
        {
            string path = await _images.CreateImageAsync(Context.User as SocketGuildUser);
            await Context.Channel.SendFileAsync(path);
            File.Delete(path);
        }
    }
}
