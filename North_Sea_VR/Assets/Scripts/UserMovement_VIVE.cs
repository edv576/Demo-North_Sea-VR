using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class UserMovement_VIVE : MonoBehaviour {

    public GameObject directionController;
    public GameObject playerObject;
    public GameObject gameManager;
    public GameObject waterObject;
    public GameObject cage;
    public GameObject checkpoint;
    public GameObject water;

    public SteamVR_ActionSet movementSet;
    public SteamVR_Action_Boolean clickMove;
    public SteamVR_Action_Vector2 clickAxis;
    public SteamVR_Input_Sources handtype;
    BoxCollider limitCollider;
    private int collisionCount;
    private bool isBeginning;
    private Vector3 previousPosition;
    bool lastFogCheck;
    

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
        //if(playerObject.transform.position.x > limitCollider.bounds.min.x && playerObject.transform.position.x < limitCollider.bounds.max.x &&
        //    playerObject.transform.position.z > limitCollider.bounds.min.z && playerObject.transform.position.z < limitCollider.bounds.max.z &&
        //    playerObject.transform.position.y > limitCollider.bounds.min.y && playerObject.transform.position.y < limitCollider.bounds.max.y)
            if (playerObject.transform.position.x > limitCollider.bounds.min.x && playerObject.transform.position.x < limitCollider.bounds.max.x &&
                playerObject.transform.position.z > limitCollider.bounds.min.z && playerObject.transform.position.z < limitCollider.bounds.max.z &&
                playerObject.transform.position.y > checkpoint.transform.position.y && playerObject.transform.position.y < limitCollider.bounds.max.y)
            {
            return true;
        }
        else
        {
            return false;
        }

    }

    void SetCageVisible(bool status)
    {
        var meshRenderers = cage.GetComponentsInChildren<MeshRenderer>();
        for(int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].enabled = status;
        }
    }

    void OnCollisionEnter(Collision col)
    {

        if (col.gameObject.tag == "cage")
        {
            SetCageVisible(true);
            print("Collision with cage detected");
            if (col.gameObject.name == "Cage down")
            {
                transform.position = new Vector3(transform.position.x, checkpoint.transform.position.y, transform.position.z);
                //transform.position = new Vector3(transform.position.x, previousPosition.y, transform.position.z);
            }
        }


    }

    void OnCollisionExit(Collision col)
    {
        if (col.gameObject.tag == "cage")
        {
            SetCageVisible(false);
        }

    }

    // Use this for initialization
    void Start () {

        limitCollider = seaBed.GetComponent<BoxCollider>();
        collisionCount = 0;
        isBeginning = true;
        waterlevel = waterObject.GetComponent<BoxCollider>().bounds.max.y;
        previousPosition = playerObject.transform.position;
        lastFogCheck = false;

    }
	
	// Update is called once per frame
	void Update () {


        if (clickMove.GetState(handtype) && clickAxis.GetLastAxis(handtype).y > 0)
        {
            if (IsInLimits())
            {
   
                previousPosition = playerObject.transform.position;
                playerObject.transform.position += directionController.transform.forward * Time.deltaTime * 80.0f;
            }
            else
            {
     
                playerObject.transform.position = previousPosition;
           
            }
            
            
        }



        if ((clickMove.GetState(handtype) && clickAxis.GetLastAxis(handtype).y < 0))
        {
            if (IsInLimits())
            {
      
                previousPosition = playerObject.transform.position;
                playerObject.transform.position += directionController.transform.forward * Time.deltaTime * 30.0f;
            }
            else
            {
    
                playerObject.transform.position = previousPosition;
          
            }           

        }

        if (Input.GetKey(KeyCode.W))
        {
            if (IsInLimits())
            {
   
                previousPosition = playerObject.transform.position;
                playerObject.transform.position += GetComponentInChildren<Camera>().transform.forward * Time.deltaTime * 80.0f;
            }
            else
            {
      
                playerObject.transform.position = previousPosition;
     
            }
        }

        if (Input.GetKey(KeyCode.S))
        {
            if (IsInLimits())
            {
                cage.SetActive(false);
                previousPosition = playerObject.transform.position;
                playerObject.transform.position += GetComponentInChildren<Camera>().transform.forward * Time.deltaTime * 30.0f;
            }
            else
            {
                cage.SetActive(true);
                playerObject.transform.position = previousPosition;
        
            }
        }

        bool verifyUnderwater = transform.position.y < water.GetComponent<Collider>().bounds.max.y;

        if(verifyUnderwater)
        {
            if(verifyUnderwater != lastFogCheck)
            {
                GetComponentInChildren<Kino.Fog>().enabled = true;
            }
            lastFogCheck = verifyUnderwater;

        }
        else
        {
            if (verifyUnderwater != lastFogCheck)
            {
                GetComponentInChildren<Kino.Fog>().enabled = false;
            }
            lastFogCheck = verifyUnderwater;

        }



        



   










    }
}
