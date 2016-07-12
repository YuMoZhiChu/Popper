using UnityEngine;
using System.Collections;

public class RoleControl : MonoBehaviour {

	[System.Serializable]
	public class RoleAttributes
	{
		public int roleJumpCounts = 1;
		public float roleJumpDelay = 0.1f;
		public float roleJumpHeight = 4;
		public float roleMoveSpeed = 3;

		[System.Serializable]
		public class WeaponAttributes
		{
			public float roleFireRate = 0.2f;
			public int maxBulletsClips = 7;
			public float reloadClipTime = 3;
			public float normalFireDamage = 10;
			public float normalFireRange = 60;
			public float InsaneFireDamage = 20;
			public float UltFireDamage = 50;
			public float rolePrepareInsaneFireTime = 3;
		}
		public WeaponAttributes weaponAttr;
	}
	public RoleAttributes roleAttr;

    [System.NonSerialized]
	public bool isUlt = false;
    [System.NonSerialized]
    public bool isXuLiIng = false;

	public AudioClip reloadClip;

	virtual public void NormalFire(Vector3 gunposition, Vector3 aim)
	{
		
	}

	virtual public void InsaneFire(Vector3 gunposition, Vector3 aim)
	{

	}

	virtual public void UltFire(Vector3 gunposition, Vector3 aim)
	{

	}

	virtual public void XuLiFX(int jieduan)
	{

	}

	virtual public GameObject getGun()
	{
		return null;
	}

}
