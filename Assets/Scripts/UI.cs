using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    private Button lineUpShotButton;
    private Button prepSwingButton;
    private Text swingInstructionsText;

    private void Start()
    {
        lineUpShotButton = transform.Find("LineUpShot").GetComponent<Button>();
        prepSwingButton = transform.Find("PrepSwing").GetComponent<Button>();
        swingInstructionsText = transform.Find("SwingInstructions").GetComponent<Text>();
    }

    private void Update()
    {
        // Set button visibilities based on current putting state
        bool lineUpVisible = GameManager.CurPuttingState == GameManager.ePuttingState.NotPutting
            || GameManager.CurPuttingState == GameManager.ePuttingState.LinedUp;
        bool prepSwingVisible = GameManager.CurPuttingState == GameManager.ePuttingState.LinedUp;
        bool swingInstructionsVisisble = GameManager.CurPuttingState == GameManager.ePuttingState.SwingPrep ||
            GameManager.CurPuttingState == GameManager.ePuttingState.Swinging;

        if (lineUpVisible != lineUpShotButton.interactable) lineUpShotButton.interactable = lineUpVisible;
        if (prepSwingVisible != prepSwingButton.interactable) prepSwingButton.interactable = prepSwingVisible;
        if (swingInstructionsVisisble != swingInstructionsText.gameObject.activeSelf) swingInstructionsText.gameObject.SetActive(swingInstructionsVisisble);
    }

    public void LineUpShot_Click()
    {
        GameManager.CurPuttingState = GameManager.ePuttingState.LiningUpShot;
    }

    public void PrepSwing_Click()
    {
        GameManager.CurPuttingState = GameManager.ePuttingState.SwingPrep;
    }
}