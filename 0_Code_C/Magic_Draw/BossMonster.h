#pragma once
#include "Monster.h"

#define	IS_INIT_MOVE_FINISH		0x00000100
#define	IS_MOVING				0x00001000
#define IS_EYEOPENFINISH		0x00002000
#define	IS_ATTACKING			0x00010000
#define	IS_MISS					0x00100000
#define	IS_SUMMONING			0x01000000
#define	IS_SUMMON_FINISH		0x10000000

class BossMonster :
	public Monster
{
	int m_TriPattern[5][3] = { {0, 3, 4}, {1, 2, 5}, {0, 1, 2}, {3, 4, 5}, {2, 3, 4} };
	int m_RectPattern[5][4] = { {0, 1, 2, 4}, {1, 2, 3, 4}, {0, 2, 3, 5}, {2, 3, 4, 5}, {0, 1, 5, 4} };

	int m_Pattern[6] = { 3, 2, 2, 3, 2, 3 };
	int m_CurrPatternCount = 0;
	int m_CurrPatternEyeCount = 0;

	bool m_DeadFinish = false;
public:
	PANISPRITE m_HitSprite;
	PANISPRITE m_SummonSprite;
	PANISPRITE m_AttackSprite;
	PANISPRITE m_EyesSprite[6];

	float m_PatternTime = 0;

	BossMonster();

	bool Attack();
	bool AttackAnimation(float dTime);
	bool EyeOpen(float dTime);
	bool HitAnimation(float dTime);
	bool SummonAnimation(float dTime);
	bool DeadAnimation(float dTime);

	void SetEyes(ANISPRITE eye, int num);
	void SetSprites(ANISPRITE idle, ANISPRITE attack, ANISPRITE summon, ANISPRITE hit, ANISPRITE dead);

	bool Init();

	bool IsInitMoveFinish() { return m_State & IS_INIT_MOVE_FINISH; }
	bool IsAttacking() { return m_State & IS_ATTACKING; }
	bool IsSummoning() { return m_State & IS_SUMMONING; }
	bool IsSummonFinish() { return m_State & IS_SUMMON_FINISH; }
	bool IsMoving() { return m_State & IS_MOVING; }
	bool IsMiss() { return m_State & IS_MISS; }
	bool IsEyeOpenFinish() { return m_State & IS_EYEOPENFINISH; }

	void ChangePattern();

	virtual bool Init(int hp, POSITION pos, float velo, Vector2 dir);
	virtual bool Hit(int pattern);         //맞았는가
	virtual bool Draw(float dTime, float alpha = 1.0f);		//유닛 그리기
	virtual bool Move(float dTime);	    //유닛 움직이기

	virtual void Release();
};
