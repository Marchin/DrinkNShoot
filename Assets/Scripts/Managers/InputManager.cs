using UnityEngine;

public class InputManager : MonoBehaviour 
{
	static InputManager instance;
	
	IInput input;

	void Awake()
	{
		if (Instance == this)
		{
			DontDestroyOnLoad(gameObject);

			#if UNITY_STANDALONE
				input = new InputPC();
			#endif
		}
		else
			Debug.Log("An Input Manager already exists in the scene; avoid duplicates.", gameObject);
	}

    public float GetHorizontalViewAxis()
    {
        return input.GetHorizontalViewAxis();
    }

    public float GetVerticalViewAxis()
    {
        return input.GetVerticalViewAxis();
    }

    public bool GetFireButton()
    {
        return input.GetFireButton();
    }

    public bool GetReloadButton()
    {
        return input.GetReloadButton();
    }

	public bool GetPauseButton()
    {
        return input.GetPauseButton();
    }

	public float GetSwapItemAxis()
    {
        return input.GetSwapItemAxis();
    }

	public bool GetUseItemButton()
	{
		return input.GetUseItemButton();
	}

	public static InputManager Instance
	{
		get
		{
			if (!instance)
			{
				instance = FindObjectOfType<InputManager>();
				
				if (!instance)
				{
					GameObject gameObj = new GameObject("Input Manager");
					instance = gameObj.AddComponent<InputManager>();
				}
			}
			
			return instance;
		}
	}
}