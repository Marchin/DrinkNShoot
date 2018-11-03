using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputPC : IInput 
{
	public float GetHorizontalViewAxis()
	{
		return Input.GetAxis("Mouse X");
	}

	public float GetVerticalViewAxis()
	{
		return Input.GetAxis("Mouse Y");
	}

	public bool GetFireButton()
	{
		return Input.GetButtonDown("Fire");
	}

	public bool GetReloadButton()
	{
		return Input.GetButtonDown("Reload");
	}

	public bool GetPauseButton()
	{
		return Input.GetButtonDown("Cancel");
	}

	public float GetSwapItemAxis()
	{
		return Input.GetAxis("Mouse Scroll Wheel");
	}

	public bool GetUseItemButton()
	{
		return Input.GetButtonDown("Use Item");
	}
}
