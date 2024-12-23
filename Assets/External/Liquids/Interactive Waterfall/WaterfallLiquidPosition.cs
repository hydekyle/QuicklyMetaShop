using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterfallLiquidPosition : MonoBehaviour
{
    void Update()
    {
        Shader.SetGlobalVector("_ObjectPosition", new Vector4(transform.position.x, transform.position.y, transform.position.z, transform.localScale.x));
    }
}
