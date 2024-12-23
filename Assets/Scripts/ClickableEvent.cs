using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ClickableEvent : MonoBehaviour, IClickable
{
    [SerializeField] UnityEvent unityEvent;

    public void OnClick()
    {
        unityEvent.Invoke();
    }

}
