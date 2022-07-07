using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fit
{
    public int length;
    public string name = "Fit";
    public List<GridPosition> positions;
    public List<Addition> additions;

    void __construct()
    {
        positions = new List<GridPosition>();
        additions = new List<Addition>();
    }
}
