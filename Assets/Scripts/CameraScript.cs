using UnityEngine;

public class CameraScript : MonoBehaviour
{
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
            rotationValues += mouseMovement * Time.unscaledDeltaTime * 300f;
            rotationValues = new Vector2(Mathf.Clamp(rotationValues.x, -80f, 80f), rotationValues.y);
        }

        // Zoom cam
        var zoomInput = -Input.GetAxis("Mouse ScrollWheel") * 5f;
        zoomLevel = Mathf.Clamp(zoomLevel + zoomInput, 1f, 10f);

        // Follow cam
        var curRotation = Quaternion.Euler(rotationValues);
        var lookPosition = GameManager.Ball.transform.position - ((curRotation * Vector3.forward) * zoomLevel);
        transform.SetPositionAndRotation(lookPosition, curRotation);
    }
}
