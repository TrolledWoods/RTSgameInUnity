using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;
using Assets.Scripts.WorldGenerators;

public class GameController : MonoBehaviour {

    public Material world_material;

    World w;
    new WorldRenderer renderer;

    World.Vertex v;

    void Start()
    {
        w = new World(800, 800,
            new Classic(0.03f, 30f, 0.005f, 20));
        renderer = new WorldRenderer(
            this.gameObject, 
            new Assets.Scripts.RenderPipeline.WorldPallette(
            w,
            new Color32[]
            {
                new Color32(120, 72, 0, 255),
                new Color32(1, 142, 14, 255),
                new Color32(194, 178, 128, 255)
            }), world_material);

        v = w.GetVertex(0, 0);
    }

    void Update()
    {

    }

}
