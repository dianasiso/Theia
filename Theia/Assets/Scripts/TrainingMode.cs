using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrainingMode : MonoBehaviour
{
    public void goToSceneTrainingMode(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

}
