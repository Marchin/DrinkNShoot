using UnityEngine;
using System.Collections;

public class BlurControl : MonoBehaviour {
	[SerializeField] float deltaBlur;
	[SerializeField] DrunkCamera drunkCamera;
	float value; 
	float targetValue;
	
	// Use this for initialization
	void Awake () {
		value = 0.3f;
		targetValue = value;
		transform.GetComponent<Renderer>().material.SetFloat("_blurSizeXY",value);
		LevelManager.Instance.OnStartNextStage.AddListener(IncrementBlur);
	}

	private void Update() {
		float intensity = drunkCamera.GetTrembleSpeed01(); 
		value = targetValue * intensity * intensity;
		transform.GetComponent<Renderer>().material.SetFloat("_blurSizeXY",value);
	}

	public void IncrementBlur() {
		Invoke("UpdateBlur", 1.7f);
	}

	void UpdateBlur() {
		targetValue += deltaBlur;
	}
	
	// Update is called once per frame
	// void Update () {
	// 	float axis = Input.GetAxisRaw("Vertical");
	// 	if(axis > 0f)
	// 	{
	// 		value = value + Time.deltaTime;
	// 		if (value>20f) value = 20f;
	// 		transform.GetComponent<Renderer>().material.SetFloat("_blurSizeXY",value);
	// 	}
	// 	else if(axis<0f)
	// 	{
	// 		value = (value - Time.deltaTime) % 20.0f;
	// 		if (value<0f) value = 0f;
	// 		transform.GetComponent<Renderer>().material.SetFloat("_blurSizeXY",value);
	// 	}		
	// }
	
	// void OnGUI () {
	// 	GUI.TextArea(new Rect(10f,10f,200f,50f), "Press the 'Up' and 'Down' arrows \nto interact with the blur plane\nCurrent value: "+value);
	// 	}
}
