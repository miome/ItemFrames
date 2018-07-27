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
        ItemFrameFactory factory;
        private ModConfig Config;
        public override void Entry(IModHelper helper)
        {
            SaveEvents.AfterLoad += this.SaveEvents_AfterLoad;
            SaveEvents.BeforeSave += this.SaveEvents_BeforeSave;
            SaveEvents.AfterSave += this.SaveEvents_AfterSave;
            this.Config = this.Helper.ReadConfig<ModConfig>();
            this.factory = new ItemFrameFactory();
        }
        private void SaveEvents_AfterLoad(object sender, EventArgs e)
        {
            //ItemFrame frame = new ItemFrame(1547, new Vector2(0,0), this.Monitor);
            ItemFrame frame = this.factory.newItemFrame(1547, new Vector2(0, 0), this.Monitor);
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
        private void ReplaceEachFrame(StardewValley.Object[] inv){
            for (int i = 0; i < inv.Count(); i++)
            {
                if (inv[i] is ItemFrame frame)
                {
                    Furniture replacement = new Furniture(frame.ParentSheetIndex, frame.TileLocation);
                    replacement.name = "ItemFrame";
                    if (frame.displayItem.Value is StardewValley.Object o)
                    {
                        replacement.heldObject.Set(new Netcode.NetRef<StardewValley.Object>(o));
                    }
                    inv[i] = replacement;
                }
            }
            
        }
        private void RestoreItemFrames(){
            foreach (GameLocation location in ItemFrameMod.GetLocations())
            {
                if (location is StardewValley.Locations.DecoratableLocation decoLoc)
                {
                    for (int i = 0; i < decoLoc.furniture.Count; i++)
                    {
                        Furniture furniture = decoLoc.furniture[i];
                        if(furniture.Name == "ItemFrame"){
                            ItemFrame frame = new ItemFrame(furniture.ParentSheetIndex, furniture.TileLocation, this.Monitor, furniture.heldObject);
                            decoLoc.furniture[i] = frame;
                        }
                    }
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
