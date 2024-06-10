#pragma once
#include "Unit.h"
class Hero :
	public Unit
{

public:
	PANISPRITE m_IdleSprite;
	PANISPRITE m_AttackSprite;
	PANISPRITE m_NormalDamageSprite;
	PANISPRITE m_BossDamageSprite;
	PANISPRITE m_JumpingSprite;

	bool m_IsBossAttack = false;
	bool m_IsAttacking = false;
	bool m_IsFinish = false;

	Hero();

	bool Init();
	bool BossHit();

	bool SetSprites(ANISPRITE idleSpr, ANISPRITE attackSpr, ANISPRITE hitSprite, ANISPRITE bossHitSprite, ANISPRITE jumpingSprite);
	
	bool NormalHitAnimation(float dTime);
	bool BossHitAnimation(float dTime);
	bool AttackAnimation(float dTime);
	bool GameClearAnimation(float dTime);

	virtual bool Hit();
	virtual bool Draw(float dTime, float alpha = 1.0f);
	virtual bool Move(float dTime);
	virtual void Release();
};

