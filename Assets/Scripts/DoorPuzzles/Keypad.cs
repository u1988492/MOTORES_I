using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Keypad : MonoBehaviour
{
    public int password = 1234;
    public int password_length = 4;
    public GameObject door;
    private int userInput = 0;
    public Collider interactionZone;
    public TMP_Text showercode;

    private int pos = 0;

    private void Start()
    {
        userInput = 0;
        pos = password_length-1;
        showercode.text = "";
    }

    public void ButtonClicked(int number)
    {
        Debug.Log("Tecla pulsada: " + number);
        if (number < 0 && pos < password_length - 1)
        {
            int factor = (int)Mathf.Pow(10, pos + 1);
            userInput -= (userInput / factor % 10) * factor;  // Eliminar el último dígito
            pos++;            // Recuperar una posición
            if (showercode.text.Length > 0)
            {
                showercode.text = showercode.text.Substring(0, showercode.text.Length - 1);
            }
        }
        else if(pos >= 0)
        {
            userInput += number * (int)Mathf.Pow(10, pos);
            pos--;
            showercode.text += "*";
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
            showercode.text = "";
        }
    }

    private void OpenDoor()
    {
        door.GetComponent<DoorObject>().OpenDoor();
        interactionZone.tag = "Untagged";
    }
}
