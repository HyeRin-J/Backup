using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


[System.Serializable]
public class ScriptableLevel : ScriptableObject
{
    public int Chapter;
    public int Stage;
    public int MaxPhase;

    public Imagery MapTheme;

    public int MapWidth, MapHeight;

    public List<int> cameraClassCount;

    [SerializeField]
    public List<Vector2> cameraPosition;
    [SerializeField]
    public List<float> cameraSize;

    [SerializeField]
    public List<SaveTileList> backgrounds;
    [SerializeField]
    public List<SaveTileList> floor;
    [SerializeField]
    public List<SaveTileList> deco;
    [SerializeField]
    public List<SaveTileList> objects;
    [SerializeField]
    public List<SaveTileList> environment;
    [SerializeField]
    public List<SaveTileList> stageChager;

    public void Init(int maxPhase)
    {
        MaxPhase = maxPhase;

        cameraClassCount = new List<int>(maxPhase);
        backgrounds = new List<SaveTileList>(maxPhase);
        floor = new List<SaveTileList>(maxPhase);
        deco = new List<SaveTileList>(maxPhase);
        objects = new List<SaveTileList>(maxPhase);
        environment = new List<SaveTileList>(maxPhase);
        stageChager = new List<SaveTileList>(maxPhase);

        for (int i = 0; i < maxPhase; ++i)
        {
            backgrounds.Add(new SaveTileList());
            floor.Add(new SaveTileList());
            deco.Add(new SaveTileList());
            objects.Add(new SaveTileList());
            environment.Add(new SaveTileList());
            stageChager.Add(new SaveTileList());
        }

    }
}

[System.Serializable]
public class SaveTileList
{
    public List<SavedTile> savedTiles;
}

[System.Serializable]
public class CustomTile : Tile
{

}


[System.Serializable]
public class SavedTile
{
    [SerializeField]
    public Vector3Int position;

    [SerializeField]
    public string tileType;

    [SerializeField]
    public string folderName;

    [SerializeField]
    public string tileName;

    [SerializeField]
    public CustomTile levelTile;
}