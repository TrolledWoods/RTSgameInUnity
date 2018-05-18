using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Tiles
{
    public class Tree : TileTemplate
    {
        static TerrainRequirements _Requirements = new TerrainRequirements(new int[]
        {
            0, 0, 0, 0
        }, 2, 2);

        public Tree() : base(ResourceLoader.resources.Trees, _Requirements)
        {

        }

    }
}
