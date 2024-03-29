using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableTVOS : MonoBehaviour {

	// Use this for initialization
	void Start () {
		#if UNITY_TVOS
		UnityEngine.Apple.TV.Remote.touchesEnabled = true;
		//	UnityEngine.Apple.TV.Remote.reportAbsoluteDpadValues = true;
		UnityEngine.Apple.TV.Remote.allowExitToHome = false;
		#endif
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
