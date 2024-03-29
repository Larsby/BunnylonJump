using UnityEngine;

/* SoundSettings
 * 
 * Author: Mikael Sollenborn
 * 
 * Purpose: Keep track of whether or not music and sound effects are on in settings
 * 
 * Dependencies: GameUtil.cs
 * 
 * Note: ToggleMusic broadcasts to the method "OnMusicEnabledChangeCallback(bool)"
 */

public static class SoundSettings : System.Object {

	static bool isMusicEnabled = true;
	static bool isSoundEffectsEnabled = true;

	static bool initialized = false;

	static void Prepare()
	{
		if (initialized)
			return;
		initialized = true;

		if (!PlayerPrefs.HasKey("MusicOn"))
		{
			PlayerPrefs.SetInt("MusicOn", 1);
			PlayerPrefs.Save();
		}
		else
		{
			int musicOn = PlayerPrefs.GetInt("MusicOn");
			isMusicEnabled = musicOn == 0 ? false : true;
		}

		if (!PlayerPrefs.HasKey("SfxOn"))
		{
			PlayerPrefs.SetInt("SfxOn", 1);
			PlayerPrefs.Save();
		}
		else
		{
			int sfxOn = PlayerPrefs.GetInt("SfxOn");
			isSoundEffectsEnabled = sfxOn == 0 ? false : true;
		}
	}

	public static void ToggleMusic()
	{
		Prepare();
		isMusicEnabled = !isMusicEnabled;
		PlayerPrefs.SetInt("MusicOn", isMusicEnabled ? 1 : 0);
		PlayerPrefs.Save();

		GameUtil.BroadcastAll("OnMusicEnabledChangeCallback", isMusicEnabled);
	}

	public static void ToggleSfx()
	{
		Prepare();
		isSoundEffectsEnabled = !isSoundEffectsEnabled;
		PlayerPrefs.SetInt("SfxOn", isSoundEffectsEnabled ? 1 : 0);
		PlayerPrefs.Save();
	}

	public static bool IsMusicEnabled()
	{
		Prepare();
		return isMusicEnabled;
	}
	public static bool IsSfxEnabled()
	{
		Prepare();
		return isSoundEffectsEnabled;
	}
}
