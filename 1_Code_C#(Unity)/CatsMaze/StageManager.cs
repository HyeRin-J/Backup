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

            //스테이지 클리어시 보상테이블에 포함된 보상(재료)를 랜덤으로 해당 값 * 클리어 별의 갯수 만큼 지급
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
            GameManager.Instance.scriptManager.SetCurrentCondition(Conditions.첫_패스);
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
        GameManager.Instance.scriptManager.SetCurrentCondition(Conditions.첫_구출);
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

            //5분에 한번씩 실행
            if (time >= 300.0f)
            {
                GameManager.Instance.spawner.ApplyCityThemeEffect();

                time = 0.0f;

                //맵에 있는 적에게 1/3 확률로 체력 50%감소
                //2/3 확률로 체력 20% 회복
                cityBellCathedral.SetActive(true);
                isOn = true;

                yield return new WaitForSeconds(3);
            }

            yield return null;
        }
    }

    List<Vector3Int> floorPositions = new List<Vector3Int>();

    //모래 늪 타일에 빠진 몬스터와 고양이는 7초동안 이동 불가 상태에 빠진다.
    //페이즈 시작 시 방 영역을 제외한  "map3 base tile - desert sand" 타일 중 1/5(rounddown)에 모래 늪이 생성, 40초 후 소멸
    //update: 40초의 주기로 모든 모래 늪 제거 & 위의 조건으로 모래늪 생성
    //페이즈 종료 시 모든 모래늪 제거
    //모래 늪 지점에 몬스터/고양이가 빠지면 해당 모래 늪 소멸
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

    //각 페이즈 시작 전에 volcanicstone 오브젝트 생성
    //각 페이즈 시작 전 방 영역과 오브젝트를 제외한 바닥/데코 영역의 1/20(rounddown)에 랜덤으로 volcanicstone 오브젝트가 생성된다.
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
