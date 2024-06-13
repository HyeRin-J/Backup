using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; //인스턴스 객체

    public GameObject[] shipHPUI;     //함선의 체력을 표시할 UI
    ShipHP[] sHP = new ShipHP[10];      //각각의 배의 ShipHP 스크립트

    public bool turn1 = false;              //player1의 턴 표시
    public bool turn2 = false;              //player2의 턴 표시
    float startTimer = 0.0f;         //턴 시작 타임
    public float turnTime = 60.0f;   //지정된 1턴의 타임

    public Image timerImage;         //시계 UI
    public Text timerText;           //남은 시간 표시할 Text
    public Text logText1;            //Player1의 Log
    public Text logText2;            //Player2의 Log

    public int turnNum = 0;          //현재 진행 턴수

    public Text TurnText;            //진행 턴수 표시할 Text
    public Text[] skillText;         //남은 스킬 횟수 표시
    public Text[] attackText;        //공격을 했는가
    public Text[] moveText;          //이동을 했는가

    //함선 hp 표시할 text
    public Text[] shipHP1;
    public Text[] shipHP2;

    public GameObject selectedShip; //현재 선택된 배
    public GameObject[] ships;      //전체 배의 게임 오브젝트

    public bool isGameProgressing = false;  //게임이 진행중인가
    public bool isTurnLoading = false;      //다음 턴을 로딩 중인가
    public bool isAlreadyMoveShip = false;  //이미 배를 이동을 했는가
    public bool isAlreadyAttack = false;    //이미 공격을 했는가
    public bool isGameEnd = false;          //게임 종료

    public int skillCount = 2;              //스킬 사용 횟수
    public int player1Ships = 5;            //Player1의 함선 숫자
    public int player2Ships = 5;            //Player2의 함선 숫자
    public int bombMissileCount = 0;        //비행기가 떨군 미사일의 갯수

    private void Awake()
    {
        //인스턴스 객체 생성
        if (GameManager.instance == null)
        {
            GameManager.instance = this;
        }
    }

    // Use this for initialization
    void Start()
    {
        //턴 초기화
        turn1 = true;
        turn2 = false;

        //진행 턴 숫자
        turnNum = 1;

        //함선 HPUI 활성화
        shipHPUI[0].SetActive(false);
        shipHPUI[1].SetActive(false);

        //ShipHP 스크립트를 받아옴
        for (int i = 0; i < ships.Length; i++)
        {
            sHP[i] = ships[i].transform.GetChild(0).GetComponent<ShipHP>();
        }
    }

    //필요한 변수들 초기화
    void Initiallize()
    {
        isAlreadyMoveShip = false;
        isAlreadyAttack = false;
        skillCount = 2;
        bombMissileCount = 0;
    }

    //모든 UIText를 Update
    void UpdateUIText()
    {
        TurnText.text = "Turn : " + turnNum.ToString();
        string attackString = isAlreadyAttack ? "X" : "O";
        string moveString = isAlreadyMoveShip ? "X" : "O";

        //Player1의 턴이면
        if (turn1)
        {
            skillText[1].text = "스킬 : " + skillCount;
            attackText[1].text = "공격 : " + attackString;
            moveText[1].text = "이동 : " + moveString;

            shipHPUI[0].SetActive(true);
            for (int i = 0; i < 5; i++)
            {
                int hp = sHP[i].hp;
                if (hp <= 0) hp = 0;
                shipHP1[i].text = hp + "/" + sHP[i].maxHP;
            }

        }
        //Player2의 턴이면
        if (turn2)
        {
            skillText[0].text = "스킬 : " + skillCount;
            attackText[0].text = "공격 : " + attackString;
            moveText[0].text = "이동 : " + moveString;

            shipHPUI[1].SetActive(true);
            for (int j = 0; j < 5; j++)
            {
                int hp = sHP[j + 5].hp;
                if (hp <= 0) hp = 0;
                shipHP2[j].text = hp + "/" + sHP[j + 5].maxHP;
            }
        }
    }

    public void ChangeWave()
    {
        int random = Random.Range(0, 4);

        WaterTimeControll.waterArrow.RanNum(random);
    }

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

    //게임 종료 검출
    public void GameEnd()
    {
        if (turn1 && player2Ships == 0)
        {
            isGameProgressing = false;
            isGameEnd = true;
        }

        if (turn2 && player1Ships == 0)
        {
            isGameProgressing = false;
            isGameEnd = true;
        }

        if (isGameEnd)
        {
            UpdateLog("승리");
        }
    }

    //턴 종료 버튼
    public void TurnEndButton()
    {
        //선택된 함선이 없을 때만
        if (selectedShip == null)
        {
            if (isGameProgressing)
            {
                turnNum++;
            }
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
        //선택된 함선이 있고, 게임 진행중이 아니면
        else if (selectedShip != null && !isGameProgressing)
            UpdateLog("함선 위치가 고정되지 않았습니다!");
    }

    //턴 전환을 위한 코루틴
    //Player1의 턴일 때 호출
    IEnumerator Waiting1()
    {
        turn1 = !turn1;
        isTurnLoading = true;
        Initiallize();
        yield return new WaitForSeconds(5.0f);
        isTurnLoading = false;
        turn2 = !turn2;
        startTimer = Time.time;
    }

    //턴 전환을 위한 코루틴
    //Player2의 턴일 때 호출
    IEnumerator Waiting2()
    {
        turn2 = !turn2;
        isTurnLoading = true;
        Initiallize();
        yield return new WaitForSeconds(5.0f);
        isTurnLoading = false;
        startTimer = Time.time;
        turn1 = !turn1;
        ChangeWave();

        //게임 진행 중이 아닐 때
        if (!isGameProgressing)
        {
            ChangeWave();

            isGameProgressing = true;
            foreach (var obj in ships)
            {
                obj.transform.GetChild(0).GetComponent<DragAndRotateShip>().enabled = false;
            }
        }
    }

    //Log 업데이트
    public void UpdateLog(string log)
    {
        if (turn1)
            logText1.text += log + "\n";
        if (turn2)
            logText2.text += log + "\n";
    }
}

