using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverLay : MonoBehaviour
{
	public HSController hs;
	public Text scoreText;

	void Start()
    {
		TouchKit.removeAllGestureRecognizers();

		if (PastilleConstants.isDebugging)
        {
			PlayButton();
        }

		InitAds();

		SoundManager sm = SoundManager.GetInstance();

		GameObject musicPlayer = sm.GetMusicPlayer();

		if (musicPlayer.GetComponent<AudioHighPassFilter>() == null)
		{
			AudioHighPassFilter a = musicPlayer.AddComponent<AudioHighPassFilter>();
			a.cutoffFrequency = 10;
			a.highpassResonanceQ = 1;
		}

		sm.PlayMusic(MusicTune.Regular, 1.0f);


		hs.startGetScores();

		hs.LoadScores();
		int highScore = hs.GetHighScore();

		scoreText.text = "High: " + highScore;
	}

	public void PlayButton() {
		SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
	}

	public void InitAds()
	{
#if !UNITY_EDITOR
        AppLovin.SetSdkKey(PastilleConstants.applovinsdkkey);
        AppLovin.InitializeSdk();
		AppLovin.LoadRewardedInterstitial();
	//	AppLovin.SetTestAdsEnabled ("true");

		AppLovin.ShowAd(AppLovin.AD_POSITION_CENTER, AppLovin.AD_POSITION_TOP);
#endif
	}

}
