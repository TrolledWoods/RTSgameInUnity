using System;
using UnityEngine;

namespace Assets.Scripts.RenderPipeline
{
    public class WorldPallette : RenderingPipeline
    {
        World world;
        Color32[] pallette;

        public WorldPallette(World world, Color32[] pallette) : 
            base(world.Vertex_Width, world.Vertex_Height)
        {
            this.world = world;
            this.pallette = pallette;
        }

        public override VisualVertex GetVertex(int index)
        {
            return new VisualVertex(
                world.Vertices[index].Height, 
                pallette[(int)world.Vertices[index].Type.Peek()]);
        }
    }
}
