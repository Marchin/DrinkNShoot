using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
	public enum GfxSetting
	{
		Low, Medium, High, VeryHigh, Wild
	}

	[Header("Graphics Settings")]
	[SerializeField] TextMeshProUGUI gfxText;
	[SerializeField] GameObject decreaseGfxButton;
	[SerializeField] GameObject increaseGfxButton;
	
	[Header("Audio Settings")]
	[SerializeField] AudioMixer sfxMixer;
	[SerializeField] Slider sfxSlider;
	
	const string VERY_HIGH_STR = "Very High";
	const float MIXER_MULT = 20f;
	
	GfxSetting currentGfxSetting;

	void Start()
	{
		currentGfxSetting = GameManager.Instance.CurrentGfxSetting;
		sfxSlider.value = GameManager.Instance.CurrentSfxVolume;

		ChangeGfxText();

		if (currentGfxSetting == GfxSetting.Wild)
			increaseGfxButton.SetActive(false);
		if (currentGfxSetting == GfxSetting.Low)
			decreaseGfxButton.SetActive(false);
	}

	void ChangeGfxText()
	{
		if (currentGfxSetting != GfxSetting.VeryHigh)
			gfxText.text = currentGfxSetting.ToString();
		else
			gfxText.text = VERY_HIGH_STR;
	}

	float GetMixerLevel(AudioMixer audioMixer)
	{
		float volume;
		bool result = audioMixer.GetFloat("Volume", out volume);

		return result ? volume : 0f;
	}

	public void SetSfxVolume(float volume)
	{
		GameManager.Instance.CurrentSfxVolume = volume;
		sfxMixer.SetFloat("Volume", Mathf.Log(volume) * MIXER_MULT);
	}

	public void IncreaseGraphicsSetting()
	{
		if (currentGfxSetting != GfxSetting.Wild)
		{
			currentGfxSetting++;
			GameManager.Instance.CurrentGfxSetting = currentGfxSetting;
			QualitySettings.SetQualityLevel((int)currentGfxSetting);

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
		if (currentGfxSetting != GfxSetting.Low)
		{
			currentGfxSetting--;
			GameManager.Instance.CurrentGfxSetting = currentGfxSetting;
			QualitySettings.SetQualityLevel((int)currentGfxSetting);

			ChangeGfxText();

			decreaseGfxButton.GetComponent<Button>().interactable = false;

			if (currentGfxSetting == GfxSetting.Low)
				decreaseGfxButton.SetActive(false);
			else
				if (!increaseGfxButton.activeInHierarchy)
					increaseGfxButton.SetActive(true);

			decreaseGfxButton.GetComponent<Button>().interactable = true;			
		}
	}
}