using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Error to be thrown when an object is found that should have been garbage collected already.
/// </summary>
public class UncollectedGarbageException : Exception
{
    public readonly object Garbage;

    public UncollectedGarbageException(object garbage, string message) : base(message)
    {
        Garbage = garbage;
    }
}
