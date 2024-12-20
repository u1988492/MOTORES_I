using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AltarPuzzle : MonoBehaviour, IPuzzle
{
    private bool isUnlocked = false;

    //figuras del altar 
    public string[] requiredFigureNames;

    public GameObject[] figureSlots;

    public Collider interactionZone;

    //booleanos par controlar si están colocadas las figuras o no
    private bool[] placedFigures;

    //inicializar como invisibles y no colocadas
    void Start(){
        placedFigures = new bool[requiredFigureNames.Length]; //misma longitud que la cantidad de figuras necesarias para desbloquear
       for(int i=0; i<requiredFigureNames.Length; i++)
        {
            figureSlots[i].SetActive(false);
            placedFigures[i] = false;
        }
    }


     public void Interact(InventorySystem inventory)
    {
        if (!isUnlocked) {
            for(int i=0; i<requiredFigureNames.Length; i++){
                //si está la figura en el inventario y no está colocada
                if(inventory.GetItem(requiredFigureNames[i]) && !placedFigures[i]){
                    PlaceFigure(inventory, i); //colocar
                }
            }
        }

        //desbloquear altar si están todas las figuras colocadas
        if(checkFiguresPlaced()){
            isUnlocked = true;
            gameObject.tag = "Untagged";
            Debug.Log("Altar desbloqueado");
        }
    }

    public void StopInteract()
    {
        //Vacío
    }
    private void PlaceFigure(InventorySystem inventory, int n){
        figureSlots[n].SetActive(true); //mostrar
        placedFigures[n] = true; //marcar como colocada 
        inventory.RemoveItem(requiredFigureNames[n]); //retirar figura del inventario
        Debug.Log($"{requiredFigureNames[n]} se ha colocado en el altar.");
    }

    private bool checkFiguresPlaced(){
        //comprobar si están todas las figuras colocadas
        foreach (bool placed in placedFigures){
            if(!placed){
                return false;
            }
        }
        return true;
    }
}
