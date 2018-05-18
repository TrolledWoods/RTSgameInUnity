using Assets.Scripts;
using Assets.Scripts.RenderPipeline;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PlayerControl : MonoBehaviour {
    
    public GameObject selection_graphic;
    public Material falseMaterial;
    public Material trueMaterial;
    MeshRenderer selection_renderer;
    Camera cam;

    public GameController g;
    World w;

    Vector3 pHitPoint;

    Vector3 focus_point;

    int default_layer;

    public float boundary_width = 20f;
    public float boundary_height = 20f;

    public int step_size = 15;

    TerrainRequirements requirements;
    RequirementRenderer reqRenderer;
    new TerrainRenderer renderer;

	// Use this for initialization
	void Start () {
        cam = GetComponent<Camera>();
        focus_point = transform.position;

        selection_renderer = selection_graphic.GetComponent<MeshRenderer>();

        requirements = new TerrainRequirements(new int[] {
            0, 0, 0, 0, 0, 0, 0, 0, 0
        }, 3, 3);
        reqRenderer = new RequirementRenderer(requirements);
        renderer = new TerrainRenderer(selection_graphic, reqRenderer, falseMaterial);

        default_layer = (LayerMask.NameToLayer("Default") + 1) << 8;
	}
	
	// Update is called once per frame
	void Update () {
        w = g.w;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        float distance = 100;

        if(Physics.Raycast(ray, out hit, 1000, default_layer))
        {
            distance = hit.distance;
            
            if (Input.GetMouseButton(0))
            {
                if(pHitPoint != null)
                {
                    // Camera movement
                    float y = transform.position.y;
                    transform.position += (pHitPoint - hit.point) / 2f;
                    transform.position = new Vector3(transform.position.x, y, transform.position.z);
                }
            }

            pHitPoint = hit.point;

            {
                int x = Mathf.FloorToInt(hit.point.x);
                int z = Mathf.FloorToInt(hit.point.z);

                var result = requirements.ValidateTerrain(w, x, z);

                if (result.Valid)
                {

                    renderer.ChangeMaterial(trueMaterial);
                }
                else
                {
                    renderer.ChangeMaterial(falseMaterial);
                }

                selection_graphic.transform.position = new Vector3(
                    Mathf.Floor(hit.point.x),
                    Mathf.Floor(hit.point.y) + 0.1f + result.Location,
                    Mathf.Floor(hit.point.z));
            }
        }
        
        Debug.DrawRay(transform.position, ray.direction * distance, Color.white);
    }
}
