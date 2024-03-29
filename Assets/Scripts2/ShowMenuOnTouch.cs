using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ShowMenuOnTouch : MonoBehaviour
{

	private bool toggle;
	public GameObject thePnl;
	public Button button;
	public Sprite show;
	public Sprite hide;
	void Start ()
	{
		toggle = true;
	}

	public void ToggleMenu ()
	{
		
		thePnl.active = toggle;
			if (toggle) {
			GameManager.instance.SetMenuState ();
		}
		if (toggle) {
			button.image.sprite = hide;
		} else { 
			button.image.sprite = show;
		}
		toggle = !toggle;

	}

	void OnMouseDown ()
	{
		ToggleMenu ();
	}
	#if UNITY_TVOS
	void Update()
	{

		if (Input.GetKeyDown (KeyCode.JoystickButton0)) { //click on "Menu"
			ToggleMenu();
		}


	}
	#endif

}
