using UnityEngine;
using UnityEngine.Audio;

/* SongMakerScript
 * 
 * Author: Johan Larsby
 * 
 * Purpose: Play a song based on patterns and randomness 
 * 
 * Dependencies: SoundSettings.cs (to keep track of if sound is enabled or not)
 * 
 * Usage:
 *		Fill in the AudioSource arrays in the Inspector
 *
 * Todo:
 *		Should this be a singleton/DontDestroyOnLoad?
 */

public class SongMakerScript : MonoBehaviour
{
    public AudioSource[] drums;
    public AudioSource[] bas;
    public AudioSource[] lead;
    public AudioSource[] pad;
    public AudioMixer audioMixer;

    float drumScale = 0.010F;
    float bassScale = 0.020F;
    float padScale = 0.015F;
    float LeadScale = 0.015F;

	bool isDisabled = false;

    void Start()
    {
        Muteall();

		OnMusicEnabledChangeCallback(SoundSettings.IsMusicEnabled());
    }

	// Broadcast call from SoundManager (as well as called from Start)
	public void OnMusicEnabledChangeCallback(bool isOn)
	{
		if (isOn)
		{
			Muteall();
			drums[Random.Range(0, drums.Length)].volume = 1.0f;
			bas[Random.Range(0, bas.Length)].volume = 1.0f;
			pad[Random.Range(0, pad.Length)].volume = 1.0f;
			isDisabled = false;
		}
		else { 
			Muteall();
			isDisabled = true;
		}
	}

    void Muteall()
    {
        for (int i = 0; i < drums.Length; i++)
        {
	       drums[i].volume = 0;
        }

        for (int i = 0; i < bas.Length; i++)
        {
            bas[i].volume = 0;
        }

        for (int i = 0; i < lead.Length; i++)
        {
            lead[i].volume = 0;
        }

        for (int i = 0; i < pad.Length; i++)
        {
            pad[i].volume = 0;
        }
    }

    public void LowpassUs()
    {
        // audioLowPassFilter.cutoffFrequency = 100.0f;
        audioMixer.SetFloat("lpMaster", 500.0f);
    }

    public void unLowpassUs()
    {
        audioMixer.SetFloat("lpMaster", 44100.0f);
    }

    void Update()
    {
		if (isDisabled)
			return;

        //playing random set
        if (bas[0].timeSamples > 1815552)
        {

            Muteall();
            drums[Random.Range(0, drums.Length)].volume = 1.0f;
            bas[Random.Range(0, bas.Length)].volume = 1.0f;
            pad[Random.Range(0, pad.Length)].volume = 1.0f;

        }

        if (drums[0].volume > 0.0f)
        {
            drums[0].volume = Mathf.PerlinNoise(Time.time * drumScale, 0.0F);
        }
        else
        {
            drums[1].volume = Mathf.PerlinNoise(Time.time * drumScale, 0.0F);
        }

        if (pad[0].volume > 0.0f)
        {
            pad[0].volume = Mathf.PerlinNoise(Time.time * padScale, 0.0F);
        }
        else
        {
            pad[1].volume = Mathf.PerlinNoise(Time.time * padScale, 0.0F);
        }


        if (bas[0].volume > 0.0f)
        {
            bas[0].volume = Mathf.PerlinNoise(Time.time * bassScale, 0.0F);
        }
        else if (bas[1].volume > 0.0f)
        {
            bas[1].volume = Mathf.PerlinNoise(Time.time * bassScale, 0.0F);
        }
        else
        {
            bas[2].volume = Mathf.PerlinNoise(Time.time * bassScale, 0.0F);
        }


        //lead
        if (lead.Length != 0)
        {
            lead[0].volume = Mathf.PerlinNoise(Time.time * LeadScale, 0.2F);
            if (lead[0].volume > 0.75)
            {
                lead[1].volume = 1.0f;
                lead[0].volume = 0.0f;
            }
            else if (lead[0].volume > 0.5)
            {
                lead[1].volume = 0.0f;
                lead[0].volume = 1.0f;
            }
            else
            {
                lead[1].volume = 0.0f;
                lead[0].volume = 0.0f;
            }
        }
    }
}
