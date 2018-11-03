using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IInput 
{
	float GetHorizontalViewAxis();
	float GetVerticalViewAxis();
	bool GetFireButton();
	bool GetReloadButton();
	bool GetPauseButton();
	float GetSwapItemAxis();
	bool GetUseItemButton();
}
