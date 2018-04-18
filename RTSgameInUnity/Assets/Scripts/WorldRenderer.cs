using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class WorldRenderer
    {
        static int SectionSize = 200;
        
        World game_world;
        
        Color32[] vertex_colors;
        Material material;

        GameObject mesh_parent;
        MeshController[] controllers;

        public WorldRenderer(GameObject mesh_parent, World game_world, Color32[] vertex_colors, Material material)
        {
            this.mesh_parent = mesh_parent;

            this.vertex_colors = vertex_colors;

            this.game_world = game_world;
            this.material = material;

            CreateMesh();
        }

        public void CreateMesh()
        {
            // Clear all objects in the containers array
            if(controllers != null)
            {
                for(int i = controllers.Length - 1; i >= 0; i--)
                {
                    GameObject.Destroy(controllers[i].Container);
                }
            }

            int n_sections_width = Int_Ceil(game_world.Tile_Width, SectionSize);
            int n_sections_height = Int_Ceil(game_world.Tile_Height, SectionSize);
            int current_section = 0;

            int left = 0;
            int right;
            int bottom;
            int top;

            controllers = new MeshController[n_sections_width * n_sections_height];
   
            for (int i = 0; i < n_sections_width; i++)
            {
                bottom = 0;

                for(int j = 0; j < n_sections_height; j++)
                {
                    right = left + SectionSize + 1;
                    top = bottom + SectionSize + 1;

                    right = right >= game_world.Vertex_Width ? 
                        game_world.Vertex_Width - 1 : right;
                    top = top >= game_world.Vertex_Height ? 
                        game_world.Vertex_Height - 1 : top;

                    controllers[current_section] = new MeshController(left, right, 
                        bottom, top, 
                        game_world,
                        vertex_colors, material);
                    controllers[current_section].Container.transform.parent = mesh_parent.transform;
                    
                    bottom += SectionSize;
                    current_section++;
                }
                left += SectionSize;
            }
        }

        public int Int_Ceil(int a, int b)
        {
            return (a / b) + (a % b == 0 ? 0 : 1);
        }

        class MeshController
        {
            World game_world;
            
            GameObject container;
            public GameObject Container { get { return container; } }
            
            public Color32[] Vertex_Colors;

            Mesh controlling;
            MeshCollider collider;

            Vector3[] vertices;
            Color32[] colors;
            int[] triangles;

            int vert_left, vert_right, vert_top, vert_bottom;
            int vert_width, vert_height, tile_width, tile_height;

            public MeshController(int left, int right, int bottom, int top, World game_world, Color32[] vertex_colors, Material material)
            {
                container = new GameObject("World Section", 
                    typeof(MeshRenderer), 
                    typeof(MeshFilter),
                    typeof(MeshCollider));
                container.transform.position = new Vector3(left, 0, bottom);

                container.layer = 8;
                controlling = container.GetComponent<MeshFilter>().mesh;
                container.GetComponent<MeshRenderer>().material = material;
                collider = container.GetComponent<MeshCollider>();

                this.game_world = game_world;

                this.vert_left = left;
                this.vert_right = right;
                this.vert_top = top;
                this.vert_bottom = bottom;

                vert_width = right - left;
                vert_height = top - bottom;
                tile_width = vert_width - 1;
                tile_height = vert_height - 1;
                
                Vertex_Colors = vertex_colors;

                // Create arrays
                colors = new Color32[vert_width * vert_height];
                vertices = new Vector3[vert_width * vert_height];
                Debug.Log(vert_width + " " + vert_height);
                triangles = new int[(vert_width - 1) * (vert_height - 1) * 6];

                CreateMesh();
            }

            public void CreateMesh()
            {
                // Instantiate some useful variables
                int local_vert_i;
                int global_vert_i;
                int tri_i;
                World.Vertex read_vert;

                // Create vertices
                for (int j = 0; j < vert_height; j++)
                {
                    for (int i = 0; i < vert_width; i++)
                    {
                        local_vert_i = i + j * vert_width;
                        global_vert_i = (i + vert_left) + (j + vert_bottom) * game_world.Vertex_Width;

                        read_vert = game_world.Vertices[global_vert_i];
                        vertices[local_vert_i] = new Vector3(i, read_vert.Height, j);
                        colors[local_vert_i] = Vertex_Colors[(int)read_vert.Type];
                    }
                }

                // Create triangles
                tri_i = 0;
                for (int j = 0; j < tile_height; j++)
                {
                    for (int i = 0; i < tile_width; i++)
                    {
                        local_vert_i = i + j * vert_width;

                        triangles[tri_i + 0] = local_vert_i + 0;
                        triangles[tri_i + 1] = local_vert_i + vert_width + 1;
                        triangles[tri_i + 2] = local_vert_i + 1;

                        triangles[tri_i + 3] = local_vert_i + 0;
                        triangles[tri_i + 4] = local_vert_i + vert_width;
                        triangles[tri_i + 5] = local_vert_i + vert_width + 1;

                        tri_i += 6;
                    }
                }

                // Set the values of the mesh
                controlling.Clear();
                controlling.vertices = vertices;
                controlling.colors32 = colors;
                controlling.triangles = triangles;
                controlling.RecalculateNormals();
                controlling.RecalculateBounds();

                // Update the mesh collider
                if (collider != null)
                    collider.sharedMesh = controlling;
            }
        }

    }
}
