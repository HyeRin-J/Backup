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

    //Ray 검출을 위한 InputCamera 설정
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

    //맵 매니저에서 로딩이 끝났을 때 호출된다
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

        //맵 매니저에서 맵의 크기를 받아온다
        mapWidth = castMsg.mapWidth;
        mapHeight = castMsg.mapHeight;
        startPos = castMsg.startPos;
        endPos = castMsg.endPos;

        //경로를 리셋한다
        ResetPath();

        //경로를 체크하여 최종경로를 저장한다
        if (Algorithm())
        {
            completePath = path;

        }

        if (isDebug)
            aStarDebugger.CreateTiles(openList, closedList, allNodes, completePath, startPos, endPos);

        //방 체크 초기화
        roomDetection.Initialize();
        //AStar 청사진 초기화
        aStarChecker.Initialize();

        MessageQueue.Instance.QueueMesssage(new ChangeWallOrDoorMessage(false, false));

        return true;
    }

    //현재 위치가 오브젝트이면 true를 반환
    public bool IsObject(Vector3Int pos)
    {
        if (obstacles.Contains(pos) || notRemoveObstacles.Contains(pos) || notRemoveObjects.Contains(pos) || changeTiles.Contains(pos))
        {
            return true;
        }
        return false;
    }

    //맵에 원래 있었던 오브젝트와 장애물등의 위치 정보를 초기화
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

    //오디오 랜덤 출력
    private void PlayRandom()
    {
        audioSource.clip = audioClips[UnityEngine.Random.Range(0, audioClips.Length)];
        audioSource.Play();
    }

    // 좌클릭시 맵에 오브젝트를 설치한다
    public void OnClickLeftButton(Vector3 position)
    {
        // 수정 불가능 상태이거나 게임이 진행중이면 리턴
        if (isPossibleToBuild == false || GameManager.Instance.isProgressing) return;

        // 현재 위치를 셀로 변환
        Vector3Int cellLocation = ObstacleWorldToCell(position);

        // 현재 위치가 오브젝트, 시작지점, 끝지점이면 리턴
        if (notRemoveObjects.Contains(cellLocation) || (cellLocation.Equals(startPos) || cellLocation.Equals(endPos)))
        {
            GameManager.Instance.scriptManager.SetCurrentCondition(Conditions.기본_구조물_수정시도);

            return;
        }

        // 현재 위치의 방이 수정 불가능하면 리턴
        if (!roomDetection.IsPossibleToEdit(cellLocation))
        {
            GameManager.Instance.scriptManager.SetCurrentCondition(Conditions.유닛_방_수정시도);
        }

        // 로딩이 끝났고, 게임이 진행중이지 않고, 카드를 드래그 중이지 않을 경우
        if (isLoadFinish && !GameManager.Instance.isProgressing && !GameManager.Instance.isCardDrag)
        {
            // 현재 위치가 맵의 범위 안일 경우
            if (roomDetection.IsPossibleToEdit(cellLocation) &&
                cellLocation.x > -mapWidth / 2 && cellLocation.x < mapWidth / 2 - 1 && cellLocation.y > -mapHeight / 2 && cellLocation.y < mapHeight / 2 - 1)
            {
                //빌드 모드로 전환
                isBuild = true;

                //현재 위치에 오브젝트 타일 정보를 가져옴
                CustomRuleTile ruleTile = obstacleTilemap.GetTile(cellLocation) as CustomRuleTile;

                //현재 타일 수정
                ChangeTile(cellLocation);

                //경로 재탐색
                ResetPath();

                //경로가 없으면
                if (Algorithm() == false)
                {
                    //수정되었던 타일들을 전부 제거
                    obstacles.Remove(cellLocation);
                    changeTiles.Remove(cellLocation);
                    obstacleTilemap.SetTile(cellLocation, null);

                    //원래 타일로 복구
                    if (ruleTile != null && ruleTile.name.Equals("Door"))
                    {
                        obstacleTilemap.SetTile(cellLocation, door);
                    }

                    // 스크립트 출력
                    if (tileType == TileType.WALL)
                        GameManager.Instance.scriptManager.SetCurrentCondition(Conditions.벽설치_실패);
                }
                else
                {
                    //경로 재 지정
                    completePath = path;

                    //방 체크
                    roomDetection.FloodFill(cellLocation);

                    // 문 위치 정보 저장
                    if (tileType == TileType.DOOR)
                    {
                        bookMarks.Add(cellLocation);
                    }
                    //사운드 출력
                    if (!audioSource.isPlaying)
                    {
                        PlayRandom();
                    }
                }
            }
        }

        //디버그용
        if (isDebug)
            aStarDebugger.CreateTiles(openList, closedList, allNodes, completePath, startPos, endPos);
    }

    //우클릭시 맵의 오브젝트를 제거
    public void OnClickRightButton(Vector3 position)
    {
        // 수정 불가능 상태, 튜토리얼이 끝나지 않았음, 게임이 진행중이면 리턴
        if (isPossibleToBuild == false || GameManager.Instance.isTutorialFinish[0] == false || GameManager.Instance.isProgressing) return;

        // 현재 위치를 셀로 변환
        Vector3Int cellLocation = ObstacleWorldToCell(position);

        // 현재 위치가 수정한 타일이 아닌 경우,
        // 기본오브젝트, 시작지점, 끝지점이면 리턴
        if ((!changeTiles.Contains(cellLocation) || notRemoveObjects.Contains(cellLocation))
            || (cellLocation.Equals(startPos) || cellLocation.Equals(endPos)))
        {
            if (notRemoveObjects.Contains(cellLocation)) GameManager.Instance.scriptManager.SetCurrentCondition(Conditions.기본_구조물_수정시도);

            return;
        }

        // 현재 위치의 방이 수정 불가능하면 리턴
        if (!roomDetection.IsPossibleToEdit(cellLocation))
        {
            GameManager.Instance.scriptManager.SetCurrentCondition(Conditions.유닛_방_수정시도);
        }

        // 로딩이 끝났고, 게임이 진행중이지 않고, 카드를 드래그 중이지 않을 경우
        if (isLoadFinish && !GameManager.Instance.isProgressing && !GameManager.Instance.isCardDrag)
        {
            // 빌드 모드 해제
            isBuild = false;

            // 현재 위치가 맵의 범위를 벗어나지 않음
            if (roomDetection.IsPossibleToEdit(cellLocation) &&
                cellLocation.x > -mapWidth / 2 && cellLocation.x < mapWidth / 2 - 1 && cellLocation.y > -mapHeight / 2 && cellLocation.y < mapHeight / 2 - 1)
            {
                // 현재 타일 수정
                ChangeTile(cellLocation);

                //스크립트 출력
                if (obstacles.Contains(cellLocation))
                {
                    GameManager.Instance.scriptManager.SetCurrentCondition(Conditions.벽_파괴);
                }
                else if (changeTiles.Contains(cellLocation))
                {
                    bookMarks.Remove(cellLocation);
                    GameManager.Instance.scriptManager.SetCurrentCondition(Conditions.문_제거);
                }

                //경로 재탐색
                ResetPath();

                //경로가 있으면 재설정
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

    //삭제할 수 없는 타일 지정
    public bool IsNotRemovable(Vector3Int pos)
    {
        if (notRemoveObjects.Contains(pos) || notRemoveObstacles.Contains(pos))
        {
            return true;
        }
        return false;
    }
    
    //장애물로 지정
    public bool IsObstacle(Vector3Int pos)
    {
        return notRemoveObstacles.Contains(pos)
            || obstacles.Contains(pos);
    }

    //cellPos를 worldPos로 변환
    public Vector3 ObjstacleCellToWorld(Vector3Int cellPos)
    {
        return obstacleTilemap.CellToWorld(cellPos);
    }

    // worldPos를 tilemap의 cellPos로 변환
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

    // worldPos를 ObstacleTilemap의 cellPos로 변환
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

    //초기화
    private void Initialize()
    {
        //새로운 노드 생성
        current = GetNode(startPos);

        openList = new HashSet<Node>();
        closedList = new HashSet<Node>();

        openList.Add(current);
    }

    // Path를 반환하는 A* 경로 탐색 알고리즘
    public Stack<Vector3Int> Algorithm(Vector3 start, Vector3 goal)
    {
        //새 경로 생성
        Stack<Vector3Int> path = new Stack<Vector3Int>();

        //새 노드 생성
        Dictionary<Vector3Int, Node> newNodes = new Dictionary<Vector3Int, Node>();

        // 새로운 시작점과 끝 생성
        Vector3Int newStartPos = new Vector3Int(Mathf.FloorToInt(start.x), Mathf.FloorToInt(start.y));
        Vector3Int newGoalPos = new Vector3Int(Mathf.FloorToInt(goal.x), Mathf.FloorToInt(goal.y));

        //현재 노드 지정
        Node currentNode = GetNode(newStartPos, newNodes);
        
        //새로운 openList와 closedList 생성
        HashSet<Node> newOpenList = new HashSet<Node>();
        HashSet<Node> newClosedList = new HashSet<Node>();

        newOpenList.Add(currentNode);

        //경로가 생기거나, 탐색해야 할 openList가 없을 때까지 반복
        do
        {
            //현재 노드의 이웃 노드 찾기
            List<Node> neighbors = FindNeighbors(currentNode.position, newGoalPos, newNodes);

            //이웃 노드 검사
            ExamineNeighbors(neighbors, currentNode, newOpenList, newClosedList);

            //현재 노드 업데이트
            UpdateCurrentTile(ref currentNode, newOpenList, newClosedList);

            //경로 생성
            path = GeneratePath(currentNode, newStartPos, newGoalPos);
        }
        while (newOpenList.Count > 0 && path == null);

        return path;
    }

    //알고리즘 돌리고 경로 지정까지
    public void DoAstarAlgorithm()
    {
        if (Algorithm())
        {
            completePath = path;
        }
    }

    // 골까지 도달하는지 검사하는 A* 경로 탐색 알고리즘
    private bool Algorithm()
    {
        //현재 노드가 없으면 초기화
        if (current == null)
        {
            Initialize();
        }

        // 경로 생성
        //경로가 생기거나, 탐색해야 할 openList가 없을 때까지 반복
        while (openList.Count > 0 && path == null)
        {
            //현재 노드의 이웃 노드 찾기
            List<Node> neighbors = FindNeighbors(current.position, endPos, allNodes);
            
            // 이웃 노드 검사
            ExamineNeighbors(neighbors, current);

            //현재 노드 업데이트
            UpdateCurrentTile(ref current);

            //경로 생성
            path = GeneratePath(current, startPos, endPos);
        }

        //경로가 있으면 true 반환
        return path != null;
    }

    //이웃 노드 찾기
    private List<Node> FindNeighbors(Vector3Int parentPosition, Vector3Int goalPos, Dictionary<Vector3Int, Node> newNodes)
    {
        //이웃 노드 리스트 생성
        List<Node> neighbors = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                //현재 노드면 넘어감
                if (x == 0 && y == 0)
                {
                    continue;
                }

                //이웃 노드의 위치
                Vector3Int neighborPos = new Vector3Int(parentPosition.x + x, parentPosition.y + y, 0);

                //FloorTilemap에 있어야 하고, 이웃 노드가 목표지점이거나 장애물이 아니면 이웃 노드 리스트에 추가
                if (floorTilemap.GetTile(neighborPos) != null && (neighborPos == goalPos || !IsObstacle(neighborPos)))
                {
                    Node neighbor = GetNode(neighborPos, newNodes);
                    neighbors.Add(neighbor);
                }
            }
        }

        return neighbors;
    }

    //이웃 노드 검사
    private void ExamineNeighbors(List<Node> neighbors, Node current, HashSet<Node> newOpenList = null, HashSet<Node> newClosedList = null)
    {
        //현재 이웃노드들 반복
        for (int i = 0; i < neighbors.Count; ++i)
        {
            Node neighbor = neighbors[i];

            //대각선 검사
            if (!ConnectedDiagonally(current, neighbor))
            {
                continue;
            }

            //이웃 노드의 G값 계산
            int gScore = DetermineGScore(neighbor.position, current.position);

            //새로운 openList와 closedList가 없으면 만들어둔 거 사용
            //(몬스터의 이동 시에는 새로운 openList와 closedList를 사용)
            if (newOpenList == null || newClosedList == null)
            {
                //이웃 노드가 openList에 있으면
                if (openList.Contains(neighbor))
                {
                    //이웃노드의 G값과 현재 노드의 G값을 비교
                    if (current.G + gScore < neighbor.G)
                    {
                        //이웃노드 계산
                        CalcValues(current, neighbor, gScore);
                    }
                }
                //이웃 노드가 closedList에 없으면
                else if (!closedList.Contains(neighbor))
                {
                    //이웃노드 계산
                    CalcValues(current, neighbor, gScore);

                    //openList에 이웃 노드 추가
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

    //현재 노드와 이웃노드의 G, H, F 값 계산
    private void CalcValues(Node parent, Node neighbor, int cost)
    {
        //이웃 노드의 부모를 현재 노드로 지정
        neighbor.parent = parent;

        //이웃 노드의 G, H, F 값 계산
        neighbor.G = parent.G + cost;
        neighbor.H = (Mathf.Abs(neighbor.position.x - endPos.x) + Mathf.Abs(neighbor.position.y - endPos.y)) * 10;
        neighbor.F = neighbor.G + neighbor.H;
    }

    //G값 게산
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

    //현재 타일 업데이트
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
            //F값을 기준으로 openList 정렬
            if (openList.Count > 0) current = openList.OrderBy(x => x.F).First();

        }
        else
        {
            if (newOpenList.Count > 0) current = newOpenList.OrderBy(x => x.F).First();
        }
    }

    //대각선에 오브젝트와 장애물이 있는지 검사
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

    //경로 생성
    private Stack<Vector3Int> GeneratePath(Node current, Vector3Int startPos, Vector3Int goalPos)
    {
        //현재 노드가 목표지점이면
        if (current.position == goalPos)
        {
            //최종 경로 생성
            Stack<Vector3Int> finalPath = new Stack<Vector3Int>();

            //현재 노드가 시작지점이 될 때까지 반복
            while (current.position != startPos)
            {
                int x = current.position.x - current.parent.position.x;
                int y = current.position.y - current.parent.position.y;

                //현재 노드를 최종 경로에 추가
                finalPath.Push(current.position);

                if (Mathf.Abs(x - y) % 2 != 1)
                {
                    Node node = GetDiagonalNodeBetween(current.parent.position, current.position);

                    finalPath.Push(node.position);
                }

                //현재 노드를 부모 노드로 변경
                current = current.parent;
            }

            return finalPath;
        }

        return null;
    }

    //대각선 노드를 자연스럽게 연결
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

    //노드 가져오기
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

    //타일 타입 변경
    public void ChangeTileType(TileType newTileType)
    {
        tileType = newTileType;
    }

    //clickPos 타일 변경
    public void ChangeTile(Vector3Int clickPos)
    {
        //빌드모드이면
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

            //튜토리얼 완료 전까지 호출
            if (!changeTiles.Contains(clickPos))
            {
                if (!GameManager.Instance.isTutorialFinish[0])
                {
                    MessageQueue.Instance.QueueMesssage(new AddTutorialValueMessage());
                }
            }

            //changeTiles에 포함되어 있지 않으면 추가
            if (!changeTiles.Contains(clickPos))
                changeTiles.Add(clickPos);
        }
        //제거모드
        else
        {
            //장애물에 포함되면 제거
            if (obstacles.Contains(clickPos))
            {
                obstacles.Remove(clickPos);
            }

            //ClickPos초기화
            obstacleTilemap.SetTile(clickPos, null);

            //ChangeTile 제거
            changeTiles.Remove(clickPos);
        }

        //Tilemap Refresh
        obstacleTilemap.RefreshAllTiles();

        //청사진 제거
        aStarChecker.checkTilemap.ClearAllTiles();
    }

    //장애물로 추가
    public void AddObstacles(Vector3Int pos)
    {
        if (!obstacles.Contains(pos))
        {
            obstacles.Add(pos);
        }
    }

    //변경 불가능한 장애물로 추가
    public void AddNotRemoveObstacles(Vector3Int pos)
    {
        if (!notRemoveObstacles.Contains(pos))
        {
            notRemoveObstacles.Add(pos);
        }
    }

    //변경 불가능한 오브젝트 추가
    public void AddNotRemoveObjects(Vector3Int pos)
    {
        if (!notRemoveObjects.Contains(pos))
        {
            notRemoveObjects.Add(pos);
        }
    }

    //경로 초기화
    public void ResetPath()
    {
        aStarDebugger.ResetDebugger(allNodes);

        path = null;
        current = null;

        if (openList != null) openList.Clear();

        if (closedList != null) closedList.Clear();
    }

    //다음 페이즈를 위해 기존 설치한 벽과 문 정보 재저장
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
