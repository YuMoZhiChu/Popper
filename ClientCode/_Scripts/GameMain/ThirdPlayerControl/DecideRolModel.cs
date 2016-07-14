using UnityEngine;
using System.Collections;

public class DecideRolModel : MonoBehaviour {

	public GameObject[] models;

	// Use this for initialization
	//void Awake() {
	//	models[Datas.roleID].SetActive(true);
	//	this.transform.GetComponent<ThirdPlayerControl> ().role = models[Datas.roleID];
	//}
	public void setRool(int roleId)
	{
		models[roleId].SetActive(true);
		this.transform.GetComponent<ThirdPlayerControl>().role = models[roleId];
	}
	// Update is called once per frame
	void Update () {
	
	}
}
