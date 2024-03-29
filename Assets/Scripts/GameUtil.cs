using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Text;

/* GameUtil
 * 
 * Authors: Mikael Sollenborn
 * 
 * Purpose: Often used or useful static methods
 */

public static class GameUtil : System.Object {

	// Set or modify single value in vector (returns new vector)
	public static Vector3 SetX(Vector3 vector, float value) {
		return new Vector3 (value, vector.y, vector.z);
	}

	public static Vector3 SetY(Vector3 vector, float value) {
		return new Vector3 (vector.x, value, vector.z);
	}

	public static Vector3 SetZ(Vector3 vector, float value) {
		return new Vector3 (vector.x, vector.y, value);
	}

	public static Vector3 AddX(Vector3 vector, float value) {
		return new Vector3 (vector.x + value, vector.y, vector.z);
	}

	public static Vector3 AddY(Vector3 vector, float value) {
		return new Vector3 (vector.x, vector.y + value, vector.z);
	}

	public static Vector3 AddZ(Vector3 vector, float value) {
		return new Vector3 (vector.x, vector.y,  + vector.z + value);
	}
		
	public static Vector3 CloneVector3(Vector3 inv) {
		return new Vector3(inv.x, inv.y, inv.z);
	}

	// Aim object (like a camera) at given position. To animate instead of setting, set doRotation=false and use returned Vector3 with LeanTween/iTween. OR use lower step params progressively
	public static Vector3 AimTowards(GameObject obj, Vector3 pos, bool doRotation = true, float step = 1000) { // 1000 = high value to ensure that entire rotation is done at once, otherwise it starts "moving towards" the target
		Vector3 targetDir = pos - obj.transform.position;
		Vector3 newDir = Vector3.RotateTowards(obj.transform.forward, targetDir, step, 0);
		// Debug.DrawRay(transform.position, newDir, Color.red); Debug.Break ();

		if (doRotation)
			obj.transform.rotation = Quaternion.LookRotation(newDir);
		
		return Quaternion.LookRotation(newDir).eulerAngles;
	}

	// Set color based on 0-255 values instead of 0...1
	public static Color IntColor(float r, float g, float b, float a = 255) {
		return new Color (r/255f, g/255f, b/255f, a/255f);
	}

	// Find parent object with a given tag
	public static GameObject FindParentWithTag(GameObject childObject, string tag)
	{
		Transform t = childObject.transform;
		while (t != null)
		{
			if (t.tag == tag)
			{
				return t.gameObject;
			}
			if (t.parent == null)
				return null;
			t = t.parent.transform;
		}
		return null;
	}
		
	// Find parent object with a given name
	public static GameObject FindParentWithName(GameObject childObject, string name)
	{
		Transform t = childObject.transform;
		while (t != null)
		{
			if (t.name == name)
			{
				return t.gameObject;
			}
			if (t.parent == null)
				return null;
			t = t.parent.transform;
		}
		return null;
	}


	// Find nested child with specified name (breadth-first search). Normal "Find" method is only one level deep
	public static Transform FindDeepChild(Transform aParent, string aName)
	{
		var result = aParent.Find(aName);
		if (result != null)
			return result;
		foreach(Transform child in aParent)
		{
			result = FindDeepChild(child, aName);
			if (result != null)
				return result;
		}
		return null;
	}


	// Set layer on this transform and all its children
	public static void SetDeepLayer(Transform tCurrent, int layer)
	{
		tCurrent.gameObject.layer = layer;

		foreach(Transform child in tCurrent)
		{
			SetDeepLayer(child, layer);
		}
	}


	public enum ShaderMode { OPAQUE=0, CUTOUT=1, FADE=2, TRANSP=3 };

