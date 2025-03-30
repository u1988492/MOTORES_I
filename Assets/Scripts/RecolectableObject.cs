using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RecolectableObject : MonoBehaviour
{
    public string nameObject;

    public Material normalMaterial;
    public Material emissiveMaterial;

    private Renderer rendererGestor;

    void Start()
    {
        rendererGestor = GetComponent<Renderer>();
    }

    public void Recolect()
    {
        Destroy(gameObject);
    }

    public void toEmissive()
    {
        if (rendererGestor != null && emissiveMaterial != null)
        {
            rendererGestor.material = emissiveMaterial;
        }
    }

    public void toNormal()
    {
        if (rendererGestor != null && normalMaterial != null)
        {
            rendererGestor.material = normalMaterial;
        }
    }
}
