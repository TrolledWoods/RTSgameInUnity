using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceLoader : MonoBehaviour {

    public static ResourceLoader resources;

    // Buildings
    public GameObject[] Castles;

    // Resources
    public GameObject[] Grass;
    public GameObject[] Trees;
    
    // Easter eggs
    public GameObject[] SandCastles;

	// Use this for initialization
	void Awake () {
        if (resources == null)
            resources = this;
	}
}
