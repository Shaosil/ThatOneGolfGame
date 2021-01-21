using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    private Button placePutterButton;
    private Text swingInstructionsText;

    private void Start()
    {
        placePutterButton = transform.Find("PlacePutter").GetComponent<Button>();
        swingInstructionsText = transform.Find("SwingInstructions").GetComponent<Text>();
    }

    private void Update()
    {
        // Set button visibilities based on current putting state
        bool placePutterEnabled = GameManager.CurPuttingState == GameManager.ePuttingState.NotPutting;
        bool swingInstructionsVisisble = GameManager.CurPuttingState == GameManager.ePuttingState.PutterPlaced;

        if (placePutterEnabled != placePutterButton.interactable) placePutterButton.interactable = placePutterEnabled;
        if (swingInstructionsVisisble != swingInstructionsText.gameObject.activeSelf) swingInstructionsText.gameObject.SetActive(swingInstructionsVisisble);
    }

    public void PlacePutter_Click()
    {
        GameManager.CurPuttingState = GameManager.ePuttingState.PlacingPutter;
    }
}