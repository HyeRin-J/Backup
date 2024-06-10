using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

[CustomEditor(typeof(MapManager))]
public class MapManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // base.OnInspectorGUI();
        MapManager mapManager = target as MapManager;

        mapManager.scriptableLevel =
                   EditorGUILayout.ObjectField("Scriptable Level", mapManager.scriptableLevel, typeof(ScriptableLevel), false) as ScriptableLevel;

        DrawUILine(Color.cyan);

        mapManager.currentSelectedImageryBaseTile = EditorGUILayout.ObjectField("Current Imagery Base Tile", mapManager.currentSelectedImageryBaseTile, typeof(Tile), false) as Tile;

        DrawUILine(Color.cyan);

        if (mapManager.scriptableLevel == null)
            mapManager.scriptableLevel = CreateInstance<ScriptableLevel>();

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Map Size", GUILayout.Width(80));
        mapManager.mapWidth = EditorGUILayout.IntField("", mapManager.mapWidth, GUILayout.MinWidth(80));
        mapManager.mapHeight = EditorGUILayout.IntField("", mapManager.mapHeight, GUILayout.MinWidth(80));
        GUILayout.EndHorizontal();

        mapManager.maxPhase = EditorGUILayout.IntField("Max Phase", mapManager.maxPhase, GUILayout.MinWidth(80));

        GUILayout.BeginHorizontal();

        int currentPhase = mapManager.currentPhase;
        int newPhase = currentPhase;

        EditorGUILayout.LabelField("Current Phase", GUILayout.MinWidth(80));

        if (GUILayout.Button("<"))
        {
            newPhase--;
        }

        EditorGUILayout.IntField(mapManager.currentPhase, GUILayout.MinWidth(80));
        
        if (GUILayout.Button(">"))
        {
            newPhase++;
        }

        if (newPhase != mapManager.currentPhase)
        {
            mapManager.ChangePhase(newPhase);
        }
        
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        mapManager.chapter = EditorGUILayout.IntField("Chapter", mapManager.chapter, GUILayout.MinWidth(80));
        mapManager.stage = EditorGUILayout.IntField("Level", mapManager.stage, GUILayout.MinWidth(80));
        GUILayout.EndHorizontal();

        mapManager.mapTheme = (MapTheme)EditorGUILayout.EnumPopup("Theme", mapManager.mapTheme);

        if (GUILayout.Button("Create Map"))
        {
            mapManager.ClearMap();
            mapManager.GenerateMapData();
        }

        DrawUILine(Color.cyan);

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Save Map"))
        {
            mapManager.SaveMap();
        }

        if (GUILayout.Button("Load Map"))
        {
            mapManager.LoadMap();
        }

        GUILayout.EndHorizontal();

        if (GUILayout.Button("Synchro Map"))
        {
            mapManager.SynchroMap();
        }

    }

    //Draw UI Line
    public static void DrawUILine(Color color, int thickness = 1, int padding = 10)
    {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
        r.height = thickness;
        r.y += padding / 2;
        r.x -= 2;
        r.width += 6;
        EditorGUI.DrawRect(r, color);
    }
}
