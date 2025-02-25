using UnityEngine;

public class AtticDoorTrigger : MonoBehaviour
{
    public LevelLoader levelLoader;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("dog front"))
        {
            if (levelLoader != null) 
            {
                levelLoader.LoadNextLevel();
            }
        }
    }
}
