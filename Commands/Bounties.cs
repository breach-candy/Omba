using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Text.Json;

public class Bounties : BaseCommandModule
{
    [Command("setbounty")]
    [Description("Set a bounty on a player. I.E. ?setbounty Pelley, 500000000, Must be in corvette")]
    public async Task setbounty(CommandContext ctx, string player, string bounty, [RemainingText] string extra)
    {
        
    }
}