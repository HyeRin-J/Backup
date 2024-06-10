using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using DG.Tweening.Plugins;
using JetBrains.Annotations;
using static UnityEngine.Rendering.DebugUI;

[System.Serializable]
public class StadingSprite
{
    public CharacterName name;
    public Sprite[] sprite;
}

[System.Serializable]
public class RewardTable
{
    public string ID;
    public string[] Matter_list;
}

[System.Serializable]
public class RewardTableList
{
    public List<RewardTable> rewardTable;
}

public class DataManager : SingletonMonoBehaviour<DataManager>
{
    [System.Serializable]
    public class UnitPrefabs
    {
        public string ID;
        public Sprite UnitSprite;
        public GameObject UnitPrefab;
    }

    [System.Serializable]
    public class MonsterPrefabs
    {
        public string ID;
        public GameObject MonsterPrefab;
    }

    [System.Serializable]
    public class MonsterSprites
    {
        public string ID;
        public Sprite monsterSprite;
    }

    [System.Serializable]
    public class NPCPrefabs
    {
        public string ID;
        public Sprite npcSprite;
        public GameObject npcPrefab;
    }

    [System.Serializable]
    public class RewardImage
    {
        public string ID;
        public Sprite rewardSprite;
    }

    [System.Serializable]
    public class ArtifactImage
    {
        public string ID;
        public Sprite artifactImage;
    }

    [System.Serializable]
    public class MatterImage
    {
        public string ID;
        public Sprite matterImage;
    }

    public string[] characterNames = { "Mion", "Neijon", "Geros", "Matilda", "Arsha", "Donson", "Melissa", };

    public int[] stageCount = { 3, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8 };
    public bool[][] stageClear;

    public List<StadingSprite> stadingSprites;

    [Header("/DataSheets")]
    public List<UnitPrefabs> unitDatas;
    Dictionary<string, Sprite> unitSprite = new();
    Dictionary<string, GameObject> unitPrefab = new();
    public List<MonsterPrefabs> monsterDatas;
    public List<MonsterSprites> monsterSprites;
    Dictionary<string, Sprite> monsterSpriteDic = new();
    Dictionary<string, GameObject> monsterPrefab = new();
    public List<NPCPrefabs> npcDatas;

    [Header("Reward&ArtifactImages")]
    public List<RewardImage> rewardImages;
    public Dictionary<string, Sprite> rewardSprite = new();
    public List<ArtifactImage> artifactImages;
    public Dictionary<string, Sprite> artifactImageDic = new();
    public List<MatterImage> matterImages;
    public Dictionary<string, Sprite> matterImageDic = new();

    [Header("ScriptData")]
    Dictionary<string, ScriptDataList> scriptDataLists;

    [Header("BriefingData")]
    BriefingDataClass briefingDataClass;
    Dictionary<string, BriefingData> briefingDataDic = new();

    public enum Localize
    {
        Korean, English
    }

    [Header("StageInfo")]
    [SerializeField]
    StageInfoClass stageInfoClass = new();
    Dictionary<string, StageInfoData> stageInfoDic = new();
    [SerializeField]
    PhaseRushInfoClass phaseRushInfoClass = new();
    Dictionary<string, PhaseRushInfoData> phaseRushInfoDic = new();
    [SerializeField]
    RushClass rushClass = new();
    Dictionary<string, RushData> rushDic = new();
    [SerializeField]
    WaveClass waveClass = new();
    Dictionary<string, WaveData> waveDic = new();
    [SerializeField]
    RewardTableList rewardTableClass = new();
    Dictionary<string, RewardTable> rewardTableDic = new();

    [Header("ArtifactData")]
    [SerializeField]
    public ArtifactDataClass artifactDataClass;
    Dictionary<string, ArtifactData> artifactDataDic = new();
    [SerializeField]
    public ArtifactEffectDataClass artifactEffectDataClass;
    Dictionary<string, ArtifactEffectData> artifactEffectDataDic = new();
    [SerializeField]
    public MatterDataClass matterDataClass;
    Dictionary<string, MatterData> matterDataDic = new();

