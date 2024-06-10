#pragma once
class Player : public Component
{
public:
	union 
	{
		float HP = 5;
	}Status;
private:
	//std::vector<int>	m_IdleSprite;
	//std::vector<int>	m_MoveSprite;
	//std::vector<int>	m_JumpSprite;
	//std::vector<int>	m_FallSprite;
	//std::vector<int>	m_JumpBelowSprite;
	//std::vector<int>	m_DashSprite;
	//std::vector<int>	m_HitSprite;
	//std::vector<int>	m_InteractionSprite;
	//std::vector<int>	m_DieSprite;
	//std::vector<int>	m_ConvertSprite;
	//std::vector<int>	m_DoubleJumpSprite;
	
	std::vector<Sprite*>	m_IdleSprite;
	std::vector<Sprite*>	m_MoveSprite;
	std::vector<Sprite*>	m_JumpSprite;
	std::vector<Sprite*>	m_FallSprite;
	std::vector<Sprite*>	m_JumpBelowSprite;
	std::vector<Sprite*>	m_DashSprite;
	std::vector<Sprite*>	m_HitSprite;
	std::vector<Sprite*>	m_InteractionSprite;
	std::vector<Sprite*>	m_ConvertSprite;
	std::vector<Sprite*>	m_DoubleJumpSprite;

public:
	std::vector<Sprite*>	m_DieSprite;
	//AnimationController*	m_pAnimationController;

	//testing
	GameObject* m_Gun;
	BoxCollider*			m_pHitCollider;
	
	~Player();

	unsigned int m_MAX_HP = 5;

	bool		m_IsMusuk = false;
	bool		m_IsMoving = false;
	// -1->왼쪽, 1-> 오른쪽,
	int			m_Direction = 1;
	// -1 -> 점프 X, 0->점프, 1->하단점프
	int			m_JumpState = -1;
	bool		m_IsDash = false;
	bool		m_IsDoubleJump = false;
	bool		m_IsHit = false;
	bool		m_IsInteraction = false;
	bool		m_IsDead = false;
	bool		m_IsConvert = false;


	virtual void	Awake();
	virtual void	Update();
	virtual void	OnRender();
	virtual void	OnDestroy();
	void	Init();

	void	Damage(float _damage);

	void StopAllAnimation();
};