	// Change the shader material blend mode
	public static void SetMaterialBlendMode(Material material, ShaderMode blendMode )
	{
		switch (blendMode)
		{
			case ShaderMode.OPAQUE:
				material.SetFloat("_Mode", 0);
				material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
				material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
				material.SetInt("_ZWrite", 1);
				material.DisableKeyword("_ALPHATEST_ON");
				material.DisableKeyword("_ALPHABLEND_ON");
				material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
				material.renderQueue = -1;
				break;
			case ShaderMode.CUTOUT:
				material.SetFloat("_Mode", 1);
				material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
				material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
				material.SetInt("_ZWrite", 1);
				material.EnableKeyword("_ALPHATEST_ON");
				material.DisableKeyword("_ALPHABLEND_ON");
				material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
				material.renderQueue = 2450;
				break;
			case ShaderMode.FADE:
				material.SetFloat("_Mode", 2);
				material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
				material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
				material.SetInt("_ZWrite", 0);
				material.DisableKeyword("_ALPHATEST_ON");
				material.EnableKeyword("_ALPHABLEND_ON");
				material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
				material.renderQueue = 3000;
				break;
			case ShaderMode.TRANSP:
				material.SetFloat("_Mode", 3);
				material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
				material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
				material.SetInt("_ZWrite", 0);
				material.DisableKeyword("_ALPHATEST_ON");
				material.DisableKeyword("_ALPHABLEND_ON");
				material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
				material.renderQueue = 3000;
				break;
		}
	}

	// If width OR height is less than given value (in inches), then the device is considered small
	private static bool IsSmallDevice(float treshold=4f, bool debug=false) {
		float width = Screen.width / Screen.dpi;
		float height = Screen.height / Screen.dpi;

		if (debug) {
			Debug.Log ("DPI: " + Screen.dpi + "  W: " + Screen.width + "  H: " + Screen.height);
			Debug.Log ("Measured: " + width + " x " + height);
		}

		if (width < treshold || height < treshold)
			return true;
		else
			return false;
	}
	public static void DisableButton(Button b, Image img,float disabledAlphaValue) {
		
		b.interactable = false;

		Color dis = img.color;
		dis.a = disabledAlphaValue;
		img.color = dis;
	}

	// Find the edge TOP-LEFT coordinates of a specified camera's viewport in world coordinates, given a specified z. Assume z pos 0 if not set, can be set to an object's pos
	// Can be used e.g. to place an object at edges of screen. Invert e.g. x in returned vector to place in top-right corner.
	// Can also be used to calculate screen width in world coordinates at a certain depth (default is z=0)
	//
	// Example: GameObject go = GameObject.Find ("Test"); Vector3 pos = GameUtil.GetCameraWorldPosEdges (Camera.main, go.gameObject.transform.position.z); go.transform.position = new Vector3(pos.x, -pos.y, pos.z); // place object in BOTTOM-LEFT visible corner from perspective of main camera
	// Example2: Vector3 pos = GameUtil.GetCameraWorldPosEdges (Camera.main); int screenWidthWorld = pos.x * 2 // calculate screen width in world coordinates at Z=0 from view of main camera
	public static Vector3 GetCameraWorldPosEdges(Camera cam, float z = 0, float xEdge = 0, float yEdge = 0) {
		float dist = -cam.transform.position.z + z;
		return cam.ViewportToWorldPoint (new Vector3 (xEdge, 1-yEdge, dist));
	}
		

	// !! WARNING: does not seem to work on device (iOS), and disabling "Strip engine code" in Player settings did not seem to help reflection to work either!

	// Copy a component (https://answers.unity.com/questions/530178/how-to-get-a-component-from-an-object-and-add-it-t.html)
	public static T CopyComponent<T>(this GameObject go, T toAdd) where T : Component
	{
		return go.AddComponent<T>().GetCopyOf(toAdd) as T;
	}

