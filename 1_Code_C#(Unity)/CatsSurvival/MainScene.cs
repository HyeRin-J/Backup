using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScene : MonoBehaviour
{
    public void OnClickPlayButton()
    {
        GameManager.Instance.LoadNextScene("PlayScene");
    }

    public void OnClickExitButton()
    {
        Application.Quit();
    }
}
