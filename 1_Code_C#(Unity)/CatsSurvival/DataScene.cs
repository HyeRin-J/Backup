using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataScene : MonoBehaviour
{
    void Start()
    {
        GameManager.Instance.LoadNextScene("TitleScene");
    }
}
