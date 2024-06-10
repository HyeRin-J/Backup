#pragma once

#define ENEMY_MAX_ 80

struct BBOX;

class Hero;
class Monster;
class BossMonster;

class UnitManager
{
	static UnitManager*		m_Instance;
	int						m_CreateCount[3 * 4 * 2] =
							{	1, 0, 1, 0, 1, 0, 1, 0,
								13, 0, 10, 6, 0, 0, 0, 0,
								8, 0, 7, 5, 3, 0, 4, 0
							};
	int						m_CurrentMonsterCount[3 * 4 * 2] = { 0, };
	float					m_createTimeInFunction[4][2] = { 0, };


public:
	static int				m_Score[4][2];
	float					m_CreateTimer[2][4][2] = 
							{
								{2, -1, 3.9, 6.9, -1, -1, -1, -1},
								{3.7, -1, 6, 7, 17, -1, 10.5, -1},
							};
	int						m_MonsterSpeed[2][4][2] = 
							{ 
								{130, 0, 102, 182, 0, 0, 0, 0},
								{100, 0, 122, 82, 110, 0, 70, 0},
							};

	bool					m_IsHeroHit = false;
	static int				KillCount;

	SPRITE					m_HpSprite, m_HeroHitSprite;
	ANISPRITE				m_HeroSprites[5];
	Hero*					m_Hero = nullptr;

	SPRITE					m_ArrowSprite;
	ANISPRITE				m_MonsterAniSprites[4];
	ANISPRITE				m_MonsterDeadAniSprite[4];

	ANISPRITE				m_BossSprites[5];
	ANISPRITE				m_BossEyeSprite[6];
	SPRITE					m_BossHpSprites[9];

	Monster*				m_Monsters[ENEMY_MAX_] = { nullptr, };

	BossMonster*			m_BossMonster = nullptr;

	static UnitManager*		GetInstance();

	void	Init();
	void	Release();

	void	CreateInitEnemy();

	void	MoveMonster();
	void	MoveBoss();

	void	DrawHero();
	void	DrawMonster();
	void	DrawBoss();

	int		CreateHero();
	int		CreateBoss();
	int		CheckCreateTime(float dTime);
	void	MakeEnemy(int type, int pattern, float x, float y);
	void	CheckStage();

	bool	CheckCollision(BBOX left, BBOX right);
	bool	CheckHeroCollision();
};

#define STAGE_1_COUNT 4
#define STAGE_2_COUNT 29
#define STAGE_3_COUNT 27

#define STAGE_COUNT(i) ((i == 0) ? STAGE_1_COUNT : ((i == 1) ? STAGE_2_COUNT : STAGE_3_COUNT))

