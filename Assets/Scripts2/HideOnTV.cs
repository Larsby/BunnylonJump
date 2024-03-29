using UnityEngine;
using System.Collections;

public class HideOnTV : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
		/*
 
			Fire TV Stick (Gen 2)	Fire TV (Gen 2)	Fire TV Stick (Gen 1)	Fire TV (Gen 1)
			AFTT					AFTS			AFTM					AFTB
		*/

		//Debug.Log ("------------------------------- SystemInfo.deviceModel;" + SystemInfo.deviceModel);
	 
		if (SystemInfo.deviceModel.Contains ("Amazon AFT")) {
			gameObject.SetActive (false);
		}
		#if  UNITY_TVOS
		gameObject.SetActive (false);
		#endif
	 

	}
	
	
}
