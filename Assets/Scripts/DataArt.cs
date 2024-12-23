using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptables/DataArt")]
public class DataArt : ScriptableObject
{
    [TextArea()]
    public string description;
    public string websLink;
    public Texture2D image;
}
