using UnityEngine;

public class Task
{
    private string name;
    private int level;
    private bool isComplete = false;

    public string Name
    {
        get { return name; }
        set { name = value; }
    }

    public int Level
    {
        get { return level; }
        set { level = value; }
    }

    public bool IsComplete
    {
        get { return isComplete; }
        set { isComplete = value; }
    }

    public Task(string name, int level)
    {
        this.name = name;
        this.level = level;
    }

}