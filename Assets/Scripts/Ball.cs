using UnityEngine;

public class Ball : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 originPosition;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        originPosition = transform.position;
    }

    // Update is called once per frame
    private void Update()
    {
        HandleControls();
    }

    private void HandleControls()
    {
        // Reset if spacebar pressed or we fall out of bounds
        if (Input.GetKeyDown(KeyCode.Space) || transform.position.y < -10)
        {
            transform.position = originPosition;
            rb.velocity = new Vector3();
            rb.angularVelocity = new Vector3();
        }
    }
}