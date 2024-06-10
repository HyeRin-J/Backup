using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum TileType { WALL, DOOR, NONE, }

public class AStar : MonoBehaviour
{
    public AStarChecker aStarChecker;
    public AStarDebugger aStarDebugger;
    public RoomDetection roomDetection;

    [SerializeField]
    public TileType tileType;

    [SerializeField]
    private bool isBuild = true;

    public bool isLoadFinish = false;

    public int mapWidth = 10, mapHeight = 10;

    [SerializeField]
    private Tilemap obstacleTilemap, floorTilemap;

    private List<Vector3Int> obstacles = new List<Vector3Int>();
    private List<Vector3Int> notRemoveObstacles = new List<Vector3Int>();
    private List<Vector3Int> notRemoveObjects = new List<Vector3Int>();
    public List<Vector3Int> changeTiles = new List<Vector3Int>();
    public List<Vector3Int> bookMarks = new();

    [SerializeField]
    private RuleTile wall, door;

    public Vector3Int startPos, endPos;

    private Node current;
    private HashSet<Node> openList, closedList;

    private Stack<Vector3Int> path;
    public Stack<Vector3Int> completePath;

    private Dictionary<Vector3Int, Node> allNodes = new Dictionary<Vector3Int, Node>();

    public RectTransform renderTexture;
    public Camera uiCamera;
    public Camera inputCamera;
    public bool isDebug;

    public AudioSource audioSource;

    public AudioClip[] audioClips;

    public bool isPossibleToBuild = true;

    private void Awake()
    {
        MessageQueue.Instance.AttachListener(typeof(MapLoadFinishMessage), LoadFinish);
        MessageQueue.Instance.AttachListener(typeof(SetInputCameraMessage), SetInputCamera);
    }

    private void OnDestroy()
    {
        MessageQueue.Instance.DetachListener(typeof(MapLoadFinishMessage), LoadFinish);
        MessageQueue.Instance.DetachListener(typeof(SetInputCameraMessage), SetInputCamera);
    }

    //Ray ������ ���� InputCamera ����
    public bool SetInputCamera(BaseMessage msg)
    {
        if (msg == null)
        {
            Debug.Log("HandleYourMesssageListener : Message is null!");
            return false;
        }

        SetInputCameraMessage castMsg = msg as SetInputCameraMessage;

        if (castMsg == null)
        {
            Debug.Log("HandleYourMesssageListener : Cast Message is null!");
            return false;
        }

        Debug.Log(string.Format("HandleYourMesssageListener : Got the message! - {0}", castMsg.name));

        inputCamera = castMsg.inputCamera;

        return true;
    }

    //�� �Ŵ������� �ε��� ������ �� ȣ��ȴ�
    public bool LoadFinish(BaseMessage msg)
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

        isLoadFinish = true;

        //�� �Ŵ������� ���� ũ�⸦ �޾ƿ´�
        mapWidth = castMsg.mapWidth;
        mapHeight = castMsg.mapHeight;
        startPos = castMsg.startPos;
        endPos = castMsg.endPos;

        //��θ� �����Ѵ�
        ResetPath();

        //��θ� üũ�Ͽ� ������θ� �����Ѵ�
        if (Algorithm())
        {
            completePath = path;

        }

        if (isDebug)
            aStarDebugger.CreateTiles(openList, closedList, allNodes, completePath, startPos, endPos);

        //�� üũ �ʱ�ȭ
        roomDetection.Initialize();
        //AStar û���� �ʱ�ȭ
        aStarChecker.Initialize();

        MessageQueue.Instance.QueueMesssage(new ChangeWallOrDoorMessage(false, false));

