using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionTrack : MonoBehaviour
{

    public GameObject player;
    public BoxCollider seaCollider = null;
    public RectTransform panelTransform = null;
    public GameObject positionPoint = null;

    Mesh mesh;

    float maxX = -50000;
    float minX = 50000;
    float maxY = -50000;
    float minY = 50000;

    float mapSideLenghtHalf;
    float panelSideLengthHalf;

    public Vector3 getPositionFromMap(Vector3 positionInMap)
    {
        return new Vector3(positionInMap.y * mapSideLenghtHalf/panelSideLengthHalf, player.transform.position.y, -positionInMap.x * mapSideLenghtHalf/panelSideLengthHalf);

    }


    // Start is called before the first frame update
    void Start()
    {

        //Calculating half the size of the seafloor and half the size of the panel
        mapSideLenghtHalf = Mathf.Abs(seaCollider.bounds.min.x);
        panelSideLengthHalf = panelTransform.rect.width/2;

        //Putting the position point in the correct place in the map
        positionPoint.transform.localPosition = new Vector3(-player.transform.position.z * panelSideLengthHalf / mapSideLenghtHalf, player.transform.position.x * panelSideLengthHalf / mapSideLenghtHalf, positionPoint.transform.localPosition.z);

    }

    //This function determines if the user is out of bounds of the seafloor or not
    bool OutOfBounds(Vector3 point)
    {

        if ((point.x < -mapSideLenghtHalf) || (point.x > mapSideLenghtHalf))
        {
            return true;
        }

        if ((point.z < -mapSideLenghtHalf) || (point.z > mapSideLenghtHalf))
        {
            return true;
        }

        return false;
    }

    // Update is called once per frame
    void Update()
    {

        //If the user is not out of bounds of the seafloor, update the position in the map
        if (!OutOfBounds(player.transform.position))
        {
            positionPoint.transform.localPosition = new Vector3(-player.transform.position.z * panelSideLengthHalf / mapSideLenghtHalf, player.transform.position.x * panelSideLengthHalf / mapSideLenghtHalf, positionPoint.transform.localPosition.z);

        }






    }
}
