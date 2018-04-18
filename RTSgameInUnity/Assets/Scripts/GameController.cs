﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;
using Assets.Scripts.WorldGenerators;

public class GameController : MonoBehaviour {

    public Material world_material;

    void Start()
    {
        World w = new World(2000, 2000,
            new Classic(0.03f, 18f, 0.01f, 6));
        WorldRenderer renderer = new WorldRenderer(
            this.gameObject, w,
            new Color32[]
            {
                new Color32(120, 72, 0, 255),
                new Color32(1, 142, 14, 255),
                new Color32(194, 178, 128, 255)
            }, world_material);
    }

}
