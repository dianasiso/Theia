using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective
{
    public string item;
    public int goal;
    public int count;

    public Objective(string item, int goal) 
    {
        this.item = item;
        this.goal = goal;
        count = 0;
    }

    public void AddOne()
    {
        count++;
    }

    public bool Done()
    {
        return count == goal;
    }

    public override string ToString()
    {
        return $"{item}: {count}/{goal}";
    }
}
