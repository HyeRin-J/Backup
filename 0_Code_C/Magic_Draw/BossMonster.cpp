#include "../RRYEngine/framework.h"
#include "../RRYEngine/Vector.h"
#include "BossMonster.h"
#include "System.h"
#include "UnitManager.h"
#include "SoundManager.h"

BossMonster::BossMonster()
{
	m_AttackSprite = new ANISPRITE();
	m_Spr = new SPRITE();
	m_AniSprite = new ANISPRITE();
	m_HitSprite = new ANISPRITE();
	m_SummonSprite = new ANISPRITE();
	m_DeadAniSprite = new ANISPRITE();

	for (int i = 0; i < 6; i++)
	{
		m_EyesSprite[i] = new ANISPRITE();
	}
	m_CurrPatternCount = 0;
	m_Type = m_Pattern[m_CurrPatternCount];

	m_State |= IS_EYEOPENFINISH;
}

bool BossMonster::Attack()
{
	m_State |= IS_ATTACKING;
	return false;
}

bool BossMonster::AttackAnimation(float dTime)
{
	return DrawAnimation(dTime, 12, m_Pos.x, m_Pos.y, m_AttackSprite);
}

bool BossMonster::EyeOpen(float dTime)
{
	int boolCount = 0;
	switch (m_Type)
	{
	case 2:
	{
		//눈 모양 그리기
		for (int i = 0; i < 3; i++)
			if (DrawAnimation(dTime, 1, m_Pos.x, m_Pos.y, m_EyesSprite[m_TriPattern[m_CurrPatternEyeCount][i]]))
				boolCount++;

		if (boolCount == 3) return true;
	}
	break;
	case 3:
	{
		//눈 모양 그리기
		for (int i = 0; i < 4; i++)
			if (DrawAnimation(dTime, 1, m_Pos.x, m_Pos.y, m_EyesSprite[m_RectPattern[m_CurrPatternEyeCount][i]]))
				boolCount++;

		if (boolCount == 4) return true;
	}
	break;
	}
	return false;
}

bool BossMonster::HitAnimation(float dTime)
{
	return DrawAnimation(dTime, 4, m_Pos.x, m_Pos.y, m_HitSprite);
}

bool BossMonster::SummonAnimation(float dTime)
{
	if (!IsSummonFinish() && m_SummonSprite->aniFrame >= 3)
	{
		//4종 중 1개 랜덤 생성
		//확률 보정
		int r = rand() % 100;
		int type = 0;

		if (r < 20) type = 0;
		else if (r < 55) type = 1;
		else if (r < 85) type = 2;
		else type = 3;

		SoundManager::GetInstance()->PlayOnce(SoundList::BOSS_SUMMON);
		UnitManager::GetInstance()->MakeEnemy(type, 0, GetPos().x + 150, GetPos().y + 420);
		m_State |= IS_SUMMON_FINISH;
		m_State &= ~IS_COLLISION;
	}

	return DrawAnimation(dTime, 8, m_Pos.x, m_Pos.y, m_SummonSprite);
}

bool BossMonster::DeadAnimation(float dTime)
{
	return DrawAnimation(dTime, 4, m_Pos.x, m_Pos.y, m_DeadAniSprite);
}

void BossMonster::SetEyes(ANISPRITE eye, int num)
{
	memcpy(m_EyesSprite[num], &eye, sizeof(ANISPRITE));
	m_EyesSprite[num]->loopCount = 1;
}

void BossMonster::SetSprites(ANISPRITE idle, ANISPRITE attack, ANISPRITE summon, ANISPRITE hit, ANISPRITE dead)
{
	memcpy(m_AniSprite, &idle, sizeof(ANISPRITE));
	memcpy(m_Spr, &(m_AniSprite->spr), sizeof(SPRITE));

	memcpy(m_AttackSprite, &attack, sizeof(ANISPRITE));
	m_AttackSprite->loopCount = 1;

	memcpy(m_SummonSprite, &summon, sizeof(ANISPRITE));
	m_SummonSprite->loopCount = 1;

	memcpy(m_HitSprite, &hit, sizeof(ANISPRITE));
	m_HitSprite->loopCount = 1;

	memcpy(m_DeadAniSprite, &dead, sizeof(ANISPRITE));
	m_DeadAniSprite->loopCount = 1;
}

bool BossMonster::Init()
{
	m_Hp = 8;
	m_Velo = 150;
	m_Pos = { 1400, m_Pos.y };
	m_CurrPatternCount = 0;
	m_Type = m_Pattern[m_CurrPatternCount];
	m_CurrPatternEyeCount = rand() % 5;

	m_State = 0;

	m_PatternTime = 0;
	m_DeadFinish = false;

	return true;
}

void BossMonster::ChangePattern()
{
	m_CurrPatternCount++;
	m_CurrPatternCount %= 6;
	m_Type = m_Pattern[m_CurrPatternCount];

	m_CurrPatternEyeCount = rand() % 5;
	m_PatternTime = 0;
}

bool BossMonster::Init(int hp, POSITION pos, float velo, Vector2 dir)
{
	m_Pos = { 1400, m_Pos.y };
	m_Hp = hp;
	m_Pos = pos;
	m_Velo = velo;
	m_Dir = dir;

	m_CurrPatternCount = 0;
	m_Type = m_Pattern[m_CurrPatternCount];
	m_CurrPatternEyeCount = rand() % 5;

	m_State = 0;

	return true;
}

