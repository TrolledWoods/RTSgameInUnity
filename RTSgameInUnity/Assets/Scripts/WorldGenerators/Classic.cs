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

        public override float GetOrigin() { return water_level + 0.2f; }

        public override World.Vertex[] Generate_World(int width, int height)
        {
            // Generate height map
            int[] height_map = new int[width * height];

            int index = 0;
            for (int j = 0; j < height; j++)
            {
                for(int i = 0; i < width; i++)
                {
                    height_map[index] = 
                        Mathf.FloorToInt(Mathf.PerlinNoise(i * roughness, j * roughness) * 
                            scale + water_level - scale - water_level * 3 + 
                            Mathf.PerlinNoise(
                            i * biome_delta + 100, 
                            j * biome_delta + 100) * 
                            water_level * 6);
                    height_map[index] = height_map[index] <= 0 ? 1 : height_map[index];
                    index++;
                }
            }

            // Generate vertices
            World.Vertex[] vertices = new World.Vertex[width * height];

            int len = width * height;
            for (index = 0; index < len; index++)
            {
                Stack<VertexType> vert_stack = new Stack<VertexType>();

                while (vert_stack.Count <= height_map[index])
                {
                    if (vert_stack.Count < water_level)
                        vert_stack.Push(VertexType.Dirt);
                    else if (vert_stack.Count < water_level + 2)
                        vert_stack.Push(VertexType.Sand);
                    else
                        vert_stack.Push(VertexType.Grass);
                }

                vertices[index] = new World.Vertex(vert_stack);
            }

            return vertices;
        }

    }
}
