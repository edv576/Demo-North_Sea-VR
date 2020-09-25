using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightChange : MonoBehaviour {

    public Light directionalLight;
    public GameObject water;
    public GameObject player;
    public Vector3 underwaterColor;
    public Vector3 overwaterColor;
    Color usableUnderwaterColor;
    Color usableOverwaterColor;


    Vector3 convertColor(Vector3 rgb)
    {
        return new Vector3(rgb.x / 255.0f, rgb.y / 255.0f, rgb.z / 255.0f);
    }

	// Use this for initialization
	void Start () {

        usableUnderwaterColor = new Color(convertColor(underwaterColor).x, convertColor(underwaterColor).y, 
            convertColor(underwaterColor).z);

        usableOverwaterColor = new Color(convertColor(overwaterColor).x, convertColor(overwaterColor).y,
            convertColor(overwaterColor).z);

    }
	
	// Update is called once per frame
	void Update () {

        if(player.transform.position.y <= water.transform.position.y)
        {

            directionalLight.color = usableUnderwaterColor;

        }
        else
        {
            directionalLight.color = usableOverwaterColor;
        }
        

        
		
	}
}
