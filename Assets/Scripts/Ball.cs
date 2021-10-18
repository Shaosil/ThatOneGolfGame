using UnityEngine;

public class Ball : MonoBehaviour
{
    private bool draggingPlacement = false;

    public Rigidbody Rigidbody { get; private set; }
    public Collider Collider { get; private set; }

    // Start is called before the first frame update
    void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
        Rigidbody.maxAngularVelocity = 100;
        Collider = GetComponent<Collider>();
    }

    // Update is called once per frame
    private void Update()
    {
        RaycastHit hitInfo = new RaycastHit();

        // Placing or dragging the ball
        if ((GameManager.CurGameState == GameManager.eGameState.PlacingBall)
            && Physics.Raycast(GameManager.CastCursor, out hitInfo, 10, LayerMask.GetMask("PutterPlane")))
        {
            transform.position = hitInfo.point;

            if (Input.GetMouseButtonDown(0))
                GameManager.CurGameState = GameManager.eGameState.BallPlaced;
        }

        else if (GameManager.CurGameState == GameManager.eGameState.BallPlaced)
        {
            if (!draggingPlacement && Input.GetMouseButtonDown(0) && Physics.Raycast(GameManager.CastCursor, out hitInfo) && hitInfo.collider.gameObject == gameObject)
                draggingPlacement = true;
            else if (draggingPlacement && !Input.GetMouseButton(0))
                draggingPlacement = false;

            // Drag based on cursor acceleration
            if (draggingPlacement)
            {
                var mouseMovement = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse Y"));
                float oldY = transform.position.y;
                var placementBounds = GameObject.Find("Placement Zone").GetComponent<MeshCollider>().bounds;
                transform.Translate(mouseMovement * 0.075f, GameManager.TheCameraThatIsSupposedToFollowTheBall.transform);
                transform.position = new Vector3(Mathf.Clamp(transform.position.x, placementBounds.min.x, placementBounds.max.x),
                    oldY, Mathf.Clamp(transform.position.z, placementBounds.min.z, placementBounds.max.z));
            }
        }

        else if (GameManager.CurGameState == GameManager.eGameState.PlacingPutter)
        {
            // Keep the putter plane firmly attached to the ball
            GameManager.PutterPlane.position = transform.position;
        }
    }
}