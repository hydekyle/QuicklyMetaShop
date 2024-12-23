using Quickly;
using UnityEngine;

[CreateAssetMenu(menuName = "Quickly Virtual Space/StoreData")]
public class StoreData : ScriptableObject
{
    public string logoURL;
    public string shopName;
    public SavedInteractionDictionary savedInteractions;
}
