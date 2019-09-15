using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
        {
            gameObject.transform.position += new Vector3(0.0f, 100.0f, 20.0f);

        }
		
	}
}
