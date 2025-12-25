using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class SceneRandomReplaceGroupWindow : EditorWindow
{
    private string targetPrefix = "Old_";
    private GameObject[] newPrefabs = new GameObject[1];
    private string groupName = "ReplacedObjects";

    [MenuItem("Tools/Scene Random Replace Group")]
    public static void ShowWindow()
    {
        GetWindow<SceneRandomReplaceGroupWindow>("Scene Random Replace Group");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Target Settings", EditorStyles.boldLabel);
        targetPrefix = EditorGUILayout.TextField("Target Name Prefix", targetPrefix);
        groupName = EditorGUILayout.TextField("New Group Name", groupName);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("New Prefabs", EditorStyles.boldLabel);

        int size = Mathf.Max(1, EditorGUILayout.IntField("Array Size", newPrefabs.Length));
        if (size != newPrefabs.Length)
            System.Array.Resize(ref newPrefabs, size);

        for (int i = 0; i < newPrefabs.Length; i++)
        {
            newPrefabs[i] = (GameObject)EditorGUILayout.ObjectField($"Prefab {i}", newPrefabs[i], typeof(GameObject), false);
        }

        EditorGUILayout.Space();
        if (GUILayout.Button("Replace All Randomly"))
        {
            ReplaceInScene();
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Instructions:");
        EditorGUILayout.LabelField("- Objects with matching name prefix will be replaced by random prefabs.");
        EditorGUILayout.LabelField("- Original objects are not modified.");
        EditorGUILayout.LabelField("- New objects are parented under a new group in the scene.");
        EditorGUILayout.LabelField("- Only world position and rotation are copied; scale is from the new prefab.");
    }

    private void ReplaceInScene()
    {
        if (string.IsNullOrEmpty(targetPrefix))
        {
            Debug.LogError("Please set a target prefix.");
            return;
        }

        if (newPrefabs.Length == 0 || newPrefabs[0] == null)
        {
            Debug.LogError("Please assign at least one new prefab.");
            return;
        }

        // Step 1: 收集匹配对象
        List<GameObject> targets = new List<GameObject>();
        foreach (GameObject obj in GameObject.FindObjectsOfType<GameObject>())
        {
            if (obj.name.StartsWith(targetPrefix))
                targets.Add(obj);
        }

        if (targets.Count == 0)
        {
            Debug.LogWarning("No matching objects found in the scene.");
            return;
        }

        // Step 2: 创建新的 Group
        GameObject group = new GameObject(groupName);
        Undo.RegisterCreatedObjectUndo(group, "Create New Group");

        int count = 0;

        // Step 3: 遍历匹配对象，生成新对象到 Group 下
        foreach (GameObject obj in targets)
        {
            if (obj == null) continue;

            GameObject chosenPrefab = newPrefabs[Random.Range(0, newPrefabs.Length)];
            if (chosenPrefab == null) continue;

            GameObject newObj;

            if (PrefabUtility.GetPrefabAssetType(chosenPrefab) != PrefabAssetType.NotAPrefab)
            {
                newObj = (GameObject)PrefabUtility.InstantiatePrefab(chosenPrefab);
            }
            else
            {
                newObj = Instantiate(chosenPrefab);
                newObj.name = chosenPrefab.name;
            }

            Undo.RegisterCreatedObjectUndo(newObj, "Instantiate New Object");

            // 设置父级为新的 Group
            newObj.transform.SetParent(group.transform, worldPositionStays: true);

            // 复制世界坐标和旋转
            newObj.transform.position = obj.transform.position;
            newObj.transform.rotation = obj.transform.rotation;

            count++;
        }

        Debug.Log($"Randomly replaced {count} objects in the scene with prefix '{targetPrefix}' under group '{groupName}'.");
    }
}
