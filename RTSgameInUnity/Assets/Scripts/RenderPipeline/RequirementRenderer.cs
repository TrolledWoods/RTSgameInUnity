using System;
using UnityEngine;

namespace Assets.Scripts.RenderPipeline
{
    public class RequirementRenderer : RenderingPipeline
    {
        public TerrainRequirements requirements;

        public RequirementRenderer(TerrainRequirements requirements) :
            base(requirements.Width, requirements.Height)
        {
            this.requirements = requirements;
        }

        public override VisualVertex GetVertex(int index)
        {
            return new VisualVertex(requirements.GetHeight(index),
                new Color32(100, 100, 255, 100));
        }
    }
}
