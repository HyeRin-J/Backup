using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AStarDebugger : MonoBehaviour
{
    [SerializeField]
    private Grid grid;

    [SerializeField]
    private Tile debugTile;

    [SerializeField]
    private Tilemap tilemap;

    [SerializeField]
    private Color openColor, closedColor, pathColor, currentColor, startColor, goalColor;

    [SerializeField]
    private Canvas canvas;

    [SerializeField]
    private GameObject debugTextPrefab;

    private List<GameObject> debugTexts = new List<GameObject>();

    public void CreateTiles(HashSet<Node> openList, HashSet<Node> closedList, Dictionary<Vector3Int, Node> allNodes, Stack<Vector3Int> path, Vector3Int start, Vector3Int goal)
    {
        foreach (var node in openList)
        {
            ColorTile(node.position, openColor);
        }

        foreach (var node in closedList)
        {
            ColorTile(node.position, closedColor);
        }

        if (path != null)
            foreach (var node in path)
            {
                ColorTile(node, pathColor);
            }

        ColorTile(start, startColor);
        ColorTile(goal, goalColor);

        foreach (KeyValuePair<Vector3Int, Node> node in allNodes)
        {
            if (node.Value.parent != null)
            {
                GameObject go = Instantiate(debugTextPrefab, canvas.transform);
                go.transform.position = grid.CellToWorld(node.Key) + new Vector3(0.5f, 0.5f, 0);
                debugTexts.Add(go);
                GenerateDebugText(node.Value, go.GetComponent<DebugText>());
            }
        }
    }

    public void GenerateDebugText(Node node, DebugText debugText)
    {
        if (node.parent.position.x < node.position.x && node.parent.position.y == node.position.y)
        {
            debugText.MyArrow.text = "¡ç";
        }
        else if (node.parent.position.x > node.position.x && node.parent.position.y == node.position.y)
        {
            debugText.MyArrow.text = "¡æ";
        }
        else if (node.parent.position.x == node.position.x && node.parent.position.y < node.position.y)
        {
            debugText.MyArrow.text = "¡é";
        }
        else if (node.parent.position.x == node.position.x && node.parent.position.y > node.position.y)
        {
            debugText.MyArrow.text = "¡è";
        }
        else if (node.parent.position.x < node.position.x && node.parent.position.y < node.position.y)
        {
            debugText.MyArrow.text = "¢×";
        }
        else if (node.parent.position.x > node.position.x && node.parent.position.y < node.position.y)
        {
            debugText.MyArrow.text = "¢Ù";
        }
        else if (node.parent.position.x < node.position.x && node.parent.position.y > node.position.y)
        {
            debugText.MyArrow.text = "¢Ø";
        }
        else if (node.parent.position.x > node.position.x && node.parent.position.y > node.position.y)
        {
            debugText.MyArrow.text = "¢Ö";
        }

        debugText.MyG.text = "G: " + node.G;
        debugText.MyH.text = "H: " + node.H;
        debugText.MyF.text = "F: " + node.F;
    }

    public void ColorTile(Vector3Int position, Color color)
    {
        tilemap.SetTile(position, debugTile);
        tilemap.SetTileFlags(position, TileFlags.None);
        tilemap.SetColor(position, color);
    }
    public void ShowHide()
    {
        tilemap.gameObject.SetActive(!tilemap.isActiveAndEnabled);
        canvas.gameObject.SetActive(!canvas.isActiveAndEnabled);
        Color c = tilemap.color;
        c.a = c.a != 0 ? 0 : 1;
    }

    public void ResetDebugger(Dictionary<Vector3Int, Node> allNodes)
    {
        foreach (GameObject go in debugTexts)
        {
            DestroyImmediate(go);
        }

        foreach (Vector3Int pos in allNodes.Keys)
        {
            tilemap.SetTile(pos, null);
        }

        allNodes.Clear();

        debugTexts.Clear();

        tilemap.ClearAllTiles();
    }
}
