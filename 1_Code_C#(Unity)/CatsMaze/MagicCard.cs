using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public enum CardType
{
    사용, 발동, 패시브
}

public enum CostType
{
    없음,
    에르그_소모,
    서비스_소모,
    유닛_배치,
    행동불능,
    몬스터_제거,
    고양이_구출,
    일정_시간_초과,
}

public enum ActiveEffect
{
    에르그_획득,
    서비스_획득,
    대미지,
    회복,
    부활,
    공격력,
    방어력,
    에르그_슬롯_추가,
    카드_생성,
    카드_모집,
    카드_발굴,
    이미저리_변경,
    홀드,
    이동속도,
    카드_에르그_소모량,
    카드_서비스_소모량,
    무적,
    이전_카드_소모값_회수,
}

public enum ActiveTarget
{
    없음,
    전체,
    애착영역,
}

public enum ActiveApplyTarget
{
    없음,
    고양이,
    몬스터,
    카드,
}

public enum ActiveApplyCondition
{
    없음,
    전투_중,
    특정_이미저리,
    특정_카드,
    카트시_유형,
    이미저리_조화,
    시너지_상태,
    사용한_카드_랜덤,
    사용한_카드_직전,
    직후에_사용할_카드,
}

public enum ActiveInitPosition
{
    마우스_포인터,
    적용되는_개체,
    적용되는_애착영역,
    화면_가운데,
    에르그_표시창,
    집사력_표시창,
}

public enum ActiveLoopPosition
{
    마우스_포인터,
    적용되는_개체,
    적용되는_애착영역,
    화면_가운데,
    에르그_표시창,
    집사력_표시창,
}

[System.Serializable]
public class MagicCardData
{
    public string cardID;
    public string Name_korean;
    public string Description;
    public CardType UseType;
    public CostType Costtype1;
    public float Costvalue1;
    public CostType Costtype2;
    public float Costvalue2;
    public ActiveEffect Active;
    public int Active_value;
    public int Active_amplification; // 증폭 공식: value * (1 + (amplification * ampli_value))
    public int Active_ampli_value;
    public int Active_Duration;
    public int Active_interval;
    public bool Active_return; // 0이면 제거 안함
    public ActiveTarget Active_target;
    public ActiveApplyTarget Active_apply_target;
    public ActiveApplyCondition Active_apply_condition;
    public int Active_condition_value;
    public bool Active_Exception;
    public string Active_Effect_Init;
    public ActiveInitPosition Active_Init_Position;
    public string Active_Effect_loop;
    public ActiveLoopPosition Active_Loop_Position;
}

[System.Serializable]
public class MagicCardDataClass
{
    public List<MagicCardData> magicCardData = new List<MagicCardData>();
}
