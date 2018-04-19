using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.WorldGenerators
{
    public class Classic : World_Generator
    {
        float roughness;
        float scale;
        float biome_delta;
        int water_level;

        public Classic(float roughness, float scale, float biome_delta, int water_level)
        {
            this.roughness = roughness;
            this.scale = scale;
            this.biome_delta = biome_delta;
            this.water_level = water_level;
        }

        public override World.Vertex Generate_Vertex(int x, int z)
        {
            // Calculate the height of the tile
            int tileHeight = Mathf.FloorToInt(Mathf.PerlinNoise(x * roughness, z * roughness) * scale);
            
            // Get some tileData based on the current biome
            VertexType tileData = (VertexType)Mathf.FloorToInt(
                Mathf.Clamp(Mathf.PerlinNoise(x * biome_delta, z * biome_delta) * 
                (int)VertexType.N_TILES, 0, (int)VertexType.N_TILES - 1));

            // Check if the height is lower than the water level, in which case make it sand or dirt
            if (Mathf.Abs(tileHeight - water_level) < 1)
            {
                tileData = VertexType.Sand;
            }
            else if (tileHeight < water_level)
            {
                tileData = VertexType.Dirt;
            }

            return new World.Vertex(
                tileHeight,
                tileData);
        }

    }
}
