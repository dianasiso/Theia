using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.IO;
using System;
using System.Globalization;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using System.Net;

public class FileSceneScript : MonoBehaviour
{
    [SerializeField] public string[] files;
    [SerializeField] public string selectedFile;
    [SerializeField] public Text selectedFileText;

    [SerializeField] public FileInfo selectedFileInfo;

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
    [SerializeField] private GameObject noIpAddressPopup;
    [SerializeField] private GameObject connectionPopup;
    [SerializeField] private GameObject connectionSuccessfulPopup;
    [SerializeField] private GameObject internetConnectionPopup;
    [SerializeField] private GameObject successSend;
    
    [SerializeField] private GameObject itemLegend;


    public Text fileNameInput;

    string path;

    static NetworkClient client;
    public string ipAddress;
    public Text ipAddressInput;

    short messageIDSuccess = 1000;
    short messageIDReceived = 2000;

    // Start is called before the first frame update
    void Start()
    {
        noFilePopup.SetActive(false);
        createFilePopup.SetActive(false);
        noIpAddressPopup.SetActive(false);
        connectionPopup.SetActive(false);
        internetConnectionPopup.SetActive(false);
        successSend.SetActive(false);
        connectionSuccessfulPopup.SetActive(false);

        getFilesFromSystem();

        if(Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.Log("Error. Check internet connection!");
            internetConnectionPopup.SetActive(true);
        }

        

        if(ConnectTo.input == null) {
            NoIpAddressPopUp();
        } else {
            ipAddress = ConnectTo.input;
            client = new NetworkClient();
            client.Connect(ipAddress, 50000);
            client.RegisterHandler (messageIDSuccess, ConnectionSuccessfull);
            client.RegisterHandler (messageIDReceived, OnMessageReceivedInServer);
        }
       
    }

    public void sendServerList() {

        if (!client.isConnected) {
            Debug.Log("Not Connected!");
            connectionPopup.SetActive(true);
            return;
        }

        string serverIP = ConnectTo.input;

        List<string> jsonStrings = new List<string>();
        SerializableList<Item> listToSend = new SerializableList<Item>();
        string filePath = path + "/" + selectedFileInfo.fileName;

        foreach (string line in System.IO.File.ReadLines(filePath))
        {  
            jsonStrings.Add(line);
        }  
        foreach(string item in jsonStrings) {
            listToSend.items.Add(JsonUtility.FromJson<Item>(item));
        } 

        var listInJson = JsonUtility.ToJson(listToSend);

        StringMessage msg = new StringMessage();
        msg.value = listInJson;

        client.Send(666, msg);

    }

    void ConnectionSuccessfull(NetworkMessage netMessage)
    {
        var objectMessage = netMessage.ReadMessage<StringMessage>();

        Debug.Log("Connected!");
        connectionSuccessfulPopup.SetActive(true);

    }

    void OnMessageReceivedInServer(NetworkMessage netMessage)
    {
        var objectMessage = netMessage.ReadMessage<StringMessage>();

        successSend.SetActive(true);

    }

    public void getFilesFromSystem() {
        path = Application.persistentDataPath + "/SavedLists";
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
        selectedFileInfo = file;
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
    }

    public void OkayNoInternetConnection() {
        internetConnectionPopup.SetActive(false);
        SceneManager.LoadScene("menu");
    }

    public void OkayConnectionSuccesful() {
        connectionSuccessfulPopup.SetActive(false);
    }

    public void GoBack() {
        client.Disconnect();
        SceneManager.LoadScene("menu");
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
    }


    public void CreateFileCancelButtonOnClick() {
        createFilePopup.SetActive(false);
        
    }

    public void CreateFileConfirmButtonOnClick() {
        string filePath = path + "/" + fileNameInput.text.ToString() + ".json";
        FileNameInformation.path = filePath;
        SceneManager.LoadScene("WorkAreaScene");
    }

    public void NoIpAddressPopUp() {
        noIpAddressPopup.SetActive(true);
    }

    public void NoIpAddressPopUpCancelClick() {
        noIpAddressPopup.SetActive(false);
        SceneManager.LoadScene("menu");
    }

    public void CancelConnection() {
        connectionPopup.SetActive(false);
        SceneManager.LoadScene("menu");
    }

    public void TryAgainConnection() {
        connectionPopup.SetActive(false);
        sendServerList();
    }

    public void NoIpAddressPopUpDoneClick() {
        noIpAddressPopup.SetActive(false);

        ipAddress = ipAddressInput.text;  
        ConnectTo.input = ipAddress;
        client = new NetworkClient();
        client.Connect(ipAddress, 50000);  
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

[System.Serializable]
public class SerializableList<Item>{
    public List<Item> items;

    public SerializableList() {
        items = new List<Item>();
    }
}
