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
    public GameObject dataDisplayVR;
    public GameObject dataDisplayFS;

    public SteamVR_ActionSet movementSet;
    public SteamVR_Action_Boolean clickMove;
    public SteamVR_Action_Vector2 clickAxis;
    public SteamVR_Input_Sources handtype;

    //All fish positions in the map are deleted
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

    //Add fish positions in the map
    public void AddFishMarker(Vector2 posFishInMap)
    {

        GameObject newFishMarkerVR = Instantiate(fishMarkerVR, fishMarkerVR.transform.position, fishMarkerVR.transform.rotation);
        GameObject newFishMarkerFS = Instantiate(fishMarkerFS, fishMarkerFS.transform.position, fishMarkerFS.transform.rotation);

        newFishMarkerVR.SetActive(true);
        newFishMarkerFS.SetActive(true);

        newFishMarkerVR.transform.SetParent(dataDisplayVR.transform);
        newFishMarkerVR.transform.localScale = fishMarkerVR.transform.localScale;
        newFishMarkerFS.transform.SetParent(dataDisplayFS.transform);
        newFishMarkerFS.transform.localScale = fishMarkerFS.transform.localScale;

        newFishMarkerVR.transform.localPosition = new Vector3(posFishInMap.x, posFishInMap.y, fishMarkerVR.transform.localPosition.z);
        newFishMarkerFS.transform.localPosition = new Vector3(posFishInMap.x, posFishInMap.y, fishMarkerFS.transform.localPosition.z);



        fishMarkersVR.Add(newFishMarkerVR);
        fishMarkersFS.Add(newFishMarkerFS);
    }

    private void Awake()
    {
        fishMarkersVR = new List<GameObject>();
        fishMarkersFS = new List<GameObject>();
    }

    // Use this for initialization
    void Start () {

        nActualSeaBed = 0;

        dataTimeTextVR.text = "Year: " + (GetComponent<TimeChange>().years[nActualSeaBed]).ToString();       
        dataTimeTextFS.text = "Year: " + (GetComponent<TimeChange>().years[nActualSeaBed]).ToString();







    }
	
	// Update is called once per frame
	void Update () {

        //Change seabed using VR
        if ((clickMove.GetLastStateDown(handtype) && clickAxis.GetLastAxis(handtype).y < 0) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            
            if(nActualSeaBed > 0)
            {
                nActualSeaBed--;
                seaBed.GetComponent<MeshFilter>().mesh = seaBeds[nActualSeaBed];
                imageVR.sprite = seaBedMaps[nActualSeaBed];
                imageFS.sprite = seaBedMaps[nActualSeaBed];
                dataTimeTextVR.text = "Year: " + (GetComponent<TimeChange>().years[nActualSeaBed]).ToString();
                dataTimeTextFS.text = "Year: " + (GetComponent<TimeChange>().years[nActualSeaBed]).ToString();
            }

        }

        //Change seabed using Flat Screens
        if ((clickMove.GetLastStateDown(handtype) && clickAxis.GetLastAxis(handtype).y > 0) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (nActualSeaBed < seaBeds.Length - 1)
            {
                nActualSeaBed++;
                seaBed.GetComponent<MeshFilter>().mesh = seaBeds[nActualSeaBed];
                imageVR.sprite = seaBedMaps[nActualSeaBed];
                imageFS.sprite = seaBedMaps[nActualSeaBed];
                dataTimeTextVR.text = "Year: " + (GetComponent<TimeChange>().years[nActualSeaBed]).ToString();
                dataTimeTextFS.text = "Year: " + (GetComponent<TimeChange>().years[nActualSeaBed]).ToString();
            }

        }






    }
}