	public static T GetCopyOf<T>(this Component comp, T other) where T : Component
	{
		Type type = comp.GetType();
		if (type != other.GetType()) return null; // type mis-match
//		BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
		BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default; // removing DeclaredOnly seems to copy more stuff, such as Materials etc. Don't know if it has any unwanted side-effects.
		PropertyInfo[] pinfos = type.GetProperties(flags);
		foreach (var pinfo in pinfos) {
			if (pinfo.CanWrite) {
				try {
					pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
				}
				catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
			}
		}
		FieldInfo[] finfos = type.GetFields(flags);
		foreach (var finfo in finfos) {
			finfo.SetValue(comp, finfo.GetValue(other));
		}
		return comp as T;
	}

	// Check if a tag exists (and catch exception if it does not)
	public static bool DoesTagExist(string tag)
	{
		try
		{
			GameObject.FindGameObjectWithTag(tag);
			return true;
		}
		catch
		{
			return false;
		}
	}

	// Find the inverse Lerp float value. That is, given a start point a and an end point b, and a vector "value" along its path, what is the Lerp float to produce "value"
	public static float InverseLerp(Vector3 a, Vector3 b, Vector3 value)
	{
		Vector3 AB = b - a;
		Vector3 AV = value - a;
		return Vector3.Dot(AV, AB) / Vector3.Dot(AB, AB);
	}

	// Used to print out messages on screen ("local" in the sense that it can be used for clients that are not servers in networked games). To use, have a GameObject in a canvas called "DebugLocal", which contains n Text components as children, stacked on top of each other
	private static Color lightGrey = new Color(0.85f, 0.85f, 0.85f);
	private static Text[] localDebugText = null;
	public static void LocalDebug(string msg, GameObject includeMyName = null)
	{
		if (localDebugText == null)
		{
			GameObject g = GameObject.Find("DebugLocal");
			if (!g || g.transform.childCount < 1) return;

			localDebugText = new Text[g.transform.childCount];
			for (int i = 0; i < localDebugText.Length; i++)
				localDebugText[i] = g.transform.GetChild(i).GetComponent<Text>();
		}

		int len = localDebugText.Length;

		for (int i = 0; i < len; i++)
			localDebugText[i].color = lightGrey;

		for (int i = 0; i < len - 1; i++)
			localDebugText[i].text = localDebugText[i + 1].text;

		localDebugText[len - 1].color = Color.white;
		localDebugText[len - 1].text = (includeMyName == null ? "" : includeMyName.name + ": ") + msg;
	}


	// Push and pop scenes on a stack to simplify going back to previous scenes
	private static List<string> sceneStack = new List<string>();

	public static void PushScene(string scene = null)
	{
		if (scene == null)
			sceneStack.Add(SceneManager.GetActiveScene().name);
		else
			sceneStack.Add(scene);
	}

	public static void PopScene()
	{
		if (sceneStack.Count <= 0)
			return;

		string loadScene = sceneStack[sceneStack.Count - 1];
		sceneStack.RemoveAt(sceneStack.Count - 1);
		// Debug.Log (sceneStack.Count);
		SceneManager.LoadScene(loadScene);
	}

	// PlayerPrefs simulated bool type (which doesn't actually exist)
	public static void SetPlayerPrefsBool(string key, bool state) {
		PlayerPrefs.SetInt(key, state ? 1 : 0);
	}
	public static bool GetPlayerPrefsBool(string key) {
		int value = PlayerPrefs.GetInt(key);
		return (value == 0) ? false : true;
	}


