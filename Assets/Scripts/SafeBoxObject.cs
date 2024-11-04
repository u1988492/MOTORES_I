using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeBoxObject : MonoBehaviour
{
    public float rotationSpeed = 90f;
    public float rotationAmount = 90f;
    
    private float currentRotation = 0f;
    private InteractionSystem interactionSystem;
    
    public bool isOpening = false;

    private void Start()
    {
        interactionSystem = FindObjectOfType<InteractionSystem>();
        if (interactionSystem == null)
        {
            Debug.LogError("No se encontr� InteractionSystem en la escena");
        }
    }

    private IEnumerator OpeningSafeBox()
    {
        isOpening = true;
        Debug.Log("Caja fuerte abri�ndose...");

        if (interactionSystem != null)
        {
            interactionSystem.ExitZoom();
        }

        // Importante: Obtenemos el transform del padre (el pivote)
        Transform pivote = transform.parent;

        while (currentRotation < rotationAmount)
        {
            float rotationThisFrame = rotationSpeed * Time.deltaTime;
            currentRotation += rotationThisFrame;

            // Rotamos el pivote en lugar de la puerta
            pivote.Rotate(Vector3.up, rotationThisFrame);

            yield return null;
        }

        isOpening = false;
        Debug.Log("Caja fuerte completamente abierta!");
        gameObject.tag = "Untagged";
    }

    public void OpenSafeBox()
    {
        StartCoroutine(OpeningSafeBox());
    }
}