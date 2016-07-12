using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class DelayAudioPlay : MonoBehaviour {

    public float time = 0;
    public AudioSource AS;

	// Use this for initialization
	void Start () {
        AS.playOnAwake = false;
        AS.Stop();
        StartCoroutine(play());
	}

    IEnumerator play()
    {
        yield return new WaitForSeconds(time);
        AS.Play();
    }
}
