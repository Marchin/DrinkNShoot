using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class PlaceGrassPrefab : MonoBehaviour
{
	[MenuItem("Custom Scripts/Replace Grass")]

	static void PlaceGrass()
	{
		Transform[] elements = FindObjectsOfType<Transform>();
		string prefabName = "Grass";
		
		GameObject grass = Resources.Load(prefabName, typeof(GameObject)) as GameObject;

		int n = 1;
		foreach (Transform element in elements)
		{
			if (element.gameObject.layer == LayerMask.NameToLayer("Grass"))
			{
				GameObject gameObj = PrefabUtility.InstantiatePrefab(grass, SceneManager.GetActiveScene()) as GameObject;
				gameObj.name = prefabName + " (" + n + ")";
				gameObj.transform.position = element.position;
				gameObj.transform.rotation = element.rotation;

				DestroyImmediate(element.gameObject);
				n++;
			}
		}
	}

	[MenuItem("Custom Scripts/Replace Cacti")]

	static void PlaceCacti()
	{
		Transform[] elements = FindObjectsOfType<Transform>();
		string prefabName = "Cactus";
		
		GameObject cactus = Resources.Load(prefabName, typeof(GameObject)) as GameObject;

		int n = 1;
		foreach (Transform element in elements)
		{
			if (element.gameObject.layer == LayerMask.NameToLayer("Cactus"))
			{
				GameObject gameObj = PrefabUtility.InstantiatePrefab(cactus, SceneManager.GetActiveScene()) as GameObject;
				gameObj.name = prefabName + " (" + n + ")";
				gameObj.transform.position = element.position;
				gameObj.transform.rotation = element.rotation;

				DestroyImmediate(element.gameObject);
				n++;
			}
		}
	}
}
