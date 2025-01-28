using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; 

[System.Serializable] // Esto hace que la clase sea visible en el Inspector
public class InventoryItem
{
    public string itemName;
    public Sprite itemSprite;
    public string itemDescription;
    public bool showTooltipSprite;

}

public class InventorySystem : MonoBehaviour
{
    public InventoryItem[] availableItems; //"Diccionario" de los objetos de la sala
    public GameObject inventoryPanel; // Panel de Inventario
    public GameObject slotPrefab;     // Prefab del Slot (para intercambiarlo por la imagen de Sprite)
    public int slotsCount = 5;        // Número de slots 


    private List<GameObject> inventorySlots = new List<GameObject>(); //Aquí habrá los slots del inventario
    private Dictionary<int, InventoryItem> inventoryItems = new Dictionary<int, InventoryItem>(); //Diccionario de la posición que tiene cada objeto en el inv y su objeto


    private void Start()
    {
        for (int i = 0; i < slotsCount; i++)
        {
            GameObject slot = Instantiate(slotPrefab, inventoryPanel.transform);

            // Añadir eventos para el tooltip
            var eventTrigger = slot.GetComponent<EventTrigger>();
            if (eventTrigger == null)
                eventTrigger = slot.AddComponent<EventTrigger>();

            // Evento de entrada del ratón
            EventTrigger.Entry entryPointerEnter = new EventTrigger.Entry();
            entryPointerEnter.eventID = EventTriggerType.PointerEnter;
            entryPointerEnter.callback.AddListener((data) => { OnPointerEnterSlot(slot); });
            eventTrigger.triggers.Add(entryPointerEnter);

            // Evento de salida del ratón
            EventTrigger.Entry entryPointerExit = new EventTrigger.Entry();
            entryPointerExit.eventID = EventTriggerType.PointerExit;
            entryPointerExit.callback.AddListener((data) => { OnPointerExitSlot(); });
            eventTrigger.triggers.Add(entryPointerExit);

            inventorySlots.Add(slot);
        }

        inventoryPanel.SetActive(false);
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

    private void OnPointerEnterSlot(GameObject slot)
    {
        int slotIndex = inventorySlots.IndexOf(slot); //Buscamos el slot
        if (inventoryItems.ContainsKey(slotIndex))
        {
            InventoryItem item = inventoryItems[slotIndex]; //Conseguimos el objeto
            TooltipSystem.Instance.Show(item.showTooltipSprite, item.itemName, item.itemDescription, item.itemSprite);
            //Podemos referenciar así al Tooltip ya que tiene un patrón Singleton
            
        }
    }

    private void OnPointerExitSlot()
    {
        TooltipSystem.Instance.Hide();
    }
}