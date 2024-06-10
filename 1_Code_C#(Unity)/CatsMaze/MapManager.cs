using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    public CameraManager cameraManager;
    public AStar aStar;
    public StageManager stageManager;

    public Tilemap backGround;
    public Tilemap floor;
    public Tilemap deco;
    public Tilemap objects;
    public Tilemap environment;
    public Tilemap stageChager;

    public ScriptableLevel scriptableLevel;

    public int mapWidth = 10;
    public int mapHeight = 10;

    public int maxPhase = 3;
    public int currentPhase = 1;

    public int chapter = 0;
    public int stage = 0;

    Vector3Int startPos, endPos;

    public Imagery mapTheme;
    private Dictionary<Vector3Int, bool> tileIsLoaded;

    private async void Awake()
    {
        ClearMap();

        chapter = GameManager.Instance.chapterNum;
        stage = GameManager.Instance.stageNum;

        scriptableLevel = LoadLevelFile(chapter, stage + 1);

        cameraManager.RemoveCamera();

        mapWidth = scriptableLevel.MapWidth;
        mapHeight = scriptableLevel.MapHeight;
        mapTheme = scriptableLevel.MapTheme;
        stageManager.maxPhase = maxPhase = scriptableLevel.MaxPhase;
        currentPhase = 1;

        tileIsLoaded = new Dictionary<Vector3Int, bool>();

        for (int i = -mapHeight / 2; i < mapHeight / 2; i++)
        {
            for (int j = -mapWidth / 2; j < mapWidth / 2; j++)
            {
                tileIsLoaded.Add(new Vector3Int(j, i, 0), false);
            }
        }

        await LoadMap();
    }

    private void ClearMap()
    {
        backGround.ClearAllTiles();
        floor.ClearAllTiles();
        deco.ClearAllTiles();
        objects.ClearAllTiles();
        environment.ClearAllTiles();
        stageChager.ClearAllTiles();
    }

    async UniTaskVoid LoadTiles(Tilemap tilemap, SaveTileList saveTileList, int startIndex, int finishIndex)
    {
        if (saveTileList != null && saveTileList.savedTiles.Count != 0)
        {
            for (int i = startIndex; i < finishIndex; i++)
            {
                SetTile(tilemap, saveTileList.savedTiles[i]);

                tileIsLoaded[saveTileList.savedTiles[i].position] = true;

                await UniTask.Yield(this.GetCancellationTokenOnDestroy());
            }
        }
    }

    async UniTaskVoid LoadObject(int startIndex, int finishIndex)
    {
        if (scriptableLevel.objects != null && scriptableLevel.objects.Count != 0)
            for (int i = startIndex; i < finishIndex; i++)
            {
                tileIsLoaded[scriptableLevel.objects[currentPhase - 1].savedTiles[i].position] = false;

                var saveTile = scriptableLevel.objects[currentPhase - 1].savedTiles[i];

                SetTile(objects, saveTile);

                aStar.AddNotRemoveObjects(saveTile.position);

                if (saveTile.tileName.Contains("start", StringComparison.OrdinalIgnoreCase))
                {
                    startPos = saveTile.position;
                }
                else if (saveTile.tileName.Contains("end", StringComparison.OrdinalIgnoreCase))
                {
                    endPos = saveTile.position;
                }
                else if (saveTile.tileName.Contains("door", StringComparison.OrdinalIgnoreCase))
                {
                    aStar.tileType = TileType.DOOR;
                    aStar.ChangeTile(saveTile.position);

                    if (!GameManager.Instance.IsTutorialFinish())
                        MessageQueue.Instance.QueueMesssage(new AddTutorialValueMessage(true));
                }
                else if (saveTile.tileName.Contains("wall", StringComparison.OrdinalIgnoreCase))
                {
                    aStar.tileType = TileType.WALL;
                    aStar.AddNotRemoveObstacles(saveTile.position);
                }
                else
                {
                    aStar.AddNotRemoveObstacles(saveTile.position);
                }


                tileIsLoaded[scriptableLevel.objects[currentPhase - 1].savedTiles[i].position] = true;

                await UniTask.Yield(this.GetCancellationTokenOnDestroy());
            }
    }

    private void LoadTilesByPhase(SaveTileList tileList, Tilemap tile, int chunkSize)
    {
        int count = tileList.savedTiles.Count;

        for (int i = 0; i < Mathf.CeilToInt((float)count / chunkSize); i++)
        {
            int start = i * chunkSize;
            int end = Mathf.Min((i + 1) * chunkSize, count);
            LoadTiles(tile, tileList, start, end).Forget();
        }
    }

    private void LoadObjectByPhase(SaveTileList objectList, int chunkSize)
    {
        int count = objectList.savedTiles.Count;

        for (int i = 0; i < Mathf.CeilToInt((float)count / chunkSize); i++)
        {
            int start = i * chunkSize;
            int end = Mathf.Min((i + 1) * chunkSize, count);
            LoadObject(start, end).Forget();
        }
    }

    async UniTask LoadMap()
    {
        ClearMap();

        cameraManager.LoadData(scriptableLevel.cameraClassCount[currentPhase - 1], scriptableLevel.cameraPosition, scriptableLevel.cameraSize);

        int chunkSize = scriptableLevel.cameraClassCount[currentPhase - 1];
        SaveTileList backGroundList = scriptableLevel.backgrounds[currentPhase - 1];
        LoadTilesByPhase(backGroundList, backGround, chunkSize);
        SaveTileList floorList = scriptableLevel.floor[currentPhase - 1];
        LoadTilesByPhase(floorList, floor, chunkSize);
        SaveTileList decoList = scriptableLevel.deco[currentPhase - 1];
        LoadTilesByPhase(decoList, deco, chunkSize);
        SaveTileList objectList = scriptableLevel.objects[currentPhase - 1];
        LoadObjectByPhase(objectList, chunkSize);

        await UniTask.WaitUntil(() => AllTilesLoaded(), cancellationToken: this.GetCancellationTokenOnDestroy());

        MessageQueue.Instance.QueueMesssage(new MapLoadFinishMessage(mapWidth, mapHeight, startPos, endPos));
    }

    bool AllTilesLoaded()
    {
        foreach (var val in tileIsLoaded.Values)
        {
            if (!val)
            {
                return false; // 아직 로드되지 않은 타일이 있으면 false 반환
            }
        }

        return true; // 모든 타일이 로드되었으면 true 반환
    }

    public void SetTile(Vector3Int pos, TileBase tile, bool isObject)
    {
        if (isObject)
        {
            objects.SetTile(pos, tile);
            objects.RefreshTile(pos);

            aStar.AddNotRemoveObstacles(pos);
        }
        else
        {
            aStar.roomDetection.allTiles[pos].savedTile.tileName = "sandQuick";
            deco.SetTile(pos, tile);
            deco.RefreshTile(pos);
        }
    }

    void SetTile(Tilemap tileMap, SavedTile savedTile)
    {
        switch (savedTile.tileType)
        {
            case "Tile":
                string path = "Tiles/Tiles/";

                if (savedTile.folderName != null && savedTile.folderName.Length > 0)
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

                if (savedTile.folderName != null && savedTile.folderName.Length > 0)
                {
                    path2 += savedTile.folderName + "/";
                }

                path2 += savedTile.tileName;

                RuleTile ruleTile = Resources.Load<RuleTile>(path2);
                tileMap.SetTile(savedTile.position, ruleTile);
                break;
            case "AnimatedTile":
                string path3 = "Tiles/AnimatedTiles/";

                if (savedTile.folderName != null && savedTile.folderName.Length > 0)
                {
                    path3 += savedTile.folderName + "/";
                }

                path3 += savedTile.tileName;

                AnimatedTile animatedTile = Resources.Load<AnimatedTile>(path3);
                tileMap.SetTile(savedTile.position, animatedTile);
                break;
        }

        tileMap.RefreshTile(savedTile.position);
    }

    public async void ChangeNextPhase()
    {
        aStar.ClearAllNotRemovableObjectsAndObstacles();
        currentPhase = stageManager.currentPhase;
        await LoadMap();
        aStar.RefreshNextPhase();
    }

    public static ScriptableLevel LoadLevelFile(int chapter, int stage)
    {
        ScriptableLevel map = ScriptableObject.CreateInstance<ScriptableLevel>();

        TextAsset jsonTxt = Resources.Load<TextAsset>("TileMaps/Map " + chapter.ToString() + "_" + (stage).ToString());
        JsonUtility.FromJsonOverwrite(jsonTxt.text, map);

        map.name = "Map " + chapter.ToString() + "_" + stage.ToString();

        if (map == null)
        {
            Debug.Log("Map " + chapter.ToString() + "_" + stage.ToString() + " is not exist");
            return null;
        }

        return map;
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

        string path = "Assets/Resources/TileMaps/" + level.name + ".asset";

        if (!UnityEditor.AssetDatabase.Contains(level))
            UnityEditor.AssetDatabase.CreateAsset(level, path);

        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
    }
}
#endif