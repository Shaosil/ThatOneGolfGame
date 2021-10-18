using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    private Button placeBallButton, placePutterButton;
    private Text ballPlaceInstructions, swingInstructionsText;

    private void Start()
    {
        placeBallButton = transform.Find("ButtonLayout/PlaceBall").GetComponent<Button>();
        placePutterButton = transform.Find("ButtonLayout/PlacePutter").GetComponent<Button>();
        ballPlaceInstructions = transform.Find("BallPlaceInstructions").GetComponent<Text>();
        swingInstructionsText = transform.Find("SwingInstructions").GetComponent<Text>();
    }

    private void Update()
    {
        // Set button visibilities based on current putting state
        bool placeBallEnabled = GameManager.CurGameState == GameManager.eGameState.PreGame;
        bool placePutterEnabled = GameManager.CurGameState == GameManager.eGameState.BallPlaced;
        bool ballInstructionsVisible = GameManager.CurGameState == GameManager.eGameState.PlacingBall || GameManager.CurGameState == GameManager.eGameState.BallPlaced;
        bool swingInstructionsVisisble = GameManager.CurGameState >= GameManager.eGameState.PutterPlaced && GameManager.CurGameState <= GameManager.eGameState.Swinging;

        if (placeBallEnabled != placeBallButton.interactable) placeBallButton.interactable = placeBallEnabled;
        if (placePutterEnabled != placePutterButton.interactable) placePutterButton.interactable = placePutterEnabled;
        if (ballInstructionsVisible != ballPlaceInstructions.gameObject.activeSelf) ballPlaceInstructions.gameObject.SetActive(ballInstructionsVisible);
        if (swingInstructionsVisisble != swingInstructionsText.gameObject.activeSelf) swingInstructionsText.gameObject.SetActive(swingInstructionsVisisble);
    }

    public void PlaceBall_Click()
    {
        GameManager.CurGameState = GameManager.eGameState.PlacingBall;
    }

    public void PlacePutter_Click()
    {
        GameManager.CurGameState = GameManager.eGameState.PlacingPutter;
        GameManager.PutterPlane.gameObject.SetActive(true);
        GameManager.PlacementZone.gameObject.SetActive(false);
        GameManager.Ball.Rigidbody.isKinematic = false;
    }
}