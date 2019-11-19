using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class UserMovement_VIVE : MonoBehaviour {

    public GameObject directionController;
    public GameObject playerObject;

    public SteamVR_ActionSet movementSet;
    public SteamVR_Action_Boolean clickMove;
    public SteamVR_Action_Vector2 clickAxis;
    public SteamVR_Input_Sources handtype;
    BoxCollider limitCollider;

    public GameObject seaBed;


    private void Awake()
    {
        clickMove = SteamVR_Actions.MovementSet.ClickMove;
        clickAxis = SteamVR_Actions.MovementSet.ClickAxis;
    }

    bool IsInLimits()
    {
        if(playerObject.transform.position.x > limitCollider.bounds.min.x && playerObject.transform.position.x < limitCollider.bounds.max.x &&
            playerObject.transform.position.z > limitCollider.bounds.min.z && playerObject.transform.position.z < limitCollider.bounds.max.z)
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

        limitCollider = seaBed.GetComponent<BoxCollider>();


   
    }
	
	// Update is called once per frame
	void Update () {


        if (clickMove.GetState(handtype) && clickAxis.GetLastAxis(handtype).y > 0)
        {
            if (IsInLimits())
            {
                playerObject.transform.position += directionController.transform.forward * Time.deltaTime * 20.0f;
            }
            else
            {
                playerObject.transform.position -= directionController.transform.forward * Time.deltaTime * 100.0f;
            }
            
            
        }



        if (clickMove.GetState(handtype) && clickAxis.GetLastAxis(handtype).y < 0)
        {
            if (IsInLimits())
            {
                playerObject.transform.position += directionController.transform.forward * Time.deltaTime * 3.0f;
            }
            else
            {
                playerObject.transform.position -= directionController.transform.forward * Time.deltaTime * 100.0f;
            }

            

        }
   
       




    }
}
