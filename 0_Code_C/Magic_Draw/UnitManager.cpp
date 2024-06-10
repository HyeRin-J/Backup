#include "../RRYEngine/framework.h"
#include "../RRYEngine/Vector.h"
#include "Unit.h"
#include "Hero.h"
#include "Monster.h"
#include "BossMonster.h"
#include "SoundManager.h"
#include "UnitManager.h"
#include "System.h"
#include "GameProc.h"

UnitManager* UnitManager::m_Instance;
int UnitManager::KillCount = 0;
int UnitManager::m_Score[4][2] = { {20, 0}, {30, 30}, {40, 40}, {50, 50} };

void UnitManager::Init()
{
	KillCount = 0;

	m_Hero->Init();
	m_IsHeroHit = false;

	for (int i = 0; i < ENEMY_MAX_; i++)
	{
		//생성이 안 되었을 경우를 대비
		if (m_Monsters[i] != nullptr)
			m_Monsters[i]->Init();
	}

	for (int i = 0; i < 3 * 4 * 2; i++)
		m_CurrentMonsterCount[i] = m_CreateCount[i];

	for (int i = 0; i < 4; i++)
	{
		for (int j = 0; j < 2; j++)
		{
			m_createTimeInFunction[i][j] = 0;
		}
	}

	m_BossMonster->Init();
}

//적 초기 데이터들 설정
void UnitManager::CreateInitEnemy()
{
	//40개...
	for (int i = 0; i < ENEMY_MAX_; i++)
	{
		int type = i / 20;
		int pattern = (i / 10) % 2;
		m_Monsters[i] = new Monster();

		m_Monsters[i]->Init(1, { 1450, 300 }, m_MonsterSpeed[WaveCount - 1][i / 20][pattern], { -1, 0 }, type);
		m_Monsters[i]->SetSprites(m_MonsterAniSprites[type], m_MonsterDeadAniSprite[type]);
		// 충돌 영역 정보
		m_Monsters[i]->SetBBox(60, 100, 40, 150);
	}
}

//히어로만 출력
void UnitManager::DrawHero()
{
	if(!m_Hero->Draw(System::GetInstance()->m_DTime)) StageInfo = Stage::stageClear;

	int x = 20, y = 20;

	static float warningTime = 0;

	if (m_IsHeroHit)
	{
		warningTime += System::GetInstance()->m_DTime;

		if (warningTime < 0.5f)
			DrawAlphaSprite(backBufferDC, 0, 0, m_HeroHitSprite.Width, m_HeroHitSprite.Height, &m_HeroHitSprite);
		else
		{
			m_IsHeroHit = false;
			warningTime = 0;
		}
	}

	for (int i = 0; i < m_Hero->GetHP(); i++)
	{
		DrawAlphaSprite(backBufferDC, x, y, m_HpSprite.Width, m_HpSprite.Height, &m_HpSprite);
		x += 60;
	}

}

//적 이동
void UnitManager::MoveMonster()
{
	for (int i = 0; i < ENEMY_MAX_; i++)
	{
		//생성이 안 되었을 경우를 대비
		if (m_Monsters[i] == nullptr || !m_Monsters[i]->IsCreated() || m_Monsters[i]->IsDead()) continue;

		if (!m_Monsters[i]->Move(System::GetInstance()->m_DTime))
		{
			m_Monsters[i]->Init();
		}
	}
}

//적 그리기
void UnitManager::DrawMonster()
{
	float minPosX = 1440, minPosY = 0;

	for (int i = 0; i < ENEMY_MAX_; i++)
	{
		if (m_Monsters[i] == nullptr || !m_Monsters[i]->IsCreated()) continue;

		m_Monsters[i]->Draw(System::GetInstance()->m_DTime);
		if (m_Monsters[i]->GetPos().x < minPosX)
		{
			minPosX = m_Monsters[i]->GetPos().x;
			minPosY = m_Monsters[i]->GetPos().y;
		}
	}

	//맨 앞을 표시한다
	DrawAlphaSprite(backBufferDC, minPosX, minPosY, m_ArrowSprite.Width, m_ArrowSprite.Height, &m_ArrowSprite);
}

