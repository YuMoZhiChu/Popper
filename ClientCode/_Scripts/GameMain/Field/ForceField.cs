/**
* ======== All the code changes must be recorded below. ========
* 
* ==============================================
*/
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SphereCollider))]
public class ForceField : MonoBehaviour {

	// Add public variables here:
    public float colliderRadius = 0;
    public delegate void TriggerFunc(ref Collider other);
    public TriggerFunc triggerFunc;
	
	// Add private members here:
	
	// Add member functions here:

	// Use this for initialization
	void Start () {
        GetComponent<SphereCollider>().radius = colliderRadius;
	}

    void OnTriggerEnter(Collider other)
    {
        if (triggerFunc != null)
            triggerFunc(ref other);
    }

}
