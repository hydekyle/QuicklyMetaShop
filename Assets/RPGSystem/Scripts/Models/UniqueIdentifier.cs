using Sirenix.OdinInspector;
using UnityEngine;

namespace RPGSystem
{
    public class UniqueIdentifier : MonoBehaviour
    {
        [SerializeField]
        int _id;
        [ShowInInspector]
        public int ID { get => _id; }

#if UNITY_EDITOR
        public UniqueIdentifier()
        {
            _id = GetHashCode();
        }
#endif
    }

}