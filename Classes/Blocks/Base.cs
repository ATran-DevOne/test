using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base
{
    public int length;
    public string name = "Base";
    public List<GridPosition> positions;

    void __construct()
    {
        positions = new List<GridPosition>();
    }
}
