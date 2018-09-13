using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {
	uint m_crowCurrency;
	public uint CrowCurrency {
		get {
			return m_crowCurrency;
		}
		set {
			m_crowCurrency = value;
		}
	}
	
	private void Awake() {
		DontDestroyOnLoad(gameObject);
	}
}
