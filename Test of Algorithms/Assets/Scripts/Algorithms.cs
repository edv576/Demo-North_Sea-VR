using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Algorithms : MonoBehaviour
{

    float AreaOfTriangle(Vector2 point1, Vector2 point2, Vector2 point3)
    {
        return Mathf.Abs((point1.x * (point2.y - point3.y) + point2.x * (point3.y - point1.y) + point3.x * (point1.y - point2.y)) / 2);

    }

    float SumOfAreas_with_Point_4Vertices(Vector2 P, Vector2 V1, Vector2 V2, Vector2 V3, Vector2 V4)
    {
        float A_PV2V1, A_PV1V3, A_PV3V4, A_PV4V2;

        A_PV2V1 = AreaOfTriangle(P, V2, V1);
        A_PV1V3 = AreaOfTriangle(P, V1, V3);
        A_PV3V4 = AreaOfTriangle(P, V3, V4);
        A_PV4V2 = AreaOfTriangle(P, V4, V2);

        return (A_PV2V1 + A_PV1V3 + A_PV3V4 + A_PV4V2);
    }

    bool InAreaOfStudy_4Vertices(Vector2 P, Vector2 V1, Vector2 V2, Vector2 V3, Vector2 V4)
    {

        Vector2 barycenter = new Vector2((V1.x + V2.x + V3.x + V4.x) / 4, (V1.y + V2.y + V3.y + V4.y) / 4);

        float fullAreaOfStudy = Mathf.Pow((V2 - V1).magnitude, 2.0f);
        float fullAreaOfStudy2 = SumOfAreas_with_Point_4Vertices(barycenter, V1, V2, V3, V4);
        float fullAreaWithPoint = SumOfAreas_with_Point_4Vertices(P, V1, V2, V3, V4);


        if(fullAreaWithPoint <= fullAreaOfStudy2)
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
        bool isIn = InAreaOfStudy_4Vertices(new Vector2(0.003f, 2.0f), new Vector2(0.0f, 1.0f), new Vector2(-1.0f, 0), new Vector2(1.0f, 0), new Vector2(0.0f, -1.0f));

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
