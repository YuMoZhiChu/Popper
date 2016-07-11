/**
* ======== All the code changes must be recorded below. ========
* 
* ==============================================
*/
using UnityEngine;
using System.Collections;

public class DamageUp : PlayerBuff {

	// Add public variables here:
	public float DamageIncrease = 0.0f;
	public float DamageIncreasePercentage = 1.0f;

	// Add private members here:

	// Add member functions here:
	public override void StartBuff()
	{
		base.StartBuff();
		if (TPC != null)
		{
			TPC.roleAttr.weaponAttr.normalFireDamage *= DamageIncreasePercentage;
			TPC.roleAttr.weaponAttr.normalFireDamage += DamageIncrease;
		}

	}

	public override void EndBuff()
	{
		base.EndBuff();
		if (TPC != null)
		{
			TPC.roleAttr.weaponAttr.normalFireDamage -= DamageIncrease;
			TPC.roleAttr.weaponAttr.normalFireDamage /= DamageIncreasePercentage;

		}
	}

	// Use this for initialization
	void Start () {
		base.Init();
	}
}
