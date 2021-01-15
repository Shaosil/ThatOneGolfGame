using UnityEngine;

public class Putter : MonoBehaviour
{
    // TODO: OPTIONS
    private bool rightHanded = true;

    private Rigidbody ballRb;
    private Transform putterBaseTransform;
    private GameObject putterMesh;
    private Animator animator;

    private void Start()
    {
        ballRb = GameObject.Find("Ball").GetComponent<Rigidbody>();
        putterBaseTransform = transform.parent;
        putterMesh = transform.GetChild(0).gameObject;
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
                    animator.SetBool("startingSwing", true);
                }
                else if (Input.GetKeyDown(KeyCode.Escape))
                {
                    // Cancel and go back to a lined up state
                    GameManager.CurPuttingState = GameManager.ePuttingState.LinedUp;
                }
                
                break;

            case GameManager.ePuttingState.Swinging:
                swung = animator.GetBool("swung");
                if (swung)
                    break;

                if (!Input.GetMouseButton(0))
                {
                    // Commit the swing
                    animator.SetBool("swung", true);
                }

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    // Cancel the swing
                    animator.SetBool("startingSwing", false);
                    GameManager.CurPuttingState = GameManager.ePuttingState.SwingPrep;
                }
                break;
        }

        // Hit the ball once the putter reaches a certain angle
        var angle = transform.localEulerAngles.z % 360;
        angle = angle > 180 ? angle - 360 : angle;

        if (swung && angle > 6f)
        {
            GameManager.CurPuttingState = GameManager.ePuttingState.NotPutting;
            animator.SetBool("swung", false);
            animator.SetBool("startingSwing", false);

            var normalizedVector = (ballRb.transform.position - transform.parent.position).normalized;
            ballRb.AddForce(normalizedVector * 7.5f, ForceMode.Impulse);
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
}