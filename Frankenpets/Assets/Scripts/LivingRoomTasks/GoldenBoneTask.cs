using UnityEngine;
using UnityEngine.UI;

public class GoldenBoneTask : MonoBehaviour
{
    [Header("Task Manager")]
    public GameObject taskItem;
    //public Color completedColor;
    //public TMPro.TextMeshProUGUI taskLabel;
    public Task task = new Task("Find Golden Bone", 1);

    [Header("Locate Task Variables")]
    public GameObject taskLight;
    public GameObject taskParticle;
    public GameObject arrow;

    private bool isBoneFound = false;

    void Awake()
    {
        // Register the task with the TaskManager
        TaskManager.RegisterTask(task);
        print("Task registered: " + task.Name);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if a player collided with the bone
        if (!isBoneFound && collision.gameObject.layer == LayerMask.NameToLayer("Pet"))
        {
            FindGoldenBone();
        }
    }

    void FindGoldenBone()
    {
        isBoneFound = true;
        print("Golden Bone found!");
        FinishTask();
    }

    private void FinishTask()
    {
        //taskItem.color = completedColor;
        task.IsComplete = true;
        //taskLabel.fontStyle = TMPro.FontStyles.Strikethrough;
        taskItem.transform.GetChild(1).gameObject.SetActive(true);
        
        // Destroy task location indicators
        FindTasks.Instance.DestroyFindTaskMechanic(arrow, taskParticle, taskLight);
        
        // Mark task as complete in the TaskManager
        TaskManager.Instance.CompleteTask();
        AudioManager.Instance.PlayTaskCompletionSound();
    }
}