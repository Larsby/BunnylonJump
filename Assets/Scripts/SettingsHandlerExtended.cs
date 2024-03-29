using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Example of how to subclass SettingsHandler, using 3 different types of buttons (not to be actually used other than for inspiration)

public class SettingsHandlerExtended : SettingsHandler
{
	public Button restartButton;

	public Button anotherTogglerButton;
	private bool isToggled = false;
	private const string isToggledString = "IsToggled";

	public Button integerSettingButton;
	private int intSet = 0;
	private const string intSetString = "IntSet";

	void Start () {
		base.Start();

		if (restartButton)
			restartButton.onClick.AddListener(Restart);

		if (anotherTogglerButton)
			anotherTogglerButton.onClick.AddListener(MyToggler);
		if (PlayerPrefs.HasKey(isToggledString))
			isToggled = GameUtil.GetPlayerPrefsBool(isToggledString);

		if (integerSettingButton)
			integerSettingButton.onClick.AddListener(MySetInt);
		if (PlayerPrefs.HasKey(intSetString))
			intSet = PlayerPrefs.GetInt(intSetString);

	}

	void Restart()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	void MyToggler()
	{
		isToggled = !isToggled;
		GameUtil.SetPlayerPrefsBool(isToggledString, isToggled);
		PlayerPrefs.Save();
		SetMenuButtonState(anotherTogglerButton, isToggled);
	}

	void MySetInt()
	{
		intSet++;
		if (intSet > 4) intSet = 0;
		PlayerPrefs.SetInt(intSetString, intSet);
		PlayerPrefs.Save();
		integerSettingButton.GetComponentInChildren<Text>().text = "ANTAL: " + intSet;
	}

	protected override void SetMenuState()
	{
		if (anotherTogglerButton)
			SetMenuButtonState(anotherTogglerButton, isToggled);

		if (integerSettingButton)
			integerSettingButton.GetComponentInChildren<Text>().text = "ANTAL: " + intSet;

		base.SetMenuState();
	}

}
