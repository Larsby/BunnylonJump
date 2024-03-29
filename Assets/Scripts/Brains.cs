using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

public class Brains : MonoBehaviour
{
    // public GameObject gameOverLay;

    public HSController hs;
    public Text scoreText;

    public GameObject player;
    public Animator playerAnimator;


    int score = 0;
    int highScore = 0;

    public GameObject BlueStone;
    public GameObject YellowStone;
    public GameObject GreenStone;
    public GameObject RedGreenStone;
    public GameObject YellowBlueStone;

    public GameObject controls;
    List<GameObject> stones = new List<GameObject>();

    int currentStone = 0;

    public Color playerColor;
    public List<GameObject> targets = new List<GameObject>();
    float dissapeartime = 1.5f;
    bool isGameOver = true;

    public Popup popup;
    const string continueKey = "ContinueScore";

    private SoundManager sm;
    private bool wasContinued = false;

    private float nextJump = 0.0f;
    void Awake()
    {
#if !UNITY_EDITOR
		AppLovin.SetUnityAdListener("Brains"); // name of myself
#endif
    }


    public bool LogAppEvent(string eventName, string key, string value)
    {

        print("logevent: " + eventName + " " + key + " " + value);
        Facebook.Unity.FB.LogAppEvent(
            eventName,
            null,
            new Dictionary<string, object>()
            {
                { key, value }
            });
        return true;

    }

    void Start()
    {
        sm = SoundManager.GetInstance();
        AudioHighPassFilter filter = FindObjectOfType<AudioHighPassFilter>();
        if (filter)
            filter.cutoffFrequency = 10;


        TouchKit.removeAllGestureRecognizers();
        var recognizer = new TKSwipeRecognizer();
        recognizer.gestureRecognizedEvent += (r) =>
        {

            if (isGameOver)
                return;

            if (r.completedSwipeDirection.CompareTo(TKSwipeDirection.Left) == 0)
            {
                MoveToNext(0);
            }
            if (r.completedSwipeDirection.CompareTo(TKSwipeDirection.Right) == 0)
            {
                MoveToNext(1);
            }

        };
        TouchKit.addGestureRecognizer(recognizer);


        // gameOverLay.SetActive(true);

        hs.startGetScores();

        hs.LoadScores();
        highScore = hs.GetHighScore();

        scoreText.text = score + " / " + highScore;
        StartNewGame();
    }

    void loadMenu()
    {
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }

    void Update()
    {
        if (isGameOver)
            return;
        if (Input.GetKeyDown(KeyCode.Z))
        {
            MoveToNext(0);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            MoveToNext(1);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (stones[currentStone + 1].transform.name.Contains("Blue"))
                MoveToNext(0);
            else if (stones[currentStone + 1].transform.name.Contains("Red"))
            {
                print("wait");
            }
            else
                MoveToNext(1);
        }
        /*
		if (Input.GetKeyDown(KeyCode.M))
			PauseMusic();
		if (Input.GetKeyDown(KeyCode.N))
			ResumeMusic();
		*/

    }

