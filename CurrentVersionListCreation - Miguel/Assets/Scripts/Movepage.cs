using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Movepage : MonoBehaviour
{
    [SerializeField] private GameObject notAllowed;

    void Start()
    {
        notAllowed.SetActive(false);
    }

    public void goTo(string scene){
	    SceneManager.LoadScene(scene);
    }

    public void goToTrainerMode(){
        if (Application.platform == RuntimePlatform.Android
        || Application.platform == RuntimePlatform.WindowsEditor 
        || Application.platform == RuntimePlatform.OSXEditor) {
            SceneManager.LoadScene("FileScene");
        }  else {
            notAllowed.SetActive(true);
        }
    }

    public void goToTraineeMode(){
        if (Application.platform == RuntimePlatform.WindowsPlayer
        || Application.platform == RuntimePlatform.OSXPlayer 
        || Application.platform == RuntimePlatform.WindowsEditor 
        || Application.platform == RuntimePlatform.OSXEditor ) {
            SceneManager.LoadScene("ModeMenuScene");
        } else {
            notAllowed.SetActive(true);
        }

    }

    public void OkayNotAllowedButton() {
        notAllowed.SetActive(false);
    }
}
