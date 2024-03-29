using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public static GameManager instance = null;
	public GameObject soundButton;
	public GameObject sfxButton;




	public GameObject LoadLevelObj;
	public GameObject menuButton;

	public  GameObject[] sounds;
	public  GameObject[] sfx;
	private bool sfxEnabled;
	private bool soundEnabled;

	void Awake ()
	{

		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);    



	}

	public void SavePrefs ()
	{
		SavePrefs (soundEnabled, sfxEnabled);	
	}

	void SavePrefs (bool music, bool sfx)
	{
		PlayerPrefs.SetInt ("music", music == true ? 1 : 0);
		PlayerPrefs.SetInt ("sfx", sfx == true ? 1 : 0);
		PlayerPrefs.Save ();
	}

	void SetButtonState (GameObject obj, bool active)
	{
		int activeIndex = active == true ? 0 : 1;
		int inactiveIndex = active == true ? 1 : 0;
		Image img = obj.transform.GetChild (inactiveIndex).gameObject.GetComponent<Image> ();
		img.enabled = false;
		img = obj.transform.GetChild (activeIndex).gameObject.GetComponent<Image> ();
		img.enabled = true;
	}

	public void SetMenuState ()
	{

		SetButtonState (soundButton, soundEnabled);
		SetButtonState (sfxButton, sfxEnabled);
		SavePrefs (soundEnabled, sfxEnabled);

	}

	public void ReturnToMainMenu ()
	{
		SavePrefs ();
		LoadLevelObj.GetComponent<LoadGame> ().LoadMenu ();

	}


	// Use this for initialization
	void Start ()
	{
		
	
	}

	public bool SFXEnabled ()
	{
		return sfxEnabled;
	}

	public bool SoundEnabled ()
	{
		return soundEnabled;
	}



	public void Restart ()
	{
		
	}



	public void SetSFX (bool b)
	{
		foreach (GameObject obj in sfx) {
			if (obj != null)
				obj.GetComponent<AudioSource> ().enabled = b;
		}
	}

	public void SFX ()
	{ 
		sfxEnabled = !sfxEnabled;
		SetSFX (sfxEnabled);
		SetMenuState ();
	}

	void SetSound (bool b)
	{
		if (sounds == null || sounds.Length == 0)
			return;
		foreach (GameObject obj in sounds) {
			if (obj != null)
			{
				obj.SetActive(b);
				obj.GetComponent<AudioSource>().enabled = b;
				if (b == true)
				{
					obj.GetComponent<AudioSource>().volume = 1.0f; //0.4 works much better for the volume for the song.
					obj.GetComponent<AudioSource>().Play();
				}
				else
				{
					obj.GetComponent<AudioSource>().volume = 0.0f;
				}
			}
		}
		SetMenuState ();

	}

	public void Sound ()
	{
		soundEnabled = !soundEnabled;
		SetSound (soundEnabled);
		SetMenuState ();
	}






}
