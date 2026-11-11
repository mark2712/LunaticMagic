using UnityEngine;

public class Markers1 : MonoBehaviour
{
    public GameObject mainObject;
    private string _markerTag = "MeshSizeMarker";

    public string markerTag
    {
        get
        {
            if (!TagExists(_markerTag))
            {
                CreateTag(_markerTag);
            }
            return _markerTag;
        }
        set
        {
            if (!TagExists(value))
            {
                CreateTag(value);
            }
            _markerTag = value;
        }
    }

    public void Initialize(GameObject mainObject)
    {
        this.mainObject = mainObject;
    }

    public void CreateMarker(Vector3 position, string name)
    {
        GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Cube);
        marker.name = name;
        marker.transform.position = position;
        marker.transform.localScale = Vector3.one * 0.025f; // Уменьшение кубиков в 4 раза
        marker.transform.SetParent(mainObject.transform);

        // Установить тег
        if (!TagExists(markerTag))
        {
            CreateTag(markerTag);
        }
        marker.tag = markerTag;

        // Яркий цвет
        Renderer renderer = marker.GetComponent<Renderer>();
        renderer.sharedMaterial.color = Color.red; // Используем sharedMaterial
    }

    public void RemoveMarkers()
    {
        Transform[] allChildren = mainObject.GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            if (child.CompareTag(markerTag))
            {
                DestroyImmediate(child.gameObject);
            }
        }
    }

    public bool TagExists(string tag)
    {
        // foreach (string definedTag in UnityEditorInternal.InternalEditorUtility.tags)
        // {
        //     if (definedTag == tag)
        //     {
        //         return true;
        //     }
        // }
        return false;
    }

    public void CreateTag(string tag)
    {
        // SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        // SerializedProperty tagsProp = tagManager.FindProperty("tags");

        // for (int i = 0; i < tagsProp.arraySize; i++)
        // {
        //     SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
        //     if (t.stringValue == tag)
        //     {
        //         return;
        //     }
        // }

        // tagsProp.InsertArrayElementAtIndex(0);
        // tagsProp.GetArrayElementAtIndex(0).stringValue = tag;

        // tagManager.ApplyModifiedProperties();
    }

    public Transform FindChildByName(Transform parent, string name)
    {
        foreach (Transform child in parent.GetComponentsInChildren<Transform>())
        {
            if (child.name == name)
            {
                return child;
            }
        }
        return null;
    }
}