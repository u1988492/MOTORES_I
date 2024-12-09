using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keypad : MonoBehaviour
{
    public int password = 1234;
    public int password_length = 4;
    public GameObject door;
    private int userInput = 0;
    public Collider interactionZone;

    private int pos = 0;

    private void Start()
    {
        userInput = 0;
        pos = password_length-1;
    }

    public void ButtonClicked(int number)
    {
        if (number < 0 && pos < password_length - 1)
        {
            int factor = (int)Mathf.Pow(10, pos + 1);
            userInput -= (userInput / factor % 10) * factor;  // Eliminar el último dígito
            pos++;            // Recuperar una posición
        }
        else if(pos >= 0)
        {
            userInput += number * (int)Mathf.Pow(10, pos);
            pos--;
        }

        Debug.Log("Current password: " + userInput);

        // Verifica si se alcanzó la longitud esperada
        if (pos < 0)
        {
            // Check password
            if (userInput == password)
            {
                Debug.Log("Entry Allowed");
                OpenDoor();
            }
            else
            {
                Debug.Log("Not this time");
                userInput = 0;
                pos = password_length - 1;
            }
        }
    }

    private void OpenDoor()
    {
        door.GetComponent<DoorObject>().OpenDoor();
        interactionZone.tag = "Untagged";
    }
}
