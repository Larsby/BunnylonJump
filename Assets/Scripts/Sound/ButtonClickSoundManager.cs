using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* ButtonClickSoundManager
 * 
 * Author: Mikael Sollenborn
 * 
 * Purpose: Automatically assign one or more sounds to all buttons in the scene
 * 
 * Dependencies: SoundManagerInitializer.cs
 * 				 SoundManager.cs
 * 
 * Usage:
 *		 By default all buttons get the "sfx" sound. Create arrays in the component to change the sound for individual buttons
 */

public class ButtonClickSoundManager : MonoBehaviour {

	public SingleSfx sfx = SingleSfx.None;
	public SingleSfx altSfx = SingleSfx.None;
	public bool randomPitch = true;
	public List<Button> altSoundButtons = new List<Button>();
	public List<Button> noSoundButtons = new List<Button>();

	public List<Button> specificSoundButtons = new List<Button>();
	public List<SingleSfx> specificSoundSfx = new List<SingleSfx>();

	void Start () {
		Invoke ("AssignButtonSounds", 0.1f);
	}

	void AssignButtonSounds () {
		Button[] allButtons = Resources.FindObjectsOfTypeAll<Button> ();

		foreach (Button b in allButtons) {

			GameObject go = b.gameObject;

			if (go.hideFlags != HideFlags.None)
				continue;

			if (noSoundButtons != null && noSoundButtons.Contains (b))
				continue;

			if (specificSoundButtons != null && specificSoundButtons.IndexOf (b) != -1) {
				int index = specificSoundButtons.IndexOf (b);
				b.onClick.AddListener (() =>
					{
						PlaySpecificSound (index);
					});
			} else if (altSoundButtons != null && altSoundButtons.Contains (b))
				b.onClick.AddListener (PlayAltSound);
			else
				b.onClick.AddListener (PlayDefaultSound);
		}
	}
		
	public void PlayDefaultSound() {
		SoundManager.GetInstance().PlaySingleSfx (sfx, randomPitch);
	}

	public void PlayAltSound() {
		SoundManager.GetInstance().PlaySingleSfx (altSfx, randomPitch);
	}

	public void PlaySpecificSound(int index) {
		if (index >= 0 && index < specificSoundSfx.Count)
		{
			SoundManager.GetInstance().PlaySingleSfx(specificSoundSfx[index], false);
		}
	}

}
