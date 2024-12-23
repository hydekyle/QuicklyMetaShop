using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptables/DataCloth")]
public class Purchasable : ScriptableObject
{
    [DllImport("__Internal")]
    private static extern void OpenNewTabURL(string url);

    public string goToShopLink;
    public string title;
    public float price;
    [TextArea(6, 16)]
    public string description;
    public List<Material> colorVariants = new();

    public void GoToShop()
    {
        OpenNewTabURL(goToShopLink);
    }
}
