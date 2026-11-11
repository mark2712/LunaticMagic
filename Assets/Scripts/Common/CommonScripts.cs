using System.Collections.Generic;
using UnityEngine;




public static class CounterFPS
{
    static int fpsNow = 120;
    static CounterFPS() { }

    public static void Inc()
    {
        QualitySettings.vSyncCount = 0;
        fpsNow += 30;
        Application.targetFrameRate = fpsNow;

        if (fpsNow > 120)
        {
            fpsNow = 30;
            Application.targetFrameRate = fpsNow;
        }
        // Time.fixedDeltaTime = 1f / Application.targetFrameRate;
    }
}



public static class CommonScripts
{
    /// <summary>
    /// Находит объект с точным совпадением имени в иерархии
    /// </summary>
    public static GameObject FindChildByName(GameObject parent, string name)
    {
        if (parent.name == name)
            return parent;

        foreach (Transform child in parent.transform)
        {
            GameObject result = FindChildByName(child.gameObject, name);
            if (result != null)
                return result;
        }

        return null;
    }

    /// <summary>
    /// Находит все объекты, содержащие подстроку в имени, и возвращает их иерархически
    /// </summary>
    public static List<GameObject> FindDirectChildrenBySubstring(GameObject parent, string substring)
    {
        List<GameObject> result = new List<GameObject>();
        foreach (Transform child in parent.transform)
        {
            if (child.name.Contains(substring))
            {
                result.Add(child.gameObject);
            }
        }
        return result;
    }

    public static SkinnedMeshRenderer FindMeshRenderer(GameObject model, string name)
    {
        foreach (var renderer in model.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            if (renderer.name == name)
                return renderer;
        }
        return null;
    }
}


