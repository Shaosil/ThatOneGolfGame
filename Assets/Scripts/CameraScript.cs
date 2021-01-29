using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private float RotationSensitivity = 50f;
    private float zoomLevel;
    private Vector2 rotationValues;

    // Start is called before the first frame update
    void Start()
    {
        zoomLevel = (GameManager.TheCameraThatIsSupposedToFollowTheBall.transform.position - GameManager.Ball.position).magnitude;
        rotationValues = new Vector2(GameManager.TheCameraThatIsSupposedToFollowTheBall.transform.eulerAngles.x, GameManager.TheCameraThatIsSupposedToFollowTheBall.transform.eulerAngles.y); ;
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate cam
        if (Input.GetMouseButton(1))
        {
            var mouseMovement = new Vector2(-Input.GetAxis("Mouse Y") * 3f, Input.GetAxis("Mouse X") * 3f);
            rotationValues += mouseMovement * RotationSensitivity * Time.unscaledDeltaTime;
            rotationValues = new Vector2(Mathf.Clamp(rotationValues.x, -80f, 80f), rotationValues.y);
        }

        if (GameManager.CurGameState <= GameManager.eGameState.PlacingPutter)
            FreeCam();
        else
            FollowAndOrbitBall();
    }

    private void FreeCam()
    {
        float speed = 8f;

        // WASD controls
        if (Input.GetKey(KeyCode.LeftShift))
            speed *= 2;

        if (Input.GetKey(KeyCode.W))
            transform.position += transform.forward * speed * Time.unscaledDeltaTime;
        else if (Input.GetKey(KeyCode.S))
            transform.position -= transform.forward * speed * Time.unscaledDeltaTime;
        if (Input.GetKey(KeyCode.A))
            transform.position -= transform.right * speed * Time.unscaledDeltaTime;
        else if (Input.GetKey(KeyCode.D))
            transform.position += transform.right * speed * Time.unscaledDeltaTime;

        if (Input.GetKey(KeyCode.Space))
            transform.position += transform.up * speed * Time.unscaledDeltaTime;
        else if (Input.GetKey(KeyCode.LeftControl))
            transform.position -= transform.up * speed * Time.unscaledDeltaTime;

        transform.rotation = Quaternion.Euler(rotationValues);
    }

    private void FollowAndOrbitBall()
    {

        // Zoom cam
        var zoomInput = -Input.GetAxis("Mouse ScrollWheel") * 5f;
        zoomLevel = Mathf.Clamp(zoomLevel + zoomInput, 1f, 10f);

        // Follow cam
        var curRotation = Quaternion.Euler(rotationValues);
        var lookPosition = GameManager.Ball.transform.position - (curRotation * Vector3.forward * zoomLevel);
        transform.SetPositionAndRotation(lookPosition, curRotation);
    }
}