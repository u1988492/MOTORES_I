using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPuzzle
{
    // Este método será llamado cuando el jugador interactúe con el puzle
    void Interact(InventorySystem inventory);
}
