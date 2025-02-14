using UnityEngine;
using UnityEngine.UI;

public class MoveRugTask : MonoBehaviour
{
    public Image taskItem;
    public Color completedColor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Enter " + collision.transform.name);
    }


    void OnCollisionExit(Collision collision)
    {
        Debug.Log("Exit " + collision.transform.name);

        if (collision.transform.name == "rug") 
        {
            FinishTask();
        }
    }

    private void FinishTask()
    {
        taskItem.color = completedColor;
    }
}
