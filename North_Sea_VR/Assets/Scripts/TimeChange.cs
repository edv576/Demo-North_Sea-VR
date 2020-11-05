using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using System.Linq;
using MIConvexHull;
using System;


////2D point used for the maps in the application
//struct Point2D
//{
//    public float x;
//    public float y;
//};

////Struct for the salinity point. Contains the salinity in PPT, layer and the year
//[Serializable]
//public struct SalinityPoint
//{
//    public float x;
//    public float y;
//    public float salinity;
//    public int waterLayer;
//    public int year;

//};


////Used to save data of one of the subdivisions that divides the whole water space in the area of study.
////It takes in account brackish water which wasn't considered for the first version of the system
//struct WaterSubdivision
//{
//    public float x0;
//    public float xf;
//    public float gradientY0;
//    public float gradientYf;
//    public bool thereIsData;
//    public float layer;
    
//};



public class TimeChange : MonoBehaviour {
  

    public float[] freshWaterProportions;
    public Vector3[] fishPositions;
    //Integer representing the index of the current year for the application. If 3 years are used then there are 3 indexes.
    int nActualYear;   
    public GameObject freshWater;
    public GameObject playerObject;
    //Template object for the fish school. It will be replicated later in every position the fish are according to the data.
    public GameObject fishSchool;
    //Object that is the template of the freshwater data point. It's then replicated for all the datapoints that represent freshwater.
    public GameObject unitSalinityDivision;
    //Maximun alpha for the transparence of the freshwater
    public int maximumAlpha;
    public int visibilityAlpha;
    //Maximum and minimum limits for freshwater in PPT
    public float limitUpFreshWaterValue;
    public float limitDownFreshWaterValue;
    //Indicates if the freshwater is visible or not
    public bool isVisibilityMode;
    List<GameObject> allUnitSalinityDivisions;
    List<GameObject> allUnitSecondarySalinityDivisions;
    List<GameObject> listFishSchools;

    float xInitialProportion;
    Text dataTimeText;

    //Initial number of fish. Will change depending on the data
    int numberFish = 63;
    //Number of years used for the application. Its 5 by default but can be changed in the Inspector mode.
    public int yearSamples = 5;
    //Initial year of the ones used in the application.
    public int startYear = 2005;
    //No longer used. It helped if the the years were separated by a constant number.
    public int yearStep = 2;
    //Contains all the years
    public int[] years;
    //Positions were the fish are in the area of study. Obtained from the read data.
    List<Vector2>[] fishPositionsXYear;
    List<int>[] fishNumberXYearInPos;
    //Lists of all salinity points for all years
    List<SalinityPoint>[] salinityPointsXYear;
    List<int>[] salinityIndexesXYearMixDLimit;
    List<int>[] salinityIndexesXYearMixUlimit;
    //Has all salinity points for an specific year
    SalinityPoint[] salinityPoints;
    //Position were the fish schools are moved to be invisible to the user. This happens when the number of schools is less than the
    //maximum number of usable schools.
    Vector3 vanishPos;
    List<double[]> allFreshSalinityPoints;
    bool areSalinityPointsVisible;

    

    public BoxCollider seaFloorCollider = null;
    public BoxCollider seaWaterCollider = null;
    float maxX, minX, maxY, minY;
    //Diagonal of the area of study in the real world
    float RWDiagonalDistance;
    //Diagonal of the are of study in the virtual world
    float VRDiagonalDistance;
    //4 points that define the area of study in the real world
    Vector2 p1, p2, p3, p4;
    Vector2 Q;


    //Corners of the area of study in the virtual world
    Vector2 upperRight;
    Vector2 downLeft;
    public int numberWaterLayers;
    public float deepestLevel;
    public int subdivisions;
    List<WaterSubdivision[,]> listWaterSubdivisionsXYear;
    
    //Commands used for the VR controllers
    public SteamVR_ActionSet movementSet;
    public SteamVR_Action_Boolean clickMove;
    public SteamVR_Action_Vector2 clickAxis;
    public SteamVR_Action_Boolean clickGrip;
    public SteamVR_Input_Sources handtype;

    //Rotates the vector a number of degrees (0-360)
    public Vector2 RotateVector(Vector2 v, float angle)
    {
        float radian = angle * Mathf.Deg2Rad;
        float _x = v.x * Mathf.Cos(radian) - v.y * Mathf.Sin(radian);
        float _y = v.x * Mathf.Sin(radian) + v.y * Mathf.Cos(radian);
        return new Vector2(_x, _y);
        
    }




  // From real world units to Virtual units. It does a rotation first since the virtual area of study is not aligned to the real one.
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



    //Creates the points for freshwater salinity using the unitysalinitydivision object as template
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

    //In case user needs to hide the freshwater
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

    //Only used when user wants brackish water. Not useful for the current implementation.
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



        xInitialProportion = freshWater.transform.localScale.x;
        numberFish = fishSchool.GetComponent<SchoolController>()._childAmount;

        listFishSchools = new List<GameObject>();


        //Initial points in the real world. The other 2 are calculated from here in a way that all 4 create a perfect square.
        p1 = new Vector2(53543.941f, 434126.177f);
        p2 = new Vector2(56260.2f, 430603.6f);

        Vector2 direction = (p2 - p1).normalized;

        float sideMagnitude = (p2 - p1).magnitude;
        Vector2 perpDirection = Vector2.Perpendicular(direction);

        p3 = p1 + perpDirection * sideMagnitude;
        p4 = p2 + perpDirection * sideMagnitude;





        Q = 0.5f * (p2 + p3);


     

        //find the minimum and maximum points
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

        


 

        print(Q.x);

        //Load salinity data

        SalinityPreCalculations preCalculations = new SalinityPreCalculations();

        salinityPoints = preCalculations.LoadSalinityPoints();

        salinityIndexesXYearMixDLimit = preCalculations.Load(1);
        salinityIndexesXYearMixUlimit = preCalculations.Load(2);


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



        //Load fish data
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

        //Instantiate maximum number of fish schools

        listFishSchools = new List<GameObject>();

        for(int i = 0; i < max; i++)
        {
            GameObject cloneFishSchool = Instantiate(fishSchool, fishSchool.transform.position, fishSchool.transform.rotation) as GameObject;
            listFishSchools.Add(cloneFishSchool);

        }


        vanishPos = new Vector3(0.0f, -300.0f, 0.0f);
       
        //Used to change the number of fish in schools. It doesn't work without a temporal delay.
        StartCoroutine("WaitRespawn");


    }



	
	// Update is called once per frame
	void Update () {
    
        //Change the salinity with years - for VR
        if ((clickMove.GetLastStateDown(handtype) && clickAxis.GetLastAxis(handtype).y < 0) || Input.GetKeyDown(KeyCode.DownArrow))
        {           
            if(nActualYear > 0)
            {

                nActualYear--;
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

                StartCoroutine(WaitRespawn());

                
                

            }

        }

        //Change the salinity with years for Flat Screens

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

       
                StartCoroutine(WaitRespawn());



            }
            
        }

        //Hide or show freshwater
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

    //Respawns the fish schools - one fish at a time
    //There is a bug that needs to be fixed here. If the user switches years too fast in a continuous way the fish will be spawned but not
    //destroyed to correspond to new year. Eventually the big number of fish will make the system slow.
    IEnumerator WaitRespawn()
    {

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
