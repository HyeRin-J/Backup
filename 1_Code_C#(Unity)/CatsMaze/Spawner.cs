using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Mesh;

[System.Serializable]
public class PhaseRushInfoData
{
    public string Phase_Id;
    public int rescue_cat;
    public string[] Rush_list;
}

[System.Serializable]
public class PhaseRushInfoClass
{
    public List<PhaseRushInfoData> PhaseRushData;
}

[System.Serializable]
public class RushData
{
    public string ID;
    public string[] wavelist;   //해당 공세에 포함될 웨이브들의 리스트
    public int starttime; //웨이브 시작시간(초)
    public int endtime;   //웨이브 종료시간(초)
    public WaveRushType rushType;
}

[System.Serializable]
public class RushClass
{
    public List<RushData> rushData;
}

public enum WaveRushType
{
    Random, Order, SameTime
}

[System.Serializable]
public class WaveData
{
    public string wave_id;
    public string[] include_wave; //해당 웨이브도 같이 참조함, 0일 경우 포함되는 웨이브 없음
    public int delay; //딜레이, 몬스터가 출현하는 간격(초)
    public string[] monster; // 해당 웨이브에 포함된 몬스터 리스트
    public WaveRushType wave_type;
}

[System.Serializable]
public class WaveClass
{
    public List<WaveData> waveData;
}
public class Spawner : MonoBehaviour
{
    public AStar aStar;
    public StageManager stageManager;

    public List<List<RushData>> rushData = new();
    public List<WaveData> monsterWaveData = new();
    int orderWaveIndex = 0;
    int rushIndex = 0;

    public int totalRescueCatCount;

    public WaveData currentWaveData;

    public float spawnTimer = 0.0f;
    public float rushTimer = 0.0f;

    public List<GameObject> monsters = new();
    public List<GameObject> NPCs = new();

    public AudioSource audioSource;
    public AudioClip[] attackSounds;

    public bool firstSpawn = false;
    public bool fistCatSpawn = false;
    public int inCountEnemy = 0;
    public int reachCatCount = 0;

    public bool isEnemyReachGoal = false;

    public TMPro.TMP_Text timerText;
    public float timer;

    public void Start()
    {
        SetCurrentStageSpawnData();

        GameManager.Instance.spawner = this;
    }

    IEnumerator SpawnMonsterCoroutine(string monsterID)
    {
        if (monsterID.Contains("Cat"))
        {
            if (!fistCatSpawn && GameManager.Instance.stageNum == 2 &&
                !GameManager.Instance.isTutorialFinish[2] &&
                GameManager.Instance.tutorialManager != null &&
                GameManager.Instance.tutorialManager.tutorialStep == 6)
            {
                fistCatSpawn = true;
                stageManager.mapManager.cameraManager.SetActiveCamera(0);
                GameManager.Instance.tutorialManager.ShowTutorialMessage();
            }

            GameObject npcPrefab = DataManager.Instance.GetNPCPrefab(monsterID);
            if (npcPrefab != null)
            {
                GameObject npcObj = Instantiate(DataManager.Instance.GetNPCPrefab(monsterID));
                npcObj.transform.position = aStar.ObjstacleCellToWorld(aStar.startPos) + new Vector3(0.5f, 0);
                npcObj.transform.parent = transform;
                npcObj.GetComponent<NPC>().Init(aStar, this);
                NPCs.Add(npcObj);
            }

            yield return new WaitForSecondsRealtime(0.5f);
        }
        else
        {
            bool monsterType = DataManager.Instance.GetMonsterData(monsterID).Specialty;

            if (monsterType)
                GameManager.Instance.scriptManager.SetCurrentCondition(Conditions.보스_출현);
            else GameManager.Instance.scriptManager.SetCurrentCondition(Conditions.몹_출현);

            GameObject monsterPrefab = DataManager.Instance.GetMonsterPrefab(monsterID);
            if (monsterPrefab != null)
            {
                GameObject monsterObj = Instantiate(monsterPrefab);
                monsterObj.transform.position = aStar.ObjstacleCellToWorld(aStar.startPos) + new Vector3(0.5f, 0);
                monsterObj.transform.parent = transform;

                Monster monsterComp = monsterObj.GetComponent<Monster>();
                monsterComp.aStar = aStar;
                monsterComp.monsterSpawner = this;

                monsterObj.name = monsterID + monsterComp.monsterData.Korean_name;

                monsters.Add(monsterObj);
            }

            yield return new WaitForSecondsRealtime(0.5f);
        }
    }

    IEnumerator CheckWave(WaveData waveData)
    {
        Queue<string> spawnMonster = new();
        List<string> monsterID = DataManager.Instance.GetMonsterInWaveList(waveData.wave_id);

        int orderIndex = 0;

        switch (waveData.wave_type)
        {
            case WaveRushType.Random:
                int randomIndex = Random.Range(0, monsterID.Count);
                if (randomIndex < monsterID.Count)
                {
                    spawnMonster.Enqueue(monsterID[randomIndex]);
                    monsterID.RemoveAt(randomIndex);
                }
                break;
            case WaveRushType.Order:
                if (orderIndex < monsterID.Count)
                    spawnMonster.Enqueue(monsterID[orderIndex++]);
                break;
            case WaveRushType.SameTime:
                for (int i = 0; i < monsterID.Count; ++i)
                {
                    spawnMonster.Enqueue(monsterID[i]);
                }
                monsterID.Clear();
                break;
        }

        while (spawnMonster.Count > 0)
        {
            if (spawnTimer <= 0)
            {
                switch (waveData.wave_type)
                {
                    case WaveRushType.Random:
                        int randomIndex = Random.Range(0, monsterID.Count);
                        if (randomIndex < monsterID.Count)
                        {
                            spawnMonster.Enqueue(monsterID[randomIndex]);
                            monsterID.RemoveAt(randomIndex);
                        }
                        break;
                    case WaveRushType.Order:
                        if (orderIndex < monsterID.Count)
                            spawnMonster.Enqueue(monsterID[orderIndex++]);
                        break;
                }

                string monster = spawnMonster.Dequeue();
                StartCoroutine(SpawnMonsterCoroutine(monster));

                spawnTimer = waveData.delay;
            }

            spawnTimer -= Time.deltaTime;

            yield return null;
        }

        if (rushTimer < rushData[stageManager.currentPhase - 1][rushIndex].endtime)
            RushStart();
    }

