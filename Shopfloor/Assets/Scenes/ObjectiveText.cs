using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveText : MonoBehaviour
{
    public Text txt;

    // Update is called once per frame
    void Update()
    {
        txt.text = ScoreManager.instance.ObjectiveText();
    }
}
