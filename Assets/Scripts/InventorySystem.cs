using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public class InventoryItem
    {
        public string itemName;
        public int quantity;

        public InventoryItem(string name, int qty) //Constructor de un objeto
        {
            itemName = name;
            quantity = qty;
        }
    }

    private Dictionary<string, InventoryItem> inventory = new Dictionary<string, InventoryItem>(); //se Guarda un diccionario con el nombre y el item

    public void AddItem(string itemName, int quantity = 1) //añadir item
    {
        if (inventory.ContainsKey(itemName))
        {
            inventory[itemName].quantity += quantity;
        }
        else
        {
            inventory[itemName] = new InventoryItem(itemName, quantity);
        }
        Debug.Log($"Added {quantity} {itemName}(s) to inventory. Total: {inventory[itemName].quantity}");
    }

    public bool RemoveItem(string itemName, int quantity = 1) //quitar item
    {
        if (inventory.ContainsKey(itemName) && inventory[itemName].quantity >= quantity)
        {
            inventory[itemName].quantity -= quantity;
            if (inventory[itemName].quantity == 0)
            {
                inventory.Remove(itemName);
            }
            Debug.Log($"Removed {quantity} {itemName}(s) from inventory.");
            return true;
        }
        Debug.Log($"Not enough {itemName} in inventory.");
        return false;
    }

    public int GetItemCount(string itemName) //saber cantidad de item
    {
        if (inventory.ContainsKey(itemName))
        {
            return inventory[itemName].quantity;
        }
        return 0;
    }

    public void DisplayInventory() //mostrar inventario
    {
        Debug.Log("Current Inventory:");
        foreach (var item in inventory.Values)
        {
            Debug.Log($"{item.itemName}: {item.quantity}");
        }
    }
}