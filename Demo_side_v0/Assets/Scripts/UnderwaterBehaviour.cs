using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderwaterBehaviour : MonoBehaviour
{
    public GameObject userObject;
    public GameObject waterObject;
    public Material skyboxOverWater;
    public Material skyboxUnderWater;
    bool isUnderwater;
    float waterlevel;
    Color normalColor;
    Color underwaterColor;

    // Start is called before the first frame update
    void Start()
    {
        normalColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        underwaterColor = new Color(0.22f, 0.65f, 0.77f, 0.5f);
        isUnderwater = true;
        waterlevel = waterObject.GetComponent<BoxCollider>().bounds.max.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (userObject.transform.position.y < waterlevel)
        {
            SetUnderwater();

        }
        else
        {
            SetNormal();
        }


    }

    void SetNormal()
    {
        //RenderSettings.fog = false;
        RenderSettings.skybox = skyboxOverWater;
        RenderSettings.fogColor = normalColor;
        RenderSettings.fogDensity = 0.001f;
    }

    void SetUnderwater()
    {
        RenderSettings.fog = true;
        RenderSettings.skybox = skyboxUnderWater;
        RenderSettings.fogColor = underwaterColor;
        RenderSettings.fogDensity = 0.0025f;
        RenderSettings.fogStartDistance *= 10;

    }
}
