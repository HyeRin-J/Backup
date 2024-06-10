using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;
using System.Linq;

public enum MapTheme
{
    WitchForest, City, Ocean, Desert, Snow, Plain, Night, Volcano, Space, MAX
};

public class MapManager : MonoBehaviour
{
    public Tilemap backGround;
    public Tilemap floor;
    public Tilemap deco;
    public Tilemap objects;
    public Tilemap environment;
    public Tilemap stageChager;

    public RuleTile backgroundTile;

    public int mapWidth = 10;
    public int mapHeight = 10;

    public int maxPhase = 3;
    public int currentPhase = 1;

    public int chapter = 0;
    public int stage = 0;

    public MapTheme mapTheme;
    public Tile currentSelectedImageryBaseTile;

    public ScriptableLevel scriptableLevel;
    public CameraManager cameraManager;

    // 리소스 폴더 위치
    public string[] texturePath =
    {
        "Textures/Tiles&Objects/00_Floor/",
        "Textures/Tiles&Objects/01_Deco/",
        "Textures/Tiles&Objects/02_Object/",
        "Textures/Tiles&Objects/03_Hole/",
        "Textures/Tiles&Objects/04_Environment/",
        "Textures/Tiles&Objects/05_StageChanger/",
    };

    IEnumerable<SavedTile> GetTilesFromMap(Tilemap tileMap)
    {
        foreach (var pos in tileMap.cellBounds.allPositionsWithin)
        {
            if (tileMap.HasTile(pos))
            {
                var tile = tileMap.GetTile<TileBase>(pos);

                SavedTile savedTile = new SavedTile();
                savedTile.position = pos;
                savedTile.levelTile = ScriptableObject.CreateInstance<CustomTile>();

                string tiletype = tile.GetType().Name;

                savedTile.tileType = tiletype;

                switch (tiletype)
                {
                    case "Tile":
                        var tile1 = tile as Tile;
                        var di = new DirectoryInfo("Assets/Resources/Tiles/Tiles/");
                        FileInfo[] file = di.GetFiles(tile1.name, SearchOption.AllDirectories);
                        if (file.Length > 0)
                            savedTile.folderName = file[0].DirectoryName.Replace(Application.dataPath + "\\Resources\\Tiles\\Tiles\\", "");
                        savedTile.tileName = tile1.name;
                        savedTile.levelTile.sprite = tile1.sprite;
                        break;
                    case "RuleTile":
                    case "CustomRuleTile":
                        var tile2 = tile as RuleTile;
                        var di2 = new DirectoryInfo("Assets/Resources/Tiles/RuleTiles/");
                        FileInfo[] file2 = di2.GetFiles(tile2.name + ".asset", SearchOption.AllDirectories);
                        if (file2.Length > 0)
                            savedTile.folderName = file2[0].DirectoryName.Replace(Application.dataPath + "\\Resources\\Tiles\\RuleTiles\\", "");
                        savedTile.tileName = tile2.name;
                        savedTile.levelTile.sprite = tile2.m_TilingRules[0].m_Sprites[0];
                        break;
                    case "AnimatedTile":
                        var tile3 = tile as AnimatedTile;
                        var di3 = new DirectoryInfo("Assets/Resources/Tiles/AnimatedTile/");
                        FileInfo[] file3 = di3.GetFiles(tile3.name, SearchOption.AllDirectories);
                        if (file3.Length > 0)
                            savedTile.folderName = file3[0].DirectoryName.Replace(Application.dataPath + "\\Resources\\Tiles\\AnimatedTile\\", "");
                        savedTile.tileName = tile3.name;
                        savedTile.levelTile.sprite = tile3.m_AnimatedSprites[0];
                        break;
                    default:
                        break;
                }
                yield return savedTile;
            }
        }
    }

    public void ClearMap()
    {
        backGround.ClearAllTiles();
        floor.ClearAllTiles();
        deco.ClearAllTiles();
        objects.ClearAllTiles();
        environment.ClearAllTiles();
        stageChager.ClearAllTiles();
    }