        return true;
    }

    //���� ��ġ�� ������Ʈ�̸� true�� ��ȯ
    public bool IsObject(Vector3Int pos)
    {
        if (obstacles.Contains(pos) || notRemoveObstacles.Contains(pos) || notRemoveObjects.Contains(pos) || changeTiles.Contains(pos))
        {
            return true;
        }
        return false;
    }

    //�ʿ� ���� �־��� ������Ʈ�� ��ֹ����� ��ġ ������ �ʱ�ȭ
    public void ClearAllNotRemovableObjectsAndObstacles()
    {
        foreach (var tile in notRemoveObjects)
        {
            roomDetection.allTiles[tile].isWallOrDoor = false;
        }
        notRemoveObjects.Clear();

        foreach (var tile in notRemoveObjects)
        {
            roomDetection.allTiles[tile].isWallOrDoor = false;
        }
        notRemoveObjects.Clear();
    }

    //����� ���� ���
    private void PlayRandom()
    {
        audioSource.clip = audioClips[UnityEngine.Random.Range(0, audioClips.Length)];
        audioSource.Play();
    }

    // ��Ŭ���� �ʿ� ������Ʈ�� ��ġ�Ѵ�
    public void OnClickLeftButton(Vector3 position)
    {
        // ���� �Ұ��� �����̰ų� ������ �������̸� ����
        if (isPossibleToBuild == false || GameManager.Instance.isProgressing) return;

        // ���� ��ġ�� ���� ��ȯ
        Vector3Int cellLocation = ObstacleWorldToCell(position);

        // ���� ��ġ�� ������Ʈ, ��������, �������̸� ����
        if (notRemoveObjects.Contains(cellLocation) || (cellLocation.Equals(startPos) || cellLocation.Equals(endPos)))
        {
            GameManager.Instance.scriptManager.SetCurrentCondition(Conditions.�⺻_������_�����õ�);

            return;
        }

        // ���� ��ġ�� ���� ���� �Ұ����ϸ� ����
        if (!roomDetection.IsPossibleToEdit(cellLocation))
        {
            GameManager.Instance.scriptManager.SetCurrentCondition(Conditions.����_��_�����õ�);
        }

        // �ε��� ������, ������ ���������� �ʰ�, ī�带 �巡�� ������ ���� ���
        if (isLoadFinish && !GameManager.Instance.isProgressing && !GameManager.Instance.isCardDrag)
        {
            // ���� ��ġ�� ���� ���� ���� ���
            if (roomDetection.IsPossibleToEdit(cellLocation) &&
                cellLocation.x > -mapWidth / 2 && cellLocation.x < mapWidth / 2 - 1 && cellLocation.y > -mapHeight / 2 && cellLocation.y < mapHeight / 2 - 1)
            {
                //���� ���� ��ȯ
                isBuild = true;

                //���� ��ġ�� ������Ʈ Ÿ�� ������ ������
                CustomRuleTile ruleTile = obstacleTilemap.GetTile(cellLocation) as CustomRuleTile;

                //���� Ÿ�� ����
                ChangeTile(cellLocation);

                //��� ��Ž��
                ResetPath();

                //��ΰ� ������
                if (Algorithm() == false)
                {
                    //�����Ǿ��� Ÿ�ϵ��� ���� ����
                    obstacles.Remove(cellLocation);
                    changeTiles.Remove(cellLocation);
                    obstacleTilemap.SetTile(cellLocation, null);

                    //���� Ÿ�Ϸ� ����
                    if (ruleTile != null && ruleTile.name.Equals("Door"))
                    {
                        obstacleTilemap.SetTile(cellLocation, door);
                    }

                    // ��ũ��Ʈ ���
                    if (tileType == TileType.WALL)
                        GameManager.Instance.scriptManager.SetCurrentCondition(Conditions.����ġ_����);
                }
                else
                {
                    //��� �� ����
                    completePath = path;

                    //�� üũ
                    roomDetection.FloodFill(cellLocation);

                    // �� ��ġ ���� ����
                    if (tileType == TileType.DOOR)
                    {
                        bookMarks.Add(cellLocation);
                    }
                    //���� ���
                    if (!audioSource.isPlaying)
                    {
                        PlayRandom();
                    }
                }
            }
        }

        //����׿�
        if (isDebug)
            aStarDebugger.CreateTiles(openList, closedList, allNodes, completePath, startPos, endPos);
    }

    //��Ŭ���� ���� ������Ʈ�� ����
    public void OnClickRightButton(Vector3 position)
    {
        // ���� �Ұ��� ����, Ʃ�丮���� ������ �ʾ���, ������ �������̸� ����
        if (isPossibleToBuild == false || GameManager.Instance.isTutorialFinish[0] == false || GameManager.Instance.isProgressing) return;

        // ���� ��ġ�� ���� ��ȯ
        Vector3Int cellLocation = ObstacleWorldToCell(position);

        // ���� ��ġ�� ������ Ÿ���� �ƴ� ���,
        // �⺻������Ʈ, ��������, �������̸� ����
        if ((!changeTiles.Contains(cellLocation) || notRemoveObjects.Contains(cellLocation))
            || (cellLocation.Equals(startPos) || cellLocation.Equals(endPos)))
        {
            if (notRemoveObjects.Contains(cellLocation)) GameManager.Instance.scriptManager.SetCurrentCondition(Conditions.�⺻_������_�����õ�);

            return;
        }

        // ���� ��ġ�� ���� ���� �Ұ����ϸ� ����
        if (!roomDetection.IsPossibleToEdit(cellLocation))
        {
            GameManager.Instance.scriptManager.SetCurrentCondition(Conditions.����_��_�����õ�);
        }

        // �ε��� ������, ������ ���������� �ʰ�, ī�带 �巡�� ������ ���� ���
        if (isLoadFinish && !GameManager.Instance.isProgressing && !GameManager.Instance.isCardDrag)
        {
            // ���� ��� ����
            isBuild = false;

            // ���� ��ġ�� ���� ������ ����� ����
            if (roomDetection.IsPossibleToEdit(cellLocation) &&
                cellLocation.x > -mapWidth / 2 && cellLocation.x < mapWidth / 2 - 1 && cellLocation.y > -mapHeight / 2 && cellLocation.y < mapHeight / 2 - 1)
            {
                // ���� Ÿ�� ����
                ChangeTile(cellLocation);

                //��ũ��Ʈ ���
                if (obstacles.Contains(cellLocation))
                {
                    GameManager.Instance.scriptManager.SetCurrentCondition(Conditions.��_�ı�);
                }
                else if (changeTiles.Contains(cellLocation))
                {
                    bookMarks.Remove(cellLocation);
                    GameManager.Instance.scriptManager.SetCurrentCondition(Conditions.��_����);
                }

                //��� ��Ž��
                ResetPath();

                //��ΰ� ������ �缳��
                if (Algorithm())
                {
                    completePath = path;

                    roomDetection.FloodFill(cellLocation, false);

                    if (!audioSource.isPlaying)
                    {
                        PlayRandom();
                    }
                }
            }
            if (isDebug)
                aStarDebugger.CreateTiles(openList, closedList, allNodes, completePath, startPos, endPos);
        }
    }

    //������ �� ���� Ÿ�� ����
    public bool IsNotRemovable(Vector3Int pos)
    {
        if (notRemoveObjects.Contains(pos) || notRemoveObstacles.Contains(pos))
        {
            return true;
        }
        return false;
    }
    
    //��ֹ��� ����
    public bool IsObstacle(Vector3Int pos)
    {
        return notRemoveObstacles.Contains(pos)
            || obstacles.Contains(pos);
    }

    //cellPos�� worldPos�� ��ȯ
    public Vector3 ObjstacleCellToWorld(Vector3Int cellPos)
    {
        return obstacleTilemap.CellToWorld(cellPos);
    }

    // worldPos�� tilemap�� cellPos�� ��ȯ
    public Vector3Int RectWorldToCell(Tilemap tilemap)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(renderTexture, Input.mousePosition, uiCamera, out Vector2 localPos))
        {
            var rect = renderTexture.rect;

            localPos.x += renderTexture.pivot.x;
            localPos.y += renderTexture.pivot.y;

            Vector2 normalPoint = Rect.PointToNormalized(rect, localPos);
            Ray ray = inputCamera.ViewportPointToRay(normalPoint);

            Plane plane = new Plane(Vector3.up, Vector3.zero);

            float d;
            plane.Raycast(ray, out d);

            Vector3 hit = ray.GetPoint(d);

            return tilemap.WorldToCell(hit);
        }
        else
        {
            return new Vector3Int(-99, -99, -99);
        }
    }

    // worldPos�� ObstacleTilemap�� cellPos�� ��ȯ
    public Vector3Int ObstacleWorldToCell(Vector3 pos)
    {
        pos.z = 100;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(renderTexture, pos, uiCamera, out Vector2 localPos))
        {
            var rect = renderTexture.rect;

            localPos.x += renderTexture.pivot.x;
            localPos.y += renderTexture.pivot.y;

            Vector2 normalPoint = Rect.PointToNormalized(rect, localPos);
            Ray ray = inputCamera.ViewportPointToRay(normalPoint);

            Plane plane = new Plane(Vector3.up, Vector3.zero);

            float d;
            plane.Raycast(ray, out d);

            Vector3 hit = ray.GetPoint(d);

            return obstacleTilemap.WorldToCell(hit);
        }
        else
        {
            return new Vector3Int(-99, -99, -99);
        }

    }

    //�ʱ�ȭ
    private void Initialize()
    {
        //���ο� ��� ����
        current = GetNode(startPos);

        openList = new HashSet<Node>();
        closedList = new HashSet<Node>();

        openList.Add(current);
    }

    // Path�� ��ȯ�ϴ� A* ��� Ž�� �˰���
    public Stack<Vector3Int> Algorithm(Vector3 start, Vector3 goal)
    {
        //�� ��� ����
        Stack<Vector3Int> path = new Stack<Vector3Int>();

        //�� ��� ����
        Dictionary<Vector3Int, Node> newNodes = new Dictionary<Vector3Int, Node>();

        // ���ο� �������� �� ����
        Vector3Int newStartPos = new Vector3Int(Mathf.FloorToInt(start.x), Mathf.FloorToInt(start.y));
        Vector3Int newGoalPos = new Vector3Int(Mathf.FloorToInt(goal.x), Mathf.FloorToInt(goal.y));

        //���� ��� ����
        Node currentNode = GetNode(newStartPos, newNodes);
        
        //���ο� openList�� closedList ����
        HashSet<Node> newOpenList = new HashSet<Node>();
        HashSet<Node> newClosedList = new HashSet<Node>();

        newOpenList.Add(currentNode);

        //��ΰ� ����ų�, Ž���ؾ� �� openList�� ���� ������ �ݺ�
        do
        {
            //���� ����� �̿� ��� ã��
            List<Node> neighbors = FindNeighbors(currentNode.position, newGoalPos, newNodes);

            //�̿� ��� �˻�
            ExamineNeighbors(neighbors, currentNode, newOpenList, newClosedList);

            //���� ��� ������Ʈ
            UpdateCurrentTile(ref currentNode, newOpenList, newClosedList);

            //��� ����
            path = GeneratePath(currentNode, newStartPos, newGoalPos);
        }
        while (newOpenList.Count > 0 && path == null);

        return path;
    }

    //�˰��� ������ ��� ��������
    public void DoAstarAlgorithm()
    {
        if (Algorithm())
        {
            completePath = path;
        }
    }

    // ����� �����ϴ��� �˻��ϴ� A* ��� Ž�� �˰���
    private bool Algorithm()
    {
        //���� ��尡 ������ �ʱ�ȭ
        if (current == null)
        {
            Initialize();
        }

        // ��� ����
        //��ΰ� ����ų�, Ž���ؾ� �� openList�� ���� ������ �ݺ�
        while (openList.Count > 0 && path == null)
        {
            //���� ����� �̿� ��� ã��
            List<Node> neighbors = FindNeighbors(current.position, endPos, allNodes);
            
            // �̿� ��� �˻�
            ExamineNeighbors(neighbors, current);

            //���� ��� ������Ʈ
            UpdateCurrentTile(ref current);

            //��� ����
            path = GeneratePath(current, startPos, endPos);
        }

        //��ΰ� ������ true ��ȯ
        return path != null;
    }

    //�̿� ��� ã��
    private List<Node> FindNeighbors(Vector3Int parentPosition, Vector3Int goalPos, Dictionary<Vector3Int, Node> newNodes)
    {
        //�̿� ��� ����Ʈ ����
        List<Node> neighbors = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                //���� ���� �Ѿ
                if (x == 0 && y == 0)
                {
                    continue;
                }

                //�̿� ����� ��ġ
                Vector3Int neighborPos = new Vector3Int(parentPosition.x + x, parentPosition.y + y, 0);

                //FloorTilemap�� �־�� �ϰ�, �̿� ��尡 ��ǥ�����̰ų� ��ֹ��� �ƴϸ� �̿� ��� ����Ʈ�� �߰�
                if (floorTilemap.GetTile(neighborPos) != null && (neighborPos == goalPos || !IsObstacle(neighborPos)))
                {
                    Node neighbor = GetNode(neighborPos, newNodes);
                    neighbors.Add(neighbor);
                }
            }
        }

        return neighbors;
    }

    //�̿� ��� �˻�
    private void ExamineNeighbors(List<Node> neighbors, Node current, HashSet<Node> newOpenList = null, HashSet<Node> newClosedList = null)
    {
        //���� �̿����� �ݺ�
        for (int i = 0; i < neighbors.Count; ++i)
        {
            Node neighbor = neighbors[i];

            //�밢�� �˻�
            if (!ConnectedDiagonally(current, neighbor))
            {
                continue;
            }

            //�̿� ����� G�� ���
            int gScore = DetermineGScore(neighbor.position, current.position);

            //���ο� openList�� closedList�� ������ ������ �� ���
            //(������ �̵� �ÿ��� ���ο� openList�� closedList�� ���)
            if (newOpenList == null || newClosedList == null)
            {
                //�̿� ��尡 openList�� ������
                if (openList.Contains(neighbor))
                {
                    //�̿������ G���� ���� ����� G���� ��
                    if (current.G + gScore < neighbor.G)
                    {
                        //�̿���� ���
                        CalcValues(current, neighbor, gScore);
                    }
                }
                //�̿� ��尡 closedList�� ������
                else if (!closedList.Contains(neighbor))
                {
                    //�̿���� ���
                    CalcValues(current, neighbor, gScore);

                    //openList�� �̿� ��� �߰�
                    openList.Add(neighbor);
                }
            }
            else
            {
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
    }

    //���� ���� �̿������ G, H, F �� ���
    private void CalcValues(Node parent, Node neighbor, int cost)
    {
        //�̿� ����� �θ� ���� ���� ����
        neighbor.parent = parent;

        //�̿� ����� G, H, F �� ���
        neighbor.G = parent.G + cost;
        neighbor.H = (Mathf.Abs(neighbor.position.x - endPos.x) + Mathf.Abs(neighbor.position.y - endPos.y)) * 10;
        neighbor.F = neighbor.G + neighbor.H;
    }

    //G�� �Ի�
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

    //���� Ÿ�� ������Ʈ
    private void UpdateCurrentTile(ref Node current, HashSet<Node> newOpenList = null, HashSet<Node> newClosedList = null)
    {
        if (newOpenList == null)
            openList.Remove(current);
        else
            newOpenList.Remove(current);

        if (newClosedList == null)
            closedList.Add(current);
        else
            newClosedList.Add(current);

        if (newOpenList == null)
        {
            //F���� �������� openList ����
            if (openList.Count > 0) current = openList.OrderBy(x => x.F).First();

        }
        else
        {
            if (newOpenList.Count > 0) current = newOpenList.OrderBy(x => x.F).First();
        }
    }

    //�밢���� ������Ʈ�� ��ֹ��� �ִ��� �˻�
    private bool ConnectedDiagonally(Node currentNode, Node neighbor)
    {
        Vector3Int direction = neighbor.position - currentNode.position;

        Vector3Int first = new Vector3Int(currentNode.position.x + direction.x, currentNode.position.y, currentNode.position.z);
        Vector3Int second = new Vector3Int(currentNode.position.x, currentNode.position.y + direction.y, currentNode.position.z);

        if ((obstacles.Contains(first) || notRemoveObstacles.Contains(first)) ||
            (obstacles.Contains(second) || notRemoveObstacles.Contains(second)))
        {
            return false;
        }

        return true;
    }

    //��� ����
    private Stack<Vector3Int> GeneratePath(Node current, Vector3Int startPos, Vector3Int goalPos)
    {
        //���� ��尡 ��ǥ�����̸�
        if (current.position == goalPos)
        {
            //���� ��� ����
            Stack<Vector3Int> finalPath = new Stack<Vector3Int>();

            //���� ��尡 ���������� �� ������ �ݺ�
            while (current.position != startPos)
            {
                int x = current.position.x - current.parent.position.x;
                int y = current.position.y - current.parent.position.y;

                //���� ��带 ���� ��ο� �߰�
                finalPath.Push(current.position);

                if (Mathf.Abs(x - y) % 2 != 1)
                {
                    Node node = GetDiagonalNodeBetween(current.parent.position, current.position);

                    finalPath.Push(node.position);
                }

                //���� ��带 �θ� ���� ����
                current = current.parent;
            }

            return finalPath;
        }

        return null;
    }

    //�밢�� ��带 �ڿ������� ����
    private Node GetDiagonalNodeBetween(Vector3Int pos1, Vector3Int pos2)
    {
        Vector3Int direction = pos2 - pos1;

        Vector3Int first = new Vector3Int(pos1.x + direction.x, pos1.y, pos1.z);
        Vector3Int second = new Vector3Int(pos1.x, pos1.y + direction.y, pos1.z);

        if (obstacles.Contains(first) || notRemoveObstacles.Contains(first))
        {
            return GetNode(second);
        }
        else
        {
            return GetNode(first);
        }
    }

    //��� ��������
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

    //Ÿ�� Ÿ�� ����
    public void ChangeTileType(TileType newTileType)
    {
        tileType = newTileType;
    }

    //clickPos Ÿ�� ����
    public void ChangeTile(Vector3Int clickPos)
    {
        //�������̸�
        if (isBuild)
        {
            switch (tileType)
            {
                case TileType.WALL:
                    if (!obstacles.Contains(clickPos))
                    {
                        obstacles.Add(clickPos);
                    }
                    obstacleTilemap.SetTile(clickPos, wall);
                    break;
                case TileType.DOOR:
                    obstacleTilemap.SetTile(clickPos, door);
                    break;
                default:
                    break;
            }

            //Ʃ�丮�� �Ϸ� ������ ȣ��
            if (!changeTiles.Contains(clickPos))
            {
                if (!GameManager.Instance.isTutorialFinish[0])
                {
                    MessageQueue.Instance.QueueMesssage(new AddTutorialValueMessage());
                }
            }

            //changeTiles�� ���ԵǾ� ���� ������ �߰�
            if (!changeTiles.Contains(clickPos))
                changeTiles.Add(clickPos);
        }
        //���Ÿ��
        else
        {
            //��ֹ��� ���ԵǸ� ����
            if (obstacles.Contains(clickPos))
            {
                obstacles.Remove(clickPos);
            }

            //ClickPos�ʱ�ȭ
            obstacleTilemap.SetTile(clickPos, null);

            //ChangeTile ����
            changeTiles.Remove(clickPos);
        }

        //Tilemap Refresh
        obstacleTilemap.RefreshAllTiles();

        //û���� ����
        aStarChecker.checkTilemap.ClearAllTiles();
    }

    //��ֹ��� �߰�
    public void AddObstacles(Vector3Int pos)
    {
        if (!obstacles.Contains(pos))
        {
            obstacles.Add(pos);
        }
    }

    //���� �Ұ����� ��ֹ��� �߰�
    public void AddNotRemoveObstacles(Vector3Int pos)
    {
        if (!notRemoveObstacles.Contains(pos))
        {
            notRemoveObstacles.Add(pos);
        }
    }

    //���� �Ұ����� ������Ʈ �߰�
    public void AddNotRemoveObjects(Vector3Int pos)
    {
        if (!notRemoveObjects.Contains(pos))
        {
            notRemoveObjects.Add(pos);
        }
    }

    //��� �ʱ�ȭ
    public void ResetPath()
    {
        aStarDebugger.ResetDebugger(allNodes);

        path = null;
        current = null;

        if (openList != null) openList.Clear();

        if (closedList != null) closedList.Clear();
    }

    //���� ����� ���� ���� ��ġ�� ���� �� ���� ������
    public void RefreshNextPhase()
    {
        foreach (var tile in changeTiles)
        {
            if (obstacles.Contains(tile))
            {
                obstacleTilemap.SetTile(tile, wall);
            }
            else
            {
                obstacleTilemap.SetTile(tile, door);
            }

            obstacleTilemap.RefreshTile(tile);
        }
    }
}
