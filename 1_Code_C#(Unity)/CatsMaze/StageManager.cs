using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class StageInfoData
{
    public string ID;
    public int chapter;
    public int stage;
    public string title;
    public string description;
    public int reward_table;
    public int reward_value;
    public int reward_catnip;
    public string[] Phase;
    public int Give_Erg;
    public bool Erg_infinty;
    public bool Special_Deck_mode;
    public int Stage_Heart;
    public int total_rescue_cat;
}
[System.Serializable]
public class StageInfoClass
{
    public List<StageInfoData> stageInfo;
}


public class StageManager : MonoBehaviour
{
    public MapManager mapManager;
    public StageUIManager stageUiManager;

    public bool isProgressWave = false;

    public int maxPhase = 3;
    public int currentPhase = 1;

    public int StageHP = 5;

    public int clearStar = 3;

    Imagery currentImagery;

    public GameObject cityBellCathedral;
    public TileBase sandQuick;
    public AnimatedTile volcanicStone;

    public StageInfoData stageInfo;

    public Sprite stageRewardImage;

    private void Awake()
    {
        stageInfo = DataManager.Instance.GetStageInfoData();
        MessageQueue.Instance.AttachListener(typeof(PhaseStartMessage), PhaseStart);
    }

    private void OnDestroy()
    {
        MessageQueue.Instance.DetachListener(typeof(PhaseStartMessage), PhaseStart);
    }

    public void CheckPhaseClear()
    {
        isProgressWave = false;
        mapManager.aStar.isLoadFinish = false;
        GameManager.Instance.isProgressing = isProgressWave;

        if (currentPhase >= maxPhase)
        {
            UserData.Instance.AddRewardAmount("RW_Catnip", stageInfo.reward_catnip * clearStar);

            RewardTable rewardTable = DataManager.Instance.GetRewardTables(stageInfo.reward_table);

            //�������� Ŭ����� �������̺� ���Ե� ����(���)�� �������� �ش� �� * Ŭ���� ���� ���� ��ŭ ����
            int randomIndex = Random.Range(0, rewardTable.Matter_list.Length);

            stageRewardImage = DataManager.Instance.GetRewardAndMatterSprite(rewardTable.Matter_list[randomIndex]);
            UserData.Instance.AddMatterAmount(rewardTable.Matter_list[randomIndex], stageInfo.reward_value * clearStar);

            GameManager.Instance.isBeforeScene = false;

            MessageQueue.Instance.QueueMesssage(new PhaseClearMessage(false));
        }
        else
        {
            if (currentPhase < maxPhase)
            {
                currentPhase++;
                mapManager.ChangeNextPhase();
            }

            GameManager.Instance.scriptManager.PhaseStartInitValues();

            MessageQueue.Instance.QueueMesssage(new PhaseClearMessage());
        }
    }

    public bool PhaseStart(BaseMessage msg)
    {
        if (msg == null)
        {
            Debug.Log("HandleYourMesssageListener : Message is null!");
            return false;
        }

        PhaseStartMessage castMsg = msg as PhaseStartMessage;

        if (castMsg == null)
        {
            Debug.Log("HandleYourMesssageListener : Cast Message is null!");
            return false;
        }

        Debug.Log(string.Format("HandleYourMesssageListener : Got the message! - {0}", castMsg.name));

        CheckMapTheme();

        return true;
    }

    public void EnemyReachGoal()
    {
        StageHP -= 1;

        if (StageHP <= 0)
        {
            isProgressWave = false;
            GameManager.Instance.isProgressing = isProgressWave;
            MessageQueue.Instance.QueueMesssage(new StageFailMessage());
        }
        else
        {
            GameManager.Instance.scriptManager.SetCurrentCondition(Conditions.ù_�н�);
        }
    }

    public void NPCIsDead()
    {
        StageHP -= 1;

        if (StageHP <= 0)
        {
            isProgressWave = false;
            GameManager.Instance.isProgressing = isProgressWave;
            MessageQueue.Instance.QueueMesssage(new StageFailMessage());
        }
    }

    public void NPCReachGoal()
    {
        GameManager.Instance.scriptManager.SetCurrentCondition(Conditions.ù_����);
    }

