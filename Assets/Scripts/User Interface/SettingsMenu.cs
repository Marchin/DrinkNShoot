using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
	public enum GfxSetting
	{
		Low, Medium, Wild
	}

	[Header("Graphics Settings")]
	[SerializeField] TextMeshProUGUI gfxText;
	[SerializeField] GameObject decreaseGfxButton;
	[SerializeField] GameObject increaseGfxButton;
	
	[Header("Audio Settings")]
	[SerializeField] Slider sfxSlider;
	[SerializeField] Slider musicSlider;

	[Header("Other Settings")]
	[SerializeField] Slider mouseSensitivitySlider;
	[SerializeField] Toggle tutorialEnabledToggle;
	
	const float MIXER_MULT = 20f;

	void Start()
	{		
		sfxSlider.value = GameManager.Instance.CurrentSfxVolume;
		musicSlider.value = GameManager.Instance.CurrentMusicVolume;
		mouseSensitivitySlider.value = GameManager.Instance.CurrentMouseSensitivity;
		tutorialEnabledToggle.isOn = GameManager.Instance.TutorialEnabled;

        gfxText.text = GameManager.Instance.CurrentGfxSetting.ToString();

		if (GameManager.Instance.CurrentGfxSetting == GfxSetting.Wild)
			increaseGfxButton.SetActive(false);
		if (GameManager.Instance.CurrentGfxSetting == GfxSetting.Low)
			decreaseGfxButton.SetActive(false);
	}

	public void SetSfxVolume(float volume)
	{
		GameManager.Instance.CurrentSfxVolume = volume;
		GameManager.Instance.SfxMixer.SetFloat("Volume", Mathf.Log(volume) * MIXER_MULT);
	}

	public void SetMusicVolume(float volume)
	{
		GameManager.Instance.CurrentMusicVolume = volume;
		GameManager.Instance.MusicMixer.SetFloat("Volume", Mathf.Log(volume) * MIXER_MULT);
	}

	public void IncreaseGraphicsSetting()
	{
		if (GameManager.Instance.CurrentGfxSetting != GfxSetting.Wild)
		{
            GameManager.Instance.CurrentGfxSetting++;
			GameManager.Instance.CurrentGfxSetting = GameManager.Instance.CurrentGfxSetting;
			QualitySettings.SetQualityLevel((int)GameManager.Instance.CurrentGfxSetting);

            gfxText.text = GameManager.Instance.CurrentGfxSetting.ToString();

			increaseGfxButton.GetComponent<Button>().interactable = false;

			if (GameManager.Instance.CurrentGfxSetting == GfxSetting.Wild)
				increaseGfxButton.SetActive(false);
			else
				if (!decreaseGfxButton.activeInHierarchy)
					decreaseGfxButton.SetActive(true);

			increaseGfxButton.GetComponent<Button>().interactable = true;			
		}
	}

	public void DecreaseGraphicsSetting()
	{
		if (GameManager.Instance.CurrentGfxSetting != GfxSetting.Low)
		{
            GameManager.Instance.CurrentGfxSetting--;
			GameManager.Instance.CurrentGfxSetting = GameManager.Instance.CurrentGfxSetting;
			QualitySettings.SetQualityLevel((int)GameManager.Instance.CurrentGfxSetting);

            gfxText.text = GameManager.Instance.CurrentGfxSetting.ToString();

			decreaseGfxButton.GetComponent<Button>().interactable = false;

			if (GameManager.Instance.CurrentGfxSetting == GfxSetting.Low)
				decreaseGfxButton.SetActive(false);
			else
				if (!increaseGfxButton.activeInHierarchy)
					increaseGfxButton.SetActive(true);

			decreaseGfxButton.GetComponent<Button>().interactable = true;			
		}
	}

	public void SetMouseSensitivity(float sensitivity)
	{
		GameManager.Instance.CurrentMouseSensitivity = sensitivity;
	}

	public void SetTutorialEnabled(bool enabled)
	{
		GameManager.Instance.TutorialEnabled = enabled;
	}

	public static float GetActualVolume(float volume)
	{
		return (Mathf.Log(volume) * MIXER_MULT);
	}
}