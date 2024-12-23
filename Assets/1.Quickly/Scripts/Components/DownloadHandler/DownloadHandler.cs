using System.ComponentModel;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Quickly
{
    public static class DownloadHandler
    {
        public static async UniTask<Sprite> GetSpriteByURL(string url)
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Texture2D downloadedTexture = DownloadHandlerTexture.GetContent(request);
                return Sprite.Create(downloadedTexture, new Rect(0, 0, downloadedTexture.width, downloadedTexture.height), Vector2.zero);
            }
            else
            {
                throw new WarningException("Error downloading image: " + request.error);
            }
        }

    }
}
