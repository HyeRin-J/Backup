using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class AStarChecker : MonoBehaviour
{
    public AStar aStar;

    public Tilemap checkTilemap, floorTilemap;
    public RuleTile wallBluePrint, doorBluePrint;
    public RuleTile roomCheckTile_safe, roomCheckTile_None;

    Vector3Int prevCellPos = new Vector3Int(-99, -99, 0);

    private Dictionary<Vector3Int, Node> allNodes = new();

    [SerializeField]
    public Dictionary<Vector3Int, RoomTile> allTiles;

    private void Awake()
    {
        MessageQueue.Instance.AttachListener(typeof(PhaseStartMessage), ResetAllTiles);
    }

    private void OnDestroy()
    {
        MessageQueue.Instance.DetachListener(typeof(PhaseStartMessage), ResetAllTiles);
    }

    public void Initialize()
    {
        allTiles = new();

        foreach(var dic in aStar.roomDetection.allTiles)
        {
            RoomTile roomTile = new RoomTile();
            roomTile.savedTile = new SavedTile();
            roomTile.savedTile.position = dic.Value.savedTile.position;

            roomTile.room = null;

            allTiles.Add(roomTile.savedTile.position, roomTile);
        }
    }

    private Node GetNode(Vector3Int position, Dictionary<Vector3Int, Node> nodes = null)
    {
        if (nodes == null)
        {
            if (allNodes.ContainsKey(position))
            {
                return allNodes[position];
            }
            else
            {
                Node node = new Node(position);

                allNodes.Add(position, node);

                return node;
            }
        }
        else
        {
            if (nodes.ContainsKey(position))
            {
                return nodes[position];
            }
            else
            {
                Node node = new Node(position);

                nodes.Add(position, node);

                return node;
            }
        }
    }

    private List<Node> FindNeighbors(Vector3Int parentPosition, Vector3Int goalPos, Dictionary<Vector3Int, Node> newNodes)
    {
        List<Node> neighbors = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }

                Vector3Int neighborPos = new Vector3Int(parentPosition.x + x, parentPosition.y + y, 0);

                if (floorTilemap.GetTile(neighborPos) != null && (neighborPos == goalPos ||
                    (!aStar.IsObstacle(neighborPos) &&
                    ((aStar.tileType == TileType.WALL && neighborPos != prevCellPos) ||
                      aStar.tileType == TileType.DOOR))))
                {
                    Node neighbor = GetNode(neighborPos, newNodes);
                    neighbors.Add(neighbor);
                }
            }
        }

        return neighbors;
    }

    private void CalcValues(Node parent, Node neighbor, int cost)
    {
        neighbor.parent = parent;

        neighbor.G = parent.G + cost;
        neighbor.H = (Mathf.Abs(neighbor.position.x - aStar.endPos.x) + Mathf.Abs(neighbor.position.y - aStar.endPos.y)) * 10;
        neighbor.F = neighbor.G + neighbor.H;
    }

    private int DetermineGScore(Vector3Int neighbor, Vector3Int current)
    {
        int gScore = 0;

        int x = current.x - neighbor.x;
        int y = current.y - neighbor.y;

        if (Mathf.Abs(x - y) % 2 == 1)
        {
            gScore = 10;
        }
        else
        {
            gScore = 14;
        }

        return gScore;
    }

    private bool ConnectedDiagonally(Node currentNode, Node neighbor)
    {
        Vector3Int direction = neighbor.position - currentNode.position;

        Vector3Int first = new Vector3Int(currentNode.position.x + direction.x, currentNode.position.y, currentNode.position.z);
        Vector3Int second = new Vector3Int(currentNode.position.x, currentNode.position.y + direction.y, currentNode.position.z);

        if (aStar.IsObstacle(first) || aStar.IsObstacle(second))
        {
            return false;
        }

        return true;
    }

    private void ExamineNeighbors(List<Node> neighbors, Node current, HashSet<Node> newOpenList = null, HashSet<Node> newClosedList = null)
    {
        for (int i = 0; i < neighbors.Count; ++i)
        {
            Node neighbor = neighbors[i];

            if (!ConnectedDiagonally(current, neighbor))
            {
                continue;
            }

            int gScore = DetermineGScore(neighbor.position, current.position);

            if (newOpenList.Contains(neighbor))
            {
                if (current.G + gScore < neighbor.G)
                {
                    CalcValues(current, neighbor, gScore);
                }
            }
            else if (!newClosedList.Contains(neighbor))
            {
                CalcValues(current, neighbor, gScore);
                newOpenList.Add(neighbor);
            }
        }
    }

    private Node GetDiagonalNodeBetween(Vector3Int pos1, Vector3Int pos2)
    {
        Vector3Int direction = pos2 - pos1;

        Vector3Int first = new Vector3Int(pos1.x + direction.x, pos1.y, pos1.z);
        Vector3Int second = new Vector3Int(pos1.x, pos1.y + direction.y, pos1.z);

        if (aStar.IsObstacle(first))
        {
            return GetNode(second);
        }
        else
        {
            return GetNode(first);
        }
    }

    private Stack<Vector3Int> GeneratePath(Node current, Vector3Int startPos, Vector3Int goalPos)
    {
        if (current.position == goalPos)
        {
            Stack<Vector3Int> finalPath = new Stack<Vector3Int>();

            while (current.position != startPos)
            {
                int x = current.position.x - current.parent.position.x;
                int y = current.position.y - current.parent.position.y;

                finalPath.Push(current.position);

                if (Mathf.Abs(x - y) % 2 != 1)
                {
                    Node node = GetDiagonalNodeBetween(current.parent.position, current.position);

                    finalPath.Push(node.position);
                }

                current = current.parent;
            }

            return finalPath;
        }

        return null;
    }

    private void UpdateCurrentTile(ref Node current, HashSet<Node> newOpenList = null, HashSet<Node> newClosedList = null)
    {
        newOpenList.Remove(current);
        newClosedList.Add(current);

        if (newOpenList.Count > 0) current = newOpenList.OrderBy(x => x.F).First();
    }

    private bool Algorithm()
    {
        Stack<Vector3Int> path = new Stack<Vector3Int>();

        Dictionary<Vector3Int, Node> newNodes = new Dictionary<Vector3Int, Node>();

        Vector3Int newStartPos = new Vector3Int(Mathf.FloorToInt(aStar.startPos.x), Mathf.FloorToInt(aStar.startPos.y));
        Vector3Int newGoalPos = new Vector3Int(Mathf.FloorToInt(aStar.endPos.x), Mathf.FloorToInt(aStar.endPos.y));

        Node currentNode = GetNode(newStartPos, newNodes);

        HashSet<Node> newOpenList = new HashSet<Node>();
        HashSet<Node> newClosedList = new HashSet<Node>();

        newOpenList.Add(currentNode);

        do
        {
            List<Node> neighbors = FindNeighbors(currentNode.position, newGoalPos, newNodes);

            ExamineNeighbors(neighbors, currentNode, newOpenList, newClosedList);

            UpdateCurrentTile(ref currentNode, newOpenList, newClosedList);

            path = GeneratePath(currentNode, newStartPos, newGoalPos);
        }
        while (newOpenList.Count > 0 && path == null);

        return path != null;
    }

    public void CheckRoomTiles()
    {
        if (allTiles == null || allTiles.Count <= 0) return;

        foreach (var tile in allTiles)
        {
            if (tile.Value.room != null && tile.Value.room.Count > 0)
                tile.Value.room.Clear();
        }

        RoomTile upTile = UpTile(prevCellPos);
        RoomTile downTile = DownTile(prevCellPos);
        RoomTile leftTile = LeftTile(prevCellPos);
        RoomTile rightTile = RightTile(prevCellPos);

        var room = RoomCheck(upTile);

        if (room != null)
        {
            foreach (var tile in room.tiles)
            {
                if (room.tiles.Count >= 6)
                    checkTilemap.SetTile(tile.savedTile.position, roomCheckTile_safe);
                else
                    checkTilemap.SetTile(tile.savedTile.position, roomCheckTile_None);
            }
        }

        room = RoomCheck(downTile);

        if (room != null)
        {
            foreach (var tile in room.tiles)
            {
                if (room.tiles.Count >= 6)
                    checkTilemap.SetTile(tile.savedTile.position, roomCheckTile_safe);
                else
                    checkTilemap.SetTile(tile.savedTile.position, roomCheckTile_None);
            }
        }

        room = RoomCheck(leftTile);

        if (room != null)
        {
            foreach (var tile in room.tiles)
            {
                if (room.tiles.Count >= 6)
                    checkTilemap.SetTile(tile.savedTile.position, roomCheckTile_safe);
                else
                    checkTilemap.SetTile(tile.savedTile.position, roomCheckTile_None);

            }
        }

        room = RoomCheck(rightTile);

        if (room != null)
        {
            foreach (var tile in room.tiles)
            {
                if (room.tiles.Count >= 6)
                    checkTilemap.SetTile(tile.savedTile.position, roomCheckTile_safe);
                else
                    checkTilemap.SetTile(tile.savedTile.position, roomCheckTile_None);

            }
        }
    }
    public RoomTile UpTile(Vector3Int pos)
    {
        if (allTiles.ContainsKey(pos + Vector3Int.up))
        {
            return allTiles[pos + Vector3Int.up];
        }
        else
        {
            return null;
        }
    }

    public RoomTile DownTile(Vector3Int pos)
    {
        if (allTiles.ContainsKey(pos + Vector3Int.down))
        {
            return allTiles[pos + Vector3Int.down];
        }
        else
        {
            return null;
        }
    }

    public RoomTile LeftTile(Vector3Int pos)
    {
        if (allTiles.ContainsKey(pos + Vector3Int.left))
        {
            return allTiles[pos + Vector3Int.left];
        }
        else
        {
            return null;
        }
    }

    public RoomTile RightTile(Vector3Int pos)
    {
        if (allTiles.ContainsKey(pos + Vector3Int.right))
        {
            return allTiles[pos + Vector3Int.right];
        }
        else
        {
            return null;
        }
    }

    public Room RoomCheck(RoomTile tile)
    {
        if (tile == null) return null;

        Room tempRoom = new Room();
        Queue<RoomTile> tileToCheck = new Queue<RoomTile>();

        if (allTiles.ContainsKey(prevCellPos))
        {
            if (aStar.IsObject(tile.savedTile.position))
            {
                if (aStar.IsObstacle(tile.savedTile.position))
                {
                    if (!tempRoom.walls.Contains(tile))
                        tempRoom.walls.Add(tile);
                }
                else
                {
                    if (!tempRoom.doors.Contains(tile))
                        tempRoom.doors.Add(tile);
                }
            }
            else
            {
                if (tile.room == null || tile.room.Count == 0)
                    tileToCheck.Enqueue(tile);
            }
        }

        while (tileToCheck.Count > 0)
        {
            if (tempRoom.tiles.Count > 25)
            {
                foreach (var tp in allTiles)
                {
                    if (tp.Value.isAlreadyCheck)
                    {
                        tp.Value.isAlreadyCheck = false;
                    }

                }

                foreach (var door in tempRoom.doors)
                {
                    if (door.room != null && door.room.Count > 0)
                        door.room.Remove(tempRoom);
                }

                foreach (var wall in tempRoom.walls)
                {
                    if (wall.room != null && wall.room.Count > 0)
                        wall.room.Remove(tempRoom);
                }

                return null;
            }

            RoomTile currentTile = tileToCheck.Dequeue();
            currentTile.isAlreadyCheck = true;

            tempRoom.tiles.Add(currentTile);

            RoomTile upTile = UpTile(currentTile.savedTile.position);

            if (upTile != null)
            {
                if (aStar.IsObject(upTile.savedTile.position) || upTile.savedTile.position == prevCellPos)
                {
                    if (aStar.IsObstacle(upTile.savedTile.position))
                    {
                        if (!tempRoom.walls.Contains(upTile))
                        {
                            tempRoom.walls.Add(upTile);
                            if (upTile.room == null) upTile.room = new();
                            if (!upTile.room.Contains(tempRoom))
                                upTile.room.Add(tempRoom);
                        }
                    }
                    else
                    {
                        if (!tempRoom.doors.Contains(upTile))
                        {
                            tempRoom.doors.Add(upTile);
                            if (upTile.room == null) upTile.room = new();
                            if (!upTile.room.Contains(tempRoom))
                                upTile.room.Add(tempRoom);
                        }
                    }
                }
                else
                {
                    if (!tileToCheck.Contains(upTile) && upTile.isAlreadyCheck == false) tileToCheck.Enqueue(upTile);
                }
            }

            RoomTile downTile = DownTile(currentTile.savedTile.position);

            if (downTile != null)
            {
                if (aStar.IsObject(downTile.savedTile.position) || downTile.savedTile.position == prevCellPos)
                {
                    if (aStar.IsObstacle(downTile.savedTile.position))
                    {
                        if (!tempRoom.walls.Contains(downTile))
                        {
                            tempRoom.walls.Add(downTile);
                            if (downTile.room == null) downTile.room = new();
                            if (!downTile.room.Contains(tempRoom))
                                downTile.room.Add(tempRoom);
                        }
                    }
                    else
                    {
                        if (!tempRoom.doors.Contains(downTile))
                        {
                            tempRoom.doors.Add(downTile);
                            if (downTile.room == null) { downTile.room = new(); }
                            if (!downTile.room.Contains(tempRoom))
                                downTile.room.Add(tempRoom);
                        }
                    }
                }
                else
                {
                    if (!tileToCheck.Contains(downTile) && downTile.isAlreadyCheck == false) tileToCheck.Enqueue(downTile);
                }
            }

            RoomTile leftTile = LeftTile(currentTile.savedTile.position);

            if (leftTile != null)
            {
                if (aStar.IsObject(leftTile.savedTile.position) || leftTile.savedTile.position == prevCellPos)
                {
                    if (aStar.IsObstacle(leftTile.savedTile.position))
                    {
                        if (!tempRoom.walls.Contains(leftTile))
                        {
                            tempRoom.walls.Add(leftTile);
                            if (leftTile.room == null) leftTile.room = new();
                            if (!leftTile.room.Contains(tempRoom))
                                leftTile.room.Add(tempRoom);
                        }
                    }
                    else
                    {
                        if (!tempRoom.doors.Contains(leftTile))
                        {
                            tempRoom.doors.Add(leftTile);
                            if (leftTile.room == null) leftTile.room = new();
                            if (!leftTile.room.Contains(tempRoom))
                                leftTile.room.Add(tempRoom);
                        }
                    }
                }
                else
                {
                    if (!tileToCheck.Contains(leftTile) && leftTile.isAlreadyCheck == false) tileToCheck.Enqueue(leftTile);
                }
            }

            RoomTile rightTile = RightTile(currentTile.savedTile.position);

            if (rightTile != null)
            {
                if (aStar.IsObject(rightTile.savedTile.position) || rightTile.savedTile.position == prevCellPos)
                {
                    if (aStar.IsObstacle(rightTile.savedTile.position))
                    {
                        if (!tempRoom.walls.Contains(rightTile))
                        {
                            tempRoom.walls.Add(rightTile);
                            if (rightTile.room == null) rightTile.room = new();
                            if (!rightTile.room.Contains(tempRoom))
                                rightTile.room.Add(tempRoom);
                        }
                    }
                    else
                    {
                        if (!tempRoom.doors.Contains(rightTile))
                        {
                            tempRoom.doors.Add(rightTile);
                            if (rightTile.room == null) rightTile.room = new();
                            if (!rightTile.room.Contains(tempRoom))
                                rightTile.room.Add(tempRoom);
                        }
                    }
                }
                else
                {
                    if (!tileToCheck.Contains(rightTile) && rightTile.isAlreadyCheck == false) tileToCheck.Enqueue(rightTile);
                }
            }
        }

        foreach (var tp in allTiles)
        {
            if (tp.Value.isAlreadyCheck)
            {
                tp.Value.isAlreadyCheck = false;
            }

        }

        tempRoom.tiles.Sort(delegate (RoomTile a, RoomTile b)
        {
            if (a.savedTile.position.x == b.savedTile.position.x)
            {
                return a.savedTile.position.y.CompareTo(b.savedTile.position.y);
            }
            else
            {
                return a.savedTile.position.x.CompareTo(b.savedTile.position.x);
            }
        });

        foreach (var tp in tempRoom.tiles)
        {
            if (tp.room == null) tp.room = new();
            if (!tp.room.Contains(tempRoom)) tp.room.Add(tempRoom);
        }

        return tempRoom;
    }

    public void ShowBluePrint(Vector3 position)
    {
        if (!aStar.isPossibleToBuild || GameManager.Instance.isProgressing) return;

        Vector3Int cellLocation = aStar.ObstacleWorldToCell(position);

        if (prevCellPos != cellLocation)
        {
            checkTilemap.ClearAllTiles();

            prevCellPos = cellLocation;

            if (aStar.tileType == TileType.WALL)
            {
                checkTilemap.SetTile(cellLocation, wallBluePrint);
            }
            else if (aStar.tileType == TileType.DOOR)
            {
                checkTilemap.SetTile(cellLocation, doorBluePrint);
            }

            if (aStar.IsNotRemovable(cellLocation) || !Algorithm())
            {
                checkTilemap.SetColor(cellLocation, Color.red);
            }
            else
            {
                CheckRoomTiles();

                checkTilemap.SetColor(cellLocation, Color.green);
            }
        }
    }

    public bool ResetAllTiles(BaseMessage msg)
    {
        if (msg == null)
        {
            Debug.Log("HandleYourMesssageListener : Message is null!");
            return false;
        }

        MapLoadFinishMessage castMsg = msg as MapLoadFinishMessage;

        if (castMsg == null)
        {
            Debug.Log("HandleYourMesssageListener : Cast Message is null!");
            return false;
        }

        Debug.Log(string.Format("HandleYourMesssageListener : Got the message! - {0}", castMsg.name));

        checkTilemap.RefreshAllTiles();

        return true;
    }
}
