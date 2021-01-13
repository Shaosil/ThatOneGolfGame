using UnityEngine;

public class Putter : MonoBehaviour
{
    // TODO: OPTIONS
    private bool rightHanded = true;

    private Rigidbody ballRb;
    private Transform putterBaseTransform;
    private GameObject putterMesh;

    private void Start()
    {
        ballRb = GameObject.Find("Ball").GetComponent<Rigidbody>();
        putterBaseTransform = transform.parent;
        putterMesh = transform.GetChild(0).gameObject;
    }

    private void Update()
    {
        if (GameManager.LiningUpShot)
            UpdatePutterPosition();

        if (Input.GetMouseButtonDown(0) && GameManager.LiningUpShot)
        {
            // Prepare the putter
            GameManager.LiningUpShot = false;
            GameManager.UI.LineUpShotButton.interactable = true;
            GameManager.UI.StartSwingButton.interactable = true;
        }
        else if (Input.GetMouseButton(0) && GameManager.Swinging)
        {
            // Hold down to adjust power - TODO
        }
        
        if (Input.GetMouseButtonDown(1) && GameManager.Swinging)
        {
            // Cancel the swing - TODO
        }
    }

    private void UpdatePutterPosition()
    {
        float putterDistanceAway = 0.5f;

        // Figure out the angle from the ball to the cursor
        var ray = GameManager.TheCameraThatIsSupposedToFollowTheBall.ScreenPointToRay(Input.mousePosition);
        var putterLayer = 1 << LayerMask.NameToLayer("PutterCollider");
        if (Physics.Raycast(ray.origin, ray.direction, out var hitInfo, 10, putterLayer)
            && Vector3.Distance(GameManager.Ball.transform.position, hitInfo.point) > 0.25f)
        {
            // Make it visible if it isn't already
            if (!putterMesh.gameObject.activeSelf)
                putterMesh.gameObject.SetActive(true);

            // Get angle from cursor to ball
            float hitDeltaX = hitInfo.point.z - GameManager.Ball.transform.position.z;
            float hitDeltaY = hitInfo.point.x - GameManager.Ball.transform.position.x;
            float putterAngle = Mathf.Atan2(hitDeltaX, hitDeltaY) * 180 / Mathf.PI; // Degrees

            // Calclate base point offset from ball based on found angle
            float putterAngleRads = putterAngle * Mathf.Deg2Rad;
            var newPointOffset = new Vector3(Mathf.Cos(putterAngleRads), 0, Mathf.Sin(putterAngleRads));

            putterBaseTransform.position = GameManager.Ball.transform.position + (newPointOffset * putterDistanceAway);

            // Rotate putter to face ball
            putterBaseTransform.rotation = Quaternion.Euler(new Vector3(putterBaseTransform.eulerAngles.x, -putterAngle + (rightHanded ? 180 : 0), putterBaseTransform.eulerAngles.z));
        }
        else if (putterMesh.gameObject.activeSelf)
        {
            putterMesh.gameObject.SetActive(false);
        }
    }

    private void TestAnimationCall(string s)
    {
        var normalizedVector = (ballRb.transform.position - transform.parent.position).normalized;
        ballRb.AddForce(normalizedVector * 7.5f, ForceMode.Impulse);
    }
}