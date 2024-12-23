
using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Quickly
{
    [Serializable]
    public class TransformInteraction : IQuicklyValueUpdatable
    {
        public Transform targetTransform;

        public async UniTask ValueUpdate(string value)
        {
            if (targetTransform != null)
            {
                var valueList = value.Split(',');
                var x = float.Parse(valueList[0]);
                var y = float.Parse(valueList[1]);
                var z = float.Parse(valueList[2]);
                targetTransform.position = targetTransform.position = new Vector3(x, y, z);
            }
        }

    }

}