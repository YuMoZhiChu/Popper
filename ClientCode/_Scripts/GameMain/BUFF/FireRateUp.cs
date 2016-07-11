/**
* ======== All the code changes must be recorded below. ========
* 
* ==============================================
*/
using UnityEngine;
using System.Collections;

public class FireRateUp : PlayerBuff {

	// Add public variables here:
	public float RateIncreasePercentage = 1.0f;

	// Add private members here:

	// Add member functions here:
	public override void StartBuff()
	{
		base.StartBuff();
		if (TPC != null)
		{
			TPC.roleAttr.weaponAttr.roleFireRate /= RateIncreasePercentage;
		}

	}

	public override void EndBuff()
	{
		base.EndBuff();
		if (TPC != null)
		{
			TPC.roleAttr.weaponAttr.roleFireRate *= RateIncreasePercentage;
		}
	}

	// Use this for initialization
	void Start () {
		base.Init();
	}
}
