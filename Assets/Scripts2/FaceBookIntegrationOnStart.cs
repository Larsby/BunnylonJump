using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook;
/* FaceBookIntegrationOnStart
 * 
 * Author: Mattias Öhman
 * 
 * Purpose: Simplifies sharing an link to the game on the players facebook. 
 * 			Also logs game start in facebook analytics.
 * 
 * Usage:
 *		
 *
 */
public class FaceBookIntegrationOnStart : MonoBehaviour
{

	// Use this for initialization
	private bool init = false;
	private bool share = false;
	public bool useAnalytics = true;
	 System.Uri link;
	public string title;
	public string description;
	public string linkString;
	public string imageLinkString;
	System.Uri photoLink;
	public enum FaceBookResponse{
		SUCCESS,
		FAILURE, 
		CANCELED, 
		EMPTY
	}
	private System.Action<FaceBookResponse> callbackEvent;
	private System.Uri CreateLink(string s) {
		System.Uri uri= null;
		if (s != null)
		{
			try
			{
				uri = new System.Uri(s);
			}
			catch (System.Exception)
			{
				uri = null;
			}
		}
		return uri;
	}
	public void RegisterFBRespoonse(System.Action<FaceBookResponse> callbackEvent) {
		this.callbackEvent = callbackEvent;
	}
	private void SendResponse(FaceBookResponse response) {
		if(callbackEvent != null) {
			callbackEvent(response);
		}
	}
	void Start()
	{
		init = false;
		share = false;
		callbackEvent = null;
		Facebook.Unity.FB.Init(this.OnInitComplete);
		link = CreateLink(linkString);
		photoLink = CreateLink(imageLinkString);

	}

	private void OnInitComplete()
	{

		if (useAnalytics)
		{
			Facebook.Unity.FB.LogAppEvent(
				"ApplicationStarted",
				null,
				new Dictionary<string, object>()
				{
			{ "Started", Application.productName+ " Game Started" }
				});

			init = true;
		}
	}
	public bool LogAppEvent(string eventName, string key, string value) {
		if(init) {

			Facebook.Unity.FB.LogAppEvent(
				eventName,
				null,
				new Dictionary<string, object>()
				{
				{ key, value }
				});
			return true;
		} else {
			return false;
		}
	}
	protected void HandleResult(Facebook.Unity.IResult result)
	{
		if (result == null)
		{
			SendResponse(FaceBookResponse.EMPTY);
			return;
		}

	

		if (!string.IsNullOrEmpty(result.Error))
		{
			SendResponse(FaceBookResponse.FAILURE);

		}
		else if (result.Cancelled)
		{
			SendResponse(FaceBookResponse.CANCELED);
		}
		else if (!string.IsNullOrEmpty(result.RawResult))
		{
			SendResponse(FaceBookResponse.SUCCESS);

		}
		else
		{
			SendResponse(FaceBookResponse.EMPTY);
		}


	}
	private void DoShare() {

		Facebook.Unity.FB.ShareLink(link, title, description,photoLink,HandleResult);


	}
	public void Share()
	{
		if(init == false) {
			share = true;
			return;
		}
		DoShare();
	}



    // Update is called once per frame
    void Update () {
		if(init && share) {
			share = false;
			DoShare();
		}
    }
}
