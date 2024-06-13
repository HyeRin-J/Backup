<div align="center">
  <h1 style="border-bottom: 1px solid #21262d; color: #c9d1d9;"> 💻 Code_C# </h1>
  <div style="font-weight: 700; font-size: 15px; text-align: center; color: #c9d1d9;"> c#으로 작성한 코드 모음입니다. </div>
   <br>
</div>

<details open> <summary>
<h2 style="border-bottom: 1px solid #21262d; color: #c9d1d9;"> ⭐ BattleShip (2018) </h2> </summary>

<h3 style="border-bottom: 1px solid #21262d; color: #c9d1d9;">영상</h3>

[![BattleShip](http://img.youtube.com/vi/1MGY0dA-vns/0.jpg)](https://youtu.be/1MGY0dA-vns)
<h3 style="border-bottom: 1px solid #21262d; color: #c9d1d9;"> 세부 사항 </h3>
  <h5 style="border-bottom: 1px solid #21262d; color: #c9d1d9;"> 보드게임 배틀쉽(BattleShip)에서 아이디어를 착안하여 개발, 각 함선에 고유스킬을 추가 및 함선을 회전 및 이동할 수 있도록 하여 전략적인 요소 추가 </h5>
  <br>
  <ul style="display: table;  margin: auto;">
    <li>🎮 장르 : 전략, 보드게임 </li>
    <li>📅 개발 기간 : 1주 </li>
    <li>🙋 개발 인원 : 3명    
      <ul style="border-bottom: 1px;">
        <li>프로그래머 3</li>
      </ul>
    </li>
    <li>📃 개발 환경 : Unity3D </li>
  </ul>
데모 : 

[![Google Drive](https://img.shields.io/badge/Google%20Drive-4285F4?style=for-the-badge&logo=googledrive&logoColor=white)]()

[Full Source Code](https://github.com/HyeRin-J/Atents/tree/master/BattleShip)</li>

<h3 style="border-bottom: 1px solid #21262d; color: #c9d1d9;"> Source Code </h3>

<li>Select&Drag&RotateShip</li>

```csharp
 // Update is called once per frame
    void Update() {
        //선택이 되었으면     
        if (isSelected)
        {
            //아웃라인을 그려줌
            if (gameObject.GetComponentInChildren<Outline>() != null)
                GetComponentInChildren<Outline>().color = 0;

            //게임이 진행중이 아닐때
            if (!GameManager.instance.isGameProgressing)
            {
                //왼쪽버튼을 클릭할 때
                if (Input.GetMouseButton(0))
                {
                    //선택된 Grid(공격포인트)의 좌표를 가져옴
                    Transform selectedGrid = Camera.main.GetComponent<Camera2DPointToWorldPoint>().GetWorldPointIfMouseDown();
                    //부모의 좌표를 선택된 좌표로 옮김
                    parentTr.position = selectedGrid.position;
                }

                //오른쪽 버튼을 클릭할 때
                if (Input.GetMouseButtonDown(1))
                {
                    //y축 방향으로 90도 회전
                    parentTr.Rotate(Vector3.up, 90.0f);

                    //회전할 때 grid와 틀어지는 좌표를 막기 위한 부분임
                    if (transform.rotation.y == 90.0f)
                        parentTr.position = new Vector3(transform.position.x - 0.5f, transform.position.y, transform.position.z);
                    else if (transform.rotation.y == -90.0f)
                        parentTr.position = new Vector3(transform.position.x + 0.5f, transform.position.y, transform.position.z);
                    else if (transform.rotation.y == -180.0f)
                        parentTr.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.5f);
                }

                //휠버튼 클릭 시
                if (Input.GetMouseButtonDown(2))
                {
                    //선택 상태 해제
                    //선택 상태 해제 안 하고 다른 오브젝트 클릭하면 둘이 겹쳐서 이동하므로 주의할 것.
                    if (!isCollision)
                    {
                        isSelected = false;
                        GameManager.instance.selectedShip = null;
                    }
                }
            }
        }
        //선택이 안 되었으면
        else
        {
            //아웃라인 빼줌
            if (gameObject.GetComponentInChildren<Outline>() != null)
                GetComponentInChildren<Outline>().color = 1;
        }

        //배치 및 회전시 grid를 벗어나는 걸 처리하기 위한 부분으로,
        //Player1과 Player2 각각 좌표를 다르게 설정해야 함.
        if (boundary.CompareTag("Player1"))
        {
            if ((boundary.position.x <= 1.4f || boundary.position.x >= 10.6f) || (boundary.position.z >= 2.6f || boundary.position.z <= -6.6f))
            {
                isOut = true;
            }
            else
            {
                isOut = false;
            }
        }
        else if (boundary.CompareTag("Player2"))
        {
            if ((boundary.position.x <= (1.4f - 12f) || boundary.position.x >= (10.6f - 12f)) || (boundary.position.z >= 2.6f || boundary.position.z <= -6.6f))
            {
                isOut = true;
            }
            else
            {
                isOut = false;
            }
        }

        //이전 Transform을 저장하고 있다가 밖으로 나갔다고 판단되면 이전 Transform으로 변경한다.
        if (!isOut)
        {
            originPos = parentTr.position;
            originRot = parentTr.rotation;
        }
        else
        {
            parentTr.position = originPos;
            parentTr.rotation = originRot;
        }
    }
```

<li>GameManager</li>

```csharp
    // Update is called once per frame
    void Update()
    {
        UpdateUIText();

        //턴 로딩중
        if (isTurnLoading)
        {
            timerText.text = "로딩중";
            timerImage.fillAmount = 1.0f;
            shipHPUI[0].SetActive(false);
            shipHPUI[1].SetActive(false);
        }
        else
        {
            //게임 진행중
            if (isGameProgressing && !isGameEnd)
            {
                float amount = ((Time.time - startTimer) / turnTime);
                //UI 갱신
                timerImage.fillAmount = amount;
                timerText.text = (turnTime - (Time.time - startTimer)).ToString("#");

                //턴 전환
                if (Time.time - startTimer >= turnTime)
                {
                    turnNum++;
                    selectedShip = null;
                    Initiallize();

                    if (turn1)
                    {
                        logText1.text = "";
                        StartCoroutine(Waiting1());
                    }
                    else if (turn2)
                    {
                        logText2.text = "";
                        StartCoroutine(Waiting2());
                    }
                }
            }
            else if (isGameEnd)
            {
                timerText.text = "게임 종료";
            }
            //게임 진행 중 아님
            else
            {
                timerText.text = "함선 배치";
            }
        }


        //턴 전환시 상대방의 함선은 보이지 않게 함
        foreach (var obj in ships)
        {
            Transform child = obj.transform.GetChild(0);

            if (obj.CompareTag("Player1"))
            {
                for (int i = 0; i < child.childCount; i++)
                {
                    if (turn1)
                        child.GetChild(i).gameObject.SetActive(true);
                    else
                        child.GetChild(i).gameObject.SetActive(false);
                }

            }
            if (obj.CompareTag("Player2"))
            {
                for (int i = 0; i < child.childCount; i++)
                {
                    if (turn2)
                        child.GetChild(i).gameObject.SetActive(true);
                    else
                        child.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
    }
```
</details>

<details open> <summary>
<h2 style="border-bottom: 1px solid #21262d; color: #c9d1d9;"> ⭐ CatsMaze (2021 ~ 2023) </h2> </summary>

<h3 style="border-bottom: 1px solid #21262d; color: #c9d1d9;">영상</h3>

[![CatsMaze](http://img.youtube.com/vi/FC5PvDIiphQ/0.jpg)](https://youtu.be/FC5PvDIiphQ)
<h3 style="border-bottom: 1px solid #21262d; color: #c9d1d9;"> 세부 사항 </h3>
  <h5 style="border-bottom: 1px solid #21262d; color: #c9d1d9;"> 자유롭게 정해진 구역을 만들고, 구역에 고양이 카드를 배치하여 쳐들어오는 인간들을 막아내는 디펜스 게임 </h5>
  <br>
  <ul style="display: table;  margin: auto;">
    <li>🎮 장르 : 덱 빌딩 던전 디펜스 </li>
    <li>🙋 개발 인원 : 8명    
      <ul style="border-bottom: 1px;">
        <li>프로그래머 1</li>
        <li>기획 2</li>
        <li>아트 5</li>
      </ul>
    </li>
    <li>📃 개발 환경 : Unity3D </li>
    <li>사용 기술</li>
      <ul style="border-bottom: 1px">
        <li>A* PathFinding</li>
        <li>FloodFill</li>
        <li>FSM(Finite-state Machine)</li>
        <li>Unity Custom Editor(맵 에디터 제작)</li>
        <li>Addressable Asset System</li>
        <li>Localization</li>
      </ul>
  </ul>
데모 : 

[![Google Drive](https://img.shields.io/badge/Google%20Drive-4285F4?style=for-the-badge&logo=googledrive&logoColor=white)](https://drive.google.com/file/d/1obgVseJZtQnqWKuZtycdy1nw3Oll5NIp/view?usp=sharing)

\[Full Source Code\](미공개)

<h3 style="border-bottom: 1px solid #21262d; color: #c9d1d9;"> Source Code </h3>

<li>AStar</li>

```csharp
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
```

<li>LoadMap</li>

```csharp
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
```

<li>FloodFill</li>

```csharp
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
```
</details>

<details open> <summary>
<h2 style="border-bottom: 1px solid #21262d; color: #c9d1d9;"> ⭐ CatsSurvival (2023) </h2> </summary>

<h3 style="border-bottom: 1px solid #21262d; color: #c9d1d9;"> 세부 사항 </h3>
  <h5 style="border-bottom: 1px solid #21262d; color: #c9d1d9;"> 뱀파이어 서바이버류 게임.</h5>
  <br>
  <ul style="display: table;  margin: auto;">
    <li>🎮 장르 : 뱀서류 </li>
    <li>🙋 개발 인원 : 1명    
      <ul style="border-bottom: 1px;">
        <li>프로그래머 1</li>
      </ul>
    </li>
    <li>📃 개발 환경 : Unity3D </li>
      </ul>
  </ul>
데모 : 

\[Full Source Code\](미공개)

<h3 style="border-bottom: 1px solid #21262d; color: #c9d1d9;"> Source Code </h3>

<li>MonsterMoveTarget</li>

```csharp
 public async UniTaskVoid MoveToTarget()
    {
        while (life > 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.fixedDeltaTime);
            rigidbody.velocity = Vector2.zero;

            if (transform.position.x > target.position.x)
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
            }
            else
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }

            await UniTask.Yield(PlayerLoopTiming.Update);
        }

        await UniTask.Delay(1000);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            SetDamage(10);
        }
    }
```

<li>RepositionPlayer</li>

```csharp
 private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Area")) return;

        Vector3 playerPos = PlayScene.Instance.player.transform.position;
        Vector3 pos = transform.position;

        float x = Mathf.Abs(playerPos.x - pos.x);
        float y = Mathf.Abs(playerPos.y - pos.y);

        Vector3 playerDir = PlayScene.Instance.player.GetComponent<Player>().inputValue;
        float dirX = playerDir.x < 0 ? -1 : 1;
        float dirY = playerDir.y < 0 ? -1 : 1;

        switch(transform.tag)
        {
            case "Ground":
                if(x > y)
                {
                    transform.Translate(Vector3.right * dirX * 40);
                }
                else if(x < y)
                {
                    transform.Translate(Vector3.up * dirY * 40);
                }
                break;
            case "Monster":
                if(collider.enabled)
                {
                    transform.Translate(playerDir * 20 + new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), 0));
                }
                break;
        }
    }
```

</details>

<details open> <summary>
<h2 style="border-bottom: 1px solid #21262d; color: #c9d1d9;"> ⭐ CatsSurvival (2023) </h2> </summary>

<h3 style="border-bottom: 1px solid #21262d; color: #c9d1d9;"> 세부 사항 </h3>
  <h5 style="border-bottom: 1px solid #21262d; color: #c9d1d9;"> 뱀파이어 서바이버류 게임.</h5>
  <br>
  <ul style="display: table;  margin: auto;">
    <li>🎮 장르 : 뱀서류 </li>
    <li>🙋 개발 인원 : 1명    
      <ul style="border-bottom: 1px;">
        <li>프로그래머 1</li>
      </ul>
    </li>
    <li>📃 개발 환경 : Unity3D </li>
      </ul>
  </ul>
데모 : 

\[Full Source Code\](미공개)

<h3 style="border-bottom: 1px solid #21262d; color: #c9d1d9;"> Source Code </h3>

<li>MonsterMoveTarget</li>

```csharp
 public async UniTaskVoid MoveToTarget()
    {
        while (life > 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.fixedDeltaTime);
            rigidbody.velocity = Vector2.zero;

            if (transform.position.x > target.position.x)
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
            }
            else
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }

            await UniTask.Yield(PlayerLoopTiming.Update);
        }

        await UniTask.Delay(1000);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            SetDamage(10);
        }
    }
```

<li>RepositionPlayer</li>

```csharp
 private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Area")) return;

        Vector3 playerPos = PlayScene.Instance.player.transform.position;
        Vector3 pos = transform.position;

        float x = Mathf.Abs(playerPos.x - pos.x);
        float y = Mathf.Abs(playerPos.y - pos.y);

        Vector3 playerDir = PlayScene.Instance.player.GetComponent<Player>().inputValue;
        float dirX = playerDir.x < 0 ? -1 : 1;
        float dirY = playerDir.y < 0 ? -1 : 1;

        switch(transform.tag)
        {
            case "Ground":
                if(x > y)
                {
                    transform.Translate(Vector3.right * dirX * 40);
                }
                else if(x < y)
                {
                    transform.Translate(Vector3.up * dirY * 40);
                }
                break;
            case "Monster":
                if(collider.enabled)
                {
                    transform.Translate(playerDir * 20 + new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), 0));
                }
                break;
        }
    }
```

</details>

<details open> <summary>
<h2 style="border-bottom: 1px solid #21262d; color: #c9d1d9;"> ⭐ EOVR(2018) </h2> </summary>

<h3 style="border-bottom: 1px solid #21262d; color: #c9d1d9;">영상</h3>

[![CatsMaze](http://img.youtube.com/vi/bwDjgqxpFag/0.jpg)](https://youtu.be/bwDjgqxpFag)
<h3 style="border-bottom: 1px solid #21262d; color: #c9d1d9;"> 세부 사항 </h3>
  <h5 style="border-bottom: 1px solid #21262d; color: #c9d1d9;"> 세계수의 미궁 모작 프로젝트 </h5>
  <br>
  <ul style="display: table;  margin: auto;">
    <li>🎮 장르 : 3D 던전 RPG </li>
    <li>📅 개발 기간 : 3주 </li>
    <li>🙋 개발 인원 : 4명    
      <ul style="border-bottom: 1px;">
        <li>프로그래머 4</li>
      </ul>
    </li>
    <li>📃 개발 환경 : Unity3D </li>
  </ul>
데모 : 

[![Google Drive](https://img.shields.io/badge/Google%20Drive-4285F4?style=for-the-badge&logo=googledrive&logoColor=white)]()

\[Full Source Code\](https://github.com/HyeRin-J/Atents/tree/master/EOVR)

<h3 style="border-bottom: 1px solid #21262d; color: #c9d1d9;"> Source Code </h3>

<li>ActionSelect</li>

```csharp
 //몬스터의 행동 결정
        for (int i = 0; i < GetComponent<SpawnManager>().spawnMonsters.Length; i++)
        {
            int randIndex = Random.Range(0, 100);
            BattleMonster mon = GetComponent<SpawnManager>().spawnMonsters[i].GetComponent<BattleMonster>();
            if (randIndex < 20)
            {
                mon.monCurActionState = MonCurrentActionState.Attack;
                mon.monsterSelectedSpeed = 100;
            }
            else
            {
                mon.monCurActionState = MonCurrentActionState.Skill;
                mon.monsterSelectedSpeed = 120;
            }

            if (turnProgressSequence.Count == 0)
                turnProgressSequence.Add(mon.gameObject);
            else
            {
                int j = 0;
                for (; j < turnProgressSequence.Count; j++)
                {
                    BattleMonster temp = turnProgressSequence[j].GetComponent<BattleMonster>();

                    if (mon.monsterSelectedSpeed >= temp.monsterSelectedSpeed)
                    {
                        turnProgressSequence.Insert(j, mon.gameObject);
                        break;
                    }
                }
                if (j == turnProgressSequence.Count)
                {
                    turnProgressSequence.Add(mon.gameObject);
                }
            }
        }
        //플레이어의 행동에 따라 스피드 결정
        foreach (BattlePlayer player in players)
        {
            switch (player.currentActionState)
            {
                case CurrentActionState.Attack:
                    player.selectedSpeed = 100;
                    break;
                case CurrentActionState.Skill:
                    player.selectedSpeed = player.selectedSkill.Speed;
                    break;
                case CurrentActionState.Defend:
                    player.selectedSpeed = 1000;
                    break;
                case CurrentActionState.Item:
                    player.selectedSpeed = 120;
                    break;
                case CurrentActionState.Escape:
                    break;
            }
            int j = 0;
            for (; j < turnProgressSequence.Count; j++)
            {
                if (turnProgressSequence[j].GetComponent<BattlePlayer>() != null)
                {
                    BattlePlayer temp = turnProgressSequence[j].GetComponent<BattlePlayer>();

                    if (player.selectedSpeed >= temp.selectedSpeed)
                    {
                        turnProgressSequence.Insert(j, player.gameObject);
                        break;
                    }
                }
                else if (turnProgressSequence[j].GetComponent<BattleMonster>() != null)
                {
                    BattleMonster temp = turnProgressSequence[j].GetComponent<BattleMonster>();

                    if (player.selectedSpeed >= temp.monsterSelectedSpeed)
                    {
                        turnProgressSequence.Insert(j, player.gameObject);
                        break;
                    }
                }
            }
            if (j == turnProgressSequence.Count)
            {
                turnProgressSequence.Add(player.gameObject);
            }
        }
```

<li>MonsterDamage</li>
```csharp
 //몬스터일 경우
            if (character.GetComponent<BattleMonster>() != null)
            {
                latelyAttackMonster = character;
                var monster = character;

                switch (monster.GetComponent<BattleMonster>().monCurActionState)
                {
                    //일반공격
                    case MonCurrentActionState.Attack:
                        //몬스터의 공격모션 트리거 설정
                        monster.GetComponent<Animator>().SetTrigger("Attack");

                        //랜덤으로 공격할 플레이어 설정
                        int index = Random.Range(0, players.Length);

                        //현재 몬스터 데이터 가져옴
                        Monster_StatRecord monsterData = monster.GetComponent<BattleMonster>().monsterCharacter.monStatRecord;

                        //죽은 플레이어가 걸리면 다시 랜덤 돌림
                        while (players[index].playerInfo.isDown)
                        {
                            index = Random.Range(0, players.Length);
                        }

                        //상태 출력
                        statusText.text = string.Format("{0}은(는) {1}을 공격했다!", monsterData.Name, players[index].playerInfo.characterName);

                        //몬스터의 공격 애니메이션과 피격 이펙트의 싱크를 맞추기 위한 시간
                        yield return new WaitForSeconds(1f);

                        //피격 이펙트 생성
                        //최적화 ㅜㅜ
                        GameObject effect = Instantiate(playerEffects[3], players[index].transform.parent);
                        Destroy(effect, 1.0f);

                        //맞았을때 흔들리는 애니메이션
                        players[index].GetComponent<Animation>().Play();

                        #region 데미지계산식
                        float monsterAtk = (float)monsterData.Patk * 2; //몬스터 물공
                        float playerDef = (float)players[index].playerInfo.physicalDef / 2; //플레이어 물방

                        /* A
                         * 공격하는 적 ATK X 2 < 방어력(총물리방어력)/2) 일 때,   A=0.95
                         * 공격하는 적 ATK X 2 = 방어력(총물리방어력)/2) 일 때,  A=1.0
                         * 공격하는 적 ATK X 2 > 방어력(총물리방어력)/2) 일 때,   A=ATK X 2 ÷ (DEF+VIT)로 서서히 상승 (이게 힘드시면 1.05로 해주시면 됩니다.)
                         */
                        float inc = monsterAtk < playerDef ? 0.95f : (monsterAtk == playerDef ? 1.0f : monsterAtk / playerDef * 2);
                        //물공(공격하는 적의 ATK - 공격받는 캐릭터의 총물리방어력)   X A
                        //마공(공격하는 적의 MATK - 공격받는 캐릭터의 총마법방어력)  X A
                        int damage = (int)((monsterAtk / 2 - playerDef * 2) * inc);
                        damage = players[index].playerInfo.Damaged(damage, PlayerData.SkillData.DMGTYPE.Physics);
                        Debug.Log("damage : " + damage);
                        //최소 데미지 1
                        damage = damage <= 0 ? 1 : damage;
                        #endregion

                        //플레이어가 받은 데미지 표시
                        //이거도 최적화 ㅜㅜ
                        GameObject damageText = Instantiate(playerDamageText.gameObject, players[index].transform);
                        damageText.GetComponent<Text>().text = string.Format("{0}", damage);
                        Destroy(damageText, 1.0f);

                        //hpBar를 줄이기 위해 새로운 변수를 하나 만든다
                        int temp = players[index].playerInfo.currentHp - damage;
                        temp = temp <= 0 ? 1 : temp;

                        //현재 hp에서 데미지를 뺀 숫자가 될 때까지 현재 hp를 1씩 감소시키고, hpBar를 갱신
                        while (players[index].playerInfo.currentHp >= temp)
                        {
                            players[index].playerInfo.currentHp--;
                            players[index].SetText();
                            yield return new WaitForSeconds(0.05f);
                        }
                        //플레이어의 hp가 0이면 죽은 걸로 판정
                        if (players[index].playerInfo.currentHp <= 0)
                        {
                            if (players[index].playerInfo.KnockDown())  //사망 판정 1회 무시 하는 스킬이 있어서 넣어놓음
                            {
                                deathCount++;
                                players[index].playerInfo.isDown = true;
                            }
                            else
                            {
                                players[index].playerInfo.currentHp = 1;
                            }
                        }
                        break;
                    case MonCurrentActionState.Skill:
                        monster.GetComponent<Animator>().SetTrigger("SkillA");
                        statusText.text = string.Format("{0}는 스킬을 사용했다!", monster.name);
                        yield return new WaitForSeconds(1f);
                        break;
                }

                yield return new WaitForSeconds(2.0f);
            }
        }
```