    [Header("MagicCardData")]
    [SerializeField]
    MagicCardDataClass magicCardDataClass;
    Dictionary<string, MagicCardData> magicCardDataDic = new();

    [Header("Unit Data")]
    [SerializeField]
    UnitDataClass unitDataClass;
    Dictionary<string, UnitData> unitDataDic = new();

    [Header("Monster Data")]
    [SerializeField]
    MonsterDataClass monsterDataClass;
    Dictionary<string, MonsterData> monsterDataDic = new();

    [Header("NPC Data")]
    [SerializeField]
    NPCDataClass npcDataClass;
    Dictionary<string, NPCData> npcDataDic = new();

    [Header("Skill Data")]
    [SerializeField]
    SkillDataClass skillDataClass;
    Dictionary<string, Skill> skillDataDic = new();

    [Header("Synergy Data")]
    [SerializeField]
    public Unit_Area_ImageryDataClass unitArea_ImageryDataClass = new();
    Dictionary<string, UnitArea_Imagery> unitArea_ImageryDataDic = new();
    [SerializeField]
    public Area_Area_ImageryDataClass areaArea_ImageryDataClass = new();
    Dictionary<string, AreaArea_Imagery> areaArea_ImageryDataDic = new();

    public static Localize localize = Localize.Korean;

    public enum emotions { Angry, Happy, Hate, Normal, Sad, Serious, Surprise, Think };
    public enum Direction { LeftUp, Up, RightUp, Left, Center, Right, LeftDown, Down, RightDown };

