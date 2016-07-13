using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WaitRebornConreol : MonoBehaviour {

	public GameObject rebornBar;

	private float amount = 0;
	private Image image;

	public void SetRebornBar()
	{
		this.gameObject.SetActive (true);
		amount = 0;
		image = rebornBar.GetComponent<Image> ();
	}

	// Use this for initialization
	void Awake()
	{
	}

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		amount += Time.deltaTime;
		if (amount <= 5) {
			image.fillAmount = amount / 5.0f;
		} else {
			this.gameObject.SetActive (false);
		}
	}
}
