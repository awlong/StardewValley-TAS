﻿using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAS.GameState;
using TAS.Utilities;
using Object = StardewValley.Object;

namespace TAS.Overlays
{
    public class ObjectDrop : IOverlay
    {
        public override string Name => "objectdrop";
        private string currentLocationName = "";
        private int currentLocationNumObjects = -1;
        private Dictionary<Vector2, IEnumerable<string>> objectsThatHaveDrops;

        public Color RectColor = new Color(0, 0, 0, 180);
        public Color TextColor = Color.White;

        public override string[] HelpText()
        {
            return new string[] { string.Format("{0}: display object drops", Name) };
        }

        public override void Reset()
        {
            objectsThatHaveDrops = null;
            currentLocationName = "";
            currentLocationNumObjects = -1;
        }

        public override void ActiveUpdate()
        {
            if (CurrentLocation.Active)
            {
                if (Game1.currentLocation.Name != currentLocationName || Game1.currentLocation.Objects.Count() != currentLocationNumObjects)
                {
                    objectsThatHaveDrops = new Dictionary<Vector2, IEnumerable<string>>();
                    foreach (KeyValuePair<Vector2, Object> pair in Game1.currentLocation.Objects.Pairs)
                    {
                        if (pair.Value is BreakableContainer obj)
                        {
                            IEnumerable<string> contents = DropInfo.BreakContainer(pair.Key, obj);
                            if (contents != null && contents.Count() > 0)
                                objectsThatHaveDrops.Add(pair.Key, contents);
                        }
                        else if (pair.Value.DisplayName == "Stone")
                        {
                            IEnumerable<string> contents = DropInfo.OnStoneDestroyed(pair.Value.ParentSheetIndex, (int)pair.Key.X, (int)pair.Key.Y);
                            if (contents != null && contents.Count() > 0)
                                objectsThatHaveDrops.Add(pair.Key, contents);
                        }
                        else if (pair.Value.ParentSheetIndex == 590)
                        {
                            string contents = ArtifactSpot.DigUp(pair.Key, CurrentLocation.Name);
                            if (contents != null && contents.Count() > 0)
                                objectsThatHaveDrops.Add(pair.Key, new List<string> { contents });
                        }
                    }
                    currentLocationName = Game1.currentLocation.Name;
                    currentLocationNumObjects = Game1.currentLocation.Objects.Count();
                }
            }
        }

        public override void ActiveDraw(SpriteBatch spriteBatch)
        {
            if (Game1.currentLocation != null && objectsThatHaveDrops != null)
            {
                foreach (KeyValuePair<Vector2, IEnumerable<string>> pair in objectsThatHaveDrops)
                {
                    DrawTextAtTile(spriteBatch, pair.Value, pair.Key, TextColor, RectColor);
                }
            }
        }
    }
}
