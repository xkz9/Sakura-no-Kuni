using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Moves the RTS camera based on input and settings.
/// Pan moves RTSCameraRig on the X/Z plane. Zoom moves the camera child up and down.
/// Attach this to RTSCameraRig alongside RTSCameraInput.
/// </summary>
public class RTSCameraController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Drag Assets/ScriptableObjects/Camera/RTSCameraSettings here.")]
    [SerializeField] private RTSCameraSettings settings;

    [Tooltip("Reads Pan and Zoom from the Input Actions asset.")]
    [SerializeField] private RTSCameraInput cameraInput;

    [Tooltip("Drag the Main Camera child object here.")]
    [SerializeField] private Transform cameraTransform;

    private void Awake()
    {
        // Auto-find RTSCameraInput on the same GameObject if not assigned.
        if (cameraInput == null)
        {
            cameraInput = GetComponent<RTSCameraInput>();
        }

        if (settings == null)
        {
            Debug.LogError("RTSCameraController: Assign RTSCameraSettings in the Inspector.", this);
        }

        if (cameraInput == null)
        {
            Debug.LogError("RTSCameraController: Assign RTSCameraInput in the Inspector.", this);
        }

        if (cameraTransform == null)
        {
            Debug.LogError("RTSCameraController: Assign the Main Camera transform in the Inspector.", this);
        }
    }

    private void Update()
    {
        if (settings == null || cameraInput == null || cameraTransform == null)
        {
            return;
        }

        ApplyPan();
        ApplyZoom();
    }

    /// <summary>
    /// Moves the rig horizontally using keyboard/arrows and optional edge scrolling.
    /// </summary>
    private void ApplyPan()
    {
        Vector3 movement = Vector3.zero;

        // Keyboard and arrow keys (from RTSCameraInput).
        Vector2 keyboardPan = cameraInput.PanInput;
        if (keyboardPan.sqrMagnitude > 0.001f)
        {
            movement += GetWorldPanDirection(keyboardPan) * settings.keyboardPanSpeed;
        }

        // Edge scrolling uses mouse screen position (not in the Input Actions asset).
        if (settings.edgeScrollEnabled)
        {
            Vector2 edgePan = GetEdgeScrollInput();
            if (edgePan.sqrMagnitude > 0.001f)
            {
                movement += GetWorldPanDirection(edgePan) * settings.edgeScrollSpeed;
            }
        }

        if (movement.sqrMagnitude < 0.001f)
        {
            return;
        }

        // Move on the ground plane only (X and Z).
        Vector3 newPosition = transform.position + movement * Time.deltaTime;
        newPosition.y = transform.position.y;
        transform.position = newPosition;
    }

    /// <summary>
    /// Converts 2D input (x = strafe, y = forward) into a world direction
    /// based on which way the rig is currently facing.
    /// </summary>
    private Vector3 GetWorldPanDirection(Vector2 input)
    {
        Vector3 forward = transform.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 right = transform.right;
        right.y = 0f;
        right.Normalize();

        return (right * input.x + forward * input.y).normalized;
    }

    /// <summary>
    /// Returns pan direction when the mouse is near the screen edge.
    /// </summary>
    private Vector2 GetEdgeScrollInput()
    {
        if (Mouse.current == null)
        {
            return Vector2.zero;
        }

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        float margin = settings.edgeScrollMargin;
        Vector2 edgePan = Vector2.zero;

        if (mousePosition.x < margin)
        {
            edgePan.x = -1f;
        }
        else if (mousePosition.x > Screen.width - margin)
        {
            edgePan.x = 1f;
        }

        if (mousePosition.y < margin)
        {
            edgePan.y = -1f;
        }
        else if (mousePosition.y > Screen.height - margin)
        {
            edgePan.y = 1f;
        }

        return edgePan.normalized;
    }

    /// <summary>
    /// Zooms by changing the camera child's local Y position (height).
    /// </summary>
    private void ApplyZoom()
    {
        float zoomInput = cameraInput.ZoomInput;
        if (Mathf.Abs(zoomInput) < 0.001f)
        {
            return;
        }

        Vector3 localPosition = cameraTransform.localPosition;

        // Scroll values are already per-frame, so we do not multiply by Time.deltaTime.
        localPosition.y -= zoomInput * settings.zoomSpeed;
        localPosition.y = Mathf.Clamp(
            localPosition.y,
            settings.minCameraHeight,
            settings.maxCameraHeight);

        cameraTransform.localPosition = localPosition;
    }
}
