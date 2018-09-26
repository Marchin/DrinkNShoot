using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EndLevelMenu : MonoBehaviour 
{
	[SerializeField] UnityEvent onContinue;

	public void Restart()
	{
		Time.timeScale = 1;
		LevelManager.Instance.RestartLevel();
	}

	public void PlayNextStage()
	{
		Time.timeScale = 1;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		LevelManager.Instance.MoveToNextStage();
		onContinue.Invoke();
	}

	public void Quit()
	{
		Time.timeScale = 1;
		LevelManager.Instance.QuitLevel();
	}

	public UnityEvent OnContinue
	{
		get { return onContinue; }
	}
}
