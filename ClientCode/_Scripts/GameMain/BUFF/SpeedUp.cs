/**
* ======== All the code changes must be recorded below. ========
* 
* ==============================================
*/
using UnityEngine;
using System.Collections;

public class SpeedUp : PlayerBuff {

	// Add public variables here:
	public float SpeedIncrease = 3;
	
	// Add private members here:
	
	// Add member functions here:
    public override void StartBuff()
    {
        base.StartBuff();
        if (TPC != null)
        {
			TPC.roleAttr.moveSpeed += SpeedIncrease;
        }
        
    }

    public override void EndBuff()
    {
        base.EndBuff();
        if (TPC != null)
        {
			TPC.roleAttr.moveSpeed -= SpeedIncrease;
        }
    }

	// Use this for initialization
	void Start () {
        base.Init();
	}
}
