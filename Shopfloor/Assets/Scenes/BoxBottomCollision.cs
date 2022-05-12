using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxBottomCollision : MonoBehaviour
{
    GameObject prevObj;
    bool alreadyCollided;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == prevObj)
        {
            alreadyCollided = true;
        }
        else
        {
            alreadyCollided = false;
            prevObj = collision.gameObject;
        }
        if (!alreadyCollided)
            ScoreManager.instance.HandleObjInBox(collision.gameObject);
    }

    void Start()
    {
        alreadyCollided = false;
    }
}
