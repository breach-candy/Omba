using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class requireChannelAttribute : CheckBaseAttribute
{
    public string AllowedChannel { get; private set; }

    public requireChannelAttribute(string Id)
    {
        AllowedChannel = Id;
    }

    public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
    {
        return Task.FromResult(AllowedChannel == ctx.Channel.Name);
    }
}

