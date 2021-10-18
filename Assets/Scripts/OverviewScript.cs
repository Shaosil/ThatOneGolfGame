using UnityEngine;

public class OverviewScript : MonoBehaviour
{
    private void Start()
    {
        GameManager.Initialize();
    }

    private void Update()
    {
        // Initial raycast activation of the ball (Ball.cs handles the rest)
        if (GameManager.CurGameState == GameManager.eGameState.PlacingBall && !GameManager.Ball.gameObject.activeSelf
            && Physics.Raycast(GameManager.CastCursor, out var hitInfo, 10, LayerMask.GetMask("PutterPlane")))
        {
            GameManager.Ball.gameObject.SetActive(true);
            GameManager.Ball.Rigidbody.isKinematic = true;
            GameManager.Ball.transform.position = hitInfo.point;
        }

        // Listen for reset level requests
        // TODO: Remove or disable debug-only feature
        if (Input.GetKeyDown(KeyCode.Backspace))
            GameManager.Reset();
    }
}