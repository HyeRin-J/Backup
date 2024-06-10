#pragma once

#define UPGRADE_ATTACK_DAMAGE			0x00001
#define UPGRADE_ATTACK_RANGE			0x00002
#define UPGRADE_ATTACK_SPEED			0x00004
#define UPGRADE_OVERLOADING_DURATION	0x00008
#define UPGRADE_MAX_BULLET				0x00010

#define ONE				(UPGRADE_ATTACK_RANGE)
#define TWO				(UPGRADE_ATTACK_DAMAGE)
#define THREE			(UPGRADE_MAX_BULLET)
#define ONE_TWO			(ONE | TWO)
#define ONE_THREE		(ONE | THREE)
#define TWO_THREE		(TWO | THREE)
#define ONE_TWO_THREE	(ONE | TWO | THREE)

class Weapon : public Component
{
private:
	std::vector<Sprite*> m_pSprite_0;
	std::vector<Sprite*> m_pSprite_1;
	std::vector<Sprite*> m_pSprite_2;
	std::vector<Sprite*> m_pSprite_3;
	std::vector<Sprite*> m_pSprite_12;
	std::vector<Sprite*> m_pSprite_13;
	std::vector<Sprite*> m_pSprite_23;
	std::vector<Sprite*> m_pSprite_123;

public:
	int		m_MaxBullet = 4;
	float	m_FireTime = 0;
	float	m_FireDelay = 0.8f;
	float	m_OverloadingReduceDelay = 0.5f;
	float	m_InitOverloadingDuration = 8.0f;
	float	m_OverloadingDuration = 8.0f;
	float	m_OverloadingDelayDuration = 20.0f;
	float	m_AttackRange = 1000;
	bool	m_IsDelayFinish = true;
	bool	m_IsOverloadingEndBGMPlay = false;
	int		m_Upgrade = 0x00000;
	float	m_OverloadingInversionDelay = 0;
	int		m_OverloadingInversionIndex = 0;

public:
	float	m_AttackDamage[2] = { 100, 150 };

	int		m_UpgradeLevel[5] = { 0 };
	int		m_CurrBulletIndex = 0;

	std::vector<GameObject*> m_Bullets;
	D2D1_POINT_2F m_PlayerPos;
	//D2D1_POINT_2F m_StartPos;
	~Weapon();

	bool		m_IsOverloadingDelay = false;
	bool		m_IsOverloading = false;
	bool		m_IsFire = false;

	virtual void	Awake();
	virtual void	Update();
	virtual void	OnRender();
	virtual void	OnDestroy();

	void			Init();

	void			Upgrade(int index);
	int				GetMaxBullet() { return m_MaxBullet; }
};