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
        ballRb = GameManager.Ball.GetComponent<Rigidbody>();
        putterBaseTransform = transform.parent;
        putterMesh = transform.GetChild(0).gameObject;
        arrowMesh = putterBaseTransform.Find("Arrow").gameObject;
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        bool swung = false;

        switch (GameManager.CurGameState)
        {
            case GameManager.eGameState.PlacingPutter:
                PlacePutter();

                if (Input.GetMouseButtonDown(0) && putterMesh.activeSelf)
                    GameManager.CurGameState = GameManager.eGameState.PutterPlaced;
                break;

            case GameManager.eGameState.PutterPlaced:
                // Click and drag
                if (!draggingPutter && Input.GetMouseButtonDown(0))
                {
                    // Detect if the putter has been clicked
                    if (Physics.Raycast(GameManager.CastCursor, out var hitInfo, 10, LayerMask.GetMask("Putter")))
                        draggingPutter = true;
                }
                else if (draggingPutter)
                {
                    if (!Input.GetMouseButton(0))
                        draggingPutter = false;
                    else
                        ClickAndDrag();
                }

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    // Initial left click - go to swinging state and start swinging animation
                    GameManager.CurGameState = GameManager.eGameState.Swinging;
                    animator.SetBool(startingSwingName, true);
                    arrowMesh.SetActive(false);
                }
                break;

            case GameManager.eGameState.Swinging:
                swung = animator.GetBool(swungName);
                if (swung)
                    break;

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    // Cancel the swing
                    animator.SetBool(startingSwingName, false);
                    arrowMesh.SetActive(true);
                    GameManager.CurGameState = GameManager.eGameState.PutterPlaced;
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
            animator.SetBool(swungName, false);
            animator.SetBool(startingSwingName, false);

            var normalizedVector = (ballRb.transform.position - transform.parent.position).normalized;
            ballRb.AddForce(normalizedVector * (fullSwingForce * swingPowerPercent), ForceMode.Impulse);
            swingPowerPercent = 0f;
        }
    }

    private void PlacePutter()
    {
        // Figure out the angle from the ball to the cursor
        if (Physics.Raycast(GameManager.CastCursor, out var hitInfo, 10, LayerMask.GetMask("PutterPlane"))
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
        var speed = -Input.GetAxis("Mouse X");
        var newRotation = Quaternion.Euler(putterBaseTransform.eulerAngles.x, putterBaseTransform.eulerAngles.y + (speed * 5f), putterBaseTransform.eulerAngles.z);
        var lookPosition = GameManager.Ball.transform.position - ((newRotation * Vector3.right) * putterDistanceFromBall);
        putterBaseTransform.SetPositionAndRotation(lookPosition, newRotation);
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