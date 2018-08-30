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
		return Input.GetButtonDown("Fire1");
	}

	public bool GetReloadButton()
	{
		return Input.GetButton("Reload");
	}
}
