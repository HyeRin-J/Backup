using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum UnitType
{
    재담,
    괴담,
    치유,
    서사,
    활극,
    기담,
}

[System.Serializable]
public class UnitData
{
    public string ID;
    public string Korean_name;
    public Imagery Imagery;
    public UnitType Type;
    public int Erg;
    public int Att;
    public int Def;
    public int HP;
    public int AggroCount;
    public float CritChance;
    public float AttSpeed;
    public int MaxTakeALook;
    public float AttRange;
    public int Maxtarget;
    public string SkillID;
    public string synergy1;
    public string synergy2;
    public string synergy3;
    public string Description;
}

public class UnitDataClass
{
    public List<UnitData> UnitData;
}

public enum UnitState
{
    Init, Idle, Search, AttackReady, Attack, Dead
}

public class Unit : MonoBehaviour
{
    public UnitState unitState;

    public UnitData unitData;
    public Skill skill;
    public Animator animator;
    public bool isDead;
    public bool isTargetFull;
    public Room room;

    public float currentHP;

    public Monster[] defTarget;
    public Monster[] attTarget;
    public float attackTimer;

    public GameObject hpBar;

    public Vector3Int location;

    public bool[] isReadyToAttack;
    public bool isMove;

    public AStar aStar;

    Coroutine baseState;
    Coroutine moveCoroutine;

    private void Start()
    {
        unitState = UnitState.Init;

        baseState = StartCoroutine(BaseCoroutine());
    }

    public void Init()
    {
        animator = GetComponent<Animator>();
        aStar = GameObject.Find("AStar").GetComponent<AStar>();

        skill = new Skill();
        skill = DataManager.Instance.GetSkillData(unitData.SkillID);

        currentHP = unitData.HP;
        defTarget = new Monster[unitData.AggroCount];
        isDead = false;
        isTargetFull = false;

        isReadyToAttack = new bool[unitData.Maxtarget];

        animator.SetBool("Dead", false);

        unitState = UnitState.Idle;
    }

    IEnumerator BaseCoroutine()
    {
        float checkTimer = Random.Range(5.0f, 15.0f);
        float randomIdleTimer = 0.0f;
        float attackTimer = 0.0f;
        float deadTimer = 0.0f;

        while (true)
        {
            switch (unitState)
            {
                case UnitState.Init:
                    Init();
                    break;
                case UnitState.Idle:
                    randomIdleTimer += Time.deltaTime;

                    for (int i = 0; i < unitData.Maxtarget; i++)
                    {
                        isReadyToAttack[i] = false;
                    }

                    if (randomIdleTimer >= checkTimer)
                    {
                        randomIdleTimer = 0.0f;
                        checkTimer = Random.Range(5.0f, 15.0f);

                        Idle();
                    }

                    break;
                case UnitState.Search:
                    if (isMove)
                    {
                        randomIdleTimer = 0.0f;
                        isMove = false;
                        animator.SetBool("Move", false);
                    }

                    Search();

                    break;
                case UnitState.AttackReady:
                    AttackReady();
                    break;
                case UnitState.Attack:
                    attackTimer += Time.deltaTime;

                    if (attackTimer >= unitData.AttSpeed)
                    {
                        attackTimer = 0;
                        animator.SetTrigger("Attack");
                    }

                    break;
                case UnitState.Dead:
                    deadTimer += Time.deltaTime;

                    if (deadTimer >= 1.0f)
                    {
                        deadTimer = 0;

                        Dead();
                    }

                    break;
                default:
                    break;
            }

            yield return null;
        }
    }

