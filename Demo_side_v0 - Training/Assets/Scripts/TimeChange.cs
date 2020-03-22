﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using System.Linq;
using System.Windows;
using CoordinateSharp;
using MIConvexHull;
using g3;
using System;


struct Point2D
{
    public float x;
    public float y;
};

[Serializable]
public struct SalinityPoint
{
    public float x;
    public float y;
    public float salinity;
    public int waterLayer;
    public int year;

};

struct WaterSubdivision
{
    public float x0;
    public float xf;
    public float gradientY0;
    public float gradientYf;
    public bool thereIsData;
    public float layer;
    
};



public class TimeChange : MonoBehaviour {
  

    public float[] freshWaterProportions;
    public Vector3[] fishPositions;
    int nActualYear;   
    public GameObject freshWater;
    public Renderer waterRenderer;
    public GameObject playerObject;
    public GameObject fishSchool;
    public GameObject unitSalinityDivision;
    public int maximumAlpha;
    public int visibilityAlpha;
    public float limitUpFreshWaterValue;
    public float limitDownFreshWaterValue;
    public bool isVisibilityMode;
    List<GameObject> allUnitSalinityDivisions;
    List<GameObject> allUnitSecondarySalinityDivisions;
    List<GameObject> listFishSchools;
    Vector3 initialPlayerPosition;
    Vector3 lastPlayerPosition;
    float xInitialProportion;
    Text dataTimeText;
    Text dataCoordinatesText;

    int numberFish = 63;
    public int yearSamples = 5;
    public int startYear = 2005;
    public int yearStep = 2;
    public int[] years;
    List<Vector2>[] fishPositionsXYear;
    List<int>[] fishNumberXYearInPos;
    List<SalinityPoint>[] salinityPointsXYear;
    List<int>[] salinityIndexesXYearMixDLimit;
    List<int>[] salinityIndexesXYearMixUlimit;
    SalinityPoint[] salinityPoints;
    float yPos;
    Vector3 vanishPos;
    List<double[]> allFreshSalinityPoints;
    bool areSalinityPointsVisible;

    

    public BoxCollider seaFloorCollider = null;
    public BoxCollider seaWaterCollider = null;
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
    public int numberWaterLayers;
    public float deepestLevel;
    public int subdivisions;
    List<WaterSubdivision[,]> listWaterSubdivisionsXYear;
    

    public SteamVR_ActionSet movementSet;
    public SteamVR_Action_Boolean clickMove;
    public SteamVR_Action_Vector2 clickAxis;
    public SteamVR_Action_Boolean clickGrip;
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

    Vector2 ConvertVRtoReal(Vector2 point)
    {

        Vector2 realDir;
        Vector2 virtualDir;
        float rotationAngle;
        Vector2 change;
        Vector2 realPosition;


        virtualDir = (point - downLeft);
        rotationAngle = Vector2.Angle(Vector2.down, virtualDir.normalized);
        realDir = RotateVector((p4 - p2).normalized, rotationAngle);
        change = realDir.normalized * virtualDir.magnitude * (RWDiagonalDistance / VRDiagonalDistance);
        realPosition = p2 + change;

        float rotationAngle2 = Vector2.Angle((p4 - p2).normalized, Vector2.right);

        return realPosition;

    }

    Vector2 ConvertRealtoVR(Vector2 point)
    {

        Vector2 realDir;
        Vector2 virtualDir;
        float rotationAngle;
        Vector2 change;
        Vector2 VRPosition;


        realDir = (point - p2);
        rotationAngle = Vector2.Angle((p4 - p2).normalized, realDir.normalized);
        virtualDir = RotateVector(Vector2.down, rotationAngle);
        change = virtualDir.normalized * realDir.magnitude * (VRDiagonalDistance / RWDiagonalDistance);
        VRPosition = downLeft + change;

        return VRPosition;

    }