    public void GenerateMapData()
    {
        scriptableLevel = ScriptableObject.CreateInstance<ScriptableLevel>();

        scriptableLevel.Init(maxPhase);
        scriptableLevel.name = "Map " + chapter.ToString() + "_" + stage.ToString();

        backGround.ResizeBounds();
        floor.ResizeBounds();

        cameraManager.RemoveAllCamera();
        cameraManager.CreateNewMap();

        for (int i = -mapHeight / 2 - 1; i < mapHeight / 2 + 1; ++i)
        {
            for (int j = -mapWidth / 2 - 1; j < mapWidth / 2 + 1; ++j)
            {
                backGround.SetTile(new Vector3Int(j, i, 0), backgroundTile);
            }
        }

        for (int i = -mapHeight / 2; i < mapHeight / 2; ++i)
        {
            for (int j = -mapWidth / 2; j < mapWidth / 2; ++j)
            {
                Vector3Int position = new Vector3Int(j, i, 0);

                floor.SetTile(position, currentSelectedImageryBaseTile);
            }
        }

        currentPhase = 1;
        scriptableLevel.backgrounds[currentPhase - 1].savedTiles = GetTilesFromMap(backGround).ToList();
        scriptableLevel.floor[currentPhase - 1].savedTiles = GetTilesFromMap(floor).ToList();

        floor.RefreshAllTiles();
    }

    public void ChangePhase(int newPhase)
    {
        currentPhase = newPhase;

        //MaxPhase를 넘어가지 않도록 조정
        if (currentPhase > maxPhase)
        {
            currentPhase = maxPhase;
        }
        if (currentPhase < 1)
        {
            currentPhase = 1;
        }

        //현재 Phase에 맞는 맵을 불러옴
        if (scriptableLevel.backgrounds != null && scriptableLevel.backgrounds.Count != 0)
        {
            backGround.ClearAllTiles();

            if (scriptableLevel.backgrounds[currentPhase - 1].savedTiles != null && scriptableLevel.backgrounds[currentPhase - 1].savedTiles.Count > 0)
            {
                foreach (var tile in scriptableLevel.backgrounds[currentPhase - 1].savedTiles)
                {
                    SetTile(backGround, tile);
                }
            }
        }


        if (scriptableLevel.floor != null && scriptableLevel.floor.Count != 0)
        {
            floor.ClearAllTiles();

            if (scriptableLevel.floor[currentPhase - 1].savedTiles != null && scriptableLevel.floor[currentPhase - 1].savedTiles.Count > 0)
            {
                foreach (var tile in scriptableLevel.floor[currentPhase - 1].savedTiles)
                {
                    SetTile(floor, tile);
                }
            }
        }

        if (scriptableLevel.deco != null && scriptableLevel.deco.Count != 0)
        {
            deco.ClearAllTiles();

            if (scriptableLevel.deco[currentPhase - 1].savedTiles != null && scriptableLevel.deco[currentPhase - 1].savedTiles.Count > 0)
            {
                foreach (var tile in scriptableLevel.deco[currentPhase - 1].savedTiles)
                {
                    SetTile(deco, tile);
                }
            }
        }

        if (scriptableLevel.objects != null && scriptableLevel.objects.Count != 0)
        {
            objects.ClearAllTiles();

            if (scriptableLevel.objects[currentPhase - 1].savedTiles != null && scriptableLevel.objects[currentPhase - 1].savedTiles.Count > 0)
            {
                foreach (var tile in scriptableLevel.objects[currentPhase - 1].savedTiles)
                {
                    SetTile(objects, tile);
                }
            }
        }

        if (scriptableLevel.environment != null && scriptableLevel.environment.Count != 0)
        {
            environment.ClearAllTiles();

            if (scriptableLevel.environment[currentPhase - 1].savedTiles != null && scriptableLevel.environment[currentPhase - 1].savedTiles.Count > 0)
            {
                foreach (var tile in scriptableLevel.environment[currentPhase - 1].savedTiles)
                {
                    SetTile(environment, tile);
                }
            }
        }

        if (scriptableLevel.stageChager != null && scriptableLevel.stageChager.Count != 0)
        {
            stageChager.ClearAllTiles();

            if (scriptableLevel.stageChager[currentPhase - 1].savedTiles != null && scriptableLevel.stageChager[currentPhase - 1].savedTiles.Count > 0)
            {
                foreach (var tile in scriptableLevel.stageChager[currentPhase - 1].savedTiles)
                {
                    SetTile(stageChager, tile);
                }
            }
        }

        if (cameraManager.cameraList[currentPhase - 1] == null || cameraManager.cameraList[currentPhase - 1].Count == 0)
        {
            cameraManager.cameraList[currentPhase - 1] = new();

            if (cameraManager.cameraList[currentPhase - 2] != null && cameraManager.cameraList[currentPhase - 2].Count > 0)
                cameraManager.cameraList[currentPhase - 1].AddRange(cameraManager.cameraList[currentPhase - 2]);
        }
    }

