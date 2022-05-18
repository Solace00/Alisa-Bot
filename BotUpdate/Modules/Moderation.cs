using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotUpdate.Modules
{
    public class Moderation : ModuleBase<SocketCommandContext>
    {
        private readonly ILogger<Moderation> _logger;

        public Moderation(ILogger<Moderation> logger)
            => _logger = logger;

        [Command("purge")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task PurgeAsync(int amount)
        {
            var message = await Context.Channel.GetMessagesAsync(amount + 1).FlattenAsync();
            await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(message);

            var messages = await ReplyAsync($"{message.Count()} messages deleted from chat!");
            await Task.Delay(3000);
            await messages.DeleteAsync();

            _logger.LogInformation($"{Context.User.Username} executed the purge commmand!");
        }
    }
}
