using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionTrack : MonoBehaviour
{

    public GameObject player;
    public BoxCollider seaCollider = null;
    public RectTransform panelTransform = null;
    public GameObject positionPoint = null;
    public GameObject mainCamera;

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

    Vector2 CWRotation(Vector2 v, float degrees)
    {
        return new Vector2(v.x * Mathf.Cos(degrees) + v.y * Mathf.Sin(degrees), -v.x * Mathf.Sin(degrees) + v.y * Mathf.Cos(degrees));

    }

    Vector2 CCWRotation(Vector2 v, float degrees)
    {
        return new Vector2(v.x * Mathf.Cos(degrees) - v.y * Mathf.Sin(degrees), v.x * Mathf.Sin(degrees) + v.y * Mathf.Cos(degrees));

    }

    public Vector2 RotateVector(Vector2 v, float angle)
    {
        float radian = angle * Mathf.Deg2Rad;
        float _x = v.x * Mathf.Cos(radian) - v.y * Mathf.Sin(radian);
        float _y = v.x * Mathf.Sin(radian) + v.y * Mathf.Cos(radian);
        return new Vector2(_x, _y);
    }


    private void Awake()
    {


    }

    // Start is called before the first frame update
    void Start()
    {

        //Calculating half the size of the seafloor and half the size of the panel
        mapSideLenghtHalf = Mathf.Abs(seaCollider.bounds.min.x);
        double test = seaCollider.bounds.min.z;
        panelSideLengthHalf = panelTransform.rect.width/2;

        //Putting the position point in the correct place in the map
        positionPoint.transform.localPosition = new Vector3(-player.transform.position.z * panelSideLengthHalf / mapSideLenghtHalf, player.transform.position.x * panelSideLengthHalf / mapSideLenghtHalf, positionPoint.transform.localPosition.z);

        float factor = 100000f;

        Vector2 p1 = new Vector2(3.92442f, 51.88204f);
        Vector2 p2 = new Vector2(3.99102f, 51.82542f);

        Vector2 direction = (p2 - p1).normalized;

        float mag = direction.magnitude;

        float x = direction.x;
        float y = direction.y;

        float sideMagnitude = (p2 - p1).magnitude;
        Vector2 perpDirection = Vector2.Perpendicular(direction);
        //perpDirection.Normalize();

        Vector2 p3 = p1 + perpDirection * sideMagnitude;
        Vector2 p4 = p2 + perpDirection * sideMagnitude;

        float angle = Vector2.Angle(Vector2.right, direction);
        float angleRad = angle * Mathf.Deg2Rad;

        Vector2 rotDirection = RotateVector(direction, angle);
        Vector2 t = RotateVector(new Vector2(1, 0), 90);

        //print(rotDirection.x.ToString());
        //print(rotDirection.y.ToString());
        //rotPerpDirection.Normalize();
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
            positionPoint.transform.eulerAngles = new Vector3(positionPoint.transform.eulerAngles.x, positionPoint.transform.eulerAngles.y,
                -mainCamera.transform.eulerAngles.y + 90);


        }






    }
}
