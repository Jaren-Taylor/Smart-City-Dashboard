using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolable
{
    void OnLoanOut();
    void OnReclaim();
}

public enum ObjectType
{
    Entity,
    Model,
    Path
}
