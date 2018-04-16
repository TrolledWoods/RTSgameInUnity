using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PlayerControl : MonoBehaviour {
    
    public GameObject selection_graphic;
    MeshRenderer selection_renderer;
    Camera cam;

    Vector3 pHitPoint;

    Vector3 focus_point;

    int default_layer;

    public float boundary_width = 20f;
    public float boundary_height = 20f;

    public int step_size = 15;

	// Use this for initialization
	void Start () {
        cam = GetComponent<Camera>();
        focus_point = transform.position;

        selection_renderer = selection_graphic.GetComponent<MeshRenderer>();

        default_layer = (LayerMask.NameToLayer("Default") + 1) << 8;
	}
	
	// Update is called once per frame
	void Update () {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        float distance = 100;

        if(Physics.Raycast(ray, out hit, 1000, default_layer))
        {
            selection_graphic.transform.position = hit.point + Vector3.up * 6;
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
        }

        Debug.DrawRay(transform.position, ray.direction * distance, Color.white);
    }
}
