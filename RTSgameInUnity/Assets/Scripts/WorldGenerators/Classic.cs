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

        GenerationTile[][] generationMap;

        public Classic(float roughness, float scale, float biome_delta, int water_level)
        {
            this.roughness = roughness;
            this.scale = scale;
            this.biome_delta = biome_delta;
            this.water_level = water_level;

            generationMap = new GenerationTile[((int)VertexType.N_TILES)][];
            generationMap[(int)VertexType.Grass] = new GenerationTile[] {
                new GenerationTile(new Tiles.Tree(), 0.01f),
                new GenerationTile(new Tiles.Grass(), 0.3f)
            };
        }

        public override float GetOrigin() { return water_level + 0.2f; }

        public override Generated_World Generate_World(GameObject entity_parent, int width, int height)
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

            // Generate tiles
            World.TileData[] tiles = new World.TileData[(width-1) * (height-1)];

            index = 0;
            int v_index = 0;
            for (int j = 0; j < height - 1; j++)
            {
                for (int i = 0; i < width - 1; i++)
                {
                    GenerationTile[] available = generationMap[(int)vertices[v_index].Type.Peek()];

                    if (available != null)
                    {
                        for(int e = 0; e < available.Length; e++)
                        {
                            if(UnityEngine.Random.value < available[e].probability)
                            {
                                GenerationTile genT = available[e];

                                if (genT.tile.Requirements.ValidateTerrain(vertices, width, height, i, j).Valid)
                                {
                                    Tiles.Tile t = genT.tile.CreateInstance(
                                        entity_parent,
                                        new Vector3(i, height_map[v_index] + 1, j));

                                    tiles[index] = new World.TileData(t);

                                    break;
                                }
                            }
                        }
                    }
                    v_index++;
                    index++;
                }
                v_index++;
            }

            // Instantiate the world
            Generated_World world = new Generated_World();
            world.Vertices = vertices;
            world.Tiles = tiles;
            world.Entities = new QuadTree(0, 0, width, height);

            return world;
        }

        class GenerationTile
        {
            public Tiles.TileTemplate tile;
            public float probability;

            public GenerationTile(Tiles.TileTemplate tile, float prob)
            {
                this.tile = tile;
                this.probability = prob;
            }
        }

    }
}
