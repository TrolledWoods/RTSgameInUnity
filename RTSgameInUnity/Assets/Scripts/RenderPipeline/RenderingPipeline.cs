using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.RenderPipeline
{
    public struct VisualVertex
    {
        public int Height;
        public Color32 Color;

        public VisualVertex(int height, Color32 color)
        {
            Height = height;
            Color = color;
        }
    }

    public abstract class RenderingPipeline
    {
        int width;
        int height;
        public int Width { get { return width; } }
        public int Height { get { return height; } }

        public RenderingPipeline(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        public abstract VisualVertex GetVertex(int index);
        public virtual VisualVertex GetVertex(int x, int y)
        {
            return GetVertex(x + y * width);
        }
    }
}