    public void SynchroMap()
    {
        if (scriptableLevel.backgrounds != null && scriptableLevel.backgrounds.Count != 0)
        {
            if (scriptableLevel.backgrounds[currentPhase - 1].savedTiles != null && scriptableLevel.backgrounds[currentPhase - 1].savedTiles.Count > 0)
                scriptableLevel.backgrounds[currentPhase - 1].savedTiles.Clear();

            if (scriptableLevel.backgrounds[currentPhase - 2].savedTiles != null && scriptableLevel.backgrounds[currentPhase - 2].savedTiles.Count > 0)
            {
                scriptableLevel.backgrounds[currentPhase - 1].savedTiles = new List<SavedTile>(scriptableLevel.backgrounds[currentPhase - 2].savedTiles.Count);

                for (int index = 0; index < scriptableLevel.backgrounds[currentPhase - 2].savedTiles.Count; ++index)
                {
                    scriptableLevel.backgrounds[currentPhase - 1].savedTiles.Add(scriptableLevel.backgrounds[currentPhase - 2].savedTiles[index]);

                    SetTile(backGround, scriptableLevel.backgrounds[currentPhase - 1].savedTiles[index]);
                }
            }
        }

        if (scriptableLevel.floor != null && scriptableLevel.floor.Count != 0)
        {
            if (scriptableLevel.floor[currentPhase - 1].savedTiles != null && scriptableLevel.floor[currentPhase - 1].savedTiles.Count > 0)
                scriptableLevel.floor[currentPhase - 1].savedTiles.Clear();

            if (scriptableLevel.floor[currentPhase - 2].savedTiles != null && scriptableLevel.floor[currentPhase - 2].savedTiles.Count > 0)
            {
                scriptableLevel.floor[currentPhase - 1].savedTiles = new List<SavedTile>(scriptableLevel.floor[currentPhase - 2].savedTiles.Count);

                for (int index = 0; index < scriptableLevel.floor[currentPhase - 2].savedTiles.Count; ++index)
                {
                    scriptableLevel.floor[currentPhase - 1].savedTiles.Add(scriptableLevel.floor[currentPhase - 2].savedTiles[index]);

                    SetTile(floor, scriptableLevel.floor[currentPhase - 1].savedTiles[index]);
                }
            }
        }

        if (scriptableLevel.deco != null && scriptableLevel.deco.Count != 0)
        {
            if (scriptableLevel.deco[currentPhase - 1].savedTiles != null && scriptableLevel.deco[currentPhase - 1].savedTiles.Count > 0)
                scriptableLevel.deco[currentPhase - 1].savedTiles.Clear();

            if (scriptableLevel.deco[currentPhase - 2].savedTiles != null && scriptableLevel.deco[currentPhase - 2].savedTiles.Count > 0)
            {
                scriptableLevel.deco[currentPhase - 1].savedTiles = new List<SavedTile>(scriptableLevel.deco[currentPhase - 2].savedTiles.Count);

                for (int index = 0; index < scriptableLevel.deco[currentPhase - 2].savedTiles.Count; ++index)
                {
                    scriptableLevel.deco[currentPhase - 1].savedTiles.Add(scriptableLevel.deco[currentPhase - 2].savedTiles[index]);

                    SetTile(deco, scriptableLevel.deco[currentPhase - 1].savedTiles[index]);
                }
            }
        }

        if (scriptableLevel.objects != null && scriptableLevel.objects.Count != 0)
        {
            if (scriptableLevel.objects[currentPhase - 1].savedTiles != null && scriptableLevel.objects[currentPhase - 1].savedTiles.Count > 0)
                scriptableLevel.objects[currentPhase - 1].savedTiles.Clear();

            if (scriptableLevel.objects[currentPhase - 2].savedTiles != null && scriptableLevel.objects[currentPhase - 2].savedTiles.Count > 0)
            {
                scriptableLevel.objects[currentPhase - 1].savedTiles = new List<SavedTile>(scriptableLevel.objects[currentPhase - 2].savedTiles.Count);

                for (int index = 0; index < scriptableLevel.objects[currentPhase - 2].savedTiles.Count; ++index)
                {
                    scriptableLevel.objects[currentPhase - 1].savedTiles.Add(scriptableLevel.objects[currentPhase - 2].savedTiles[index]);

                    SetTile(objects, scriptableLevel.objects[currentPhase - 1].savedTiles[index]);
                }
            }
        }

        if (scriptableLevel.environment != null && scriptableLevel.environment.Count != 0)
        {
            if (scriptableLevel.environment[currentPhase - 1].savedTiles != null && scriptableLevel.environment[currentPhase - 1].savedTiles.Count > 0)
                scriptableLevel.environment[currentPhase - 1].savedTiles.Clear();

            if (scriptableLevel.environment[currentPhase - 2].savedTiles != null && scriptableLevel.environment[currentPhase - 2].savedTiles.Count > 0)
            {
                scriptableLevel.environment[currentPhase - 1].savedTiles = new List<SavedTile>(scriptableLevel.environment[currentPhase - 2].savedTiles.Count);

                for (int index = 0; index < scriptableLevel.environment[currentPhase - 2].savedTiles.Count; ++index)
                {
                    scriptableLevel.environment[currentPhase - 1].savedTiles.Add(scriptableLevel.environment[currentPhase - 2].savedTiles[index]);

                    SetTile(environment, scriptableLevel.environment[currentPhase - 1].savedTiles[index]);
                }
            }
        }

        if (scriptableLevel.stageChager != null && scriptableLevel.stageChager.Count != 0)
        {
            if (scriptableLevel.stageChager[currentPhase - 1].savedTiles != null && scriptableLevel.stageChager[currentPhase - 1].savedTiles.Count > 0)
                scriptableLevel.stageChager[currentPhase - 1].savedTiles.Clear();

            if (scriptableLevel.stageChager[currentPhase - 2].savedTiles != null && scriptableLevel.stageChager[currentPhase - 2].savedTiles.Count > 0)
            {
                scriptableLevel.stageChager[currentPhase - 1].savedTiles = new List<SavedTile>(scriptableLevel.stageChager[currentPhase - 2].savedTiles.Count);

                for (int index = 0; index < scriptableLevel.stageChager[currentPhase - 2].savedTiles.Count; ++index)
                {
                    scriptableLevel.stageChager[currentPhase - 1].savedTiles.Add(scriptableLevel.stageChager[currentPhase - 2].savedTiles[index]);

                    SetTile(stageChager, scriptableLevel.stageChager[currentPhase - 1].savedTiles[index]);
                }
            }
        }

        if (cameraManager.cameraList[currentPhase - 1] == null)
        {
            cameraManager.cameraList[currentPhase - 1] = new();
        }
        else
        {
            cameraManager.cameraList[currentPhase - 1].Clear();
        }

        if (cameraManager.cameraList[currentPhase - 2] != null && cameraManager.cameraList[currentPhase - 2].Count > 0)
            cameraManager.cameraList[currentPhase - 1].AddRange(cameraManager.cameraList[currentPhase - 2]);
    }

