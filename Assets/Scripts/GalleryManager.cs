using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.Video;

public class GalleryManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    async void Start()
    {
        await UniTask.Delay(3000);
        videoPlayer.Play();
        // videoPlayer.loopPointReached += (vPlayer) =>
        // {
        //     vPlayer.Play();
        // };
    }
}
