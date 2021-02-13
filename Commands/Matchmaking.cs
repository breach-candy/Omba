using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Text.Json;

public class Matchmaking : BaseCommandModule
{
    List<string> queue = new List<string>();

    public bool nameset(string name, [RemainingText] string nickname)
    {
        var nicknames = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText("Nicknames.json"));
        if (nicknames.ContainsValue(nickname))
            return (true);
        else
        {
            string oldNick = "0";
            if (nicknames.ContainsKey(name))
            {
                oldNick = nicknames[name];
                nicknames.Remove(name);
            }
            nicknames.Add(name, nickname);
            File.WriteAllText("Nicknames.json", JsonSerializer.Serialize<Dictionary<string, string>>(nicknames));
            if (queue.Contains(name))
            {
                queue.Remove(name);
                queue.Add(nicknames[name]);
            }
            else if (queue.Contains(oldNick))
            {
                queue.Remove(oldNick);
                queue.Add(nicknames[name]);
            }
            return (false);
        }
    }

    [Command("setname")]
    [Description("Set your nickname.")]
    [requireChannel("the-matchmaking-channel")]
    public async Task setName(CommandContext ctx, [RemainingText] string nickname)
    {
        if (nameset(ctx.Message.Author.Mention, nickname))
        {
            await ctx.RespondAsync($"The name of `{nickname}` already exists!");
        }
        else if (nameset(ctx.Message.Author.Mention, nickname))
        {
            await ctx.RespondAsync($"Nickname set for `{ctx.Message.Author.Username}`: **{nickname}**");
        }
    }
    [Command("setnamefor")]
    [Description("Set another users name. Admin only.")]
    [RequireUserPermissions(DSharpPlus.Permissions.Administrator)]
    [requireChannel("the-matchmaking-channel")]
    public async Task setNameFor(CommandContext ctx, string name, [RemainingText] string nickname)
    {
        if (nameset(name, nickname))
        {
            await ctx.RespondAsync($"The name of `{nickname}` already exists!");
        }
        else if (nameset(name, nickname))
        {
            await ctx.RespondAsync($"Nickname set for `{name}`: **{nickname}**");
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

    [Command("register"), Aliases("r", "reg")]
    [Description("Please type ?reg in the matchmaking channel.")]
    [requireChannel("the-matchmaking-channel")]
    public async Task reg(CommandContext ctx)
    {
        if (register(ctx.Message.Author.Mention))
            await ctx.RespondAsync($"You are already in the queue. `{queue.Count}` in queue.");
        else
            await ctx.RespondAsync($"{switcheroo(ctx.Message.Author.Mention)} added to queue. `{queue.Count}` in queue.");
    }

    [Command("forcereg"), Aliases("fr", "freg")]
    [Description("Force register another user.")]
    [requireChannel("the-matchmaking-channel")]
    public async Task freg(CommandContext ctx, [RemainingText] string name)
    {
        if (register(name))
            await ctx.RespondAsync($"{name} is already in the queue. `{queue.Count}` in queue.");
        else
            await ctx.RespondAsync($"{ctx.Message.Author.Username} added `{name}` to queue. `{queue.Count}` in queue.");
    }

    [Command("clear"), Aliases("c")]
    [Description("Clear the queue.")]
    [requireChannel("the-matchmaking-channel")]
    public async Task clear(CommandContext ctx)
    {
        queue.Clear();
        await ctx.RespondAsync($"{ctx.Message.Author.Username} cleared the queue.");
    }

    [Command("unregister"), Aliases("unreg", "ur")]
    [Description("Unregister yourself from the queue.")]
    [requireChannel("the-matchmaking-channel")]
    public async Task unreg(CommandContext ctx)
    {
        if (unregister(ctx.Message.Author.Mention))
            await ctx.RespondAsync($"Removed from queue. `{queue.Count}` in queue.");
        else
            await ctx.RespondAsync("You are not in the queue.");
    }

    [Command("removereg"), Aliases("rfreg", "rf", "rreg")]
    [Description("Unregister another user from the queue.")]
    [requireChannel("the-matchmaking-channel")]
    public async Task rfreg(CommandContext ctx, [RemainingText] string name)
    {
        if (unregister(name))
            await ctx.RespondAsync($"Removed from queue. `{queue.Count}` in queue.");
        else
            await ctx.RespondAsync($"{name} is not in the queue.");
    }

    [Command("roll"), Aliases("rr", "reroll")]
    [Description("Roll a fight.")]
    [requireChannel("the-matchmaking-channel")]
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

    [Command("queue"), Aliases("q", "show")]
    [Description("Display the queue.")]
    [requireChannel("the-matchmaking-channel")]
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
