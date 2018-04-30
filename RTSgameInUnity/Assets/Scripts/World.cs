﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

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

        public struct Tile
        {

        }
        
        public static Color32[] Tile_Colors;

        public float Origin;

        public Vertex[] Vertices;
        public Tile[] Tiles;

        public WorldGenerators.World_Generator generator;

        int tile_width;
        int tile_height;

        public int Tile_Width { get { return tile_width; } }
        public int Tile_Height { get { return tile_height; } }
        public int Tile_Count { get { return tile_width * tile_height; } }

        public int Vertex_Width { get { return tile_width + 1; } }
        public int Vertex_Height { get { return tile_height + 1; } }
        public int Vertex_Count { get { return (tile_width + 1) * (tile_height + 1); } }

        public World(int tile_width, int tile_height, WorldGenerators.World_Generator generator)
        {
            this.tile_width = tile_width;
            this.tile_height = tile_height;

            this.generator = generator;
            this.Origin = generator.GetOrigin();

            // If the tile colors are not initialized, initialize them with
            // their default value
            if(Tile_Colors == null)
            {
                Tile_Colors = new Color32[(int)VertexType.N_TILES];
                Tile_Colors[(int)VertexType.Dirt] = new Color32(120, 72, 0, 255);
                Tile_Colors[(int)VertexType.Grass] = new Color32(1, 142, 14, 255);
                Tile_Colors[(int)VertexType.Sand] = new Color32(194, 178, 128, 255);
            }

            Generate_World();
        }

        public void Generate_World()
        {
            Vertices = generator.Generate_World(Vertex_Width, Vertex_Height);
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