	// Broadcast to all root objects in the scene
	public static void BroadcastAll(string fun, System.Object msg)
	{
		GameObject[] gos = (GameObject[])GameObject.FindObjectsOfType(typeof(GameObject));
		foreach (GameObject go in gos)
		{
			if (go && go.transform.parent == null)
			{
				go.gameObject.BroadcastMessage(fun, msg, SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	// determine if event with P 0...100% happened 
	public static bool P(float prob)
	{
		return UnityEngine.Random.Range(0f, 0.999999999f) < prob;
	}

	// transform mesh into obj 3d string 
	// borrowed from http://wiki.unity3d.com/index.php?title=ObjExporter
	public static string MeshToObj3dString(MeshFilter mf)
	{
		Mesh m = mf.mesh;
		Renderer rend = mf.GetComponent<Renderer>();
		Material[] mats = rend.sharedMaterials;

		StringBuilder sb = new StringBuilder();

		sb.Append("g ").Append(mf.name).Append("\n");
		foreach (Vector3 v in m.vertices)
		{
			sb.Append(string.Format("v {0} {1} {2}\n", v.x, v.y, v.z));
		}
		sb.Append("\n");
		foreach (Vector3 v in m.normals)
		{
			sb.Append(string.Format("vn {0} {1} {2}\n", v.x, v.y, v.z));
		}
		sb.Append("\n");
		foreach (Vector3 v in m.uv)
		{
			sb.Append(string.Format("vt {0} {1}\n", v.x, v.y));
		}
		for (int material = 0; material < m.subMeshCount; material++)
		{
			sb.Append("\n");
			sb.Append("usemtl ").Append(mats[material].name).Append("\n");
			sb.Append("usemap ").Append(mats[material].name).Append("\n");

			int[] triangles = m.GetTriangles(material);
			for (int i = 0; i < triangles.Length; i += 3)
			{
				sb.Append(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n",
					triangles[i] + 1, triangles[i + 1] + 1, triangles[i + 2] + 1));
			}
		}
		return sb.ToString();
	}

	// transform mesh into obj 3d file. Adds ".obj" to filename. Saves to project root of if no path given
	public static void MeshToObj3dFile(MeshFilter mf, string filename)
	{
		if (!filename.EndsWith(".obj", StringComparison.OrdinalIgnoreCase))
			filename = filename + ".obj";

		using (StreamWriter sw = new StreamWriter(filename))
		{
			sw.Write(MeshToObj3dString(mf));
		}
	}

	// create duplicate reverse of each polygon, to e.g create a plane with a backside
	private static void DoubleFaces(MeshFilter mf)
	{
		var mesh = mf.mesh;
		var vertices = mesh.vertices;
		var uv = mesh.uv;
		var normals = mesh.normals;
		var szV = vertices.Length;
		var newVerts = new Vector3[szV * 2];
		var newUv = new Vector2[szV * 2];
		var newNorms = new Vector3[szV * 2];
		for (var j = 0; j < szV; j++)
		{
			newVerts[j] = newVerts[j + szV] = vertices[j];
			newUv[j] = newUv[j + szV] = uv[j];
			newNorms[j] = normals[j];
			newNorms[j + szV] = -normals[j];
		}
		var triangles = mesh.triangles;
		var szT = triangles.Length;
		var newTris = new int[szT * 2];
		for (var i = 0; i < szT; i += 3)
		{
			newTris[i] = triangles[i];
			newTris[i + 1] = triangles[i + 1];
			newTris[i + 2] = triangles[i + 2];
			var j = i + szT;
			newTris[j] = triangles[i] + szV;
			newTris[j + 2] = triangles[i + 1] + szV;
			newTris[j + 1] = triangles[i + 2] + szV;
		}
		mesh.vertices = newVerts;
		mesh.uv = newUv;
		mesh.normals = newNorms;
		mesh.triangles = newTris; // assign triangles last!
	}


	// Set and restore timestep
	private static int pushCounter = 0;
	public static float originalLevelTimeStep = -1;

	public static int PushFixedTimeStep(float timeStep)
	{
		if (originalLevelTimeStep < 0)
			originalLevelTimeStep = Time.fixedDeltaTime;

		Time.fixedDeltaTime = timeStep;
		pushCounter++;
		return pushCounter;
	}

	public static void RestoreTimeStep(int timeStepPushIndex)
	{
		if (timeStepPushIndex == pushCounter)
			Time.fixedDeltaTime = originalLevelTimeStep;
	}

}
