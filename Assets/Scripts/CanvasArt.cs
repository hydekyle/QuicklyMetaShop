using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class CanvasArt : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void OpenNewTabURL(string url);

    [SerializeField] GameObject textParent;
    [SerializeField] Image previewImage;
    [SerializeField] TMP_Text artTitle, artDescription, artAuthor;
    // [SerializeField] HorizontalLayoutGroup horizontalLayout;
    Image activePreviewImage;
    DataArt activeDataArt;
    float onOpenWidth;
    Vector3 cachedLocalScale;

    void Start()
    {
        cachedLocalScale = previewImage.rectTransform.localScale;
    }

    void Update()
    {
        // if (onOpenWidth != Screen.width)
        // {
        //     gameObject.SetActive(false);
        //     //DesertFreeFlightController.Instance.ShowArtInfo(activeDataArt);
        // }
        if (Input.GetKeyDown(KeyCode.Mouse1) && gameObject.activeSelf) BTN_ExitArtInfo();
        if (activePreviewImage != null)
        {
            activePreviewImage.rectTransform.localScale = new Vector3(
                Mathf.Clamp(activePreviewImage.rectTransform.localScale.x + Input.mouseScrollDelta.y * 0.1f, 0.5f, 100f),
                Mathf.Clamp(activePreviewImage.rectTransform.localScale.y + Input.mouseScrollDelta.y * 0.1f, 0.5f, 100f));
        }
    }

    void OnDisable()
    {
        previewImage.rectTransform.localScale = cachedLocalScale;
    }

    Vector3 previewInitialPosition;

    public void ShowArtInfo(DataArt artData)
    {
        if (previewInitialPosition == null) previewInitialPosition = previewImage.rectTransform.localPosition;
        gameObject.SetActive(true);
        previewImage.rectTransform.localPosition = previewInitialPosition;
        activeDataArt = artData;
        //onOpenWidth = Screen.width;
        previewImage.rectTransform.sizeDelta = new Vector2(artData.image.width, artData.image.height);
        previewImage.gameObject.SetActive(true);
        activePreviewImage = previewImage;
        previewImage.sprite = previewImage.sprite = Sprite.Create(artData.image, new Rect(0f, 0f, artData.image.width, artData.image.height), Vector2.one / 4, 100f);
        // artTitle.text = artData.title;
        // artAuthor.text = artData.author;
        artDescription.text = artData.description;
        // artDescription.rectTransform.sizeDelta = new Vector2(artDescription.rectTransform.sizeDelta.x, artDescription.renderedHeight);
        // artDescription.rectTransform.localPosition = new Vector3(artDescription.rectTransform.localPosition.x, 0, artDescription.rectTransform.localPosition.z);
        var hasText = artData.description != "";
        // if (horizontalLayout != null)
        // {
        //     horizontalLayout.padding.left = hasText ? -100 : 0;
        // }
        textParent.SetActive(hasText);
        DesertFreeFlightController.Instance.SetIsBusy(true);
    }

    public void BTN_ExitArtInfo()
    {
        gameObject.SetActive(false);
        DesertFreeFlightController.Instance.SetIsBusy(false);
        activePreviewImage = null;
    }

    public void BTN_MoreInfo()
    {
        OpenNewTabURL(activeDataArt.websLink);
    }

}
