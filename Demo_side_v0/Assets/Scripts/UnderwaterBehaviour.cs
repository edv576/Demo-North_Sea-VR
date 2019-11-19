using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderwaterBehaviour : MonoBehaviour
{
    public GameObject userObject;
    public GameObject waterObject;
    bool isUnderwater;
    Color normalColor;
    Color underwaterColor;

    // Start is called before the first frame update
    void Start()
    {
        normalColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        underwaterColor = new Color(0.22f, 0.65f, 0.77f, 0.5f);
        isUnderwater = true;
        
    }

    // Update is called once per frame
    void Update()
    {
        if((userObject.transform.position.y < waterObject.transform.position.y) != isUnderwater)
        {
            isUnderwater = userObject.transform.position.y < waterObject.transform.position.y;

            if (isUnderwater) SetUnderwater();
            else SetNormal();

        }

        
    }

    void SetNormal()
    {
        RenderSettings.fogColor = normalColor;
        RenderSettings.fogDensity = 0.002f;
    }

    void SetUnderwater()
    {
        RenderSettings.fogColor = underwaterColor;
        RenderSettings.fogDensity = 0.03f;

    }
}
