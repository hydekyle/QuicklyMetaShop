using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Michsky.UI.ModernUIPack;
using System.Linq;
using DG.Tweening;
using System.Runtime.InteropServices;

public class CanvasPurchaseItem : MonoBehaviour
{
    public static CanvasPurchaseItem Instance;
    [SerializeField] HorizontalSelector horizontalSelector;
    [SerializeField] TMP_Text titleText, descriptionText, priceText, clickToExpandText;
    [SerializeField] GameObject purchaseUI;
    [SerializeField] CanvasGroup canvasGroupPreview;
    [SerializeField] RectTransform rootRectTransformPreview, targetAnimPositionPreviewShow, targetAnimPositionPreviewHide;
    [SerializeField] float fadeTransitionSpeed;
    public bool isInfoOpened, isVisible;
    public int punchForce, vibratio, elasticity;
    public float punchDuration;
    ExpositionItem activeExpositionItem;

    [DllImport("__Internal")]
    private static extern void OpenNewTabURL(string url);

    public void BTN_GoToShop()
    {
        OpenNewTabURL(activeExpositionItem.purchasable.goToShopLink);
    }

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (activeExpositionItem == null) return;

        if (Input.GetKeyDown(KeyCode.Escape)) Back();

        canvasGroupPreview.alpha = Mathf.Lerp(
                canvasGroupPreview.alpha,
                isVisible ? 1f : 0f,
                Time.deltaTime * fadeTransitionSpeed
            );

        rootRectTransformPreview.localPosition = Vector3.Lerp(
            rootRectTransformPreview.localPosition,
            isInfoOpened ? targetAnimPositionPreviewShow.localPosition : targetAnimPositionPreviewHide.localPosition,
            Time.deltaTime * fadeTransitionSpeed
        );
    }

    public void Back()
    {
        if (isInfoOpened) SwapInfoPosition();
        else
        {
            isVisible = false;
            purchaseUI.SetActive(false);
            if (horizontalSelector.gameObject.activeSelf)
                horizontalSelector.itemList[activeExpositionItem.defaultIndexHorizontalSelector].onItemSelect.Invoke();
            activeExpositionItem.EscapePreview();
            activeExpositionItem = null;
        }
    }

    public void ShowItem(ExpositionItem expositionItem)
    {
        if (expositionItem == null) return;
        activeExpositionItem = expositionItem;
        titleText.text = expositionItem.purchasable.title;
        descriptionText.text = expositionItem.purchasable.description;
        priceText.text = expositionItem.purchasable.price.ToString();
        rootRectTransformPreview.localPosition = targetAnimPositionPreviewHide.localPosition;
        isVisible = true;
        PopulateHorizontalSelector(expositionItem);
        purchaseUI.SetActive(true);
    }

    public void PopulateHorizontalSelector(ExpositionItem expositionItem)
    {
        var colorVariantList = expositionItem.purchasable.colorVariants;
        if (colorVariantList.Count == 0)
        {
            horizontalSelector.gameObject.SetActive(false);
            return;
        }
        horizontalSelector.gameObject.SetActive(true);
        List<HorizontalSelector.Item> hzItemList = new();
        for (var x = 0; x < colorVariantList.Count; x++)
        {
            var hsItem = new HorizontalSelector.Item();
            hsItem.itemTitle = String.Format("Option {0}", x + 1);
            var materials = expositionItem.clotheMeshRenderer.materials.ToList();
            for (var y = 0; y < materials.Count; y++)
            {
                materials[y] = colorVariantList[x];
            }
            hsItem.onItemSelect.AddListener(() =>
            {
                expositionItem.clotheMeshRenderer.SetMaterials(materials);
                var punchDirection = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
                expositionItem.transform.DOBlendablePunchRotation(punchDirection * punchForce, punchDuration, vibratio, elasticity);
            });
            hzItemList.Add(hsItem);
        }
        horizontalSelector.itemList = hzItemList;
        horizontalSelector.defaultIndex = expositionItem.defaultIndexHorizontalSelector;
        horizontalSelector.SetupSelector();
    }

    public void SwapInfoPosition()
    {
        isInfoOpened = !isInfoOpened;
        clickToExpandText.text = isInfoOpened ? "CLICK TO HIDE INFO" : "CLICK TO SHOW INFO";
    }

    public void SetIsVisible(bool shouldBeVisible)
    {
        isVisible = shouldBeVisible;
        canvasGroupPreview.blocksRaycasts = shouldBeVisible;
    }

}
