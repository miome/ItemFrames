using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using ItemFrames.Framework;

namespace ItemFrames
{
    public class ItemFrameMod : Mod, IAssetEditor
    {
        private ModConfig Config;
        public override void Entry(IModHelper helper)
        {
            SaveEvents.AfterLoad += this.SaveEvents_AfterLoad;
            this.Config = this.Helper.ReadConfig<ModConfig>();
        }
        private void SaveEvents_AfterLoad(object sender, EventArgs e)
        {
            Game1.player.addItemToInventory(new ItemFrame());
        }
        public bool CanEdit<T>(IAssetInfo asset)
        {
            return false;
        }
        public void Edit<T>(IAssetData asset)
        {
        }

    }
}
