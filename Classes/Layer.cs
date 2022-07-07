using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Layer
{
    public int id;
    public PoolGrid poolGrid;
    public List<Block> blocks;
    public List<Addition> additions;
    public List<Sub> subs;
    public bool valid;

    void __construct()
    {
        poolGrid = new PoolGrid();
        blocks = new List<Block>();
        additions = new List<Addition>();
        subs = new List<Sub>();
    }
}
