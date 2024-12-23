using System;
using UnityEngine.UI;
using UnityEngine;
using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks;

namespace Quickly
{
    [Serializable]
    public class ImageChangeInteraction : IQuicklyValueUpdatable
    {
        [ShowIf("@targetSpriteRenderer == null")]
        public Image targetImage;
        [ShowIf("@targetImage == null")]
        public SpriteRenderer targetSpriteRenderer;

        public async UniTask ValueUpdate(string imageURL)
        {
            var spriteImage = await DownloadHandler.GetSpriteByURL(imageURL);

            if (targetImage != null)
            {
                targetImage.sprite = spriteImage;

            }
            if (targetSpriteRenderer != null)
            {
                targetSpriteRenderer.sprite = spriteImage;
            }
        }

    }

}

