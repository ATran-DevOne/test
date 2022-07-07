using Isotras;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PoolData
{
    public int baseSize;
    public List<Layer> layers;
    public AutoGenerate autoGenerate;
    public Stair stair;
    public Floor floor;
    public bool valid;

    [SerializeField]
    public string type;
    [SerializeField]
    public int width;
    [SerializeField]
    public int length;

    void __construct()
    {
        layers = new List<Layer>();
        stair = new Stair();
        autoGenerate = new AutoGenerate();
    }
    
    public int _length
    {
        get
        {
            return length;
        }
        set
        {
            length = value;
        }
    }

    public int _width
    {
        get
        {
            return width;
        }
        set
        {
            width = value;
        }
    }

    public int _stairWidth
    {
        get
        {
            return stair.width;
        }
        set
        {
            stair.width = value;
        }
    }
}
