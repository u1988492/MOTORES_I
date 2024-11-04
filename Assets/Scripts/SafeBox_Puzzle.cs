using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeBox_Puzzle : MonoBehaviour, IPuzzle
{
    public Collider interactionZone;
    public SafeBoxObject safebox;
    public GameObject dial;

    private bool isUnlocked = false;
    public int[] positions = new int[40]; //vector con rango de valores de 0 a 39
    private int currentPos = 0; //posición actual sobre el candado
    private int attemptCounter = 0; //intentos incorrectos

    //inicialización de combinación correcta
    public (int position, string direction)[] correctCombination = {
        (15, "R"),
        (25, "L"),
        (5, "R")
    };

    private int currentStep = 0; //paso actual en la combinación, contando las entradas correctas
    private string currentDirection = "R"; //dirección del movimiento

    void Start(){
        //inicializar
        for(int i = 0; i < 40; i++) positions[i] = i;
    }


     public void Interact(InventorySystem inventory){
        if(Input.GetKeyDown(KeyCode.LeftArrow)){
            TurnLeft();
        }
        else if(Input.GetKeyDown(KeyCode.RightArrow)){
            TurnRight();
        }
     }

    private void UpdateDialRotation(){
    float angle = currentPos * 9f; // 360 degrees / 40 positions = 9 degrees per position
    dial.transform.localRotation = Quaternion.Euler(0, 0, -angle);
    }

    public void TurnRight(){
        currentDirection = "R";
        currentPos = (currentPos+1) % 40; //dcha = avanzar en el vector
        UpdateDialRotation();
        Debug.Log("Giro dcha, pos: " + currentPos);
        CheckCombination();
    }

    public void TurnLeft(){
        currentDirection = "L";
        currentPos = (currentPos-1+40) % 40; //izq = retroceder en el vector
        UpdateDialRotation();
        Debug.Log("Giro izq, pos: " + currentPos);
        CheckCombination();
    }

    private void CheckCombination(){
        //si el paso actual coincide con los requisitos de la combinación
        if(currentDirection == correctCombination[currentStep].direction    && currentPos==correctCombination[currentStep].position){
            Debug.Log("Paso " + currentStep + " correcto, posicion " + currentPos);
            currentStep++;
            //si es el último paso y es correcto, abrir candado
            if(currentStep>=correctCombination.Length){
                Unlock();
                return;
            }
        }
        //si no es correcto el paso actual
        else{
            attemptCounter++;
            Debug.Log("Paso incorrecto. Intento " + attemptCounter + "/3");
            //si es el tercer intento incorrecto, reiniciar candado
            if(attemptCounter>=3){
                ResetLock();
            }
        }
    }

    private void ResetLock(){
        currentPos = 0;
        currentStep = 0;
        attemptCounter = 0;
        currentDirection = "R";
        Debug.Log("Reiniciando candado.");
    }

    private void Unlock()
    {   
        safebox.GetComponent<SafeBoxObject>().OpenSafeBox();
        isUnlocked = true;
        interactionZone.tag = "Untagged";
        Debug.Log("Candado abierto!");
    }

}
