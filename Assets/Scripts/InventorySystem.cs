using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{
    [System.Serializable] // Esto hace que la clase sea visible en el Inspector
    public class InventoryItem
    {
        public string itemName;
        public Sprite itemSprite;

    }

    public InventoryItem[] availableItems; //"Diccionario" de los objetos de la sala
    public GameObject inventoryPanel; // Panel de Inventario
    public GameObject slotPrefab;     // Prefab del Slot (para intercambiarlo por la imagen de Sprite)
    public int slotsCount = 5;        // Número de slots 


    private List<GameObject> inventorySlots = new List<GameObject>(); //Aquí habrá los slots del inventario
    private Dictionary<int, InventoryItem> inventoryItems = new Dictionary<int, InventoryItem>(); //Diccionario de la posición que tiene cada objeto en el inv y su objeto


    void Start()
    {
        for(int i= 0; i < slotsCount; i++) //Crear los slots vacíos
        {
            GameObject slot = Instantiate(slotPrefab, inventoryPanel.transform);
            inventorySlots.Add(slot);
        }

        inventoryPanel.SetActive(false); //Desactivamos inventario
    }

    public void ToggleVisibilityInventory()
    {
        inventoryPanel.SetActive(!inventoryPanel.activeSelf); //Intercambiamos visibilidad
    }

    public void AddItemToSlot(string itemName, int slotIndex) //Añadir objeto a un Slot
    {
        if(slotIndex >= 0 && slotIndex < inventorySlots.Count) //Si existe la slot
        {
            InventoryItem itemToAdd = System.Array.Find(availableItems, item => item.itemName == itemName); //Buscamos si ese objeto existe en el diccionario

            if(itemToAdd != null){ //Si lo hace

                //Guardar el item en el diccionario
                inventoryItems[slotIndex] = itemToAdd;

                //Actualizar la imagen del slot
                Image slotImage = inventorySlots[slotIndex].GetComponent<Image>();
                slotImage.sprite = itemToAdd.itemSprite;
                slotImage.color = new Color(1, 1, 1, 1); //Hacer visible
            }
        }
    }

    public void RemoveItemFromSlot(int slotIndex) //Quitar objeto de slot
    {
        if (inventoryItems.ContainsKey(slotIndex)) //Si en en los objetos hay alguno con la slot puesta
        {
            inventoryItems.Remove(slotIndex); //Lo quitamos

            //Borramos su imagen
            Image SlotImage = inventorySlots[slotIndex].GetComponent<Image>();
            SlotImage.sprite = null;
            SlotImage.color = new Color(1,1,1,0);
        }
    }

    public bool GetItem(string itemName) //Comprobar si existe un objeto
    {
        foreach (var slot in inventoryItems) 
        {
            if (slot.Value.itemName == itemName) //Buscamos en todos los objetos si existe uno con el nombre pedido
            {
                return true;
            }
        }
        return false;
    }

    public void AddItem(string itemName) //Añadir objeto
    {
        int emptySlot = FindEmptySlot(); 
        if(emptySlot != -1) //Si hay una slot vacía se añade
        {
            AddItemToSlot(itemName, emptySlot);
        }
    }

    public void RemoveItem(string itemName) //Quitar objeto
    {
        int slotItem = FindSlotWithItem(itemName);
        if(slotItem != -1) //Si hay una slot con el nombre del objeto lo quitamos
        {
            RemoveItemFromSlot(slotItem);
        }
    }

    private int FindEmptySlot() //Buscar slots vacías
    {
        for(int i=0; i<slotsCount; i++)
        {
            if (!inventoryItems.ContainsKey(i)) //Si no hay en el diccionario de objetos alguno con ese índice, se devuelve ese índice
            {
                return i;
            }
        }
        return -1;
    }

    private int FindSlotWithItem(string itemName) //Buscar slots con nombre
    {
        foreach(var slot in inventoryItems)
        {
            if(slot.Value.itemName == itemName) //Si existe el objeto con el nombre, se devuelve el índice del slot
            {
                return slot.Key;
            }
        }
        return -1;
    }
}