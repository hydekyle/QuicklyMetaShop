using System.Runtime.InteropServices;
using UnityEngine;

public class StandingKioskScreen : MonoBehaviour, IClickable
{
    public string targetURL;

    [DllImport("__Internal")]
    private static extern void OpenNewTabURL(string url);

    public void OnClick()
    {
        OpenNewTabURL(targetURL);
    }
}
