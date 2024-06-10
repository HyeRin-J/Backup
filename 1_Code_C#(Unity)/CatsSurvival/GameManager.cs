using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    public bool isSceneLoading = false;

    public bool isClickEscapeButton = false;

    private void Awake()
    {
        if (Instance != null)
        {
            if (initialized == false)
                DestroyImmediate(gameObject);
        }

        DontDestroyOnLoad(this);

        Application.targetFrameRate = 60;
        Time.timeScale = 1;
    }

    public void LoadNextScene(string sceneName)
    {
        isSceneLoading = true;

        LoadSceneManager.LoadScene(sceneName);
    }
}
