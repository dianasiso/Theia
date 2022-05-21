using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreMessage : MonoBehaviour
{

    public Text feedbackText;

    int score = 90;

    // Start is called before the first frame update
    void Start()
    {
        //feedbackText = GameObject.Find("FeedbackMessage").GetComponent<Text>();
        if (score >= 90) { 
            feedbackText.text = "Good Job !";
        } else if(score > 50)
        {
            feedbackText.text = "You're in a good path !";
        } else
        {
            feedbackText.text = "Keep Working !";
        }
    }
}
