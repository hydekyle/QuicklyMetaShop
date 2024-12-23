using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class AddressableSpawner : MonoBehaviour
{
    public AssetReferenceGameObject assetReference;

#if UNITY_EDITOR
    [Button("Instantiate as GameObject")]
    void InstantiateReference()
    {
        var go = (GameObject)PrefabUtility.InstantiatePrefab(assetReference.editorAsset);
        go.transform.SetParent(transform);
        go.transform.localPosition = Vector3.zero;
    }

    [Button("Remove GameObject")]
    void RemoveGameObject()
    {
        DestroyImmediate(transform.GetChild(0).gameObject);
    }
#endif

    void Awake()
    {
        if (transform.childCount == 0)
            AddressablesQueue.addressableSpawnerList.Add(this);
    }

}

public static class AddressablesQueue
{
    public static List<AddressableSpawner> addressableSpawnerList = new();

    /// <summary>Download and instantiate addressables one by one</summary>
    public static async UniTask StartDownloadAndInstantiateQueue()
    {
        var enumerator = addressableSpawnerList.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var spawner = enumerator.Current;
            var spawn = spawner.assetReference.InstantiateAsync(spawner.transform.position, Quaternion.identity);
            while (!spawn.IsDone) await UniTask.NextFrame();
        }
    }
}
