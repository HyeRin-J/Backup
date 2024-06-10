using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomDetection : MonoBehaviour
{
    public Tilemap floorTilemap;
    public Tilemap decoTilemap;
    public Tilemap objectTilemap;

    [SerializeField]
    public List<Room> allRooms;

    [SerializeField]
    public Dictionary<Vector3Int, RoomTile> allTiles;

    public AStar aStar;
    public StageUIManager stageUIManager;

    private bool isInit = false;

    public void Initialize()
    {
        if (isInit == true) return;

        allTiles = new Dictionary<Vector3Int, RoomTile>();
        allRooms = new List<Room>();
        for (int x = -floorTilemap.size.x; x < floorTilemap.size.x; x++)
        {
            for (int y = -floorTilemap.size.y; y < floorTilemap.size.y; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);

                if (floorTilemap.GetTile(position) || objectTilemap.GetTile(position))
                {
                    RoomTile roomTile = new RoomTile();
                    roomTile.savedTile = new SavedTile();
                    roomTile.savedTile.position = position;

                    if (objectTilemap.GetTile(position))
                    {
                        roomTile.isWallOrDoor = true;
                        roomTile.savedTile.tileName = objectTilemap.GetTile(position).name;
                        if(roomTile.savedTile.tileName.Equals("door"))
                        {

                            aStar.bookMarks.Add(roomTile.savedTile.position);
                        }
                    }
                    else if (decoTilemap.GetTile(position))
                    {
                        roomTile.isWallOrDoor = false;
                        roomTile.savedTile.tileName = decoTilemap.GetTile(position).name;
                    }
                    else
                    {
                        roomTile.isWallOrDoor = false;
                        roomTile.savedTile.tileName = floorTilemap.GetTile(position).name;
                    }

                    roomTile.room = null;

                    allTiles.Add(position, roomTile);
                }
            }
        }

        isInit = true;
    }

    public void SetTileName(Vector3Int pos)
    {
        if (allTiles.ContainsKey(pos))
        {
            if (objectTilemap.GetTile(pos))
            {
                allTiles[pos].savedTile.tileName = objectTilemap.GetTile(pos).name;
            }
            else if (decoTilemap.GetTile(pos))
            {
                allTiles[pos].savedTile.tileName = decoTilemap.GetTile(pos).name;
            }
            else
            {
                allTiles[pos].savedTile.tileName = floorTilemap.GetTile(pos).name;
            }
        }
    }

    public bool isWallOrDoor(Vector3Int position)
    {
        if (allTiles.ContainsKey(position))
        {
            return allTiles[position].isWallOrDoor;
        }

        return false;
    }

    public List<Room> GetRoom(Vector3Int location)
    {
        if (allTiles.ContainsKey(location))
        {
            if (allTiles[location].room != null && allTiles[location].room.Count > 0)
                return allTiles[location].room;
        }

        return null;
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

    public bool IsPossibleToEdit(Vector3Int cellLocation)
    {
        RoomTile upTile = UpTile(cellLocation);
        RoomTile downTile = DownTile(cellLocation);
        RoomTile leftTile = LeftTile(cellLocation);
        RoomTile rightTile = RightTile(cellLocation);

        if ((upTile == null || (upTile != null && upTile.IsPossibleToEdit())) &&
            (downTile == null || (downTile != null && downTile.IsPossibleToEdit())) &&
            (leftTile == null || (leftTile != null && leftTile.IsPossibleToEdit())) &&
            (rightTile == null || (rightTile != null && rightTile.IsPossibleToEdit())))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void FloodFill(Vector3Int cellLocation, bool isLeft = true)
    {
        RoomTile currentTile = allTiles[cellLocation];
        currentTile.isWallOrDoor = isLeft;

        //현재 타일이 수정 불가능하면 리턴
        if (!currentTile.IsPossibleToEdit())
        {
            return;
        }

        RoomTile upTile = UpTile(cellLocation);
        RoomTile downTile = DownTile(cellLocation);
        RoomTile leftTile = LeftTile(cellLocation);
        RoomTile rightTile = RightTile(cellLocation);

        //있는 방을 삭제한다.
        if (upTile != null && !upTile.isWallOrDoor && upTile.room != null)
        {
            for (int i = 0; i < upTile.room.Count; i++)
            {
                allRooms.Remove(upTile.room[i]);
                stageUIManager.RemoveStatusPanel(upTile.room[i].statusPanel);
                upTile.room[i].Destroy();
            }
        }
        if (downTile != null && !downTile.isWallOrDoor && downTile.room != null)
        {
            for (int i = 0; i < downTile.room.Count; i++)
            {
                allRooms.Remove(downTile.room[i]);
                stageUIManager.RemoveStatusPanel(downTile.room[i].statusPanel);
                downTile.room[i].Destroy();
            }
        }
        if (leftTile != null && !leftTile.isWallOrDoor && leftTile.room != null)
        {
            for (int i = 0; i < leftTile.room.Count; i++)
            {
                allRooms.Remove(leftTile.room[i]);
                stageUIManager.RemoveStatusPanel(leftTile.room[i].statusPanel);
                leftTile.room[i].Destroy();
            }
        }
        if (rightTile != null && !rightTile.isWallOrDoor && rightTile.room != null)
        {
            for (int i = 0; i < rightTile.room.Count; i++)
            {
                allRooms.Remove(rightTile.room[i]);
                stageUIManager.RemoveStatusPanel(rightTile.room[i].statusPanel);
                rightTile.room[i].Destroy();
            }
        }

        //상하좌우 기준 새로운 방을 만든다
        Room newRoom = RoomCheck(upTile);
        if (newRoom != null)
        {
            if (aStar.IsObject(currentTile.savedTile.position))
            {
                if (aStar.IsObstacle(currentTile.savedTile.position))
                {
                    if (!newRoom.walls.Contains(currentTile))
                    {
                        newRoom.walls.Add(currentTile);
                        currentTile.room.Add(newRoom);
                    }
                }
                else
                {
                    if (!newRoom.doors.Contains(currentTile))
                    {
                        newRoom.doors.Add(currentTile);
                        currentTile.room.Add(newRoom);
                    }

                }
            }

            allRooms.Add(newRoom);
            newRoom.statusPanel = stageUIManager.CreateStatusPanel(newRoom.tiles[0].savedTile.position + new Vector3(0.1f, 0.5f));
            newRoom.Init();
        }

        newRoom = RoomCheck(downTile);
        if (newRoom != null)
        {
            if (aStar.IsObject(currentTile.savedTile.position))
            {
                if (aStar.IsObstacle(currentTile.savedTile.position))
                {
                    if (!newRoom.walls.Contains(currentTile))
                    {
                        newRoom.walls.Add(currentTile);
                        currentTile.room.Add(newRoom);
                    }
                }
                else
                {
                    if (!newRoom.doors.Contains(currentTile))
                    {
                        newRoom.doors.Add(currentTile);
                        currentTile.room.Add(newRoom);
                    }
                }
            }

            allRooms.Add(newRoom);
            newRoom.statusPanel = stageUIManager.CreateStatusPanel(newRoom.tiles[0].savedTile.position + new Vector3(0.1f, 0.5f));
            newRoom.Init();
        }

        newRoom = RoomCheck(leftTile);
        if (newRoom != null)
        {
            if (aStar.IsObject(currentTile.savedTile.position))
            {
                if (aStar.IsObstacle(currentTile.savedTile.position))
                {
                    if (!newRoom.walls.Contains(currentTile))
                    {
                        newRoom.walls.Add(currentTile);
                        currentTile.room.Add(newRoom);
                    }
                }
                else
                {
                    if (!newRoom.doors.Contains(currentTile))
                    {
                        newRoom.doors.Add(currentTile);
                        currentTile.room.Add(newRoom);
                    }
                }
            }

            allRooms.Add(newRoom);
            newRoom.statusPanel = stageUIManager.CreateStatusPanel(newRoom.tiles[0].savedTile.position + new Vector3(0.1f, 0.5f));
            newRoom.Init();
        }

        newRoom = RoomCheck(rightTile);
        if (newRoom != null)
        {
            if (aStar.IsObject(currentTile.savedTile.position))
            {
                if (aStar.IsObstacle(currentTile.savedTile.position))
                {
                    if (!newRoom.walls.Contains(currentTile))
                    {
                        newRoom.walls.Add(currentTile);
                        currentTile.room.Add(newRoom);
                    }
                }
                else
                {
                    if (!newRoom.doors.Contains(currentTile))
                    {
                        newRoom.doors.Add(currentTile);
                        currentTile.room.Add(newRoom);
                    }
                }
            }

            allRooms.Add(newRoom);
            newRoom.statusPanel = stageUIManager.CreateStatusPanel(newRoom.tiles[0].savedTile.position + new Vector3(0.1f, 0.5f));
            newRoom.Init();
        }
    }

    public Room RoomCheck(RoomTile tile)
    {
        if (tile == null) return null;

        Room tempRoom = new Room();
        Queue<RoomTile> tileToCheck = new Queue<RoomTile>();

        if (aStar.IsObject(tile.savedTile.position))
        {
            if (aStar.IsObstacle(tile.savedTile.position))
            {
                if (!tempRoom.walls.Contains(tile))
                {
                    tempRoom.walls.Add(tile);
                    if (tile.room == null) tile.room = new();
                    if (!tile.room.Contains(tempRoom))
                        tile.room.Add(tempRoom);
                }
            }
            else
            {
                if (!tempRoom.doors.Contains(tile))
                {
                    tempRoom.doors.Add(tile);
                    if (tile.room == null) tile.room = new();
                    if (!tile.room.Contains(tempRoom))
                        tile.room.Add(tempRoom);
                }
            }
        }
        else
        {
            if (tile.room == null || tile.room.Count == 0)
                tileToCheck.Enqueue(tile);
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
                    door.room.Remove(tempRoom);
                }

                foreach (var wall in tempRoom.walls)
                {
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
                if (aStar.IsObject(upTile.savedTile.position))
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
                if (aStar.IsObject(downTile.savedTile.position))
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
                if (aStar.IsObject(leftTile.savedTile.position))
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
                if (aStar.IsObject(rightTile.savedTile.position))
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

        if (tempRoom.tiles.Count >= 6 && tempRoom.doors.Count > 0)
        {
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
        else
        {
            foreach (var door in tempRoom.doors)
            {
                door.room.Remove(tempRoom);
            }

            foreach (var wall in tempRoom.walls)
            {
                wall.room.Remove(tempRoom);
            }

            return null;
        }
    }
}
