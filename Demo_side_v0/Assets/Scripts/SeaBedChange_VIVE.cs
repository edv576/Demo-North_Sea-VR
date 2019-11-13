using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

public class SeaBedChange_VIVE : MonoBehaviour {

    public Mesh[] seaBeds;
    int nActualSeaBed;
    public GameObject seaBed;
    public int initialYear;
    Text dataTimeText;
    Text dataCoordinatesText;
    public GameObject userObject;

    public SteamVR_ActionSet movementSet;
    public SteamVR_Action_Boolean clickMove;
    public SteamVR_Action_Vector2 clickAxis;
    public SteamVR_Input_Sources handtype;

    // Use this for initialization
    void Start () {

        nActualSeaBed = 0;
        dataTimeText = GameObject.Find("Data Time Text").GetComponent<Text>();
        //dataCoordinatesText = GameObject.Find("Data Coordinates Text").GetComponent<Text>();
        //userObject = GameObject.Find("OVRPlayerController");
        dataTimeText.text = "Year: " + (nActualSeaBed*2 + initialYear).ToString();
        //dataCoordinatesText.text = "Coordinates: " + System.Math.Round(userObject.transform.position.x,2).ToString() + ", " +
        //    System.Math.Round(userObject.transform.position.y, 2) + ", " +
        //    System.Math.Round(userObject.transform.position.z, 2);

        



    }
	
	// Update is called once per frame
	void Update () {

        if (clickMove.GetLastStateDown(handtype) && clickAxis.GetLastAxis(handtype).y < 0)
        {
            
            if(nActualSeaBed > 0)
            {
                nActualSeaBed--;
                seaBed.GetComponent<MeshFilter>().mesh = seaBeds[nActualSeaBed];
                dataTimeText.text = "Year: " + (nActualSeaBed*2 + initialYear).ToString();
                
            }

        }

        if (clickMove.GetLastStateDown(handtype) && clickAxis.GetLastAxis(handtype).y > 0)
        {
            if (nActualSeaBed < seaBeds.Length - 1)
            {
                nActualSeaBed++;
                seaBed.GetComponent<MeshFilter>().mesh = seaBeds[nActualSeaBed];
                dataTimeText.text = "Year: " + (nActualSeaBed*2 + initialYear).ToString();

            }

        }

        //dataCoordinatesText.text = "Coordinates: " + System.Math.Round(userObject.transform.position.x, 1).ToString() + ", " +
        //            System.Math.Round(userObject.transform.position.y, 1) + ", " +
        //            System.Math.Round(userObject.transform.position.z, 1);




    }
}
