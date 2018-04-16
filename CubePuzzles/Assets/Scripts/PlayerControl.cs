using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PlayerControl : MonoBehaviour {

    public GameObject placing;

    public MapLoader map;
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

            if (Input.GetMouseButtonDown(2))
            {
                int place_x = Mathf.FloorToInt(selection_graphic.transform.position.x);
                int place_z = Mathf.FloorToInt(selection_graphic.transform.position.z);
                int place_y = map.get_data(place_x, place_z).y;

                GameObject newObj = (GameObject)Instantiate(placing,
                    new Vector3(place_x, place_y, place_z), 
                    Quaternion.identity);
            }
            if (Input.GetMouseButton(0))
            {
                if(pHitPoint != null)
                {
                    // Camera movement
                    float y = transform.position.y;
                    transform.position += (pHitPoint - hit.point) / 2f;
                    transform.position = new Vector3(transform.position.x, y, transform.position.z);

                    // Move world to fit camera
                    float left = transform.position.x - boundary_width;
                    float right = transform.position.x + boundary_width;
                    float bottom = transform.position.z - boundary_height;
                    float top = transform.position.z + boundary_height;

                    // Check if boundaries are outside the world'
                    int dx = 0;
                    int dz = 0;
                    if (left < map.transform.position.x)
                        dx -= step_size;
                    if (right > map.transform.position.x + map.width)
                        dx += step_size;
                    if (bottom < map.transform.position.z)
                        dz -= step_size;
                    if (top > map.transform.position.z + map.height)
                        dz += step_size;

                    // Check if the boundary moved, and if it did, translate map
                    if(dx != 0 || dz != 0)
                    {
                        map.translate_map(dx, dz);
                    }
                }
            }

            pHitPoint = hit.point;
        }

        Debug.DrawRay(transform.position, ray.direction * distance, Color.white);
    }
}