    public void SaveMap()
    {
        scriptableLevel.Chapter = chapter;
        scriptableLevel.Stage = stage;
        scriptableLevel.MapTheme = mapTheme;
        scriptableLevel.MapWidth = mapWidth;
        scriptableLevel.MapHeight = mapHeight;
        scriptableLevel.MaxPhase = maxPhase;

        int cameraMaxCount = 0;
        int cameraMaxIndex = 0;

        for (int i = 0; i < maxPhase; i++)
        {
            if (cameraMaxCount < cameraManager.cameraList[i].Count)
            {
                cameraMaxCount = cameraManager.cameraList[i].Count;
                cameraMaxIndex = i;
            }

            if (cameraManager.cameraList[i] != null)
                scriptableLevel.cameraClassCount[i] = cameraManager.cameraList[i].Count;
        }

        scriptableLevel.cameraPosition = new List<Vector2>();
        scriptableLevel.cameraSize = new List<float>();

        for (int i = 0; i < cameraMaxCount; ++i)
        {
            Vector3 pos = new Vector3();

            pos.x = float.Parse(cameraManager.cameraList[cameraMaxIndex][i].position.x.ToString("N1"));
            pos.y = float.Parse(cameraManager.cameraList[cameraMaxIndex][i].position.y.ToString("N1"));

            scriptableLevel.cameraPosition.Add(pos);
            scriptableLevel.cameraSize.Add(cameraManager.cameraList[cameraMaxIndex][i].cameraSize);
        }

        scriptableLevel.backgrounds[currentPhase - 1].savedTiles = GetTilesFromMap(backGround).ToList();
        scriptableLevel.floor[currentPhase - 1].savedTiles = GetTilesFromMap(floor).ToList();
        scriptableLevel.deco[currentPhase - 1].savedTiles = GetTilesFromMap(deco).ToList();
        scriptableLevel.objects[currentPhase - 1].savedTiles = GetTilesFromMap(objects).ToList();

        ScriptableObjectUtility.SaveLevelFile(scriptableLevel);
    }
    public void LoadMap()
    {
        scriptableLevel = ScriptableObjectUtility.LoadLevelFile(chapter, stage, currentPhase);

        ClearMap();
        cameraManager.RemoveAllCamera();

        mapWidth = scriptableLevel.MapWidth;
        mapHeight = scriptableLevel.MapHeight;
        chapter = scriptableLevel.Chapter;
        stage = scriptableLevel.Stage;
        mapTheme = scriptableLevel.MapTheme;
        maxPhase = scriptableLevel.MaxPhase;
        currentPhase = 1;

        if (scriptableLevel.backgrounds != null && scriptableLevel.backgrounds.Count != 0)
            foreach (var saveTile in scriptableLevel.backgrounds[currentPhase - 1].savedTiles)
            {
                SetTile(backGround, saveTile);
            }

        if (scriptableLevel.floor != null && scriptableLevel.floor.Count != 0)
            foreach (var saveTile in scriptableLevel.floor[currentPhase - 1].savedTiles)
            {
                SetTile(floor, saveTile);
            }

        if (scriptableLevel.deco != null && scriptableLevel.deco.Count != 0)
            foreach (var saveTile in scriptableLevel.deco[currentPhase - 1].savedTiles)
            {
                SetTile(deco, saveTile);
            }

        if (scriptableLevel.objects != null && scriptableLevel.objects.Count != 0)
            foreach (var saveTile in scriptableLevel.objects[currentPhase - 1].savedTiles)
            {
                SetTile(objects, saveTile);
            }

        if (scriptableLevel.environment != null && scriptableLevel.environment.Count != 0)
            foreach (var saveTile in scriptableLevel.environment[currentPhase - 1].savedTiles)
            {
                SetTile(environment, saveTile);
            }

        if (scriptableLevel.stageChager != null && scriptableLevel.stageChager.Count != 0)
            foreach (var saveTile in scriptableLevel.stageChager[currentPhase - 1].savedTiles)
            {
                SetTile(stageChager, saveTile);
            }

        cameraManager.cameraList = new List<List<CameraClass>>();

        for (int i = 0; i < scriptableLevel.MaxPhase; ++i)
            cameraManager.cameraList.Add(new List<CameraClass>());

        for (int i = 0; i < scriptableLevel.cameraClassCount.Count(); ++i)
        {
            for (int j = 0; j < scriptableLevel.cameraClassCount[i]; j++)
            {
                CameraClass cameraClass = new CameraClass();

                cameraClass.position = scriptableLevel.cameraPosition[j];
                cameraClass.cameraSize = scriptableLevel.cameraSize[j];

                cameraClass.CreateCamera(j).transform.SetParent(cameraManager.transform);

                cameraManager.cameraList[i].Add(cameraClass);
            }
        }
    }

