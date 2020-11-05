using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//2D point used for the maps in the application
struct Point2D
{
    public float x;
    public float y;
};

//Struct for the salinity point. Contains the salinity in PPT, layer and the year
[Serializable]
public struct SalinityPoint
{
    public float x;
    public float y;
    public float salinity;
    public int waterLayer;
    public int year;

};


//Used to save data of one of the subdivisions that divides the whole water space in the area of study.
//It takes in account brackish water which wasn't considered for the first version of the system
[Serializable]
struct WaterSubdivision
{
    public float x0;
    public float xf;
    public float gradientY0;
    public float gradientYf;
    public bool thereIsData;
    public float layer;

};


public class CommonStructs : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
