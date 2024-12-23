using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ExpositionItem : MonoBehaviour, IClickable
{
    public static ExpositionItem activeExpositionItem;
    [Tooltip("Assign other GameObject MeshRenderer instead this one")]
    public MeshRenderer clotheMeshRenderer;
    public float minDistanceForPlayerCamera = 1.2f;
    Vector3 originPosition;
    Quaternion originRotation;
    bool isAnimating = false;
    bool isPreviewModeActive = false;
    bool shouldRotate;
    public Purchasable purchasable;
    public int defaultIndexHorizontalSelector = 0;

    [SerializeField] float previewAnimTranslationDuration = 1f;
    [SerializeField] float maxRotationY;
    [SerializeField] float extraDistanceCloserToCamera;

    #region ItemRotationController
    [Tooltip("Rotation Sensitivity")]
    public float mouseSensitivity = 1f;

    public UnityEvent onAnimStart, onAnimEnd;

    private float rotationX = 0f;
    private float rotationY = 0f;

    private Camera attachedCamera;
    private Vector3 axis;
    private Vector3 axisLastFrame;
    private Vector3 axisDelta;
    #endregion

    // This is called when this component is added to a GameObject to save time for devs
    void Reset()
    {
        if (!TryGetComponent<Collider>(out var col))
        {
            var newCollider = gameObject.AddComponent<BoxCollider>();
            newCollider.isTrigger = true;
        }
    }

    void Start()
    {
        attachedCamera = Camera.main;
        originPosition = transform.position;
        originRotation = transform.rotation;
        if (clotheMeshRenderer == null) clotheMeshRenderer = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        if (isPreviewModeActive && !isAnimating)
        {
            shouldRotate = !DesertFreeFlightController.IsMouseOverUI() && DesertFreeFlightController.Instance.canRotate;

            // UI Visibility
            if (Input.GetMouseButtonDown(0))
            {
                CanvasPurchaseItem.Instance.SetIsVisible(!shouldRotate);
            }

            if (Input.GetMouseButtonUp(0) && Input.touchCount == 0)
            {
                CanvasPurchaseItem.Instance.SetIsVisible(true);
            }

            // Rotates Previewed Item at Mouse Drag
            if (!Application.isMobilePlatform && shouldRotate) RotatePreview();
            else if (shouldRotate && Input.touchCount == 1) RotatePreview();
        }
    }

    void RotatePreview()
    {
        if (Input.GetMouseButtonDown(0) || Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            axisLastFrame = attachedCamera.ScreenToViewportPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(0))
        {
            axis = attachedCamera.ScreenToViewportPoint(Input.mousePosition);
            axisDelta = (axisLastFrame - axis) * 90f;
            axisLastFrame = axis;

            rotationX += axisDelta.x * mouseSensitivity;
            if (maxRotationY != 0f)
                rotationY = Mathf.Clamp(rotationY + axisDelta.y * mouseSensitivity, -maxRotationY, maxRotationY);
            else
                rotationY += axisDelta.y * mouseSensitivity;

            Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
            Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, Vector3.left);

            transform.rotation = originRotation * xQuaternion * yQuaternion;
        }
    }

    public void Preview()
    {
        if (!isPreviewModeActive && !isAnimating)
        {
            DesertFreeFlightController.Instance.SetIsBusy(true);
            var playerPos = DesertFreeFlightController.Instance.transform.position;
            var distance = Vector3.Distance(playerPos, transform.position);
            var targetPreviewPos = DesertFreeFlightController.previewTargetTransform.position;
            targetPreviewPos += (DesertFreeFlightController.Instance.transform.position - DesertFreeFlightController.previewTargetTransform.position).normalized * extraDistanceCloserToCamera;
            if (distance < minDistanceForPlayerCamera - minDistanceForPlayerCamera * 0.1f)
            {
                var dir = (playerPos - transform.position).normalized;
                var targetPos = transform.position + dir * minDistanceForPlayerCamera;
                DesertFreeFlightController.Instance.SetTargetPosition(new Vector3(
                    targetPos.x,
                    playerPos.y,
                    targetPos.z
                ));
                var vectorDif = DesertFreeFlightController.Instance.targetPosition - playerPos;
                targetPreviewPos += vectorDif;
            }
            _PreviewSelected(targetPreviewPos);
            DesertFreeFlightController.Instance.canvasPurchaseItem.ShowItem(this);
            activeExpositionItem = this;
            shouldRotate = false;
            rotationX = transform.rotation.eulerAngles.x;
            rotationY = transform.rotation.eulerAngles.y;
            onAnimStart.Invoke();
        }
    }

    void _PreviewSelected(Vector3 targetPos)
    {
        isPreviewModeActive = true;
        // var targetPos = DesertFreeFlightController.previewTargetTransform.position;
        // targetPos += (DesertFreeFlightController.Instance.transform.position - DesertFreeFlightController.previewTargetTransform.position).normalized * extraDistanceCloserToCamera;
        transform.DOMove(targetPos, previewAnimTranslationDuration, false);

        ResetMouseRotationValues(); // This avoid caching rotation values while animating
    }

    public void EscapePreview()
    {
        activeExpositionItem._EscapePreview().Forget();
    }

    public async UniTaskVoid _EscapePreview()
    {
        if (!isPreviewModeActive) return;
        isPreviewModeActive = false;
        transform.DOMove(originPosition, previewAnimTranslationDuration, false);
        transform.DORotate(originRotation.eulerAngles, previewAnimTranslationDuration, RotateMode.Fast);
        DesertFreeFlightController.Instance.SetIsBusy(false);
        DesertFreeFlightController.Instance.canvasPurchaseItem.Back();
        await UniTask.Delay((int)(previewAnimTranslationDuration * 1000 * 0.8f));
        if (!isPreviewModeActive) onAnimEnd.Invoke();
    }

    void ResetMouseRotationValues()
    {
        axisLastFrame = attachedCamera.ScreenToViewportPoint(Input.mousePosition);
        axis = Vector3.zero;
        axisDelta = Vector3.zero;
    }

    public bool IsBusy()
    {
        return isPreviewModeActive;
    }

    public void OnClick()
    {
        Preview();
    }
}
