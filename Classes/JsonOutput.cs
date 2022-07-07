using System.Collections.Generic;

public class JsonOutput
{
    public Session session;
    public PoolData poolData;
    public Dictionary<string, Addition> additionTypes;
    public Dictionary<string, Stair> stairTypes;
    public Error error = Error.NONE;

    void __construct()
    {
        session = new Session();
        poolData = new PoolData();
        additionTypes = new Dictionary<string, Addition>();
        stairTypes = new Dictionary<string, Stair>();
    }
}
