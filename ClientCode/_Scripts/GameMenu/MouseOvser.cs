/**
* ======== All the code changes must be recorded below. ========
* 
* ==============================================
*/
using UnityEngine;
using System.Collections;
using DG.Tweening;

public class MouseOvser : MonoBehaviour {

	// Add public variables here:
    public GameObject Don;
	
	// Add private members here:
    Vector3 mouseDeltaPosition = new Vector3(0, 0, 0);
	Tweener rotateTween = null;
	
	// Add member functions here:

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButton(0))
        {
			if (rotateTween != null)
				rotateTween.Kill();
            mouseDeltaPosition = new Vector3(0, -Input.GetAxis("Mouse X") * 5f, 0);
            this.transform.Rotate(mouseDeltaPosition);
        }
		if (Input.GetMouseButtonUp(0))
		{
			float angle = this.transform.localRotation.eulerAngles.y;
			if (angle > 180)
				angle = 360 - angle;
			rotateTween = this.transform.DOLocalRotate(new Vector3(0, 0, 0), angle / 90).SetEase(Ease.Linear);
		}
	}
}
