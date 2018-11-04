using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour {
	private enum PoolType {
		Enemy
	}

	[SerializeField] PoolType poolType;
	[SerializeField] GameObject m_gameObject;
	[SerializeField] int m_size;
	List<GameObject> m_list;
	Transform poolObjectsParent;
	const int m_initSize = 10;

	private void Awake() {
		foreach (Transform child in transform) {
			if (child.name == "Pool of Objects") {
				poolObjectsParent = child;
			}
		}
	}

	private void Start() {
		m_list = new List<GameObject>();
		for (uint i = 0; i < m_initSize; i++) {
			AddObjectToPool();
		}
	}

	public bool Request(out GameObject requested) {
		requested = null;
		for (int i = 0; i < m_size; i++) {
			GameObject go = m_list[i];
			if (!go.activeInHierarchy) {
				go.SetActive(true);
				requested = go;
				break;
			}
		}
		return (requested != null);
	}

	public bool RequestActive(out GameObject requested) {
		requested = null;
		int randomOffset = Random.Range(0, m_size);
		for (int i = 0; i < m_size; i++) {
			if (m_list[(i + randomOffset) % m_list.Count].activeInHierarchy) {
				requested = m_list[(i + randomOffset) % m_size];
				break;
			}
		}
		return (requested != null);
	}

	public void DisableAll() {
		foreach (GameObject go in m_list) {
			if (go.activeInHierarchy) {
				go.SetActive(false);
			}
		}
	}

	public void AddObjectToPool() {
		GameObject go;
		go = Instantiate(m_gameObject, poolObjectsParent);
		if (poolType == PoolType.Enemy)
			LevelManager.Instance.AddEnemyLife(go.GetComponent<Life>());
		go.SetActive(false);
		m_list.Add(go);
	}

	public void SetSize(int size) {
		m_size = size;
		while (m_list.Count < m_size) {
			AddObjectToPool();
		}
	}
}