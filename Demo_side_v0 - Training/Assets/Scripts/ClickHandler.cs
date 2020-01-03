using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickHandler : MonoBehaviour
{
    public Material selectedMaterial;
    public Material unselectedMaterial;
    public GameObject player;

    public Camera camera;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                if (hit.collider.gameObject.name.Contains("interest"))
                {
                    player.transform.position = GetComponent<PositionTrack>().GetPositionFromMap(hit.collider.gameObject.transform.localPosition);

                }
       
            }
        }

        

    }
}
