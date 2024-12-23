using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MarksAssets.VideoPlayerWebGL;
using Cysharp.Threading.Tasks;

public class VideoPlayerLoader : MonoBehaviour
{
    [SerializeField] VideoPlayerWebGL videoPlayer;

    void Start()
    {
        PlayAfterUserInteraction().Forget();
    }

    async UniTaskVoid PlayAfterUserInteraction()
    {
        while (!Input.GetMouseButton(0))
            await UniTask.Yield();

        videoPlayer.PlayPointerDown();
    }
}
