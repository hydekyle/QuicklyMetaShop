using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerZone : MonoBehaviour
{
    [SerializeField] LayerMask targetLayer;
    [SerializeField] UnityEvent onTriggerEnter, onTriggerExit;

    void OnTriggerEnter(Collider other)
    {
        if ((targetLayer & 1 << other.gameObject.layer) == 1 << other.gameObject.layer)
            onTriggerEnter.Invoke();
    }

    void OnTriggerExit(Collider other)
    {
        if ((targetLayer & 1 << other.gameObject.layer) == 1 << other.gameObject.layer)
            onTriggerExit.Invoke();
    }

}
