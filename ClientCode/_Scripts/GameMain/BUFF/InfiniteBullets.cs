/**
* ======== All the code changes must be recorded below. ========
* 
* ==============================================
*/
using UnityEngine;
using System.Collections;

public class InfiniteBullets : PlayerBuff {

	// Add public variables here:
	public int BulletsLocked = 99;

	// Add private members here:
    private bool state = false;

	// Add member functions here:
	public override void StartBuff()
	{
		base.StartBuff();
		if (TPC != null)
		{
            state = true;
            TPC.roleAttr.weaponAttr.bulletsClpips = BulletsLocked;
		}

	}

	public override void EndBuff()
	{
		base.EndBuff();
		if (TPC != null)
		{
            state = false;
            TPC.roleAttr.weaponAttr.bulletsClpips = TPC.roleAttr.weaponAttr.maxBulletsClips;
		}
	}

	// Use this for initialization
	void Start () {
		base.Init();
	}

    void Update()
    {
        if (state == true)
            TPC.roleAttr.weaponAttr.bulletsClpips = BulletsLocked;
    }
}
