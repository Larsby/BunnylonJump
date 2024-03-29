using UnityEngine;
using System.Collections;
//using Facebook.Unity;
#if UNITY_IOS
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.GameCenter;
#endif
using System.IO;
using System;

public class Utilities : MonoBehaviour
{
    public string game;
    public string HighScore;

    public void Share()
    {


        /*Facebook.Unity.FB.FeedShare ("",
			new Uri ("http://www.pastille.se/"),
			"Get "+game+
			",Join me and get "+game+
			",Can you beat my high score of " + HighScore + "?",
			null, null);*/

    }

    public void More()
    {
        Application.OpenURL("http://www.pastille.se");
    }

    public void Rate()
    {
        /*
		UniRate r = GameObject.FindObjectOfType<UniRate> ();
		r.ShowPrompt ();
		*/
    }


}