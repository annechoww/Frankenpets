using UnityEngine;

[System.Serializable]
public class Player : MonoBehaviour
{
    private int playerNumber;
    public bool isFront;
    public string species;

    public int PlayerNumber
    {
        set { playerNumber = value; }
        get { return playerNumber; }
    }

    public bool IsFront
    {
        get { return isFront; }
        set { isFront = !isFront; }
    }

    public string Species
    {
        get { return species; }
        set { species = species == "cat" ? "dog" : "cat"; }
    }

    public Player P1 = new Player();
    public Player P2 = new Player();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize the players
        P1.PlayerNumber = 1;
        P1.IsFront = true;
        P1.Species = "cat";
        P2.PlayerNumber = 2;
        P2.IsFront = false;
        P2.Species = "dog";
    }
}

// public class InitializePlayers : MonoBehaviour
// {
//     public Player P1 = new Player();
//     public Player P2 = new Player();

//     // Start is called once before the first execution of Update after the MonoBehaviour is created
//     void Start()
//     {
//         // Initialize the players
//         P1.PlayerNumber = 1;
//         P1.IsFront = true;
//         P1.Species = "cat";
//         P2.PlayerNumber = 2;
//         P2.IsFront = false;
//         P2.Species = "dog";
//     }
// }