    bool IsInSubdivision(WaterSubdivision subdivision, Vector2 point)
    {
        if (point.y <= subdivision.x0 && point.y > subdivision.xf)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    public void AddSecondarySalinityDivision(GameObject secondarySalinityDivision)
    {
        allUnitSecondarySalinityDivisions.Add(secondarySalinityDivision);
    }

    public int NumberSecondarySalinityDivision()
    {
        return allUnitSecondarySalinityDivisions.Count;
    }

    void CreateAllWaterSubdivisions()
    {
        listWaterSubdivisionsXYear = new List<WaterSubdivision[,]>();
        float intervalSubdivision;

        intervalSubdivision = Mathf.Abs(seaFloorCollider.bounds.max.z - seaFloorCollider.bounds.min.z)/subdivisions;

        

        for (int i = 0; i < years.Length; i++)
        {
            WaterSubdivision[,] waterSubdivisionsXLayer = new WaterSubdivision[numberWaterLayers, subdivisions];

            for(int j = 0; j < numberWaterLayers; j++)
            {
                for(int k = 0; k < subdivisions; k++)
                {
                    WaterSubdivision startWaterSubdivision = new WaterSubdivision();
                    startWaterSubdivision.thereIsData = false;
                    startWaterSubdivision.x0 = downLeft.y - k * intervalSubdivision;
                    startWaterSubdivision.xf = downLeft.y - (k + 1) * intervalSubdivision;
                    startWaterSubdivision.gradientY0 = 10000;
                    startWaterSubdivision.gradientYf = 10000;
                    startWaterSubdivision.layer = 0;
                    waterSubdivisionsXLayer[j, k] = startWaterSubdivision;
                }
            }

            Algorithms alg = new Algorithms();
            int matrixLimit = numberWaterLayers * subdivisions;
            int actualCount = 0;

            for(int j = 0; j < salinityIndexesXYearMixDLimit[i].Count; j++)
            {
                for(int k = 0; k < numberWaterLayers; k++)
                {
                    Vector2 VRPoint = ConvertRealtoVR(new Vector2(salinityPoints[salinityIndexesXYearMixDLimit[i][j]].x, salinityPoints[salinityIndexesXYearMixDLimit[i][j]].y));

                    bool gotIt = false;
      

                    for(int l = 0; l < subdivisions; l++)
                    {
                        if((salinityPoints[salinityIndexesXYearMixDLimit[i][j]].waterLayer - 1 == k) && (waterSubdivisionsXLayer[k,l].gradientY0 == 10000) && IsInSubdivision(waterSubdivisionsXLayer[k, l], VRPoint) && alg.InAreaOfStudy_4Vertices(ConvertVRtoReal(VRPoint), p1, p2,p3,p4))
                        {
                            waterSubdivisionsXLayer[k, l].gradientY0 = VRPoint.x;
                            waterSubdivisionsXLayer[k, l].layer = k + 1;
                            actualCount++;
                            gotIt = true;
                            break;
                        }

                    }

                    if (gotIt)
                    {
                        break;
                    }



                }
                if(actualCount == matrixLimit)
                {
                    break;
                }

            }

            actualCount = 0;

            for (int j = 0; j < salinityIndexesXYearMixUlimit[i].Count; j++)
            {
                for (int k = 0; k < numberWaterLayers; k++)
                {
                    Vector2 VRPoint = ConvertRealtoVR(new Vector2(salinityPoints[salinityIndexesXYearMixUlimit[i][j]].x, salinityPoints[salinityIndexesXYearMixUlimit[i][j]].y));

                    bool gotIt = false;

                    for (int l = 0; l < subdivisions; l++)
                    {


                        if ((salinityPoints[salinityIndexesXYearMixUlimit[i][j]].waterLayer - 1 == k) && (waterSubdivisionsXLayer[k, l].gradientYf == 10000) && IsInSubdivision(waterSubdivisionsXLayer[k, l], VRPoint) && alg.InAreaOfStudy_4Vertices(ConvertVRtoReal(VRPoint), p1, p2, p3, p4))
                        {
                            waterSubdivisionsXLayer[k, l].gradientYf = VRPoint.x;
                            waterSubdivisionsXLayer[k, l].layer = k + 1;
                            actualCount++;
                            gotIt = true;
                            break;
                        }

                    }

                    if (gotIt)
                    {
                        break;
                    }



                }

                if (actualCount == matrixLimit)
                {
                    break;
                }

            }

            actualCount = 0;

            for (int j = 0; j < numberWaterLayers; j++)
            {
                for (int k = 0; k < subdivisions; k++)
                {
                    if(waterSubdivisionsXLayer[j, k].gradientYf != 10000)
                    {
                        waterSubdivisionsXLayer[j, k].thereIsData = true;
                        actualCount++;
                    }

                }
            }

            listWaterSubdivisionsXYear.Add(waterSubdivisionsXLayer);



        }

    }

    void CreateSalinityDivisions(int indexYear)
    {
        float initialDepth = seaWaterCollider.bounds.max.y;
        float interval = (initialDepth - unitSalinityDivision.transform.position.y)/10;

        if(allUnitSalinityDivisions.Count > 0)
        {
            for(int i = 0; i < allUnitSalinityDivisions.Count; i++)
            {
                Destroy(allUnitSalinityDivisions[i]);
            }
        }

        if (allUnitSecondarySalinityDivisions.Count > 0)
        {
            for (int i = 0; i < allUnitSecondarySalinityDivisions.Count; i++)
            {
                Destroy(allUnitSecondarySalinityDivisions[i]);
            }
        }



        allUnitSalinityDivisions = new List<GameObject>();
        allUnitSecondarySalinityDivisions = new List<GameObject>();
        allFreshSalinityPoints = new List<double[]>();
        List<DefaultVertex> listConvexHullSalinityV = new List<DefaultVertex>();
        List<DefaultConvexFace<DefaultVertex>> listConvexHullSalinityF = new List<DefaultConvexFace<DefaultVertex>>();

        Algorithms alg = new Algorithms();



        



        for (int i=0;i< salinityIndexesXYearMixUlimit[indexYear].Count; i++)
        {
            Vector2 realPoint = new Vector2(salinityPoints[salinityIndexesXYearMixUlimit[indexYear][i]].x, salinityPoints[salinityIndexesXYearMixUlimit[indexYear][i]].y);
            Vector2 VRPoint = ConvertRealtoVR(realPoint);



        
            if (alg.InAreaOfStudy_4Vertices(realPoint, p1, p2, p3, p4))
                {
                Vector3 unitSalinityDivisionPos = new Vector3(VRPoint.x,
                        initialDepth - interval * (salinityPoints[salinityIndexesXYearMixUlimit[indexYear][i]].waterLayer - 1), VRPoint.y);

                GameObject cloneUnitySalinityDivision = Instantiate(unitSalinityDivision, unitSalinityDivisionPos,
                            unitSalinityDivision.transform.rotation);



                cloneUnitySalinityDivision.transform.localScale = new Vector3(8.0f, 1.5f, 10.0f) *  15f;

                Color tempColor = unitSalinityDivision.GetComponent<Renderer>().material.color;

                tempColor.a = (1 - salinityPoints[salinityIndexesXYearMixUlimit[indexYear][i]].salinity / limitUpFreshWaterValue) * tempColor.a;

                tempColor.a = (1 - salinityPoints[salinityIndexesXYearMixUlimit[indexYear][i]].salinity / limitUpFreshWaterValue) * (float)maximumAlpha/255;

                cloneUnitySalinityDivision.GetComponent<Renderer>().material.color = tempColor;

                cloneUnitySalinityDivision.GetComponent<SalinityPointInfo>().originalAlpha = tempColor.a;
                cloneUnitySalinityDivision.GetComponent<SalinityPointInfo>().visibilityAlpha = (float)visibilityAlpha / 255;
                cloneUnitySalinityDivision.GetComponent<SalinityPointInfo>().salinityValue = salinityPoints[salinityIndexesXYearMixUlimit[indexYear][i]].salinity;

                allUnitSalinityDivisions.Add(cloneUnitySalinityDivision);
                
            }



        }






    }

    void HideSalinityDivisions(bool hide)
    {
        if (hide)
        {
            for (int i = 0; i < allUnitSalinityDivisions.Count; i++)
            {
                allUnitSalinityDivisions[i].SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < allUnitSalinityDivisions.Count; i++)
            {
                allUnitSalinityDivisions[i].SetActive(true);
            }
        }


    }

    public void ChangeColorSalinityPoints(int typeChange)
    {
        if((typeChange == 1) || (typeChange == 2) || (allFreshSalinityPoints.Count > 0))
        {
            Color tempColor;


            for(int i = 0; i < allUnitSalinityDivisions.Count; i++)
            {
                tempColor = allUnitSalinityDivisions[i].GetComponent<Renderer>().material.color;

                if(typeChange == 1)
                {
                    tempColor.a = allUnitSalinityDivisions[i].GetComponent<SalinityPointInfo>().originalAlpha;
                    isVisibilityMode = false;
                }
                else
                {
                    tempColor.a = allUnitSalinityDivisions[i].GetComponent<SalinityPointInfo>().visibilityAlpha;
                    isVisibilityMode = true;
                }
                
                allUnitSalinityDivisions[i].GetComponent<Renderer>().material.color = tempColor;
            }

        }
        else
        {
            return;
        }

    }

    // Use this for initialization
    void Start () {

        nActualYear = 0;
        dataTimeText = GameObject.Find("Data Time Text").GetComponent<Text>();
        dataTimeText.text = "Year: " + (years[nActualYear]).ToString();
        initialPlayerPosition = new Vector3(playerObject.transform.position.x, playerObject.transform.position.y,
            playerObject.transform.position.z);

        lastPlayerPosition = playerObject.transform.position;
        xInitialProportion = freshWater.transform.localScale.x;
        numberFish = fishSchool.GetComponent<SchoolController>()._childAmount;

        listFishSchools = new List<GameObject>();



        p1 = new Vector2(53543.941f, 434126.177f);
        p2 = new Vector2(56260.2f, 430603.6f);

        p1 = new Vector2(54014f, 434364f);
        p2 = new Vector2(57126f, 430334f);

        Vector2 direction = (p2 - p1).normalized;

        float sideMagnitude = (p2 - p1).magnitude;
        Vector2 perpDirection = Vector2.Perpendicular(direction);

        p3 = p1 + perpDirection * sideMagnitude;
        p4 = p2 + perpDirection * sideMagnitude;

        float t1 = (p3 - p1).magnitude;
        float t2 = (p4 - p2).magnitude;



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

        upperRight = new Vector2(seaFloorCollider.bounds.max.x, seaFloorCollider.bounds.min.z);
        downLeft = new Vector2(seaFloorCollider.bounds.min.x, seaFloorCollider.bounds.max.z);
        VRDiagonalDistance = (upperRight - downLeft).magnitude;

        
        a = 0.5f * (p4 - p1).magnitude;
        b = 0.5f * (p3 - p2).magnitude;

        U = (p4 - p1) / (2 * a);
        V = (p3 - p2) / (2 * b);

        float tx = p1.x;
        float ty = p1.y;

        print(Q.x);

        SalinityPreCalculations preCalculations = new SalinityPreCalculations();

        salinityPoints = preCalculations.LoadSalinityPoints();

        salinityIndexesXYearMixDLimit = preCalculations.Load(1);
        salinityIndexesXYearMixUlimit = preCalculations.Load(2);

      

        //CreateAllWaterSubdivisions();

        salinityPointsXYear = new List<SalinityPoint>[years.Length];

        allUnitSalinityDivisions = new List<GameObject>();
        allUnitSecondarySalinityDivisions = new List<GameObject>();

        CreateSalinityDivisions(nActualYear);
        areSalinityPointsVisible = true;

        for (int i = 0; i < years.Length; i++)
        {
            List<SalinityPoint> dummySalinityPoints = new List<SalinityPoint>();

            

            for(int j = 0;j< salinityPoints.Length; j++)
            {
                if(years[i] == salinityPoints[j].year)
                {
                    dummySalinityPoints.Add(salinityPoints[j]);
                }
            }
            salinityPointsXYear[i] = dummySalinityPoints;
        }




        List<Dictionary<string, object>> dataFish = CSVReader.Read("Clupea");

        Algorithms alg = new Algorithms();

        fishPositionsXYear = new List<Vector2>[years.Length];
        fishNumberXYearInPos = new List<int>[years.Length];

        for(int i = 0; i < years.Length; i++)
        {
            List<Vector2> listFishPos = new List<Vector2>();
            List<int> listFishNumberInPos = new List<int>();

            for (int j = 0; j < dataFish.Count; j++)
            {

                double x = double.Parse(dataFish[j]["X"].ToString());
                double y = double.Parse(dataFish[j]["Y"].ToString());




                int n = (int)dataFish[j]["n"];
                string dateExtraction = (string)dataFish[j]["date"];

                Vector2 pos = new Vector2((float)x, (float)y);

                int year = years[i];

                string yearMonth = "-5-" + year.ToString();

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



    int GetWaterLayer(float actualDepth)
    {
        if(actualDepth <= upperRight.y && actualDepth >= deepestLevel)
        {
            float depthChunk = (upperRight.y - deepestLevel) / 10;

            int waterLayer = 1;
            for (int i = 0; i < numberWaterLayers; i++)
            {
                if (actualDepth >= upperRight.y - depthChunk * waterLayer)
                {

                    break;
                }
                waterLayer++;

            }

            return waterLayer;
        }
        else
        {

            return -1;
        }



    }
	
	// Update is called once per frame
	void Update () {



        lastPlayerPosition = playerObject.transform.position;


        
        //if (OVRInput.GetUp(OVRInput.Button.Three) || Input.GetKeyDown(KeyCode.DownArrow))
        if ((clickMove.GetLastStateDown(handtype) && clickAxis.GetLastAxis(handtype).y < 0) || Input.GetKeyDown(KeyCode.DownArrow))
        {           
            if(nActualYear > 0)
            {

                nActualYear--;
                dataTimeText.text = "Year: " + (years[nActualYear]).ToString();
                //playerObject.transform.position = initialPlayerPosition;
                freshWater.transform.localScale = new Vector3(xInitialProportion * freshWaterProportions[nActualYear],
                    freshWater.transform.localScale.y, freshWater.transform.localScale.z);

                print(years[nActualYear]);

                CreateSalinityDivisions(nActualYear);

                if (areSalinityPointsVisible)
                {
                    HideSalinityDivisions(false);

                }
                else
                {
                    HideSalinityDivisions(true);
   
                }

                StartCoroutine("WaitRespawn");

                
                

            }

        }

        //if (OVRInput.GetUp(OVRInput.Button.Four) || Input.GetKeyDown(KeyCode.UpArrow))
        if ((clickMove.GetLastStateDown(handtype) && clickAxis.GetLastAxis(handtype).y > 0) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (nActualYear < years.Length - 1)
            {


                nActualYear++;
                dataTimeText.text = "Year: " + (years[nActualYear]).ToString();
                freshWater.transform.localScale = new Vector3(xInitialProportion * freshWaterProportions[nActualYear],
                    freshWater.transform.localScale.y, freshWater.transform.localScale.z);

                print(years[nActualYear]);

                CreateSalinityDivisions(nActualYear);

                if (areSalinityPointsVisible)
                {
                    HideSalinityDivisions(false);
          

                }
                else
                {
                    HideSalinityDivisions(true);
                  
                }

                StartCoroutine("WaitRespawn");



            }
            
        }

        if (Input.GetKeyDown(KeyCode.N) || clickGrip.GetLastStateDown(handtype))
        {
            if (areSalinityPointsVisible)
            {
                HideSalinityDivisions(true);
                areSalinityPointsVisible = false;

            }
            else
            {
                HideSalinityDivisions(false);
                areSalinityPointsVisible = true;
            }
        }




    }

    IEnumerator WaitRespawn()
    {
        Vector2 realDir;
        Vector2 virtualDir;
        float rotationAngle;
        Vector2 change;
        Vector2 newPosition;

        GetComponent<SeaBedChange_VIVE>().ResetFishMarkers();

        for (int i = 0; i < listFishSchools.Count; i++)
        {

            if(i< fishPositionsXYear[nActualYear].Count)
            {
                Vector2 pos = fishPositionsXYear[nActualYear].ElementAt<Vector2>(i);


                newPosition = ConvertRealtoVR(pos);
                listFishSchools.ElementAt<GameObject>(i).GetComponent<SchoolController>()._childAmount = 1;
                listFishSchools.ElementAt<GameObject>(i).GetComponent<SchoolController>().Respawn();

                listFishSchools.ElementAt<GameObject>(i).transform.position = new Vector3(newPosition.x, fishSchool.transform.position.y, newPosition.y);

                GetComponent<SeaBedChange_VIVE>().AddFishMarker(GetComponent<PositionTrack>().GetPositionInMapFromVR(listFishSchools.ElementAt<GameObject>(i).transform.position));

                for (int j = 0; j < numberFish; j++)
                {

                    yield return new WaitForSeconds(.001f);
                }

                listFishSchools.ElementAt<GameObject>(i).GetComponent<SchoolController>()._childAmount = fishNumberXYearInPos[nActualYear].ElementAt<int>(i)*10;
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