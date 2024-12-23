using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using Sirenix.Serialization;

namespace Quickly
{
    [Serializable, RequireComponent(typeof(Collider))]
    public class QuicklyInteractable : UniqueIdentifier, IClickable
    {
        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern void RequestChangeInteractionValue(string url);

        [ListDrawerSettings(ShowFoldout = true)]
        [SerializeReference]
        public List<IQuicklyValueUpdatable> availableInteraction;

        public void OnClick()
        {
            print(this.ToJSON());
            RequestChangeInteractionValue(this.ToJSON());
        }

        void OnValidate()
        {
            if (!TryGetComponent<Collider>(out var pinga))
            {
                gameObject.AddComponent<BoxCollider>();
            }
        }

        string ToJSON()
        {
            var interactions = "";
            foreach (var interaction in availableInteraction)
            {
                interactions += interaction.GetType().ToString() + ",";
            }

            return "{" + @$"
                uniqueID: " + '"' + $"{UniqueID}" + '"' + @$"
                availableInteraction: " + '"' + $"{interactions}" + '"' +
             "\n}";
        }

    }
}