    //모든 정보를 다시 저장
    void Awake()
    {
        if (Instance != null)
        {
            if (initialized == false)
                DestroyImmediate(gameObject);
        }

        stageClear = new bool[13][];

        for (int i = 0; i < 13; i++)
        {
            stageClear[i] = new bool[stageCount[i]];
        }

        //보상 아이템 이미지
        for (int i = 0; i < rewardImages.Count; i++)
        {
            rewardSprite[rewardImages[i].ID] = rewardImages[i].rewardSprite;
        }

        //아티팩트 이미지
        for (int i = 0; i < artifactImages.Count; i++)
        {
            artifactImageDic[artifactImages[i].ID] = artifactImages[i].artifactImage;
        }

        //아티팩트 재료 이미지
        for (int i = 0; i < matterImages.Count; i++)
        {
            matterImageDic[matterImages[i].ID] = matterImages[i].matterImage;
        }

        //몬스터 이미지 Dictionary
        for (int i = 0; i < monsterSprites.Count; i++)
        {
            monsterSpriteDic[monsterSprites[i].ID] = monsterSprites[i].monsterSprite;
        }

        //몬스터 프리팹 Dictionary
        for (int i = 0; i < monsterDatas.Count; i++)
        {
            monsterPrefab[monsterDatas[i].ID] = monsterDatas[i].MonsterPrefab;
        }

        //Unit 이미지, 프리팹 Dictionary
        for (int i = 0; i < unitDatas.Count; i++)
        {
            unitSprite[unitDatas[i].ID] = unitDatas[i].UnitSprite;
            unitPrefab[unitDatas[i].ID] = unitDatas[i].UnitPrefab;
        }

        //브리핑 데이터
        string path = Application.streamingAssetsPath + "/Data/BriefingData.json";
        if (File.Exists(path))
        {
            string data = File.ReadAllText(path);
            briefingDataClass = JsonUtility.FromJson<BriefingDataClass>(data);
            for (int i = 0; i < briefingDataClass.data.Count; i++)
            {
                briefingDataDic[briefingDataClass.data[i].ID] = briefingDataClass.data[i];
            }
        }

        //스테이지 정보
        path = Application.streamingAssetsPath + "/Data/StageInfo/StageInfos.json";

        if (File.Exists(path))
        {
            string data = File.ReadAllText(path);
            stageInfoClass = JsonUtility.FromJson<StageInfoClass>(data);
            for (int i = 0; i < stageInfoClass.stageInfo.Count; i++)
            {
                stageInfoDic[stageInfoClass.stageInfo[i].ID] = stageInfoClass.stageInfo[i];
            }
        }

        //웨이브 정보
        path = Application.streamingAssetsPath + "/Data/StageInfo/PhaseRushData.json";
        if (File.Exists(path))
        {
            string data = File.ReadAllText(path);
            phaseRushInfoClass = JsonUtility.FromJson<PhaseRushInfoClass>(data);
            for (int i = 0; i < phaseRushInfoClass.PhaseRushData.Count; i++)
            {
                phaseRushInfoDic[phaseRushInfoClass.PhaseRushData[i].Phase_Id] = phaseRushInfoClass.PhaseRushData[i];
            }
        }

        path = Application.streamingAssetsPath + "/Data/StageInfo/RushData.json";
        if (File.Exists(path))
        {
            string data = File.ReadAllText(path);
            rushClass = JsonUtility.FromJson<RushClass>(data);
            for (int i = 0; i < rushClass.rushData.Count; i++)
            {
                rushDic[rushClass.rushData[i].ID] = rushClass.rushData[i];
            }
        }

        path = Application.streamingAssetsPath + "/Data/StageInfo/WaveData.json";
        if (File.Exists(path))
        {
            string data = File.ReadAllText(path);
            waveClass = JsonUtility.FromJson<WaveClass>(data);
            for (int i = 0; i < waveClass.waveData.Count; i++)
            {
                waveDic[waveClass.waveData[i].wave_id] = waveClass.waveData[i];
            }
        }

        //보상 정보
        path = Application.streamingAssetsPath + "/Data/StageInfo/RewardTable.json";
        if (File.Exists(path))
        {
            string data = File.ReadAllText(path);
            rewardTableClass = JsonUtility.FromJson<RewardTableList>(data);
            for (int i = 0; i < rewardTableClass.rewardTable.Count; i++)
            {
                rewardTableDic[rewardTableClass.rewardTable[i].ID] = rewardTableClass.rewardTable[i];
            }
        }

        //아티팩트 관련데이터
        path = Application.streamingAssetsPath + "/Data/ArtifactData/Artifact_Data.json";
        if (File.Exists(path))
        {
            string data = File.ReadAllText(path);
            artifactDataClass = JsonUtility.FromJson<ArtifactDataClass>(data);
            for (int i = 0; i < artifactDataClass.Artifact_Data.Count; i++)
            {
                artifactDataDic[artifactDataClass.Artifact_Data[i].ID] = artifactDataClass.Artifact_Data[i];
            }
        }

        path = Application.streamingAssetsPath + "/Data/ArtifactData/Artifact_Effect.json";
        if (File.Exists(path))
        {
            string data = File.ReadAllText(path);
            artifactEffectDataClass = JsonUtility.FromJson<ArtifactEffectDataClass>(data);
            for (int i = 0; i < artifactEffectDataClass.Artifact_Effect.Count; i++)
            {
                artifactEffectDataDic[artifactEffectDataClass.Artifact_Effect[i].ID] = artifactEffectDataClass.Artifact_Effect[i];
            }
        }

        path = Application.streamingAssetsPath + "/Data/ArtifactDataMatter_Data.json";
        if (File.Exists(path))
        {
            string data = File.ReadAllText(path);
            matterDataClass = JsonUtility.FromJson<MatterDataClass>(data);
            for (int i = 0; i < matterDataClass.Matter_Data.Count; i++)
            {
                matterDataDic[matterDataClass.Matter_Data[i].ID] = matterDataClass.Matter_Data[i];
            }
        }

        //마법카드 데이터
        path = Application.streamingAssetsPath + "/Data/MagicCardData.json";
        if (File.Exists(path))
        {
            string data = File.ReadAllText(path);
            magicCardDataClass = JsonUtility.FromJson<MagicCardDataClass>(data);
            for (int i = 0; i < magicCardDataClass.magicCardData.Count; i++)
            {
                magicCardDataDic[magicCardDataClass.magicCardData[i].cardID] = magicCardDataClass.magicCardData[i];
            }
        }

        //유닛 데이터
        path = Application.streamingAssetsPath + "/Data/UnitData.json";
        if (File.Exists(path))
        {
            string data = File.ReadAllText(path);
            unitDataClass = JsonUtility.FromJson<UnitDataClass>(data);

            for (int i = 0; i < unitDatas.Count; i++)
            {
                if (unitDatas[i].UnitPrefab != null)
                    unitDatas[i].UnitPrefab.GetComponent<Unit>().unitData = unitDataClass.UnitData[i];

                unitDataDic[unitDataClass.UnitData[i].ID] = unitDataClass.UnitData[i];
            }
        }

        //몬스터 데이터
        path = Application.streamingAssetsPath + "/Data/MonsterData.json";
        if (File.Exists(path))
        {
            string data = File.ReadAllText(path);
            monsterDataClass = JsonUtility.FromJson<MonsterDataClass>(data);

            for (int i = 0; i < monsterDatas.Count; i++)
            {
                if (monsterDatas[i].MonsterPrefab != null)
                    monsterDatas[i].MonsterPrefab.GetComponent<Monster>().monsterData = monsterDataClass.MonsterData[i];

                monsterDataDic[monsterDataClass.MonsterData[i].ID] = monsterDataClass.MonsterData[i];
            }
        }

        //NPC 데이터
        path = Application.streamingAssetsPath + "/Data/NPCData.json";
        if (File.Exists(path))
        {
            string data = File.ReadAllText(path);
            npcDataClass = JsonUtility.FromJson<NPCDataClass>(data);

            for (int i = 0; i < npcDatas.Count; i++)
            {
                if (npcDatas[i].npcPrefab != null)
                    npcDatas[i].npcPrefab.GetComponent<NPC>().npcData = npcDataClass.CatData[i];
            }
        }

        //스킬 데이터
        path = Application.streamingAssetsPath + "/Data/SkillData.json";
        if (File.Exists(path))
        {
            string data = File.ReadAllText(path);
            skillDataClass = JsonUtility.FromJson<SkillDataClass>(data);
            for (int i = 0; i < skillDataClass.SkillData.Count; i++)
            {
                skillDataDic[skillDataClass.SkillData[i].ID] = skillDataClass.SkillData[i];
            }
        }

        //시너지 관련
        path = Application.streamingAssetsPath + "/Data/ImageryData/Unit_Area_ImageryData.json";
        if (File.Exists(path))
        {
            string data = File.ReadAllText(path);
            unitArea_ImageryDataClass = JsonUtility.FromJson<Unit_Area_ImageryDataClass>(data);
            for (int i = 0; i < unitArea_ImageryDataClass.Unit_Area_ImageryData.Count; i++)
            {
                unitArea_ImageryDataDic[unitArea_ImageryDataClass.Unit_Area_ImageryData[i].ID] = unitArea_ImageryDataClass.Unit_Area_ImageryData[i];
            }
        }

        path = Application.streamingAssetsPath + "/Data/ImageryData/Area_Area_ImageryData.json";
        if (File.Exists(path))
        {
            string data = File.ReadAllText(path);
            areaArea_ImageryDataClass = JsonUtility.FromJson<Area_Area_ImageryDataClass>(data);
            for (int i = 0; i < areaArea_ImageryDataClass.Area_Area_ImageryData.Count; i++)
            {
                areaArea_ImageryDataDic[areaArea_ImageryDataClass.Area_Area_ImageryData[i].ID] = areaArea_ImageryDataClass.Area_Area_ImageryData[i];
            }
        }

        //스크립트 관련
        path = Application.streamingAssetsPath + "/Data/Scripts/";

        ScriptDataList scriptDataList = new ScriptDataList();

        foreach (var file in Directory.GetFiles(path))
        {
            scriptDataLists = new Dictionary<string, ScriptDataList>();

            if (file.EndsWith(".meta")) continue;

            string data = File.ReadAllText(file);
           
            scriptDataList = JsonUtility.FromJson<ScriptDataList>(data);
            scriptDataLists.Add(file.Replace(path, "").Replace("Data", ""), scriptDataList);
        }

    }

