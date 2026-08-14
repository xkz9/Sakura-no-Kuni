using UnityEngine;

/// <summary>
/// Stores tunable values for the RTS camera (speeds, zoom limits, etc.).
/// Create one asset and assign it to RTSCameraController in a later step.
/// </summary>
[CreateAssetMenu(
    fileName = "RTSCameraSettings",
    menuName = "Japanese City Builder/Camera/RTS Camera Settings")]
public class RTSCameraSettings : ScriptableObject
{
    [Header("Pan - Keyboard")]
    [Tooltip("How fast the camera moves when using WASD or arrow keys.")]
    public float keyboardPanSpeed = 20f;

    [Header("Zoom")]
    [Tooltip("How fast the camera zooms when using the scroll wheel.")]
    public float zoomSpeed = 5f;

    [Tooltip("Lowest height (local Y) the camera can reach when there is no terrain.")]
    public float minCameraHeight = 8f;

    [Tooltip("Highest height (local Y) the camera can reach when zoomed out.")]
    public float maxCameraHeight = 200f;

    [Tooltip("How far above the ground the camera must stay when zooming in over hills.")]
    public float minClearanceAboveGround = 5f;

    [Header("Rotation")]
    [Tooltip("How fast the camera rotates when using Q and E (degrees per second).")]
    public float rotationSpeed = 105f;
}
