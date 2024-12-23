using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableArt : MonoBehaviour, IClickable
{
    public DataArt artData;

    public void OnClick()
    {
        DesertFreeFlightController.Instance.ShowArtInfo(artData);
    }

}

