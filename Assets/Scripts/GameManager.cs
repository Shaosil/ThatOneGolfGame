using UnityEngine;

public static class GameManager
{
    private static bool initialized = false;

    public enum eGameState { PreGame,
        PlacingBall, BallPlaced,
        PlacingPutter, PutterPlaced, Swinging }
    public static eGameState CurGameState { get; set; } = eGameState.PreGame;

    public static Camera TheCameraThatIsSupposedToFollowTheBall { get; private set; }
    public static Ray CastCursor => TheCameraThatIsSupposedToFollowTheBall.ScreenPointToRay(Input.mousePosition);
    public static Transform Ball { get; private set; }
    public static Transform PlacementZone { get; set; }
    public static Transform PutterPlane { get; set; }

    public static void Initialize()
    {
        if (initialized) return;

        TheCameraThatIsSupposedToFollowTheBall = GameObject.Find("Main Camera").GetComponent<Camera>();
        Ball = GameObject.Find("Course").transform.Find("Ball");
        PlacementZone = GameObject.Find("Course").transform.Find("Placement Zone");
        PutterPlane = GameObject.Find("Course").transform.Find("PutterPlane");

        initialized = true;
    }
}