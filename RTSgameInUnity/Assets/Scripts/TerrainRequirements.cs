using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    public class TerrainRequirements
    {
        public struct Result
        {
            public bool Valid;
            public int Location;

            public Result(bool valid, int location)
            {
                Valid = valid;
                Location = location;
            }
        }

        int[] requirements;

        int width;
        int height;
        public int Width { get { return width; } }
        public int Height { get { return height; } }

        public TerrainRequirements(int[] requirements, int width, int height)
        {
            this.requirements = requirements;
            this.width = width;
            this.height = height;
        }
        
        public Result ValidateTerrain(World.Vertex[] vertices, int width, int height, int x, int y)
        {
            if (x < 0 || y < 0 || x >= width - this.width || y >= height - this.height)
                return new Result(false, 0);

            int zero_point = vertices[x + y * width].Height - requirements[0];
            int max_delta = 0;
            bool valid = true;

            for (int i = 0; i < this.width; i++)
            {
                for (int j = 0; j < this.height; j++)
                {
                    int delta = vertices[i + x + (j + y) * width].Height - zero_point;
                    if (delta != GetHeight(i, j))
                    {
                        valid = false;
                    }

                    int delta_reality = (GetHeight(i, j) - requirements[0]) - delta;
                    if (delta_reality < max_delta)
                    {
                        max_delta = delta_reality;
                    }
                }
            }

            return new Result(valid, -max_delta);
        }

        public Result ValidateTerrain(World w, int x, int y)
        {
            return ValidateTerrain(w.Vertices, w.Vertex_Width, w.Vertex_Height, x, y);
        }

        public int GetHeight(int x, int y)
        {
            return GetHeight(x + y * width);
        }

        public int GetHeight(int i)
        {
            return requirements[i];
        }
    }
}
