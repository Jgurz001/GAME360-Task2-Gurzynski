using UnityEngine;



// PLEASE REFERENCE https://www.youtube.com/watch?v=TYzZsBl3OI0&t=61s
public class Grappling : MonoBehaviour
{
    [Header("References")]
    private PlayerController pm;
    public Transform cam;
    public Transform gunTip;
    public LayerMask whatIsGrappleable;
    public LineRenderer lr;

    [Header("Grappling")]
    public float maxGrappleDistance;
    public float grappleDelayTime;
    public float overshootYAxis;

    private Vector3 grapplePoint;

    [Header("Cooldown")]
    public float grapplingCd;
    private float grapplingCdTimer;

    [Header("Input")]
    public KeyCode grapplingKey = KeyCode.Mouse1;

    private bool grappling;





    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pm = GetComponent<PlayerController>();
    }

    private void StartGrapple()
    {
        if (grapplingCdTimer > 0) return;
        grappling = true;

        

        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappleable))
        {
            grapplePoint = hit.point;

            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
        }
        else
        {
            grapplePoint = cam.position + cam.forward * maxGrappleDistance;

            Invoke(nameof(StopGrappling), grappleDelayTime);
        }

        lr.enabled = true;
        lr.SetPosition(1, grapplePoint);

    }

    // Just pick up here tomorrow. Started building the grapple. Not sure what all of this is tbh. 
    // Still confused why the grapple does NOT disappear
    //  Watch video starting at 6:30
    private void ExecuteGrapple()
    {
       

        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;

        if (grapplePointRelativeYPos < 0) highestPointOnArc = overshootYAxis;

        pm.JumpToPosition(grapplePoint, highestPointOnArc);

        Invoke(nameof(StopGrappling), 1f);
    }

    public void StopGrappling()
    {
        
        pm.activeGrapple = false;

        grappling = false;

        grapplingCdTimer = grapplingCd;

        lr.enabled = false;
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(grapplingKey)) StartGrapple();

        if (grapplingCdTimer > 0)
            grapplingCdTimer -= Time.deltaTime; 
    }

    private void LateUpdate()
    {
        if (grappling)
            lr.SetPosition(0, gunTip.position);
    }
}
