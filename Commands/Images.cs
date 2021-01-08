using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Text.Json;

public class Images : BaseCommandModule
{
    [Command("image"), Aliases("img", "i", "gif", "meme")]
    public async Task displayImage(CommandContext ctx, [RemainingText] string key)
    {
        var images = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText("Images.json"));
        if (images.ContainsKey(key))
        {
            await ctx.RespondAsync(images[key]);
        }
    }

    [Command("addimage"), Aliases("addimg", "addmeme")]
    [RequireUserPermissions(DSharpPlus.Permissions.Administrator)]
    public async Task addImage(CommandContext ctx, string key, [RemainingText] string value)
    {
        var images = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText("Images.json"));
        if (!images.ContainsKey(key))
        {
            images.Add(key, value);
            File.WriteAllText("Images.json", JsonSerializer.Serialize<Dictionary<string, string>>(images));
        }
        else
        {
            var embed = new DiscordEmbedBuilder
            {
                Title = "Key exists",
                Description = $":no_entry: This key already exists, try something else.",
                Color = new DiscordColor(0xFF0000) // red
            };
            await ctx.RespondAsync("", embed: embed);
        }
    }
}