using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMovement : MonoBehaviour {

    public GameObject player;
    public Camera playerCamera;
    public GameObject rController;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        //OVRInput.Update();

        if (OVRInput.Get(OVRInput.Button.One))
        {

            //gameObject.transform.position += new Vector3(0.0f, 0.0f, 5.0f);
            //Vector3 rControllerVelocity;

            //rControllerVelocity = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);

        

            //player.transform.position += new Vector3(0.0f, 0.0f, 0.03f);

            player.transform.position += rController.transform.forward * Time.deltaTime * 0.3f;

            //if (gameObject.activeInHierarchy)
            //{
            //    gameObject.SetActive(false);

            //}
            //else
            //{
            //    gameObject.SetActive(true);

            //}
        }

        if (OVRInput.Get(OVRInput.Button.Two))
        {
            player.transform.position += rController.transform.forward * Time.deltaTime * 2.0f;
        }





        }
}
