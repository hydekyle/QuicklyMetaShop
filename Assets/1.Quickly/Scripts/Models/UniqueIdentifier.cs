using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Quickly
{
    public class UniqueIdentifier : MonoBehaviour
    {
        public string UniqueID;

        public static SavedInteractionDictionary cachedUniqueIdentifiers = new();

#if UNITY_EDITOR
        public UniqueIdentifier()
        {
            UniqueID = GetHashCode().ToString();
        }
#endif
    }

}