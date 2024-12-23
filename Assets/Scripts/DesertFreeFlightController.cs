using System.Threading;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityObservables;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.Events;
using TMPro;

public interface IClickable
{
    public void OnClick();
}

[RequireComponent(typeof(Camera))]
public class DesertFreeFlightController : MonoBehaviour
{
    public static DesertFreeFlightController Instance;
    public static Transform previewTargetTransform; // Where to put the item to preview after clicking it, usually in front of the camera
    /// <summary>Bake probes after Addressable GameObject instantiated</summary>
    public List<ReflectionProbe> reflectionProbeListToBakeAfterAddressables = new();

    public CanvasPurchaseItem canvasPurchaseItem;
    public CanvasArt previewArtLandscape;

    [Tooltip("Mouse sensitivity")]
    public float movementSpeed = 6f;

    [Tooltip("Mouse sensitivity")]
    public float mouseSensitivity = 1f;

    [SerializeField] float minZoom = 12f, maxZoom = 48f;
    public float zoomSpeed = 1f, zoomForceStep = 20f;
    float targetZoomValue;

    public bool isWalkingEnabled = true, isInteractionEnabled = true;
    public float dragVsClickTime = 0.1f;
    public float timeForClickBehavior = 0.6f;
    public LayerMask clickRayLayerMask;
    public bool isBusy;
    public bool startCinematic;
    public Vector3 targetPosition;

    Vector3 axis, axisLastFrame, axisDelta;
    float minimumX = -360f;
    float maximumX = 360f;
    float minimumY = -90f;
    float maximumY = 90f;
    float rotationX = 0f;
    float rotationY = 0f;

    public Quaternion originalRotation;
    Camera attachedCamera;
    Vector3 lastPositionClick;
    CancellationTokenSource sourceClickToMove = new();
    float lastTimeClick = -1f;

    public TMP_Text debugText;
    float lastDist, touchDist;
    public bool canRotate = true;

    void Awake()
    {
        Instance = this;
        previewTargetTransform = transform.GetChild(0);

#if !UNITY_EDITOR
        Debug.unityLogger.logEnabled = false;
#endif

    }

    void Start()
    {
        originalRotation = transform.localRotation;
        attachedCamera = GetComponent<Camera>();
        targetPosition = transform.position;
        targetZoomValue = minZoom;
        attachedCamera.focalLength = minZoom;
        // await AddressablesQueue.StartDownloadAndInstantiateQueue();
        // reflectionProbeListToBakeAfterAddressables.ForEach((probe) => probe.RenderProbe());
    }

    void Update()
    {
        attachedCamera.focalLength = Mathf.Lerp(attachedCamera.focalLength, targetZoomValue, Time.deltaTime * zoomSpeed); // Lerp zoom camera to target zoom
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * movementSpeed);

