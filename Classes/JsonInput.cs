using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonInput
{
    public Session session;
    public PoolData poolData;

    void __construct()
    {
        session = new Session();
        poolData = new PoolData();
    }
}
