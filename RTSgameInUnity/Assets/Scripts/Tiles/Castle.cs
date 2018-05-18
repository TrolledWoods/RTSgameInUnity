using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Tiles
{
    public class Castle : TileTemplate
    {
        static TerrainRequirements _Requirements = TerrainRequirements.CreateFlat(9, 9);

        public Castle() : base(ResourceLoader.resources.Castles, _Requirements)
        {

        }

    }
}
