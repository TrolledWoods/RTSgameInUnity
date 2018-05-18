using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Tiles
{
    public abstract class TileTemplate
    {
        GameObject[] Prefabs;
        public TerrainRequirements Requirements;

        public TileTemplate(GameObject[] prefabs, TerrainRequirements requirements)
        {
            this.Prefabs = prefabs;
            this.Requirements = requirements;
        }

        public Tile CreateInstance(GameObject parent, Vector3 position)
        {
            GameObject prefab = Prefabs[Mathf.FloorToInt(UnityEngine.Random.Range(0, Prefabs.Length - 0.01f))];

            GameObject visual = new GameObject("TileParent");
            visual.transform.parent = parent.transform;

            GameObject obj = new GameObject("ChildEntity");
            obj.transform.parent = visual.transform;
            obj.transform.position = position;
            MeshFilter filter = obj.AddComponent<MeshFilter>();
            MeshRenderer renderer = obj.AddComponent<MeshRenderer>();
            filter.mesh = prefab.GetComponent<MeshFilter>().sharedMesh;
            renderer.material = prefab.GetComponent<MeshRenderer>().sharedMaterial;

            return new Tile(visual, null);
        }
    }
}
