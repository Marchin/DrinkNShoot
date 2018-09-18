using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
	enum GfxSetting
	{
		VeryLow, Low, Medium, High, VeryHigh, Wild
	}

	[Header("Graphics Settings")]
	[SerializeField] GfxSetting currentGfxSetting;
	[SerializeField] GameObject[] gfxOptions;
	[SerializeField] GameObject decreaseGfxButton;
	[SerializeField] GameObject increaseGfxButton;
	GameObject currentGfxOption;

	void Start()
	{
		currentGfxOption = gfxOptions[(int)currentGfxSetting];

		if (currentGfxSetting == GfxSetting.Wild)
			increaseGfxButton.SetActive(false);
		if (currentGfxSetting == GfxSetting.VeryLow)
			decreaseGfxButton.SetActive(false);

		currentGfxOption.SetActive(true);
	}

	public void IncreaseGraphicsSetting()
	{
		if (currentGfxSetting != GfxSetting.Wild)
		{
			currentGfxSetting++;
			currentGfxOption.SetActive(false);
			currentGfxOption = gfxOptions[(int)currentGfxSetting];
			currentGfxOption.SetActive(true);

			increaseGfxButton.GetComponent<Button>().interactable = false;

			if (currentGfxSetting == GfxSetting.Wild)
				increaseGfxButton.SetActive(false);
			else
				if (!decreaseGfxButton.activeInHierarchy)
					decreaseGfxButton.SetActive(true);

			increaseGfxButton.GetComponent<Button>().interactable = true;			
		}
	}

	public void DecreaseGraphicsSetting()
	{
		if (currentGfxSetting != GfxSetting.VeryLow)
		{
			currentGfxSetting--;
			currentGfxOption.SetActive(false);
			currentGfxOption = gfxOptions[(int)currentGfxSetting];
			currentGfxOption.SetActive(true);

			decreaseGfxButton.GetComponent<Button>().interactable = false;

			if (currentGfxSetting == GfxSetting.VeryLow)
				decreaseGfxButton.SetActive(false);
			else
				if (!increaseGfxButton.activeInHierarchy)
					increaseGfxButton.SetActive(true);

			decreaseGfxButton.GetComponent<Button>().interactable = true;			
		}
	}
}