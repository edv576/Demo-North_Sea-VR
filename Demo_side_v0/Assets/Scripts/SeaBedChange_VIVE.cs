using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

public class SeaBedChange_VIVE : MonoBehaviour {

    public Mesh[] seaBeds;
    public Sprite[] seaBedMaps;
    int nActualSeaBed;
    public GameObject seaBed;
    public int initialYear;
    public Text dataTimeTextVR;
    public Text dataTimeTextFS;
    public Image imageVR;
    public Image imageFS;
    Text dataCoordinatesText;
    public GameObject userObject;
    public GameObject fishMarkerVR;
    public GameObject fishMarkerFS;
    private List<GameObject> fishMarkersVR;
    private List<GameObject> fishMarkersFS;

    public SteamVR_ActionSet movementSet;
    public SteamVR_Action_Boolean clickMove;
    public SteamVR_Action_Vector2 clickAxis;
    public SteamVR_Input_Sources handtype;

    public void ResetFishMarkers()
    {
        if(fishMarkersVR.Count > 0)
        {
            for(int i = 0; i < fishMarkersVR.Count; i++)
            {
                Destroy(fishMarkersVR[i]);
                Destroy(fishMarkersFS[i]);
            }

            fishMarkersVR = new List<GameObject>();
            fishMarkersFS = new List<GameObject>();
        }

    }

    public void AddFishMarker(Vector2 posFishInMap)
    {

        GameObject newFishMarkerVR = Instantiate(fishMarkerVR, new Vector3(posFishInMap.x, posFishInMap.y, fishMarkerVR.transform.localPosition.z), 
            fishMarkerVR.transform.rotation);
        GameObject newFishMarkerFS = Instantiate(fishMarkerFS, new Vector3(posFishInMap.x, posFishInMap.y, fishMarkerFS.transform.localPosition.z),
            fishMarkerFS.transform.rotation);

        fishMarkersVR.Add(newFishMarkerVR);
        fishMarkersFS.Add(newFishMarkerFS);
    }

    // Use this for initialization
    void Start () {

        nActualSeaBed = 0;
        //dataTimeText = GameObject.Find("Data Time Text").GetComponent<Text>();
        //dataCoordinatesText = GameObject.Find("Data Coordinates Text").GetComponent<Text>();
        //userObject = GameObject.Find("OVRPlayerController");
        dataTimeTextVR.text = "Year: " + (nActualSeaBed*2 + initialYear).ToString();
        dataTimeTextFS.text = "Year: " + (nActualSeaBed * 2 + initialYear).ToString();
        fishMarkersVR = new List<GameObject>();
        fishMarkersFS = new List<GameObject>();
        //dataCoordinatesText.text = "Coordinates: " + System.Math.Round(userObject.transform.position.x,2).ToString() + ", " +
        //    System.Math.Round(userObject.transform.position.y, 2) + ", " +
        //    System.Math.Round(userObject.transform.position.z, 2);





    }
	
	// Update is called once per frame
	void Update () {

        if ((clickMove.GetLastStateDown(handtype) && clickAxis.GetLastAxis(handtype).y < 0) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            
            if(nActualSeaBed > 0)
            {
                nActualSeaBed--;
                seaBed.GetComponent<MeshFilter>().mesh = seaBeds[nActualSeaBed];
                imageVR.sprite = seaBedMaps[nActualSeaBed];
                imageFS.sprite = seaBedMaps[nActualSeaBed];
                dataTimeTextVR.text = "Year: " + (nActualSeaBed*2 + initialYear).ToString();
                dataTimeTextFS.text = "Year: " + (nActualSeaBed * 2 + initialYear).ToString();
            }

        }

        if ((clickMove.GetLastStateDown(handtype) && clickAxis.GetLastAxis(handtype).y > 0) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (nActualSeaBed < seaBeds.Length - 1)
            {
                nActualSeaBed++;
                seaBed.GetComponent<MeshFilter>().mesh = seaBeds[nActualSeaBed];
                imageVR.sprite = seaBedMaps[nActualSeaBed];
                imageFS.sprite = seaBedMaps[nActualSeaBed];
                dataTimeTextVR.text = "Year: " + (nActualSeaBed*2 + initialYear).ToString();
                dataTimeTextFS.text = "Year: " + (nActualSeaBed * 2 + initialYear).ToString();
            }

        }

        //dataCoordinatesText.text = "Coordinates: " + System.Math.Round(userObject.transform.position.x, 1).ToString() + ", " +
        //            System.Math.Round(userObject.transform.position.y, 1) + ", " +
        //            System.Math.Round(userObject.transform.position.z, 1);




    }
}
