using UnityEngine;
using System.Collections;

public class Hunk : RoleControl {

	public GameObject Hero;
	public GameObject hunkGun;
	public GameObject bullet;
	public GameObject insaneBullet;
	public GameObject UltBullet;



	[System.Serializable]
	public class ParticlesFX
	{
		public GameObject NormalFireShootFX;
		public GameObject[] xuli;
		public GameObject UltFX;
		public GameObject walkSomke;
		public GameObject UltGunFX;
		public GameObject UltShootFX;
		public GameObject huohuaFX;
	}
	public ParticlesFX FX;

	private GameObject zhunbeixuli;
	private GameObject baochixuli;



	private Camera followCamera;
	private Ray fireRay;
	private RaycastHit fireTargetPointRayHit;
	private Vector3 fireTargetPoint;



	override public GameObject getGun ()
	{
		return hunkGun;
	}

	override public void NormalFire(Vector3 gunposition, Vector3 aim)
	{
		Vector3 ap = Hero.GetComponent<ThirdPlayerControl> ().fireTargetPoint;
		GameObject smallBullet = Instantiate(bullet, hunkGun.transform.position, Quaternion.identity) as GameObject;
		/*smallBullet.transform.LookAt(ap);
		smallBullet.transform.SetParent(GameControl.instance.bulletsLayer);
		smallBullet.GetComponent<SmallBulletMove>().aimPoint = ap;*/

		SmallBulletMove.Set (smallBullet, ap, GameControl.instance.bulletsLayer, Hero.GetComponent<ThirdPlayerControl> ().roleAttr.weaponAttr.normalFireDamage, Hero);

		Instantiate(FX.NormalFireShootFX, hunkGun.transform.position, Hero.transform.rotation * Quaternion.Euler(0, 90, 0));
	}

	override public void InsaneFire(Vector3 gunposition, Vector3 aim)
	{
        Vector3 ap = Hero.GetComponent<ThirdPlayerControl>().fireTargetPoint;

		GameObject smallBullet = Instantiate(insaneBullet, hunkGun.transform.position, Quaternion.identity) as GameObject;
		smallBullet.transform.rotation = Hero.transform.rotation;
		smallBullet.transform.SetParent(GameControl.instance.bulletsLayer);
		smallBullet.transform.GetChild(0).GetComponent<HunkVirtualBullet>().damage = Hero.GetComponent<ThirdPlayerControl> ().roleAttr.weaponAttr.InsaneFireDamage;
        smallBullet.transform.GetChild(0).GetComponent<HunkVirtualBullet>().damageResource = Hero;
        smallBullet.transform.GetChild(0).GetComponent<HunkVirtualBullet>().direction = ap;

		if (baochixuli != null)
			Destroy (baochixuli);
		GameObject go = Instantiate (FX.xuli [2], hunkGun.transform.position, Hero.transform.rotation) as GameObject;
        go.transform.LookAt(ap);
	}

	override public void UltFire(Vector3 gunposition, Vector3 aim)
	{
		StartCoroutine (SoManyBullets (aim));
		GameObject go = Instantiate (FX.UltFX, Hero.transform.position, Hero.transform.rotation) as GameObject;
		go.transform.position = Hero.transform.position + Hero.transform.TransformVector(new Vector3 (0, 0.25f, 1.5f));
		go.transform.SetParent (Hero.transform);

		go = Instantiate (FX.UltGunFX, hunkGun.transform.position, Quaternion.identity) as GameObject;
		go.transform.SetParent (hunkGun.transform);

		go = Instantiate(FX.UltShootFX, hunkGun.transform.position, Hero.transform.rotation) as GameObject;
		go.transform.SetParent (hunkGun.transform);
	}

	IEnumerator SoManyBullets(Vector3 aim)
	{
		while (isUlt == true) {
			Vector3 ap = Hero.GetComponent<ThirdPlayerControl> ().fireTargetPoint;
			ABullet (ap);
			yield return new WaitForSeconds (0.125f);
		}
	}

	void ABullet(Vector3 aim)
	{
		GameObject smallBullet = Instantiate(UltBullet, hunkGun.transform.position, Quaternion.identity) as GameObject;
		/*smallBullet.transform.LookAt(aim);
		smallBullet.transform.SetParent(GameControl.instance.bulletsLayer);
		smallBullet.GetComponent<SmallBulletMove>().aimPoint = aim;
		smallBullet.GetComponent<SmallBulletMove> ().damage = Hero.GetComponent<ThirdPlayerControl>().roleAttr.weaponAttr.UltFireDamage;*/

		SmallBulletMove.Set (smallBullet, aim, GameControl.instance.bulletsLayer, Hero.GetComponent<ThirdPlayerControl> ().roleAttr.weaponAttr.UltFireDamage, Hero);
	}

	#region xulitexiao
	override public void XuLiFX(int jieduan)
	{
		if (jieduan == 3) {  // quxiao
			if (zhunbeixuli != null) {
				foreach (ParticleSystem ps in zhunbeixuli.GetComponentsInChildren<ParticleSystem>()) {
					ps.enableEmission = false;
				}
			}
		} else {
			if (jieduan == 1) {
				baochixuli = Instantiate (FX.xuli [jieduan], hunkGun.transform.position, Quaternion.identity) as GameObject;
				baochixuli.transform.SetParent (hunkGun.transform);
			} else if (jieduan == 0) {
				zhunbeixuli = Instantiate (FX.xuli [jieduan], hunkGun.transform.position, Quaternion.identity) as GameObject;
				zhunbeixuli.transform.SetParent (hunkGun.transform);
			}
		}
	}
	#endregion

	void huohua()
	{
		Instantiate (FX.huohuaFX, hunkGun.transform.position, Quaternion.identity);
	}

	void stopULT()
	{
		isUlt = false;
	}

	// Use this for initialization
	void Start () {
		Hero.GetComponent<ThirdPlayerControl>().XuLiEvent += XuLiFX;
		followCamera = Hero.GetComponent<ThirdPlayerControl>().sight.transform.FindChild("Camera").GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnDestroy()
	{
		Hero.GetComponent<ThirdPlayerControl>().XuLiEvent -= XuLiFX;
	}
}
