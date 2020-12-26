using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class requireChannelAttribute : CheckBaseAttribute
{
    public ulong AllowedChannel { get; private set; }

    public requireChannelAttribute(ulong Id)
    {
        AllowedChannel = Id;
    }

    public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
    {
        return Task.FromResult(AllowedChannel == ctx.Channel.Id);
    }
} 

