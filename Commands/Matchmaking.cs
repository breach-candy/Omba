using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Text.Json;
using System.Text.Json.Serialization;

public class Matchmaking : BaseCommandModule
{
    List<string> queue = new List<string>();

    [Command("setname")]
    [Description("set nickname")]
    [requireChannel(747801198312030310)]
    public async Task setName(CommandContext ctx, [RemainingText] string nickname)
    {
        var nicknames = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText("Nicknames.json"));
        if (nicknames.ContainsValue(nickname))
            await ctx.RespondAsync($"The name of `{nickname}` already exists!");
        else
        {
            try
            {
                string oldNick = "0";
                if (nicknames.ContainsKey(ctx.Message.Author.Mention))
                {
                    oldNick = nicknames[ctx.Message.Author.Mention];
                    nicknames.Remove(ctx.Message.Author.Mention);
                }
                nicknames.Add(ctx.Message.Author.Mention, nickname);
                File.WriteAllText("Nicknames.json", JsonSerializer.Serialize<Dictionary<string, string>>(nicknames));
                await ctx.RespondAsync($"Nickname set for `{ctx.Message.Author.Username}`: **{nickname}**");
                if (queue.Contains(ctx.Message.Author.Mention))
                {
                    queue.Remove(ctx.Message.Author.Mention);
                    queue.Add(nicknames[ctx.Message.Author.Mention]);
                }
                else if (queue.Contains(oldNick))
                {
                    queue.Remove(oldNick);
                    queue.Add(nicknames[ctx.Message.Author.Mention]);
                }
            }
            catch (System.Exception ex)
            {
                await ctx.RespondAsync($"nigga smth broke {ex}");
            }

        }
    }

    public string switcheroo(string name)
    {
        var nicknames = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText("Nicknames.json"));
        if (nicknames.ContainsKey(name))
            return nicknames[name];
        else
            return name;
    }

    public bool register(string name)
    {
        if (queue.Contains(switcheroo(name)))
            return true;
        else
        {
            queue.Add(switcheroo(name));
            return false;
        }
    }

    public bool unregister(string name)
    {
        if (queue.Contains(switcheroo(name)))
        {
            queue.Remove(switcheroo(name));
            return true;
        }
        else
            return false;
    }

    //Register command
    [Command("reg"), Aliases("r", "register")]
    [Description("please type ?reg in the matchmaking channel")]
    [requireChannel(747801198312030310)]
    public async Task reg(CommandContext ctx)
    {
        if (register(ctx.Message.Author.Mention))
            await ctx.RespondAsync($"You are already in the queue. `{queue.Count}` in queue.");
        else
            await ctx.RespondAsync($"{switcheroo(ctx.Message.Author.Mention)} added to queue. `{queue.Count}` in queue.");
    }

    //Force register
    [Command("freg"), Aliases("fr")]
    [Description("force register")]
    [requireChannel(747801198312030310)]
    public async Task freg(CommandContext ctx, string name)
    {
        if (register(name))
            await ctx.RespondAsync($"{name} is already in the queue. `{queue.Count}` in queue.");
        else
            await ctx.RespondAsync($"{ctx.Message.Author.Username} added `{name}` to queue. `{queue.Count}` in queue.");
    }

    //clear
    [Command("clear"), Aliases("c")]
    [Description("queue clear")]
    [requireChannel(747801198312030310)]
    public async Task clear(CommandContext ctx)
    {
        queue.Clear();
        await ctx.RespondAsync($"{ctx.Message.Author.Username} cleared the queue.");
    }

    //unreg
    [Command("ur"), Aliases("unreg", "unregister")]
    [Description("unregister")]
    [requireChannel(747801198312030310)]
    public async Task unreg(CommandContext ctx)
    {
        if (unregister(ctx.Message.Author.Mention))
            await ctx.RespondAsync($"Removed from queue. `{queue.Count}` in queue.");
        else
            await ctx.RespondAsync("You are not in the queue.");
    }

    //Remove force reg
    [Command("rreg"), Aliases("rfreg", "rf")]
    [Description("remove reg")]
    [requireChannel(747801198312030310)]
    public async Task rfreg(CommandContext ctx, string name)
    {
        if (unregister(name))
            await ctx.RespondAsync($"Removed from queue. `{queue.Count}` in queue.");
        else
            await ctx.RespondAsync($"{name} is not in the queue.");
    }

    //roll / reroll
    [Command("roll"), Aliases("rr", "reroll")]
    [Description("please type ?reg in the matchmaking channel")]
    [requireChannel(747801198312030310)]
    public async Task roll(CommandContext ctx)
    {
        List<string> team1 = new List<string>();
        List<string> team2 = new List<string>();
        Random random = new Random();
        if (queue.Count == 0)
        {
            await ctx.RespondAsync("There is no one in the queue.");
        }
        else
        {
            List<string> teams = queue.ToList();
            for (int i = teams.Count; i > 0; i--)
            {
                var randomIndex = random.Next(teams.Count);

                if (i % 2 != 0)
                {
                    team1.Add(teams[randomIndex]);
                }
                else
                {
                    team2.Add(teams[randomIndex]);
                }
                teams.RemoveAt(randomIndex);
            }
            string theDate = DateTime.Now.ToString("HH:mm:ss");
            var embed = new DiscordEmbedBuilder()
                .WithTitle($"{team1.Count()}v{team2.Count()} prepared")
                .WithColor(new DiscordColor(0xFF55FF))
                .WithThumbnail("https://i.imgur.com/neLGbRS.png")
                .AddField($"Team 1", $"{string.Join("\n", team1)}", true)
                .AddField($"Team 2", $"{string.Join("\n", team2)}", true);
            await ctx.RespondAsync("", embed: embed);
            team1.Clear(); team2.Clear();
        }

    }

    //queue
    [Command("q"), Aliases("queue")]
    [Description("display queue")]
    [requireChannel(747801198312030310)]
    public async Task displayQueue(CommandContext ctx)
    {
        if (queue.Count == 0)
            await ctx.RespondAsync("There is no one in the queue.");
        else
        {
            var embed = new DiscordEmbedBuilder
            {
                Title = $"{queue.Count()} in queue",
                Description = $"{string.Join(", ", queue)}",
                Color = new DiscordColor(0xFF55FF)
            };
            await ctx.RespondAsync("", embed: embed);
        }
    }
}
