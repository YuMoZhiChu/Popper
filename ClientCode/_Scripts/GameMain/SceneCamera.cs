using UnityEngine;
using System.Collections;
using DG.Tweening;

public class SceneCamera : MonoBehaviour {

	// Use this for initialization
	void Start () {
		transform.DORotate (new Vector3 (0, 60, 0), 5, RotateMode.FastBeyond360).SetEase (Ease.Linear).SetLoops (10000, LoopType.Incremental);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
