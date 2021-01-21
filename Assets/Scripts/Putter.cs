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

    private float putterDistanceFromBall = 0.5f;
    private float swingPowerPercent = 0f;
    private float fullSwingForce = 10f;
    private float maxSwingAngle = 45f;

    private bool draggingPutter = false;

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
            case GameManager.ePuttingState.PlacingPutter:
                DetectPutterPlacement();

                if (Input.GetMouseButtonDown(0) && putterMesh.activeSelf)
                    GameManager.CurPuttingState = GameManager.ePuttingState.PutterPlaced;
                break;

            case GameManager.ePuttingState.PutterPlaced:
                // Click and drag
                if (!draggingPutter && Input.GetMouseButtonDown(0))
                {
                    // Detect if the putter has been clicked
                    var ray = GameManager.TheCameraThatIsSupposedToFollowTheBall.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out var hitInfo, 10, LayerMask.GetMask("Putter")))
                    {
                        // TODO
                        draggingPutter = true;
                    }
                }
                else if (draggingPutter && !Input.GetMouseButton(0))
                {
                    // TODO
                    draggingPutter = false;
                }

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    // Initial left click - go to swinging state and start swinging animation
                    GameManager.CurPuttingState = GameManager.ePuttingState.Swinging;
                    animator.SetBool(startingSwingName, true);
                    arrowMesh.SetActive(false);
                }
                break;

            case GameManager.ePuttingState.Swinging:
                swung = animator.GetBool(swungName);
                if (swung)
                    break;

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    // Cancel the swing
                    animator.SetBool(startingSwingName, false);
                    arrowMesh.SetActive(true);
                    GameManager.CurPuttingState = GameManager.ePuttingState.PutterPlaced;
                }

                if (Input.GetKeyUp(KeyCode.Space))
                {
                    // Commit the swing - calculate the force of the swing by percentage of max angle
                    animator.SetBool(swungName, true);
                    float swingAngle = Mathf.Abs(GetWrappedAngle(transform.localEulerAngles.z));
                    swingPowerPercent = Mathf.Clamp(swingAngle / maxSwingAngle, 0.1f, 1);
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

    private void DetectPutterPlacement()
    {
        // Figure out the angle from the ball to the cursor
        var ray = GameManager.TheCameraThatIsSupposedToFollowTheBall.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray.origin, ray.direction, out var hitInfo, 10, LayerMask.GetMask("PutterPlane"))
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

            putterBaseTransform.position = GameManager.Ball.transform.position + (newPointOffset * putterDistanceFromBall);

            // Rotate putter to face ball
            putterBaseTransform.rotation = Quaternion.Euler(new Vector3(putterBaseTransform.eulerAngles.x, -putterAngle + (rightHanded ? 180 : 0), putterBaseTransform.eulerAngles.z));
        }
        else if (putterMesh.gameObject.activeSelf)
        {
            TogglePutterAndArrow(false);
        }
    }

    private void ClickAndDrag()
    {
        //// Get angle from cursor to ball
        //float hitDeltaX = hitInfo.point.z - GameManager.Ball.transform.position.z;
        //float hitDeltaY = hitInfo.point.x - GameManager.Ball.transform.position.x;
        //float putterAngle = Mathf.Atan2(hitDeltaX, hitDeltaY) * 180 / Mathf.PI; // Degrees

        //// Calclate base point offset from ball based on found angle
        //float putterAngleRads = putterAngle * Mathf.Deg2Rad;
        //var newPointOffset = new Vector3(Mathf.Cos(putterAngleRads), 0, Mathf.Sin(putterAngleRads));

        //putterBaseTransform.position = GameManager.Ball.transform.position + (newPointOffset * putterDistanceFromBall);

        //// Rotate putter to face ball
        //putterBaseTransform.rotation = Quaternion.Euler(new Vector3(putterBaseTransform.eulerAngles.x, -putterAngle + (rightHanded ? 180 : 0), putterBaseTransform.eulerAngles.z));
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