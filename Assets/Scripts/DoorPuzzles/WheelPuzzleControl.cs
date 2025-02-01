/*Este script controla todo el objeto del candado de tres ruedas
CAMBIAR CUANDO SEA NECESARIO PARA EL JUEGO: correctCombination /  wheelName (añadir o quitar las que sean necesarias) / objeto que debe ser abierto (ahora es DoorObject)*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelControl : MonoBehaviour {
    private int[] result;
    public int[] correctCombination;
    public GameObject door;
    public Collider interactionZone;

    private void Start(){
        result  = new int[] {0, 0, 0};
        Wheel.Rotated += CheckResults;
    }

    private void CheckResults(string wheelName, int number){
        switch(wheelName){
            case "NumericWheel_1":
                result [0] = number;
                break;
            case "NumericWheel_2":
                result [1] = number;
                break;
            case "NumericWheel_3":
                result [2] = number;
                break;
        }
        
        if(result[0] == correctCombination[0] && result[1] == correctCombination[1] && result[2] == correctCombination[2]){
            door.GetComponent<DoorObject>().OpenDoor();
            interactionZone.tag = "Untagged";
            Debug.Log("Correct combination.");
        }
    }

    private void OnDestroy(){
        Wheel.Rotated -= CheckResults;
    }
}