//보스 몬스터 이동
void UnitManager::MoveBoss()
{
	if (m_BossMonster == nullptr || m_BossMonster->IsDead()) return;
	m_BossMonster->Move(System::GetInstance()->m_DTime);
}

//보스 몬스터 출력
void UnitManager::DrawBoss()
{
	if (m_BossMonster == nullptr) return;
	if (!m_BossMonster->Draw(System::GetInstance()->m_DTime))
	{
		m_Hero->m_IsFinish = true;
	}

	int x = 500;
	int y = 720;

	for (int i = 0; i < m_BossMonster->GetHP() + 1; i++)
	{
		DrawAlphaSprite(backBufferDC, x, y, m_BossHpSprites[i].Width, m_BossHpSprites[i].Height, &m_BossHpSprites[i]);
	}
}

//히어로 만들기
int UnitManager::CreateHero()
{
	m_Hero = new Hero();
	m_Hero->Init();

	//애니메이션 대응용
	m_Hero->SetSprites(m_HeroSprites[0], m_HeroSprites[1], m_HeroSprites[2], m_HeroSprites[3], m_HeroSprites[4]);
	//충돌 박스
	m_Hero->SetBBox(150, 200, 50, 400);

	return TRUE;
}

void UnitManager::MakeEnemy(int type, int pattern, float x, float y)
{
	bool isCreate = false;
	for (int i = 0; i < ENEMY_MAX_; i++)
	{
		if (m_Monsters[i]->IsCreated() || m_Monsters[i]->IsDead()) continue;			//생성된 상태면

		if (m_Monsters[i] != nullptr && m_Monsters[i]->GetType() == type && (i / 10) % 2 == pattern)
		{
			m_Monsters[i]->Init();

			int speed = m_MonsterSpeed[WaveCount - 1][i / 20][pattern];

			//튜토리얼 스테이지, 보스 스테이지
			if (WaveCount == 0 || StageInfo == Stage::boss)
				speed = 50;

			m_Monsters[i]->Init(1, { x, y }, speed, { -1, 0 }, type);
			m_Monsters[i]->m_State |= IS_CREATED;
			isCreate = true;
			break;
		}
		if (isCreate) break;
	}
}

int UnitManager::CheckCreateTime(float dTime)
{
	static int monCount = 0;

	if (WaveCount == 0)
	{
		if (monCount < 4)
		{
			MakeEnemy(monCount, 0, 1000 + (100 * monCount), (float)380 + (60 * monCount));
			monCount++;
		}
	}
	else
	{
		monCount = 0;

		//적 생성 시간 체크. 
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 2; j++)
			{
				if (m_CurrentMonsterCount[WaveCount * 8 + i * 2 + j] <= 0) continue;

				m_createTimeInFunction[i][j] += dTime;

				if (m_CreateTimer[WaveCount - 1][i][j] != -1 && (int)(m_createTimeInFunction[i][j]) >= m_CreateTimer[WaveCount - 1][i][j])
				{
					m_createTimeInFunction[i][j] = 0;
					MakeEnemy(i, j, 1450, (float)380 + (60 * i));
					m_CurrentMonsterCount[WaveCount * 8 + i * 2 + j]--;
				}
			}
		}
	}
	return TRUE;
}

void UnitManager::CheckStage()
{
	KillCount++;
	if (KillCount >= STAGE_COUNT(WaveCount))
	{
		WaveCount++;
		KillCount = 0;
		SoundManager::GetInstance()->PlayOnce(SoundList::WAVE_CLEAR);

		if (WaveCount > 2)
		{
			StageInfo = Stage::introBoss;
			SoundManager::GetInstance()->PlayBackGround();
			m_Hero->Init();
		}
	}
}

