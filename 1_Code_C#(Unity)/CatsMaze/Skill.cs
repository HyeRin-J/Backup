using System.Collections.Generic;

[System.Serializable]
public class Skill
{
    public string ID;
    public string Name;
    public string Description;
    public int Factor1;
    public int Factor1_target;
    public int Factor1_target_number;
    public float Factor1_delay;
    public float Factor1_value;
    public int Factor1_unit;
    public int Factor1_type;
    public float Factor1_Duration;
    public bool Factor1_Critical;
    public float Factor1_effect;
    public int Factor2_condition;
    public int Factor2_condition_target;
    public float Factor2_condition_value;
    public int Factor2_condition_unit;
    public int Factor2_condition_validity;
    public int Factor2;
    public int Factor2_target;
    public int Factor2_target_number;
    public float Factor2_delay;
    public float Factor2_value;
    public int Factor2_unit;
    public int Factor2_type;
    public float Factor2_Duration;
    public bool Factor2_Critical;
    public float Factor2_effect;
    public int Factor3_condition;
    public int Factor3_condition_target;
    public float Factor3_condition_value;
    public int Factor3_condition_unit;
    public int Factor3_condition_validity;
    public int Factor3;
    public int Factor3_target;
    public int Factor3_target_number;
    public float Factor3_delay;
    public float Factor3_value;
    public int Factor3_unit;
    public int Factor3_type;
    public float Factor3_Duration;
    public bool Factor3_Critical;
    public float Factor3_effect;
    public float Channering;
}

[System.Serializable]
public class SkillDataClass
{
    public List<Skill> SkillData;
}