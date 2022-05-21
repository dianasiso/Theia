using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class TrainingModeServerController : MonoBehaviour
{

    [SerializeField] Image circleImg;
    [SerializeField] [Range(0,1)] float progress = 0f;
    bool turn = true;
    bool loadImage = false;

    [SerializeField] private GameObject liveListPopup;
    [SerializeField] private GameObject listReceivedPopup;

    short messageIDSuccess = 1000;
    short messageIDReceived = 2000;

    // Start is called before the first frame update
    void Start()
    {
        liveListPopup.SetActive(false);
        listReceivedPopup.SetActive(false);
    }

    public void LiveListClick() {
        loadImage = true;
        liveListPopup.SetActive(true);
        NetworkServer.Listen(50000);
        NetworkServer.RegisterHandler(666, receiveMessageClient);
        NetworkServer.RegisterHandler (MsgType.Connect, OnClientConnected);
    }

    private void receiveMessageClient(NetworkMessage message) {

        StringMessage msg = new StringMessage();
        msg.value = message.ReadMessage<StringMessage>().value;

        string itemList = msg.value;

        Debug.Log(itemList);

        List<Item> items = JsonUtility.FromJson<SerializableList<Item>>(itemList).items;

        foreach(Item item in items) {
            Debug.Log(item.itemName + " " + item.itemQuantity);
        }

        liveListPopup.SetActive(false);
        listReceivedPopup.SetActive(true);

        StringMessage messageToSend = new StringMessage();
        messageToSend.value = "Message received!";
        NetworkServer.SendToClient(message.conn.connectionId, messageIDReceived,messageToSend);
    }

    public void CloseSuccesfulReceive() {
        NetworkServer.Shutdown();
        listReceivedPopup.SetActive(false);
    }

    public void CloseWaiting() {
        NetworkServer.Shutdown();
        liveListPopup.SetActive(false);
    }

    void OnClientConnected(NetworkMessage netMessage)
    {
        StringMessage messageToSend = new StringMessage();
        messageToSend.value = "Connection made successfully!";
        NetworkServer.SendToClient(netMessage.conn.connectionId,messageIDSuccess,messageToSend);
    }


    private void GoToVE() {

    }

    private void LoadAnimation() {
        circleImg.fillAmount = progress;
        if(turn) {
            progress = progress + 0.005f;
        } else {
            progress = progress - 0.005f;
        }
        if (Mathf.Floor(progress) == 1f) {
            turn = !turn;
            circleImg.fillClockwise = !circleImg.fillClockwise;
        }

        if (progress < 0f) {
            turn = !turn;
            circleImg.fillClockwise = !circleImg.fillClockwise;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(loadImage) {
            LoadAnimation();
        }

    }
}

