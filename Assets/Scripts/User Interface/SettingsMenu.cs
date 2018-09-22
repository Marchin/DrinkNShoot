﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
	enum GfxSetting
	{
		VeryLow, Low, Medium, High, VeryHigh, Wild
	}

	[Header("Graphics Settings")]
	[SerializeField] GfxSetting currentGfxSetting;
	[SerializeField] TextMeshProUGUI gfxText;
	[SerializeField] GameObject decreaseGfxButton;
	[SerializeField] GameObject increaseGfxButton;
	const string VERT_HIG_STR = "Very High";
	const string VERY_LOW_STR = "Very Low";

	void Start()
	{
		ChangeGfxText();

		if (currentGfxSetting == GfxSetting.Wild)
			increaseGfxButton.SetActive(false);
		if (currentGfxSetting == GfxSetting.VeryLow)
			decreaseGfxButton.SetActive(false);
	}

	void ChangeGfxText()
	{
		if (currentGfxSetting != GfxSetting.VeryHigh && currentGfxSetting != GfxSetting.VeryLow)
			gfxText.text = currentGfxSetting.ToString();
		else
		{
			if (currentGfxSetting == GfxSetting.VeryHigh)
				gfxText.text = VERT_HIG_STR;
			else
				gfxText.text = VERY_LOW_STR;
		}
	}

	public void IncreaseGraphicsSetting()
	{
		if (currentGfxSetting != GfxSetting.Wild)
		{
			currentGfxSetting++;

			ChangeGfxText();

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

			ChangeGfxText();

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