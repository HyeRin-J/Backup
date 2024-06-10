#include "../RRYEngine/framework.h"
#include "../RRYEngine/Vector.h"
#include "Monster.h"
#include "System.h"

Monster::Monster()
{
	m_AniSprite = new ANISPRITE();

	m_DeadAniSprite = new ANISPRITE();
}

bool Monster::SetSprites(ANISPRITE anispr, ANISPRITE deadSpr)
{
	memcpy(m_AniSprite, &anispr, sizeof(ANISPRITE));

	memcpy(m_DeadAniSprite, &deadSpr, sizeof(ANISPRITE));
	m_DeadAniSprite->loopCount = 1;
	
	return Unit::SetSprite(m_AniSprite->spr);
}

void Monster::Init()
{
	m_Pos = { 1450, m_Pos.y };
	m_State = 0;
	m_DeadTime = 0;
	SetBBox(m_BBox);
}

//애니메이션 스프라이트 기준 초기화
bool Monster::Init(int hp, POSITION pos, float velo, Vector2 dir, int type)
{
	m_Type = type;

	return Unit::Init(hp, pos, velo, dir);
}

bool Monster::DeadAnimation(float dTime)
{
	m_DeadTime += dTime;
	float alpha = 1;
	if (m_DeadTime > 1)
		alpha = 0;
	else alpha = 1 - m_DeadTime;

	return DrawAnimation(dTime, 4, m_Pos.x, m_Pos.y, m_DeadAniSprite, alpha);
}

bool Monster::Hit(int pattern)
{
	if (m_Type == pattern)
	{
		m_State |= (IS_COLLISION | IS_DEAD);
		m_DeadTime = 0;
		return true;
	}
	else return false;
}

//몬스터 출력
bool Monster::Draw(float dTime, float alpha)
{
	if (!IsDead())
	{
		DrawAnimation(dTime, m_AniSprite->cutX * m_AniSprite->cutY, (int)m_Pos.x, (int)m_Pos.y, m_AniSprite);
	}
	else
	{
		if (DeadAnimation(dTime))
		{
			Init();
		}
	}

	return Unit::Draw(dTime);
}

//몬스터 이동
bool Monster::Move(float dTime)
{
	//일반 유닛의 이동

	int w = 0;
	int h = 0;

	//애니메이션 스프라이트의 경우, width와 height가 다르므로 보정이 필요함
	if (m_AniSprite != nullptr)
	{
		w = m_AniSprite->spr.Width;
		h = m_AniSprite->spr.Height;
	}
	else
	{
		w = m_Spr->Width;
		h = m_Spr->Height;
	}

	if ((m_Pos.y >= System::GetInstance()->m_ScreenSize.cy + h || m_Pos.y < -h) ||
		(m_Pos.x >= System::GetInstance()->m_ScreenSize.cx + w || m_Pos.x < -w))
	{
		Init();
		return false;
	}
	SetBBox(m_BBox);

	m_Pos.y += m_Dir.y * dTime * m_Velo;
	m_Pos.x += m_Dir.x * dTime * m_Velo;

	return true;
}

void Monster::Release()
{
	ReleaseSprite(&m_AniSprite);
	delete m_AniSprite;
	m_AniSprite = nullptr;

	ReleaseSprite(&m_DeadAniSprite);
	delete m_DeadAniSprite;
	m_DeadAniSprite = nullptr;

	Unit::Release();
}
