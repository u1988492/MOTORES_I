using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public Camera Camera;
    public Collider interactionZone;

    public Material normalMaterial;
    public Material emissiveMaterial;

    private Renderer rendererGestor;

    void Start()
    {
        rendererGestor = GetComponent<Renderer>();
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