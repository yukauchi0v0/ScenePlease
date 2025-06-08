#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class MissingScriptFinder
{
    [MenuItem("Tools/🔍 Find Missing Scripts In Scene")]
    public static void FindMissingScripts()
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        int count = 0;

        foreach (GameObject go in allObjects)
        {
            Component[] components = go.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] == null)
                {
                    Debug.LogWarning($"Missing script found in: {GetFullPath(go)}", go);
                    count++;
                }
            }
        }

        if (count == 0)
            Debug.Log("✅ 沒有遺失的 Script！");
        else
            Debug.LogError($"❌ 共發現 {count} 個遺失的 Script！");
    }

    private static string GetFullPath(GameObject obj)
    {
        string path = obj.name;
        while (obj.transform.parent != null)
        {
            obj = obj.transform.parent.gameObject;
            path = obj.name + "/" + path;
        }
        return path;
    }
}
#endif
