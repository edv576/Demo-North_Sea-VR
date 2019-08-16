using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public float speed = 25.0f;
    GameObject rightController;
    GameObject leftController;
    int n = 0;

	// Use this for initialization
	void Start () {
        rightController = GameObject.Find("RightControllerAnchor");
        leftController = GameObject.Find("LeftControllerAnchor");

	}
	
	// Update is called once per frame
	void Update () {

        if (OVRInput.Get(OVRInput.Button.SecondaryThumbstick))
        {
            gameObject.transform.position += rightController.transform.forward * Time.deltaTime * speed;

            n++;

        }


        Debug.Log(n);
		
	}
}
