using UnityEngine;

public class Task3Tutorial : MonoBehaviour
{
    private bool isFirstTrigger = true;
    private TutorialText tutorialText;

    void Start()
    {
        tutorialText = GameObject.Find("TutorialTextManager").GetComponent<TutorialText>();
    }

    void OnTriggerEnter(Collider other)
    {            

        if (other.CompareTag("cat front") && isFirstTrigger && (tutorialText.getCurrTutorialStage() == TutorialText.tutMoveToRug))
        {
            isFirstTrigger = false;
            tutorialText.advanceTutorialStage();
        }
    }
}
