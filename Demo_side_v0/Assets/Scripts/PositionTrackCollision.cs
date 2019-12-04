using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionTrackCollision : MonoBehaviour
{
    public Material targetPointMaterial;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.tag == "interest_point")
    //    {
    //        other.gameObject.GetComponent<Renderer>().material = GetComponent<Renderer>().material;
    //    }
    //}

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "interest_point")
        {
            other.gameObject.GetComponent<Renderer>().material = targetPointMaterial;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "interest_point" && (other.gameObject.GetComponent<Renderer>().material != GetComponent<Renderer>().material))
        {
            other.gameObject.GetComponent<Renderer>().material = GetComponent<Renderer>().material;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if(collision.gameObject.tag == "interest_point")
        //{
        //    collision.gameObject.GetComponent<Renderer>().material = GetComponent<Renderer>().material;
        //}
    }

    private void OnCollisionExit(Collision collision)
    {
        //if (collision.gameObject.tag == "interest_point")
        //{
        //    collision.gameObject.GetComponent<Renderer>().material = targetPointMaterial;
        //}
    }

}
