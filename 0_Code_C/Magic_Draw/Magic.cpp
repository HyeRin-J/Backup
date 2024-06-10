#include "../RRYEngine/framework.h"
#include "../RRYEngine/Vector.h"
#include "Unit.h"
#include "Monster.h"
#include "BossMonster.h"
#include "UnitManager.h"
#include "Magic.h"
#include "System.h"
#include "GameProc.h"

Magic::Magic()
{
	m_FireSprite = new ANISPRITE();
	m_ExploAniSprite = new ANISPRITE();
	m_Spr = new SPRITE();
}

int Magic::DrawBBox()
{
	return Unit::DrawBBox();
}

bool Magic::Hit(BBOX colBox)
{
	return false;
}

bool Magic::Explosion(float dTime, float alpha)
{
	return DrawAnimation(dTime, 10, m_Pos.x + 340, m_Pos.y, m_ExploAniSprite, alpha);
}

bool Magic::SetSprites(ANISPRITE fireSpr, ANISPRITE exploSpr)
{
	memcpy(m_FireSprite, &fireSpr, sizeof(ANISPRITE));

	memcpy(m_ExploAniSprite, &exploSpr, sizeof(ANISPRITE));
	m_ExploAniSprite->loopCount = 1;

	return Unit::SetSprite(m_FireSprite->spr);
}

void Magic::Init()
{
	m_Pos = { -100, 500 };

	m_State = 0;

	Unit::SetBBox(m_BBox);
}

bool Magic::Init(int hp, POSITION pos, float velo, Vector2 dir, int type)
{
	m_Type = type;

	m_Hp = hp;
	m_Pos = pos;
	m_Velo = velo;
	m_Dir = dir;

	Unit::SetBBox(m_BBox);

	return true;
}

bool Magic::Move(float dTime)
{
	if (IsFire() && !IsCollision())
	{
		if (!Unit::Move(dTime))
		{
			Init();
			return false;
		}
		return true;
	}
	else
	{
		return false;
	}
}

bool Magic::Draw(float dTime, float alpha)
{
	if (IsFire() && !IsCollision())
	{
		DrawAnimation(dTime, 20, (int)m_Pos.x, (int)m_Pos.y, m_FireSprite);
	}

	if (IsCollision())
	{
		long currTime = GetTickCount64();
		float alpha = 1;
		if ((currTime - m_CollTime) / 1000 > 1)
			alpha = 0;
		else alpha = 1 * (1 - ((currTime - m_CollTime) / 1000.0f));

		if (Explosion(dTime, alpha))
		{
			Init();
		}
	}

	if (System::GetInstance()->m_State & SHOWBBOX)
	{
		DrawBBox();
	}
	return true;
}

void Magic::Release()
{
	ReleaseSprite(&m_Spr);
	delete m_Spr;
	m_Spr = nullptr;

	ReleaseSprite(&m_FireSprite);
	delete m_FireSprite;
	m_FireSprite = nullptr;

	ReleaseSprite(&m_ExploAniSprite);
	delete m_ExploAniSprite;
	m_ExploAniSprite = nullptr;
}

bool Magic::CheckCollision()
{
	for (int i = 0; i < ENEMY_MAX_; i++)
	{
		Monster* mon = UnitManager::GetInstance()->m_Monsters[i];

		if (!mon->IsCreated() || mon->IsDead()) continue;
		
		if (UnitManager::GetInstance()->CheckCollision(GetBBox(), mon->GetBBox()))
		{
			m_CollTime = GetTickCount64();
			if (mon->Hit(m_Type) && StageInfo == Stage::stage)
			{
				g_Score += UnitManager::m_Score[i / 20][(i / 10) % 2];
				UnitManager::GetInstance()->CheckStage();
			}
			return true;
		}
	}
	if (StageInfo == Stage::boss)
	{
		BossMonster* boss = UnitManager::GetInstance()->m_BossMonster;

		if (UnitManager::GetInstance()->CheckCollision(GetBBox(), boss->GetBBox()))
		{
			m_CollTime = GetTickCount64();
			boss->Hit(m_Type);

			return true;
		}
	}
	return false;
}