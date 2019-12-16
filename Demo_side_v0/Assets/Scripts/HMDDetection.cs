using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HMDDetection : MonoBehaviour
{
    public GameObject VRPanel;
    public GameObject FSPanel;
    // Start is called before the first frame update
    void Start()
    {
        if (XRDevice.isPresent)
        {
            FSPanel.SetActive(false);
            VRPanel.SetActive(true);
        }
        else
        {
            FSPanel.SetActive(true);
            VRPanel.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
