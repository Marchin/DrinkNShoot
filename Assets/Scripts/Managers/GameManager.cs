using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	static GameManager instance;
	[SerializeField] SettingsMenu.GfxSetting currentGfxSetting;
	[SerializeField] float currentSfxVolume = 0.75f;

	void Awake()
	{
		if (Instance == this)
			DontDestroyOnLoad(gameObject);
		else
			Destroy(gameObject);

	}

	void Start()
	{
		QualitySettings.SetQualityLevel((int)currentGfxSetting);
	}

	public static GameManager Instance
	{
		get
		{
			if (!instance)
			{
				instance = FindObjectOfType<GameManager>();

				if (!instance)
				{
					GameObject gameObj = new GameObject("Game Manager");
					instance = gameObj.AddComponent<GameManager>();
				}
			}
			
			return instance;
		}
	}

	public SettingsMenu.GfxSetting CurrentGfxSetting
	{
		get { return currentGfxSetting; }
		set { currentGfxSetting = value; }
	}

	public float CurrentSfxVolume
	{
		get { return currentSfxVolume; }
		set { currentSfxVolume = value; }
	}
}
