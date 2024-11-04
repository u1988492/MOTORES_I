using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keypad : MonoBehaviour
{
    public string password = "1234";
    private string userInput = "";


    private void Start()
    {
        userInput = "";

    }

    public void ButtonClicked(string number)
    {
        userInput += number;
        if(userInput.Length >= 4)
        {
            //check password
            if(userInput == password)
            {
                Debug.Log("Entry Allowed");
            }
            else {
                Debug.Log("Not this time");
                userInput = "";
            }

        }
    }
}