    public RewardTable GetRewardTables(int index)
    {
        return rewardTableClass.rewardTable[index];
    }

    public string[] GetMatterList(int index)
    {
        return rewardTableClass.rewardTable[index].Matter_list;
    }

    public List<string> GetWaveList(string key)
    {
        List<string> result = new();

        if (phaseRushInfoDic.ContainsKey(key))
            foreach (var rush in phaseRushInfoDic[key].Rush_list)
            {
                if (rushDic.ContainsKey(rush))
                {
                    for (int i = 0; i < rushDic[rush].wavelist.Length; ++i)
                        if (!result.Contains(rushDic[rush].wavelist[i]))
                        {
                            result.Add(rushDic[rush].wavelist[i]);
                        }
                }
            }

        return result;
    }

    public List<string> GetMonsterInWaveList(string key)
    {
        List<string> result = new();

        if (waveDic[key].include_wave.Equals("0") == false)
        {
            foreach (var k in waveDic[key].include_wave)
            {
                var re = GetMonsterInWaveList(k);
                for (int i = 0; i < re.Count; ++i)
                    if (result.Contains(re[i]) == false)
                    {
                        result.Add(re[i]);
                    }
            }
        }

        for (int i = 0; i < waveDic[key].monster.Length; ++i)
        {
            if ((result.Contains(waveDic[key].monster[i]) == false))
                result.Add(waveDic[key].monster[i]);
        }

        return result;
    }

