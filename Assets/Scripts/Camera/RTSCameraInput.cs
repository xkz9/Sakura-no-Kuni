using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Reads player input for the RTS camera and exposes Pan and Zoom values.
/// Does not move the camera — RTSCameraController will use these values in Step 5.
/// Attach this script to RTSCameraRig.
/// </summary>
public class RTSCameraInput : MonoBehaviour
{
    [Header("Input Asset")]
    [Tooltip("Drag Assets/Input/RTSCameraInputActions into this slot.")]
    [SerializeField] private InputActionAsset inputActions;

    private InputAction panAction;
    private InputAction zoomAction;

    /// <summary>
    /// Pan direction from WASD and arrow keys.
    /// X = left (-1) / right (+1), Y = down (-1) / up (+1).
    /// </summary>
    public Vector2 PanInput { get; private set; }

    /// <summary>
    /// Scroll wheel value this frame.
    /// Positive = scroll up, negative = scroll down.
    /// </summary>
    public float ZoomInput { get; private set; }

    private void Awake()
    {
        if (inputActions == null)
        {
            Debug.LogError("RTSCameraInput: Assign RTSCameraInputActions in the Inspector.", this);
            return;
        }

        // Find the action map and actions defined in RTSCameraInputActions.
        InputActionMap cameraMap = inputActions.FindActionMap("RTSCamera", true);
        panAction = cameraMap.FindAction("Pan", true);
        zoomAction = cameraMap.FindAction("Zoom", true);
    }

    private void OnEnable()
    {
        // Enable input while this GameObject is active.
        panAction?.Enable();
        zoomAction?.Enable();
    }

    private void OnDisable()
    {
        panAction?.Disable();
        zoomAction?.Disable();
    }

    private void Update()
    {
        if (panAction == null || zoomAction == null)
        {
            return;
        }

        PanInput = panAction.ReadValue<Vector2>();
        ZoomInput = zoomAction.ReadValue<float>();
    }
}
