using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnVisibilityChange : MonoBehaviour
{
    [SerializeField] UnityEvent onBecameVisible, onBecameInvisible;

    void OnBecameVisible()
    {
        onBecameVisible.Invoke();
    }

    void OnBecameInvisible()
    {
        onBecameInvisible.Invoke();
    }
}
