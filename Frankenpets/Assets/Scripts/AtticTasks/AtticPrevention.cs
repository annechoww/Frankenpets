using UnityEngine;

public class AtticPrevention : MonoBehaviour
{
    public GameObject blocker;
    // private PlayerManager playerManager;
    private TutorialText tutorialText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // playerManager = FindObjectOfType<PlayerManager>();
        tutorialText = FindObjectOfType<TutorialText>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(tutorialText.getCurrTutorialStage());
        if (other.CompareTag("dog front") && (tutorialText.getCurrTutorialStage() == TutorialText.tutComplete || tutorialText.getCurrTutorialStage() == TutorialText.annoyedCat))
        {
            UnityEngine.Debug.Log("attic prevention");
            blocker.SetActive(true);
            tutorialText.leaveAtticSpeech();
            
        } else if (other.CompareTag("cat front") && (tutorialText.getCurrTutorialStage() == TutorialText.scaredDog || tutorialText.getCurrTutorialStage() == TutorialText.leaveAttic))
        {
            UnityEngine.Debug.Log("can fall ");
            blocker.SetActive(false);
            tutorialText.hideLeaveAtticSpeech();
        }

       
    }
}