    public void RushStart()
    {
        RushData currentPhaseRushData = rushData[stageManager.currentPhase - 1][0];

        switch (currentPhaseRushData.rushType)
        {
            case WaveRushType.Random:
                int randomIndex = Random.Range(0, monsterWaveData.Count);
                if (randomIndex < monsterWaveData.Count)
                {
                    currentWaveData = monsterWaveData[randomIndex];
                    monsterWaveData.RemoveAt(randomIndex);
                }
                break;
            case WaveRushType.Order:
                if (orderWaveIndex < monsterWaveData.Count)
                    currentWaveData = monsterWaveData[orderWaveIndex++];
                break;
            case WaveRushType.SameTime:
                for (int i = 0; i < currentPhaseRushData.wavelist.Length; ++i)
                {
                    StartCoroutine(CheckWave(monsterWaveData[i]));
                }
                break;
        }

        if (currentPhaseRushData.rushType != WaveRushType.SameTime && currentWaveData != null)
            StartCoroutine(CheckWave(currentWaveData));
    }


    public void SetCurrentStageSpawnData()
    {
        int chapterNum = GameManager.Instance.chapterNum;
        int stageNum = GameManager.Instance.stageNum;

        string[] phaseList = stageManager.stageInfo.Phase;

        foreach (var list in phaseList)
        {
            rushData.Add(DataManager.Instance.GetRushData(list));
        }

        totalRescueCatCount = DataManager.Instance.GetStageRescueCat();

        foreach (var list in rushData[stageManager.currentPhase - 1])
        {
            monsterWaveData.AddRange(DataManager.Instance.GetWaveData(list.wavelist));
        }
    }

    public void FixedUpdate()
    {
        if (GameManager.Instance.isProgressing)
        {
            if (!firstSpawn)
            {
                firstSpawn = true;
                RushStart();
            }

            rushTimer += Time.deltaTime;
            timerText.text = string.Format("{0:0.00}", rushTimer);

            if (currentWaveData != null)
            {
                if (rushTimer >= rushData[stageManager.currentPhase - 1][rushIndex].endtime)
                {
                    if (monsters.Count == 0 && NPCs.Count == 0)
                    {
                        stageManager.CheckPhaseClear();
                        firstSpawn = false;
                        spawnTimer = 0;
                        rushTimer = 0.0f;
                    }
                }

                if (rushIndex >= rushData[stageManager.currentPhase - 1].Count)
                {
                    rushIndex++;
                }
            }
        }

        if (GameManager.Instance.isProgressing && rushTimer > rushData[stageManager.currentPhase - 1][rushIndex].endtime &&
            (monsterWaveData.Count == 0 && monsters.Count == 0 && NPCs.Count == 0))
        {
            spawnTimer = 0;
            rushTimer = 0.0f;
            firstSpawn = false;
            timerText.text = string.Format("{0:0.00}", rushTimer);
            stageManager.CheckPhaseClear();
        }

        //임시 스테이지 클리어 키
        if(Input.GetKeyDown(KeyCode.Z))
        {
            stageManager.currentPhase = stageManager.maxPhase;
            stageManager.CheckPhaseClear();
        }
    }

    public void DeleteEnemy(GameObject enemy)
    {
        monsters.Remove(enemy);
        Destroy(enemy);
    }

    public void DeleteNPC(GameObject NPC)
    {
        NPC npc = NPC.GetComponent<NPC>();

        reachCatCount++;

        if (!npc.isDead)
        {
            MessageQueue.Instance.QueueMesssage(new AddErgCostMessage(npc.npcData.Reward));

            if (reachCatCount >= totalRescueCatCount) GameManager.Instance.scriptManager.SetCurrentCondition(Conditions.마지막_구출);
            else stageManager.NPCReachGoal();
        }
        else
        {
            stageManager.NPCIsDead();
        }

        NPCs.Remove(NPC);
        Destroy(NPC);
    }

    public void PlayRandom()
    {
        audioSource.PlayOneShot(attackSounds[UnityEngine.Random.Range(0, attackSounds.Length)]);
    }

    public void ApplyCityThemeEffect()
    {
        foreach (var mon in monsters)
        {
            Monster comp = mon.GetComponent<Monster>();
            if (comp.isfanatic)
            {
                comp.Recovery();
            }
            else
            {
                comp.Damaged(Mathf.RoundToInt(comp.monsterData.HP * 0.5f), false);
            }
        }
    }

    public void ApplyNightSpeedDecrease()
    {
        foreach (var mon in monsters)
        {
            Monster comp = mon.GetComponent<Monster>();

            comp.speed = comp.monsterData.MoveSpeed * 0.7f;
        }
    }

    public void ApplyNightThemeEffect()
    {
        foreach (var mon in monsters)
        {
            Monster comp = mon.GetComponent<Monster>();

            comp.speed = comp.monsterData.MoveSpeed;
        }
    }

}
