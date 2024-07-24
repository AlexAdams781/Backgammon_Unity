using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenes : MonoBehaviour
{
    public void PlayGame()
    {
        Debug.Log("here");
        SceneManager.LoadSceneAsync(1);
    }
}
