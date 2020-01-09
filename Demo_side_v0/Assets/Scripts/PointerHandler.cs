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
        if(e.target.name.Contains("interest"))
        {
            player.transform.position = GetComponent<PositionTrack>().GetPositionFromMap(e.target.gameObject.transform.localPosition);


        }


    }

    public void PointerInside(object sender, PointerEventArgs e)
    {

        if (e.target.name.Contains("interest"))
        {
            e.target.gameObject.GetComponent<Renderer>().material = selectedMaterial;

        }

    }

    public void PointerOutside(object sender, PointerEventArgs e)
    {

        if (e.target.name.Contains("interest"))
        {
            e.target.gameObject.GetComponent<Renderer>().material = unselectedMaterial;

        }

    }
}
