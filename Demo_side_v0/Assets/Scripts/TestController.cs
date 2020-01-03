using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestController : MonoBehaviour
{
    public GameObject[] pointsInterestVR;
    public GameObject[] cutsInterestVR;
    public GameObject[] pointsInterestFS;
    public GameObject[] cutsInterestFS;
    public GameObject[] genericPointsInterestVR;
    public GameObject[] genericPointsInterestFS;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    void SetGenericPointsInterestActive(bool status)
    {
        for(int i = 0; i < genericPointsInterestFS.Length; i++)
        {
            genericPointsInterestVR[i].SetActive(status);
            genericPointsInterestFS[i].SetActive(status);
         
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.B))
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SetGenericPointsInterestActive(false);
                for (int i = 0; i < pointsInterestFS.Length; i++)
                {
                    pointsInterestFS[i].SetActive(false);
                    cutsInterestFS[i].SetActive(false);
                    pointsInterestVR[i].SetActive(false);
                    cutsInterestVR[i].SetActive(false);
                }
                if (GetComponent<HMDDetection>().IsHMDActive())
                {
                    pointsInterestVR[0].SetActive(true);
                }
                else
                {
                    pointsInterestFS[0].SetActive(true);
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SetGenericPointsInterestActive(false);
                for (int i = 0; i < pointsInterestFS.Length; i++)
                {
                    pointsInterestFS[i].SetActive(false);
                    cutsInterestFS[i].SetActive(false);
                    pointsInterestVR[i].SetActive(false);
                    cutsInterestVR[i].SetActive(false);
                }
                if (GetComponent<HMDDetection>().IsHMDActive())
                {
                    pointsInterestVR[1].SetActive(true);
                }
                else
                {
                    pointsInterestFS[1].SetActive(true);
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SetGenericPointsInterestActive(false);
                for (int i = 0; i < pointsInterestFS.Length; i++)
                {
                    pointsInterestFS[i].SetActive(false);
                    cutsInterestFS[i].SetActive(false);
                    pointsInterestVR[i].SetActive(false);
                    cutsInterestVR[i].SetActive(false);
                }
                if (GetComponent<HMDDetection>().IsHMDActive())
                {
                    pointsInterestVR[2].SetActive(true);
                }
                else
                {
                    pointsInterestFS[2].SetActive(true);
                }
            }
        }

        if (Input.GetKey(KeyCode.Y))
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SetGenericPointsInterestActive(false);
                for (int i = 0; i < pointsInterestFS.Length; i++)
                {
                    pointsInterestFS[i].SetActive(false);
                    cutsInterestFS[i].SetActive(false);
                    pointsInterestVR[i].SetActive(false);
                    cutsInterestVR[i].SetActive(false);
                }
                if (GetComponent<HMDDetection>().IsHMDActive())
                {
                    cutsInterestVR[0].SetActive(true);
                }
                else
                {
                    cutsInterestFS[0].SetActive(true);
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SetGenericPointsInterestActive(false);
                for (int i = 0; i < pointsInterestFS.Length; i++)
                {
                    pointsInterestFS[i].SetActive(false);
                    cutsInterestFS[i].SetActive(false);
                    pointsInterestVR[i].SetActive(false);
                    cutsInterestVR[i].SetActive(false);
                }
                if (GetComponent<HMDDetection>().IsHMDActive())
                {
                    cutsInterestVR[1].SetActive(true);
                }
                else
                {
                    cutsInterestFS[1].SetActive(true);
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SetGenericPointsInterestActive(false);
                for (int i = 0; i < pointsInterestFS.Length; i++)
                {
                    pointsInterestFS[i].SetActive(false);
                    cutsInterestFS[i].SetActive(false);
                    pointsInterestVR[i].SetActive(false);
                    cutsInterestVR[i].SetActive(false);
                }
                if (GetComponent<HMDDetection>().IsHMDActive())
                {
                    cutsInterestVR[2].SetActive(true);
                }
                else
                {
                    cutsInterestFS[2].SetActive(true);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            SetGenericPointsInterestActive(true);
            for (int i = 0; i < pointsInterestFS.Length; i++)
            {
                pointsInterestFS[i].SetActive(false);
                cutsInterestFS[i].SetActive(false);
                pointsInterestVR[i].SetActive(false);
                cutsInterestVR[i].SetActive(false);
            }
        }
    }
}
