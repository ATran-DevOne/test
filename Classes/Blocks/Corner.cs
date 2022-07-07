using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corner
{
    public int length;
    public string name = "Corner";
    public List<GridPosition> positions;

    void __construct()
    {
        positions = new List<GridPosition>();
    }
}
