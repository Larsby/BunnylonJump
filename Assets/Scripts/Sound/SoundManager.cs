using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* SoundManager
 * 
 * Author: Mikael Sollenborn
 * 
 * Purpose: Play music and/or sound effects
 * 
 * Dependencies: SoundManagerInitializer.cs (this is the project dependent part that sets up enums and links them to Resource paths of sound files)
 * 				 RandomNonRepeating.cs
 *				 SoundSettings.cs
 * 
 * Usage:
 *		All sounds are loaded from the "Resources" folder in "Assets", by default from "Audio/Music" for music, "Audio/Sfx" for single sound effects, "Audio/RandomSfx" for multi/random sound effects. This can be changed from SoundManagerInitializer.Initalize by calling SetResourcePathsOnInit
 *
 * Note:
 * 		SoundManager does not implement OnMusicEnabledChangeCallback (sent from SoundSettings), because it is not clear what the correct behavior would be in all cases. This has to be done manually instead if changing the setting (by calling SoundSettings.IsMusicEnabled() followed by StopMusic() and so on.
 */

public class SoundManager : MonoBehaviour {

	private AudioSource musicPlayer = null;
	private AudioSource[] randomSfx;
	private AudioSource[] singleSfx;
	private SfxRandomType[] randomSfxIndex;

	private int sourceSfxPlayerCnt = 0, singleSfxCnt = 0;
	private const int MAX_SOURCES = 8; // at the moment we have this * 2 + 1 (for music) audiosources. I.e. 17. Too many, affects performance? Decrease?

	private Dictionary<SfxRandomType, AudioClip[]> randomSfxDatabase;
	private Dictionary<SingleSfx, AudioClip> singleSfxDatabase;
	private Dictionary<MusicTune, AudioClip> musicSongsDatabase;

	private bool initialized = false;
	private static SoundManager instance = null;

	private MusicTune songPlaying = MusicTune.Undefined;

	public static SoundManager GetInstance() {
		if (instance == null) {
			GameObject g = new GameObject ();
			g.name = "SoundManager";
			g.AddComponent<SoundManager> ();
			Object.Instantiate (g);
		}
		return instance;
	}
	public static MusicTune lastMusicTune;

	void Awake ()
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
		{
			Destroy(gameObject);
			return;
		}

		DontDestroyOnLoad (gameObject);

		Initialize ();

		lastMusicTune = MusicTune.Undefined;
	}

	void Start () {}

	public class RandomSoundClipPath
	{
		public SfxRandomType sfxRandomType;
		public string [] paths;
		public RandomSoundClipPath(SfxRandomType sfxRandomType, string[] paths) { this.sfxRandomType = sfxRandomType; this.paths = paths; }
	}

	public class SingleSoundClipPath
	{
		public SingleSfx singleSfx;
		public string path;
		public SingleSoundClipPath(SingleSfx singleSfx, string path) { this.singleSfx = singleSfx; this.path = path; }
	}

	public class MusicClipPath
	{
		public MusicTune musicTune;
		public string path;
		public MusicClipPath(MusicTune musicTune, string path) { this.musicTune = musicTune; this.path = path; }
	}


	List<RandomSoundClipPath> randomSoundClipPaths;
	List<SingleSoundClipPath> singleSoundClipPaths;
	List<MusicClipPath> musicSoundPaths;

	private string randomResourcePath 	= "Audio/RandomSfx/";
	private string sfxResourcePath 		= "Audio/Sfx/";
	private string musicResourcePath 	= "Audio/Music/";

	public void SetResourcePathsOnInit(string randomPath, string sfxPath, string musicPath)
	{
		randomResourcePath = randomPath;
		sfxResourcePath = sfxPath;
		musicResourcePath = musicPath;
	}

	public void AddRandomSoundClipPathOnInit(SfxRandomType type, string [] paths)
	{
		randomSoundClipPaths.Add(new RandomSoundClipPath(type, paths));
	}

	public void AddSingleSoundClipPathOnInit(SingleSfx sfx, string path)
	{
		singleSoundClipPaths.Add(new SingleSoundClipPath(sfx, path));
	}

	public void AddMusicSoundPathOnInit(MusicTune tune, string path)
	{
		musicSoundPaths.Add(new MusicClipPath(tune, path));
	}


	private void Initialize ()
	{
		int i, j;

		if (initialized)
			return;
		initialized = true;


		// Generate text arrays by, in parent folder, write:
		// echo -n { ; for f in _matad/*.wav; do echo -n \"${f:0:${#f}-4}\", ; done ; echo }\;
		// alt:
		// echo } ; for f in _matad/*.wav; do echo \"${f:0:${#f}-4}\", ; done ; echo }\;

		randomSoundClipPaths = new List<RandomSoundClipPath> ();

		singleSoundClipPaths = new List<SingleSoundClipPath>();

		musicSoundPaths = new List<MusicClipPath>();


		SoundManagerInitializer.Initialize(this);

		randomSfxDatabase = new Dictionary<SfxRandomType, AudioClip[]>();

		GameObject g = new GameObject();
		g.name = "SoundManagerMusicPlayer";
		g.transform.SetParent(gameObject.transform);

		musicPlayer = g.AddComponent<AudioSource>();

		//musicPlayer = gameObject.AddComponent<AudioSource>();

		randomSfx = new AudioSource[MAX_SOURCES];
		singleSfx = new AudioSource[MAX_SOURCES];

		randomSfxIndex = new SfxRandomType[MAX_SOURCES];

		for (i = 0; i < MAX_SOURCES; i++) {
			randomSfx [i] = gameObject.AddComponent<AudioSource> ();
			randomSfx [i].volume = 1;
			singleSfx [i] = gameObject.AddComponent<AudioSource> ();
			singleSfx [i].volume = 1;
			randomSfxIndex [i] = SfxRandomType.None;
		}

		for (j = 0; j < randomSoundClipPaths.Count; j++) {
			if (randomSoundClipPaths [j].paths.Length == 1 && randomSoundClipPaths [j].paths [0].EndsWith ("/*")) {
				string allPath = randomResourcePath + randomSoundClipPaths [j].paths[0].Substring(0, randomSoundClipPaths [j].paths [0].Length - 1);

				Object [] allSounds = Resources.LoadAll(allPath, typeof(AudioClip));
				//print(allPath + " : " + allSounds.Length);

				AudioClip[] sounds = new AudioClip[allSounds.Length];
				for (i = 0; i < allSounds.Length; i++) {
					sounds[i] = (AudioClip)allSounds[i];
				}
				randomSfxDatabase.Add(randomSoundClipPaths[j].sfxRandomType, sounds);

			} else {
				List<AudioClip> sounds = new List<AudioClip>();

				for (i = 0; i < randomSoundClipPaths [j].paths.Length; i++) {

					string fullPath = randomResourcePath + randomSoundClipPaths [j].paths [i];

					// I think this should only be done for iOS (if string may contain special characters, like Swedish). To be tested, if necessary.
					/*	#if UNITY_IOS || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
					fullPath = fullPath.Normalize (System.Text.NormalizationForm.FormD);
					#endif */

					AudioClip sound = (AudioClip)Resources.Load (fullPath);
					if (sound == null)
						Debug.Log("Could not load audio clip: " + randomSoundClipPaths[j].paths[i] + " , index " + j + " " + i);
					else
						sounds.Add(sound);
				}
				randomSfxDatabase.Add(randomSoundClipPaths[j].sfxRandomType, sounds.ToArray());
			}
		}

		singleSfxDatabase = new Dictionary<SingleSfx, AudioClip>();
		for (i = 0; i < singleSoundClipPaths.Count; i++) {
			string fullPath = sfxResourcePath + singleSoundClipPaths [i].path;

			AudioClip sound = (AudioClip)Resources.Load (fullPath);
			if (sound == null)
				Debug.Log("Could not load single audio clip: " + singleSoundClipPaths[i].path + " , index " + i);
			else
				singleSfxDatabase.Add(singleSoundClipPaths[i].singleSfx, sound);
		}

		musicSongsDatabase = new Dictionary<MusicTune, AudioClip>();
		for (i = 0; i < musicSoundPaths.Count; i++) {
			string fullPath = musicResourcePath + musicSoundPaths [i].path;

			AudioClip sound = (AudioClip)Resources.Load (fullPath);
			if (sound == null)
				Debug.Log ("Could not load music clip: " + musicSoundPaths[i].path + " , index " + i);
			else
				musicSongsDatabase.Add(musicSoundPaths[i].musicTune, sound);
		}	
	}

	public GameObject GetMusicPlayer()
	{
		return musicPlayer.gameObject;
	}

	public int lastRandomIndex = -1;
	public float PlayRandomFromType (SfxRandomType sfxType, int forcedIndex = -1, float startDelay = 0, float volume=-1, float pitch=-1, bool randomPitch = false)
	{
		float len = 0;
		float randMax = 0.2f;
		float defaultPitch = 1;

		if (!SoundSettings.IsSfxEnabled())
			return len;

		if (!randomSfxDatabase.ContainsKey(sfxType))
			return 0;

		int index = UnityEngine.Random.Range (0, randomSfxDatabase [sfxType].Length);
		if (forcedIndex >= 0)
			index = forcedIndex;

		lastRandomIndex = index;

		randomSfx [sourceSfxPlayerCnt].clip = randomSfxDatabase [sfxType] [index];
		len = randomSfx [sourceSfxPlayerCnt].clip.length;
		if (startDelay > 0)
			randomSfx [sourceSfxPlayerCnt].PlayDelayed (startDelay);
		else
			randomSfx [sourceSfxPlayerCnt].Play ();

		if (pitch >= 0)
			defaultPitch = pitch;
		
		if (randomPitch)
			randomSfx [sourceSfxPlayerCnt].pitch = Random.Range (defaultPitch - randMax, defaultPitch + randMax);
		else
			randomSfx [sourceSfxPlayerCnt].pitch = defaultPitch;
		
		randomSfx [sourceSfxPlayerCnt].volume = volume >= 0? volume : 1;

		randomSfxIndex [sourceSfxPlayerCnt] = sfxType;

		sourceSfxPlayerCnt++;
		if (sourceSfxPlayerCnt >= MAX_SOURCES)
			sourceSfxPlayerCnt = 0;

		return len;
	}

	public int GetNofSoundsForRandomType(SfxRandomType sfxType) {
		if (!randomSfxDatabase.ContainsKey(sfxType))
			return -1;

		return randomSfxDatabase [sfxType].Length;
	}

	public bool IsPlayingRandomSfx (SfxRandomType sfxType = SfxRandomType.None)
	{
		for (int i = 0; i < MAX_SOURCES; i++)
			if (randomSfx [i].isPlaying && (sfxType == SfxRandomType.None || sfxType == randomSfxIndex[i]))
				return true;
		
		return false;
	}

	public void StopPlayingRandomSfx (SfxRandomType sfxType = SfxRandomType.None)
	{
		for (int i = 0; i < MAX_SOURCES; i++)
			if (randomSfx [i].isPlaying) {
				if (sfxType == SfxRandomType.None)
					randomSfx [i].Stop ();
				else {
					if (randomSfxIndex[i] == sfxType)
						randomSfx [i].Stop ();
				}
			}
	}


	public void StopSingleSfx (SingleSfx sfxKey = SingleSfx.None) {

		if (!singleSfxDatabase.ContainsKey(sfxKey))
			return;

		for (int i = 0; i < MAX_SOURCES; i++) {
			if ((sfxKey == SingleSfx.None || singleSfx [i].clip == singleSfxDatabase [sfxKey]) && singleSfx [i].isPlaying)
				singleSfx [i].Stop ();
		}
	}

	public float PlaySingleSfx (SingleSfx sfxKey, bool randomPitch = false, float startDelay = 0, float volume=-1, float pitch=-1)
	{
		float randMax = 0.3f;
		float len = 0;

		if (!SoundSettings.IsSfxEnabled())
			return len;

		if (!singleSfxDatabase.ContainsKey(sfxKey))
			return 0;
		
		if (singleSfx == null)
		{
			Debug.Log("SingleSFX is null");
			return 0;
		}

		var sing = singleSfx[singleSfxCnt];
		if (sing == null)
			return 0;
		
		sing.clip = singleSfxDatabase [sfxKey];
		len = singleSfx [singleSfxCnt].clip.length;
		if (randomPitch)
			sing.pitch = Random.Range (1.0f - randMax, 1.0f + randMax);
		else
			sing.pitch = 1;

		singleSfx [singleSfxCnt].volume = volume >= 0? volume : 1;
		if (pitch >= 0)
			sing.pitch = pitch;

		if (startDelay > 0)
			sing.PlayDelayed (startDelay);
		else
			singleSfx [singleSfxCnt].Play ();
		singleSfxCnt++;
		if (singleSfxCnt >= MAX_SOURCES)
			singleSfxCnt = 0;

		return len;
	}

	public float PlayMusic (MusicTune tuneKey, float volume = 0.3f)
	{
		float len = 0;

		musicPlayer.Stop ();
		musicPlayer.clip = musicSongsDatabase [tuneKey];
		musicPlayer.loop = true;
		musicPlayer.volume = volume;
		len = musicPlayer.clip.length;
		songPlaying = tuneKey;

		musicSequenceId++;

		if (SoundSettings.IsMusicEnabled())
			musicPlayer.Play();
		else
			return 0;

		return len;
	}


	private int oldMusicSequenceId;
	private void UpdateMusicPlayerVolume(float val) {
		if (oldMusicSequenceId == musicSequenceId && SoundSettings.IsMusicEnabled())
			musicPlayer.volume = val;
	}

	private int musicSequenceId = 0;
	private RandomNonRepeating musicSequenceOrderGenerator;

	private IEnumerator RunMusicSequence(int sequenceId, float fadeInTime = 0, float fadeOutTime = 0, bool fadeOnlyFirstAndLast = false)
	{
		bool firstFaded = false;
		float orgVolume = musicPlayer.volume;

		do
		{
			int index = musicSequenceOrderGenerator.GetRandom();

			musicPlayer.Stop();
			musicPlayer.clip = musicSongsDatabase[(MusicTune)index];
			float len = musicPlayer.clip.length;
			musicPlayer.Play();
			songPlaying = (MusicTune)index;

			if (fadeInTime > 0)
			{
				if (!(fadeOnlyFirstAndLast && firstFaded == true)) {
					oldMusicSequenceId = sequenceId;
					LeanTween.value(gameObject, UpdateMusicPlayerVolume, 0f, orgVolume, fadeInTime).setEase(LeanTweenType.linear);
					musicPlayer.volume = 0;
					firstFaded = true;
				}
			}

			if (fadeOutTime > 0 && !(fadeOnlyFirstAndLast && musicSequenceOrderGenerator.GetNofRemainingNumbers() == 0) ) {
				yield return new WaitForSeconds(len - fadeOutTime);
				oldMusicSequenceId = sequenceId;
				LeanTween.value(gameObject, UpdateMusicPlayerVolume, orgVolume, 0, fadeOutTime).setEase(LeanTweenType.linear);
				yield return new WaitForSeconds(fadeOutTime);
			}	else
				yield return new WaitForSeconds(len);
			
		} while (sequenceId == musicSequenceId && musicSequenceOrderGenerator.GetNofRemainingNumbers() > 0 && SoundSettings.IsMusicEnabled());
	}

	public float PlayMusicSequence(MusicTune [] tuneIndices, float volume = 0.3f, bool playRandomOrder = false, bool loop = true, float fadeInTime = 0, float fadeOutTime = 0, bool fadeOnlyFirstAndLast = false)
	{
		float len = 0;

		if (!SoundSettings.IsMusicEnabled())
			return len;
		if (tuneIndices.Length < 1)
			return len;

		int[] indices = new int[tuneIndices.Length];
		for (int i = 0; i < tuneIndices.Length; i++) {
			indices[i] = (int)tuneIndices[i];
			len += musicSongsDatabase[(MusicTune)indices[i]].length;
		}

		musicSequenceOrderGenerator = new RandomNonRepeating(indices, loop? RandomRegenerationLoop.LoopNonRepeating : RandomRegenerationLoop.None);
		if (playRandomOrder == false) musicSequenceOrderGenerator.linearNotRandom = true;

		musicPlayer.loop = false;
		musicPlayer.volume = volume;

		musicSequenceId++;
		StartCoroutine(RunMusicSequence(musicSequenceId, fadeInTime, fadeOutTime, fadeOnlyFirstAndLast));

		return len;
	}


	public MusicTune GetPlayingSong() {
		return songPlaying;
	}


	public void StopMusic ()
	{
		musicPlayer.Stop ();
	}

	public void FadeRandomPlayingSfx ()
	{

		for (int i = 0; i < MAX_SOURCES; i++)
			if (randomSfx [i].isPlaying)
				randomSfx [i].volume = 0.98f;

		Invoke ("ContinousFadeOfRandom", 0.05f);
	}

	private void ContinousFadeOfRandom ()
	{
		bool keepFading = false;
		int i;

		for (i = 0; i < MAX_SOURCES; i++) {
			if (randomSfx [i].isPlaying && randomSfx [i].volume < 1) {
				keepFading = true;
				randomSfx [i].volume -= 0.2f;
				if (randomSfx [i].volume <= 0) {
					randomSfx [i].Stop ();
					randomSfx [i].volume = 1;
				}
			}
		}
		
		if (keepFading)
			Invoke ("ContinousFadeOfRandom", 0.05f);
	}

	public void PauseMusic(bool force = false)
	{
		if (!SoundSettings.IsMusicEnabled() && !force)
			return;

		musicPlayer.Pause();
	}

	public void ResumeMusic(bool restart = false, bool force = false)
	{
		if (!SoundSettings.IsMusicEnabled() && !force)
			return;

		if (!musicPlayer.isPlaying)
		{
			if (restart)
			{
				musicPlayer.Stop();
				musicPlayer.Play();
			}
			else
				musicPlayer.UnPause();
		}
	}

	private IEnumerator FadeRandom(SfxRandomType sfxRandomType) {
		bool keepGoing;
		do {
			keepGoing = false;
			for (int i = 0; i < MAX_SOURCES; i++) {
				if ((sfxRandomType == SfxRandomType.None || randomSfxIndex [i] == sfxRandomType) && randomSfx[i].isPlaying && randomSfx[i].volume > 0) {
					randomSfx [i].volume = Mathf.Clamp01(randomSfx [i].volume - 0.2f);
					keepGoing = true;
				}
			}
			yield return new WaitForSeconds(0.05f);
		} while (keepGoing);
	}

	public void FadeRandomPlayingSfx (SfxRandomType sfxRandomType) {

		for (int i = 0; i < MAX_SOURCES; i++) {
			if ((sfxRandomType == SfxRandomType.None || randomSfxIndex [i] == sfxRandomType) && randomSfx[i].isPlaying) {
				StartCoroutine (FadeRandom(sfxRandomType));
				break;
			}
		}
	}
		
	private IEnumerator FadeSingle(SingleSfx sfx) {
		bool keepGoing;
		do {
			keepGoing = false;
			for (int i = 0; i < MAX_SOURCES; i++) {
				if ((sfx == SingleSfx.None || singleSfx [i].clip == singleSfxDatabase [sfx]) && singleSfx[i].isPlaying && singleSfx[i].volume > 0) {
					randomSfx [i].volume = Mathf.Clamp01(randomSfx [i].volume - 0.2f);
					keepGoing = true;
				}
			}
			yield return new WaitForSeconds(0.05f);
		} while (keepGoing);
	}

	public void FadeSingleSfx (SingleSfx sfx = SingleSfx.None) {
		for (int i = 0; i < MAX_SOURCES; i++) {
			if ((sfx == SingleSfx.None || singleSfx [i].clip == singleSfxDatabase [sfx]) && singleSfx[i].isPlaying) {
				StartCoroutine (FadeSingle(sfx));
				break;
			}
		}
	}

	public void StopAll() {
		StopPlayingRandomSfx ();
		StopSingleSfx ();
	}
	public void FadeAll() {
		FadeRandomPlayingSfx ();
		FadeSingleSfx ();
	}

}
