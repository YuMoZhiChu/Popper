using UnityEngine;
using System.Collections;
using DG.Tweening;

public class FeiTingXuanZhuan : MonoBehaviour {

	public float time = 300;

	// Use this for initialization
	void Start () {
		this.transform.DORotate (new Vector3 (0, -360, 0), time, RotateMode.FastBeyond360).SetEase (Ease.Linear).SetLoops (1000);
	}

}
