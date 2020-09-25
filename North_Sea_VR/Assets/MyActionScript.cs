using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class MyActionScript : MonoBehaviour
{

    public SteamVR_ActionSet movementSet;
    public SteamVR_Action_Boolean clickMove;
    public SteamVR_Action_Vector2 clickAxis;
    public SteamVR_Input_Sources handtype;
    public GameObject sphere;
    private int counter = 0;


    private void Awake()
    {
        clickMove = SteamVR_Actions.MovementSet.ClickMove;
        clickAxis = SteamVR_Actions.MovementSet.ClickAxis;
    }

    // Start is called before the first frame update
    void Start()
    {
        //moveUser.AddOnStateDownListener(TriggerDown, handtype);
        //moveUser.AddOnStateUpListener(TriggerUp, handtype);
        //moveUser.AddOnChangeListener(TriggerStay, handtype);

        

        
    }

    public void TriggerUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        Debug.Log("Trigger is up");
        sphere.GetComponent<MeshRenderer>().enabled = false;
    }
    public void TriggerDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        Debug.Log("Trigger is down");
        sphere.GetComponent<MeshRenderer>().enabled = true;
        counter++;
        Debug.Log(counter);
    }


    // Update is called once per frame
    void Update()
    {

        if (clickMove.GetState(handtype) && clickAxis.GetLastAxis(handtype).y > 0)
        {
            counter += 50;
            Debug.Log(counter);
            //Debug.Log()

        }

        if (clickMove.GetState(handtype) && clickAxis.GetLastAxis(handtype).y < 0)
        {
            counter += 5;
            Debug.Log(counter);
            //Debug.Log()

        }

        //if(moveUser2.GetLastAxis(handtype).y > 0)
        //{
        //    counter++;
        //    Debug.Log(counter);

        //}



        //Debug.Log(moveUser2.GetLastAxis(handtype).y);



    }
}
