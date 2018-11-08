using UnityEngine;

public class BlurControl : MonoBehaviour {
	[SerializeField] float deltaBlur;
	[SerializeField] DrunkCamera drunkCamera;
	float value; 
	float targetValue;
	
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
}