using UnityEngine;

public static class GameManager
{
    private static bool initialized = false;

    public enum ePuttingState { NotPutting, PlacingPutter, PutterPlaced, Swinging }
    public static ePuttingState CurPuttingState { get; set; } = ePuttingState.NotPutting;

    public static Camera TheCameraThatIsSupposedToFollowTheBall { get; private set; }
    public static Transform Ball { get; private set; }

    public static void Initialize()
    {
        if (initialized) return;

        TheCameraThatIsSupposedToFollowTheBall = GameObject.Find("Main Camera").GetComponent<Camera>();
        Ball = GameObject.Find("Ball").transform;

        initialized = true;
    }
}