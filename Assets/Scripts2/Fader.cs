using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fader : MonoBehaviour
{
	public Texture2D texture;

	public float speed;
	public float dir;
	public bool fade = false;
	public float x = 0;
	public float y = 0;
	public int height = -1;
	public int width = -1;
	private float alpha = 0.0f;

	void OnGUI ()
	{
		if (fade == false) {
			alpha = 0.0f;
		}
		if (fade) {
			if (height == -1) {
				height = Screen.height;
			}
			if (width == -1) {
				width = Screen.width;
			}
			alpha += dir * speed * Time.deltaTime;
			alpha = Mathf.Clamp01 (alpha);
			GUI.color = new Color (GUI.color.r, GUI.color.g, GUI.color.b, alpha);
			GUI.depth = -10000;
			GUI.DrawTexture (new Rect (0, 0, width, height), texture);
			if (dir == -1 && alpha <= 0.0f) {
				fade = false;
			}

		}
	}

	

	
		
}