    // City, Desert, Night, Volcano
    public void CheckMapTheme()
    {
        currentImagery = mapManager.mapTheme;

        switch (currentImagery)
        {
            case Imagery.City:
                StartCoroutine(CityTheme());
                break;
            case Imagery.Desert:
                StartCoroutine(DesertTheme());
                break;
            case Imagery.Night:
                GameManager.Instance.spawner.ApplyNightSpeedDecrease();
                StartCoroutine(NightTheme());
                break;
            case Imagery.Volcano:
                VolcanTheme();
                break;
        }
    }

    IEnumerator CityTheme()
    {
        float time = 0.0f;

        bool isOn = false;

        while (isProgressWave)
        {
            time += Time.deltaTime;

            if (isOn) cityBellCathedral.SetActive(false);

            //5�п� �ѹ��� ����
            if (time >= 300.0f)
            {
                GameManager.Instance.spawner.ApplyCityThemeEffect();

                time = 0.0f;

                //�ʿ� �ִ� ������ 1/3 Ȯ���� ü�� 50%����
                //2/3 Ȯ���� ü�� 20% ȸ��
                cityBellCathedral.SetActive(true);
                isOn = true;

                yield return new WaitForSeconds(3);
            }

            yield return null;
        }
    }

    List<Vector3Int> floorPositions = new List<Vector3Int>();

    //�� �� Ÿ�Ͽ� ���� ���Ϳ� ����̴� 7�ʵ��� �̵� �Ұ� ���¿� ������.
    //������ ���� �� �� ������ ������  "map3 base tile - desert sand" Ÿ�� �� 1/5(rounddown)�� �� ���� ����, 40�� �� �Ҹ�
    //update: 40���� �ֱ�� ��� �� �� ���� & ���� �������� �𷡴� ����
    //������ ���� �� ��� �𷡴� ����
    //�� �� ������ ����/����̰� ������ �ش� �� �� �Ҹ�
    IEnumerator DesertTheme()
    {
        float time = 0.0f;

        while (isProgressWave)
        {
            if (time <= 0f)
            {
                var tileList = mapManager.aStar.roomDetection.allTiles.Values;

                foreach (var tile in tileList)
                {
                    if (tile.savedTile.tileName.Equals("map3 base tile - desert sand")
                        && tile.room == null
                        && tile.isWallOrDoor == false
                        && tile.isUnitOn == false)
                    {
                        floorPositions.Add(tile.savedTile.position);
                    }
                }

                for (int i = 0; i < Mathf.CeilToInt(floorPositions.Count / 20.0f); i++)
                {
                    Vector3Int randomPosition = floorPositions[Random.Range(0, floorPositions.Count)];
                    mapManager.SetTile(randomPosition, sandQuick, false);
                }

                time = 40.0f;
            }

            yield return null;
        }
    }

    public void ResetDesetTile(Vector3Int pos)
    {
        mapManager.SetTile(pos, null, false);

        mapManager.aStar.roomDetection.SetTileName(pos);

        Vector3Int randomPosition = floorPositions[Random.Range(0, floorPositions.Count)];
        mapManager.SetTile(randomPosition, sandQuick, false);
    }

    IEnumerator NightTheme()
    {
        float time = 15.0f;

        while (isProgressWave)
        {
            time -= Time.deltaTime;

            if (time > 5.0f)
            {
                GameManager.Instance.spawner.ApplyNightSpeedDecrease();

            }
            else
            {
                GameManager.Instance.spawner.ApplyNightThemeEffect();

                if (time > 0)
                {

                }
            }
            if (time <= 0.0f)
                time = 15;

            yield return null;
        }
    }

    //�� ������ ���� ���� volcanicstone ������Ʈ ����
    //�� ������ ���� �� �� ������ ������Ʈ�� ������ �ٴ�/���� ������ 1/20(rounddown)�� �������� volcanicstone ������Ʈ�� �����ȴ�.
    void VolcanTheme()
    {
        var tileList = mapManager.aStar.roomDetection.allTiles.Values;

        foreach (var tile in tileList)
        {
            if (tile.room == null && tile.isWallOrDoor == false && tile.isUnitOn == false)
            {
                floorPositions.Add(tile.savedTile.position);
            }
        }

        for (int i = 0; i < floorPositions.Count / 20; i++)
        {
            Vector3Int randomPosition = floorPositions[Random.Range(0, floorPositions.Count)];
            mapManager.SetTile(randomPosition, volcanicStone, true);
        }
    }
}
