using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour {
	[SerializeField] GameObject m_gameObject;
	[SerializeField] uint m_size;
	List<GameObject> m_list;

	private void Awake() {
		m_list = new List<GameObject>();
		GameObject go;
		for (uint i = 0; i < m_size; i++) {
			go = Instantiate(m_gameObject, transform);
			go.SetActive(false);
			m_list.Add(go);
		}
	}

	public bool Request(out GameObject requested) {
		requested = null;
		foreach (GameObject go in m_list) {
			if (!go.activeInHierarchy) {
				go.SetActive(true);
				requested = go;
				break;
			}
		}
		return (requested != null);
	}
}