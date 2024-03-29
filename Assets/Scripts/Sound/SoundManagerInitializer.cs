using UnityEngine;

/* SoundManagerInitializer / sound enums
 * 
 * Author: Mikael Sollenborn
 * 
 * Purpose: Does the project specific setup for sounds, creating enums and paths to the sounds
 * 
 * Dependencies: SoundManager.cs
 * 
 * Usage:
 *   Note that ALL setup must be done from within Initialize. Calling e.g AddRandomSoundClipPathOnInit at a later stage will have no effect.
 * 
 *   All sounds are loaded from the "Resources" folder in "Assets", by default from "Audio/Music" for music, "Audio/Sfx" for single sound effects, "Audio/RandomSfx" for multi/random sound effects. If needed, this can be changed in Initalize by calling SetResourcePathsOnInit
 *
 *   For AddRandomSoundClipPathOnInit, providing a folder name ending with "/*" means to load all sound files in that folder. Individula files can also be put into the string array
 */

public enum SfxRandomType
{
	None = -1,
	Jump,
};

public enum SingleSfx
{
	None = -1,
	Button1, Button2, Fail
};

public enum MusicTune
{
	Undefined = -1,
	Regular,
};

public class SoundManagerInitializer : MonoBehaviour {

	// Example Initialize
	public static void Initialize(SoundManager sm) {
		
		// sm.SetResourcePathsOnInit("Audio/RandomSfx/", "Audio/Sfx/", "Audio/Music/");

		sm.AddRandomSoundClipPathOnInit(SfxRandomType.Jump, new string[] { "Jump/*" });

		sm.AddSingleSoundClipPathOnInit(SingleSfx.Button1, "button01");
		sm.AddSingleSoundClipPathOnInit(SingleSfx.Button2, "button02");
		sm.AddSingleSoundClipPathOnInit(SingleSfx.Fail, "fail");

		sm.AddMusicSoundPathOnInit(MusicTune.Regular, "Theme");
	}
}