    private bool isInAMovingState()
    {

        if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Jump_straight"))
            return true;
        if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Jump_left"))
            return true;
        if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Jump_right"))
            return true;
        if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("EndFrameLeft"))
            return true;
        if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("EndFrameRight"))
            return true;

        return false;
    }

    void jumpPlayer()
    {

        /*
        print(isInAMovingState());

        print(playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime);
        float tjofgff = 1.0f - playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        if (isInAMovingState())
        {
            if (System.Math.Abs(stones[currentStone].transform.position.x - stones[currentStone - 1].transform.position.x) < Mathf.Epsilon)
            {

                playerAnimator.CrossFade("Jump_straight", tjofgff / 2.0f);
            }
            else if (stones[currentStone].transform.position.x <= stones[currentStone - 1].transform.position.x)
            {

                playerAnimator.CrossFade("Jump_left", tjofgff / 2.0f);
            }
            else if (stones[currentStone].transform.position.x >= stones[currentStone - 1].transform.position.x)
            {

                playerAnimator.CrossFade("Jump_right", tjofgff / 2.0f);
            }
        }
        else*/
        {
            if (System.Math.Abs(stones[currentStone].transform.position.x - stones[currentStone - 1].transform.position.x) < Mathf.Epsilon)
            {
                playerAnimator.Play("");

                if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Jump_straight"))
                {
                    playerAnimator.Play("Jump_straight_alt");

                }
                else
                {
                    playerAnimator.Play("Jump_straight");
                }
            }
            else if (stones[currentStone].transform.position.x <= stones[currentStone - 1].transform.position.x)
            {
                if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Jump_left"))
                {
                    playerAnimator.Play("Jump_left_alt");

                }
                else
                {
                    playerAnimator.Play("Jump_left");
                }
            }
            else if (stones[currentStone].transform.position.x >= stones[currentStone - 1].transform.position.x)
            {


                if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Jump_right"))
                {
                    playerAnimator.Play("Jump_right_alt");

                }
                else
                {
                    playerAnimator.Play("Jump_right");
                }
            }
        }



        try
        {
            iTween.Stop();
        }
        catch (System.Exception e)
        {

        }

        iTween.MoveTo(player, iTween.Hash("position", stones[currentStone].transform.position, "time", 0.333, "easetype", iTween.EaseType.easeInOutCubic));


    }



    public void MoveToNext(int color)
    {
        Camera.main.GetComponent<CameraFollower>().rolling = true;

        //implemented a cooldown period between jumps for 0.15 seconds;
        if (Time.time < nextJump)
        {
            return;
        }
        nextJump = Time.time + 0.15f;

        //  while (iTween.Count(player) > 0)
        //   { return; }
        sm.PlayRandomFromType(SfxRandomType.Jump);
        bool wrong = true;
        Camera.main.GetComponent<CameraFollower>().changeDirection();
        if (stones[currentStone + 1].transform.name.Contains("Green"))
        {
            score++;
            currentStone++;

            jumpPlayer();
            MakeStone();
            RemoveLastStone();
            wrong = false;
        }

        else
        {
            if (color == 0)
            {

                if (stones[currentStone + 1].transform.name.Contains("Blue"))
                {
                    score++;
                    currentStone++;
                    jumpPlayer();
                    MakeStone();
                    RemoveLastStone();
                    wrong = false;
                }
            }

            if (color == 1)
            {
                if (stones[currentStone + 1].transform.name.Contains("Yellow"))
                {
                    score++;
                    currentStone++;
                    jumpPlayer();
                    MakeStone();
                    RemoveLastStone();
                    wrong = false;

                }
            }
        }
        if (wrong)
        {

            gameOver();



        }

        hs.setCurrentScore(score);
        scoreText.text = score + " / " + highScore;
        stones[currentStone].GetComponent<Dissapear>().startDissapear(dissapeartime);

    }

    public void RemoveLastStone()
    {

        //  iTween.ScaleTo(stones[currentStone - 1], new Vector3(0.1f, 0.1f, 0.1f), 0.5f);
        //  stones.Remove(stones[0]);

    }

    public void Created(float damage)
    {
    }

    public void dissapear(int place)
    {

        if (place == currentStone)
        {
            gameOver();

        }
    }


    public void MakeStone()
    {
        Vector3 newPos = stones[stones.Count - 1].transform.position;
        Quaternion qId = Quaternion.identity;
        newPos.y += 2f;

        if (currentStone < 20)
        {
            if (Random.Range(0f, 1f) < 0.5)
            {
                newPos.x -= 2.0f;
                stones.Add(Instantiate(BlueStone, newPos, qId));

            }
            else
            {
                newPos.x += 2.0f;
                stones.Add(Instantiate(YellowStone, newPos, qId));
            }
        }
        else if (currentStone < 40)
        {
            if (Random.Range(0f, 1f) < 0.25)
            {
                newPos.x -= Random.Range(-1, 1) * 2;
                stones.Add(Instantiate(BlueStone, newPos, qId));

            }
            else
            {
                newPos.x -= Random.Range(-1, 1) * 2;
                stones.Add(Instantiate(YellowStone, newPos, qId));
            }

        }
        else if (currentStone < 60)
        {
            if (Random.Range(0f, 1f) < 0.25)
            {

                newPos.x -= Random.Range(-1, 1) * 2;
                stones.Add(Instantiate(GreenStone, newPos, qId));


            }
            else
            {
                if (Random.Range(0f, 1f) < 0.5)
                {
                    newPos.x -= Random.Range(-1, 1) * 2;
                    stones.Add(Instantiate(BlueStone, newPos, qId));

                }
                else
                {
                    newPos.x -= Random.Range(-1, 1) * 2;
                    stones.Add(Instantiate(YellowStone, newPos, qId));
                }
            }
        }
        else if (currentStone < 100)
        {
            if (Random.Range(0f, 1f) < 0.25)
            {

                newPos.x -= Random.Range(-1, 1) * 2;
                stones.Add(Instantiate(GreenStone, newPos, qId));


            }
            else if (Random.Range(0f, 1f) < 0.25)
            {
                newPos.x -= Random.Range(-1, 1) * 2;
                GameObject g = Instantiate(YellowBlueStone, newPos, qId);
                g.GetComponentInChildren<ChangingStone>().nofFlips = 1 + ((currentStone - 60) / 20 + 1) * 1 + Random.Range(0, 2);
                stones.Add(g);
            }
            else
            {
                if (Random.Range(0f, 1f) < 0.5)
                {
                    newPos.x -= Random.Range(-1, 1) * 2;
                    stones.Add(Instantiate(BlueStone, newPos, qId));

                }
                else
                {
                    newPos.x -= Random.Range(-1, 1) * 2;
                    stones.Add(Instantiate(YellowStone, newPos, qId));
                }
            }
        }
        else if (currentStone < 120)
        {
            if (Random.Range(0f, 1f) < 0.25)
            {

                newPos.x -= Random.Range(-1, 1) * 2;
                stones.Add(Instantiate(YellowBlueStone, newPos, qId));
            }
            else if (Random.Range(0f, 1f) < 0.25)
            {

                newPos.x -= Random.Range(-1, 1) * 2;
                stones.Add(Instantiate(GreenStone, newPos, qId));
            }
            else if (Random.Range(0f, 1f) < 0.25)
            {

                newPos.x -= Random.Range(-1, 1) * 2;
                stones.Add(Instantiate(RedGreenStone, newPos, qId));
            }
            else
            {
                if (Random.Range(0f, 1f) < 0.5)
                {
                    newPos.x -= Random.Range(-1, 1) * 2;
                    stones.Add(Instantiate(BlueStone, newPos, qId));

                }
                else
                {
                    newPos.x -= Random.Range(-1, 1) * 2;
                    stones.Add(Instantiate(YellowStone, newPos, qId));
                }
            }


        }
        else
        {

            int ramdomData = Random.Range(0, 5);

            switch (ramdomData)
            {
                case 0:
                    {
                        newPos.x -= Random.Range(-1, 1) * 2;
                        stones.Add(Instantiate(GreenStone, newPos, qId));
                        break;
                    }
                case 1:
                    {
                        newPos.x -= Random.Range(-1, 1) * 2;
                        stones.Add(Instantiate(BlueStone, newPos, qId));
                        break;
                    }
                case 2:
                    {
                        newPos.x -= Random.Range(-1, 1) * 2;
                        stones.Add(Instantiate(YellowStone, newPos, qId));
                        break;
                    }
                case 3:
                    {
                        newPos.x -= Random.Range(-1, 1) * 2;
                        stones.Add(Instantiate(YellowBlueStone, newPos, qId));
                        break;
                    }
                case 4:
                    {
                        newPos.x -= Random.Range(-1, 1) * 2;
                        stones.Add(Instantiate(RedGreenStone, newPos, qId));
                        break;
                    }

                default:
                    {
                        newPos.x -= Random.Range(-1, 1) * 2;
                        stones.Add(Instantiate(BlueStone, newPos, qId));
                        break;
                    }

            }

        }

        stones[stones.Count - 1].transform.localScale = new Vector3(0, 0, 0);
        iTween.ScaleTo(stones[stones.Count - 1], new Vector3(1.0f, 1.0f, 1.0f), 0.5f);

        stones[stones.Count - 1].GetComponent<Dissapear>().setMyPlace(stones.Count - 1);
    }

    public void removeAllStonesFromScene()
    {

        for (int i = 0; i < stones.Count; i++)
        {
            Destroy(stones[i]);
        }
    }

    public void StartNewGame()
    {
        isGameOver = false;

        player.GetComponent<SpriteRenderer>().color = playerColor;
        controls.active = true;
        currentStone = 0;
        score = 0;

        removeAllStonesFromScene();
        stones.Clear();

        if (PlayerPrefs.HasKey(continueKey))
        {
            score = currentStone = PlayerPrefs.GetInt(continueKey);
            hs.setCurrentScore(score);
            PlayerPrefs.DeleteKey(continueKey);
            PlayerPrefs.Save();
            wasContinued = true;
            GameObject g = Instantiate(BlueStone, new Vector3(0, 1000, 0), Quaternion.identity);
            for (int i = 0; i < currentStone; i++)
                stones.Add(g); // dummies to not break old code
        }


        Camera.main.GetComponent<CameraFollower>().changeTheDriftVariable(0.005f);
        stones.Add(Instantiate(YellowStone));
        stones[stones.Count - 1].GetComponent<Dissapear>().setMyPlace(stones.Count - 1);

        for (int i = 0; i < 3; i++)
            MakeStone();


        highScore = hs.GetHighScore();
        scoreText.text = score + " / " + highScore;
    }


    public void gameOver()
    {
        if (isGameOver)
            return;
        else
            isGameOver = true;


        LogAppEvent("gameover", "gameover", "" + score);

        playerAnimator.Play("Fail");
        sm.PlaySingleSfx(SingleSfx.Fail);
        AudioHighPassFilter filter = FindObjectOfType<AudioHighPassFilter>();
        if (filter)
            filter.cutoffFrequency = 2000;

        int oldHi = hs.GetHighScoreNoSaveCurrent();

        hs.SaveScores();
        highScore = hs.GetHighScore();
        scoreText.text = score + " / " + highScore;

        //   iTween.MoveTo(player, stones[currentStone + 1].transform.position, 0.5f);

        player.GetComponent<SpriteRenderer>().color = Color.black;

        controls.active = false;

        hs.setScoreAndUpload(score);
        hs.startGetScores();
        Camera.main.GetComponent<CameraFollower>().changeTheDriftVariable(0);

        if (!wasContinued && score > 4 && score >= oldHi - (int)((float)oldHi * 0.3f))
            //		
            Invoke("ShowPopup", 1.0f);
        else
            StartCoroutine(GoGoGameover(2.0f));
    }

    void ShowPopup()
    {
        popup.ShowYesNo(HandleAction, false, "Watch an ad to continue?", null, "YES", "NO", true);
    }

    void HandleAction(Popup.PopupButtonChoice ch)
    {
        if (ch != Popup.PopupButtonChoice.YES)
            StartCoroutine(GoGoGameover(0.5f));
        else
        {
            LogAppEvent("showextraAd", "showextraAd", "" + score);

#if UNITY_EDITOR
            PlayerPrefs.SetInt(continueKey, score);
            PlayerPrefs.Save();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
#else
			AppLovin.ShowRewardedInterstitial();
			PauseMusic();
#endif
        }
    }

    private IEnumerator GoGoGameover(float waitTime)
    {

        yield return new WaitForSeconds(waitTime);
        Camera.main.GetComponent<CameraFollower>().resetDrift();
        iTween.MoveTo(player, new Vector3(0, 0, -10), 0.0f);
        // iTween.MoveTo(gameOverLay, new Vector3(0, 0, 0), 0.5f);
        //  iTween.ScaleTo(gameOverLay, new Vector3(1, 1, 0), 0.5f);


        highScore = hs.GetHighScore();
        scoreText.text = score + " / " + highScore;

        iTween.MoveTo(gameObject, iTween.Hash("position", new Vector3(0, 2.15f, 0), "easetype", iTween.EaseType.linear, "time", 0.1f));
        hs.SaveScores();
        var filter = FindObjectOfType<AudioHighPassFilter>();
        if (filter)
            filter.cutoffFrequency = 10;

        loadMenu();
    }


    void onAppLovinEventReceived(string ev)
    {
        if (ev.Contains("REWARDAPPROVEDINFO"))
        {
        }
        else if (ev.Contains("LOADEDREWARDED"))
        {
            // A rewarded video was successfully loaded
        }
        else if (ev.Contains("LOADREWARDEDFAILED"))
        {
            ResumeMusic();
            StartCoroutine(GoGoGameover(1.0f));
        }
        else if (ev.Contains("HIDDENREWARDED"))
        {
            // A rewarded video was closed.  Preload the next rewarded video.
            AppLovin.LoadRewardedInterstitial();
            ResumeMusic();

            PlayerPrefs.SetInt(continueKey, score);
            PlayerPrefs.Save();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    void PauseMusic()
    {
        FindObjectOfType<AudioHighPassFilter>().GetComponent<AudioSource>().Pause();
    }
    void ResumeMusic()
    {
        FindObjectOfType<AudioHighPassFilter>().GetComponent<AudioSource>().UnPause();
    }


}