    void Idle()
    {
        int index = Random.Range(0, 3);

        if (index == 1) animator.SetTrigger("Idle2");
        else if (index == 2)
        {
            while (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle_Act"))
            {
                return;
            }

            Vector3 targetPos = room.tiles[Random.Range(0, room.tiles.Count)].savedTile.position + new Vector3(0.5f, 0.2f); ;

            Stack<Vector3Int> newPath = aStar.Algorithm(transform.position, targetPos);

            if (newPath != null && newPath.Count > 0)
            {
                newPath = new Stack<Vector3Int>(newPath.Reverse());
                if (moveCoroutine != null) StopCoroutine(moveCoroutine);
                moveCoroutine = StartCoroutine(MoveToTargetPos(newPath));
                isMove = true;
            }

            Vector3 dir = targetPos - transform.position;

            if (dir.x < 0)
            {
                transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, 0, transform.rotation.z));
                hpBar.transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, 0, transform.rotation.z));
                hpBar.transform.localPosition = new Vector3(-1.525f, 3.2f, 0);
            }
            else
            {
                transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, 180, transform.rotation.z));
                hpBar.transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, 0, transform.rotation.z));
                hpBar.transform.localPosition = new Vector3(1.525f, 3.2f, 0);
            }

            transform.DOMove(targetPos, 1.0f).SetSpeedBased().SetEase(Ease.Linear).OnComplete(() =>
            {
                isMove = false;
                animator.SetBool("Move", false);
            });

            animator.SetBool("Move", true);
        }
    }

    void Search()
    {
        int count = 0;

        for (int i = 0; i < unitData.AggroCount; i++)
        {
            if (defTarget[i] != null && !defTarget[i].isDead)
            {
                unitState = UnitState.AttackReady;

                if (!isMove)
                {
                    Stack<Vector3Int> newPath = aStar.Algorithm(transform.position, defTarget[i].transform.position);

                    if (newPath != null && newPath.Count > 0)
                    {
                        newPath = new Stack<Vector3Int>(newPath.Reverse());
                        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
                        moveCoroutine = StartCoroutine(MoveToTargetPos(newPath));
                        isMove = true;
                    }
                }
            }
            else
            {
                if (room.monsters.Count > 0)
                {
                    Stack<Vector3Int> newPath = aStar.Algorithm(transform.position, room.monsters[i].transform.position);

                    if (newPath != null && newPath.Count > 0)
                    {
                        newPath = new Stack<Vector3Int>(newPath.Reverse());
                        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
                        moveCoroutine = StartCoroutine(MoveToTargetPos(newPath));
                        isMove = true;

                        unitState = UnitState.AttackReady;
                    }
                }
                else count++;
            }
        }

        if (count == unitData.AggroCount) unitState = UnitState.Idle;
    }

    void AttackReady()
    {
        transform.DOPause();
        transform.DOKill(); 
        
        int count = 0;

        for (int i = 0; i < unitData.AggroCount; i++)
        {
            if (i >= unitData.Maxtarget) break;

            if (defTarget[i] != null)
            {
                if (Vector3.SqrMagnitude(defTarget[i].transform.position - transform.position) <= unitData.AttRange)
                {
                    isReadyToAttack[i] = true;
                    StopCoroutine(moveCoroutine);
                    unitState = UnitState.Attack;
                }
            }
            else
            {
                if (room.monsters.Count > 0)
                {
                    if (Vector3.SqrMagnitude(room.monsters[i].transform.position - transform.position) <= unitData.AttRange)
                    {

                        StopCoroutine(moveCoroutine);
                        unitState = UnitState.Attack;
                    }

                    else
                        count++;
                }
                else
                {
                    unitState = UnitState.Search;
                }
            }
        }

        if (count == unitData.AggroCount) unitState = UnitState.Search;
    }

    public void SetLayerOrder(int roomUnitCount)
    {
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].sortingOrder = roomUnitCount * 10 + spriteRenderers[i].sortingOrder;
        }
    }

    public void Attack()
    {
        transform.DOPause();
        transform.DOKill();

        int damage = (int)unitData.Att;

        bool isCrit = false;

        if (UnityEngine.Random.Range(0, 1.0f) <= unitData.CritChance) isCrit = true;

        for (int index = 0, count = 0; index < unitData.AggroCount && count < unitData.Maxtarget; index++)
        {
            if (defTarget[index] != null && index < isReadyToAttack.Length && isReadyToAttack[index])
            {
                count++;

                Vector3 dir = defTarget[index].transform.position - transform.position;

                if (dir.x <= 0)
                {
                    transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, 0, transform.rotation.z));
                    hpBar.transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, 0, transform.rotation.z));
                    hpBar.transform.localPosition = new Vector3(-1.525f, 3.2f, 0);
                }
                else
                {
                    transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, 180, transform.rotation.z));
                    hpBar.transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, 0, transform.rotation.z));
                    hpBar.transform.localPosition = new Vector3(1.525f, 3.2f, 0);
                }

                if (defTarget[index].Damaged(damage, isCrit))
                {

                    GameManager.Instance.scriptManager.SetCurrentCondition(Conditions.다수_적_처치);
                    AttackFinish(index);
                }
            }
        }

        unitState = UnitState.AttackReady;
    }

    public bool Damaged(int damage, bool isCrit)
    {
        float finalDamage = Mathf.RoundToInt((damage - unitData.Def) * (isCrit ? 1.3f : 1));

        if (finalDamage < 0) finalDamage = 0;

        currentHP -= finalDamage;

        if (currentHP <= 0)
        {
            hpBar.transform.DOScaleX(0, 0.2f);
            isDead = true;
            animator.SetBool("Dead", true);
            unitState = UnitState.Dead;

            GameManager.Instance.scriptManager.SetCurrentCondition(Conditions.다운);
        }
        else
        {
            hpBar.transform.DOScaleX((currentHP / unitData.HP) * 1.525f, 0.2f);
        }

        return isDead;
    }

    public int SetTarget(Monster monster)
    {
        int count = 0;
        int index = -1;

        transform.DOKill();
        isMove = false;

        for (int i = 0; i < defTarget.Length; i++)
        {
            if (defTarget[i] == monster) return i;

            if (defTarget[i] == null)
            {
                defTarget[i] = monster;

                index = i;

                break;
            }

            if (defTarget[i] != null) count++;
        }

        if (count >= unitData.AggroCount) isTargetFull = true;
        else isTargetFull = false;

        unitState = UnitState.Search;

        return index;
    }

    public void AttackFinish(int index)
    {
        defTarget[index].targetUnit = null;
        defTarget[index].isReadyToAttack = false;
        defTarget[index] = null;

        isReadyToAttack[index] = false;

        int i = 0;

        for (; i < defTarget.Length; i++)
        {
            if (defTarget[i] != null) break;
        }

        if (i == defTarget.Length) isTargetFull = false;
    }

    IEnumerator MoveToTargetPos(Stack<Vector3Int> newPath)
    {
        Vector3 dest = newPath.Pop();

        while (newPath.Count > 0)
        {
            yield return null;

            Vector3 dir = dest - transform.position;

            if (dir.x > 0)
            {
                transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, 180, transform.rotation.z));
                hpBar.transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, 0, transform.rotation.z));
                hpBar.transform.localPosition = new Vector3(1.525f, 3.2f, 0);
            }
            else
            {
                transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, 0, transform.rotation.z));
                hpBar.transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, 0, transform.rotation.z));
                hpBar.transform.localPosition = new Vector3(-1.525f, 3.2f, 0);
            }

            transform.DOKill();
            transform.DOMove(dest, 1).SetSpeedBased().SetEase(Ease.Linear).OnComplete(() =>
            {
                if (newPath.Count > 1) dest = newPath.Pop() + new Vector3(0.5f, 0);
                else newPath.Clear();
            });
        }

        if (newPath.Count == 0)
        {
            for (int i = 0; i < defTarget.Length; i++)
            {
                if (defTarget[i] != null)
                {
                    Vector3 dir = defTarget[i].transform.position - transform.position;

                    if (dir.x > 0)
                    {
                        transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, 180, transform.rotation.z));
                        hpBar.transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, 0, transform.rotation.z));
                        hpBar.transform.localPosition = new Vector3(1.525f, 3.2f, 0);
                        transform.position = new Vector3(defTarget[i].transform.position.x - 0.5f, transform.position.y, transform.position.z);
                    }
                    else
                    {
                        transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, 0, transform.rotation.z));
                        hpBar.transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, 0, transform.rotation.z));
                        hpBar.transform.localPosition = new Vector3(-1.525f, 3.2f, 0);
                        transform.position = new Vector3(defTarget[i].transform.position.x + 0.5f, transform.position.y, transform.position.z);
                    }
                }
            }
        }

        isMove = false;

        animator.SetBool("Move", false);

        moveCoroutine = null;
    }

    void Dead()
    {
        transform.DOPause();
        transform.DOKill();

        currentHP += (unitData.HP * 0.01f);
        hpBar.transform.DOScaleX((currentHP / unitData.HP) * 1.525f, 0.2f);

        if (currentHP >= unitData.HP)
        {
            Revive();
        }
    }

    public void Revive()
    {
        currentHP = unitData.HP;
        hpBar.transform.DOScaleX(1.95f, 0.2f);
        isDead = false;
        animator.SetBool("Dead", false);

        unitState = UnitState.Idle;
    }
}
