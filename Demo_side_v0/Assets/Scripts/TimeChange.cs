﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using System.Linq;

struct Point2D
{
    public float x;
    public float y;
};

public class TimeChange : MonoBehaviour {
  

    public float[] freshWaterProportions;
    public Vector3[] fishPositions;
    int nActualYear;
    public GameObject freshWater;
    public GameObject playerObject;
    public GameObject fishSchool;
    List<GameObject> listFishSchools;
    Vector3 initialPlayerPosition;
    float xInitialProportion;
    Text dataTimeText;
    Text dataCoordinatesText;
    //GameObject userObject;
    int numberFish = 63;
    public int yearSamples = 5;
    public int startYear = 2005;
    public int yearStep = 2;
    List<Vector2>[] fishPositionsXYear;
    List<int>[] fishNumberXYearInPos;
    float yPos;
    Vector3 vanishPos;

    public BoxCollider seaCollider = null;
    float maxX, minX, maxY, minY;
    float rotationAngle2;
    float RWDiagonalDistance;
    float VRDiagonalDistance;
    Vector2 p1, p2, p3, p4;
    Vector2 Q, U, V;
    float a, b;
    Point2D[] pointsMap;
    Vector2 upperRight;
    Vector2 downLeft;

    public SteamVR_ActionSet movementSet;
    public SteamVR_Action_Boolean clickMove;
    public SteamVR_Action_Vector2 clickAxis;
    public SteamVR_Input_Sources handtype;

    public Vector2 RotateVector(Vector2 v, float angle)
    {
        float radian = angle * Mathf.Deg2Rad;
        float _x = v.x * Mathf.Cos(radian) - v.y * Mathf.Sin(radian);
        float _y = v.x * Mathf.Sin(radian) + v.y * Mathf.Cos(radian);
        return new Vector2(_x, _y);
    }

    bool onSegment(Point2D p, Point2D q, Point2D r)
    {
        if (q.x <= Mathf.Max(p.x, r.x) && q.x >= Mathf.Min(p.x, r.x) &&
                q.y <= Mathf.Max(p.y, r.y) && q.y >= Mathf.Min(p.y, r.y))
            return true;
        return false;
                
    }

    int orientation(Point2D p, Point2D q, Point2D r)
    {
        float val = (q.y - p.y) * (r.x - q.x) -
                  (q.x - p.x) * (r.y - q.y);

        if (val == 0) return 0;  // colinear 
        return (val > 0) ? 1 : 2; // clock or counterclock wise 
    }

    bool doIntersect(Point2D p1, Point2D q1, Point2D p2, Point2D q2)
    {
        // Find the four orientations needed for general and 
        // special cases 
        int o1 = orientation(p1, q1, p2);
        int o2 = orientation(p1, q1, q2);
        int o3 = orientation(p2, q2, p1);
        int o4 = orientation(p2, q2, q1);

        // General case 
        if (o1 != o2 && o3 != o4)
            return true;

        // Special Cases 
        // p1, q1 and p2 are colinear and p2 lies on segment p1q1 
        if (o1 == 0 && onSegment(p1, p2, q1)) return true;

        // p1, q1 and p2 are colinear and q2 lies on segment p1q1 
        if (o2 == 0 && onSegment(p1, q2, q1)) return true;

        // p2, q2 and p1 are colinear and p1 lies on segment p2q2 
        if (o3 == 0 && onSegment(p2, p1, q2)) return true;

        // p2, q2 and q1 are colinear and q1 lies on segment p2q2 
        if (o4 == 0 && onSegment(p2, q1, q2)) return true;

        return false; // Doesn't fall in any of the above cases 
    }

    bool isInside(Point2D[] polygon, int n, Point2D p)
    {
        // There must be at least 3 vertices in polygon[] 
        if (n < 3) return false;

        // Create a point for line segment from p to infinite 
        Point2D extreme;

        extreme.x = 6000f;
        extreme.y = p.y;

        // Count intersections of the above line with sides of polygon 
        int count = 0, i = 0;
        do
        {
            int next = (i + 1) % n;

            // Check if the line segment from 'p' to 'extreme' intersects 
            // with the line segment from 'polygon[i]' to 'polygon[next]' 
            if (doIntersect(polygon[i], polygon[next], p, extreme))
            {
                // If the point 'p' is colinear with line segment 'i-next', 
                // then check if it lies on segment. If it lies, return true, 
                // otherwise false 
                if (orientation(polygon[i], p, polygon[next]) == 0)
                    return onSegment(polygon[i], p, polygon[next]);

                count++;
            }
            i = next;
        } while (i != 0);

        // Return true if count is odd, false otherwise 
        return (count % 2 == 1);  // Same as (count%2 == 1) 
    }

