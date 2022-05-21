using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.IO;

public class ShelfSceneScript : MonoBehaviour
{
    public Text shelfTitleText;
    public Text listItems;
    [SerializeField] private List<Item> currentItemList = new List<Item>();
    [SerializeField] private List<Item> toAddItemList = new List<Item>();
    [SerializeField] private Transform m_ContentContainer;
    [SerializeField] private GameObject m_ItemPrefab;
    public string currentListFilePath;

    // Start is called before the first frame update
    void Start()
    {
        shelfTitleText.text = ShelfInformation.shelfTitle;

        currentListFilePath = FileNameInformation.path;
        Debug.Log(currentListFilePath);
        if (System.IO.File.Exists(currentListFilePath)) {
            List<string> jsonStrings = new List<string>();
            foreach (string line in System.IO.File.ReadLines(currentListFilePath))
            {  
                jsonStrings.Add(line);
            }  
            foreach(string item in jsonStrings) {
                currentItemList.Add(JsonUtility.FromJson<Item>(item));
            }
            
        } else {
            System.IO.File.Create(currentListFilePath);
        }

        placeItemsInController(currentItemList);

        // string itemsToShow = toStringItemsList(currentItemList);
        // listItems.text = itemsToShow;
    }

    public string toStringItemsList(List<Item> currentItemList) {
        string itemsToShow = "";
        foreach(Item item in currentItemList) {
            if(item.shelfName == shelfTitleText.text) {
                itemsToShow = itemsToShow + "--> " + item.itemName + " in " + item.shelfName + " x" + item.itemQuantity + "\n";
            }
        }
        return itemsToShow;
    }

    public void placeItemsInController(List<Item> currentItemList) {
        foreach(Item item in currentItemList)
        {
            if(item.shelfName != shelfTitleText.text) {
                continue;
            }
            var item_go = Instantiate(m_ItemPrefab);
            // do something with the instantiated item -- for instance
            var textComponents = item_go.GetComponentsInChildren<Text>();
            foreach (Text component in textComponents) {
                if(component.name == "Item") {
                    component.text = item.itemName;
                } else if(component.name == "Shelf") {
                    component.text = item.shelfName;
                } else if(component.name == "Quantity"){
                    component.text = "x" + item.itemQuantity;
                } else {
                    continue;
                }
            }
            var buttons = item_go.GetComponentsInChildren<Button>();
            buttons[1].onClick.AddListener(() => RemoveItemFromList(item));
            buttons[0].onClick.AddListener(() => AddItemToList(item));
            //parent the item to the content container
            item_go.transform.SetParent(m_ContentContainer);
            //reset the item's scale -- this can get munged with UI prefabs
            item_go.transform.localScale = Vector2.one;
        }
    }

    void RemoveItemFromList(Item item) {
        foreach(Item i in currentItemList) {
            if (item.shelfName == i.shelfName && item.itemName == i.itemName) {
                if (i.itemQuantity == 1) {
                    currentItemList.Remove(item);
                    clearContainer();
                    placeItemsInController(currentItemList);
                    break;
                } else {
                    i.itemQuantity = i.itemQuantity - 1;
                    //changeQuantity(item);
                    clearContainer();
                    placeItemsInController(currentItemList);
                }
            }
        }
    }

    void AddItemToList(Item item) {
        foreach(Item i in currentItemList) {
            if (item.shelfName == i.shelfName && item.itemName == i.itemName) {
                i.itemQuantity = i.itemQuantity + 1;
                clearContainer();
                placeItemsInController(currentItemList);
            }
        }
    }

    public void AddItemToList() {
        Button button = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        Text buttonText = button.GetComponentInChildren<Text>();
        Item itemToAdd = new Item();
        itemToAdd.itemName = buttonText.text;
        itemToAdd.itemQuantity = 1;
        itemToAdd.shelfName = shelfTitleText.text;
        int count = 0;
        foreach(Item i in currentItemList) {
            if (i.itemName == itemToAdd.itemName && i.shelfName == itemToAdd.shelfName){
                int itemQuantity = i.itemQuantity + 1;
                i.itemQuantity = itemQuantity;
                break;
            }
            count ++;
        }
        if (count == currentItemList.Count) {
            currentItemList.Add(itemToAdd);
        }
        clearContainer();
        placeItemsInController(currentItemList);
    }

    // Update is called once per frame
    void Update()
    {
        //placeItemsInController(currentItemList);
        // string itemsToShow = toStringItemsList(currentItemList);
        // listItems.text = itemsToShow;
    }

    public void ClickBackButton() {
        writeListToFile();
        SceneManager.LoadScene("WorkAreaScene");
    }

    void clearContainer(){
        foreach (Transform child in m_ContentContainer) {
             GameObject.Destroy(child.gameObject);
         }
    }

    void writeListToFile() {
        List<string> jsonItems = new List<string>();
        foreach(Item item in currentItemList) {
            string itemJson = JsonUtility.ToJson(item);
            jsonItems.Add(itemJson);
        }
        using (StreamWriter writer = new StreamWriter(currentListFilePath, false))  
        {  
            foreach(string item in jsonItems) {
                writer.WriteLine(item); 
            }
        }  
    }
}

[System.Serializable]
public class Item{
    public string itemName;
    public int itemQuantity;
    public string shelfName;
}


