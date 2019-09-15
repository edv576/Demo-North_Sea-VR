using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserMovement : MonoBehaviour {

    public GameObject directionController;
    public GameObject playerObject;
   

    // Use this for initialization
    void Start () {



        

        //seaBeds = new Mesh[ns];

        //seaBeds.
		
	}
	
	// Update is called once per frame
	void Update () {

        if (OVRInput.Get(OVRInput.Button.One))
        {

            playerObject.transform.position += directionController.transform.forward * Time.deltaTime * 3.0f;

          

        }

        if (OVRInput.Get(OVRInput.Button.Two))
        {
            playerObject.transform.position += directionController.transform.forward * Time.deltaTime * 20.0f;
        }


    }
}
