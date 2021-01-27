using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UncollectedGarbageException : Exception
{
    public readonly object Garbage;

    public UncollectedGarbageException(object garbage, string message) : base(message)
    {
        Garbage = garbage;
    }
}
