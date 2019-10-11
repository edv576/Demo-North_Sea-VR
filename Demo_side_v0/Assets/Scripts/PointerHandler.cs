using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Valve.VR.Extras;

public class PointerHandler : MonoBehaviour
{
    public SteamVR_LaserPointer laserPointer;
    public Material selectedMaterial;
    public Material unselectedMaterial;
    public GameObject player;

    void Awake()
    {
        laserPointer.PointerIn += PointerInside;
        laserPointer.PointerOut += PointerOutside;
        laserPointer.PointerClick += PointerClick;
    }

    public void PointerClick(object sender, PointerEventArgs e)
    {
        if(e.target.name.Contains("Point of interest"))
        {
            player.transform.position = GetComponent<PositionTrack>().getPositionFromMap(e.target.gameObject.transform.localPosition);


        }

        //if (e.target.name == "Cube")
        //{
        //    Debug.Log("Cube was clicked");
        //}
        //else if (e.target.name == "Button")
        //{
        //    Debug.Log("Button was clicked");
        //}
    }

    public void PointerInside(object sender, PointerEventArgs e)
    {

        if (e.target.name.Contains("Point of interest"))
        {
            e.target.gameObject.GetComponent<Renderer>().material = selectedMaterial;

        }
        //if (e.target.name == "Cube")
        //{           
        //    Debug.Log("Cube was entered");
        //}
        //else if (e.target.name == "Button")
        //{
        //    Debug.Log("Button was entered");
        //}
    }

    public void PointerOutside(object sender, PointerEventArgs e)
    {

        if (e.target.name.Contains("Point of interest"))
        {
            e.target.gameObject.GetComponent<Renderer>().material = unselectedMaterial;

        }
        //if (e.target.name == "Cube")
        //{
        //    Debug.Log("Cube was exited");
        //}
        //else if (e.target.name == "Button")
        //{
        //    Debug.Log("Button was exited");
        //}
    }
}
