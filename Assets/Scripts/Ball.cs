using UnityEngine;

public class Ball : MonoBehaviour
{
    private Transform putter;

    private Rigidbody rb;
    private Vector3 originPosition;

    // TEMP
    public float AbsHorizMagnitude;
    public float Speed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        putter = GameObject.Find("PutterPlane").transform.Find("PutterBase");
        rb = GetComponent<Rigidbody>();
        originPosition = transform.position;
    }

    // Update is called once per frame
    private void Update()
    {
        HandleControls();

        AbsHorizMagnitude = Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.z);
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

        // "Hit" the ball when left clicking
        //var velocity = rb.velocity.sqrMagnitude;
        //if (Input.GetMouseButtonDown(0) && velocity > -0.01f && velocity < 0.01f)
        //{
        //    // Start the putter animation swing
        //    GameObject.Find("PutterPivot").GetComponent<Animation>().Play();
        //}
    }
}