    public Sprite GetRewardAndMatterSprite(string key)
    {
        if (rewardSprite.ContainsKey(key))
            return rewardSprite[key];
        else if (matterImageDic.ContainsKey(key))
            return matterImageDic[key];

        return null;
    }

    public StageInfoData GetStageInfoData()
    {
        int chapter = GameManager.Instance.chapterNum;
        int stage = GameManager.Instance.stageNum;

        int index = 0;
        for (int i = 0; i < chapter; i++)
        {
            index += stageCount[i];
        }
        index += stage;

        //키값 : info_000
        string key = string.Format("info_{0:D3}", index);
        return stageInfoDic.ContainsKey(key) ? stageInfoDic[key] : null;
    }

    public bool IsErgInfinityStage()
    {
        int chapter = GameManager.Instance.chapterNum;
        int stage = GameManager.Instance.stageNum;

        int index = 0;
        for (int i = 0; i < chapter; i++)
        {
            index += stageCount[i];
        }
        index += stage;

        string key = string.Format("info_{0:D3}", index);

        return stageInfoDic[key].Erg_infinty;
    }


    public bool IsSpecialDeckMode()
    {
        int chapter = GameManager.Instance.chapterNum;
        int stage = GameManager.Instance.stageNum;

        int index = 0;
        for (int i = 0; i < chapter; i++)
        {
            index += stageCount[i];
        }
        index += stage;

        string key = string.Format("info_{0:D3}", index);

        return stageInfoDic[key].Special_Deck_mode;
    }

    public int GetStageErg()
    {
        int chapter = GameManager.Instance.chapterNum;
        int stage = GameManager.Instance.stageNum;

        int index = 0;
        for (int i = 0; i < chapter; i++)
        {
            index += stageCount[i];
        }
        index += stage;

        string key = string.Format("info_{0:D3}", index);

        return stageInfoDic[key].Give_Erg;
    }

    public int GetStageRescueCat()
    {
        int chapter = GameManager.Instance.chapterNum;
        int stage = GameManager.Instance.stageNum;

        int index = 0;
        for (int i = 0; i < chapter; i++)
        {
            index += stageCount[i];
        }
        index += stage;

        string key = string.Format("info_{0:D3}", index);

        return stageInfoDic[key].total_rescue_cat;
    }

    public Skill GetSkillData(string ID)
    {
        for (int i = 0; i < skillDataClass.SkillData.Count; ++i)
        {
            if (skillDataClass.SkillData[i].ID == ID)
            {
                return skillDataClass.SkillData[i];
            }
        }
        return null;
    }

    public NPCData GetNPCData(string ID)
    {
        for (int i = 0; i < npcDataClass.CatData.Count; ++i)
        {
            if (npcDataClass.CatData[i].ID == ID)
            {
                return npcDataClass.CatData[i];
            }
        }
        return null;
    }

    public Sprite GetMagicCardSprite(string ID)
    {


        return null;
    }

