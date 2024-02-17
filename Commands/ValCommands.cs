using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace Ker_NelBot.Commands
{
    public struct mapDetails
    {
        public string mapName;
        public string mapComp;
    }

    public class ValCommands : BaseCommandModule
    {
        [Command("MapComp")]
        [Description("Gets the current map team comp.")]
        public async Task MapComp(CommandContext ctx, string mapName)
        {
            List<mapDetails> mapList = new List<mapDetails>();
            mapList.Add(new mapDetails()
            {
                mapName = "Ascent",
                mapComp =
                    "\nController: Brimstone\nSentinel: Killjoy\nDuelist: Reyna\nInitiator: Sova\nInitiator: Kay/O"
            });
            mapList.Add(new mapDetails()
            {
                mapName = "Bind",
                mapComp = "\nController: Brimstone\nDuelist: Raze\nSentinel: Sage\nInitiator: Skye\nController: Astra"
            });
            mapList.Add(new mapDetails()
            {
                mapName = "Breeze",
                mapComp = "\nController: Viper\nDuelist: Jett\nSentinel: Chamber\nInitiator:Sova\nInitiator: Kay/O"
            });
            mapList.Add(new mapDetails()
            {
                mapName = "Fracture",
                mapComp =
                    "\nSentinel: Chamber\nController: Brimstone\nDuelist: Raze\nInitiator: Fade\nInitiator: Breach"
            });
            mapList.Add(new mapDetails()
            {
                mapName = "Haven",
                mapComp = "\nSentinel: Killjoy\nInitiator: Skye\nDuelist: Jett\nController: Omen\nInitiator: Breach"
            });
            mapList.Add(new mapDetails()
            {
                mapName = "Icebox",
                mapComp = "\nSentinel: Killjoy\nController: Viper\nSentinel: Sage\nSentinel: Chamber\nInitiator: Sova"
            });
            mapList.Add(new mapDetails()
            {
                mapName = "Lotus",
                mapComp = "\nDuelist: Jett\nInitiator: Skye\nController: Viper\nController: Harbor\nSentinel: Killjoy"
            });
            mapList.Add(new mapDetails()
            {
                mapName = "Pearl",
                mapComp = "\nDuelist: Neon\nController: Astra\nInitiator: Fade\nInitiator: Kay/O\nSentinel: Cypher"
            });
            mapList.Add(new mapDetails()
            {
                mapName = "Split",
                mapComp = "\nDuelist: Raze\nDuelist: Jett\nController: Omen\nSentinel: Cypher\nSentinel: Sage"
            });
            foreach (mapDetails map in mapList)
            {
                if (map.mapName == mapName || map.mapName.ToLower() == mapName.ToLower())
                {
                    await ctx.Channel.SendMessageAsync("Based off functionality and winning percentages:")
                        .ConfigureAwait(false);
                    await ctx.Channel.SendMessageAsync(map.mapComp).ConfigureAwait(false);
                }
            }
        }

        [Command("roulette")]
        [Description("Gets random fun challenge(s).")]
        public async Task FunChallenges(CommandContext ctx, [Description("The amount of challenges you want to generate")]int challAmt)
        {
            List<string> challengeList = new List<string>();
            challengeList.Add("No abilities");
            challengeList.Add("No guns");
            challengeList.Add("No shields");
            challengeList.Add("No movement (after round start or after planting/defusing)");
            challengeList.Add("Only running");
            challengeList.Add("No running");
            challengeList.Add("Scoping only");
            challengeList.Add("No reloading (you can only reload when you run out of ammo)");
            challengeList.Add("No defusing");
            challengeList.Add("No planting");
            challengeList.Add("You can only walk backwards");
            challengeList.Add("You can only walk forwards");
            challengeList.Add("You can only walk left");
            challengeList.Add("You can only walk right");
            challengeList.Add("Everyone has to set the master volume to 0");
            challengeList.Add("Everyone has to set the master volume to 100");
            challengeList.Add("Sheriff only, no knifing");
            challengeList.Add("Everyone must 'cast' their gameplay. If you die, 'cast' the person you are watching");
            challengeList.Add(
                "Each team can only send one person out of spawn at a time, the rest must stay in spawn until the person who left dies");
            challengeList.Add("You can only use your abilities to kill");
            challengeList.Add("Every time you kill someone, you must swap your gun with theirs");
            challengeList.Add("Shotguns only");
            challengeList.Add(
                "You are only allowed to kill enemies through walls, you can see where people are; To kill them, it must be through a wall");
            challengeList.Add("Every person must buy the most expensive gun they can afford and you cant buy armor");
            challengeList.Add("Every person must put their headset on backwards");
            challengeList.Add("Everyone on defense must go to one site");
            challengeList.Add("Everyone on offense must go to one site");
            challengeList.Add("Only the person on the bottom of the leaderboard can comm");
            challengeList.Add(
                "You can only move if you are holding your knife out. If you want to shoot, you must stop moving and then take out your gun");
            challengeList.Add("Only crouching");
            challengeList.Add("No descritive callouts, only callouts like \"They're over there!\"");
            challengeList.Add("You must stay in spawn until the spike has been planted");
            challengeList.Add("Operator only, no knifing");
            challengeList.Add("Every time you get a kill, you must return to spawn before you can get another kill");
            challengeList.Add("Shout out \"DOOR!\" everytime you go through a door");
            challengeList.Add(
                "Everyone must buy all their abilities. With the remaining money, all players (on the same team) must use the same weapon");
            challengeList.Add("Both teams cannot spend more than $1500 this round, no buying for another team member");
            challengeList.Add("Everyone must swap their keybinds for Fire and Jump");
            challengeList.Add("Only one player at a time can have their primary weapon drawn");
            challengeList.Add("After every kill, the player to earn the kill must stop and clap for 5 seconds");
            challengeList.Add("You can only shoot your weapons when you are moving");
            challengeList.Add(
                "Players cant reload their weapons. Once your clip is out of ammo, you must drop your weapon. If you find a weapon (that is not your own) that has been discarded, you may pick it up. Repeat when you run out of ammo in the clip for this weapon");
            challengeList.Add("Comms can only consist of \"Front\", \"Back\", \"Left\", and \"Right\"");
            challengeList.Add("Everyone must set their in-game mouse sensitivity to the minimum");
            challengeList.Add("Everyone must set their in-game mouse sensitivity to the maximum");
            challengeList.Add(
                "Players must buy a Shorty. You are allowed to damage enemies with other weapons, but you can only kill them with the Shorty. If you kill another player with a weapon other than the Shorty, you must drop your weapons and go only for knife kills");
            challengeList.Add("Only one player on each team can reload weapons");
            challengeList.Add("No comms or pings allowed");
            challengeList.Add("Every time the game clock shows a \"0\", everyone must do two 360s");
            challengeList.Add("Players can only use abilities when they have no ammo in their clip");
            challengeList.Add(
                "Designate one player on each team to be the \"leader\". The leader must give verbal permissions before the rest of the team can swap weapons, reload, or use abilities");
            challengeList.Add(
                "The floor is lava! When you are on the \"floor\" you must constantly jump. Any objects that are not the ground are safe");

            Random rnd = new Random();
            int[] usedChallenges = new int[challengeList.Count - 1];
            int challengeNum = 0;

            if (challAmt > challengeList.Count)
            {
                await ctx.Channel.SendMessageAsync("There are only " + challengeList.Count + " challenges.").ConfigureAwait(false);
            }
            else
            {
                int counter = 1;
                for (int i = 0; i < challAmt; i++)
                {
                    challengeNum = rnd.Next(0, challengeList.Count - 1);
                    while (usedChallenges.Contains(challengeNum))
                    {
                        challengeNum = rnd.Next(0, challengeList.Count - 1);
                    }

                    usedChallenges[i] = challengeNum;
                    await ctx.Channel.SendMessageAsync($"{counter}.\t" + challengeList[challengeNum]).ConfigureAwait(false);
                    counter++;
                }
            }
        }

        [Command("roulette")]
        [Description("Gets random fun challenge(s).")]
        public async Task FunChallengesSingle(CommandContext ctx)
        {
            List<string> challengeList = new List<string>();
            challengeList.Add("No abilities");
            challengeList.Add("No guns");
            challengeList.Add("No shields");
            challengeList.Add("No movement (after round start or after planting/defusing)");
            challengeList.Add("Only running");
            challengeList.Add("No running");
            challengeList.Add("Scoping only");
            challengeList.Add("No reloading (you can only reload when you run out of ammo)");
            challengeList.Add("No defusing");
            challengeList.Add("No planting");
            challengeList.Add("You can only walk backwards");
            challengeList.Add("You can only walk forwards");
            challengeList.Add("You can only walk left");
            challengeList.Add("You can only walk right");
            challengeList.Add("Everyone has to set the master volume to 0");
            challengeList.Add("Everyone has to set the master volume to 100");
            challengeList.Add("Sheriff only, no knifing");
            challengeList.Add("Everyone must 'cast' their gameplay. If you die, 'cast' the person you are watching");
            challengeList.Add(
                "Each team can only send one person out of spawn at a time, the rest must stay in spawn until the person who left dies");
            challengeList.Add("You can only use your abilities to kill");
            challengeList.Add("Every time you kill someone, you must swap your gun with theirs");
            challengeList.Add("Shotguns only");
            challengeList.Add(
                "You are only allowed to kill enemies through walls, you can see where people are; To kill them, it must be through a wall");
            challengeList.Add("Every person must buy the most expensive gun they can afford and you cant buy armor");
            challengeList.Add("Every person must put their headset on backwards");
            challengeList.Add("Everyone on defense must go to one site");
            challengeList.Add("Everyone on offense must go to one site");
            challengeList.Add("Only the person on the bottom of the leaderboard can comm");
            challengeList.Add(
                "You can only move if you are holding your knife out. If you want to shoot, you must stop moving and then take out your gun");
            challengeList.Add("Only crouching");
            challengeList.Add("No descritive callouts, only callouts like \"They're over there!\"");
            challengeList.Add("You must stay in spawn until the spike has been planted");
            challengeList.Add("Operator only, no knifing");
            challengeList.Add("Every time you get a kill, you must return to spawn before you can get another kill");
            challengeList.Add("Shout out \"DOOR!\" everytime you go through a door");
            challengeList.Add(
                "Everyone must buy all their abilities. With the remaining money, all players (on the same team) must use the same weapon");
            challengeList.Add("Both teams cannot spend more than $1500 this round, no buying for another team member");
            challengeList.Add("Everyone must swap their keybinds for Fire and Jump");
            challengeList.Add("Only one player at a time can have their primary weapon drawn");
            challengeList.Add("After every kill, the player to earn the kill must stop and clap for 5 seconds");
            challengeList.Add("You can only shoot your weapons when you are moving");
            challengeList.Add(
                "Players cant reload their weapons. Once your clip is out of ammo, you must drop your weapon. If you find a weapon (that is not your own) that has been discarded, you may pick it up. Repeat when you run out of ammo in the clip for this weapon");
            challengeList.Add("Comms can only consist of \"Front\", \"Back\", \"Left\", and \"Right\"");
            challengeList.Add("Everyone must set their in-game mouse sensitivity to the minimum");
            challengeList.Add("Everyone must set their in-game mouse sensitivity to the maximum");
            challengeList.Add(
                "Players must buy a Shorty. You are allowed to damage enemies with other weapons, but you can only kill them with the Shorty. If you kill another player with a weapon other than the Shorty, you must drop your weapons and go only for knife kills");
            challengeList.Add("Only one player on each team can reload weapons");
            challengeList.Add("No comms or pings allowed");
            challengeList.Add("Every time the game clock shows a \"0\", everyone must do two 360s");
            challengeList.Add("Players can only use abilities when they have no ammo in their clip");
            challengeList.Add(
                "Designate one player on each team to be the \"leader\". The leader must give verbal permissions before the rest of the team can swap weapons, reload, or use abilities");
            challengeList.Add(
                "The floor is lava! When you are on the \"floor\" you must constantly jump. Any objects that are not the ground are safe");

            Random rnd = new Random();
            int[] usedChallenges = new int[challengeList.Count - 1];
            int challengeNum = 0;

            challengeNum = rnd.Next(0, challengeList.Count - 1);

            await ctx.Channel.SendMessageAsync(challengeList[challengeNum]).ConfigureAwait(false);
            
        }
        
        /*[Command("agents")]
        [Description("Shows an embed of all the agents.")]
        public async Task AgentList(CommandContext ctx)
        {
            var filename = Path.GetFileName("C:\\Users\\Carter\\Desktop\\ValorantBot\\AgentIcons\\");
            
            var embed = new DiscordEmbedBuilder
            {
                Title = "Agent List",
                Description = "All the agents in Valorant",
                Color = DiscordColor.Red,
            }.WithImageUrl($"attachment://{filename}");
            
            var basePath = "C:\\Users\\Carter\\Desktop\\ValorantBot\\AgentIcons\\";

            embed.AddField("Astra", embed.ImageUrl(basePath + "astra.png"), true);
            embed.AddField("Breach", $"{basePath + "breach.png"}", true);
            embed.AddField("Brimstone", $"{basePath + "brimstone.png"}", true);
            embed.AddField("Chamber", $"{basePath + "chamber.png"}", true);
            embed.AddField("Cypher", $"{basePath + "cypher.png"}", true);
            embed.AddField("Fade", $"{basePath + "fade.png"}", true);
            embed.AddField("Gekko", $"{basePath + "gekko.png"}", true);
            embed.AddField("Harbor", $"{basePath + "harbor.png"}", true);
            embed.AddField("Jett", $"{basePath + "jett.png"}", true);
            embed.AddField("KAY/O", $"{basePath + "kayo.png"}", true);
            embed.AddField("Killjoy", $"{basePath + "killjoy.png"}", true);
            embed.AddField("Neon", $"{basePath + "neon.png"}", true);
            embed.AddField("Omen", $"{basePath + "omen.png"}", true);
            embed.AddField("Phoenix", $"{basePath + "phoenix.png"}", true);
            embed.AddField("Raze", $"{basePath + "raze.png"}", true);
            embed.AddField("Reyna", $"{basePath + "reyna.png"}", true);
            embed.AddField("Sage", $"{basePath + "sage.png"}", true);
            embed.AddField("Skye", $"{basePath + "skye.png"}", true);
            embed.AddField("Sova", $"{basePath + "sova.png"}", true);
            embed.AddField("Viper", $"{basePath + "viper.png"}", true);
            embed.AddField("Yoru", $"{basePath + "yoru.png"}", true);
            

            await ctx.Channel.SendMessageAsync(embed: embed).ConfigureAwait(false);
        }*/
        
        /*[Command("maps")]
        [Description("Shows an embed of all the maps.")]
        public async Task MapList(CommandContext ctx)
        {
            var filename = Path.GetFileName("C:\\Users\\Carter\\Desktop\\ValorantBot\\MapIcons\\");
            
            var embed = new DiscordEmbedBuilder
            {
                Title = "Map List",
                Description = "All the maps in Valorant",
                Color = DiscordColor.Red,
            }.WithImageUrl($"attachment://{filename}");
            
            var basePath = "C:\\Users\\Carter\\Desktop\\ValorantBot\\MapIcons\\";

            embed.AddField("Ascent", embed.ImageUrl(basePath + "ascent.png"), true);
            embed.AddField("Bind", $"{basePath + "bind.png"}", true);
            embed.AddField("Breeze", $"{basePath + "breeze.png"}", true);
            embed.AddField("Fracture", $"{basePath + "fracture.png"}", true);
            embed.AddField("Haven", $"{basePath + "haven.png"}", true);
            embed.AddField("Icebox", $"{basePath + "icebox.png"}", true);
            embed.AddField("Split", $"{basePath + "split.png"}", true);
            embed.AddField("The Range", $"{basePath + "therange.png"}", true);
            

            await ctx.Channel.SendMessageAsync(embed: embed).ConfigureAwait(false);
        }*/
    }
}