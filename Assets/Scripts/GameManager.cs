using System.Collections.Generic;
using UnityEngine;

public static class GameManager
{
    private static bool initialized = false;
    private static Dictionary<GameObject, Vector3> _originalPositions;

    public enum eGameState { PreGame,
        PlacingBall, BallPlaced,
        PlacingPutter, PutterPlaced,
        Swinging, BallMoving }
    public static eGameState CurGameState { get; set; } = eGameState.PreGame;

    public static Camera TheCameraThatIsSupposedToFollowTheBall { get; private set; }
    public static Ray CastCursor => TheCameraThatIsSupposedToFollowTheBall.ScreenPointToRay(Input.mousePosition);
    public static Ball Ball { get; private set; }
    public static Transform PlacementZone { get; set; }
    public static Transform PutterPlane { get; set; }
    public static Transform Putter { get; set; }

    public static void Initialize()
    {
        if (initialized) return;

        TheCameraThatIsSupposedToFollowTheBall = GameObject.Find("Main Camera").GetComponent<Camera>();
        var startingObjets = GameObject.Find("/Starting Objects").transform;
        Ball = startingObjets.Find("Ball").GetComponent<Ball>();
        PlacementZone = startingObjets.Find("Placement Zone");
        PutterPlane = startingObjets.Find("PutterPlane");
        Putter = PutterPlane.Find("PutterPlane/PutterBase/PutterPivot/Putter");

        _originalPositions = new Dictionary<GameObject, Vector3>
        {
            { TheCameraThatIsSupposedToFollowTheBall.gameObject, TheCameraThatIsSupposedToFollowTheBall.transform.position },
            { Ball.gameObject, Ball.transform.position }
        };

        initialized = true;
    }

    public static void Reset()
    {
        CurGameState = eGameState.PlacingBall;
        PlacementZone.gameObject.SetActive(true);
        PutterPlane.gameObject.SetActive(false);
        Ball.Rigidbody.isKinematic = true;

        foreach (var obj in _originalPositions.Keys)
        {
            obj.transform.position = _originalPositions[obj];

            var rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }
}