    bool IsInLimits(Vector2 position)
    {

        Vector2 W = position - Q;
        float xabs = Mathf.Abs(Vector2.Dot(W, U));
        float yabs = Mathf.Abs(Vector2.Dot(W, V));

        if(xabs/a + yabs/b <= 1)
        {
            return true;

        }
        else
        {
            return false;
        }

    }

    // Use this for initialization
    void Start () {

        nActualYear = 0;
        dataTimeText = GameObject.Find("Data Time Text").GetComponent<Text>();
        dataTimeText.text = "Year: " + (nActualYear*2 + 2005).ToString();
        initialPlayerPosition = new Vector3(playerObject.transform.position.x, playerObject.transform.position.y,
            playerObject.transform.position.z);
        xInitialProportion = freshWater.transform.localScale.x;
        //fishSchool.transform.position = fishPositions[nActualYear];
        numberFish = fishSchool.GetComponent<SchoolController>()._childAmount;

        listFishSchools = new List<GameObject>();


        p1 = new Vector2(3.92442f, 51.88204f);
        p2 = new Vector2(3.99102f, 51.82542f);

        Vector2 direction = (p2 - p1).normalized;

        float sideMagnitude = (p2 - p1).magnitude;
        Vector2 perpDirection = Vector2.Perpendicular(direction);

        p3 = p1 + perpDirection * sideMagnitude;
        p4 = p2 + perpDirection * sideMagnitude;



        Q = 0.5f * (p2 + p3);


        rotationAngle2 = Vector2.Angle((p4 - p2).normalized, (Q - p2).normalized);


        maxX = -5000;
        minX = 5000;
        maxY = -5000;
        minY = 5000;

        Vector2[] allpoints;

        allpoints = new Vector2[4];

        allpoints[0] = p1;
        allpoints[1] = p2;
        allpoints[2] = p3;
        allpoints[3] = p4;

        for (int i = 0; i < allpoints.Length; i++)
        {
            if (allpoints[i].x > maxX)
            {
                maxX = allpoints[i].x;
            }
            if (allpoints[i].x < minX)
            {
                minX = allpoints[i].x;
            }
            if (allpoints[i].y > maxY)
            {
                maxY = allpoints[i].y;
            }
            if (allpoints[i].y < minY)
            {
                minY = allpoints[i].y;
            }
        }

        RWDiagonalDistance = (p3 - p2).magnitude;

        upperRight = new Vector2(seaCollider.bounds.max.z, seaCollider.bounds.max.x);
        downLeft = new Vector2(seaCollider.bounds.min.z, seaCollider.bounds.min.x);
        VRDiagonalDistance = (upperRight - downLeft).magnitude;

        
        a = 0.5f * (p4 - p1).magnitude;
        b = 0.5f * (p3 - p2).magnitude;

        U = (p4 - p1) / (2 * a);
        V = (p3 - p2) / (2 * b);

        float tx = p1.x;
        float ty = p1.y;

        print(Q.x);




        List<Dictionary<string, object>> dataFish = CSVReader.Read("Clupea_harengus_WMR2");

        Algorithms alg = new Algorithms();

        fishPositionsXYear = new List<Vector2>[yearSamples];
        fishNumberXYearInPos = new List<int>[yearSamples];

        for(int i = 0; i < yearSamples; i++)
        {
            List<Vector2> listFishPos = new List<Vector2>();
            List<int> listFishNumberInPos = new List<int>();

            for (int j = 0; j < dataFish.Count; j++)
            {

                double x = double.Parse(dataFish[j]["x"].ToString());
                double y = double.Parse(dataFish[j]["y"].ToString());




                int n = (int)dataFish[j]["n"];
                string dateExtraction = (string)dataFish[j]["date"];

                Vector2 pos = new Vector2((float)x, (float)y);

                int year = startYear + i * yearStep;

                string yearMonth = year.ToString() + "-05-";

                if (alg.InAreaOfStudy_4Vertices(pos, p1, p2, p3, p4) && dateExtraction.Contains(yearMonth))
                {
                    listFishPos.Add(pos);
                    listFishNumberInPos.Add(n);
                }

 
            }

            int p = 0;

            fishPositionsXYear[i] = listFishPos;
            fishNumberXYearInPos[i] = listFishNumberInPos;



        }



        int max = -1000;

        for(int i = 0; i < fishPositionsXYear.Length; i++)
        {
            if(fishPositionsXYear[i].Count > max)
            {
                max = fishPositionsXYear[i].Count;
            }

        }

        listFishSchools = new List<GameObject>();

        for(int i = 0; i < max; i++)
        {
            GameObject cloneFishSchool = Instantiate(fishSchool, fishSchool.transform.position, fishSchool.transform.rotation) as GameObject;
            listFishSchools.Add(cloneFishSchool);

        }

        yPos = fishSchool.transform.position.y;

        vanishPos = new Vector3(0.0f, -300.0f, 0.0f);
       

        StartCoroutine("WaitRespawn");


    }

