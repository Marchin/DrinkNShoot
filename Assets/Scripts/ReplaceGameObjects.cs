using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class ReplaceGameObjects : MonoBehaviour
{
	static void Replace(Transform[] elements, GameObject prefab, string prefabName, string layerName)
	{
        int n = 1;
        foreach (Transform element in elements)
        {
            if (element.gameObject.layer == LayerMask.NameToLayer(layerName))
            {
                GameObject gameObj = PrefabUtility.InstantiatePrefab(prefab, SceneManager.GetActiveScene()) as GameObject;
                gameObj.name = prefabName + " (" + n + ")";
                gameObj.transform.position = element.position;
                gameObj.transform.rotation = element.rotation;

                DestroyImmediate(element.gameObject);
                n++;
            }
        }
	}

	[MenuItem("Custom Scripts/Replace Grass")]

	static void PlaceGrass()
	{
		Transform[] elements = FindObjectsOfType<Transform>();
		string prefabName = "Grass 2";
		string layerName = "Grass";
		
		GameObject grass = Resources.Load(prefabName, typeof(GameObject)) as GameObject;

		Replace(elements, grass, prefabName, layerName);
	}

	[MenuItem("Custom Scripts/Replace Cacti 1")]

	static void PlaceCacti1()
	{
		Transform[] elements = FindObjectsOfType<Transform>();
		string prefabName = "Cactus 1";
		string layerName = "Cactus1";
		
		GameObject cactus = Resources.Load(prefabName, typeof(GameObject)) as GameObject;
		
		Replace(elements, cactus, prefabName, layerName);
	}

	[MenuItem("Custom Scripts/Replace Cacti 2")]

	static void PlaceCacti2()
	{
		Transform[] elements = FindObjectsOfType<Transform>();
		string prefabName = "Cactus 2";
		string layerName = "Cactus2";
		
		GameObject cactus = Resources.Load(prefabName, typeof(GameObject)) as GameObject;
		
		Replace(elements, cactus, prefabName, layerName);
	}

	[MenuItem("Custom Scripts/Replace Cacti 3")]

	static void PlaceCacti3()
	{
		Transform[] elements = FindObjectsOfType<Transform>();
		string prefabName = "Cactus 3";
		string layerName = "Cactus3";
		
		GameObject cactus = Resources.Load(prefabName, typeof(GameObject)) as GameObject;

		Replace(elements, cactus, prefabName, layerName);
	}

	[MenuItem("Custom Scripts/Replace Wood 1")]

	static void PlaceWood1()
	{
		Transform[] elements = FindObjectsOfType<Transform>();
		string prefabName = "Wood 1";
		string layerName = "Wood1";
		
		GameObject wood = Resources.Load(prefabName, typeof(GameObject)) as GameObject;

		Replace(elements, wood, prefabName, layerName);
	}

    [MenuItem("Custom Scripts/Replace Wood 2")]

    static void PlaceWood2()
    {
        Transform[] elements = FindObjectsOfType<Transform>();
        string prefabName = "Wood 2";
        string layerName = "Wood2";

        GameObject wood = Resources.Load(prefabName, typeof(GameObject)) as GameObject;

        Replace(elements, wood, prefabName, layerName);
    }

    [MenuItem("Custom Scripts/Replace Wood 3")]

    static void PlaceWood3()
    {
        Transform[] elements = FindObjectsOfType<Transform>();
        string prefabName = "Wood 3";
        string layerName = "Wood3";

        GameObject wood = Resources.Load(prefabName, typeof(GameObject)) as GameObject;

        Replace(elements, wood, prefabName, layerName);
    }

	
    [MenuItem("Custom Scripts/Replace Rocks")]

    static void PlaceRocks()
    {
        Transform[] elements = FindObjectsOfType<Transform>();
        string prefabName = "Rock";
        string layerName = "Rocks";

        GameObject rock = Resources.Load(prefabName, typeof(GameObject)) as GameObject;

        Replace(elements, rock, prefabName, layerName);
    }

	[MenuItem("Custom Scripts/Replace Boxes")]

    static void PlaceBoxes()
    {
        Transform[] elements = FindObjectsOfType<Transform>();
        string prefabName = "Box";
        string layerName = "Boxes";

        GameObject box = Resources.Load(prefabName, typeof(GameObject)) as GameObject;

        Replace(elements, box, prefabName, layerName);
    }

	[MenuItem("Custom Scripts/Replace Mountains")]

    static void PlaceMountains()
    {
        Transform[] elements = FindObjectsOfType<Transform>();
        string prefabName = "Mountains";
        string layerName = "Mountains";

        GameObject mountain = Resources.Load(prefabName, typeof(GameObject)) as GameObject;

        Replace(elements, mountain, prefabName, layerName);
	}

    [MenuItem("Custom Scripts/Replace Barrels")]

    static void PlaceBarrels()
    {
        Transform[] elements = FindObjectsOfType<Transform>();
        string prefabName = "Barrel";
        string layerName = "Barrels";

        GameObject barrel = Resources.Load(prefabName, typeof(GameObject)) as GameObject;

        Replace(elements, barrel, prefabName, layerName);
    }

    [MenuItem("Custom Scripts/Replace Fences")]

    static void PlaceFences()
    {
        Transform[] elements = FindObjectsOfType<Transform>();
        string prefabName = "Fence";
        string layerName = "Fences";

        GameObject fence = Resources.Load(prefabName, typeof(GameObject)) as GameObject;

        Replace(elements, fence, prefabName, layerName);
    }

	[MenuItem("Custom Scripts/Replace Tombs")]

    static void PlaceTombs()
    {
        Transform[] elements = FindObjectsOfType<Transform>();
        string prefabName = "Tomb";
        string layerName = "Tombs";

        GameObject tomb = Resources.Load(prefabName, typeof(GameObject)) as GameObject;

        Replace(elements, tomb, prefabName, layerName);
    }
}
