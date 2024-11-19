using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keypad : MonoBehaviour
{
    public string password = "1234";
    public GameObject door;
    private string userInput = "";
    public Collider interactionZone;



    private void Start()
    {
        userInput = "";

    }
    
    public void ButtonClicked(string number)
    {
        userInput += number;
        Debug.Log("Number introduced: " + number);
        if(userInput.Length >= 4)
        {
            //check password
            if(userInput == password){
                Debug.Log("Entry Allowed");
                OpenDoor();
            }
            else {
                Debug.Log("Not this time");
                userInput = "";
            }

        }
    }

    private void OpenDoor()
    {
        door.GetComponent<DoorObject>().OpenDoor();
        interactionZone.tag = "Untagged";
    }
}
