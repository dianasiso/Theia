using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    public Transform theDest;

    void OnMouseDown()
    {
        Debug.Log("Camera: " + Camera.current);
        // GetComponent<Collider>().enabled = false;
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().freezeRotation = true;
        this.transform.position = theDest.position;
        this.transform.parent = GameObject.Find("PlayerCam").transform;
        Debug.Log(this.transform.parent);
    }

    void OnMouseUp() {
        this.transform.parent = null;
        // GetComponent<Collider>().enabled = true;
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Rigidbody>().freezeRotation = false;
    }
}
