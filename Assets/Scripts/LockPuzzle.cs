using UnityEngine;
using System.Collections;

public class LockPuzzle : MonoBehaviour, IPuzzle
{
    public string requiredKeyName;
    public float rotationSpeed = 90f; // Velocidad de rotaci�n en grados por segundo
    public float rotationAmount = 90f; // Cantidad total de rotaci�n en grados

    private bool isOpening = false;
    private float currentRotation = 0f;

    public void Interact(InventorySystem inventory)
    {
        if (!isOpening && inventory.GetItemCount(requiredKeyName) > 0)
        {
            inventory.RemoveItem(requiredKeyName);
            StartCoroutine(OpenLock());
        }
        else if (isOpening)
        {
            Debug.Log("El candado ya se est� abriendo.");
        }
        else
        {
            Debug.Log("Necesitas la llave correcta.");
        }
    }

    private IEnumerator OpenLock()
    {
        isOpening = true;
        Debug.Log("Candado abri�ndose...");

        while (currentRotation < rotationAmount)
        {
            float rotationThisFrame = rotationSpeed * Time.deltaTime;
            currentRotation += rotationThisFrame;

            // Rotamos alrededor del eje Y (ajusta esto si necesitas otro eje)
            transform.Rotate(Vector3.up, rotationThisFrame);

            yield return null;
        }

        Debug.Log("Candado completamente abierto!");
        isOpening = false;

        // Aqu� puedes a�adir cualquier l�gica adicional para cuando el candado est� completamente abierto
        // Por ejemplo, abrir la puerta asociada
    }
}