using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private void Awake()
    {
        // If another instance exists, destroy this one
        if (FindObjectsOfType<MusicManager>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        // Preserve this object across scene loads
        DontDestroyOnLoad(gameObject);
    }
}
