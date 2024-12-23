using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class ReflectionProveUpdater : MonoBehaviour
{
    [SerializeField] ReflectionProbe prover;

    void Start()
    {
        Pinga().Forget();
    }

    async UniTaskVoid Pinga()
    {
        while (gameObject.activeSelf)
        {
            prover.RenderProbe();
            await UniTask.Delay(300);
        }
    }
}
