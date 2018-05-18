using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Tiles
{
    public class Grass : TileTemplate
    {
        static TerrainRequirements _Requirements = new TerrainRequirements(new int[]
        {
            0, 0, 0, 0
        }, 2, 2);

        public Grass() : base(ResourceLoader.resources.Grass, _Requirements)
        {
            
        }

    }
}
