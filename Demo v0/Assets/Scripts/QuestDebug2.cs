using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestDebug2 : MonoBehaviour {

    public static QuestDebug2 Instance;
    bool inMenu;

    Text logText;

    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start () {

        var rt = DebugUIBuilder.instance.AddLabel("Debug");
        logText = rt.GetComponent<Text>();
        inMenu = true;

    }
	
	// Update is called once per frame
	void Update () {
        if (OVRInput.GetDown(OVRInput.Button.Two) || OVRInput.GetDown(OVRInput.Button.Start))
        {
            if (inMenu) DebugUIBuilder.instance.Hide();
            else DebugUIBuilder.instance.Show();
            inMenu = !inMenu;
        }
    }

    public void Log(string msg)
    {
        logText.text = msg;

    }
}
