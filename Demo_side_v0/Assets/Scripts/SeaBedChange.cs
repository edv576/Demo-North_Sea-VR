using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeaBedChange : MonoBehaviour {

    public Mesh[] seaBeds;
    int nActualSeaBed;
    public GameObject seaBed;
    public int initialYear;
    Text dataTimeText;
    Text dataCoordinatesText;
    public GameObject userObject;
    
    // Use this for initialization
    void Start () {

        nActualSeaBed = 0;
        dataTimeText = GameObject.Find("Data Time Text").GetComponent<Text>();
        dataCoordinatesText = GameObject.Find("Data Coordinates Text").GetComponent<Text>();
 
        dataTimeText.text = "Year: " + (nActualSeaBed*2 + initialYear).ToString();


        



    }
	
	// Update is called once per frame
	void Update () {

        if (OVRInput.GetUp(OVRInput.Button.Three))
        {
            
            if(nActualSeaBed > 0)
            {
                nActualSeaBed--;
                seaBed.GetComponent<MeshFilter>().mesh = seaBeds[nActualSeaBed];
                dataTimeText.text = "Year: " + (nActualSeaBed + 2010).ToString();
                
            }

        }

        if (OVRInput.GetUp(OVRInput.Button.Four))
        {
            if (nActualSeaBed < seaBeds.Length - 1)
            {
                nActualSeaBed++;
                seaBed.GetComponent<MeshFilter>().mesh = seaBeds[nActualSeaBed];
                dataTimeText.text = "Year: " + (nActualSeaBed + 2010).ToString();

            }

        }

        dataCoordinatesText.text = "Coordinates: " + System.Math.Round(userObject.transform.position.x, 1).ToString() + ", " +
                    System.Math.Round(userObject.transform.position.y, 1) + ", " +
                    System.Math.Round(userObject.transform.position.z, 1);




    }
}
