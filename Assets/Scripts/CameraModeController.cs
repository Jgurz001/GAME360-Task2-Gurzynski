using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class CameraModeController : MonoBehaviour
{
    // Header makes a label in inspector (didnt even realize this)
    [Header("Cameras")]
    //
    [SerializeField] private CinemachineCamera thirdPersonCamera;
    [SerializeField] private CinemachineCamera firstPersonCamera;

    [Header("Player Visuals")]
    // Will allow for disappearing body parts when in first person
    [SerializeField] private Renderer[] playerRenderers;
    // false means we are now in third person instead
    private bool isFirstPerson = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetCameraMode(false);


    }

    // Update is called once per frame
    void Update()
    {
        // Check game paused
        if (Time.timeScale == 0f) return;
        // Prevent camera switching while menu or pause screen open
        if (Keyboard.current == null) return;
        // Check input system
        if (Keyboard.current.vKey.wasPressedThisFrame) { ToggleCamera(); }
    }

    public void ToggleCamera() 
    {
        SetCameraMode(!isFirstPerson);
    }

    private void SetCameraMode(bool useFirstPerson) 
    {
        if (thirdPersonCamera == null || firstPersonCamera == null) 
        {
            Debug.LogError("CameraMOdeController is missing a camera juust not sure which one ya know figure it out");
            return;
        }
        // Store requested camera mode in class variable
        isFirstPerson = useFirstPerson;

        // Give active camera higher priority
        // Lower third person priority when first person is active
        thirdPersonCamera.Priority = isFirstPerson ? 10 : 20;
        // Rasie first person priority when first person is active
        firstPersonCamera.Priority = isFirstPerson? 20 : 10;

        // Hide player model while using first person
        foreach (Renderer playerRenderer in playerRenderers) 
        {
            if (playerRenderer != null) { playerRenderer.enabled = !isFirstPerson; }
        }

    }

}

