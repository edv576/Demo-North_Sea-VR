using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishHandler : MonoBehaviour
{
    public BoxCollider seaCollider = null;
    float maxX, minX, maxY, minY;
    float rotationAngle;
    float RWDiagonalDistance;
    float VRDiagonalDistance;

    public Vector2 RotateVector(Vector2 v, float angle)
    {
        float radian = angle * Mathf.Deg2Rad;
        float _x = v.x * Mathf.Cos(radian) - v.y * Mathf.Sin(radian);
        float _y = v.x * Mathf.Sin(radian) + v.y * Mathf.Cos(radian);
        return new Vector2(_x, _y);
    }

    bool IsInLimits(Vector2 position)
    {
        if(position.x >= minX && position.x <= maxX && position.y >= minY && position.y <= maxY)
        {
            return true;

        }
        else
        {
            return false;
        }
    }

    

    // Start is called before the first frame update
    void Start()
    {


        Vector2 p1 = new Vector2(3.92442f, 51.88204f);
        Vector2 p2 = new Vector2(3.99102f, 51.82542f);

        Vector2 direction = (p2 - p1).normalized;

        float sideMagnitude = (p2 - p1).magnitude;
        Vector2 perpDirection = Vector2.Perpendicular(direction);

        Vector2 p3 = p1 + perpDirection * sideMagnitude;
        Vector2 p4 = p2 + perpDirection * sideMagnitude;

        rotationAngle = Vector2.Angle(perpDirection, Vector2.right);




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

        Vector2 upperRight = new Vector2(seaCollider.bounds.max.z, seaCollider.bounds.max.x);
        Vector2 downLeft = new Vector2(seaCollider.bounds.min.z, seaCollider.bounds.min.x);
        VRDiagonalDistance = (upperRight - downLeft).magnitude;

        
  
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
