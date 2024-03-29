using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGame : MonoBehaviour
{
    public enum Fade
    {
        In,
        Out
    }

    ;
    // test
    float fadeTime = 0.100F;
    public AudioSource myAudio;
    public Canvas tehCaller;
    public AudioSource buttonsound;

    public float songMaxVolume = 0.385f;
    public string mainMenuName;
    public string firstSceneName;

    public IEnumerator FadeAudio(AudioSource source, float timer, Fade fadeType, bool destroySource)
    {
        float start = fadeType == Fade.In ? 0.0F : 1.0F;
        float end = fadeType == Fade.In ? 1.0F : 0.0F;
        float i = 0.0F;
        float step = 1.0F / timer;

        while (i <= 1.0F)
        {
            i += step * Time.deltaTime;
            source.volume = Mathf.Lerp(start, end, i) * songMaxVolume;
            yield return new WaitForSeconds(step * Time.deltaTime);
        }
        if (destroySource)
        {
            Destroy(source);
        }
        //	Destroy (tehCaller);


    }


    public IEnumerator LoadAndDim(float timer, string scene)
    {

        Fader f = gameObject.GetComponent<Fader>();
        f.fade = true;
        f.dir = 1;
        yield return new WaitForSeconds(timer);

        SceneManager.LoadScene(scene);
    }

    public void LoadMenu()
    {
        if (buttonsound != null)
            buttonsound.Play();
        StartCoroutine(LoadAndDim(fadeTime, mainMenuName));
    }

    public void LoadLevel()
    {
        if (buttonsound != null)
            buttonsound.Play();
        if (GameManager.instance != null)
        {
            GameManager.instance.SavePrefs();
        }
        if (myAudio != null)
        {
            DontDestroyOnLoad(myAudio);
            StartCoroutine(FadeAudio(myAudio, fadeTime, Fade.Out, true));
        }
        DontDestroyOnLoad(tehCaller);

        //SceneManager.LoadScene(firstSceneName);

        StartCoroutine(LoadAndDim(fadeTime, firstSceneName));

        //theButton.SetActive (false);
        tehCaller.enabled = false;

    }

    void OnLevelWasLoaded()
    {
        Fader f = gameObject.GetComponent<Fader>();
        if (f != null)
            f.dir = -1;
    }
}
