using UnityEngine;

public class Ball : MonoBehaviour
{
    // TODO: OPTIONS
    private bool rightHanded = true;

    private Camera theCameraThatIsSupposedToFollowTheBall;
    private float camZoomLevel;
    private Vector2 camRotationValues;

    private GameObject putterPlane;
    private GameObject putter;

    private Rigidbody rb;
    private Vector3 originPosition;

    // TEMP
    public float AbsHorizMagnitude;
    public float Speed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        theCameraThatIsSupposedToFollowTheBall = GameObject.Find("Main Camera").GetComponent<Camera>();
        camZoomLevel = (theCameraThatIsSupposedToFollowTheBall.transform.position - transform.position).magnitude;
        camRotationValues = new Vector2(theCameraThatIsSupposedToFollowTheBall.transform.eulerAngles.x, theCameraThatIsSupposedToFollowTheBall.transform.eulerAngles.y); ;
        putterPlane = GameObject.Find("PutterPlane");
        putter = GameObject.Find("PutterBase");
        rb = GetComponent<Rigidbody>();
        originPosition = transform.position;
    }

    // Update is called once per frame
    private void Update()
    {
        HandleControls();
        UpdateCamera();
        UpdatePutterPosition();

        AbsHorizMagnitude = Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.z);
    }

    private void HandleControls()
    {
        if (Input.GetKey(KeyCode.W))
            rb.AddForce(theCameraThatIsSupposedToFollowTheBall.transform.forward * Speed);
        else if (Input.GetKey(KeyCode.S))
            rb.AddForce(-theCameraThatIsSupposedToFollowTheBall.transform.forward * Speed);
        if (Input.GetKey(KeyCode.A))
            rb.AddForce(-theCameraThatIsSupposedToFollowTheBall.transform.right * Speed);
        else if (Input.GetKey(KeyCode.D))
            rb.AddForce(theCameraThatIsSupposedToFollowTheBall.transform.right * Speed);

        // Reset if spacebar pressed or we fall out of bounds
        if (Input.GetKeyDown(KeyCode.Space) || transform.position.y < -10)
        {
            transform.position = originPosition;
            rb.velocity = new Vector3();
            rb.angularVelocity = new Vector3();
        }

        // "Hit" the ball when left clicking
        var velocity = rb.velocity.sqrMagnitude;
        if (Input.GetMouseButtonDown(0) && velocity > -0.01f && velocity < 0.01f)
        {
            // Start the putter animation swing
            GameObject.Find("PutterPivot").GetComponent<Animation>().Play();
        }
    }

    private void UpdateCamera()
    {
        // Rotate cam
        if (Input.GetMouseButton(1))
        {
            var mouseMovement = new Vector2(-Input.GetAxis("Mouse Y") * 3f, Input.GetAxis("Mouse X") * 3f);
            camRotationValues += mouseMovement * Time.unscaledDeltaTime * 300f;
            camRotationValues = new Vector2(Mathf.Clamp(camRotationValues.x, -80f, 80f), camRotationValues.y);
        }

        // Zoom cam
        var zoomInput = -Input.GetAxis("Mouse ScrollWheel") * 5f;
        camZoomLevel = Mathf.Clamp(camZoomLevel + zoomInput, 1f, 10f);

        // Follow cam
        var curRotation = Quaternion.Euler(camRotationValues);
        var lookPosition = transform.position - ((curRotation * Vector3.forward) * camZoomLevel);
        theCameraThatIsSupposedToFollowTheBall.transform.SetPositionAndRotation(lookPosition, curRotation);
    }

    private void UpdatePutterPosition()
    {
        float putterDistanceAway = 0.5f;

        // Figure out the angle from the ball to the cursor
        var ray = theCameraThatIsSupposedToFollowTheBall.ScreenPointToRay(Input.mousePosition);
        var putterLayer = 1 << LayerMask.NameToLayer("PutterCollider");
        if (Physics.Raycast(ray.origin, ray.direction, out var hitInfo, 10, putterLayer)
            && Vector3.Distance(transform.position, hitInfo.point) > 0.25f)
        {
            // Get angle from cursor to ball
            float hitDeltaX = hitInfo.point.z - transform.position.z;
            float hitDeltaY = hitInfo.point.x - transform.position.x;
            float putterAngle = Mathf.Atan2(hitDeltaX, hitDeltaY) * 180 / Mathf.PI; // Degrees

            // Calclate base point offset from ball based on found angle
            float putterAngleRads = putterAngle * Mathf.Deg2Rad;
            var newPointOffset = new Vector3(Mathf.Cos(putterAngleRads), 0, Mathf.Sin(putterAngleRads));

            putter.transform.position = transform.position + (newPointOffset * putterDistanceAway);

            // Rotate putter to face ball
            putter.transform.rotation = Quaternion.Euler(new Vector3(putter.transform.eulerAngles.x, -putterAngle + (rightHanded ? 180 : 0), putter.transform.eulerAngles.z));
        }
    }
}