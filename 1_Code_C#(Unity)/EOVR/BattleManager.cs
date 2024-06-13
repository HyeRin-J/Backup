using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using QDRuntime.Internal;

public class BattleManager : MonoBehaviour
{
    //BattleStartState battleStartState = BattleStartState.Normal;
    public List<GameObject> turnProgressSequence = new List<GameObject>();
    public GameObject latelyAttackMonster;
    public GameObject[] monsterEffects;
    public GameObject[] playerEffects;
    public bool isTurnProgressing = false;
    Text statusText;
    public Text monsterDamageText;
    public Text playerDamageText;
    public BattlePlayer[] players = new BattlePlayer[5];
    BattleUIManager uIManager;

    public int deathCount = 0;

    private static BattleManager _instance;
    public static BattleManager Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        _instance = FindObjectOfType<BattleManager>();
    }

    // Use this for initialization
    void Start()
    {
        uIManager = GetComponent<BattleUIManager>();
        statusText = uIManager.statusText;

    }

    public void PassiveSkillActivation()
    {
        foreach (var player in players)
        {
            player.playerInfo.passiveSkill(player.gameObject);
        }
    }

    public void StartTurnProgressingCoroutine()
    {
        StartCoroutine(TurnProgressing());
    }

    public void StopTurnProgressingCoroutine()
    {
        StopCoroutine(TurnProgressing());
    }

    public IEnumerator TurnProgressing()
    {
        isTurnProgressing = true;

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

        yield return new WaitForSeconds(2.0f);

        //플레이어 별로 버프, 디버프 스킬 돌림
        foreach (BattlePlayer character in players)
        {
            if (character.buffSkills.Count > 0)
            {
                for (int i = 0; i < character.buffSkills.Count; i++)
                {
                    //죽었으면 스킵 후 버프 초기화
                    if (character.playerInfo.isDown)
                    {
                        character.buffSkills.Clear();
                        continue;
                    }
                    //버프 스킬 정보 가져옴
                    BattlePlayer.BuffSkillsInfo buff = character.buffSkills[i];

                    statusText.text = string.Format("{0} : {1}", character.playerInfo.characterName, buff.buffs.Name);
                    character.playerInfo.UseSkill(buff.buffs.nID, character.gameObject);  //버프 스킬 사용
                    buff.duration--;    //턴수 감소
                    character.buffSkills[i] = buff;
                    if (character.buffSkills[i].duration <= 0)
                    {
                        character.buffSkills.RemoveAt(i);  //턴수 0 이면 리스트에서 삭제
                        i--;
                    }
                    //버프 이펙트
                    character.effects[0].Play();
                    yield return new WaitForSeconds(1f);                  
                }
            }
            if (character.debuffSkills.Count > 0)
            {
                for (int i = 0; i < character.debuffSkills.Count; i++)
                {
                    //죽었으면 스킵 후 디버프 초기화
                    if (character.playerInfo.isDown)
                    {
                        character.debuffSkills.Clear();
                        continue;
                    }
                    //디버프 스킬 정보 가져옴
                    BattlePlayer.DebuffSkillsInfo debuff = character.debuffSkills[i];

                    statusText.text = string.Format("{0} : {1}", character.playerInfo.characterName, debuff.debuffs.MonSkillName);
                    character.playerInfo.UseSkill(debuff.debuffs.nID, character.gameObject);  //디버프 스킬 사용
                    debuff.duration--;  //턴수 감소
                    character.debuffSkills[i] = debuff;
                    if (character.debuffSkills[i].duration <= 0)
                    {
                        character.debuffSkills.RemoveAt(i);  //턴수 0이면 리스트에서 삭제
                        i--;
                    }
                    //디버프 이펙트
                    character.effects[1].Play();
                    yield return new WaitForSeconds(1f);
                }
            }
        }
        //몬스터의 버프, 디버프
        foreach (GameObject monster in GetComponent<SpawnManager>().spawnMonsters)
        {
            BattleMonster mon = monster.GetComponent<BattleMonster>();

            if (mon.buffSkills.Count > 0)
            {
                for (int i = 0; i < mon.buffSkills.Count; i++)
                {
                    //버프 스킬 정보 가져옴
                    BattleMonster.BuffSkillsInfo buff = mon.buffSkills[i];

                    statusText.text = string.Format("{0} : {1}", mon.monsterCharacter.monStatRecord.Name, buff.buffs.MonSkillName);
                    //mon.UseSkill(buff.buffs.nID, mon.gameObject);  //버프 스킬 사용
                    buff.duration--;    //턴수 감소
                    mon.buffSkills[i] = buff;
                    if (mon.buffSkills[i].duration <= 0)
                    {
                        mon.buffSkills.RemoveAt(i);  //턴수 0 이면 리스트에서 삭제
                        i--;
                    }
                    //버프 이펙트
                    //GameObject buffEffect = Instantiate(playerEffects[0], mon.transform.parent);
                    //Destroy(buffEffect, 1f);
                    yield return new WaitForSeconds(1f);
                }
            }
            if (mon.debuffSkills.Count > 0)
            {
                for (int i = 0; i < mon.debuffSkills.Count; i++)
                {
                    //디버프 스킬 정보 가져옴
                    BattleMonster.DebuffSkillsInfo debuff = mon.debuffSkills[i];

                    statusText.text = string.Format("{0} : {1}", mon.monsterCharacter.monStatRecord.Name, debuff.debuffs.Name);
                    //mon.UseSkill(debuff.debuffs.nID, mon.gameObject);  //디버프 스킬 사용
                    debuff.duration--;  //턴수 감소
                    mon.debuffSkills[i] = debuff;
                    if (mon.debuffSkills[i].duration <= 0)
                    {
                        mon.debuffSkills.RemoveAt(i);  //턴수 0이면 리스트에서 삭제
                        i--;
                    }
                    //디버프 이펙트
                    //GameObject debuffEffect = Instantiate(playerEffects[1], mon.transform.parent);
                    //Destroy(debuffEffect, 1f);
                    yield return new WaitForSeconds(1f);
                }
            }
        }

        //턴 진행
        //스피드 고려
        for (int i = 0; i < turnProgressSequence.Count; i++)
        {
            GameObject character = turnProgressSequence[i];

            //플레이어일 경우
            if (character.GetComponent<BattlePlayer>() != null)
            {
                BattlePlayer player = character.GetComponent<BattlePlayer>();
                if (player.playerInfo.isDown) continue; //플레이어가 사망 상태인 경우, 턴을 스킵

                //현재 턴이 진행중인 플레이어의 색깔을 변경
                player.GetComponent<Image>().color = GetComponent<BattleUIManager>().selected;

                //선택된 행동에 따라 처리할 게 달라짐
                switch (player.currentActionState)
                {
                    //일반공격
                    case CurrentActionState.Attack:
                        //공격하려고 타겟해놓은 몬스터
                        GameObject target = player.targetedMonsters[0];

                        //만약 선택했던 몬스터가 죽었으면, 남은 몬스터 중 가장 첫번째 몬스터를 자동으로 공격
                        if (target == null && GetComponent<SpawnManager>().spawnCount != 0)
                            target = GetComponent<SpawnManager>().spawnMonsters[0];

                        if (target == null) yield break;

                        //타겟의 Monster 스크립트 가져옴
                        BattleMonster monster = target.GetComponent<BattleMonster>();

                        //현재 상태를 표시
                        statusText.text = string.Format("{0}은(는) {1}을 공격했다!", player.playerInfo.characterName, monster.monsterCharacter.monStatRecord.Name);

                        //몬스터의 포지션에 공격 이펙트 생성
                        GameObject effect = Instantiate(monsterEffects[0], target.transform.parent);
                        Destroy(effect, 1.0f);

                        #region 데미지 계산식
                        float playerAttack = (float)player.playerInfo.Attack(); //플레이어의 물공
                        float monsterDef = (float)monster.monsterCharacter.monStatRecord.Pdef;     //몬스터의 물방

                       /* A
                        * ATK < DEF × 3 일 때, 0.95
                        * ATK = DEF × 3 일 때, 1.0
                        * ATK> DEF × 3 일 때, ATK ÷ (DEF × 3)로 서서히 상승 (이게 힘드시면 1.05로 해주시면 됩니다.)
                        */

                        float inc = playerAttack < (monsterDef * 3) ? 0.95f : (playerAttack == (monsterDef * 3) ? 1.0f : playerAttack / (monsterDef * 3));
                        // (공격하는 캐릭터의ATK - 공격받는 몬스터의 DEF) X A
                        int damage = (int)((playerAttack - monsterDef) * inc);

                        //최소 데미지 1
                        damage = damage <= 0 ? 1 : damage;
                        monster.monCurHp -= damage;
                        #endregion

                        //몬스터 데미지 TextUI에 데미지를 출력
                        monsterDamageText = target.transform.parent.GetChild(0).GetChild(1).GetComponent<Text>();
                        monsterDamageText.text = string.Format("{0}", damage);

                        monsterDamageText.gameObject.SetActive(true);


                        #region 몬스터 hp에 따른 판단
                        if (monster.monCurHp <= 0) //0이하, 죽음
                        {
                            MessagingSystem.Instance.QueueMesssage(new DieMessage(target));
                            i--;
                            yield return new WaitForSeconds(1.0f);
                            monsterDamageText.gameObject.SetActive(false);
                            GetComponent<SpawnManager>().DestoryMonster(monster.gameObject);
                        }
                        else if ((float)monster.monCurHp / (float)monster.monCurHp <= 0.25f)  //25% 이하, 빈사
                        {
                            monster.Coma();
                            target.GetComponent<Animator>().SetTrigger("Damage");
                        }
                        else    //나머지
                        {
                            target.GetComponent<Animator>().SetTrigger("Damage");
                        }
                        #endregion

                        yield return new WaitForSeconds(0.5f);  //데미지 출력시간

                        monsterDamageText.gameObject.SetActive(false);
                        break;
                    //스킬
                    case CurrentActionState.Skill:
                        statusText.text = string.Format("{0}은(는) {1}을 사용했다!", player.playerInfo.characterName, player.selectedSkill.Name);

                        //스킬별로 처리 다르게
                        if (player.selectedSkill.Allies == PlayerData.SkillData.ALLIES.Enemy)
                        {
                            player.TargetedMonsterHasNull();    //타겟몬스터 널 체킹

                            //targetedMonster의 길이가 0이고, spawnCount가 0보다 크면 후열에 몬스터가 남아있다는 뜻임
                            if (player.targetedMonsters.Length <= 0 && GetComponent<SpawnManager>().spawnCount > 0)
                            {
                                player.targetedMonsters = GetComponent<SpawnManager>().spawnMonsters;
                            }
                            //targetedMonster의 길이가 1인데 targetedMonsters[0]이 null이면 다음 인덱스에 있는 몬스터를 가져옴
                            else if (player.targetedMonsters.Length == 1 && player.targetedMonsters[0] == null)
                            {
                                player.targetedMonsters[0] = GetComponent<SpawnManager>().spawnMonsters[0];
                            }

                            //플레이어의 스킬 사용
                            int tempDamage = player.playerInfo.UseSkill(player.selectedSkill.nID, player.gameObject, player.targetedMonsters);
                            GameObject tempEffect = null;

                            //물리 스킬
                            if (player.selectedSkill.MatkType == PlayerData.SkillData.MATKTYPE.None)
                            {
                                //물리 어택 타입에 따라 스킬 이펙트 다르게 생성
                                switch (player.selectedSkill.PatkType)
                                {
                                    case PlayerData.SkillData.PATKTYPE.Cut:
                                        tempEffect = monsterEffects[0];
                                        break;
                                    case PlayerData.SkillData.PATKTYPE.Bash:
                                        tempEffect = monsterEffects[1];
                                        break;
                                    case PlayerData.SkillData.PATKTYPE.Melee:
                                        tempEffect = monsterEffects[2];
                                        break;
                                }
                            }
                            //마법 스킬
                            else if (player.selectedSkill.PatkType == PlayerData.SkillData.PATKTYPE.None)
                            {
                                tempEffect = monsterEffects[3];
                                ParticleSystem.MainModule main = tempEffect.GetComponent<ParticleSystem>().main;
                                ParticleSystem.MainModule main2 = tempEffect.transform.GetChild(0).GetComponent<ParticleSystem>().main;
                                ParticleSystem.MainModule main3 = tempEffect.transform.GetChild(1).GetComponent<ParticleSystem>().main;

                                //마법속성에 따라 이펙트의 색깔을 다르게 만듬
                                switch (player.selectedSkill.MatkType)
                                {
                                    case PlayerData.SkillData.MATKTYPE.Fire:
                                        main.startColor = new Color(0.7f, 0, 0);
                                        main2.startColor = new Color(1.0f, 0, 0);
                                        main3.startColor = new Color(1.0f, 0, 0);
                                        break;
                                    case PlayerData.SkillData.MATKTYPE.Ice:
                                        main.startColor = new Color(1f, 1, 1);
                                        main2.startColor = new Color(1.0f, 1, 1);
                                        main3.startColor = new Color(1.0f, 1, 1);
                                        break;
                                    case PlayerData.SkillData.MATKTYPE.Lightening:
                                        main.startColor = new Color(0.8f, 0.8f, 0);
                                        main2.startColor = new Color(0.8f, 0.8f, 0);
                                        main3.startColor = new Color(0.8f, 0.8f, 0);
                                        break;
                                }
                            }

                            //몬스터가 받는 데미지를 출력할 Text 배열
                            Text[] monsterDamageTexts = new Text[player.targetedMonsters.Length];
                            int index = 0;

                            //플레이어가 공격하려고 한 몬스터
                            foreach (GameObject mon in player.targetedMonsters)
                            {
                                GameObject temp = Instantiate(tempEffect, mon.transform.parent);    //스킬 이펙트
                                monsterDamageTexts[index] = mon.transform.parent.GetChild(0).GetChild(1).GetComponent<Text>();  //데미지 Text 지정
                                int skillDamage = mon.GetComponent<BattleMonster>().monDamageAmount;   //몬스터가 받는 데미지 가져옴
                                skillDamage = skillDamage <= 0 ? 1 : skillDamage;       //최소 데미지 1
                                monsterDamageTexts[index].text = string.Format("{0}", skillDamage);    //데미지 Text에 데미지를 넣음
                                mon.GetComponent<BattleMonster>().monCurHp -= skillDamage;   //monster 의 hp를 깎음
                                index++;
                                Destroy(temp, 1.0f);    //스킬 이펙트 1초 후에 삭제

                                #region 몬스터 hp에 따른 판단
                                if (mon.GetComponent<BattleMonster>().monCurHp <= 0) //0이하, 죽음
                                {
                                    mon.GetComponent<BattleMonster>().Dead();
                                }
                                else if ((float)mon.GetComponent<BattleMonster>().monCurHp / (float)mon.GetComponent<BattleMonster>().monMaxHp <= 0.25f)  //25% 이하, 빈사
                                {
                                    mon.GetComponent<BattleMonster>().Coma();
                                    mon.GetComponent<Animator>().SetTrigger("Damage");
                                }
                                else    //나머지
                                {
                                    mon.GetComponent<Animator>().SetTrigger("Damage");
                                }
                            }

                            yield return new WaitForSeconds(1.0f);  //죽는 모션을 기다리기 위한 시간

                            //spawnMonster 배열을 당겨줌
                            foreach (GameObject mon in player.targetedMonsters)
                            {
                                if (mon.GetComponent<BattleMonster>().monCurHp <= 0)
                                {
                                    GetComponent<SpawnManager>().DestoryMonster(mon);
                                }
                            }
                            #endregion

                            //몬스터 데미지 출력
                            for (index = 0; index < monsterDamageTexts.Length; index++)
                            {
                                monsterDamageTexts[index].gameObject.SetActive(true);

                            }

                            yield return new WaitForSeconds(0.5f);  //데미지 출력시간

                            //몬스터 데미지 출력 해제
                            for (index = 0; index < monsterDamageTexts.Length; index++)
                            {
                                monsterDamageTexts[index].gameObject.SetActive(false);
                            }
                        }
                        else
                        {
                            damage = player.playerInfo.UseSkill(player.selectedSkill.nID, player.gameObject, player.targetedPlayers);

                            //스킬 타입별로 이펙트 생성
                            for (int j = 0; j < player.targetedPlayers.Length; j++)
                            {
                                if (player.selectedSkill.DmgType == PlayerData.SkillData.DMGTYPE.Buff)
                                {
                                    GameObject buffEffect = Instantiate(playerEffects[0], player.targetedPlayers[j].transform.parent);
                                    Destroy(buffEffect, 1f);
                                }
                                else if (player.selectedSkill.DmgType == PlayerData.SkillData.DMGTYPE.Heal)
                                {
                                    GameObject buffEffect = Instantiate(playerEffects[4], player.targetedPlayers[j].transform.parent);
                                    Destroy(buffEffect, 1f);
                                }
                                else if (player.selectedSkill.DmgType == PlayerData.SkillData.DMGTYPE.Guard)
                                {
                                    GameObject buffEffect = Instantiate(playerEffects[5], player.targetedPlayers[j].transform.parent);
                                    Destroy(buffEffect, 1f);
                                }
                                else if (player.selectedSkill.DmgType == PlayerData.SkillData.DMGTYPE.Charge)
                                {
                                    GameObject buffEffect = Instantiate(playerEffects[2], player.targetedPlayers[j].transform.parent);
                                    ParticleSystem.MainModule main = buffEffect.GetComponent<ParticleSystem>().main;
                                    ParticleSystem.MainModule main2 = buffEffect.transform.GetChild(0).GetComponent<ParticleSystem>().main;
                                    //속성별로 이펙트 색깔 바꿈
                                    //색깔 놀이~
                                    switch (player.selectedSkill.MatkType)
                                    {
                                        case PlayerData.SkillData.MATKTYPE.Fire:
                                            main.startColor = new Color(0.7f, 0, 0);
                                            main2.startColor = new Color(1.0f, 0, 0);
                                            break;
                                        case PlayerData.SkillData.MATKTYPE.Ice:
                                            main.startColor = new Color(1f, 1, 1);
                                            main2.startColor = new Color(1.0f, 1, 1);
                                            break;
                                        case PlayerData.SkillData.MATKTYPE.Lightening:
                                            main.startColor = new Color(0.8f, 0.8f, 0);
                                            main2.startColor = new Color(0.8f, 0.8f, 0);
                                            break;
                                        case PlayerData.SkillData.MATKTYPE.None:
                                            break;
                                    }
                                    Destroy(buffEffect, 1f);
                                }
                            }

                            Debug.Log(player.targetedPlayers[0].GetComponent<BattlePlayer>().flowHp);
                            Debug.Log(player.targetedPlayers[0].GetComponent<BattlePlayer>().healAmount);
                        }

                        break;
                    //방어, 데미지 50% 감소
                    case CurrentActionState.Defend:
                        statusText.text = string.Format("{0}은(는) 방어하고 있다.", player.playerInfo.characterName);
                        player.playerInfo.Defense();
                        break;
                    //아이템 사용
                    case CurrentActionState.Item:
                        statusText.text = string.Format("{0}은(는) {1}을 사용했다.", player.playerInfo.characterName, player.selectedItem.cStrName);
                        player.playerInfo.UseItem(player.targetedPlayers, player.selectedItem.cID);
                        break;
                    //도망
                    case CurrentActionState.Escape:
                        statusText.text = string.Format("{0}은(는) 도망쳤다!", player.playerInfo.characterName);
                        break;
                    default:
                        break;
                }
                //턴 바뀌는 시간, 각종 애니메이션 등등을 기다리는 시간
                yield return new WaitForSeconds(2.0f);
                //선택된 플레이어의 색깔을 변경
                player.GetComponent<Image>().color = GetComponent<BattleUIManager>().nonSelected;
            }
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
        yield return new WaitForSeconds(2.0f);

        foreach (var player in players)
        {
            player.playerInfo.BuffEffectReset();
        }

        turnProgressSequence.Clear();

        isTurnProgressing = false;
        //턴종료
        GetComponent<BattleUIManager>().TurnEnd();
    }
}