using System;

namespace ShipExpander.Core;

[Serializable]
public class ShipExpanderException : Exception
{
    public ShipExpanderException() { }

    public ShipExpanderException(string message)
        : base(message) { }

    public ShipExpanderException(string message, Exception inner)
        : base(message, inner) { }
}