//보스 몬스터의 경우.. 뒤로 밀려남 구현
bool BossMonster::Hit(int pattern)
{
	if (IsInitMoveFinish())
	{
		m_State |= IS_MOVING;
		m_State &= ~IS_INIT_MOVE_FINISH;

		m_Velo /= 15;
	}

	if (!(IsMoving())) return false;

	if (m_Type == pattern)
	{
		m_State |= IS_COLLISION;

		m_Hp -= 1;

		ChangePattern();
		m_State |= IS_EYEOPENFINISH;

		//만약 죽었으면,
		if (m_Hp <= 0)
		{
			m_State |= IS_DEAD;
			SoundManager::GetInstance()->PlayOnce(SoundList::BOSS_DIE);
			SoundManager::GetInstance()->PlayBackGround();

			SetBBox(m_BBox);
			return true;
		}
	}
	else
	{
		m_State |= IS_MISS;
		return false;
	}

	return false;
}

bool BossMonster::Draw(float dTime, float alpha)
{
	if (IsDead())
	{
		if (!m_DeadFinish && DeadAnimation(dTime))
		{
			m_DeadFinish = true;
			DrawAlphaSprite(backBufferDC, m_Pos.x, m_Pos.y, m_Spr->Width, m_Spr->Height,
				&m_DeadAniSprite->spr, m_Spr->Width * 8, 0);
		}

		if (m_DeadFinish)
		{
			DrawAlphaSprite(backBufferDC, m_Pos.x, m_Pos.y, m_Spr->Width, m_Spr->Height,
				&m_DeadAniSprite->spr, m_Spr->Width * 8, 0);
			
			return false;
		}
	}
	else if (IsCollision())
	{
		if (HitAnimation(dTime))
		{
			m_State |= IS_SUMMONING;
			m_State &= ~IS_COLLISION;
		}
	}
	else if (IsSummoning())
	{
		if (SummonAnimation(dTime))
		{
			m_State &= ~(IS_SUMMONING | IS_SUMMON_FINISH);
		}
	}
	//타입별로 그려줄 스프라이트나 애니메이션이 다름
	else if (IsAttacking())
	{
		if (AttackAnimation(dTime))
		{
			m_State &= ~(IS_ATTACKING | IS_MISS);
		}
	}
	else
	{
		DrawAnimation(dTime, 4, m_Pos.x, m_Pos.y, m_AniSprite);
	}

	if (!IsDead())
	{
		if (IsEyeOpenFinish())
		{
			switch (m_Type)
			{
			case 2:
			{
				//눈 모양 그리기
				for (int i = 0; i < 3; i++)
					DrawAlphaSprite(backBufferDC, m_Pos.x, m_Pos.y, m_Spr->Width, m_Spr->Height,
						&m_EyesSprite[m_TriPattern[m_CurrPatternEyeCount][i]]->spr, m_Spr->Width, 0);
			}
			break;
			case 3:
			{
				//눈 모양 그리기
				for (int i = 0; i < 4; i++)
					DrawAlphaSprite(backBufferDC, m_Pos.x, m_Pos.y, m_Spr->Width, m_Spr->Height,
						&m_EyesSprite[m_RectPattern[m_CurrPatternEyeCount][i]]->spr, m_Spr->Width, 0);
			}
			break;
			}
		}
		else
		{
			if (EyeOpen(dTime))
				m_State |= IS_EYEOPENFINISH;
		}
	}

	return Unit::Draw(dTime);
	//그 외에는 기본 몬스터의 애니메이션을 따름
	//return Monster::Draw(dTime, alpha);
}

bool BossMonster::Move(float dTime)
{
	if (IsMoving())
	{
		if (IsCollision())
		{
			m_State &= (~IS_EYEOPENFINISH);
			m_PatternTime = 0;
		}
		else if (IsSummoning())
		{

		}
		else if (IsMiss())
		{
			Attack();
		}
		else
		{
			m_PatternTime += dTime;
			if (m_PatternTime >= rand() % 5 + 10)
			{
				ChangePattern();
				m_State &= (~IS_EYEOPENFINISH);
			}
			return Monster::Move(dTime);
		}
	}
	else
	{
		int w = m_Spr->Width;
		int h = m_Spr->Height;

		if (m_Pos.x > System::GetInstance()->m_ScreenSize.cx - ((w * 6) / 7) - 25)
		{
			return Monster::Move(dTime);
		}
		else
		{
			m_State |= IS_INIT_MOVE_FINISH;
		}
	}
}

void BossMonster::Release()
{
	ReleaseSprite(&m_AttackSprite);
	delete m_AttackSprite;
	m_AttackSprite = nullptr;

	ReleaseSprite(&m_HitSprite);
	delete m_HitSprite;
	m_HitSprite = nullptr;

	ReleaseSprite(&m_SummonSprite);
	delete m_SummonSprite;
	m_SummonSprite = nullptr;

	for (int i = 0; i < 6; i++)
	{
		ReleaseSprite(&m_EyesSprite[i]);
		delete m_EyesSprite[i];
		m_EyesSprite[i] = nullptr;
	}

	Monster::Release();
}
