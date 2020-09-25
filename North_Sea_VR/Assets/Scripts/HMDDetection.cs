using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;


//This script detects if the user is using a HMD or not. It adapts the application accordingly to Flat Screens or VR use.
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

    public bool IsHMDActive()
    {
        return XRDevice.isPresent;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
