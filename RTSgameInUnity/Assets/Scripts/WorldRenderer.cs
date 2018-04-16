using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class WorldRenderer
    {
        World game_world;

        Mesh world_mesh;
        MeshCollider world_collider;

        Color32[] vertex_colors;

        Vector3[] vertices;
        Color32[] colors;
        int[] triangles;
        
        public WorldRenderer(World game_world, Color32[] vertex_colors,
            Mesh world_mesh, MeshCollider world_collider = null)
        {
            this.game_world = game_world;

            this.world_mesh = world_mesh;
            this.world_collider = world_collider;
            this.vertex_colors = vertex_colors;

            vertices = new Vector3[game_world.Vertex_Count];
            colors = new Color32[game_world.Vertex_Count];
            triangles = new int[game_world.Tile_Count * 2 * 3];

            CreateMesh();
        }

        public void CreateMesh()
        {
            int vert_count = game_world.Vertex_Count;
            int vert_width = game_world.Vertex_Width;
            int vert_height = game_world.Vertex_Height;

            int tile_width = game_world.Tile_Width;
            int tile_height = game_world.Tile_Height;

            int vert_index = 0;
            for (int j = 0; j < vert_height; j++)
            {
                for (int i = 0; i < vert_width; i++)
                {
                    vertices[vert_index] = new Vector3(i, game_world.Vertices[vert_index].Height, j);
                    colors[vert_index] = vertex_colors[(int)game_world.Vertices[vert_index].Type];
                    vert_index++;
                }
            }

            vert_index = 0;
            int tri_index = 0;
            for(int j = 0; j < tile_height; j++)
            {
                for(int i = 0; i < tile_width; i++)
                {
                    // Triangle 1
                    triangles[tri_index + 0] = vert_index + vert_width;
                    triangles[tri_index + 1] = vert_index + 1;
                    triangles[tri_index + 2] = vert_index + 0;

                    // Triangle 2
                    triangles[tri_index + 3] = vert_index + vert_width;
                    triangles[tri_index + 4] = vert_index + vert_width + 1;
                    triangles[tri_index + 5] = vert_index + 1;
                    
                    vert_index++;
                    tri_index += 6;
                }
                vert_index++;
            }

            // Set the values of the mesh
            world_mesh.Clear();
            world_mesh.vertices = vertices;
            world_mesh.colors32 = colors;
            world_mesh.triangles = triangles;
            world_mesh.RecalculateNormals();
            world_mesh.RecalculateBounds();
            
            // Update the mesh collider
            if (world_collider != null)
                world_collider.sharedMesh = world_mesh;
        }
    }
}
