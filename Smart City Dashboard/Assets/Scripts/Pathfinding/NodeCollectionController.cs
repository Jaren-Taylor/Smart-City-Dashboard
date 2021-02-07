using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Schema;
using UnityEngine;

public class NodeCollectionController : MonoBehaviour
{
    [SerializeField]
    private GameObject[] NodeCollection; 

    public GameObject GetNode(int col, int row) => NodeCollection[ col + row * 4 ]; 
}
