using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;
using Assets.Scripts.WorldGenerators;
using Assets.Scripts.Entities;

public class GameController : MonoBehaviour {

    public Material world_material;

    public World w;
    new TerrainRenderer renderer;
    
    World.Vertex v;

    QuadTree tree;

    void Start()
    {
        w = new World(gameObject, 200, 200,
            new Classic(0.03f, 30f, 0.005f, 20));
        renderer = new TerrainRenderer(
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
        //w.Entities.DebugRenderer(Vector3.up * 50);
    }

}