    void SetTile(Tilemap tileMap, SavedTile savedTile)
    {
        switch (savedTile.tileType)
        {
            case "Tile":
                string path = "Tiles/Tiles/";

                if (savedTile.folderName.Length > 0)
                {
                    path += savedTile.folderName + "/";
                }
                path += savedTile.tileName;

                Tile tile = Resources.Load<Tile>(path);
                tileMap.SetTile(savedTile.position, tile);
                break;
            case "RuleTile":
            case "CustomRuleTile":
                string path2 = "Tiles/RuleTiles/";

                if (savedTile.folderName.Length > 0)
                {
                    path2 += savedTile.folderName + "/";
                }

                path2 += savedTile.tileName;

                RuleTile ruleTile = Resources.Load<RuleTile>(path2);
                tileMap.SetTile(savedTile.position, ruleTile);
                break;
            case "AnimatedTile":
                string path3 = "Tiles/AnimatedTiles/";

                if (savedTile.folderName.Length > 0)
                {
                    path3 += savedTile.folderName + "/";
                }

                path3 += savedTile.tileName;

                AnimatedTile animatedTile = Resources.Load<AnimatedTile>(path3);
                tileMap.SetTile(savedTile.position, animatedTile);
                break;
        }

        tileMap.RefreshAllTiles();

    }

}

