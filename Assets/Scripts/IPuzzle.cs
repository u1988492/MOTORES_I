using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPuzzle
{
    // Este m�todo ser� llamado cuando el jugador interact�e con el puzle
    void Interact(InventorySystem inventory);
}
