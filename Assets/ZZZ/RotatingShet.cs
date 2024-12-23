using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingShet : MonoBehaviour
{
    public float speedRotation;

    void Update()
    {
        transform.Rotate(Vector3.up * Time.deltaTime * speedRotation);
    }
}
