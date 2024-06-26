﻿using UnityEngine;
using UnityEngine.UI;


/* Popup
 * 
 * Author: Mikael Sollenborn
 * 
 * Purpose: Show a popup, possibly animated
 * 
 * Dependencies: LeanTween
 * 				 Uncomment I2 lines in LocalizeString to allow I2 localization / dependency
 * 
 * Usage:
 *		Make sure both CoverButton and popupRoot are disabled. Then find popup, call ShowOk or ShowYesNo, and provide a callback method for the user's selection.
 *		The icon can be animated, set sprites in Inspector or code to an array of images
 */

public class Popup : MonoBehaviour
{

	public enum PopupButtonChoice
	{
		Unselected,
		OK,
		YES,
		NO,
		ABORT
	};

	public Button yesButton;
	public Button noButton;
	public Button okButton;
	public Button abortButton;
	public Text breadText;
	public Text header;
	public bool singleButton = false;
	public bool useHeader = false;

	public Image iconImage;
	//public SVGImage iconImage;

	private System.Action<PopupButtonChoice> callback;
	private PopupButtonChoice buttonChoice = PopupButtonChoice.Unselected;

	public RectTransform parentScaler = null;

	public float inFromSize = 1f;
	public LeanTweenType inAnimEaseType = LeanTweenType.easeOutBounce;
	public float inAnimTime = 0.5f;
	public bool fadeInAnim = false;

	public float outToSize = 1f;
	public LeanTweenType outAnimEaseType = LeanTweenType.linear;
	public float outAnimTime = 0.3f;
	public bool fadeOutAnim = false;

	private bool isHiding = false;

	private Vector3 orgScale;
	private CanvasGroup canvasGroup = null;

	public bool localizeStrings = true;
	int spriteCnt = 0;
	public Sprite[] sprites;
	public float iconAnimDelay = 0.5f;
	public int iconRefSize = -1;

	void Start()
	{
		yesButton.onClick.AddListener(YesButton);
		noButton.onClick.AddListener(NoButton);
		okButton.onClick.AddListener(OkButton);
		if (abortButton != null)
			abortButton.onClick.AddListener(AbortButton);

		HideUnhidePopupComponents();

		if (parentScaler)
			orgScale = parentScaler.localScale;

		canvasGroup = GetComponent<CanvasGroup>();
	}

	private void HideUnhidePopupComponents()
	{
		yesButton.gameObject.SetActive(!singleButton);
		noButton.gameObject.SetActive(!singleButton);
		okButton.gameObject.SetActive(singleButton);
		header.gameObject.SetActive(useHeader);
	}

	void Update()
	{
	}

	private void UpdateParentScaler(float val)
	{
		parentScaler.localScale = orgScale * val;
	}
	private void UpdateCanvasGroupOpacity(float val)
	{
		canvasGroup.alpha = val;
	}

