/**
* ======== All the code changes must be recorded below. ========
* 
* ==============================================
*/
using UnityEngine;
using System.Collections;

public class AnimationEventSendMessage : MonoBehaviour {

	// Add public variables here:
    public GameObject fatherNode;
	
	// Add private members here:
	
	// Add member functions here:
    public void SendMessageToFatherNode(string function)
    {
        fatherNode.SendMessage(function);
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
