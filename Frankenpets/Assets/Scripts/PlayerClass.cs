using UnityEngine;

public class Player
{
    private int playerNumber;
    private bool isFront;
    private string species;
    private GameObject half;
    private GameObject magnet;

    public int PlayerNumber
    {
        get { return playerNumber; }
        set { playerNumber = value; }
    }

    public bool IsFront
    {
        get { return isFront; }
        set { isFront = value; }
    }

    public string Species
    {
        get { return species; }
        set { species = value; }
    }

    public GameObject Half
    {
        get { return half; }
        set { half = value; }
    }

    public GameObject Magnet
    {
        get { return magnet; }
        set { magnet = value; }
    }

}