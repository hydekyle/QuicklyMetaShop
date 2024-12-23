using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using Cysharp.Threading.Tasks;
using System;

public class VideoPlayerAutoStart : MonoBehaviour
{
    [SerializeField] VideoPlayer videoPlayer;
    [SerializeField] AudioSource audioSource;
    [SerializeField] GameObject removeOnInitialized;

    void Start()
    {
        audioSource.Play();

        // if (Application.isMobilePlatform) return;

        // videoPlayer.errorReceived += (vPlayer, errorMSG) =>
        // {
        //     if (!vPlayer.isPlaying) RetryVideoDelayed().Forget();
        // };

        videoPlayer.prepareCompleted += (preparedVideoPlayer) =>
        {
            removeOnInitialized.SetActive(false);
            preparedVideoPlayer.Play();
        };

        videoPlayer.loopPointReached += (vPlayer) =>
        {
            vPlayer.time = 0;
            vPlayer.Play();
        };

#if !UNITY_EDITOR
        PlayAfterUserInteraction().Forget();
#endif

    }

    async UniTaskVoid PlayAfterUserInteraction()
    {
        while (!Input.GetMouseButtonUp(0))
            await UniTask.Yield();

        await UniTask.Yield();
        PrepareVideo();
    }

    void PrepareVideo()
    {
        videoPlayer.Prepare();
    }

}
