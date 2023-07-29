using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public ItemData itemData;

    private void Start()
    {
        CreateNewItem("Health Potion", Resources.Load<Sprite>("HealthPotion"), 10);
        CreateNewItem("Mana Potion", Resources.Load<Sprite>("ManaPotion"), 10);
        CreateNewItem("Atk Potion", Resources.Load<Sprite>("AtkPotion"), 10);
    }
    
    private void CreateNewItem(string name, Sprite icon, int price)
    {
        // ItemData newItem = new ItemData(); // Ŭ���� �ν��Ͻ� ����
        ItemData newItem = ScriptableObject.CreateInstance<ItemData>(); // SO �ν��Ͻ� ����
        newItem.itemName = name;
        newItem.itemIcon = icon;
        newItem.itemPrice = price;

        string assetPath = "Assets/" + name + ".asset";

        // UnityEdigor�� ��Ÿ�ӿ����� ���� �� ��
        // + namespace ��� ���� �ֵ��� ��Ÿ�ӿ����� ���� �� ��
        UnityEditor.AssetDatabase.CreateAsset(newItem, assetPath);
        UnityEditor.AssetDatabase.SaveAssets();

        Debug.Log("ScriptableObject �������� ����Ǿ����ϴ�. ���: " + assetPath);
    }
}