using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SalinityPointInfo : MonoBehaviour
{

    public float originalAlpha;
    public float visibilityAlpha;
    public float salinityValue;
    public GameObject gameManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.tag == "salinity" && this.gameObject.tag == "salinity")
    //    {
    //        //Transform transformCollision = collision.gameObject.transform;
    //        //Vector3 unitSalinityDivisionPos = new Vector3((transform.position.x + transformCollision.position.x) / 2, (transform.position.y + transformCollision.position.y) / 2, (transform.position.z + transformCollision.position.z) / 2);


    //        //GameObject cloneUnitySalinityDivision = Instantiate(this.gameObject, unitSalinityDivisionPos, this.gameObject.transform.rotation);



    //        //cloneUnitySalinityDivision.tag = "salinity_secondary";

    //        //Color tempColor = cloneUnitySalinityDivision.GetComponent<Renderer>().material.color;

    //        //float cloneSalinity = (this.gameObject.GetComponent<SalinityPointInfo>().salinityValue + collision.gameObject.GetComponent<SalinityPointInfo>().salinityValue) / 2;

    //        //tempColor.a = (1 - cloneSalinity / gameManager.GetComponent<TimeChange>().limitUpFreshWaterValue) * tempColor.a;

    //        //cloneUnitySalinityDivision.GetComponent<SalinityPointInfo>().originalAlpha = tempColor.a;
    //        //cloneUnitySalinityDivision.GetComponent<SalinityPointInfo>().visibilityAlpha = (float)gameManager.GetComponent<TimeChange>().visibilityAlpha / 255;
    //        //cloneUnitySalinityDivision.GetComponent<SalinityPointInfo>().salinityValue = cloneSalinity;

    //        //gameManager.GetComponent<TimeChange>().AddSecondarySalinityDivision(cloneUnitySalinityDivision);

    //        //if(gameManager.GetComponent<TimeChange>().NumberSecondarySalinityDivision() == 13552)
    //        //{

    //        //    int p = 0;
    //        //}

    //        //System.Diagnostics.Debug.WriteLine(gameManager.GetComponent<TimeChange>().NumberSecondarySalinityDivision());



    //    }
    //}
}
