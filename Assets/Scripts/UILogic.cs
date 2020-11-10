using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum Panel {
    START,
    IN,
    END,
    HELP,
}

public class UILogic : MonoBehaviour
{
    public Director director;
    public GameObject StartPanel;
    public GameObject InPanel;
    public GameObject EndPanel;
    public GameObject HelpPanel;
    public TextMeshProUGUI HighScore;
    public TextMeshProUGUI Score;
    public TextMeshProUGUI Breakdown;
    public int helpTime = 6;

    // Start is called before the first frame update
    void Start()
    {
        // load saved data
        SetPanelActive(Panel.START);
        ChangeHighScore(0);
        ChangeScore(0);
        ChangeBreakdown(0, 0, 0);
    }

    public void StartButton() {
        // start from director
        director.InScene();
    }

    public void HelpButton() {
        StartCoroutine(ShowHelp());
    }

    public void ChangeScore(int val) {
        Score.text = "Score: " + val.ToString();
    }

    public void ChangeHighScore(int val) {
        HighScore.text = "Highscore: " + val.ToString();
    }

    public void ChangeBreakdown(int orbs, int blocks, int score) {
        Breakdown.text = "Orbs - " + orbs.ToString() + "\n" + "Distance - " + blocks.ToString() + "\n" + "Score - " + score.ToString();
    }

    IEnumerator ShowHelp() {
        SetPanelActive(Panel.HELP);
        yield return new WaitForSeconds(helpTime);
        SetPanelActive(Panel.START);
    }

    public void SetPanelActive(Panel val) {
        switch (val) {
            case Panel.START: {
                StartPanel.SetActive(true);
                InPanel.SetActive(false);
                EndPanel.SetActive(false);
                HelpPanel.SetActive(false);
                return;
            }
            case Panel.IN: {
                StartPanel.SetActive(false);
                InPanel.SetActive(true);
                EndPanel.SetActive(false);
                HelpPanel.SetActive(false);
                return;
            }
            case Panel.END: {
                StartPanel.SetActive(false);
                InPanel.SetActive(false);
                EndPanel.SetActive(true);
                HelpPanel.SetActive(false);
                return;
            }
            case Panel.HELP: {
                StartPanel.SetActive(false);
                InPanel.SetActive(false);
                EndPanel.SetActive(false);
                HelpPanel.SetActive(true);
                return;
            }
        }
    }
}
