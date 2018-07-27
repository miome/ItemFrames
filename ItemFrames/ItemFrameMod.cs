using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley.Objects;
using StardewValley.Locations;
using StardewValley;
using ItemFrames.Framework;
using System.Linq;

namespace ItemFrames
{
    public class ItemFrameMod : Mod, IAssetEditor
    {
        private ModConfig Config;
        public override void Entry(IModHelper helper)
        {
            SaveEvents.AfterLoad += this.SaveEvents_AfterLoad;
            SaveEvents.BeforeSave += this.SaveEvents_BeforeSave;
            SaveEvents.AfterSave += this.SaveEvents_AfterSave;
            this.Config = this.Helper.ReadConfig<ModConfig>();
        }
        private void SaveEvents_AfterLoad(object sender, EventArgs e)
        {
            ItemFrame frame = new ItemFrame(1547, new Vector2(0,0), this.Monitor);
            Game1.player.addItemToInventory(frame);
            Game1.addHUDMessage(new HUDMessage($"New ItemFrame added to Inventory"));
            this.Monitor.Log("New ItemFrame added to inventory", LogLevel.Trace);
            this.RestoreItemFrames();
        }
        private void SaveEvents_BeforeSave(object sender, EventArgs eventArgs){
            this.ConvertItemFrames();
        }
        private void SaveEvents_AfterSave(object sender, EventArgs eventArgs){
            this.RestoreItemFrames();
        }
        private void ConvertItemFrames()
        {
            foreach (GameLocation location in ItemFrameMod.GetLocations())
            {
                foreach (StardewValley.Object o in location.objects.Values)
                {
                    if(o is Chest chest){
                        for (int i = 0; i < chest.items.Count; i++)
                        {
                            if (chest.items[i] is ItemFrame frame)
                            {
                                chest.items[i] = frame.asFurniture();
                            }
                        }
                    }
                }
                if (location is StardewValley.Locations.DecoratableLocation decoLoc)
                {
                    for (int i = 0; i < decoLoc.furniture.Count; i++)
                    {
                        if (decoLoc.furniture[i] is ItemFrame frame)
                        {
                            decoLoc.furniture[i] = frame.asFurniture();
                        }
                    }
                }
            }
            for (int i = 0; i < Game1.player.items.Count; i++){
                if(Game1.player.items[i] is ItemFrame frame){
                    Game1.player.items[i] = frame.asFurniture();
                }
            } 
        }

        private void RestoreItemFrames(){
            foreach (GameLocation location in ItemFrameMod.GetLocations())
            {
                foreach (StardewValley.Object o in location.objects.Values){
                    this.Monitor.Log($"{location.name} has {o.Name}");
                    if (o is Chest chest){
                        for (int i = 0; i<chest.items.Count; i++){
                            if(chest.items[i] is Furniture furniture2 && furniture2.Name=="ItemFrame"){
                                chest.items[i] = new ItemFrame(furniture2, this.Monitor);
                            }
                        }
                    }

                }
                if (location is StardewValley.Locations.DecoratableLocation decoLoc)
                {
                    for (int i = 0; i < decoLoc.furniture.Count; i++)
                    {
                        if(decoLoc.furniture[i].Name == "ItemFrame"){
                            decoLoc.furniture[i] = new ItemFrame(decoLoc.furniture[i],this.Monitor);
                        }
                    }
                }
            }
            for (int i = 0; i < Game1.player.items.Count; i++){
                if (Game1.player.items[i] is Furniture furniture && furniture.Name=="ItemFrame"){
                    Game1.player.items[i] = new ItemFrame(furniture, this.Monitor);
                }
            }
        }

        /// <summary>Get all game locations.</summary>
        public static IEnumerable<GameLocation> GetLocations()
        {
            return Game1.locations.Concat(
                    from location in Game1.locations.OfType<BuildableGameLocation>()
                    from building in location.buildings
                    where building.indoors.Value != null
                    select building.indoors.Value
                );
        }
        public bool CanEdit<T>(IAssetInfo asset)
        {
            return false;
        }
        public void Edit<T>(IAssetData asset)
        {
        }

        private List<ItemFrame> LoadItemFrames(){
            return new List<ItemFrame>();
            
        }

    }
}
