using System.Collections.Generic;

public enum Error
{
    NONE = 0,
    JSON_TIMEOUT = 1,
    JSON_DESERIALIZE = 2
}

public class ErrorMessage
{
    Dictionary<Error, string> errorListing;
    void __construct(Error error)
    {
        errorListing = new Dictionary<Error, string>();
        errorListing[Error.JSON_TIMEOUT] = "De verbinding met de server kon niet tot stand worden gebracht.";
        errorListing[Error.JSON_DESERIALIZE] = "Er trad een fout op tijdens het ophalen van de gegevens vanaf de server.";
    }
}
