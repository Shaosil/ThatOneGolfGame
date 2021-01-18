using UnityEngine;

public class Putter : MonoBehaviour
{
    // TODO: OPTIONS
    private bool rightHanded = true;
    private string startingSwingName = "startingSwingR";
    private string swungName = "swungR";

    private Rigidbody ballRb;
    private Transform putterBaseTransform;
    private GameObject putterMesh;
    private GameObject arrowMesh;
    private Animator animator;

    private float swingPowerPercent = 0f;
    private float fullSwingForce = 10f;
    private float maxSwingAngle = 45f;

    private void Start()
    {
        ballRb = GameObject.Find("Ball").GetComponent<Rigidbody>();
        putterBaseTransform = transform.parent;
        putterMesh = transform.GetChild(0).gameObject;
        arrowMesh = putterBaseTransform.Find("Arrow").gameObject;
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        bool swung = false;

        switch (GameManager.CurPuttingState)
        {
            case GameManager.ePuttingState.LiningUpShot:
                UpdatePutterPosition();

                if (Input.GetMouseButtonDown(0) && putterMesh.activeSelf)
                {
                    // Prepare the putter
                    GameManager.CurPuttingState = GameManager.ePuttingState.LinedUp;
                }
                break;

            case GameManager.ePuttingState.SwingPrep:
                if (Input.GetMouseButtonDown(0))
                {
                    // Initial left click - go to swinging state and start swinging animation
                    GameManager.CurPuttingState = GameManager.ePuttingState.Swinging;
                    animator.SetBool(startingSwingName, true);
                    arrowMesh.SetActive(false);
                }
                else if (Input.GetKeyDown(KeyCode.Escape))
                {
                    // Cancel and go back to a lined up state
                    GameManager.CurPuttingState = GameManager.ePuttingState.LinedUp;
                }

                break;

            case GameManager.ePuttingState.Swinging:
                swung = animator.GetBool(swungName);
                if (swung)
                    break;

                if (!Input.GetMouseButton(0))
                {
                    // Commit the swing - calculate the force of the swing by percentage of max angle
                    animator.SetBool(swungName, true);
                    swingPowerPercent = Mathf.Clamp(Mathf.Abs(GetWrappedAngle(transform.localEulerAngles.z)) / maxSwingAngle, 0.1f, 1);
                }

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    // Cancel the swing
                    animator.SetBool(startingSwingName, false);
                    arrowMesh.SetActive(true);
                    GameManager.CurPuttingState = GameManager.ePuttingState.SwingPrep;
                }
                break;
        }

        // Hit the ball once the putter reaches a certain angle
        var angle = GetWrappedAngle(transform.localEulerAngles.z);
        if (swung && ((rightHanded && angle > 6f) || (!rightHanded && angle < -6f)))
        {
            GameManager.CurPuttingState = GameManager.ePuttingState.NotPutting;
            animator.SetBool(swungName, false);
            animator.SetBool(startingSwingName, false);

            var normalizedVector = (ballRb.transform.position - transform.parent.position).normalized;
            ballRb.AddForce(normalizedVector * (fullSwingForce * swingPowerPercent), ForceMode.Impulse);
            swingPowerPercent = 0f;
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
                TogglePutterAndArrow(true);

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
            TogglePutterAndArrow(false);
        }
    }

    private void TogglePutterAndArrow(bool visible)
    {
        putterMesh.SetActive(visible);
        arrowMesh.SetActive(visible);
    }

    private float GetWrappedAngle(float angle)
    {
        // Constrain an angle to -180 to 180
        angle %= 360;
        return angle > 180 ? angle - 360 : angle;
    }
}