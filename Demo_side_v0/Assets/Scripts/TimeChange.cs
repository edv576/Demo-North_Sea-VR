using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeChange : MonoBehaviour {

    public float[] freshWaterProportions;
    public Vector3[] fishPositions;
    int nActualProportion;
    public GameObject freshWater;
    public GameObject playerObject;
    public GameObject fishSchool;
    Vector3 initialPlayerPosition;
    float xInitialProportion;
    Text dataTimeText;
    Text dataCoordinatesText;
    GameObject userObject;
    int numberFish = 63;
    
    // Use this for initialization
    void Start () {

        nActualProportion = 0;
        dataTimeText = GameObject.Find("Data Time Text").GetComponent<Text>();
        //dataCoordinatesText = GameObject.Find("Data Coordinates Text").GetComponent<Text>();
        userObject = GameObject.Find("OVRPlayerController");
        dataTimeText.text = "Year: " + (nActualProportion + 2010).ToString();
        initialPlayerPosition = new Vector3(playerObject.transform.position.x, playerObject.transform.position.y,
            playerObject.transform.position.z);
        xInitialProportion = freshWater.transform.localScale.x;
        fishSchool.transform.position = fishPositions[nActualProportion];
        numberFish = fishSchool.GetComponent<SchoolController>()._childAmount;
        //dataCoordinatesText.text = "Coordinates: " + System.Math.Round(userObject.transform.position.x,2).ToString() + ", " +
        //    System.Math.Round(userObject.transform.position.y, 2) + ", " +
        //    System.Math.Round(userObject.transform.position.z, 2);

        



    }
	
	// Update is called once per frame
	void Update () {

        if (OVRInput.GetUp(OVRInput.Button.Three) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            
            if(nActualProportion > 0)
            {
                nActualProportion--;
                dataTimeText.text = "Year: " + (nActualProportion + 2010).ToString();
                playerObject.transform.position = initialPlayerPosition;
                freshWater.transform.localScale = new Vector3(xInitialProportion * freshWaterProportions[nActualProportion],
                    freshWater.transform.localScale.y, freshWater.transform.localScale.z);

                StartCoroutine("WaitRespawn");

                //fishSchool.GetComponent<SchoolController>()._childAmount = 63;
                //fishSchool.GetComponent<SchoolController>().AutoRandomWaypointPosition();


            }

        }

        if (OVRInput.GetUp(OVRInput.Button.Four) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (nActualProportion < freshWaterProportions.Length - 1)
            {
                nActualProportion++;
                dataTimeText.text = "Year: " + (nActualProportion + 2010).ToString();
                playerObject.transform.position = initialPlayerPosition;
                freshWater.transform.localScale = new Vector3(xInitialProportion * freshWaterProportions[nActualProportion],
                    freshWater.transform.localScale.y, freshWater.transform.localScale.z);

                StartCoroutine("WaitRespawn");

            }

        }

        //dataCoordinatesText.text = "Coordinates: " + System.Math.Round(userObject.transform.position.x, 1).ToString() + ", " +
        //            System.Math.Round(userObject.transform.position.y, 1) + ", " +
        //            System.Math.Round(userObject.transform.position.z, 1);




    }

    IEnumerator WaitRespawn()
    {
        fishSchool.GetComponent<SchoolController>()._childAmount = 0;
        fishSchool.GetComponent<SchoolController>().Respawn();
        fishSchool.transform.position = fishPositions[nActualProportion];
        for (int i = 0; i < numberFish; i++)
        {

            yield return new WaitForSeconds(.001f);
        }
        fishSchool.GetComponent<SchoolController>()._childAmount = numberFish;
        fishSchool.GetComponent<SchoolController>().Respawn();

    }
}
