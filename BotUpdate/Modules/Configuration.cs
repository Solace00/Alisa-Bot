using BotUpdate.Utilities;
using Discord;
using Discord.Commands;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotUpdate.Modules
{
    public class Configuration : ModuleBase<SocketCommandContext>
    {
        private readonly RanksHelper _rankshelper;
        private readonly AutoRolesHelper _autoRolesHelper;
        private readonly Servers _servers;
        private readonly Ranks _ranks;
        private readonly AutoRoles _autoRoles;

        public Configuration(RanksHelper rankshelper, Servers servers, Ranks ranks, AutoRolesHelper autoRolesHelper, AutoRoles autoRoles)
        {
            _rankshelper = rankshelper;
            _servers = servers;
            _ranks = ranks;
            _autoRolesHelper = autoRolesHelper;
            _autoRoles = autoRoles;
        }

        [Command("prefix", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Prefix(string prefix = null)
        {
            if (prefix == null)
            {
                var guildprefix = await _servers.GetGuildPrefix(Context.Guild.Id) ?? "!";
                await ReplyAsync($"The Current prefix of this bot is '{guildprefix}'.");
                return;
            }


            if (prefix.Length > 8)
            {
                await ReplyAsync("The length of the new prefix is too long!");
                return;
            }

            await _servers.ModifyGuildPrefex(Context.Guild.Id, prefix);
            await ReplyAsync($"The prefix has been updated to `{prefix}`.");
        }

        [Command("ranks", RunMode = RunMode.Async)]
        public async Task Rank()
        {
            var ranks = await _rankshelper.GetRanksAsync(Context.Guild);
            if(ranks.Count == 0)
            {
                await ReplyAsync("this server does not have any ranks!"); //can add embeds
                return;
            }

            await Context.Channel.TriggerTypingAsync();

            string description = "This message lists all available ranks.";
            foreach (var rank in ranks)
            {
                description += $"\n{rank.Mention} ({rank.Id})"; //Remove Rank ID if you wan
            }

            await ReplyAsync(description);
        }

        [Command("addrank", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task AddRank([Remainder]string name)
        {
            await Context.Channel.TriggerTypingAsync();
            var ranks = await _rankshelper.GetRanksAsync(Context.Guild);

            var role = Context.Guild.Roles.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.CurrentCultureIgnoreCase));
            if (role == null)
            {
                await ReplyAsync("That role does not exist!");
                return;
            }

            if (role.Position > Context.Guild.CurrentUser.Hierarchy)
            {
                await ReplyAsync("That role has a higher position than the bot!");
                return;
            }

            if (ranks.Any(x => x.Id == role.Id))
            {
                await ReplyAsync("That role is already a rank!");
                return;
            }

            await _ranks.AddRankAsync(Context.Guild.Id, role.Id);
            await ReplyAsync($"The role {role.Mention} has been added to the ranks!");
        }

        //AUTO ROLES

        [Command("autoroles", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task AddAutoRoles()
        {
            var autoRoles = await _autoRolesHelper.GetAutoRolesAsync(Context.Guild);
            if (autoRoles.Count == 0)
            {
                await ReplyAsync("this server does not have any autoroles!"); //can add embeds
                return;
            }

            await Context.Channel.TriggerTypingAsync();

            string description = "This message lists all available autoRoles.";
            foreach (var autoRole in autoRoles)
            {
                description += $"\n{autoRole.Mention} ({autoRole.Id})"; //Remove Rank ID if you wan
            }

            await ReplyAsync(description);
        }

        [Command("addautoroles", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task AddAutoRole([Remainder] string name)
        {
            await Context.Channel.TriggerTypingAsync();
            var autoRoles = await _autoRolesHelper.GetAutoRolesAsync(Context.Guild);

            var role = Context.Guild.Roles.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.CurrentCultureIgnoreCase));
            if (role == null)
            {
                await ReplyAsync("That role does not exist!");
                return;
            }

            if (role.Position > Context.Guild.CurrentUser.Hierarchy)
            {
                await ReplyAsync("That role has a higher position than the bot!");
                return;
            }

            if (autoRoles.Any(x => x.Id == role.Id))
            {
                await ReplyAsync("That role is already an autorank!");
                return;
            }

            await _autoRoles.AddAutoRoleAsync(Context.Guild.Id, role.Id);
            await ReplyAsync($"The role {role.Mention} has been added to the autoRoles!");
        }
    }
}
