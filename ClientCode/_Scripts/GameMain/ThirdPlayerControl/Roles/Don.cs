using UnityEngine;
using System.Collections;

public class Don : RoleControl {

	public GameObject Hero;
	public GameObject donGun;
	public GameObject bullet;
	public GameObject insaneBullet;

	[System.Serializable]
	public class ParticlesFX
	{
		public GameObject NormalFireShootFX;
		public GameObject[] xuli;
		public GameObject UltFX;
		public GameObject walkSomke;
		public GameObject UltGunFX;
	}
	public ParticlesFX FX;

	private GameObject zhunbeixuli;
	private GameObject baochixuli;
	private Rigidbody rg;



	override public GameObject getGun ()
	{
		return donGun;
	}

	override public void NormalFire(Vector3 gunposition, Vector3 aim)
	{
		Vector3 ap = Hero.GetComponent<ThirdPlayerControl> ().fireTargetPoint;
		GameObject smallBullet = Instantiate(bullet, donGun.transform.position, Quaternion.identity) as GameObject;
		/*smallBullet.transform.LookAt(ap);
		smallBullet.transform.SetParent(GameControl.instance.bulletsLayer);
		SmallBulletMove sbm = smallBullet.GetComponent<SmallBulletMove> ();
		sbm.aimPoint = ap;
		sbm.damage = Hero.GetComponent<ThirdPlayerControl>().roleAttr.weaponAttr.normalFireDamage;
		sbm.damageResource = Hero;*/

		SmallBulletMove.Set (smallBullet, ap, GameControl.instance.bulletsLayer, Hero.GetComponent<ThirdPlayerControl> ().roleAttr.weaponAttr.normalFireDamage, Hero);

		Instantiate(FX.NormalFireShootFX, donGun.transform.position, Hero.transform.rotation);
	}

	override public void InsaneFire(Vector3 gunposition, Vector3 aim)
	{
		Vector3 ap = Hero.GetComponent<ThirdPlayerControl> ().fireTargetPoint;
		GameObject smallBullet = Instantiate(insaneBullet, donGun.transform.position, Quaternion.identity) as GameObject;
		/*smallBullet.transform.LookAt(ap);
		smallBullet.transform.SetParent(GameControl.instance.bulletsLayer);
		smallBullet.GetComponent<SmallBulletMove>().aimPoint = ap;
		smallBullet.GetComponent<SmallBulletMove> ().damage = Hero.GetComponent<ThirdPlayerControl> ().roleAttr.weaponAttr.InsaneFireDamage;
		*/

		SmallBulletMove.Set (smallBullet, ap, GameControl.instance.bulletsLayer, Hero.GetComponent<ThirdPlayerControl> ().roleAttr.weaponAttr.InsaneFireDamage, Hero);

		if (baochixuli != null)
			Destroy (baochixuli);
		Instantiate (FX.xuli [2], donGun.transform.position, Hero.transform.rotation);
	}

	override public void UltFire(Vector3 gunposition, Vector3 aim)
	{
		//5yue6hao
		Hero.AddComponent<SpeedUp>().SpeedIncrease = 9;
		Hero.AddComponent<FireRateUp> ().RateIncreasePercentage = 100f;
		Hero.AddComponent<DamageUp> ().DamageIncrease = 5;
		Hero.AddComponent<InfiniteBullets> ();
		GameObject go = Instantiate (FX.UltFX, Hero.transform.position, Quaternion.identity) as GameObject;
		go.transform.position = Hero.transform.position + Hero.transform.TransformVector(new Vector3 (0, 0.25f, 1.5f));

		GameObject goo = Instantiate (FX.UltGunFX, donGun.transform.position, Quaternion.identity) as GameObject;
		goo.transform.SetParent(donGun.transform);
		DelayDie dd = goo.AddComponent<DelayDie> ();
		dd.selfDie = true;
		dd.delayDieTime = 8;
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
            if (baochixuli != null)
                Destroy(baochixuli);
		} else {
			if (jieduan == 1) {
				baochixuli = Instantiate (FX.xuli [jieduan], donGun.transform.position, Quaternion.identity) as GameObject;
				baochixuli.transform.SetParent (donGun.transform);
			} else if (jieduan == 0) {
				zhunbeixuli = Instantiate (FX.xuli [jieduan], donGun.transform.position, Quaternion.identity) as GameObject;
				zhunbeixuli.transform.SetParent (donGun.transform);
			}
		}
	}
	#endregion

	// Use this for initialization
	void Start () {
		Hero.GetComponent<ThirdPlayerControl>().XuLiEvent += XuLiFX;
		rg = Hero.GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (rg.velocity.sqrMagnitude <= 0.01f) {
			FX.walkSomke.SetActive (false);
		} else {
			FX.walkSomke.SetActive (true);
		}
	}

	void OnDestroy()
	{
		Hero.GetComponent<ThirdPlayerControl>().XuLiEvent -= XuLiFX;
	}
}
