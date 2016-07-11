using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NameTipsFade : MonoBehaviour {

	public void ShowTips()
	{
		this.GetComponent<Graphic> ().CrossFadeAlpha (0, 0, false);
		this.GetComponent<Graphic> ().CrossFadeAlpha (1, 2, false);
		startFadeTime = Time.time + 5;
	}

	private float startFadeTime = 0;

	// Use this for initialization
	void Start () {
		ShowTips ();
	}

	IEnumerator Fadeout()
	{
		yield return new WaitForSeconds (5);
		this.GetComponent<Graphic> ().CrossFadeAlpha (0, 2, false);
	}

	// Update is called once per frame
	void Update () {
		if (startFadeTime <= Time.time && startFadeTime >= 0) {
			startFadeTime = -1;
			this.GetComponent<Graphic> ().CrossFadeAlpha (0, 2, false);
		}
	}
}