	private void DelayedHidePopup()
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			transform.GetChild(i).gameObject.SetActive(false);
		}
		isHiding = false;
	}

	private void ShowHidePopup(bool show)
	{
		bool immediateEffect = true;

		if (show && inAnimTime > 0 && (inFromSize < 1f || inFromSize > 1f || fadeInAnim))
		{
			if ((inFromSize < 1f || inFromSize > 1f) && parentScaler)
			{
				parentScaler.localScale = orgScale;
				LeanTween.value(gameObject, UpdateParentScaler, inFromSize, 1f, inAnimTime).setEase(inAnimEaseType);
			}
			if (fadeInAnim && canvasGroup)
			{
				canvasGroup.alpha = 0f;
				LeanTween.value(gameObject, UpdateCanvasGroupOpacity, 0f, 1.0f, inAnimTime / 2.5f);
			}
		}

		if (!show && outAnimTime > 0 && (outToSize < 1f || outToSize > 1f || fadeOutAnim))
		{
			if ((outToSize < 1f || outToSize > 1f) && parentScaler)
			{
				parentScaler.localScale = orgScale;
				LeanTween.value(gameObject, UpdateParentScaler, 1f, outToSize, outAnimTime).setEase(outAnimEaseType);
				immediateEffect = false;
			}
			if (fadeOutAnim && canvasGroup)
			{
				canvasGroup.alpha = 1f;
				LeanTween.value(gameObject, UpdateCanvasGroupOpacity, 1f, 0f, outAnimTime / 2f);
				immediateEffect = false;
			}
		}

		if (immediateEffect)
		{
			for (int i = 0; i < transform.childCount; i++)
				transform.GetChild(i).gameObject.SetActive(show);
		}
		else
		{
			Invoke("DelayedHidePopup", outAnimTime);
			isHiding = true;
		}
	}

	private void Close()
	{
		CancelInvoke();
		ShowHidePopup(false);
		if (callback != null)
			callback(buttonChoice);
	}

	public PopupButtonChoice GetButtonChoice()
	{
		return buttonChoice;
	}

	public void YesButton()
	{
		if (isHiding) return;
		buttonChoice = PopupButtonChoice.YES;
		Close();
	}

	public void NoButton()
	{
		if (isHiding) return;
		buttonChoice = PopupButtonChoice.NO;
		Close();
	}

	public void OkButton()
	{
		if (isHiding) return;
		buttonChoice = PopupButtonChoice.OK;
		Close();
	}

	public void AbortButton()
	{
		if (isHiding) return;
		buttonChoice = PopupButtonChoice.ABORT;
		Close();
	}

	public void Show(System.Action<PopupButtonChoice> callback)
	{
		buttonChoice = PopupButtonChoice.Unselected;
		ShowHidePopup(true);
		this.callback = callback;
		HideUnhidePopupComponents();
	}

	private string LocalizeString(string str)
	{
		//if (localizeStrings)
		//	return I2.Loc.LocalizationManager.GetTranslation(str);

		return str;
	}

	public void ShowYesNo(System.Action<PopupButtonChoice> callback, bool bHeader = true, string bText = null, string hText = null, string YEStext = null, string NOtext = null, bool hideIcon = false, Sprite[] newSprites = null)
	{
		buttonChoice = PopupButtonChoice.Unselected;
		this.callback = callback;
		useHeader = bHeader;
		singleButton = false;
		ShowHidePopup(true);
		HideUnhidePopupComponents();

		if (newSprites != null)
			sprites = newSprites;

		if (iconImage && (hideIcon || sprites == null))
		{
			iconImage.enabled = false;
		}
		if (iconImage != null && sprites != null && iconImage.enabled)
		{
			spriteCnt = 0;
			if (sprites.Length > 0)
				iconImage.sprite = sprites[spriteCnt];
			if (sprites.Length > 1)
				Invoke("ChangeSprite", iconAnimDelay);

			UglyChangeImageSize();
		}

		if (bText != null)
			breadText.text = LocalizeString(bText);
		if (hText != null)
			header.text = LocalizeString(hText);

		if (YEStext != null)
			yesButton.GetComponentInChildren<Text>().text = LocalizeString(YEStext);
		if (NOtext != null)
			noButton.GetComponentInChildren<Text>().text = LocalizeString(NOtext);
	}
	public void SetText(string text)
	{
		
			breadText.text = text;
	}

	public void ShowOk(System.Action<PopupButtonChoice> callback, bool bHeader = true, string bText = null, string hText = null, string OKtext = null, bool hideIcon = false, Sprite[] newSprites = null)
	{
		buttonChoice = PopupButtonChoice.Unselected;
		this.callback = callback;
		useHeader = bHeader;
		singleButton = true;
		ShowHidePopup(true);
		HideUnhidePopupComponents();

		if (newSprites != null)
			sprites = newSprites;

		if(iconImage && (hideIcon || sprites == null)) {
			iconImage.enabled = false;
		}
		if (iconImage != null && sprites != null && iconImage.enabled)
		{
			spriteCnt = 0;
			if (sprites.Length > 0)
				iconImage.sprite = sprites[spriteCnt];
			if (sprites.Length > 1)
				Invoke("ChangeSprite", iconAnimDelay);

			UglyChangeImageSize();
		}
		if (bText != null)
			breadText.text = LocalizeString(bText);
		if (hText != null)
			header.text = LocalizeString(hText);
		if (OKtext != null)
			okButton.GetComponentInChildren<Text>().text = LocalizeString(OKtext);
	}

	void ChangeSprite()
	{
		spriteCnt++;
		if (spriteCnt >= sprites.Length)
			spriteCnt = 0;
		iconImage.sprite = sprites[spriteCnt];
		Invoke("ChangeSprite", iconAnimDelay);
	}

	void UglyChangeImageSize()
	{
		if (iconRefSize <= 0)
			return;

		RectTransform rt = iconImage.GetComponent<RectTransform>();

		if (iconImage.sprite.bounds.extents.x >= iconImage.sprite.bounds.extents.y)
			rt.sizeDelta = new Vector2(iconRefSize, iconRefSize * iconImage.sprite.bounds.extents.y / iconImage.sprite.bounds.extents.x);
		else
			rt.sizeDelta = new Vector2(iconRefSize * iconImage.sprite.bounds.extents.x / iconImage.sprite.bounds.extents.y, iconRefSize);
	}

}