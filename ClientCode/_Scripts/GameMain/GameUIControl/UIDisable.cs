using UnityEngine;
using System.Collections;

public class UIDisable : MonoBehaviour {

	public void UIdisable()
	{
		this.gameObject.SetActive (false);
		//GameControl.instance.gameUI.DaZhaoBG [Datas.roleID].SetActive (true);
	}

}
