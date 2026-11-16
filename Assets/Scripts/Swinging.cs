using UnityEngine;

public class Swinging : MonoBehaviour
{
    [Header("Input")]
    public KeyCode swingKey = KeyCode.Mouse0;

    [Header("References")]
    public LineRenderer lr;
    public Transform gunTip, cam, player;
    public LayerMask whatIsGrappleable;
    private PlayerController pm;

    [Header("Swinging")]
    [SerializeField]private float maxSwingDistance = 25f;
    private Vector3 swingPoint;
    private SpringJoint joint;



    private Vector3 currentGrapplePosition;
    private void StartSwing() 
    {
        pm.swinging = true; 
        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxSwingDistance, whatIsGrappleable))
        {
            swingPoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = swingPoint;

            float distanceFromPoint = Vector3.Distance(player.position, swingPoint);

            // The distance grapple will try to keep from grapple point
            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;


            // Customize values as you like
            joint.spring = 4.5f;
            joint.damper = 7f;
            joint.massScale = 4.5f;

            //lr.positionCount = 2;
            currentGrapplePosition = gunTip.position;

        }
    }


    void StopSwing() 
    {

        pm.swinging = false;

        //lr.positionCount = 0;
        Destroy(joint);
    }

    void DrawRope() 
    {
        // if not grappling, don't draw rope
        if (!joint) return;

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, swingPoint, Time.deltaTime * 8f);

        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, currentGrapplePosition);
    }

    private void LateUpdate()
    {
        DrawRope();
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        pm = player.GetComponent<PlayerController>();

        if (pm == null)
        {
            Debug.LogError("Swinging could not find PlayerController!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(swingKey)) StartSwing();
        if (Input.GetKeyUp(swingKey)) StopSwing();
    }
}
