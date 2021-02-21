using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class requireChannelAttribute : CheckBaseAttribute
{
    public string allowedChannel { get; private set; }

    public requireChannelAttribute(string name)
    {
        allowedChannel = name;
    }

    public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
    {
        return Task.FromResult(allowedChannel == ctx.Channel.Name);
    }
}

