using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI appVersionText;
	[SerializeField] AudioSource hoverOverButtonSound;

	void Start()
	{
		GameManager.Instance.ShowCursor();
		appVersionText.text = "v" + Application.version;
	}

	public void EnterStore()
	{
		GameManager.Instance.FadeToScene(GameManager.Instance.StoreScene);
	}

	public void Quit()
	{
		GameManager.Instance.QuitApplication();
	}

    public void PlayHoverOverButtonSound(Button button)
    {
        if (button.IsInteractable())
            hoverOverButtonSound.Play();
    }
}