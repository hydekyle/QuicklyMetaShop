using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityObservables;
using System.Linq;
using HSVPicker;

public class ColorSwapper : MonoBehaviour
{
    Observable<bool> isEnabled = new() { Value = false };
    Observable<bool> isEditing = new() { Value = false };
    [SerializeField] LayerMask clickRayLayerMask;
    [SerializeField] ColorPicker colorPicker;
    PreviousState lastSelected = new();

    void Start()
    {
        isEnabled.OnChanged += () =>
        {
            DesertFreeFlightController.Instance.isBusy = isEnabled.Value;
            if (isEnabled.Value == false)
            {
                colorPicker.Hide();
            }
            ResetLastRemarkedYellow();
            lastSelected = new();
            isEditing.Value = false;
        };

        isEditing.OnChanged += () =>
        {
            if (isEditing.Value == true)
            {
                StartEditing();
            }
            else
            {
                colorPicker.Hide();
                lastSelected = new();
            }

        };
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1)) isEnabled.Value = !isEnabled.Value;

        if (isEnabled.Value && !isEditing.Value && Input.GetMouseButtonDown(0)) isEditing.Value = true;
        if (isEnabled.Value && !isEditing.Value) RemarkYellowAtMousePosition();

        if (isEditing.Value && Input.GetMouseButtonDown(1))
        {
            isEditing.Value = false;
            isEnabled.Value = false;
        }
    }

    void StartEditing()
    {
        var renderer = GetRendererAtMousePosition();
        if (renderer == null) return;
        colorPicker.onValueChanged.RemoveAllListeners();
        colorPicker.onValueChanged.AddListener((color) =>
        {
            renderer.material.color = color;
        });
        colorPicker.Show(lastSelected.color != null ? lastSelected.color : renderer.material.color);
        isEditing.Value = true;
        ResetLastRemarkedYellow();
        lastSelected = new();
    }

    void RemarkYellowAtMousePosition()
    {
        var renderer = GetRendererAtMousePosition();
        if (renderer != null)
        {
            if (lastSelected.renderer == renderer) return;

            ResetLastRemarkedYellow();

            lastSelected.renderer = renderer;
            lastSelected.color = renderer.material.color;

            renderer.material.color = Color.yellow;
        }
    }

    MeshRenderer GetRendererAtMousePosition()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var hits = Physics.RaycastAll(ray, Mathf.Infinity, clickRayLayerMask);
        hits = hits.OrderBy(p => p.distance).ToArray();
        if (hits.Length == 0) return null;
        var hit = hits[0];
        if (hit.transform.TryGetComponent<MeshRenderer>(out var renderer))
        {
            return renderer;
        }
        else return null;
    }

    void ResetLastRemarkedYellow()
    {
        if (lastSelected.renderer != null)
        {
            lastSelected.renderer.material.color = lastSelected.color;
        }
    }

    [System.Serializable]
    struct PreviousState
    {
        public Color color;
        public MeshRenderer renderer;
    }
}
