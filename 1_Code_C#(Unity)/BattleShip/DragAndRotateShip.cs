using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;

public class DragAndRotateShip : MonoBehaviour {
    public bool isSelected = false;         //현재 선택이 되어 있는가?
    public Transform boundary;              //함선의 제일 앞부분
    public Vector3 originPos;               //원래 위치
    public Quaternion originRot;            //원래 회전값
    Transform parentTr;                     //피봇. 함선 맨 뒤쪽
    public bool isOut = false;              //격자를 벗어났는지
    public bool isCollision = false;        //다른 함선이랑 충돌중인지

    // Use this for initialization
    void Start () {
        parentTr = transform.parent;
        boundary = parentTr.GetChild(1);
        originPos = parentTr.position;
        originRot = parentTr.rotation;
    }

    //게임오브젝트에 마우스 클릭시 동작하는 함수임
    private void OnMouseDown()
    {
        //다른 턴에 선택되는 걸 막기 위해서 비교
        if (GameManager.instance.turn1 && CompareTag("Player1") || GameManager.instance.turn2 && CompareTag("Player2"))
        {
            if (GameManager.instance.selectedShip == null && !GameManager.instance.isGameProgressing)
            {
                isSelected = true;
                GameManager.instance.selectedShip = this.gameObject;
            }
        }
    }

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

    //다른 함선이랑 충돌 여부를 판단함
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player1") || other.CompareTag("Player2"))
        {
            //자기가 갖고 있는 바운더리까지 검출되서 그거 해제할거임
            if (!other.name.Equals("Ship_boundary"))
            {
                isCollision = true;
            }
        }
    }

    //함선이랑 충돌 X
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player1") || other.CompareTag("Player2"))
        {
            if (!other.name.Equals("Ship_boundary"))
            {
                isCollision = false;
            }
        }
    }
}
