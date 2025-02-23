using UnityEngine;
using UnityEngine.UI;

public class MoveRugTask : MonoBehaviour
{
    public Image taskItem;
    public Color completedColor;

    private TutorialText tutorialText;

    void Awake()
    {
        tutorialText = GameObject.Find("TutorialTextManager").GetComponent<TutorialText>();
    }

    // void OnCollisionExit(Collision collision)
    // {
    //     Debug.Log("Exit " + collision.transform.name);

    //     if (collision.transform.name == "Rug") 
    //     {
    //         FinishTask();
    //     }
    // }

    void OnTriggerExit(Collider other)
    {
        if (other.transform.name == "Rug")
        {
            FinishTask();
        }
    }

    private void FinishTask()
    {
        taskItem.color = completedColor;
        if (tutorialText.getCurrTutorialStage() == TutorialText.tutDragRug)
        {
            tutorialText.advanceTutorialStage();
        }
    }
}