    Vector2 ConvertVRtoReal(Vector2 point)
    {

        Vector2 realDir;
        Vector2 virtualDir;
        float rotationAngle;
        Vector2 change;
        Vector2 newPosition;
        //Vector2 VRPos;

        //VRPos = new Vector2(playerObject.transform.position.z, playerObject.transform.position.x);

        virtualDir = (point - downLeft);
        rotationAngle = Vector2.Angle(Vector2.right, virtualDir.normalized);
        realDir = RotateVector((p4-p2).normalized, rotationAngle);
        change = realDir.normalized * virtualDir.magnitude * (RWDiagonalDistance / VRDiagonalDistance);
        newPosition = p2 + change;

        return newPosition;

    }
	
	// Update is called once per frame
	void Update () {



        
        //if (OVRInput.GetUp(OVRInput.Button.Three) || Input.GetKeyDown(KeyCode.DownArrow))
        if (clickMove.GetLastStateDown(handtype) && clickAxis.GetLastAxis(handtype).y < 0)
            {
            
            if(nActualYear > 0)
            {

                nActualYear--;
                dataTimeText.text = "Year: " + (nActualYear*2 + startYear).ToString();
                playerObject.transform.position = initialPlayerPosition;
                freshWater.transform.localScale = new Vector3(xInitialProportion * freshWaterProportions[nActualYear],
                    freshWater.transform.localScale.y, freshWater.transform.localScale.z);

                StartCoroutine("WaitRespawn");

                

            }

        }

        //if (OVRInput.GetUp(OVRInput.Button.Four) || Input.GetKeyDown(KeyCode.UpArrow))
        if (clickMove.GetLastStateDown(handtype) && clickAxis.GetLastAxis(handtype).y > 0)
            {
            if (nActualYear < yearSamples - 1)
            {


                nActualYear++;
                dataTimeText.text = "Year: " + (nActualYear*2 + startYear).ToString();
                playerObject.transform.position = initialPlayerPosition;
                freshWater.transform.localScale = new Vector3(xInitialProportion * freshWaterProportions[nActualYear],
                    freshWater.transform.localScale.y, freshWater.transform.localScale.z);

                StartCoroutine("WaitRespawn");

                //fishSchool.GetComponent<SchoolController>()._childAmount = 63;
                //fishSchool.GetComponent<SchoolController>().AutoRandomWaypointPosition();

            }
            
        }

        //dataCoordinatesText.text = "Coordinates: " + System.Math.Round(userObject.transform.position.x, 1).ToString() + ", " +
        //            System.Math.Round(userObject.transform.position.y, 1) + ", " +
        //            System.Math.Round(userObject.transform.position.z, 1);




    }

    IEnumerator WaitRespawn()
    {
        Vector2 realDir;
        Vector2 virtualDir;
        float rotationAngle;
        Vector2 change;
        Vector2 newPosition;



        for (int i = 0; i < listFishSchools.Count; i++)
        {

            if(i< fishPositionsXYear[nActualYear].Count)
            {
                Vector2 pos = fishPositionsXYear[nActualYear].ElementAt<Vector2>(i);
                realDir = (pos - p2);
                rotationAngle = Vector2.Angle((p4 - p2).normalized, realDir.normalized);
                virtualDir = RotateVector(Vector2.right, rotationAngle);
                change = virtualDir.normalized * realDir.magnitude * (VRDiagonalDistance / RWDiagonalDistance);
                newPosition = downLeft + change;
                listFishSchools.ElementAt<GameObject>(i).GetComponent<SchoolController>()._childAmount = 1;
                listFishSchools.ElementAt<GameObject>(i).GetComponent<SchoolController>().Respawn();

                listFishSchools.ElementAt<GameObject>(i).transform.position = new Vector3(newPosition.y, fishSchool.transform.position.y, newPosition.x);

                for (int j = 0; j < numberFish; j++)
                {

                    yield return new WaitForSeconds(.001f);
                }

                listFishSchools.ElementAt<GameObject>(i).GetComponent<SchoolController>()._childAmount = fishNumberXYearInPos[nActualYear].ElementAt<int>(i); ;
                listFishSchools.ElementAt<GameObject>(i).GetComponent<SchoolController>().Respawn();
            }
            else
            {
                listFishSchools.ElementAt<GameObject>(i).GetComponent<SchoolController>()._childAmount = 1;
                listFishSchools.ElementAt<GameObject>(i).GetComponent<SchoolController>().Respawn();

                listFishSchools.ElementAt<GameObject>(i).transform.position = vanishPos;

                for (int j = 0; j < numberFish; j++)
                {

                    yield return new WaitForSeconds(.001f);
                }


            }

            

            

        }



    }
}
