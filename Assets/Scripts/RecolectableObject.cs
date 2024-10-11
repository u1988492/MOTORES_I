using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RecolectableObject : MonoBehaviour
{
    public string nameObject;

    public void Recolect()
    {
        Destroy(gameObject);
    }
}
