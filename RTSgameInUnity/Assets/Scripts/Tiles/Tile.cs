using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Tiles
{
    public class Tile
    {
        public World.TileData[] accessPoints;
        public GameObject Visual;

        public Tile(GameObject visual, World.TileData[] accessPoints)
        {
            Visual = visual;
            this.accessPoints = accessPoints;
        }
    }
}
