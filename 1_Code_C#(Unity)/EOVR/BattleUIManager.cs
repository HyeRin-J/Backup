using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Skill;

public enum SelectedButton
{
    None = 0,
    Attack,
    Skill,
    Defend,
    Item,
    Switch,
    Escape,
}

public class BattleUIManager : MonoBehaviour
{
    #region 변수들
    #region 상태텍스트
    public Text turnText;
    public Text statusText;
    public int turn = 1;
    public GameObject menu;

    public GameObject line;
    public Canvas state;
    public Canvas result;
    public Canvas actionMenu;
    public Canvas monsterStatUI;
    #endregion
    #region 버튼,플레이어
    public Button[] actions;
    public BattlePlayer[] players;
    public int actionIndex = 0;
    public int playerIndex = 0;
    float getKeyTime = 0.0f;
    #endregion
    #region 몬스터타겟팅
    public GameObject[] targetArrow;
    public int targetIndex = 0;
    public int tempMonsterIndex = 0;
    public int checkMonsterArrayIndex = 0;
    public GameObject[] tempMonster;
    public List<GameObject> tempMonsterArray = new List<GameObject>();
    public bool skillSelected = false;
    public bool itemSelected = false;
    bool isMonsterUIOpen = false;
    bool isMonsterStatus = false;
    GameObject[] monsterStats;
    GameObject[] monsterInfo;
    IdentifyScript[] monsterSkill;
    int monsterSkillIndex;

    #endregion
    #region 스킬테이블&아이템테이블
    public GameObject skillTable;
    public GameObject itemTable;
    public GameObject skillPrefab;
    public GameObject itemPrefab;
    public List<Button> skillList;
    public List<Button> itemList;
    public int skillListIndex = 0;
    public int itemListIndex = 0;
    List<int> conItems;
    #endregion
    #region 현재선택된버튼
    public SelectedButton selectedButton = SelectedButton.None;
    #endregion
    #region 색깔하드코딩
    public Color nonSelected = new Color(2 / 255f, 22 / 255f, 23 / 255f, 0.8f);
    public Color selected = new Color(164 / 255f, 225 / 255f, 120 / 255f, 0.8f);
    public Color posSelected = new Color(164 / 255f, 225 / 255f, 255 / 255f, 1.0f);
    Color statusNonSelected = new Color(83/255f, 145/255f, 128/255f, 0.0f);
    Color skillSpaceSelected = new Color(0 / 255f, 255 / 255f, 23 / 255f, 1.0f);
    Color skillSpaceNonSelected = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    #endregion
    #region 메뉴움직일때쓸거임
    bool isMenuMoving = false;
    Vector3 menuMovingStartPos = Vector3.zero;
    Vector3 menuMovingEndPos = Vector3.zero;
    float menuMovingDuration;
    float menuMovingStartTime;
    #endregion
    #region 스크립트들
    SpawnManager spawnManager;
    BattleManager battleManager;
    #endregion
    #region 스위치 변수

    public GameObject[] playerPos;
    bool switchActive = false;

    bool characterSelect;
    bool positionSelect;

    int switchIndex = 0;
    int switchToPosIndex = 0;
    public int positionIndex = 0;

    Transform characterPos;
    Transform positionPos;
    Transform tempPos;
    public Transform[] temp = new Transform[5];
    #endregion
    #region ShowResultUI
    bool BattleEnd;

    public GameObject[] characterResult;
    public Text totalExpText;
    public int totalExp;
    public GameObject resultItem;
    public Text[] resultItemText;

    // 인벤토리 정리UI
    public Canvas overInventoryCanvas;
    // 인벤토리 정리UI 메시지 창
    public GameObject messageWIndow;

    // 인벤토리 정리UI 아이템 버튼 리스트
    List<Button> allItemButtonList = new List<Button>();
    // 인벤토리 정리UI 드랍 아이템 버튼 리스트
    List<Button> dropItemButtonList = new List<Button>();

    // 인벤토리 정리 창 상단부의 현재 인벤토리 아이템 개수 표시 UI
    public Text tempInventoryText;

    // 버튼 생성용 프리펩
    public GameObject overInventoryItemPrefab;

    // 드랍아이텝이 더해졌을 경우 인벤토리가 오버되는지 체크
    bool isInventoryOver = false;
    // 인벤토리와 드랍아이템을 더한 개수가 인벤토리 오버로 검출되는지 체크
    bool checkInventoryOver = true;
    // 인벤토리 정리UI를 시작하는지 체크
    bool startCheckInventory = false;
    // 인벤토리 창을 탐색중인지 체크
    bool allItemExplore = true;
    // 인벤토리 정리UI를 빠져나가는지 체크
    bool exitInventoryCheck = false;
    // 인벤토리 정리UI에서 인벤토리 창에 버튼이 생성됐는지 체크
    bool allItemButtonExist = false;
    // 인벤토리 정리UI에서 드랍 아이템 창에 버튼이 생성됐는지 체크
    bool dropItemButtonExist = false;
    // 인벤토리에서 아이템 버리기 판별
    bool dumpItem = false;
    // 인벤토리에 아이템 추가 판별
    bool addDropItem = false;

    // 인벤토리 아이템들을 담을 리스트
    List<int> allItemList = new List<int>();
    // 드랍 아이템들을 담을 리스트
    List<DropItemRecord> dropItems = new List<DropItemRecord>();

    // 메시지 창 메시지 텍스트
    Text messageText;
    // 메시지 창 선택지 버튼
    Button[] messageButton = new Button[2];
    // 인벤토리 창과 드랍 아이템 창 구별용 빈 스크립트
    IdentifyScript[] InventoryOverWindows;

    // 버튼 내부 스트링 넘버를 int로 파싱할 떄 필요한 변수
    int itemNumberToText;

    // 최대 인벤토리 칸수
    int maxInventory = 60;
    // 드랍 아이템과 현재 인벤토리 내부 아이템 개수를 서로 합친 변수
    int checkInventoryCount = 0;
    // 인벤토리 창 탐색용 변수
    int allItemListIndex = 0;
    // 드랍 아이템 창 탐색용 변수
    int dropItemListIndex = 0;
    // 메시지 창 선택지 버튼 선택용 변수
    int messageButtonIndex = 0;
    #endregion
    #region ShowStatusUI
    public Canvas statusUI;
    public bool statusOn;
    public bool debuff;
    public List<GameObject> statusList;
    public StatusIndex[] characterStatus;
    public Mask[] skillSpace;
    int statusIndex = 0;
    int skillindex = 0;
    GameObject controlText;
    KeyCode keyCode;
    #endregion

    #endregion

    private void Start()
    {
        spawnManager = GetComponent<SpawnManager>();
        battleManager = GetComponent<BattleManager>();
        actions[actionIndex].animator.SetTrigger("Highlighted");
        skillTable.SetActive(false);
        itemTable.SetActive(false);
        debuff = false;
        statusOn = false;
        BattleEnd = false;

        monsterStats = GameObject.FindGameObjectsWithTag("MonsterStats");
        monsterInfo = GameObject.FindGameObjectsWithTag("MonsterInfo");
        resultItemText = resultItem.GetComponentsInChildren<Text>();
        characterStatus = statusUI.GetComponentsInChildren<StatusIndex>();
        monsterSkill = monsterStatUI.GetComponentsInChildren<IdentifyScript>();

        // 키보드 Y, R, B에 대한 설명 텍스트
        controlText = state.gameObject.GetComponentsInChildren<Text>()[1].gameObject;

        // 인벤토리 정리UI의 스크롤 뷰 정의
        InventoryOverWindows = overInventoryCanvas.GetComponentsInChildren<IdentifyScript>();
        // 메시지 창의 메시지 텍스트 정의
        messageText = messageWIndow.GetComponentInChildren<Text>();
        // 메시지 창의 메시지 선택지 버튼 정의
        messageButton = messageWIndow.GetComponentsInChildren<Button>();

        // 배틀씬 시작 후 5 초간 인벤토리가 오버되는지 체크
        StartCoroutine(CheckInventoryDelay());
    }

    // Update is called once per frame
    void Update()
    {
        // 몬스터가 0일때를 검출
        CheckBattle();

        //턴 진행중이면 리턴
        if (battleManager.isTurnProgressing)
        {
            AllTargetArrowActiveFalse();
            menu.SetActive(false);
            return;
        }

        // checkInventoryOver가 트루인 동안 인벤토리 오버인지 검출
        // CheckInventoryDelay 코루틴에서 5초 후 false로 바꿈
        if (checkInventoryOver == true)
        {
            // 현재 인벤토리 아이템 개수와 드랍 아이템 개수를 더해서 인벤토리 오버가 되는지 검출하는 함수
            CheckInventoryOver();
        }

        //메뉴 이동중, Vector3.Lerp 사용
        if (isMenuMoving)
        {
            float distCovered = (Time.time - menuMovingStartTime) * 0.15f;
            float frac = distCovered / menuMovingDuration;
            menu.GetComponent<RectTransform>().position = Vector3.Lerp(menuMovingStartPos, menuMovingEndPos, frac);
            if (Vector3.Distance(menu.GetComponent<RectTransform>().position, menuMovingEndPos) <= 0.001f)
            {
                menu.GetComponent<RectTransform>().position = menuMovingEndPos;
                isMenuMoving = false;
            }
        }
        //현재 targetIndex부분에 있는 몬스터 삭제
        else if (Input.GetKeyUp(KeyCode.X))
        {
            Destroy(spawnManager.spawnMonsters[targetIndex]);
            targetIndex++;
            if (targetIndex >= spawnManager.spawnCount) targetIndex = 0;
            spawnManager.spawnCount--;
        }

        // 인벤토리 정리UI는 드랍 아이템 + 현재 인벤토리 개수가 60이 넘을때 호출 가능
        // 정리UI 호출을 위해 현재 인벤토리에 드랍된 아이템 전체를 강제로 넣는 임시 기능
        else if(Input.GetKeyUp(KeyCode.T) && isInventoryOver == false)
        {
            AddInventory();
        }

        else if (Input.GetKeyUp(KeyCode.Y))
        {
            if (!statusOn)
            {
                statusOn = true;
                statusIndex = 0;
                skillindex = 0;

                for (int i = 0; i < characterStatus.Length; i++)
                {
                    //characterStatus[i] = statusList[i];
                    //characterStatus[i].GetComponent<StatusIndex>().StatusCheck();
                    characterStatus[i].GetComponent<StatusIndex>().StatusCheck();
                }

                ShowStatusUI();

                statusList[statusIndex].GetComponent<Image>().color = posSelected;
                skillSpace = statusList[statusIndex].GetComponentInChildren<Outline>().gameObject.GetComponentsInChildren<Mask>();
                skillSpace[skillindex].gameObject.GetComponent<Image>().color = skillSpaceSelected;

                List<BattlePlayer.BuffSkillsInfo> buff = statusList[statusIndex].GetComponent<StatusIndex>().battlePlayer.buffSkills;

                List<BattlePlayer.DebuffSkillsInfo> deBuff = statusList[statusIndex].GetComponent<StatusIndex>().battlePlayer.debuffSkills;

                if (buff.Count > 0)
                    statusText.text = buff[skillindex].buffs.Explain;
                else
                    statusText.text = " ";
            }

            else if (statusOn)
            {
                skillSpace[skillindex].gameObject.GetComponent<Image>().color = skillSpaceNonSelected;

                statusIndex = 0;
                skillindex = 0;
                debuff = !debuff;

                for (int i = 0; i < statusList.Count; i++)
                {
                    statusList[i].GetComponent<Image>().color = statusNonSelected;
                }

                ShowStatusUI();
                statusList[statusIndex].GetComponent<Image>().color = posSelected;
                skillSpace = statusList[statusIndex].GetComponentInChildren<Outline>().gameObject.GetComponentsInChildren<Mask>();
                skillSpace[skillindex].gameObject.GetComponent<Image>().color = skillSpaceSelected;

                List<BattlePlayer.BuffSkillsInfo> buff = statusList[statusIndex].GetComponent<StatusIndex>().battlePlayer.buffSkills;

                List<BattlePlayer.DebuffSkillsInfo> deBuff = statusList[statusIndex].GetComponent<StatusIndex>().battlePlayer.debuffSkills;

                if (buff.Count > 0 && !debuff)
                    statusText.text = buff[skillindex].buffs.Explain;

                else if (deBuff.Count > 0 && debuff)
                    statusText.text = deBuff[skillindex].debuffs.Explain;
                else
                    statusText.text = " ";
            }
        }
        //몬스터 스탯창 띄움
        else if (Input.GetKeyUp(KeyCode.R))
        {
            for(int i = 0; i < monsterSkill.Length; i++)
            {
                monsterSkill[i].gameObject.GetComponent<Image>().color = skillSpaceNonSelected;
            }

            monsterSkillIndex = 0;

            if (!isMonsterUIOpen)
            {
                isMonsterUIOpen = true;
            }

            else if (isMonsterUIOpen)
            {
                isMonsterStatus = !isMonsterStatus;
            }

            AllTargetArrowActiveFalse();
            //isMonsterUIOpen = !isMonsterUIOpen;
            //Debug.Log(isMonsterUIOpen);
        }

        else if (statusOn)
        {
            if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                NextStatusSpace();
            }

            else if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                PrevStatusSpace();
            }

