using UnityEngine;

public class Putter : MonoBehaviour
{
    private Rigidbody ballRb;
    private Transform putterBaseTransform;

    private void Start()
    {
        ballRb = GameObject.Find("Ball").GetComponent<Rigidbody>();
    }

    private void TestAnimationCall(string s)
    {
        var normalizedVector = (ballRb.transform.position - transform.parent.position).normalized;
        ballRb.AddForce(normalizedVector * 7.5f, ForceMode.Impulse);
    }
}
