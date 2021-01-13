using UnityEngine;

public static class GameManager
{
    private static bool initialized = false;

    public static UI UI { get; private set; }
    public static Camera TheCameraThatIsSupposedToFollowTheBall { get; private set; }
    public static Transform Ball { get; private set; }

    public static bool LiningUpShot { get; set; }
    public static bool Swinging { get; set; }

    public static void Initialize()
    {
        if (initialized) return;

        UI = GameObject.Find("UI").GetComponent<UI>();
        TheCameraThatIsSupposedToFollowTheBall = GameObject.Find("Main Camera").GetComponent<Camera>();
        Ball = GameObject.Find("Ball").transform;

        initialized = true;
    }
}