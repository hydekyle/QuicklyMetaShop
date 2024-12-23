using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Video;
using Cysharp.Threading.Tasks;
using System.Threading;

public class VideoPlayerManager : MonoBehaviour
{
    [SerializeField] VideoPlayer videoPlayer;
    [SerializeField] Image blackImage;
    [SerializeField] float fadeVelocity = 3f;
    [SerializeField] UnityEvent onPlayVideo, onPauseVideo;

    void Start()
    {
        videoPlayer.Prepare();
    }

    public void BTN_MuteSpeaker()
    {
        videoPlayer.SetDirectAudioMute(0, videoPlayer.GetDirectAudioMute(0) == true ? false : true);
    }

    void Update()
    {
        blackImage.color = videoPlayer.isPlaying ?
            new Color(blackImage.color.r, blackImage.color.g, blackImage.color.b, Mathf.MoveTowards(blackImage.color.a, 0f, Time.deltaTime * fadeVelocity)) :
            new Color(blackImage.color.r, blackImage.color.g, blackImage.color.b, Mathf.MoveTowards(blackImage.color.a, 1f, Time.deltaTime * fadeVelocity));
    }

    public void Play()
    {
        if (!videoPlayer.isPlaying)
        {
            videoPlayer.Play();
            onPlayVideo.Invoke();
        }
        else
        {
            videoPlayer.Pause();
            onPauseVideo.Invoke();
        }
    }

}
