using UnityEngine;
using System.Collections;

public class Betty : RoleControl {

	public GameObject Hero;
	public GameObject[] bettyGun;
	public GameObject bullet;
	public GameObject bettyInsaneBullet;
	public GameObject UltBullet;
	public GameObject RayPoint;
	public float insanebettytime = 1;
	public int insanebettyBullets = 12;
	public float insanebettyRange = 15;

	private int gunID = 1;


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


	private GameObject zhunbeixuli0;
	private GameObject baochixuli0;
	private GameObject zhunbeixuli1;
	private GameObject baochixuli1;

	private Ray ray = new Ray ();
	private RaycastHit rayHit = new RaycastHit ();

	int ToggleGun()
	{
		//int id = gunID;
		if (gunID == 0)
			gunID = 1;
		else
			gunID = 0;
		return gunID;
	}

	public override GameObject getGun ()
	{
		return bettyGun[gunID];
	}

	override public void NormalFire(Vector3 gunposition, Vector3 aim)
	{
		Vector3 ap = Hero.GetComponent<ThirdPlayerControl> ().fireTargetPoint;
		GameObject smallBullet = Instantiate(bullet, bettyGun[ToggleGun()].transform.position, Quaternion.identity) as GameObject;
		/*smallBullet.transform.LookAt(ap);
		smallBullet.transform.SetParent(GameControl.instance.bulletsLayer);
		smallBullet.GetComponent<SmallBulletMove>().aimPoint = ap;
		smallBullet.GetComponent<SmallBulletMove> ().damage = Hero.GetComponent<ThirdPlayerControl>().roleAttr.weaponAttr.normalFireDamage;*/

		SmallBulletMove.Set (smallBullet, ap, GameControl.instance.bulletsLayer, Hero.GetComponent<ThirdPlayerControl> ().roleAttr.weaponAttr.normalFireDamage, Hero);

		Instantiate(FX.NormalFireShootFX, bettyGun[gunID].transform.position, Hero.transform.rotation * Quaternion.Euler(0, 180, 0));
	}

	override public void InsaneFire(Vector3 gunposition, Vector3 aim)
	{
		//GameControl.instance.someFunctions.AddAreaDamage(transform.position, 4, -roleAttr.weaponAttr.InsaneFireDamage);

		if (baochixuli0 != null)
			Destroy (baochixuli0);
		GameObject go = Instantiate (FX.xuli [2], bettyGun[0].transform.position, Quaternion.identity) as GameObject;
		go.transform.SetParent (bettyGun [0].transform);
		if (baochixuli1 != null)
			Destroy (baochixuli1);
		go = Instantiate (FX.xuli [2], bettyGun[1].transform.position, Quaternion.identity) as GameObject;
		go.transform.SetParent (bettyGun [1].transform);

		StartCoroutine (insanebetty ());
	}

	IEnumerator insanebetty()
	{
		int count = insanebettyBullets;
		while (count-- > 0) {
			Vector3 aim = RayPoint.transform.TransformPoint (RayPoint.transform.localPosition + new Vector3 (0, 0, insanebettyRange));
			GameObject smallBullet = Instantiate(bettyInsaneBullet, bettyGun[ToggleGun()].transform.position, Quaternion.identity) as GameObject;
			/*smallBullet.transform.LookAt(aim);
			smallBullet.transform.SetParent(GameControl.instance.bulletsLayer);
			smallBullet.GetComponent<SmallBulletMove>().aimPoint = aim;
			smallBullet.GetComponent<SmallBulletMove> ().damage = Hero.GetComponent<ThirdPlayerControl>().roleAttr.weaponAttr.InsaneFireDamage;*/

			SmallBulletMove.Set (smallBullet, aim, GameControl.instance.bulletsLayer, Hero.GetComponent<ThirdPlayerControl> ().roleAttr.weaponAttr.InsaneFireDamage, Hero);

			Instantiate(FX.UltGunFX, bettyGun[gunID].transform.position, Hero.transform.rotation * Quaternion.Euler(0, 180, 0));

			RayPoint.transform.Rotate (0, -360.0f / insanebettyBullets, 0);

			yield return new WaitForSeconds (insanebettytime / insanebettyBullets);
		}
	}

	override public void UltFire(Vector3 gunposition, Vector3 aim)
	{
		StartCoroutine (SoManyBullets (aim));
		GameObject go = Instantiate (FX.UltFX, bettyGun[0].transform.position, Quaternion.identity) as GameObject;
		go.transform.SetParent (bettyGun [0].transform);
		go = Instantiate (FX.UltFX, bettyGun[1].transform.position, Quaternion.identity) as GameObject;
		go.transform.SetParent (bettyGun [1].transform);
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
		GameObject smallBullet = Instantiate(UltBullet, bettyGun[ToggleGun()].transform.position, Quaternion.identity) as GameObject;
		/*smallBullet.transform.LookAt(aim);
		smallBullet.transform.SetParent(GameControl.instance.bulletsLayer);
		smallBullet.GetComponent<SmallBulletMove>().aimPoint = aim;
		smallBullet.GetComponent<SmallBulletMove> ().damage = Hero.GetComponent<ThirdPlayerControl>().roleAttr.weaponAttr.UltFireDamage;*/

		SmallBulletMove.Set (smallBullet, aim, GameControl.instance.bulletsLayer, Hero.GetComponent<ThirdPlayerControl> ().roleAttr.weaponAttr.UltFireDamage, Hero);

		Instantiate(FX.UltGunFX, bettyGun[gunID].transform.position, Hero.transform.rotation * Quaternion.Euler(0, 180, 0));
	}

	#region xulitexiao
	override public void XuLiFX(int jieduan)
	{
		if (jieduan == 3) {  // quxiao
			if (zhunbeixuli0 != null) {
				foreach (ParticleSystem ps in zhunbeixuli0.GetComponentsInChildren<ParticleSystem>()) {
					ps.enableEmission = false;
				}
			}
			if (zhunbeixuli1 != null) {
				foreach (ParticleSystem ps in zhunbeixuli1.GetComponentsInChildren<ParticleSystem>()) {
					ps.enableEmission = false;
				}
			}
		} else {
			if (jieduan == 1) {
				baochixuli0 = Instantiate (FX.xuli [jieduan], bettyGun[0].transform.position, Quaternion.identity) as GameObject;
				baochixuli0.transform.SetParent (bettyGun[0].transform);
				baochixuli1 = Instantiate (FX.xuli [jieduan], bettyGun[1].transform.position, Quaternion.identity) as GameObject;
				baochixuli1.transform.SetParent (bettyGun[1].transform);
			} else if (jieduan == 0) {
				zhunbeixuli0 = Instantiate (FX.xuli [jieduan], bettyGun[0].transform.position, Quaternion.identity) as GameObject;
				zhunbeixuli0.transform.SetParent (bettyGun[0].transform);
				zhunbeixuli1 = Instantiate (FX.xuli [jieduan], bettyGun[1].transform.position, Quaternion.identity) as GameObject;
				zhunbeixuli1.transform.SetParent (bettyGun[1].transform);
			}
		}
	}
	#endregion

	// Use this for initialization
	void Start () {
		Hero.GetComponent<ThirdPlayerControl>().XuLiEvent += XuLiFX;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnDestroy()
	{
		Hero.GetComponent<ThirdPlayerControl>().XuLiEvent -= XuLiFX;
	}
}
