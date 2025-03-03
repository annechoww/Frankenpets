using System.Collections;
using System.Diagnostics;
using TMPro;
using UnityEngine;

public class LivingRoomText : MonoBehaviour
{
    public TextMeshProUGUI tutorialText;
    public TextMeshProUGUI tutorialSmallText;
    public GameObject speechBubbleTwoTails;
    public GameObject speechBubbleLeft;
    public GameObject speechBubbleRight;
    public GameObject catIcon;
    public GameObject dogIcon;
    private int currStage = 0;
    private Stopwatch stopwatch = new Stopwatch(); // might need to make new stopwatch every time coroutine is called

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        updateLivingRoomText();
    }

    // Update is called once per frame
    void Update()
    {
    
    }

    public const int start = 0;
    public const int dockingStation = 1;
    public void updateLivingRoomText()
    {
        switch (currStage)
        {
            case start:
                speechBubbleTwoTails.SetActive(true);
                tutorialText.text = "what's that near the stairs?";
                break;
            case dockingStation:
                tutorialText.text = "are these... other halves?!";
                StartCoroutine(waitForSeconds(4.0f));
                break;
            case 2:
                speechBubbleTwoTails.SetActive(false);
                catIcon.SetActive(false);
                dogIcon.SetActive(false);
                tutorialText.text = "";
                break;
        }
    }

    public void advanceLivingRoomStage()
    {
        currStage++;
        updateLivingRoomText();
    }

    public int getCurrLivingRoomStage()
    {
        return currStage;
    }


    private IEnumerator waitForSeconds(float seconds)
    {
        stopwatch.Start();

        while (stopwatch.Elapsed.TotalSeconds < seconds) 
        {
            yield return null;
        }

        stopwatch.Reset();
        advanceLivingRoomStage();
    }
}