//보스 몬스터 생성
int UnitManager::CreateBoss()
{
	if (m_BossMonster != nullptr) return FALSE;

	m_BossMonster = new BossMonster();
	m_BossMonster->Init(8, { 1400, 150 }, 100, { -1, 0 });
	m_BossMonster->SetSprites(m_BossSprites[0], m_BossSprites[2], m_BossSprites[3], m_BossSprites[4], m_BossSprites[1]);

	for (int i = 0; i < 6; i++)
		m_BossMonster->SetEyes(m_BossEyeSprite[i], i);
	// 충돌 영역 정보
	m_BossMonster->SetBBox(275, 50, 50, 500);

	return TRUE;
}

//충돌 체크
bool UnitManager::CheckCollision(BBOX left, BBOX right)
{
	int dX = abs(left.Cx - right.Cx);	//둘 사이의 가로값	
	int dY = abs(left.Cy - right.Cy);	//둘 사이의 세로값
	int HX = left.Hx + right.Hx;		//가로 길이 더하기
	int HY = left.Hy + right.Hy;		//세로 길이 더하기

	//충돌 체크
	if (dX < HX && dY < HY)
	{
		return true;
	}

	return false;
}

//히어로 충돌체크
bool UnitManager::CheckHeroCollision()
{
	for (int i = 0; i < ENEMY_MAX_; i++)
	{
		if (m_Monsters[i] == nullptr || !m_Monsters[i]->IsCreated() || m_Monsters[i]->IsDead())	continue;

		if (CheckCollision(m_Hero->GetBBox(), m_Monsters[i]->GetBBox()))
		{
			m_IsHeroHit = true;

			if (m_Hero->Hit())
			{
				StageInfo = Stage::gameover;
				SoundManager::GetInstance()->PlayBackGround();
			}
			m_Monsters[i]->Hit(m_Monsters[i]->GetType());

			CheckStage();
		}
	}

	if (StageInfo == Stage::boss)
	{
		if (CheckCollision(m_Hero->GetBBox(), m_BossMonster->GetBBox()))
		{
			m_IsHeroHit = true;
			StageInfo = Stage::gameover;
			SoundManager::GetInstance()->PlayOnce(SoundList::HERO_DAMAGE);
			SoundManager::GetInstance()->PlayBackGround();

		}
	}
	return false;
}

//스프라이트 및 데이터 제거
void UnitManager::Release()
{
	m_Hero->Release();
	delete m_Hero;
	m_Hero = nullptr;

	ReleaseSprite(&m_HpSprite);
	ReleaseSprite(&m_HeroHitSprite);

	for (int i = 0; i < 4; i++)
	{
		ReleaseSprite(&m_HeroSprites[i]);
		ReleaseSprite(&m_MonsterAniSprites[i]);
		ReleaseSprite(&m_MonsterDeadAniSprite[i]);
	}

	ReleaseSprite(&m_ArrowSprite);

	for (int i = 0; i < ENEMY_MAX_; i++)
	{
		m_Monsters[i]->Release();
		delete m_Monsters[i];
		m_Monsters[i] = nullptr;
	}

	for (int i = 0; i < 5; i++)
	{
		ReleaseSprite(&m_BossSprites[i]);
	}

	for (int i = 0; i < 6; i++)
	{
		ReleaseSprite(&m_BossEyeSprite[i]);
	}

	for (int i = 0; i < 9; i++)
	{
		ReleaseSprite(&m_BossHpSprites[i]);
	}

	m_BossMonster->Release();
	delete m_BossMonster;
	m_BossMonster = nullptr;

	delete m_Instance;
	m_Instance = nullptr;
}

//싱글턴
UnitManager* UnitManager::GetInstance()
{
	if (m_Instance == nullptr)
	{
		m_Instance = new UnitManager();
	}

	return m_Instance;
}

