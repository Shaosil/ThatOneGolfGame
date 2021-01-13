using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public Button LineUpShotButton { get; private set; }
    public Button StartSwingButton { get; private set; }
    public Text SwingInstructionsText { get; private set; }

    private void Start()
    {
        LineUpShotButton = transform.Find("LineUpShot").GetComponent<Button>();
        StartSwingButton = transform.Find("StartSwing").GetComponent<Button>();
        SwingInstructionsText = transform.Find("SwingInstructions").GetComponent<Text>();
    }

    public void LineUpShot_Click()
    {
        LineUpShotButton.interactable = false;
        StartSwingButton.interactable = false;
        GameManager.LiningUpShot = true;
    }

    public void StartSwing_Click()
    {
        LineUpShotButton.interactable = false;
        StartSwingButton.interactable = false;
        SwingInstructionsText.gameObject.SetActive(true);
        GameManager.Swinging = true;
    }
}