using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScene : MonoBehaviour
{
    public void OnClickTitleButton()
    {
        GameManager.Instance.LoadNextScene("MainScene");
    }
}
