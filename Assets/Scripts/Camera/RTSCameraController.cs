using UnityEngine;

/// <summary>
/// Moves the RTS camera based on input and settings.
/// Pan moves RTSCameraRig on the X/Z plane. Zoom moves the camera child up and down.
/// Rotation turns RTSCameraRig on the Y axis.
/// Attach this to RTSCameraRig alongside RTSCameraInput.
/// </summary>
public class RTSCameraController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Drag Assets/ScriptableObjects/Camera/RTSCameraSettings here.")]
    [SerializeField] private RTSCameraSettings settings;

    [Tooltip("Reads Pan, Zoom, and Rotate from the Input Actions asset.")]
    [SerializeField] private RTSCameraInput cameraInput;

    [Tooltip("Drag the Main Camera child object here.")]
    [SerializeField] private Transform cameraTransform;

    private void Awake()
    {
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
        ApplyRotation();
    }

    private void ApplyPan()
    {
        Vector2 keyboardPan = cameraInput.PanInput;
        if (keyboardPan.sqrMagnitude < 0.001f)
        {
            return;
        }

        Vector3 movement = GetWorldPanDirection(keyboardPan) * settings.keyboardPanSpeed;

        Vector3 newPosition = transform.position + movement * Time.deltaTime;
        newPosition.y = transform.position.y;
        transform.position = newPosition;
    }

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

    private void ApplyZoom()
    {
        float zoomInput = cameraInput.ZoomInput;
        if (Mathf.Abs(zoomInput) < 0.001f)
        {
            return;
        }

        Vector3 localPosition = cameraTransform.localPosition;
        localPosition.y -= zoomInput * settings.zoomSpeed;
        localPosition.y = Mathf.Clamp(
            localPosition.y,
            settings.minCameraHeight,
            settings.maxCameraHeight);

        cameraTransform.localPosition = localPosition;
        PreventCameraClippingThroughTerrain();
    }

    private void PreventCameraClippingThroughTerrain()
    {
        float minimumWorldHeight = GetMinimumAllowedCameraWorldHeight();
        if (cameraTransform.position.y >= minimumWorldHeight)
        {
            return;
        }

        Vector3 localPosition = cameraTransform.localPosition;
        localPosition.y += minimumWorldHeight - cameraTransform.position.y;
        localPosition.y = Mathf.Clamp(
            localPosition.y,
            settings.minCameraHeight,
            settings.maxCameraHeight);

        cameraTransform.localPosition = localPosition;
    }

    private float GetMinimumAllowedCameraWorldHeight()
    {
        float rigGroundHeight = SampleTerrainHeight(transform.position.x, transform.position.z);
        float cameraGroundHeight = SampleTerrainHeight(cameraTransform.position.x, cameraTransform.position.z);
        float groundHeight = Mathf.Max(rigGroundHeight, cameraGroundHeight);

        return groundHeight + settings.minClearanceAboveGround;
    }

    private static float SampleTerrainHeight(float worldX, float worldZ)
    {
        Terrain[] terrains = Terrain.activeTerrains;
        if (terrains == null || terrains.Length == 0)
        {
            return 0f;
        }

        Vector3 samplePosition = new Vector3(worldX, 0f, worldZ);
        float highestPoint = float.MinValue;
        bool foundTerrain = false;

        for (int i = 0; i < terrains.Length; i++)
        {
            Terrain terrain = terrains[i];
            if (terrain == null)
            {
                continue;
            }

            Vector3 terrainPosition = terrain.transform.position;
            Vector3 terrainSize = terrain.terrainData.size;

            if (samplePosition.x < terrainPosition.x ||
                samplePosition.x > terrainPosition.x + terrainSize.x ||
                samplePosition.z < terrainPosition.z ||
                samplePosition.z > terrainPosition.z + terrainSize.z)
            {
                continue;
            }

            highestPoint = Mathf.Max(highestPoint, terrain.SampleHeight(samplePosition));
            foundTerrain = true;
        }

        return foundTerrain ? highestPoint : 0f;
    }

    private void ApplyRotation()
    {
        float rotateInput = cameraInput.RotateInput;
        if (Mathf.Abs(rotateInput) < 0.001f)
        {
            return;
        }

        float rotationAmount = rotateInput * settings.rotationSpeed * Time.deltaTime;
        transform.Rotate(0f, rotationAmount, 0f);
    }
}