#if UNITY_EDITOR

public class ScriptableObjectUtility
{
    public static void SaveLevelFile(ScriptableLevel level)
    {
        string title = "Save Map File";
        string msg = "Do you want to save map data?";

        //if user click save button, show a dialog to confirm save
        //if user click no, do nothing just return
        if (UnityEditor.EditorUtility.DisplayDialog(title, msg, "yes", "no") == false) return;

        string jsonString = JsonUtility.ToJson(level);

        string filePath = Application.dataPath + "/Resources/TileMaps/" + level.name + ".json";

        File.WriteAllText(filePath, jsonString);

        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
    }

    public static ScriptableLevel LoadLevelFile(int chapter, int stage, int currentPhase)
    {
        ScriptableLevel map = ScriptableObject.CreateInstance<ScriptableLevel>();

        JsonUtility.FromJsonOverwrite(System.IO.File.ReadAllText("Assets/Resources/TileMaps/Map " + chapter.ToString() + "_" + stage.ToString() + ".json"), map);
        map.name = "Map " + chapter.ToString() + "_" + stage.ToString();

        if (map == null)
        {
            Debug.Log("Map " + chapter.ToString() + "_" + stage.ToString() + " is not exist");
            return null;
        }

        UnityEditor.EditorUtility.DisplayDialog("Load Map File", "Load Finish!", "ok");

        return map;
    }
}

#endif