        if (Application.isMobilePlatform)
        {
            TouchControl();
        }
        else
        {
            MouseControl();
        }

    }

    public void SetIsBusy(bool isBusy)
    {
        _SetIsBusy(isBusy).Forget();
    }

    async UniTaskVoid _SetIsBusy(bool isBusy)
    {
        await UniTask.Yield();
        this.isBusy = isBusy;
        targetZoomValue = minZoom;
    }

    public void ZoomAddDelta(float deltaZoom)
    {
        targetZoomValue = Mathf.Clamp(targetZoomValue + deltaZoom, minZoom, maxZoom);
        // debugText.text = deltaZoom.ToString();
    }

    void TouchControl()
    {
        // Pitch gesture to zoom in/out
        if (Input.touchCount == 2)
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);
            canRotate = false;

            if (touch2.phase == TouchPhase.Began)
            {
                lastDist = Vector2.Distance(touch1.position, touch2.position);
                debugText.text = lastDist.ToString();
            }

            if (touch1.phase == TouchPhase.Moved && touch2.phase == TouchPhase.Moved)
            {
                float newDist = Vector2.Distance(touch1.position, touch2.position);
                touchDist = lastDist - newDist;
                lastDist = newDist;

                // Your Code Here
                ZoomAddDelta(-touchDist * 0.1f);
                axisLastFrame = Vector3.zero;
            }
        }
        else if (Input.touchCount == 0)
        {
            canRotate = true;
        }

        if (isBusy) return;

        // Save click first position data 
        if (Input.GetMouseButtonDown(0))
        {
            axisLastFrame = attachedCamera.ScreenToViewportPoint(Input.mousePosition);
            lastTimeClick = Time.time;
            lastPositionClick = Input.mousePosition;
        }

        // Rotate camera if mouse keep pressed
        if (Input.GetMouseButton(0) && canRotate)
        {
            axis = attachedCamera.ScreenToViewportPoint(Input.mousePosition);
            axisDelta = (axisLastFrame - axis) * 90f;
            axisLastFrame = axis;

            rotationX += axisDelta.x * mouseSensitivity;
            rotationY += axisDelta.y * mouseSensitivity;

            rotationX = ClampAngle(rotationX, minimumX, maximumX);
            rotationY = ClampAngle(rotationY, minimumY, maximumY);

            Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
            Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, Vector3.left);

            transform.localRotation = originalRotation * xQuaternion * yQuaternion;
        }

        // Look for interaction if click release with no camera rotation
        if (Input.GetMouseButtonUp(0))
        {
            if (timeForClickBehavior + lastTimeClick < Time.time) return;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (lastPositionClick == Input.mousePosition)
            {
                sourceClickToMove.Cancel();
                ClickInteraction(ray);
                return;
            }
            if (lastTimeClick + dragVsClickTime < Time.time) return;
            sourceClickToMove.Cancel();
            ClickInteraction(ray);
        }
    }

    void MouseControl()
    {
        if (Input.touchCount == 0) canRotate = true;

        // Move zoom by mouse wheel
        ZoomAddDelta(Input.mouseScrollDelta.y * zoomForceStep);

        if (isBusy) return;

        // Save click first position data 
        if (Input.GetMouseButtonDown(0))
        {
            axisLastFrame = attachedCamera.ScreenToViewportPoint(Input.mousePosition);
            lastTimeClick = Time.time;
            lastPositionClick = Input.mousePosition;
        }

        // Rotate camera if mouse keep pressed
        if (Input.GetMouseButton(0) && canRotate)
        {
            axis = attachedCamera.ScreenToViewportPoint(Input.mousePosition);
            axisDelta = (axisLastFrame - axis) * 90f;
            axisLastFrame = axis;

            rotationX += axisDelta.x * mouseSensitivity;
            rotationY += axisDelta.y * mouseSensitivity;

            rotationX = ClampAngle(rotationX, minimumX, maximumX);
            rotationY = ClampAngle(rotationY, minimumY, maximumY);

            Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
            Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, Vector3.left);

            transform.localRotation = originalRotation * xQuaternion * yQuaternion;
        }

        // Look for interaction if click release with no camera rotation
        if (Input.GetMouseButtonUp(0))
        {
            if (timeForClickBehavior + lastTimeClick < Time.time) return;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (lastPositionClick == Input.mousePosition)
            {
                sourceClickToMove.Cancel();
                ClickInteraction(ray);
                return;
            }
            if (lastTimeClick + dragVsClickTime < Time.time) return;
            sourceClickToMove.Cancel();
            ClickInteraction(ray);
        }
    }

    // Trigger Clickable Interface
    void ClickInteraction(Ray ray)
    {
        if (IsMouseOverUI() || isBusy) return;
        var hits = Physics.RaycastAll(ray, Mathf.Infinity, clickRayLayerMask);
        hits = hits.OrderBy(p => p.distance).ToArray();
        if (hits.Length == 0) return;
        var hit = hits[0];

        print(hit.transform.name);
        if (hit.transform != null)
        {
            if (isWalkingEnabled && hit.transform.CompareTag("Walkable"))
            {
                SetTargetPosition(new Vector3(hit.point.x, transform.position.y, hit.point.z));
            }
            else if (isInteractionEnabled && hit.transform.TryGetComponent<IClickable>(out var interactable))
            {
                SetTargetPosition(transform.position);
                interactable.OnClick();
            }
        }
    }

    public void SetTargetPosition(Vector3 targetPos)
    {
        targetPosition = targetPos;
    }

    //Gets all event system raycast results of current mouse or touch position.
    public static bool IsMouseOverUI()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults.Count > 0;
    }

    public void ShowArtInfo(DataArt artData)
    {
        SetIsBusy(true);
        previewArtLandscape.ShowArtInfo(artData);
        // if (Screen.width > Screen.height)
        // {
        // }
        // else
        // {
        //     previewArtPortrait.ShowArtInfo(artData);
        // }
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360f)
            angle += 360f;
        if (angle > 360f)
            angle -= 360f;
        return Mathf.Clamp(angle, min, max);
    }
}