    public MagicCardData GetMagicCardData(string ID)
    {
        if (magicCardDataDic.ContainsKey(ID))
        {
            return magicCardDataDic[ID];
        }

        return null;
    }

    public UnitData GetUnitData(string ID)
    {
        if (unitDataDic.ContainsKey(ID))
        {
            return unitDataDic[ID];
        }

        return null;
    }

    public MonsterData GetMonsterData(string ID)
    {
        if (monsterDataDic.ContainsKey(ID))
        {
            return monsterDataDic[ID];
        }

        return null;
    }

    public Sprite GetNPCSprite(string id)
    {
        for (int i = 0; i < npcDatas.Count; i++)
        {
            if (npcDatas[i].ID == id)
            {
                return npcDatas[i].npcSprite;
            }
        }
        return null;
    }

    public Sprite GetUnitSprite(string id)
    {
        return unitSprite[id];
    }

    public Sprite GetMonsterSprite(string id)
    {
        return monsterSpriteDic[GetMonsterData(id).SpriteID];
    }

    public GameObject GetNPCPrefab(string id)
    {
        for (int i = 0; i < npcDatas.Count; i++)
        {
            if (npcDatas[i].ID == id)
            {
                return npcDatas[i].npcPrefab;
            }
        }
        return null;
    }

    public GameObject GetUnitPrefab(string id)
    {
        if (unitPrefab.ContainsKey(id))
            return unitPrefab[id];

        return null;
    }

    public GameObject GetMonsterPrefab(string id)
    {
        return monsterPrefab[id];
    }

    public List<BriefingData> GetBriefingData(bool isBefore)
    {
        //키값 : brief_00_01_0_001
        string key = String.Format("brief_{0:D2}_{1:D2}_{2}", GameManager.Instance.chapterNum, GameManager.Instance.stageNum + 1, isBefore ? 0 : 1);

        List<BriefingData> result = new();

        foreach (var dic in briefingDataDic)
        {
            //만든 키가 brief_00_01_0 이라서 StartWith 써야 함
            if (dic.Key.StartsWith(key))
            {
                result.Add(dic.Value);
            }
        }

        return result;
    }

    public List<ScriptData> GetScriptData(string id)
    {
        if (scriptDataLists.ContainsKey(id))
        {
            return scriptDataLists[id].data;
        }
        return null;
    }

    public Sprite GetStandingSprite(CharacterName name, int index)
    {
        for (int i = 0; i < stadingSprites.Count; i++)
        {
            if (stadingSprites[i].name == name)
            {
                return stadingSprites[i].sprite[index];
            }
        }
        return null;
    }

    public Sprite GetStandingSprite(int index)
    {
        return stadingSprites[index].sprite[0];
    }

    public ArtifactData GetArtifactData(string ID)
    {
        foreach (var data in artifactDataClass.Artifact_Data)
        {
            if (data.ID == ID)
            {
                return data;
            }
        }
        return null;
    }

    public Sprite GetArtifactSprite(string ID)
    {
        return artifactImageDic.ContainsKey(ID) ? artifactImageDic[ID] : null;
    }

    public string GetMatterName(string ID)
    {
        foreach (var data in matterDataClass.Matter_Data)
        {
            if (data.ID == ID)
            {
                return data.KoreanName;
            }
        }

        return null;
    }

    public List<RushData> GetRushData(string keys)
    {
        List<RushData> result = new();

        foreach (var list in phaseRushInfoDic[keys].Rush_list)
        {
            if (rushDic.ContainsKey(list))
            {
                result.Add(rushDic[list]);
            }
        }

        return result;
    }

    public List<WaveData> GetWaveData(string[] keys)
    {
        List<WaveData> result = new();

        foreach (var key in keys)
        {
            if (waveDic.ContainsKey(key))
            {
                if (waveDic[key].include_wave.Equals("0") == false)
                {
                    string[] includeKeys = waveDic[key].include_wave;

                    foreach (var includeKey in includeKeys)
                    {
                        if (waveDic.ContainsKey(includeKey))
                        {
                            result.Add(waveDic[includeKey]);
                        }
                    }
                }

                result.Add(waveDic[key]);
            }
        }

        return result;
    }
}
