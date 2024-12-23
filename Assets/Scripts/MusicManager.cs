using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] Sprite speakerON, speakerOFF;
    [SerializeField] Image speakerIconUI;

    void Start()
    {
        speakerIconUI.sprite = !audioSource.mute ? speakerOFF : speakerON;
    }

    public void BTN_Music()
    {
        audioSource.mute = !audioSource.mute;
        speakerIconUI.sprite = !audioSource.mute ? speakerOFF : speakerON;
    }
}
