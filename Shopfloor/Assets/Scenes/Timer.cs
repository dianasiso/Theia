using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public float time;
    public Text txt;

    // Start is called before the first frame update
    void Start()
    {
        time = 0;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        int mins = (int) time / 60;
        int secs = (int) (time % 60);
        txt.text = $"{mins}:{secs}";
    }
}
