using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCamerController : MonoBehaviour
{
    //Speed of zoom
    [SerializeField] private float zoomSpeed = 2f;

    //This variable is for the smoothing as we move closer or farther away from player
    [SerializeField] private float zoomLerpSpeed = 10f;

    //min and max distance you can get to player
    [SerializeField] private float minDistance = 3f;
    [SerializeField] private float maxDistance = 15f;


    private PlayerControls controls;

    private CinemachineCamera cam;
    private CinemachineOrbitalFollow orbital;
    private Vector2 scrollDelta;

    //important for zooming smoothly to and from player
    private float targetZoom;
    private float currentZoom;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controls = new PlayerControls();
        controls.Enable();

        //Anytime scroll wheel is pressed we call the handle mouse scroll method
        controls.CameraControls.MouseZoom.performed += HandleMouseScroll;

        //Locks the cursor from the game, hides it essentially
        Cursor.lockState = CursorLockMode.Locked;

        //Reference to our camera
        cam = GetComponent<CinemachineCamera>();
        // Reference to our 
        orbital = cam.GetComponent<CinemachineOrbitalFollow>();
        //initialize the target zoom 
        targetZoom = currentZoom = orbital.Radius;
    }

    //HOLD CONTROL PERIOD TO GIVE A LIST ON GENERATING METHODS NEEDED
    private void HandleMouseScroll(InputAction.CallbackContext context)
    {
        scrollDelta = context.ReadValue<Vector2>();
        Debug.Log($"Mouse is scrolling. ");
    }

    // Update is called once per frame
    void Update()
    {
        if (scrollDelta.y !=0) 
        {
            if (orbital != null) 
            {
                targetZoom = Mathf.Clamp(orbital.Radius - scrollDelta.y * zoomSpeed, minDistance, maxDistance);
                scrollDelta = Vector2.zero;
            }
        
        }
        currentZoom = Mathf.Lerp(currentZoom, targetZoom, Time.deltaTime * zoomLerpSpeed);
        orbital.Radius = currentZoom;
    }
}
