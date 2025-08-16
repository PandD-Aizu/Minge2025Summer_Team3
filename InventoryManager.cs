using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private bool isOpen = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }
    }

    /// <summary>
    /// インベントリを開閉する
    /// </summary>
    private void ToggleInventory()
    {
        isOpen = !isOpen;
        Debug.Log("Inventory " + (isOpen ? "Opened" : "Closed"));
    }
}   