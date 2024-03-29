using UnityEngine;
using UnityEngine.UI;

/* SettingsHandler
 * 
 * Author: Mikael Sollenborn
 * 
 * Purpose: Handles the two very common cases of turning sound and music on/off and saving/restoring these settings
 * 
 * Override this class to support other types/more buttons (see example SettingsHandlerExtended)
 *   
 */

public class SettingsHandler : MonoBehaviour
{
	public GameObject panel;
	public Button toggleButton;
	public Sprite showSprite;
	public Sprite hideSprite;

	public Button musicSettingButton;
	public Button sfxSettingButton;

	private bool toggle = true;

	// this is only here for testing purposes
	public MusicTune TESTONLY_musicTune = MusicTune.Undefined;


	protected void Start () {
		toggleButton.onClick.AddListener(ToggleMenu);

		if (musicSettingButton)
			musicSettingButton.onClick.AddListener(MusicToggle);
		if (sfxSettingButton)
			sfxSettingButton.onClick.AddListener(SfxToggle);

		if (TESTONLY_musicTune != MusicTune.Undefined)
			SoundManager.GetInstance().PlayMusic(TESTONLY_musicTune);
	}

	public void ToggleMenu ()
	{
		panel.SetActive(toggle);
		if (toggle) {
			SetMenuState();
			toggleButton.image.sprite = hideSprite;
		} else { 
			toggleButton.image.sprite = showSprite;
		}
		toggle = !toggle;
	}

	void Update()
	{
#if UNITY_TVOS
		if (Input.GetKeyDown (KeyCode.JoystickButton0)) { // click on "Menu"
			ToggleMenu();
		}
#endif
	}

	protected void SetMenuButtonState(Button obj, bool active)
	{
		int activeIndex = active == true ? 0 : 1;
		int inactiveIndex = active == true ? 1 : 0;
		Image img = obj.transform.GetChild(inactiveIndex).gameObject.GetComponent<Image>();
		img.enabled = false;
		img = obj.transform.GetChild(activeIndex).gameObject.GetComponent<Image>();
		img.enabled = true;
	}


	virtual protected void SetMenuState()
	{
		if (musicSettingButton)
			SetMenuButtonState(musicSettingButton, SoundSettings.IsMusicEnabled());
		if (sfxSettingButton)
			SetMenuButtonState(sfxSettingButton, SoundSettings.IsSfxEnabled());
	}


	public void MusicToggle()
	{
		SoundSettings.ToggleMusic();
		SetMenuState();
	}

	// broadcasted by SoundSettings after ToggleMusic
	public void OnMusicEnabledChangeCallback(bool isOn)
	{
		if (isOn)
		{
			SoundManager.GetInstance().ResumeMusic(true, true);
		}
		else
		{
			SoundManager.GetInstance().PauseMusic(true);
		}
	}

	public void SfxToggle()
	{
		SoundSettings.ToggleSfx();
		SetMenuState();
	}

}
