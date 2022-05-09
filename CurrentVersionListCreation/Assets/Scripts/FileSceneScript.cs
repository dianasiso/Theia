using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.IO;
using System;
using System.Globalization;

public class FileSceneScript : MonoBehaviour
{
    [SerializeField] public string[] files;
    [SerializeField] public string selectedFile;
    [SerializeField] public Text selectedFileText;

    [SerializeField] public List<FileInfo> infoFiles;
    [SerializeField] private Transform fileContainer;
    [SerializeField] private GameObject filePrefab;

    [SerializeField] private List<Item> currentItemList = new List<Item>();
    [SerializeField] private Transform listContainer;
    [SerializeField] private Transform itemlistContainer;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private GameObject canvas;

    [SerializeField] private GameObject noFilePopup;
    [SerializeField] private GameObject createFilePopup;
    
    [SerializeField] private GameObject itemLegend;

    public Text fileNameInput;


    string path;

    // Start is called before the first frame update
    void Start()
    {
        noFilePopup.SetActive(false);
        createFilePopup.SetActive(false);

        getFilesFromSystem();
       
        
    }

    public void getFilesFromSystem() {
        path = Application.persistentDataPath + "/SavedLists";
        Debug.Log(path);
        if(Directory.Exists(path)) {
            files = Directory.GetFiles(path, "*.json");
        } else {
            Directory.CreateDirectory(path);
        }
        foreach(string file in files) {
            string fileName = Path.GetFileName(file);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
            DateTime dt = File.GetLastWriteTime(file);
            FileInfo infoFile = new FileInfo(fileName, fileNameWithoutExtension, dt);
            infoFiles.Add(infoFile);
        }

        placeItemsInFileController();
    }

    public void placeItemsInFileController() {
        foreach(FileInfo file in infoFiles)
        {
            var fileInstance = Instantiate(filePrefab);
            // do something with the instantiated item -- for instance
            var textComponents = fileInstance.GetComponentsInChildren<Text>();
            textComponents[0].text = file.fileName;
            textComponents[1].text = file.lastWriteDate.ToString("dd/MM/yyyy H:mm:ss");
            Button button = fileInstance.GetComponentInChildren<Button>();
            button.onClick.AddListener(() => OpenPreviewList(file));
            //parent the item to the content container
            fileInstance.transform.SetParent(fileContainer);
            //reset the item's scale -- this can get munged with UI prefabs
            fileInstance.transform.localScale = Vector2.one;
        }
    }

    public void placeItemsInListController() {
        itemLegend.transform.SetParent(itemlistContainer);
        foreach(Item item in currentItemList)
        {
            Debug.Log(item.itemName);
            var item_go = Instantiate(itemPrefab);
            // do something with the instantiated item -- for instance
            var textComponents = item_go.GetComponentsInChildren<Text>();
            textComponents[0].text = item.itemName;
            textComponents[1].text = item.shelfName;
            textComponents[2].text = item.itemQuantity.ToString();
            //parent the item to the content container
            item_go.transform.SetParent(itemlistContainer);
            //reset the item's scale -- this can get munged with UI prefabs
            item_go.transform.localScale = Vector2.one;
        }
    }

    void OpenPreviewList(FileInfo file) {
        
        selectedFile = file.fileNameWithoutExtension;
        selectedFileText.text = selectedFile + ".json";
        currentItemList = new List<Item>();
        int count = 0;
        foreach (Transform child in itemlistContainer) {
            if (count == 0) {
                count += 1;
                continue;
            }
            GameObject.Destroy(child.gameObject);
        }

        string filePath = path + "/" + file.fileName;
        if (System.IO.File.Exists(filePath)) {
            List<string> jsonStrings = new List<string>();
            foreach (string line in System.IO.File.ReadLines(filePath))
            {  
                jsonStrings.Add(line);
            }  
            foreach(string item in jsonStrings) {
                currentItemList.Add(JsonUtility.FromJson<Item>(item));
            }
            placeItemsInListController();
        } else {
            noFilePopup.SetActive(true);
            canvas.SetActive(false);
            var textComponents = noFilePopup.GetComponentsInChildren<Text>();
            textComponents[0].text = "File does not open!";
            textComponents[1].text = "An error ocurred when trying to open this file! \nPlease check if the file does exist or it its open!";
        }

    }

    public void EditFileClick() {
        string filePath = path + "/" + selectedFile + ".json";
        FileNameInformation.path = filePath;
        string tempListFilePath = Application.persistentDataPath + "/SavedLists/temp.json";
        File.Copy(filePath, tempListFilePath);
        SceneManager.LoadScene("WorkAreaScene");
    }

    public void CreateFileButtonOnClick() {
        createFilePopup.SetActive(true);
        canvas.SetActive(false);
    }

    public void RemoveFileButtonOnClick() {
        selectedFileText.text = "---------------------";
        string filePath = path + "/" + selectedFile + ".json";
        selectedFile = null;
        System.IO.File.Delete(filePath);
        currentItemList = new List<Item>();
        infoFiles = new List<FileInfo>();
        int count = 0;
        foreach (Transform child in itemlistContainer) {
            if (count == 0) {
                count += 1;
                continue;
            }
            GameObject.Destroy(child.gameObject);
        }
        foreach (Transform child in fileContainer) {
            GameObject.Destroy(child.gameObject);
        }
        getFilesFromSystem();
    }

    public void GotItButtonBehaviour() {
        noFilePopup.SetActive(false);
        canvas.SetActive(true);
    }


    public void CreateFileCancelButtonOnClick() {
        createFilePopup.SetActive(false);
        canvas.SetActive(true);
    }

    public void CreateFileConfirmButtonOnClick() {
        string filePath = path + "/" + fileNameInput.text.ToString() + ".json";
        FileNameInformation.path = filePath;
        SceneManager.LoadScene("WorkAreaScene");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[System.Serializable]
public class FileInfo{
    public string fileName;
    public string fileNameWithoutExtension;
    public DateTime lastWriteDate;

    public FileInfo(string file, string fileNameWithoutExt, DateTime date)
    {
        fileName = file;
        fileNameWithoutExtension = fileNameWithoutExt;
        lastWriteDate = date;
    }

}
