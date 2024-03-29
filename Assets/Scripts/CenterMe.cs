using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterMe : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = Camera.main.ScreenToWorldPoint(new Vector3 (Screen.width / 2, (Screen.height / 2), Camera.main.nearClipPlane ));

    }
}
