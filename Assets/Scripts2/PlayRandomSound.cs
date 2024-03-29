using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayRandomSound : MonoBehaviour
{
	public AudioClip[] clips;
	public AudioClip current;
	private AudioSource source;
	public float randMax = 0;
	// Use this for initialization
	void Start ()
	{
		source = GetComponent<AudioSource> ();
	}

	public void Play ()
	{
		if (GameManager.instance != null && GameManager.instance.SFXEnabled () == false)
			return;
		source.enabled = true;
		if (source.isPlaying) {
			//source.Stop ();

		}
		source.clip = clips [Random.Range (0, clips.Length)];

		source.Play ();

		source.pitch = Random.Range (1.0f - randMax, 1.0f + randMax);

	}
}
