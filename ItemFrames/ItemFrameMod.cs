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
            ItemFrame frame = new ItemFrame(1539, new Vector2(-1, -1));
            //frame.furniture_type.Set(6);
            //frame.parentSheetIndex.Set(1539);
            Game1.player.addItemToInventory(frame);
            Game1.addHUDMessage(new HUDMessage($"New ItemFrame added to Inventory"));
            this.Monitor.Log("New ItemFrame added to inventory", LogLevel.Trace);
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
