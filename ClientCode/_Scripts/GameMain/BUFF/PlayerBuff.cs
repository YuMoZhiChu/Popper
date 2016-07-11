/**
* ======== All the code changes must be recorded below. ========
* 
* ==============================================
*/
using UnityEngine;
using System.Collections;

public class PlayerBuff : MonoBehaviour {

	// Add public variables here:
    public float persistenceTime = 8;
	
	// Add private members here:
    protected ThirdPlayerControl TPC;

	public enum BuffType
	{
		SpeedUp,
		FireRateUp,
		DamageUp,
		InfiniteBullets
	}
	
	// Add member functions here:
    IEnumerator Delay()
    {
        yield return new WaitForSeconds(persistenceTime);
		VanishBuff ();
    }

    // 初始化函数
	public void Init ()
    {
        TPC = GetComponent<ThirdPlayerControl>();
		ApplyBuff ();
        StartCoroutine(Delay());

	}

	virtual public void ApplyBuff()
	{
		Born();
		StartBuff();
	}

	virtual public void VanishBuff()
	{
		EndBuff();
		Kill();
	}

    // 出生函数
    virtual public void Born()
    {

    }

    // 开始影响
    virtual public void StartBuff()
    {

    }

    // 结束影响
    virtual public void EndBuff()
    {

    }

    // 死亡函数
    virtual public void Kill()
    {
        if (this != null)
            Destroy(this);
    }
}
