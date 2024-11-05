using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class SafeBox_Puzzle : MonoBehaviour, IPuzzle
{
    public Collider interactionZone;
    public SafeBoxObject safebox;
    public GameObject dial;

    private bool isUnlocked = false;
    
    
    public int[] positions = new int[40]; //vector con rango de valores de 0 a 39

    //inicialización de combinación correcta
    public (int position, string direction)[] correctCombination = {
        (15, "R"),
        (25, "L"),
        (5, "R")
    };

    //variables para controlar el comportamiento del candado
    private int currentPos = 0; //posición actual sobre el candado
    private int attemptCounter = 0; //intentos incorrectos
    private int currentRotationCount = 0; //numero de vueltas dadas
    private int currentStep = 0; //paso actual en la combinación, contando las entradas correctas
    private bool isTuriningRight = false; // dirección del movimiento
    private bool isTuriningLeft = false;

    private float pauseTimer = 0f; //timer para detectar si el jugador ha parado de girar
    private float pauseThreshold = 5f; // tiempo para considerar que ha parado
    private bool turning = false;
    private bool isActive = false; // detectar que está interactuando con el puzzle

    public void Interact(InventorySystem inventory){
        if(!isUnlocked){
            isActive = true;
        }
    }

    void Start(){
        //inicializar posiciones del dial
        for(int i = 0; i < 40; i++) positions[i] = i;
    }

    void Update(){
        if(!isActive || isUnlocked) return; // no permitir interactuar si está abierto o no se está interactuando activamente
        //empezar a girar si se detecta una flecha izq o dcha
        if(Input.GetKeyDown(KeyCode.LeftArrow)){
            Debug.Log("Girando a la izq");
            StartTurn("L");
        }
        else if(Input.GetKeyDown(KeyCode.RightArrow)){
            Debug.Log("Girando a la decha");
            StartTurn("R");
        }  

        //si se está girando, tratar el comportamiento del candado
        if(isTuriningRight){
            TurnRight();
        }
        else if(isTuriningLeft){
            TurnLeft();
        }

        //comprobar si el jugador ha parado de girar el dial
        if(turning){
            pauseTimer += Time.deltaTime;
            if(pauseTimer>=pauseThreshold){
                turning = false;
                StopTurn();
            }
        }
    }

    private void StartTurn(string direction){
        if(direction=="R"){
            isTuriningRight = true;
            isTuriningLeft = false;
        }
        else if(direction == "L"){
            isTuriningLeft = true;
            isTuriningRight = false;
        }

        turning = true;
        pauseTimer = 0f; // reiniciar timer de pausa
        currentRotationCount = 0; //reiniciar para siguiente turno
    }

    private void StopTurn(){
        isTuriningLeft = false;
        isTuriningRight = false;

        if(currentRotationCount>=1){
            CheckCombination();
        }
        else{
            Debug.Log("Giro incompleto. Debes rotarlo una vuelta entera primero");
        }
    }

    private void TurnRight(){
        currentPos = (currentPos+1+40) % 40; //izq = retroceder en el vector
        UpdateDialRotation();
        Debug.Log("Giro dcha, pos: " + currentPos);
        
        if(currentPos == 0) currentRotationCount++; //incrementar contador de vueltas 

        //si sigue girando, reiniciar timer de pausa
        if(Input.GetKey(KeyCode.RightArrow)){
            pauseTimer = 0f;
            turning = true;
        }

        //detectar que ha dejado de presionar la tecla para ver si ha parado de giarar
        if(Input.GetKeyUp(KeyCode.RightArrow)){
            turning = true;
        }
    }

    public void TurnLeft(){
        currentPos = (currentPos-1+40) % 40; //izq = retroceder en el vector
        UpdateDialRotation();
        Debug.Log("Giro izq, pos: " + currentPos);
        
        if(currentPos == 0) currentRotationCount++; //incrementar contador de vueltas 

        //si sigue girando, reiniciar timer de pausa
        if(Input.GetKey(KeyCode.LeftArrow)){
            pauseTimer = 0f;
            turning = true;
        }

        //detectar que ha dejado de presionar la tecla para ver si ha parado de giarar
        if(Input.GetKeyUp(KeyCode.LeftArrow)){
            turning = true;
        }
    }

    private void UpdateDialRotation(){
        float angle = currentPos * 9f; // 360 degrees / 40 positions = 9 degrees per position
        dial.transform.localRotation = Quaternion.Euler(0, 0, -angle);
    }

    private void CheckCombination(){
        var (targetPositon, requiredDirection) = correctCombination[currentStep];

        //si está girando en la dirección que debe
        if((isTuriningRight && requiredDirection == "R") || (isTuriningLeft && requiredDirection=="L")){
            //si es la posición correcta
            if(currentPos == targetPositon){
                Debug.Log("Paso " + currentStep + " correcto en la pos " + currentPos);
                currentStep++; //avanzar a siguiente pos de la combinacion

                //si es correcto el ultimo paso de la combinacion
                if(currentStep>=correctCombination.Length){
                    Unlock(); //abrir
                }
                else{
                    attemptCounter = 0;
                    Debug.Log("Procede al siguiente paso");
                }
            }
            //si no es la posicion correcta
            else{
                attemptCounter++;
                Debug.Log("Posicion incorrecta. Intento " + attemptCounter + "/3");
                if(attemptCounter>=3){
                    ResetLock();
                }
            }
        }
        //si no está girando en la dirección correcta
        else{
            Debug.Log("Girando en la direccion incorrecta");
            ResetLock();
        }
    }

    private void ResetLock(){
        currentPos = 0;
        currentStep = 0;
        attemptCounter = 0;
        isTuriningLeft = false;
        isTuriningRight = false;
        UpdateDialRotation();
        Debug.Log("Reiniciando candado.");
    }

    private void Unlock(){
        isUnlocked = true;
        isActive = false;
        safebox.GetComponent<SafeBoxObject>().OpenSafeBox();
        interactionZone.tag = "Untagged";
        interactionZone.enabled = false;
        Debug.Log("Candado abierto!");
    }

}
