using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structure : MonoBehaviour
{
    public bool isPermanent = false;
    public void MakePermanent() => isPermanent = true;
}
