using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.Scripts.Entities;

namespace Assets.Scripts
{
    public enum VertexType
    {
        Dirt, Grass, Sand, N_TILES
    }

    public class World
    {
        public struct Vertex
        {
            public int Height;
            public Stack<VertexType> Type;

            public Vertex(Stack<VertexType> blocks)
            {
                Height = blocks.Count;
                Type = blocks;
            }
        }

        public struct TileData
        {
            Tiles.Tile tile;

            public TileData(Tiles.Tile t)
            {
                tile = t;
            }
        }
        
        public static Color32[] Tile_Colors;

        public float Origin;
        
        public Vertex[] Vertices;
        public TileData[] Tiles;

        public WorldGenerators.World_Generator generator;

        int tile_width;
        int tile_height;

        public QuadTree Entities;
        
        // Some properties for the tiles array
        public int Tile_Width { get { return tile_width; } }
        public int Tile_Height { get { return tile_height; } }
        public int Tile_Count { get { return tile_width * tile_height; } }

        // Some properties for the vertices array
        public int Vertex_Width { get { return tile_width + 1; } }
        public int Vertex_Height { get { return tile_height + 1; } }
        public int Vertex_Count { get { return (tile_width + 1) * (tile_height + 1); } }

        public World(GameObject entity_parent, int tile_width, int tile_height, WorldGenerators.World_Generator generator)
        {
            this.tile_width = tile_width;
            this.tile_height = tile_height;

            this.generator = generator;
            this.Origin = generator.GetOrigin();
            
            Generate_World(entity_parent);
        }

        public void Generate_World(GameObject entity_parent)
        {
            WorldGenerators.Generated_World w = generator.Generate_World(entity_parent, Vertex_Width, Vertex_Height);
            Vertices = w.Vertices;
            Tiles = w.Tiles;
            Entities = w.Entities;
        }

        public void SetVertex(int x, int y, Vertex v)
        {
            Vertices[x + y * Vertex_Width] = v;
        }

        public Vertex GetVertex(int x, int y)
        {
            return Vertices[x + y * Vertex_Width];
        }

    }
}
