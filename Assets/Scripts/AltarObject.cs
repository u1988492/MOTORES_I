using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AltarObject : MonoBehaviour
{
    private InteractionSystem interactionSystem;

    private void Start()
    {
        interactionSystem = FindObjectOfType<InteractionSystem>();
        if (interactionSystem == null)
        {
            Debug.LogError("No se encontr� InteractionSystem en la escena");
        }
    }
}