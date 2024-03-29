using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class HSController : MonoBehaviour
{
    private static HSController instance6;


    public static HSController Instance
    {
        get { return instance6; }
    }


    public static string uniqueID = PastilleConstants.uniqueID;
    readonly string secretKey = PastilleConstants.secretKey;
    readonly string addScoreURL = PastilleConstants.addScoreURL;
    readonly string highscoreURL = PastilleConstants.highscoreURL;
    readonly string getPositionURL = PastilleConstants.getPositionURL;
    public string[] onlineHighscore;

    public float positionInTheWorld = -1;


    private string name3;
    int score;
    private int AllTimeHighScore = 0;


    int privScore = 0;

    void Awake()
    {


        if (instance6 == null)
            instance6 = this;

        else if (instance6 != this)
        {
            Destroy(gameObject);
            return;
        }


        if (SystemInfo.deviceUniqueIdentifier != SystemInfo.unsupportedIdentifier)
            name3 = "a_" + SystemInfo.deviceUniqueIdentifier;
        else if (SystemInfo.deviceName != SystemInfo.unsupportedIdentifier)
            name3 = "b_" + SystemInfo.deviceUniqueIdentifier;
        else
            name3 = "c_" + System.Guid.NewGuid().ToString();


        score = 0;
    }



    void Start()
    {


        LoadScores();
        startPostScores();

    }

    public void setCurrentScore(int inScore)
    {
        privScore = inScore;

    }

    public void LoadScores()
    {
        if (PlayerPrefs.HasKey("HighScore"))
        {
            AllTimeHighScore = PlayerPrefs.GetInt("HighScore");
        }
        else
        {
            PlayerPrefs.SetInt("HighScore", 0);
        }


        PlayerPrefs.Save();

    }
    public int GetHighScore()
    {
        if (privScore > AllTimeHighScore) { AllTimeHighScore = privScore; SaveScores(); }
        return AllTimeHighScore;
    }

	public int GetHighScoreNoSaveCurrent()
	{
		return AllTimeHighScore;
	}

	void DeletePrefs()
    {
        PlayerPrefs.DeleteAll();
    }
    public void SaveScores()
    {

        GetHighScore();
        PlayerPrefs.SetInt("HighScore", AllTimeHighScore);
        PlayerPrefs.Save();


    }

    public int GetScore()
    {
        return privScore;
    }

    public void startGetScores()
    {
        StartCoroutine(GetScores());
    }

    public void startGetPosition()
    {
        StartCoroutine(GetPosition());
    }

    public void startPostScores()
    {
        StartCoroutine(PostScores());
    }



    string Md5Sum(string strToEncrypt)
    {
        System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
        byte[] bytes = ue.GetBytes(strToEncrypt);

        // encrypt bytes
        System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] hashBytes = md5.ComputeHash(bytes);

        // Convert the encrypted bytes back to a string (base 16)
        string hashString = "";

        for (int i = 0; i < hashBytes.Length; i++)
        {
            hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
        }

        return hashString.PadLeft(32, '0');
    }

    // remember to use StartCoroutine when calling this function!
    IEnumerator PostScores()
    {
        int scoreToPost = GetHighScore();
        //This connects to a server side php script that will add the name and score to a MySQL DB.
        // Supply it with a string representing the players name and the players score.
        string hash = Md5Sum(name3 + scoreToPost + secretKey);

        string post_url = addScoreURL + "uniqueID=" + uniqueID + "&name=" + WWW.EscapeURL(name3) + "&score=" + scoreToPost + "&hash=" + hash;
           // Post the URL to the site and create a download object to get the result.
        WWW hs_post = new WWW("https://" + post_url);
        yield return hs_post; // Wait until the download is done

        if (hs_post.error != null)
        {
            print("There was an error posting the high score: " + hs_post.error);
        }

        //now lets get our position in the world
        startGetPosition(); // You get to know that you are better then hmm hmm percent here!


    }

    public void setScoreAndUpload(int scoreIn)
    {
        privScore = scoreIn;

        GetHighScore();
        StartCoroutine(PostScores());
    }

    // Get the scores from the MySQL DB to display in a GUIText.
    // remember to use StartCoroutine when calling this function!
    IEnumerator GetScores()
    {

        //	Scrolllist.Instance.loading = true;

        WWW hs_get = new WWW("https://" + highscoreURL);

        yield return hs_get;

        if (hs_get.error != null)
        {
            Debug.Log("There was an error getting the high score: " + hs_get.error);

        }
        else
        {
            //Change .text into string to use Substring and Split
            string help = hs_get.text;
            onlineHighscore = help.Split(";"[0]);
        }


    }
    IEnumerator GetPosition()
    {

        WWW hs_get = new WWW("https://" + getPositionURL + WWW.EscapeURL(name3));


        yield return hs_get;

        if (hs_get.error != null)
        {
            Debug.Log("There was an error getting the high score: " + hs_get.error);
        }
        else
        {

            string text = hs_get.text;
            positionInTheWorld = float.Parse(text);
            GameObject go = GameObject.Find("HighScoreSlider");

            if (go != null)
            {
                go.GetComponent<Slider>().value = positionInTheWorld;
            }

        }


    }



}
