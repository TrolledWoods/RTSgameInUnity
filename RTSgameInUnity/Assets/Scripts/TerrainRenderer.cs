using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.Scripts.RenderPipeline;

namespace Assets.Scripts
{
    public class TerrainRenderer
    {
        static int SectionSize = 30;
        
        RenderingPipeline pipeline;
        
        Material material;

        GameObject mesh_parent;
        MeshController[] controllers;
        int n_sections_width;
        int n_sections_height;

        public TerrainRenderer(GameObject mesh_parent, RenderingPipeline pipeline, Material material)
        {
            this.mesh_parent = mesh_parent;
            
            this.pipeline = pipeline;
            this.material = material;

            CreateMesh();
        }

        public void ChangeMaterial(Material m)
        {
            material = m;

            for(int i = 0; i < controllers.Length; i++)
            {
                controllers[i].ChangeMaterial(m);
            }
        }

        public void UpdateVertex(Vector2Int vertex_coord)
        {
            float section_x = vertex_coord.x / (float)SectionSize;
            float section_y = vertex_coord.y / (float)SectionSize;

            int top = Mathf.FloorToInt(section_y);
            int bottom = (top > 0 && (vertex_coord.y - top * SectionSize) == 0) ? top - 1 : top;
            int right = Mathf.FloorToInt(section_x);
            int left = (right > 0 && (vertex_coord.x - right * SectionSize) == 0) ? right - 1 : right;

            for (int i = left; i <= right; i++)
            {
                for(int j = bottom; j <= top; j++)
                {
                    int section_i = i + j * n_sections_width;
                    controllers[section_i].UpdateVertex(vertex_coord);
                }
            }
        }

        public void UpdateMesh()
        {
            for (int i = 0; i < controllers.Length; i++)
            {
                controllers[i].UpdateMesh();
            }
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

            n_sections_width = Int_Ceil_Divide(pipeline.Width, SectionSize);
            n_sections_height = Int_Ceil_Divide(pipeline.Height, SectionSize);
            int current_section = 0;

            int left;
            int right;
            int bottom = 0;
            int top;

            controllers = new MeshController[n_sections_width * n_sections_height];

            for (int j = 0; j < n_sections_height; j++)
            {
                left = 0;
                for (int i = 0; i < n_sections_width; i++)
                {
                    right = left + SectionSize + 1;
                    top = bottom + SectionSize + 1;

                    right = right > pipeline.Width ? 
                        pipeline.Width : right;
                    top = top >= pipeline.Height ? 
                        pipeline.Height : top;

                    controllers[current_section] = new MeshController(left, right, 
                        bottom, top, 
                        pipeline, material);
                    controllers[current_section].Container.transform.parent = mesh_parent.transform;
                    
                    left += SectionSize;
                    current_section++;
                }
                bottom += SectionSize;
            }
        }

        public static int Int_Ceil_Divide(int a, int b)
        {
            return (a / b) + (a % b == 0 ? 0 : 1);
        }

        static float GetHeight(VisualVertex vert)
        {
            return vert.Height;
        }

        class MeshController
        {
            RenderingPipeline pipeline;
            
            GameObject container;
            public GameObject Container { get { return container; } }
            
            Mesh controlling;
            MeshCollider collider;
            MeshRenderer renderer;

            Vector3[] vertices;
            Color32[] colors;
            int[] triangles;

            int vert_left, vert_right, vert_top, vert_bottom;
            int vert_width, vert_height, tile_width, tile_height;

            Queue<Vector2Int> vertex_update_queue;

            public MeshController(int left, int right, int bottom, int top, RenderingPipeline pipeline, Material material)
            {
                container = new GameObject("World Section", 
                    typeof(MeshRenderer), 
                    typeof(MeshFilter),
                    typeof(MeshCollider));
                container.transform.position = new Vector3(left, 0, bottom);

                container.layer = 8;
                controlling = container.GetComponent<MeshFilter>().mesh;
                renderer = container.GetComponent<MeshRenderer>();
                renderer.material = material;
                collider = container.GetComponent<MeshCollider>();

                this.pipeline = pipeline;

                this.vert_left = left;
                this.vert_right = right;
                this.vert_top = top;
                this.vert_bottom = bottom;

                vert_width = right - left;
                vert_height = top - bottom;
                tile_width = vert_width - 1;
                tile_height = vert_height - 1;
                
                vertex_update_queue = new Queue<Vector2Int>();

                // Create arrays
                colors = new Color32[vert_width * vert_height];
                vertices = new Vector3[vert_width * vert_height];
                triangles = new int[(vert_width - 1) * (vert_height - 1) * 6];

                CreateMesh();
            }

            public void ChangeMaterial(Material m)
            {
                renderer.material = m;
            }

            public void UpdateVertex(Vector2Int vertex_coord)
            {
                vertex_update_queue.Enqueue(vertex_coord);
            }

            public void UpdateMesh()
            {
                if (vertex_update_queue.Count == 0)
                    return;

                int local_vert_i;

                while (vertex_update_queue.Count > 0)
                {
                    Vector2Int v_coord = vertex_update_queue.Dequeue();

                    local_vert_i = v_coord.x - vert_left + (v_coord.y - vert_bottom) * vert_width;
                    VisualVertex v = pipeline.GetVertex(v_coord.x, v_coord.y);

                    vertices[local_vert_i].y = GetHeight(v);
                    colors[local_vert_i] = v.Color;
                }
                
                PassMeshProperties();
            }

            public void CreateMesh()
            {
                // Instantiate some useful variables
                int local_vert_i;
                int global_vert_i;
                int tri_i;
                VisualVertex read_vert;

                // Create vertices
                for (int j = 0; j < vert_height; j++)
                {
                    for (int i = 0; i < vert_width; i++)
                    {
                        local_vert_i = i + j * vert_width;
                        global_vert_i = (i + vert_left) + (j + vert_bottom) * pipeline.Width;

                        read_vert = pipeline.GetVertex(global_vert_i);
                        vertices[local_vert_i] = new Vector3(i, GetHeight(read_vert), j);
                        colors[local_vert_i] = read_vert.Color;
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

                PassMeshProperties();
            }

            void PassMeshProperties()
            {
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
