using System;
using UnityEngine;
using UnityEngine.UI;

public class ScoreMessage : MonoBehaviour
{

    public Text feedbackText;

    int score = 70; //ir buscar ao mundo 3d atraves do server
    string name ="Maria";
    // Start is called before the first frame update
    void Start()
    {	
	GameObject.Find("Text").GetComponent<Text>().text=score.ToString()+" pts";
	GameObject.Find("Trainee_Name").GetComponent<Text>().text=name;
        feedbackText = GameObject.Find("FeedbackMessage").GetComponent<Text>();
        if (score >= 90) { 
            feedbackText.text = "Good Job !";
        } else if(score > 50)
        {
            feedbackText.text = "You're in a good path !";
        } else
        {
            feedbackText.text = "Keep Working !";
        }
	loadScores();
    }

    void loadScores()
	{
	GameObject.Find("Text1").GetComponent<Text>().text="10 pts";
	GameObject.Find("Trainee_Name1").GetComponent<Text>().text="Bob";
	}
}
