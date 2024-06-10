using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public enum CardType
{
    ���, �ߵ�, �нú�
}

public enum CostType
{
    ����,
    ������_�Ҹ�,
    ����_�Ҹ�,
    ����_��ġ,
    �ൿ�Ҵ�,
    ����_����,
    �����_����,
    ����_�ð�_�ʰ�,
}

public enum ActiveEffect
{
    ������_ȹ��,
    ����_ȹ��,
    �����,
    ȸ��,
    ��Ȱ,
    ���ݷ�,
    ����,
    ������_����_�߰�,
    ī��_����,
    ī��_����,
    ī��_�߱�,
    �̹�����_����,
    Ȧ��,
    �̵��ӵ�,
    ī��_������_�Ҹ�,
    ī��_����_�Ҹ�,
    ����,
    ����_ī��_�Ҹ�_ȸ��,
}

public enum ActiveTarget
{
    ����,
    ��ü,
    ��������,
}

public enum ActiveApplyTarget
{
    ����,
    �����,
    ����,
    ī��,
}

public enum ActiveApplyCondition
{
    ����,
    ����_��,
    Ư��_�̹�����,
    Ư��_ī��,
    īƮ��_����,
    �̹�����_��ȭ,
    �ó���_����,
    �����_ī��_����,
    �����_ī��_����,
    ���Ŀ�_�����_ī��,
}

public enum ActiveInitPosition
{
    ���콺_������,
    ����Ǵ�_��ü,
    ����Ǵ�_��������,
    ȭ��_���,
    ������_ǥ��â,
    �����_ǥ��â,
}

public enum ActiveLoopPosition
{
    ���콺_������,
    ����Ǵ�_��ü,
    ����Ǵ�_��������,
    ȭ��_���,
    ������_ǥ��â,
    �����_ǥ��â,
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
    public int Active_amplification; // ���� ����: value * (1 + (amplification * ampli_value))
    public int Active_ampli_value;
    public int Active_Duration;
    public int Active_interval;
    public bool Active_return; // 0�̸� ���� ����
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
