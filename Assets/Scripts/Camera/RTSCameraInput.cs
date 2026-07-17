using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Reads player input for the RTS camera and exposes Pan, Zoom, and Rotate values.
/// Does not move the camera — RTSCameraController uses these values to move the rig.
/// Attach this script to RTSCameraRig.
/// </summary>
public class RTSCameraInput : MonoBehaviour
{
    [Header("Input Asset")]
    [Tooltip("Drag Assets/Input/RTSCameraInputActions into this slot.")]
    [SerializeField] private InputActionAsset inputActions;

    private InputAction panAction;
    private InputAction zoomAction;
    private InputAction rotateAction;

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

    /// <summary>
    /// Rotation input from Q and E.
    /// -1 = rotate left (Q), +1 = rotate right (E), 0 = none.
    /// </summary>
    public float RotateInput { get; private set; }

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
        rotateAction = cameraMap.FindAction("Rotate", true);
    }

    private void OnEnable()
    {
        // Enable input while this GameObject is active.
        panAction?.Enable();
        zoomAction?.Enable();
        rotateAction?.Enable();
    }

    private void OnDisable()
    {
        panAction?.Disable();
        zoomAction?.Disable();
        rotateAction?.Disable();
    }

    private void Update()
    {
        if (panAction == null || zoomAction == null || rotateAction == null)
        {
            return;
        }

        PanInput = panAction.ReadValue<Vector2>();
        ZoomInput = zoomAction.ReadValue<float>();
        RotateInput = rotateAction.ReadValue<float>();
    }
}
