using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

public class WorkingAreaScript : MonoBehaviour
{
    //public Text listItems;

    [SerializeField] private List<Item> currentItemList = new List<Item>();
    [SerializeField] private Transform m_ContentContainer;
    [SerializeField] private GameObject m_ItemPrefab;
    [SerializeField] private GameObject savedPopup;
    [SerializeField] private GameObject backPopup;

    public string currentListFilePath;
    public bool fileSaved;

    // Start is called before the first frame update
    void Start()
    {
        savedPopup.SetActive(false);
        backPopup.SetActive(false);
        fileSaved = false;


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
            itemsToShow = itemsToShow + "--> " + item.itemName + " in " + item.shelfName + " x" + item.itemQuantity + "\n";
        }
        return itemsToShow;
    }

    // public bool alreadyExists(Item item) {
    //     foreach (GameObject obj in listItemsGameObjects) {
    //         var textcomponents = obj.GetComponentsInChildren<Text>();
    //         int count = 0;
    //         foreach (Text component in textComponents) {
    //             if(component.name == "Item" && component.text == item.itemName) {
    //                 count += 1;
    //             } else if(component.name == "Shelf" && component.text == item.shelfName) {
    //                 count += 1;
    //             }
    //         }
    //         if(count == 2) {
    //             return true;
    //         }
    //     }
    //     return false;
    // }

    public void placeItemsInController(List<Item> currentItemList) {
        foreach(Item item in currentItemList)
        {
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

    public void EraseList() {
        System.IO.File.Delete(currentListFilePath);
        currentItemList = new List<Item>();
        clearContainer();
    }

    public void SaveList() {
        writeListToFile();
        StartCoroutine(PopupCoroutine(savedPopup));
        savedPopup.SetActive(true);
        fileSaved = true;
        
    }

    IEnumerator PopupCoroutine(GameObject popup)
	{
		yield return new WaitForSeconds(2);
		popup.SetActive(false);;
	}

    public void GoBack() {
        if (fileSaved) {
            string tempListFilePath = Application.persistentDataPath + "/SavedLists/temp.json";
            System.IO.File.Delete(tempListFilePath);
            SceneManager.LoadScene("FileScene");
        } else {
            backPopup.SetActive(true);
        }
    }

    public void ExitBackPopup() {
        backPopup.SetActive(false);
    }

    public void GoBackSave(bool save) {
        string tempListFilePath = Application.persistentDataPath + "/SavedLists/temp.json";
        if (save) {
            writeListToFile();
        } else {
            File.Copy(tempListFilePath, currentListFilePath, true);
        }
        System.IO.File.Delete(tempListFilePath);
        backPopup.SetActive(false);
        SceneManager.LoadScene("FileScene");
        
    }

    void clearContainer(){
        foreach (Transform child in m_ContentContainer) {
             GameObject.Destroy(child.gameObject);
         }
    }

    // Update is called once per frame
    void Update()
    {
        //clearContainer();
        //placeItemsInController(currentItemList);
        // string itemsToShow = toStringItemsList(currentItemList);
        // listItems.text = itemsToShow;
    }

    public void ClickButton() {
        Button button = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        Text buttonText = button.GetComponentInChildren<Text>();
        ShelfInformation.shelfTitle = buttonText.text;
        writeListToFile();
        SceneManager.LoadScene("ShelfScene");
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