            else if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                PrevStatusSkill();
            }

            else if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                NextStatusSkill();
            }

            else if (Input.GetKeyUp(KeyCode.B))
            {
                QuitStatusUI();
            }
        }

        //몬스터 스탯창 오픈상태
        else if (isMonsterUIOpen)
        {
            if (!isMonsterStatus)
            {
                for (int i = 0; i < monsterStats.Length; i++)
                {
                    monsterStats[i].SetActive(false);
                }
                for (int i = 0; i < monsterInfo.Length; i++)
                {
                    monsterInfo[i].SetActive(true);
                }
                
                //몬스터 스탯창의 정보 갱신
                monsterStatUI.GetComponent<MonsterUI>().MonUIActive(spawnManager.spawnMonsters[tempMonsterIndex].GetComponent<BattleMonster>());
            }

            else if (isMonsterStatus)
            {
                monsterStatUI.GetComponent<MonsterUI>().MonUIActive(spawnManager.spawnMonsters[tempMonsterIndex].GetComponent<BattleMonster>());

                for (int i = 0; i < monsterStats.Length; i++)
                {
                    monsterStats[i].SetActive(true);
                }
                for (int i = 0; i < monsterInfo.Length; i++)
                {
                    monsterInfo[i].SetActive(false);
                }

                monsterSkill[monsterSkillIndex].gameObject.GetComponent<Image>().color = skillSpaceSelected;

                if (Input.GetKeyUp(KeyCode.DownArrow))
                {
                    monsterSkill[monsterSkillIndex].gameObject.GetComponent<Image>().color = skillSpaceNonSelected;
                    monsterSkillIndex++;

                    if (monsterSkillIndex >= monsterSkill.Length)
                    {
                        monsterSkillIndex = monsterSkill.Length - 1;
                    }

                    monsterSkill[monsterSkillIndex].gameObject.GetComponent<Image>().color = skillSpaceSelected;
                }

                if (Input.GetKeyUp(KeyCode.UpArrow))
                {
                    monsterSkill[monsterSkillIndex].gameObject.GetComponent<Image>().color = skillSpaceNonSelected;
                    monsterSkillIndex--;

                    if (monsterSkillIndex < 0)
                    {
                        monsterSkillIndex = 0;
                    }

                    monsterSkill[monsterSkillIndex].gameObject.GetComponent<Image>().color = skillSpaceSelected;
                }
            }

            //현재 선택된 몬스터를 표시할 화살표
            targetArrow = new GameObject[1];
            targetArrow[0] = spawnManager.spawnMonsters[tempMonsterIndex].transform.parent.GetChild(0).GetChild(0).gameObject;
            targetArrow[0].SetActive(true);

            if (Time.time - getKeyTime >= 0.3f)
            {
                //좌우 화살표 누를때마다 몬스터 위치 이동
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    for (int i = 0; i < monsterSkill.Length; i++)
                    {
                        monsterSkill[i].gameObject.GetComponent<Image>().color = skillSpaceNonSelected;
                    }

                    monsterSkillIndex = 0;

                    //원래 선택된 몬스터의 화살표를 비활성화
                    targetArrow[0].SetActive(false);
                    //시간 초기화
                    getKeyTime = Time.time;
                    //모든 화살표 비활성화
                    AllTargetArrowActiveFalse();
                    //스탯용 인덱스
                    tempMonsterIndex++;
                    if (tempMonsterIndex >= spawnManager.spawnMonsters.Length) tempMonsterIndex = 0;
                }

                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    for (int i = 0; i < monsterSkill.Length; i++)
                    {
                        monsterSkill[i].gameObject.GetComponent<Image>().color = skillSpaceNonSelected;
                    }

                    monsterSkillIndex = 0;

                    targetArrow[0].SetActive(false);
                    getKeyTime = Time.time;
                    AllTargetArrowActiveFalse();
                    tempMonsterIndex--;
                    if (tempMonsterIndex < 0) tempMonsterIndex = spawnManager.spawnMonsters.Length - 1;
                }

                if (Input.GetKeyUp(KeyCode.B))
                {
                    isMonsterUIOpen = false;
                    isMonsterStatus = false;
                }
            }
            //스탯 켜져있을 때는 카메라가 몬스터를 바라보게 함
            Camera.main.transform.LookAt(spawnManager.spawnMonsters[tempMonsterIndex].transform.position + new Vector3(0, 0.3f, 0));
            //몬스터 스탯창 활성화
            monsterStatUI.gameObject.SetActive(true);

            //몬스터 스탯창의 정보 갱신monsterStatUI.GetComponent<MonsterUI>().MonUIActive(testSpawn.spawnMonsters[tempMonsterIndex].GetComponent<Monster>());        
        }
        else
        {
            //플레이어가 1명도 없으면
            if (players[0] == null)
            {
                //플레이어를 새로 할당
                players = line.GetComponentsInChildren<BattlePlayer>();

                //초기화
                for (int i = 0; i < players.Length; i++)
                {
                    players[i].playerNum = (PlayerNum)i;
                    players[i].GetComponent<Image>().color = nonSelected;
                }

                //처음 선택
                players[playerIndex].GetComponent<Image>().color = selected;

                //BattleManager의 players와도 연동
                GetComponent<BattleManager>().players = players;

                GetComponent<BattleManager>().PassiveSkillActivation();
            }
            else
            {
                //플레이어의 정보 갱신
                ShowLineUI();
                //몬스터 스탯UI 끔
                monsterStatUI.gameObject.SetActive(false);
                //카메라 포지션 고정
                Camera.main.transform.localEulerAngles = new Vector3(7, 180, 0);
            }
            //선택된 버튼
            switch (selectedButton)
            {
                case SelectedButton.None:
                    #region 선택된 버튼이 없을때
                    AllTargetArrowActiveFalse();

                    //플레이어가 죽어있으면 index 증가
                    while (players[playerIndex].playerInfo.isDown)
                    {
                        playerIndex++;
                    }

                    players[playerIndex].GetComponent<Image>().color = selected;

                    statusText.text = "무엇을 할까?";
                    //누르고 있으면 0.3초마다 메뉴 이동
                    if (Time.time - getKeyTime >= 0.3f)
                    {
                        if (Input.GetKey(KeyCode.DownArrow))
                        {
                            actions[actionIndex].animator.SetTrigger("Normal");
                            getKeyTime = Time.time;
                            actionIndex++;
                            if (actionIndex >= actions.Length) actionIndex = actions.Length - 1;
                            actions[actionIndex].animator.SetTrigger("Highlighted");
                        }
                        if (Input.GetKey(KeyCode.UpArrow))
                        {
                            actions[actionIndex].animator.SetTrigger("Normal");
                            getKeyTime = Time.time;
                            actionIndex--;
                            if (actionIndex < 0) actionIndex = 0;
                            actions[actionIndex].animator.SetTrigger("Highlighted");
                        }
                    }
                    //A 누르면 그 버튼 선택상태로 넘어감
                    if (Input.GetKeyUp(KeyCode.A))
                    {
                        selectedButton = (SelectedButton)actionIndex + 1;
                    }
                    //B 누르면 이전 플레이어가 선택했던 상태로 돌아감
                    if (Input.GetKeyUp(KeyCode.B))
                    {
                        if (playerIndex > 0)
                        {
                            players[playerIndex].gameObject.GetComponent<Image>().color = nonSelected;
                            players[playerIndex].currentActionState = CurrentActionState.None;

                            int tempIndex = playerIndex;
                            int downCount = 0;

                            // 이전 플레이어의 다운상태 체크
                            // 다운된 플레이어만 스킵하면 되니까
                            // 다운 안 된 플레이어가 나올경우 반복문 종료
                            for(int i = tempIndex - 1; i >= 0; i--)
                            {
                                if (players[i].playerInfo.isDown)
                                {
                                    downCount++;
                                }
                                else break;
                            }

                            //다운된 플레이어들의 숫자가 현재 playerIndex와 같으면
                            //이전 플레이어들은 전부 다운 상태임
                            if (downCount == playerIndex)
                                playerIndex = tempIndex;
                            //전부 다운 상태가 아니면 다운된 숫자만큼 playerIndex에서 더 빼주면 됨
                            else
                            {
                                playerIndex -= (downCount + 1);
                            }

                            selectedButton = (SelectedButton)players[playerIndex].currentActionState;
                            if (selectedButton == SelectedButton.Defend || selectedButton == SelectedButton.Escape || selectedButton == SelectedButton.Switch)
                            {
                                selectedButton = SelectedButton.None;
                            }

                            players[playerIndex].gameObject.GetComponent<Image>().color = selected;

                        }
                    }
                    break;
                #endregion
                case SelectedButton.Attack:
                    #region 공격 버튼 선택중
                    //현재 선택된 몬스터 오브젝트를 가지고 있음
                    GameObject MonsterTemp = TargetMonsters();
                    //0.3초마다 이동
                    if (Time.time - getKeyTime >= 0.3f)
                    {
                        //좌우 화살표 누를때마다 몬스터 위치 이동
                        if (Input.GetKey(KeyCode.RightArrow))
                        {
                            getKeyTime = Time.time;
                            AllTargetArrowActiveFalse();
                            targetIndex++;
                            if (targetIndex >= spawnManager.spawnMonsters.Length) targetIndex = 0;
                        }
                        if (Input.GetKey(KeyCode.LeftArrow))
                        {
                            getKeyTime = Time.time;
                            AllTargetArrowActiveFalse();
                            targetIndex--;
                            if (targetIndex < 0) targetIndex = spawnManager.spawnMonsters.Length - 1;
                        }
                    }
                    //A 누르면 현재 몬스터 선택
                    if (Input.GetKeyUp(KeyCode.A))
                    {
                        players[playerIndex].targetedMonsters[0] = MonsterTemp;
                        NextPlayer();
                    }
                    //B 누르면 공격 상태 취소
                    if (Input.GetKeyUp(KeyCode.B))
                    {
                        MenuMovingRight();
                        selectedButton = SelectedButton.None;
                    }
                    break;
                #endregion
                case SelectedButton.Skill:
                    //수정필요
                    #region 스킬 버튼 선택중
                    InstantiateSkillButtons();
                    //0.3초마다 이동하고, 가지고 있는 스킬이 있을 경우에만 활성화                

                    if (skillSelected)
                    {
                        int skillID = int.Parse(skillList[skillListIndex].transform.GetChild(0).GetComponent<Text>().text);
                        //현재 선택된 몬스터 오브젝트를 가지고 있음
                        if (players[playerIndex].skills[skillID].Allies == PlayerData.SkillData.ALLIES.Enemy)
                        {
                            GameObject[] monstersTemp = TargetMonsters(int.Parse(skillList[skillListIndex].transform.GetChild(0).GetComponent<Text>().text));
                            //0.3초마다 이동
                            if (Time.time - getKeyTime >= 0.3f)
                            {
                                //좌우 화살표 누를때마다 몬스터 위치 이동
                                if (Input.GetKey(KeyCode.RightArrow))
                                {
                                    getKeyTime = Time.time;
                                    AllTargetArrowActiveFalse();
                                    targetIndex++;
                                    if (targetIndex >= spawnManager.spawnMonsters.Length) targetIndex = 0;
                                }
                                if (Input.GetKey(KeyCode.LeftArrow))
                                {
                                    getKeyTime = Time.time;
                                    AllTargetArrowActiveFalse();
                                    targetIndex--;
                                    if (targetIndex < 0) targetIndex = spawnManager.spawnMonsters.Length - 1;
                                }
                                //몬스터 타겟팅 확정
                                if (Input.GetKeyUp(KeyCode.A))
                                {
                                    getKeyTime = Time.time;
                                    players[playerIndex].targetedMonsters = monstersTemp;
                                    ClearSkillButtons();
                                    skillListIndex = 0;
                                    MenuMovingRight();
                                    skillSelected = false;
                                    NextPlayer();
                                }
                            }
                        }
                        else if (players[playerIndex].skills[skillID].Allies == PlayerData.SkillData.ALLIES.Allies)
                        {
                            //선택할 플레이어를 미리 들고 있음
                            GameObject[] playerTemp = TargetPlayers(int.Parse(skillList[skillListIndex].transform.GetChild(0).GetComponent<Text>().text));
                            //0.3초마다 이동
                            if (Time.time - getKeyTime >= 0.3f)
                            {
                                //좌우 화살표 누를때마다 플레이어 위치 이동
                                if (Input.GetKey(KeyCode.RightArrow))
                                {
                                    getKeyTime = Time.time;
                                    AllPlayerColorNone();
                                    positionIndex++;
                                    if (positionIndex >= players.Length) positionIndex = 0;
                                }
                                if (Input.GetKey(KeyCode.LeftArrow))
                                {
                                    getKeyTime = Time.time;
                                    AllPlayerColorNone();
                                    positionIndex--;
                                    if (positionIndex < 0) positionIndex = players.Length - 1;
                                }
                                //플레이어 확정
                                if (Input.GetKeyUp(KeyCode.A))
                                {
                                    getKeyTime = Time.time;
                                    players[playerIndex].targetedPlayers = playerTemp;
                                    ClearSkillButtons();
                                    AllPlayerColorNone();
                                    skillListIndex = 0;
                                    positionIndex = 0;
                                    MenuMovingRight();
                                    skillSelected = false;
                                    NextPlayer();
                                }
                            }
                        }
                    }
                    else
                    {
                        //스킬 리스트 활성화 중에는 스킬 리스트를 조작하도록 함
                        if (Time.time - getKeyTime >= 0.3f && skillList.Count > 0)
                        {
                            if (Input.GetKey(KeyCode.DownArrow))
                            {
                                skillList[skillListIndex].animator.SetTrigger("Normal");
                                getKeyTime = Time.time;
                                skillListIndex++;
                                if (skillListIndex >= skillList.Count) skillListIndex = skillList.Count - 1;
                                skillTable.GetComponent<ScrollRect>().verticalScrollbar.value -= (float)1 / (float)skillList.Count;
                                skillList[skillListIndex].animator.SetTrigger("Highlighted");                             
                            }
                            if (Input.GetKey(KeyCode.UpArrow))
                            {
                                skillList[skillListIndex].animator.SetTrigger("Normal");
                                getKeyTime = Time.time;
                                skillListIndex--;
                                if (skillListIndex < 0) skillListIndex = 0;
                                skillTable.GetComponent<ScrollRect>().verticalScrollbar.value += (float)1 / (float)skillList.Count;
                                skillList[skillListIndex].animator.SetTrigger("Highlighted");
                            }
                            //스킬 설명
                            statusText.text = players[playerIndex].skills[int.Parse(skillList[skillListIndex].transform.GetChild(0).GetComponent<Text>().text)].Explain;

                            //선택된 스킬의 정보를 Player에 저장해 둠
                            //추가 조작 필요
                            if (Input.GetKeyUp(KeyCode.A))
                            {
                                OnClickSkillList(int.Parse(skillList[skillListIndex].transform.GetChild(0).GetComponent<Text>().text));
                                skillSelected = true;
                            }
                        }
                    }

                    //스킬 버튼 선택 상태 취소                
                    if (Input.GetKeyUp(KeyCode.B))
                    {
                        ClearSkillButtons();
                        skillListIndex = 0;
                        MenuMovingRight();
                        AllPlayerColorNone();
                        skillSelected = false;
                        selectedButton = SelectedButton.None;
                    }
                    break;
                #endregion
                case SelectedButton.Defend:
                    NextPlayer();
                    selectedButton = SelectedButton.None;
                    break;
                case SelectedButton.Item:
                    //수정필요
                    #region 아이템 버튼 선택중
                    InstantiateItemButtons();
                   
                    if (itemSelected)
                    {
                        if (Time.time - getKeyTime >= 0.3f)
                        {
                            players[positionIndex].GetComponent<Image>().color = posSelected;
                            //좌우 화살표 누를때마다 플레이어 위치 이동
                            if (Input.GetKey(KeyCode.RightArrow))
                            {
                                getKeyTime = Time.time;
                                AllPlayerColorNone();
                                positionIndex++;
                                if (positionIndex >= players.Length) positionIndex = 0;
                            }
                            if (Input.GetKey(KeyCode.LeftArrow))
                            {
                                getKeyTime = Time.time;
                                AllPlayerColorNone();
                                positionIndex--;
                                if (positionIndex < 0) positionIndex = players.Length - 1;
                            }

                            //아이템 설명
                            //statusText.text = PartyManager._instance.GetItemRecord
                            //    (int.Parse(skillList[skillListIndex].transform.GetChild(0).GetComponent<Text>().text)).cExplain;

                            if (Input.GetKeyUp(KeyCode.A))
                            {
                                GameObject[] playerTemp = new GameObject[1];
                                playerTemp[0] = players[positionIndex].gameObject;
                                menu.SetActive(false);
                                getKeyTime = Time.time;
                                players[playerIndex].targetedPlayers = playerTemp;
                                ClearItemButtons();
                                AllPlayerColorNone();
                                itemListIndex = 0;
                                positionIndex = 0;
                                MenuMovingRight();
                                itemSelected = false;
                                NextPlayer();
                            }
                        }
                    }
                    else
                    {
                        //0.3초마다 이동하고, 아이템리스트가 존재할 경우에만 입력을 받음
                        if (Time.time - getKeyTime >= 0.3f && itemList.Count > 0)
                        {
                            if (Input.GetKey(KeyCode.DownArrow))
                            {
                                itemList[itemListIndex].animator.SetTrigger("Normal");
                                getKeyTime = Time.time;
                                itemListIndex++;
                                if (itemListIndex >= itemList.Count) itemListIndex = itemList.Count - 1;
                                itemTable.GetComponent<ScrollRect>().verticalScrollbar.value -= (float)1 / (float)itemList.Count;
                                itemList[itemListIndex].animator.SetTrigger("Highlighted");
                            }
                            if (Input.GetKey(KeyCode.UpArrow))
                            {
                                itemList[itemListIndex].animator.SetTrigger("Normal");
                                getKeyTime = Time.time;
                                itemListIndex--;
                                if (itemListIndex < 0) itemListIndex = 0;
                                itemTable.GetComponent<ScrollRect>().verticalScrollbar.value += (float)1 / (float)itemList.Count;

                                itemList[itemListIndex].animator.SetTrigger("Highlighted");
                            }
                            //추가 조작 필요함
                            if (Input.GetKeyUp(KeyCode.A))
                            {
                                OnClickItemList(int.Parse(itemList[itemListIndex].transform.GetChild(0).GetComponent<Text>().text));
                                itemSelected = true;
                            }

                        }
                    }

                    if (Input.GetKeyUp(KeyCode.B))
                    {
                        ClearItemButtons();
                        itemListIndex = 0;
                        MenuMovingRight();
                        AllPlayerColorNone();
                        itemSelected = false;
                        selectedButton = SelectedButton.None;
                    }

                    break;
                #endregion
                case SelectedButton.Switch:
                    #region 스위치 버튼 선택중
                    LineUIState();

                    switchActive = true;
                    characterSelect = true;

                    // Switch 활성화 시 선택한 플레이어 칸의 색을 비선택 상태의 색으로 변경
                    for (int i = 0; i < playerPos.Length; i++)
                    {
                        if (i != positionIndex)
                        {
                            playerPos[i].GetComponent<Image>().color = nonSelected;
                        }
                    }

                    // 색 변경 함수 실행
                    ChangeColor();

                    if (Input.GetKeyUp(KeyCode.RightArrow))
                    {
                        NextSwitchSelect();
                    }

                    else if (Input.GetKeyUp(KeyCode.LeftArrow))
                    {
                        prevSwitchSelect();
                    }

                    // Switch 활성화 상태에서 선택시
                    else if (Input.GetKeyUp(KeyCode.A))
                    {
                        // Switch 활성화 위치를 바꿀 첫번째 플레이어 선택
                        if (characterSelect == true && positionSelect == false)
                        {
                            positionSelect = true;

                            switchToPosIndex = (int)players[switchIndex].transform.parent.GetComponent<PositionIndex>().posindex;

                            for (int i = 0; i < players.Length; i++)
                            {
                                players[i].GetComponent<Image>().color = nonSelected;
                            }

                            players[switchIndex].GetComponent<Image>().color = selected;

                            playerPos[0].GetComponent<Image>().color = posSelected;

                            characterPos = playerPos[switchToPosIndex].transform.GetChild(0);
                        }

                        // Switch 활성화 위치를 바꿀 두번째 플레이어 선택
                        else if (characterSelect == true && positionSelect == true)
                        {
                            // 두번째 칸 선택시 칸에 플레이어 캐릭터가 있을 경우
                            if (playerPos[positionIndex].transform.childCount != 0)
                            {
                                positionPos = playerPos[positionIndex].transform.GetChild(0);

                                tempPos = positionPos.transform.parent;

                                positionPos.transform.SetParent(characterPos.transform.parent);
                                characterPos.transform.SetParent(tempPos);

                                RectTransform positionRect = positionPos.GetComponent<RectTransform>();
                                RectTransform characterRect = characterPos.GetComponent<RectTransform>();

                                positionRect.localPosition = Vector3.zero;
                                positionRect.offsetMin = Vector2.zero;
                                positionRect.offsetMax = Vector2.zero;

                                characterRect.localPosition = Vector3.zero;
                                characterRect.offsetMin = Vector2.zero;
                                characterRect.offsetMax = Vector2.zero;

                                players[switchIndex].GetComponent<Image>().color = nonSelected;

                                int tempIndex = 0;
                                for (int i = 0; i < playerPos.Length; i++)
                                {
                                    if (playerPos[i].transform.childCount != 0)
                                    {
                                        temp[tempIndex] = playerPos[i].transform.GetChild(0);
                                        tempIndex++;
                                    }
                                }

                                for (int i = 0; i < players.Length; i++)
                                {
                                    BattlePlayer refPlayer = temp[i].GetComponent<BattlePlayer>();
                                    players[i] = refPlayer;
                                    players[i].GetComponent<Image>().color = nonSelected;
                                }

                                for (int i = 0; i < playerPos.Length; i++)
                                {
                                    playerPos[i].GetComponent<Image>().color = nonSelected;
                                }

                                players[0].GetComponent<Image>().color = selected;
                            }

                            // 두번째 칸 선택시 칸에 플레이어 캐릭터가 없을 경우
                            else if (playerPos[positionIndex].transform.childCount == 0)
                            {
                                positionPos = playerPos[positionIndex].transform;

                                if (line.GetComponentsInChildren<Image>()[1].GetComponentsInChildren<BattlePlayer>().Length <= 1)
                                {
                                    if (positionPos == playerPos[0].transform || positionPos == playerPos[1].transform || positionPos == playerPos[2].transform)
                                    {
                                        characterPos.transform.SetParent(positionPos);
                                    }

                                    else
                                    {
                                        characterPos.transform.SetParent(characterPos.transform.parent);
                                    }
                                }

                                else
                                {
                                    characterPos.transform.SetParent(playerPos[positionIndex].transform);
                                }

                                RectTransform characterRect = characterPos.GetComponent<RectTransform>();

                                characterRect.localPosition = Vector3.zero;
                                characterRect.offsetMin = Vector2.zero;
                                characterRect.offsetMax = Vector2.zero;

                                players[switchIndex].GetComponent<Image>().color = nonSelected;

                                int tempIndex = 0;
                                for (int i = 0; i < playerPos.Length; i++)
                                {
                                    if (playerPos[i].transform.childCount != 0)
                                    {
                                        temp[tempIndex] = playerPos[i].transform.GetChild(0);
                                        tempIndex++;
                                    }
                                }

                                for (int j = 0; j < players.Length; j++)
                                {
                                    BattlePlayer refPlayer = temp[j].GetComponent<BattlePlayer>();
                                    players[j] = refPlayer;
                                }

                                for (int i = 0; i < playerPos.Length; i++)
                                {
                                    playerPos[i].GetComponent<Image>().color = nonSelected;
                                }

                                players[0].GetComponent<Image>().color = selected;
                            }

                            // 두번째 칸 선택시 칸에 플레이어 캐릭터가 첫번째와 같은 경우
                            else if (characterPos == positionPos)
                            {
                                return;
                            }

                            positionSelect = false;

                            //ChangeColor();

                            playerIndex = 0;
                            switchIndex = 0;
                            positionIndex = 0;
                        }
                    }

                    // Switch 활성화 상태에서 취소 선택시
                    else if (Input.GetKeyUp(KeyCode.B))
                    {
                        // Switch 활성화 상태에서 비활성화 상태로 돌아감
                        if (characterSelect == true && positionSelect == false)
                        {
                            switchActive = false;
                            characterSelect = false;
                            ChangeColor();

                            switchIndex = 0;

                            LineUIState();
                            selectedButton = SelectedButton.None;

                            for (int i = 0; i < players.Length; i++)
                            {
                                players[i].playerInfo.partyPosition = (int)players[i].transform.parent.GetComponent<PositionIndex>().posindex;
                            }
                        }

                        // 첫번째 플레이어 선택 상태를 취소함
                        else if (characterSelect == true && positionSelect == true)
                        {
                            positionSelect = false;
                            ChangeColor();

                            positionIndex = 0;
                            playerPos[positionIndex].GetComponent<Image>().color = nonSelected;
                        }
                    }

                    #endregion
                    break;
                case SelectedButton.Escape:
                    NextPlayer();
                    selectedButton = SelectedButton.None;
                    break;
            }
        }
    }

    #region 마우스 클릭 대응용 함수
    public void Attack()
    {
        selectedButton = SelectedButton.Attack;
    }

    public void Skill()
    {
        selectedButton = SelectedButton.Skill;
    }

    public void Defend()
    {
        selectedButton = SelectedButton.Defend;
    }

    public void Item()
    {
        selectedButton = SelectedButton.Item;
    }

    public void Switch()
    {
        selectedButton = SelectedButton.Switch;
    }

    public void Escape()
    {
        selectedButton = SelectedButton.Escape;
    }
    #endregion

    //모든 targetarrow 비활성화
    void AllTargetArrowActiveFalse()
    {
        if (targetArrow != null)
        {
            foreach (var arrow in targetArrow)
            {
                if (arrow != null)
                    arrow.SetActive(false);
            }
        }
    }
    //전체 플레이어 색깔 초기화
    void AllPlayerColorNone()
    {
        for (int i = 0; i < players.Length; i++)
        {
            players[i].GetComponent<Image>().color = nonSelected;
        }

        players[playerIndex].GetComponent<Image>().color = selected;
    }
    //다음 플레이어로 넘김
    void NextPlayer()
    {
        players[playerIndex].currentActionState = (CurrentActionState)selectedButton;
        players[playerIndex].gameObject.GetComponent<Image>().color = nonSelected;

        //죽은 플레이어가 아닐때 or 플레이어의 숫자보다 클 때까지 index 증가
        do
        {
            playerIndex++;
            if (playerIndex >= players.Length) break;
        }
        while (players[playerIndex].playerInfo.isDown);

        //선택할 플레이어가 없음
        if (playerIndex >= players.Length)
        {
            battleManager.StartTurnProgressingCoroutine();
        }
        else
        {
            players[playerIndex].gameObject.GetComponent<Image>().color = selected;
        }
        menu.SetActive(true);
        actions[actionIndex].animator.SetTrigger("Highlighted");
        selectedButton = SelectedButton.None;
    }

    // Switch상태에서 다음 칸 선택
    public void NextSwitchSelect()
    {
        if (characterSelect == true && positionSelect == false)
        {
            players[switchIndex].GetComponent<Image>().color = nonSelected;
            switchIndex++;

            if (switchIndex >= players.Length)
            {
                switchIndex = players.Length - 1;
            }

            players[switchIndex].GetComponent<Image>().color = selected;
        }

        else if (characterSelect == true && positionSelect == true)
        {
            playerPos[positionIndex].GetComponent<Image>().color = nonSelected;
            positionIndex++;

            if (positionIndex >= playerPos.Length)
            {
                positionIndex = playerPos.Length - 1;
            }

            playerPos[positionIndex].GetComponent<Image>().color = posSelected;
        }

    }

    // Switch상태에서 이전 칸 선택
    public void prevSwitchSelect()
    {
        if (characterSelect == true && positionSelect == false)
        {
            players[switchIndex].GetComponent<Image>().color = nonSelected;
            switchIndex--;

            if (switchIndex < 0)
            {
                switchIndex = 0;
            }
            players[switchIndex].GetComponent<Image>().color = selected;
        }

        else if (characterSelect == true && positionSelect == true)
        {
            playerPos[positionIndex].GetComponent<Image>().color = nonSelected;
            positionIndex--;

            if (positionIndex < 0)
            {
                positionIndex = 0;
            }
            playerPos[positionIndex].GetComponent<Image>().color = posSelected;
        }
    }

    public void NextStatusSpace()
    {
        statusList[statusIndex].GetComponent<Image>().color = statusNonSelected;
        skillSpace[skillindex].gameObject.GetComponent<Image>().color = skillSpaceNonSelected;

        statusIndex++;
        skillindex = 0;

        if (statusIndex >= statusList.Count)
        {
            statusIndex = statusList.Count - 1;
        }


        List<BattlePlayer.BuffSkillsInfo> buff = statusList[statusIndex].GetComponent<StatusIndex>().battlePlayer.buffSkills;

        List<BattlePlayer.DebuffSkillsInfo> deBuff = statusList[statusIndex].GetComponent<StatusIndex>().battlePlayer.debuffSkills;

        if (buff.Count > 0 && !debuff)
            statusText.text = buff[skillindex].buffs.Explain;

        else if (deBuff.Count > 0 && debuff)
            statusText.text = deBuff[skillindex].debuffs.Explain;

        else
            statusText.text = " ";
        statusList[statusIndex].GetComponent<Image>().color = posSelected;
        skillSpace = statusList[statusIndex].GetComponentInChildren<Outline>().gameObject.GetComponentsInChildren<Mask>();
        skillSpace[skillindex].gameObject.GetComponent<Image>().color = skillSpaceSelected;
    }

    public void PrevStatusSpace()
    {
        statusList[statusIndex].GetComponent<Image>().color = statusNonSelected;
        skillSpace[skillindex].gameObject.GetComponent<Image>().color = skillSpaceNonSelected;

        statusIndex--;
        skillindex = 0;

        if (statusIndex < 0)
        {
            statusIndex = 0;
        }

        List<BattlePlayer.BuffSkillsInfo> buff = statusList[statusIndex].GetComponent<StatusIndex>().battlePlayer.buffSkills;

        List<BattlePlayer.DebuffSkillsInfo> deBuff = statusList[statusIndex].GetComponent<StatusIndex>().battlePlayer.debuffSkills;

        if (buff.Count > 0 && !debuff)
            statusText.text = buff[skillindex].buffs.Explain;

        else if (deBuff.Count > 0 && debuff)
            statusText.text = deBuff[skillindex].debuffs.Explain;

        else
            statusText.text = " ";

        statusList[statusIndex].GetComponent<Image>().color = posSelected;
        skillSpace = statusList[statusIndex].GetComponentInChildren<Outline>().gameObject.GetComponentsInChildren<Mask>();
        skillSpace[skillindex].gameObject.GetComponent<Image>().color = skillSpaceSelected;

    }

    public void NextStatusSkill()
    {
        skillSpace[skillindex].gameObject.GetComponent<Image>().color = skillSpaceNonSelected;
        skillindex++;

        if (skillindex >= skillSpace.Length)
        {
            skillindex = skillSpace.Length - 1;
        }

        List<BattlePlayer.BuffSkillsInfo> buff = statusList[statusIndex].GetComponent<StatusIndex>().battlePlayer.buffSkills;

        List<BattlePlayer.DebuffSkillsInfo> deBuff = statusList[statusIndex].GetComponent<StatusIndex>().battlePlayer.debuffSkills;

        if (buff.Count > 0 && buff.Count -1 >= skillindex && !debuff)
            statusText.text = buff[skillindex].buffs.Explain;

        else if (deBuff.Count > 0 && deBuff.Count - 1 >= skillindex && debuff)
            statusText.text = deBuff[skillindex].debuffs.Explain;
        else
            statusText.text = " ";

        skillSpace[skillindex].gameObject.GetComponent<Image>().color = skillSpaceSelected;
    }

    public void PrevStatusSkill()
    {
        skillSpace[skillindex].gameObject.GetComponent<Image>().color = skillSpaceNonSelected;
        skillindex--;

        if (skillindex < 0)
        {
            skillindex = 0;
        }

        List<BattlePlayer.BuffSkillsInfo> buff = statusList[statusIndex].GetComponent<StatusIndex>().battlePlayer.buffSkills;

        List<BattlePlayer.DebuffSkillsInfo> deBuff = statusList[statusIndex].GetComponent<StatusIndex>().battlePlayer.debuffSkills;

        if (buff.Count > 0 && buff.Count - 1 >= skillindex && !debuff)
            statusText.text = buff[skillindex].buffs.Explain;

        else if (deBuff.Count > 0 && deBuff.Count - 1 >= skillindex && debuff)
            statusText.text = deBuff[skillindex].debuffs.Explain;
        else
            statusText.text = " ";

        skillSpace[skillindex].gameObject.GetComponent<Image>().color = skillSpaceSelected;
    }

    // Switch Action의 활성화 여부를 판별
    public void LineUIState()
    {
        if (switchActive == false)
        {
            for (int i = 0; i < playerPos.Length; i++)
            {
                if (playerPos[i].transform.childCount == 0)
                {
                    playerPos[i].SetActive(false);
                }
            }
        }

        else if (switchActive == true)
        {
            for (int i = 0; i < playerPos.Length; i++)
            {
                playerPos[i].SetActive(true);
            }
        }

    }

    //메인메뉴 왼쪽으로
    void MenuMovingLeft()
    {
        isMenuMoving = true;
        menuMovingStartPos = menu.GetComponent<RectTransform>().position;
        menuMovingEndPos = new Vector3(0.695f, menu.GetComponent<RectTransform>().position.y, menu.GetComponent<RectTransform>().position.z);
        menuMovingDuration = Vector3.Distance(menuMovingStartPos, menuMovingEndPos);
        menuMovingStartTime = Time.time;
    }

    //메인메뉴 오른쪽으로
    void MenuMovingRight()
    {
        menu.SetActive(true);
        actions[actionIndex].animator.SetTrigger("Highlighted");
        isMenuMoving = true;
        menuMovingStartPos = menu.GetComponent<RectTransform>().position;
        menuMovingEndPos = new Vector3(0.669f, menu.GetComponent<RectTransform>().position.y, menu.GetComponent<RectTransform>().position.z);
        menuMovingDuration = Vector3.Distance(menuMovingStartPos, menuMovingEndPos);
        menuMovingStartTime = Time.time;
    }

    //각 플레이어가 갖고 있는 스킬 갯수만큼 동적 생성
    void InstantiateSkillButtons()
    {

        if (skillList.Count > 0) return;

        foreach (var skill in players[playerIndex].skills)
        {
            if (skill.Value.Type != PlayerData.SkillData.TYPE.Active) continue;
            GameObject skillTemp = Instantiate(skillPrefab, skillTable.GetComponent<ScrollRect>().content);
            skillTemp.transform.GetChild(0).GetComponent<Text>().text = string.Format("{0}", skill.Value.nID);
            skillTemp.transform.GetChild(1).GetComponent<Text>().text = skill.Value.Name.Replace('_', ' ');
            skillTemp.GetComponent<Button>().onClick.AddListener(() => OnClickSkillList(skill.Value.nID));
            skillList.Add(skillTemp.GetComponent<Button>());
        }
        skillTable.SetActive(true);
        if (skillList.Count > 0)
        {
            skillList[skillListIndex].animator.SetTrigger("Highlighted");
            skillTable.GetComponent<ScrollRect>().verticalScrollbar.value = 1;
        }
        MenuMovingLeft();
    }

    //스킬 리스트 생성할 때 동적으로 onclick에 연결할 메소드
    void OnClickSkillList(int skillId)
    {
        players[playerIndex].selectedSkill = players[playerIndex].skills[skillId];
        players[playerIndex].currentActionState = (CurrentActionState)selectedButton;
    }

    //아이템 리스트 생성할 때 동적으로 onclick에 연결할 메소드
    void OnClickItemList(int itemID)
    {
        players[playerIndex].selectedItem = PartyManager._instance.GetItemRecord(itemID);
        players[playerIndex].currentActionState = (CurrentActionState)selectedButton;
    }

    //인벤토리에 있는 소비성 아이템 동적 생성
    void InstantiateItemButtons()
    {
        //아이템 리스트의 개수가 0이상이면 함수 종료
        if (itemList.Count > 0) return;

        //아이템 버튼들이 생성될 부모 오브젝트(아이템 테이블)의 ScrollRect를 가져옴
        //Scroll Rect => Scroll View에서 쓰는 거
        ScrollRect scrollRect = itemTable.GetComponent<ScrollRect>();

        //40001번대 아이템이 소비성 아이템임
        //각각의 소비성 아이템 id를 가져옴
        conItems = PartyManager._instance.GetInventoryItemList(40001);

        //각각 소비성 아이템 돌면서 아이템 버튼 생성하고 아이템 리스트에 추가
        foreach (var itemNum in conItems)
        {
            Debug.Log(itemNum);
            //소비아이템 레코드에서 id를 기준으로 아이템을 불러옴
            ConItemRecord conItem = PartyManager._instance.GetItemRecord(itemNum);
            //조작용 임시 아이템 버튼
            GameObject itemTemp = Instantiate(itemPrefab, scrollRect.content);
            //아이템 id를 저장해놓는 text. 실 사이즈는 0이라 보이지 않음
            itemTemp.transform.GetChild(0).GetComponent<Text>().text = string.Format("{0}", itemNum);
            //아이템 이름
            itemTemp.transform.GetChild(1).GetComponent<Text>().text = conItem.cStrName;
            //마우스 클릭용으로 아이템 버튼 클릭시 그 아이템의 id를 넘기도록 AddListener에 동적으로 할당해줌
            itemTemp.GetComponent<Button>().onClick.AddListener(() => OnClickItemList(itemNum));
            //아이템 리스트에 추가
            itemList.Add(itemTemp.GetComponent<Button>());
        }

        //아이템 테이블 활성화
        itemTable.SetActive(true);

        //아이템 리스트가 존재하면 버튼 선택 애니메이션 활성화
        //스크롤바 초기화
        if (itemList.Count > 0)
        {
            itemList[itemListIndex].animator.SetTrigger("Highlighted");
            scrollRect.verticalScrollbar.value = 1;
        }
        //메뉴 왼쪽으로 이동
        MenuMovingLeft();
    }

    //생성되었던 스킬버튼 삭제
    void ClearSkillButtons()
    {
        foreach (var obj in skillList)
        {
            Destroy(obj.gameObject);
        }
        skillList.Clear();
        skillListIndex = 0;
        skillTable.SetActive(false);
    }

    //생성되었던 아이템들 삭제
    void ClearItemButtons()
    {
        foreach (var obj in itemList)
        {
            Destroy(obj.gameObject);
        }
        itemList.Clear();
        itemListIndex = 0;
        itemTable.SetActive(false);
    }

    //Attack 용
    public GameObject TargetMonsters()
    {
        GameObject targetedMonsters = spawnManager.spawnMonsters[targetIndex];
        menu.SetActive(false);
        targetArrow = new GameObject[1];
        targetArrow[0] = targetedMonsters.transform.parent.GetChild(0).GetChild(0).gameObject;
        targetArrow[0].SetActive(true);
        statusText.text = targetedMonsters.GetComponent<BattleMonster>().monsterCharacter.monStatRecord.Name;
        return targetedMonsters;
    }
    //Skill용
    public GameObject[] TargetMonsters(int skillId)
    {
        GameObject[] targetedMonsters = null;
        Character_SkillsRecord skill = players[playerIndex].skills[skillId];

        if (skill.Allies == PlayerData.SkillData.ALLIES.Enemy)
        {
            switch (skill.AreaType)
            {
                //AreaType에 따라 다르게 판단
                case PlayerData.SkillData.AREATYPE.Single:
                    targetedMonsters = new GameObject[1];   //타겟팅 몬스터 한마리
                    //targetIndex는 위에서 변함
                    targetedMonsters[0] = spawnManager.spawnMonsters[targetIndex];
                    menu.SetActive(false);
                    //타겟팅용 화살표
                    targetArrow = new GameObject[1];
                    targetArrow[0] = targetedMonsters[0].transform.parent.GetChild(0).GetChild(0).gameObject;
                    targetArrow[0].SetActive(true);
                    //statusText.text = targetedMonsters.GetComponent<Monster>().monsterData.Name;
                    break;
                //1열 타게팅
                case PlayerData.SkillData.AREATYPE.Row:
                    int frontMonsterCount = 0;  //전열 숫자
                    int backMonsterCount = 0;   //후열 숫자
                    //스폰된 몬스터 수를 셈
                    for (int i = 0; i < spawnManager.spawnMonsters.Length; i++)
                    {
                        //전열인지 후열인지 판단해서 전열에 있는 몬스터 숫자와 후열에 있는 몬스터 숫자를 계산
                        string name = spawnManager.spawnMonsters[i].transform.parent.parent.name;
                        if (name.Equals("FrontPoint"))
                            frontMonsterCount++;
                        else if (name.Equals("BackPoint"))
                            backMonsterCount++;
                    }
                    //targetindex는 계속 증가하지만, 전열 후열 둘 뿐이므로 %2로 계산
                    if (targetIndex % 2 == 0)
                    {
                        if (frontMonsterCount == 0) targetIndex++; 
                        //전열 숫자만큼 타겟팅 몬스터와 화살표 배열 생성
                        targetedMonsters = new GameObject[frontMonsterCount];
                        targetArrow = new GameObject[frontMonsterCount];

                        //전열의 몬스터를 전부 타겟팅
                        for (int i = 0; i < targetArrow.Length; i++)
                        {
                            targetedMonsters[i] = spawnManager.spawnMonsters[i];
                            targetArrow[i] = targetedMonsters[i].transform.parent.GetChild(0).GetChild(0).gameObject;
                            targetArrow[i].SetActive(true);
                        }
                    }
                    else
                    {
                        //후열에 아무도 없으면 1을 빼버림
                        if (backMonsterCount == 0) targetIndex--;

                        if (backMonsterCount > 0)
                        {
                            //후열 몬스터 수 만큼 배열 만들기
                            targetedMonsters = new GameObject[backMonsterCount];
                            targetArrow = new GameObject[backMonsterCount];

                            //후열에 있는 몬스터 전부 타겟팅
                            for (int i = 0; i < targetArrow.Length; i++)
                            {
                                targetedMonsters[i] = spawnManager.spawnMonsters[i + frontMonsterCount];
                                targetArrow[i] = targetedMonsters[i].transform.parent.GetChild(0).GetChild(0).gameObject;
                                targetArrow[i].SetActive(true);
                            }
                        }
                    }
                    menu.SetActive(false);
                    break;
                //전체 타겟팅
                case PlayerData.SkillData.AREATYPE.Area:
                    //최대 다섯마리까지
                    targetedMonsters = new GameObject[spawnManager.spawnMonsters.Length];
                    targetArrow = new GameObject[spawnManager.spawnMonsters.Length];

                    //전부 타겟팅하게 함
                    for (int i = 0; i < spawnManager.spawnMonsters.Length; i++)
                    {
                        targetedMonsters[i] = spawnManager.spawnMonsters[i];
                        targetArrow[i] = targetedMonsters[i].transform.parent.GetChild(0).GetChild(0).gameObject;
                        targetArrow[i].SetActive(true);
                    }
                    menu.SetActive(false);
                    break;
                //일단 디폴트
                case PlayerData.SkillData.AREATYPE.None:
                    targetedMonsters = new GameObject[1];
                    targetedMonsters[0] = spawnManager.spawnMonsters[targetIndex];
                    menu.SetActive(false);
                    targetArrow = new GameObject[1];
                    targetArrow[0] = targetedMonsters[0].transform.parent.GetChild(0).GetChild(0).gameObject;
                    targetArrow[0].SetActive(true);
                    break;
            }
        }

        //만약에 타겟몬스터의 길이가 2이상인데, 한마리만 선택중이면 한마리의 정보만 출력
        if (targetedMonsters.Length >= 2 && targetedMonsters[1] == null)
            statusText.text = targetedMonsters[0].GetComponent<BattleMonster>().monsterCharacter.monStatRecord.Name;
        //타겟몬스터의 길이가 1이면 한마리밖에 없으므로 그 몬스터의 정보 출력
        else if (targetedMonsters.Length <= 1)
        {
            statusText.text = targetedMonsters[0].GetComponent<BattleMonster>().monsterCharacter.monStatRecord.Name;
        }
        //나머지는 여러마리 타겟팅 중이므로 몬스터 무리로 출력함
        else
        {
            statusText.text = "몬스터 무리";
        }
        //타겟팅 몬스터 반환
        return targetedMonsters;
    }

    //스킬 플레이어 선택
    //타겟 몬스터랑 구조는 비슷함
    public GameObject[] TargetPlayers(int skillId)
    {
        GameObject[] targetedPlayers = null;
        Character_SkillsRecord skill = players[playerIndex].skills[skillId];

        if (skill.Allies == PlayerData.SkillData.ALLIES.Allies)
        {
            switch (skill.AreaType)
            {
                //1명 선택
                case PlayerData.SkillData.AREATYPE.Single:
                    targetedPlayers = new GameObject[1];
                    //positionIndex는 위에서 화살표로 조종
                    if (!(skill.nID == (int)SKILLLIST.Medic_Resurrection))
                    {
                        while (players[positionIndex].playerInfo.isDown)
                        {
                            positionIndex++;
                        }
                    }
                    targetedPlayers[0] = players[positionIndex].gameObject;
                    //targetArrow 대신 색깔을 바꿈
                    targetedPlayers[0].GetComponent<Image>().color = posSelected;
                    menu.SetActive(false);
                    break;
                //1열 선택
                case PlayerData.SkillData.AREATYPE.Row:
                    int frontPlayerCount = 0;
                    int backPlayerCount = 0;
                    int frontPlayerDownCount = 0;
                    int backPlayerDownCount = 0;

                    //파티포지션으로 비교할 수 있음
                    //0, 1, 2는 전열이고 3, 4, 5는 후열임
                    for (int i = 0; i < players.Length; i++)
                    {
                        if (players[i].playerInfo.partyPosition <= 2)
                        {
                            frontPlayerCount++;
                            if (players[i].playerInfo.isDown) frontPlayerDownCount++;
                        }
                        else if (players[i].playerInfo.partyPosition >= 3)
                        {
                            backPlayerCount++;
                            if (players[i].playerInfo.isDown) backPlayerDownCount++;
                        }
                    }

                    //마찬가지로 positionIndex는 계속 증가하지만 2가지 경우의 수 뿐이므로 %2로 계산
                    if (positionIndex % 2 == 0)
                    {
                        //전열이 다 죽어있으면 더해준다
                        if (frontPlayerDownCount == frontPlayerCount) positionIndex++;

                        //전열 숫자만큼 배열 생성
                        targetedPlayers = new GameObject[frontPlayerCount];

                        //플레이어 정보 집어넣음
                        for (int i = 0; i < targetedPlayers.Length; i++)
                        {
                            targetedPlayers[i] = players[i].gameObject;
                            if(!targetedPlayers[i].GetComponent<BattlePlayer>().playerInfo.isDown)
                                targetedPlayers[i].GetComponent<Image>().color = posSelected;
                        }
                    }
                    else
                    {
                        //후열에 아무도 없거나 다 죽어있으면 빼버림
                        if (backPlayerCount == 0 || backPlayerDownCount == backPlayerCount) positionIndex--;

                        if (backPlayerCount > 0)
                        {
                            //후열 숫자만큼 배열 생성
                            targetedPlayers = new GameObject[backPlayerCount];

                            //후열의 플레이어 정보를 집어넣음
                            for (int i = 0; i < targetedPlayers.Length; i++)
                            {
                                targetedPlayers[i] = players[i + frontPlayerCount].gameObject;
                                if (!targetedPlayers[i].GetComponent<BattlePlayer>().playerInfo.isDown)
                                    targetedPlayers[i].GetComponent<Image>().color = posSelected;
                            }
                        }
                    }
                    menu.SetActive(false);
                    break;
                //전체 선택
                case PlayerData.SkillData.AREATYPE.Area:
                    targetedPlayers = new GameObject[5];

                    //전체 플레이어를 집어넣음
                    for (int i = 0; i < players.Length; i++)
                    {
                        targetedPlayers[i] = players[i].gameObject;
                        if (!targetedPlayers[i].GetComponent<BattlePlayer>().playerInfo.isDown)
                            targetedPlayers[i].GetComponent<Image>().color = posSelected;
                    }
                    menu.SetActive(false);
                    break;
                //나 자신만 선택
                case PlayerData.SkillData.AREATYPE.Myself:
                    targetedPlayers = new GameObject[1];
                    targetedPlayers[playerIndex] = players[playerIndex].gameObject;
                    //targetArrow 대신 색깔을 바꿈
                    targetedPlayers[playerIndex].GetComponent<Image>().color = posSelected;
                    menu.SetActive(false);
                    break;
                //디폴트
                case PlayerData.SkillData.AREATYPE.None:
                    targetedPlayers = new GameObject[1];
                    targetedPlayers[0] = players[positionIndex].gameObject;
                    menu.SetActive(false);
                    break;
            }
        }
        List<GameObject> tempPlayer = new List<GameObject>();

        for(int i = 0; i < targetedPlayers.Length; i++)
        {
            if (!targetedPlayers[i].GetComponent<BattlePlayer>().playerInfo.isDown)
                tempPlayer.Add(targetedPlayers[i]);
        }

        targetedPlayers = new GameObject[tempPlayer.Count];
  
        for (int i = 0; i < tempPlayer.Count; i++)
        {
            targetedPlayers[i] = tempPlayer[i];
        }

        return targetedPlayers;
    }

    // switch시 각 칸 별로 색 변화
    public void ChangeColor()
    {
        if (switchActive == true)
        {
            players[playerIndex].GetComponent<Image>().color = nonSelected;
            players[switchIndex].GetComponent<Image>().color = selected;
        }

        else if (positionSelect == false)
        {
            players[positionIndex].GetComponent<Image>().color = nonSelected;
            players[switchIndex].GetComponent<Image>().color = nonSelected;
            playerPos[positionIndex].GetComponent<Image>().color = nonSelected;
        }

        else if (switchActive == false)
        {
            playerIndex++;

            while (players[playerIndex].playerInfo.isDown)
            {
                playerIndex++;
            }
            players[playerIndex].GetComponent<Image>().color = selected;
            players[switchIndex].GetComponent<Image>().color = nonSelected;
            playerPos[positionIndex].GetComponent<Image>().color = nonSelected;
        }
    }

    //턴 종료시 UI 갱신
    public void TurnEnd()
    {
        menu.SetActive(true);
        actions[actionIndex].animator.SetTrigger("Highlighted");
        turn++;
        turnText.text = string.Format("Turn : {0}", turn);
        actionIndex = 0;
        playerIndex = 0;
        targetIndex = 0;

        // 턴 종료시 Monster배열 정렬 함수 호출
        //CheckMonsterArray();

        targetIndex = 0;
   
    }

    // Monster배열 정렬 코루틴 호출 함수
    public void CheckMonsterArray()
    {
        for (int i = 0; i < spawnManager.spawnMonsters.Length - 1; i++)
        {
            if (spawnManager.spawnMonsters[i] == null)
                StartCoroutine(MonsterArrayRealignment());
        }
    }

    public void ShowResultUi()
    {
        for (int i = 0; i < characterResult.Length; i++)
        {
            characterResult[i].GetComponent<ShowResultInfo>().ShowResult();
        }

        totalExpText.text = string.Format("{0}", totalExp);
    }

    public void ShowLineUI()
    {
        for (int i = 0; i < players.Length; i++)
        {
            players[i].SetText();
        }
    }

    public void AllUIDown()
    {
        line.SetActive(false);
        actionMenu.gameObject.SetActive(false);
        state.gameObject.SetActive(false);
        result.gameObject.SetActive(false);
        statusUI.gameObject.SetActive(false);
    }

    public void ShowStatusUI()
    {
        AllUIDown();
        statusUI.gameObject.SetActive(true);
        controlText.SetActive(false);
        state.gameObject.SetActive(true);


        if (debuff == false)
        {
            turnText.text = string.Format("Buff");
            for (int i = 0; i < statusList.Count; i++)
            {
                if (statusList[i].activeInHierarchy == true)
                {
                    statusList[i].GetComponent<StatusIndex>().SetBuffStatus();
                }
            }
        }
        else if (debuff == true)
        {
            turnText.text = string.Format("Debuff");
            for (int i = 0; i < statusList.Count; i++)
            {
                if (statusList[i].activeInHierarchy == true)
                {
                    statusList[i].GetComponent<StatusIndex>().SetDebuffStatus();
                }
            }
        }     
    }

    public void QuitStatusUI()
    {
        controlText.SetActive(true);
        line.SetActive(true);
        actionMenu.gameObject.SetActive(true);
        statusUI.gameObject.SetActive(false);
        statusOn = false;
        debuff = false;
        statusList.Clear();

        turnText.text = string.Format("Turn : {0}", turn);
    }

    public void SetDropresultItemText()
    {
        for (int i = 0; i < resultItemText.Length; i++)
        {
            if(i < gameObject.GetComponent<SpawnManager>().monsterDropItem.Count)
                resultItemText[i].GetComponentInParent<Image>().gameObject.SetActive(true);
            else
                resultItemText[i].GetComponentInParent<Image>().gameObject.SetActive(false);
        }

        //resultItemText.text = gameObject.GetComponent<SpawnManager>().monsterDropItem[]

        for (int i = 0; i < gameObject.GetComponent<SpawnManager>().monsterDropItem.Count; i++)
        {
            resultItemText[i].text = gameObject.GetComponent<SpawnManager>().monsterDropItem[i].dropItem;
        }
    }

    // 몬스터가 없어졌을때 1초 뒤 ResultUI canvas활성화 및 기타 UI 비활성화
    public IEnumerator ResultDelay()
    {
        yield return new WaitForSeconds(1.0f);

        while (BattleEnd == false)
        {
            totalExp = gameObject.GetComponent<SpawnManager>().totalExp;

            AllUIDown();
            result.gameObject.SetActive(true);
            ShowResultUi();
            SetDropresultItemText();

            if(isInventoryOver == false)
            {
                AddInventory();
            }

            BattleEnd = true;
        }
    }

    // 배틀 씬에 몬스터가 모두 없어졌을때를 체크
    public void CheckBattle()
    {
        int aliveCount = spawnManager.spawnCount;

        if (aliveCount <= 0)
        {
            if (startCheckInventory == true)
            {
                CheckInventoryControl();
            }

            if (Input.GetKeyUp(KeyCode.Space) && isInventoryOver == true && startCheckInventory == false)
            {
                StopAllCoroutines();
                result.gameObject.SetActive(false);
                StartCheckInventory();
                startCheckInventory = true;
            }

            else if (Input.GetKeyUp(KeyCode.Space) && isInventoryOver == false && startCheckInventory == false)
            {
                if (SceneChanger.instance != null) SceneChanger.instance.SceneChange();
                SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("Battle " + SceneChanger.instance.floor));
                SceneChanger.instance.spawnCase = (SpawnCase)0;
            }
        }
    }

    // 현재 인벤토리 아이템 개수 + 드랍된 아이템 개수를 더해 60이 넘는지 검출
    public void CheckInventoryOver()
    {
        allItemList = PartyManager._instance.GetInventoryItemList();
        dropItems = gameObject.GetComponent<SpawnManager>().monsterDropItem;

        checkInventoryCount = allItemList.Count + dropItems.Count;

        if (checkInventoryCount > 60)
        {
            isInventoryOver = true;
        }
    }

    // 현재 드랍된 모든 아이템을 인벤토리에 추가하는 함수(임시 테스트용, 추후 삭제)
    public void AddInventory()
    {
        for (int i = 0; i < dropItems.Count; i++)
        {
            PartyManager._instance.SetInventoryItem(dropItems[i].nID);
        }
    }

    // 인벤토리 정리UI 호출 및 초기값 설정 함수
    public void StartCheckInventory()
    {
        // 정리UI 활성화
        overInventoryCanvas.gameObject.SetActive(true);
        // 메시지 창 비활성화
        messageWIndow.SetActive(false);

        // 인벤토리 창에 아이템 버튼 생성
        InstantiateInventoryButton(InventoryOverWindows[0], allItemList);
        // 드랍 아이템 창에 아이템 버튼 생성
        InstantiateDropItemButton(InventoryOverWindows[1], dropItems);

        // 인벤토리 창 최상단 아이템 버튼 애니메이션 활성화
        allItemButtonList[allItemListIndex].animator.SetTrigger("Highlighted");

        // 현재 인벤토리 아이템 개수와 최대 인벤토리 칸 개수 표시
        SetTempInventoryCount(allItemList, tempInventoryText);
    }

    // 인벤토리 정리UI 실행 후 정리UI 조작을 위한 함수
    public void CheckInventoryControl()
    {
        // 인벤토리 정리UI에서 인벤토리 창 선택
        if (Input.GetKeyUp(KeyCode.LeftArrow) && allItemExplore == false && allItemButtonList.Count != 0)
        {
            allItemExplore = true;
            dropItemButtonList[dropItemListIndex].animator.SetTrigger("Normal");

            allItemButtonList[allItemListIndex].animator.SetTrigger("Highlighted");
        }

        // 드랍 아이템 창 선택
        else if (Input.GetKeyUp(KeyCode.RightArrow) && allItemExplore == true && dropItemButtonList.Count != 0)
        {
            allItemExplore = false;
            allItemButtonList[allItemListIndex].animator.SetTrigger("Normal");

            dropItemButtonList[dropItemListIndex].animator.SetTrigger("Highlighted");
        }

        // 현재 아이템 버튼에서 위에 있는 아이템 버튼 선택 혹은 메시지 선택에서 위쪽 메시지 버튼 선택
        else if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            // 인벤토리 창일 경우
            if (allItemExplore == true && dumpItem == false && exitInventoryCheck == false)
            {
                // 현재 아이템 버튼 하이라이트 효과 종료
                allItemButtonList[allItemListIndex].animator.SetTrigger("Normal");

                // 버튼 이동
                allItemListIndex--;

                // 버튼 이동 인덱스 최소값 설정
                if (allItemListIndex < 0)
                    allItemListIndex = 0;

                // 수직 스크롤바 값 변경(현재 버튼이 위로 올라갈 때 값 변경으로 현재 버튼을 기준으로 스크롤 필드를 보여줌)
                InventoryOverWindows[0].gameObject.GetComponent<ScrollRect>().verticalScrollbar.value += (float)1 / (float)allItemButtonList.Count;

                // 이동 후 버튼 하이라이트 효과 활성화
                allItemButtonList[allItemListIndex].animator.SetTrigger("Highlighted");
            }

            // 드랍 아이템 창일 경우(위와 상동)
            else if (allItemExplore == false && addDropItem == false && exitInventoryCheck == false)
            {
                dropItemButtonList[dropItemListIndex].animator.SetTrigger("Normal");

                dropItemListIndex--;

                if (dropItemListIndex < 0)
                    dropItemListIndex = 0;

                dropItemButtonList[dropItemListIndex].animator.SetTrigger("Highlighted");
            }

            // 메시지 창이 활성화 돼있는 경우
            else if (exitInventoryCheck == true || dumpItem == true || addDropItem == true)
            {
                messageButton[messageButtonIndex].animator.SetTrigger("Normal");
                messageButtonIndex--;

                if (messageButtonIndex < 0)
                    messageButtonIndex = 0;

                messageButton[messageButtonIndex].animator.SetTrigger("Highlighted");
            }
        }

        // 현재 아이템 버튼에서 아래에 있는 아이템 버튼 선택 혹은 메시지 선택에서 아래쪽 메시지 버튼 선택
        else if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            if (allItemExplore == true && dumpItem == false && exitInventoryCheck == false)
            {
                allItemButtonList[allItemListIndex].animator.SetTrigger("Normal");

                allItemListIndex++;

                if (allItemListIndex >= allItemButtonList.Count)
                    allItemListIndex = allItemButtonList.Count - 1;

                InventoryOverWindows[0].gameObject.GetComponent<ScrollRect>().verticalScrollbar.value -= (float)1 / (float)allItemButtonList.Count;

                allItemButtonList[allItemListIndex].animator.SetTrigger("Highlighted");
            }

            else if (allItemExplore == false && addDropItem == false && exitInventoryCheck == false)
            {
                dropItemButtonList[dropItemListIndex].animator.SetTrigger("Normal");

                dropItemListIndex++;

                if (dropItemListIndex >= dropItemButtonList.Count)
                    dropItemListIndex = dropItemButtonList.Count - 1;

                dropItemButtonList[dropItemListIndex].animator.SetTrigger("Highlighted");
            }

            else if (exitInventoryCheck == true || dumpItem == true || addDropItem == true)
            {
                messageButton[messageButtonIndex].animator.SetTrigger("Normal");
                messageButtonIndex++;

                if (messageButtonIndex >= messageButton.Length)
                    messageButtonIndex = messageButton.Length - 1;

                messageButton[messageButtonIndex].animator.SetTrigger("Highlighted");
            }
        }

        // 인벤토리 창, 드랍 아이템 창, 메시지 창 활성화 일 경우 버튼 선택
        else if (Input.GetKeyUp(KeyCode.A))
        {
            // 인벤토리 창일 경우
            if (allItemExplore == true && dumpItem == false && exitInventoryCheck == false)
            {
                // 아이템 버리기 활성화용 bool 변수
                dumpItem = true;

                // 메시지 창 활성화
                OpenMessageWindow();

                messageText.text = "버리시겠습니까?";
            }

            // 드랍 아이템 창일 경우
            else if (allItemExplore == false && addDropItem == false && exitInventoryCheck == false)
            {
                // 아이템 추가 활성화용 bool 변수
                addDropItem = true;

                OpenMessageWindow();

                messageText.text = "인벤토리에 추가";

            }

            // 아이템 버리기 활성화
            else if (allItemExplore == true && dumpItem == true && exitInventoryCheck == false)
            {
                // 메시지 창에서 yes선택시
                if (messageButtonIndex == 0)
                {
                    // 아이템 버튼 하위 오브젝트 중 첫번째 텍스트에 저장된 아이템 넘버를 가져옴
                    itemNumberToText = int.Parse(allItemButtonList[allItemListIndex].transform.GetChild(0).GetComponent<Text>().text);

                    // 아이템 버튼 리스트 가장 마지막 버튼을 선택하고 드롭 아이템 창에 버튼이 1개 이상 있을 경우
                    if (allItemListIndex == allItemButtonList.Count - 1 && allItemButtonList.Count != 1 && dropItemButtonList.Count > 0)
                    {
                        // 아이템 버리기 함수
                        DumpedInventoryItem(itemNumberToText, allItemListIndex);

                        // 인벤토리 창 탐색 인덱스를 하나 감소
                        allItemListIndex--;

                        // 아이템 버튼 하이라이트 활성화
                        allItemButtonList[allItemListIndex].animator.SetTrigger("Highlighted");

                        // 메시지 창 종료
                        QuitMessageWindow();
                    }

                    // 아이템 버튼 개수가 1개이고 드롭 아이템 창에 버튼이 1개 이상 있을 경우
                    else if (allItemButtonList.Count == 1 && dropItemButtonList.Count > 0)
                    {
                        DumpedInventoryItem(itemNumberToText, allItemListIndex);

                        // 인벤토리 창 탐색 인덱스를 0으로
                        allItemListIndex = 0;

                        QuitMessageWindow();

                        // 인벤토리 창에 아이템 버튼이 없으므로 드롭 아이템 창의 아이템 버튼에 하이라이트 효과
                        dropItemButtonList[dropItemListIndex].animator.SetTrigger("Highlighted");

                        // 드롭 아이템 창을 활성화
                        allItemExplore = false;
                    }

                    // 아이템 버튼 수가 1 이상이고 드롭 아이템 창에 버튼이 없을 경우 버리기 불가로 처리
                    else if (allItemButtonList.Count >= 1 && dropItemButtonList.Count == 0)
                    {
                        messageText.text = "버리기 불가";
                        return;
                    }

                    // 그 외의 전체 상황에서 아이템 버리기 처리
                    else
                    {
                        DumpedInventoryItem(itemNumberToText, allItemListIndex);

                        QuitMessageWindow();

                        allItemList = PartyManager._instance.GetInventoryItemList();

                        allItemButtonList[allItemListIndex].animator.SetTrigger("Highlighted");
                    }

                    // 아이템을 버린 후 현재 인벤토리에 아이템 개수 표시
                    SetTempInventoryCount(allItemList, tempInventoryText);
                }

                // 메시지 창에서 no 선택시
                if (messageButtonIndex == 1)
                {
                    QuitMessageWindow();

                    dumpItem = false;
                }
            }

            // 인벤토리에 아이템 추가 활성화
            else if (allItemExplore == false && addDropItem == true && exitInventoryCheck == false)
            {
                // 메시지 창에서 yes 선택, 인벤토리 내부 아이템 개수가 60 미만일 때
                if (messageButtonIndex == 0 && allItemList.Count < 60)
                {
                    // 드롭 아이템 버튼 하위 텍스트에 저장된 아이템 넘버를 가져옴
                    itemNumberToText = int.Parse(dropItemButtonList[dropItemListIndex].transform.GetChild(0).GetComponent<Text>().text);

                    // 드롭 아이템 버튼 리스트에서 가장 마지막 버튼 선택, 드롭 아이템 버튼 개수가 1이 아닐 때
                    if (dropItemListIndex == dropItemButtonList.Count - 1 && dropItemButtonList.Count != 1)
                    {
                        // 아이템 추가 함수 실행
                        AddDropItemsToInventory(itemNumberToText, dropItemListIndex);

                        // 드롭 아이템 창 탐색 인덱스 1 감소
                        dropItemListIndex--;

                        // 감소 후 버튼 하이라이트 효과 활성화
                        dropItemButtonList[dropItemListIndex].animator.SetTrigger("Highlighted");

                        // 메시지 창 종료
                        QuitMessageWindow();

                        // 인벤토리 창에 드롭 아이템 버튼 추가
                        InstantiateDropItemButton(itemNumberToText, InventoryOverWindows[0]);
                    }

                    // 드롭 아이템 버튼 개수가 1일때
                    else if (dropItemButtonList.Count == 1)
                    {
                        AddDropItemsToInventory(itemNumberToText, dropItemListIndex);

                        QuitMessageWindow();

                        InstantiateDropItemButton(itemNumberToText, InventoryOverWindows[0]);

                        // 드롭 아이템 창에 버튼이 없으므로 인벤토리 창 아이템 버튼 활성화
                        allItemButtonList[allItemListIndex].animator.SetTrigger("Highlighted");

                        // 인벤토리 창 활성화
                        allItemExplore = true;
                    }

                    // 그 외의 모든 상황 처리
                    else
                    {
                        AddDropItemsToInventory(itemNumberToText, dropItemListIndex);

                        dropItemButtonList[dropItemListIndex].animator.SetTrigger("Highlighted");

                        QuitMessageWindow();

                        InstantiateDropItemButton(itemNumberToText, InventoryOverWindows[0]);
                    }
                }

                // 메시지 창에서 yes를 선택했지만 인벤토리 공간 부족 시
                else if (messageButtonIndex == 0 && allItemList.Count >= 60)
                {
                    messageText.text = "인벤토리 공간 부족";
                }

                // 메시자 창에서 no 선택시 
                else if (messageButtonIndex == 1)
                {
                    addDropItem = false;

                    QuitMessageWindow();
                }

                // 아이템이 추가된 인벤토리 창의 아이템 개수 표시
                SetTempInventoryCount(allItemList, tempInventoryText);
            }

            // 인벤토리 정리UI 종료를 활성화 시
            else if (exitInventoryCheck == true)
            {
                // 메시지 창에서 yes를 선택 시
                if (messageButtonIndex == 0)
                {
                    // 미궁 씬으로 돌아감
                    if (SceneChanger.instance != null) SceneChanger.instance.SceneChange();
                    SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("Battle " + SceneChanger.instance.floor));
                    SceneChanger.instance.spawnCase = (SpawnCase)0;
                }

                // 메시지 창에서 no를 선택 시 종료 선택 비활성화
                else if (messageButtonIndex == 1)
                {
                    exitInventoryCheck = false;

                    QuitMessageWindow();
                }
            }
        }

        else if (Input.GetKeyUp(KeyCode.B))
        {
            // 인벤토리 정리UI 종료 선택창 활성화
            if (exitInventoryCheck == false && addDropItem == false && dumpItem == false)
            {
                exitInventoryCheck = true;

                OpenMessageWindow();

                messageText.text = "종료하시겠습니까?";
            }

            // 인벤토리 정리UI 종료 선택창 비활성화
            else if (exitInventoryCheck == true)
            {
                exitInventoryCheck = false;

                QuitMessageWindow();
            }

            // 인벤토리에 아이템 추가 활성화 상태일시 상태 끄기
            else if (addDropItem == true)
            {
                addDropItem = false;

                QuitMessageWindow();
            }

            //  아이템 버리기 활성화 상태일시 상태 끄기
            else if (dumpItem == true)
            {
                dumpItem = false;

                QuitMessageWindow();
            }
        }
    }

    // 메시지 창 활성화 함수
    public void OpenMessageWindow()
    {
        // 메시지 창 활성화
        messageWIndow.SetActive(true);

        // 메시지 버튼 인덱스 초기화
        messageButtonIndex = 0;
        // 메시지 버튼 하이라이트 효과 활성화
        messageButton[messageButtonIndex].animator.SetTrigger("Highlighted");
    }

    // 메시지 창 비활성화 함수
    public void QuitMessageWindow()
    {
        // 메시지 버튼 하이라이트 효과 비활성화
        messageButton[messageButtonIndex].animator.SetTrigger("Normal");
        // 메시지 버튼 인덱스 초기화
        messageButtonIndex = 0;
        
        // 메시지 창 비활성화
        messageWIndow.SetActive(false);
    }

    // 아이템 버리기 함수(아이템 넘버, 인벤토리 창의 탐색 인덱스)
    public void DumpedInventoryItem(int num, int index)
    {
        // 파티매니저의 아이템 버리기 함수
        PartyManager._instance.DumpedItem(num);

        // 아이템 버튼 제거
        Destroy(allItemButtonList[index].gameObject);
        // 아이템 버튼 리스트에서 인덱스의 요소 제거
        allItemButtonList.RemoveAt(index);

        // 아이템 버리기 상태 해제
        dumpItem = false;
    }

    // 인벤토리에 아이템 추가 함수(아이템 넘버, 드롭 아이템 창의 탐색 인덱스)
    public void AddDropItemsToInventory(int num, int index)
    {
        // 파티매니저의 아이템 추가 함수
        PartyManager._instance.SetInventoryItem(num);

        // 아이템 버튼 제거
        Destroy(dropItemButtonList[index].gameObject);
        // 드롭 아이템 버튼 리스트에서 인덱스의 요소 제거
        dropItemButtonList.RemoveAt(index);

        // 아이템 추가 상태 해제
        addDropItem = false;
    }

    // 현재 인벤토리 아이템 개수 표시 함수(인벤토리 리스트, 표시용 텍스트)
    public void SetTempInventoryCount(List<int> itemList, Text countText)
    {
        // 아이템 리스트 갱신
        itemList = PartyManager._instance.GetInventoryItemList();
        // 텍스트 값 설정
        countText.text = string.Format("{0} / {1}", allItemList.Count, maxInventory.ToString());
    }

    // 인벤토리 창 아이템 버튼 생성(스크롤 뷰, 아이템 리스트)
    public void InstantiateInventoryButton(IdentifyScript allItemTable, List<int> allItemList)
    {
        // 아이템 버튼이 생성됐는지 판별 / 아이템 버튼이 생성 됐으면 함수 실행 패스
        if (allItemButtonExist == true)
            return;

        ScrollRect scrollRect = allItemTable.gameObject.GetComponent<ScrollRect>();
        GameObject itemTemp;

        foreach (var itemNumber in allItemList)
        {
            Debug.Log(itemNumber);

            // 아이템 넘버가 무기 아이템 넘버일 경우
            if (itemNumber > 10000 && itemNumber < 20000)
            {
                WeaponRecord record = TableMgr.Instance.weaponTable.GetRecord(itemNumber);
                itemTemp = Instantiate(overInventoryItemPrefab, scrollRect.content);
                itemTemp.transform.GetChild(0).GetComponent<Text>().text = string.Format("{0}", itemNumber);
                itemTemp.transform.GetChild(1).GetComponent<Text>().text = record.wStrName;
                allItemButtonList.Add(itemTemp.GetComponent<Button>());
            }

            // 아이템 넘버가 방어구 아이템 넘버일 경우
            else if (itemNumber > 20000 && itemNumber < 30000)
            {
                ArmorRecord record = TableMgr.Instance.armorTable.GetRecord(itemNumber);
                itemTemp = Instantiate(overInventoryItemPrefab, scrollRect.content);
                itemTemp.transform.GetChild(0).GetComponent<Text>().text = string.Format("{0}", itemNumber);
                itemTemp.transform.GetChild(1).GetComponent<Text>().text = record.aStrName;
                allItemButtonList.Add(itemTemp.GetComponent<Button>());
            }

            // 아이템 넘버가 악세서리 아이템 넘버일 경우
            else if (itemNumber > 30000 && itemNumber < 40000)
            {
                AccessoryRecord record = TableMgr.Instance.accessoryTable.GetRecord(itemNumber);
                itemTemp = Instantiate(overInventoryItemPrefab, scrollRect.content);
                itemTemp.transform.GetChild(0).GetComponent<Text>().text = string.Format("{0}", itemNumber);
                itemTemp.transform.GetChild(1).GetComponent<Text>().text = record.acStrName;
                allItemButtonList.Add(itemTemp.GetComponent<Button>());
            }

            // 아이템 넘버가 소모용 아이템 넘버일 경우
            else if (40000 < itemNumber && itemNumber < 50000)
            {
                ConItemRecord record = TableMgr.Instance.conItemTable.GetRecord(itemNumber);
                itemTemp = Instantiate(overInventoryItemPrefab, scrollRect.content);
                itemTemp.transform.GetChild(0).GetComponent<Text>().text = string.Format("{0}", itemNumber);
                itemTemp.transform.GetChild(1).GetComponent<Text>().text = record.cStrName;
                allItemButtonList.Add(itemTemp.GetComponent<Button>());
            }

            // 아이템 넘버가 드롭 아이템 넘버일 경우
            else if (50000 < itemNumber && itemNumber < 60000)
            {
                DropItemRecord record = TableMgr.Instance.dropItemTable.GetRecord(itemNumber);
                itemTemp = Instantiate(overInventoryItemPrefab, scrollRect.content);
                itemTemp.transform.GetChild(0).GetComponent<Text>().text = string.Format("{0}", itemNumber);
                itemTemp.transform.GetChild(1).GetComponent<Text>().text = record.dropItem;
                allItemButtonList.Add(itemTemp.GetComponent<Button>());
            }
        }

        // 아이템 버튼이 더 생성되지 않게 함
        allItemButtonExist = true;
    }

    // 드롭 아이템 창 버튼 생성(스크롤 뷰, 드롭 아이템 리스트)
    public void InstantiateDropItemButton(IdentifyScript dropItemTable, List<DropItemRecord> dropItemList)
    {
        // 드롭 아이템 버튼이 생성됐는지 판별
        if (dropItemButtonExist == true)
            return;

        ScrollRect scrollRect = dropItemTable.gameObject.GetComponent<ScrollRect>();
        GameObject itemTemp;

        for (int i = 0; i < dropItemList.Count; i++)
        {
            DropItemRecord record = TableMgr.Instance.dropItemTable.GetRecord(dropItemList[i].nID);
            itemTemp = Instantiate(overInventoryItemPrefab, scrollRect.content);
            itemTemp.transform.GetChild(0).GetComponent<Text>().text = string.Format("{0}", dropItemList[i].nID);
            itemTemp.transform.GetChild(1).GetComponent<Text>().text = record.dropItem;
            dropItemButtonList.Add(itemTemp.GetComponent<Button>());
        }

        // 드롭 아이템 버튼이 더 생성되지 않게 함
        dropItemButtonExist = true;
    }

    // 드롭 아이템에서 인벤토리로 옮길 때 버튼 생성(드롭 아이템 탐색 인덱스, 스크롤 뷰)
    public void InstantiateDropItemButton(int num, IdentifyScript dropItemTable)
    {
        GameObject itemTemp;
        DropItemRecord record = TableMgr.Instance.dropItemTable.GetRecord(num);
        ScrollRect scrollRect = dropItemTable.gameObject.GetComponent<ScrollRect>();

        itemTemp = Instantiate(overInventoryItemPrefab, scrollRect.content);
        itemTemp.transform.GetChild(0).GetComponent<Text>().text = string.Format("{0}", num);
        itemTemp.transform.GetChild(1).GetComponent<Text>().text = record.dropItem;
        allItemButtonList.Add(itemTemp.GetComponent<Button>());
    }

    IEnumerator CheckInventoryDelay()
    {
        yield return new WaitForSeconds(5.0f);
        checkInventoryOver = false;
    }

    // 턴 종료시 Monster배열 정렬 코루틴
    IEnumerator MonsterArrayRealignment()
    {
        tempMonsterArray.InsertRange(0, spawnManager.spawnMonsters);

        for (int i = 0; i < tempMonsterArray.Count; i++)
        {
            if (tempMonsterArray.Contains(null))
            {
                tempMonsterArray.RemoveAll(x => x == null);
            }
        }

        spawnManager.spawnMonsters = new GameObject[spawnManager.spawnCount];

        spawnManager.spawnMonsters = tempMonsterArray.ToArray();
        tempMonsterArray.Clear();

        yield return null;
    }
}
