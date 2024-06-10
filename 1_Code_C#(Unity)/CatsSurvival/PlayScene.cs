using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayScene : SingletonMonoBehaviour<PlayScene>
{
    public Player player;

    public float gameTime;
    public float maxGameTime = 60 * 1;

    public int killCount = 0;
    public int level;
    public int exp;
    public int nextExp = 10;

    public TMP_Text killCountText;
    public Slider expSlider;
    public TMP_Text timerText;
    public TMP_Text levelText;

    public float expRange = 2;

    private void Update()
    {
        gameTime += Time.deltaTime;

        if (gameTime >= maxGameTime)
        {
            Debug.Log("Game Clear");
        }

        timerText.text = string.Format("{0:D2}:{1:D2}", (int)(gameTime / 60), (int)(gameTime % 60));
    }

    public void KillMonster()
    {
        killCount++;
        killCountText.text = string.Format("Kill : {0}", killCount);
    }

    public void AddExp(int exp)
    {
        this.exp += exp;

        if (this.exp >= nextExp)
        {
            this.exp = 0;
            level++;
            nextExp += 10 * (level + 1);
            levelText.text = string.Format("Lv : {0}", level);
            expSlider.value = 0;
        }

        expSlider.value = Mathf.Min(this.exp / (float)nextExp, 1);
    }

}
