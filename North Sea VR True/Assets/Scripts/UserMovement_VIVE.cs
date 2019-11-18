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
    


    private void Awake()
    {
        
        clickMove = SteamVR_Actions.MovementSet.ClickMove;
        clickAxis = SteamVR_Actions.MovementSet.ClickAxis;
    }

    // Use this for initialization
    void Start () {




   
    }
	
	// Update is called once per frame
	void Update () {


        if (clickMove.GetState(handtype) && clickAxis.GetLastAxis(handtype).y > 0)
        {
            
            playerObject.transform.position += directionController.transform.forward * Time.deltaTime * 20.0f;
        }

        if (clickMove.GetState(handtype) && clickAxis.GetLastAxis(handtype).y < 0)
        {
            playerObject.transform.position += directionController.transform.forward * Time.deltaTime * 3.0f;

        }




    }
}
