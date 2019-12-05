using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class UserMovement_VIVE : MonoBehaviour {

    public GameObject directionController;
    public GameObject playerObject;
    public GameObject gameManager;
    public GameObject waterObject;

    public SteamVR_ActionSet movementSet;
    public SteamVR_Action_Boolean clickMove;
    public SteamVR_Action_Vector2 clickAxis;
    public SteamVR_Input_Sources handtype;
    BoxCollider limitCollider;
    private int collisionCount;
    private bool isBeginning;

    float waterlevel;


    public GameObject seaBed;

    public bool IsNotColliding
    {
        get { return collisionCount == 0; }
    }


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

    void OnCollisionEnter(Collision col)
    {
        //if (!isBeginning)
        //{
            print("Collision detected");
            if (col.gameObject.tag == "salinity")
            {
                collisionCount++;
            }
            print(collisionCount);
        //}
        
        

    }

    void OnCollisionExit(Collision col)
    {
        //if (!isBeginning)
        //{
            if (col.gameObject.tag == "salinity")
            {
                collisionCount--;
            }
        //}
        
    }

    // Use this for initialization
    void Start () {

        limitCollider = seaBed.GetComponent<BoxCollider>();
        collisionCount = 0;
        isBeginning = true;
        waterlevel = waterObject.GetComponent<BoxCollider>().bounds.max.y;


    }
	
	// Update is called once per frame
	void Update () {

        //if(isBeginning)
        //{
        //    //collisionCount = 0;
        //    isBeginning = false;
        //}


        if (clickMove.GetState(handtype) && clickAxis.GetLastAxis(handtype).y > 0)
        {
            if (IsInLimits())
            {
                playerObject.transform.position += directionController.transform.forward * Time.deltaTime * 80.0f;
            }
            else
            {
                playerObject.transform.position -= directionController.transform.forward * Time.deltaTime * 1000.0f;
            }
            
            
        }



        if ((clickMove.GetState(handtype) && clickAxis.GetLastAxis(handtype).y < 0))
        {
            if (IsInLimits())
            {
                playerObject.transform.position += directionController.transform.forward * Time.deltaTime * 30.0f;
            }
            else
            {
                playerObject.transform.position -= directionController.transform.forward * Time.deltaTime * 1000.0f;
            }           

        }

        if (Input.GetKey(KeyCode.W))
        {
            if (IsInLimits())
            {
                playerObject.transform.position += GetComponentInChildren<Camera>().transform.forward * Time.deltaTime * 80.0f;
            }
            else
            {
                playerObject.transform.position -= GetComponentInChildren<Camera>().transform.forward * Time.deltaTime * 1000.0f;
            }
        }

        if (Input.GetKey(KeyCode.S))
        {
            if (IsInLimits())
            {
                playerObject.transform.position += GetComponentInChildren<Camera>().transform.forward * Time.deltaTime * 30.0f;
            }
            else
            {
                playerObject.transform.position -= GetComponentInChildren<Camera>().transform.forward * Time.deltaTime * 1000.0f;
            }
        }



        //if (IsNotColliding)
        //{
        //    if (!gameManager.GetComponent<TimeChange>().isVisibilityMode && transform.position.y < waterlevel)
        //    {
        //        gameManager.GetComponent<TimeChange>().ChangeColorSalinityPoints(2);
        //    }
        //    else if (transform.position.y > waterlevel)
        //    {
        //        gameManager.GetComponent<TimeChange>().ChangeColorSalinityPoints(1);
        //    }
        //    else if (gameManager.GetComponent<TimeChange>().isVisibilityMode)
        //    {
        //        gameManager.GetComponent<TimeChange>().ChangeColorSalinityPoints(2);
        //    }

        //}
        //else
        //{
        //    if (gameManager.GetComponent<TimeChange>().isVisibilityMode)
        //    {
        //        gameManager.GetComponent<TimeChange>().ChangeColorSalinityPoints(1);
        //    }
        //}



        print(collisionCount);










    }
}
