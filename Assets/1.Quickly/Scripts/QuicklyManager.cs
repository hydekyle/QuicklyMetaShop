using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Quickly;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Cysharp.Threading.Tasks;

/// <summary>
/// Quickly main script
/// </summary>
public class QuicklyManager : MonoBehaviour
{
    public StoreData storeData;

    List<ReflectionProbe> probeList;
    [ShowInInspector, SerializeField]
    List<QuicklyInteractable> quicklyInteractables;

    [Header("DEVELOPMENT HTTP TEST")]
    public string devMessage;

    [Button()]
    public void DevButton()
    {
        ApplyMessage(devMessage).Forget();
    }

    public void ReceiveMessage(string message)
    {
        ApplyMessage(devMessage).Forget();
    }

    async UniTaskVoid ApplyMessage(string message)
    {
        var targetID = message.Split(',')[0];
        var selectedInteraction = message.Split(',')[1];
        var newValue = message.Split(',')[2];

        var interactable = quicklyInteractables.Find(p => p.UniqueID == targetID);
        var targetInteraction = interactable.availableInteraction.Find(i => i.GetType().ToString() == selectedInteraction);

        await targetInteraction.ValueUpdate(newValue);
        Refresh();
    }

    void Start()
    {
        // Cache recurrent references
        probeList = new List<ReflectionProbe>(FindObjectsByType<ReflectionProbe>(FindObjectsSortMode.None));
        quicklyInteractables = new List<QuicklyInteractable>(FindObjectsByType<QuicklyInteractable>(FindObjectsSortMode.None));

        // Load user preferences for the virtual space
        // TextAsset jsonData = Resources.Load<TextAsset>("store_data");
        // storeData = JsonUtility.FromJson<StoreData>(jsonData.text);

        InitStore();
    }

    /// <summary>
    /// Load user preferences for the virtual space and update visual settings
    /// </summary>
    void InitStore()
    {
        // Update image for big wallpaper
        //await ImageDownloader.GetSpriteByURL(targetImage, storeData.mainImage);
        Refresh();
    }

    void Refresh()
    {
        // Update all reflection probes after changes
        probeList.ForEach(probe => probe.RenderProbe());
    }

}
