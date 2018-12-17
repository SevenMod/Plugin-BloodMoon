// <copyright file="BloodMoon.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Plugin.BloodMoon
{
    using SevenMod.Console;
    using SevenMod.ConVar;
    using SevenMod.Core;

    /// <summary>
    /// Plugin that displays the number of days until the next blood moon.
    /// </summary>
    public sealed class BloodMoon : PluginAbstract
    {
        /// <summary>
        /// The value of the BloodMoonInterval <see cref="ConVar"/>.
        /// </summary>
        private ConVarValue interval;

        /// <summary>
        /// The value of the BloodMoonShowOnSpawn <see cref="ConVar"/>.
        /// </summary>
        private ConVarValue showOnSpawn;

        /// <inheritdoc/>
        public override PluginInfo Info => new PluginInfo
        {
            Name = "BloodMoon",
            Author = "SevenMod",
            Description = "Displays the number of days until the next blood moon.",
            Version = "0.1.0.0",
            Website = "https://github.com/SevenMod/Plugin-BloodMoon"
        };

        /// <inheritdoc/>
        public override void OnLoadPlugin()
        {
            this.interval = this.CreateConVar("BloodMoonInterval", "7", "The number of days between blood moons.", true, 1).Value;
            this.showOnSpawn = this.CreateConVar("BloodMoonShowOnSpawn", "True", "Whether to show the number of days until the next blood moon to newly spawned players.").Value;

            this.AutoExecConfig(true, "BloodMoon");

            this.RegAdminCmd("bloodmoon", 0, "Show the number of days until the next blood moon.").Executed += this.OnBloodmoonCommandExecuted;
        }

        /// <inheritdoc/>
        public override void OnPlayerSpawnedInWorld(SMClient client, SMRespawnType respawnReason, Pos pos)
        {
            if (this.showOnSpawn.AsBool)
            {
                this.PrintToChat(client, this.GetMessage());
            }
        }

        /// <summary>
        /// Called when the bloodmoon admin command is executed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="AdminCommandEventArgs"/> object containing the event data.</param>
        private void OnBloodmoonCommandExecuted(object sender, AdminCommandEventArgs e)
        {
            var message = this.GetMessage();
            if (!this.ShouldReplyToChat(e.Client))
            {
                this.ReplyToCommand(e.Client, message);
            }

            this.PrintToChatAll(message);
        }

        /// <summary>
        /// Gets a formatted string describing the number of days until the next blood moon.
        /// </summary>
        /// <returns>The formatted string.</returns>
        private string GetMessage()
        {
            var days = this.GetDays();
            if (days == 1)
            {
                return "There is [b]1[/b] day until the next blood moon.";
            }
            else if (days > 0)
            {
                return $"There are [b]{days}[/b] days until the next blood moon.";
            }
            else
            {
                return "The next blood moon is [b]tonight[b]!";
            }
        }

        /// <summary>
        /// Gets the number of days until the next blood moon.
        /// </summary>
        /// <returns>The number of days between today and the next blood moon.</returns>
        private int GetDays()
        {
            return this.interval.AsInt - (GameUtils.WorldTimeToDays(GameManager.Instance.World.GetWorldTime()) % this.interval.AsInt);
        }
    }
}
