using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MapLoader : MonoBehaviour {

    enum Tiles
    {
        Dirt, Grass, Sand, N_TILES
    }
    
    public struct VertexData
    {
        public int y;
        public Color32 c;

        public VertexData(int y, Color32 c)
        {
            this.y = y;
            this.c = c;
        }
    }

    public int water_level = 5;
    public int camera_x = 0;
    public int camera_z = 0;
    int p_camera_x = 0;
    int p_camera_y = 0;

    Color32[] tileColors;

    VertexData[] vertice_data;
    List<GameObject> objects;

    MeshFilter filter;
    new MeshCollider collider;

    Vector3[] vertices;
    int[] triangles;
    Color32[] colors;

    public int width;
    public int height;
    
    void Start()
    {
        tileColors = new Color32[(int)Tiles.N_TILES];
        tileColors[(int)Tiles.Dirt] = new Color32(120, 72, 0, 255);
        tileColors[(int)Tiles.Grass] = new Color32(1, 142, 14, 255);
        tileColors[(int)Tiles.Sand] = new Color32(194, 178, 128, 255);

        filter = GetComponent<MeshFilter>();
        collider = GetComponent<MeshCollider>();
        objects = new List<GameObject>();

        generate_map(150, 100);
        /*load_map_from_string("3,3," +
            "3.0,0.0,3.0,0.0,2.7,3.0,3.0,3.0,2.4,2.0,2.0,2.0," +
            "3.0,0.0,3.0,0.0,2.7,3.0,3.0,3.0,2.4,0.7,0.7,0.7," +
            "3.0,0.7,0.7,0.7,2.7,2.0,2.0,2.0,2.4,0.7,0.7,0.7"); */
            
    }

    public void add_object(GameObject obj)
    {
        obj.transform.position = new Vector3(
            Mathf.Floor(obj.transform.position.x) + 0.5f,
            Mathf.Floor(obj.transform.position.y) + 0.5f,
            Mathf.Floor(obj.transform.position.z) + 0.5f
            );
        objects.Add(obj);
    }

    public VertexData get_data(int x, int z)
    {
        Debug.Log((x-camera_x) + " " + (z-camera_z));
        return vertice_data[(x-camera_x) + (z-camera_z) * width];
    }
    
    public void translate_map(int dx, int dz)
    {
        camera_x += dx;
        camera_z += dz;

        transform.position += new Vector3(dx, 0, dz);

        // First translate horizontally
        if(dx > 0)
        {
            for(int i = 0; i < width - dx; i++)
            {
                for(int j = 0; j < height; j++)
                {
                    int index = i + j * width;

                    vertice_data[index] = vertice_data[index + dx];
                }
            }

            for(int i = width - dx; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int index = i + j * width;

                    vertice_data[index] = generate_vertex(i + camera_x, j + camera_z);
                }
            }
        }else if(dx < 0)
        {
            dx *= -1;

            for (int i = width-1; i >= dx; i--)
            {
                for (int j = 0; j < height; j++)
                {
                    int index = i + j * width;
                    
                    vertice_data[index] = vertice_data[index - dx];
                }
            }

            for (int i = 0; i < dx; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int index = i + j * width;

                    vertice_data[index] = generate_vertex(i + camera_x, j + camera_z);
                }
            }
        }

        if(dz > 0)
        {
            for(int i = 0; i < width; i++)
            {
                for(int j = 0; j < height - dz; j++)
                {
                    int index = i + j * width;

                    vertice_data[index] = vertice_data[index + dz * width];
                }
            }

            for(int i = 0; i < width; i++)
            {
                for(int j = height - dz; j < height; j++)
                {
                    vertice_data[i + j * width] = generate_vertex(i+camera_x, j+camera_z);
                }
            }
        }else if(dz < 0)
        {
            dz *= -1;

            for(int i = 0; i < width; i++)
            {
                for(int j = height - 1; j >= dz; j--)
                {
                    int index = i + j * width;

                    vertice_data[index] = vertice_data[index - dz * width];
                }
            }

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < dz; j++)
                {
                    vertice_data[i + j * width] = generate_vertex(i + camera_x, j + camera_z);
                }
            }
        }

        create_mesh();
    }

    public void generate_map(int width, int height)
    {
        Debug.Log(camera_x + " " + camera_z);

        this.width = width;
        this.height = height;
        
        // Generate map
        vertice_data = new VertexData[width * height];
        int index = 0;
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                int x = i + camera_x;
                int z = j + camera_z;
                
                vertice_data[index] = generate_vertex(x, z);

                index++;
            }
        }

        create_mesh();
    }

    VertexData generate_vertex(int x, int z)
    {
        int tileHeight = Mathf.FloorToInt(Mathf.PerlinNoise(x * 0.005f + 100000f, z * 0.005f + 100000f) * 30f);
        int tileData = Mathf.FloorToInt(
            Mathf.Clamp(Mathf.PerlinNoise(x * 0.01f, z * 0.01f) * (int)Tiles.N_TILES, 0, (int)Tiles.N_TILES - 1));

        if (Mathf.Abs(tileHeight - water_level) < 2)
        {
            tileData = (int)Tiles.Sand;
        }
        else if (tileHeight < water_level)
        {
            tileData = (int)Tiles.Dirt;
        }

        return new VertexData(
            tileHeight,
            new Color32(tileColors[tileData].r, tileColors[tileData].g, tileColors[tileData].b, 255)
            );
    }

    public void load_map_from_string(string s)
    {
        int index = 0;

        // Load width and height
        width = (int)parse_float(s, ref index);
        height = (int)parse_float(s, ref index);

        // Load map
        vertice_data = new VertexData[width * height]; 
        for(int i = 0; i < width * height; i++)
        {
            vertice_data[i] = new VertexData(Mathf.FloorToInt(parse_float(s, ref index)),
                    tileColors[Mathf.FloorToInt(parse_float(s, ref index))]);
        }

        // TODO:: Load objects

        create_mesh();
    }

    public void create_mesh()
    {
        // Create world mesh
        if (vertices == null) vertices = new Vector3[width * height];
        if (triangles == null) triangles = new int[(width - 1) * (height - 1) * 6];
        if (colors == null) colors = new Color32[width * height];

        // Create vertices and colors
        int index = 0;
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                vertices[index] = new Vector3(i, vertice_data[index].y, j);
                colors[index] = vertice_data[index].c;
                index++;
            }
        }

        // Create triangles
        index = 0;
        int tri_index = 0;
        for (int j = 0; j < height - 1; j++)
        {
            for (int i = 0; i < width - 1; i++)
            {
                triangles[tri_index + 2] = index;
                triangles[tri_index + 1] = index + 1;
                triangles[tri_index + 0] = index + width + 1;

                triangles[tri_index + 3] = index;
                triangles[tri_index + 4] = index + width;
                triangles[tri_index + 5] = index + width + 1;
                tri_index += 6;
                index += 1;
            }
            index += 1;
        }

        update_mesh();
    }

    public void update_mesh()
    {
        // Create the mesh
        Mesh world_mesh = filter.mesh;
        world_mesh.Clear();
        world_mesh.vertices = vertices;
        world_mesh.triangles = triangles;
        world_mesh.colors32 = colors;
        world_mesh.RecalculateNormals();

        filter.mesh = world_mesh;

        if (collider != null)
        {
            collider.sharedMesh = world_mesh;
        }
    }

    public float parse_float(string s, ref int index)
    {
        int n_values = 0;

        while(index < s.Length && s[index] != ',')
        {
            n_values++;
            index++;
        }
        
        float result = 0f;

        string sub = s.Substring(index - n_values, n_values);
        float.TryParse(sub, out result);
        
        index++;

        return result;
    }
}
