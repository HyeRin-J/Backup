#include "GamePCH.h"
#include "Player.h"

Player::~Player()
{
	OnDestroy();
}

void Player::Awake()
{
	m_MAX_HP = 5;
	Status.HP = m_MAX_HP;
	this->m_GameObject->SetStringName("Player");
	this->m_GameObject->SetStringTag("Player");
	this->m_GameObject->SetPos(D2D1::Point2F(0, 300.f));
	this->m_GameObject->m_pTransform->SetScale(1.f, 1.f);

	/*
	m_pAnimationController = m_GameObject->AddComponent<AnimationController>();
	m_pAnimationController->AddCondition("ConversionState");
	m_pAnimationController->AddCondition("Move");
	m_pAnimationController->AddCondition("Jump");
	m_pAnimationController->AddCondition("Dash");
	m_pAnimationController->AddCondition("Damage");
	m_pAnimationController->AddCondition("Interaction");
	m_pAnimationController->AddCondition("Dead");
	m_pAnimationController->AddCondition("Double Jump");
	m_pAnimationController->AddCondition("Fall");
	m_pAnimationController->AddCondition("Down Jump");
	*/
	m_pHitCollider = this->m_GameObject->AddComponent<BoxCollider>();
	m_pHitCollider->SetRect(D2D1::Point2F(0, -10), D2D1::SizeF(80, 160));
	m_pHitCollider->SetTrigger(true);
	m_pHitCollider->SetStringTag("Hit Collider");
	m_pHitCollider->AddIgnoreTag("Weapon");
	m_pHitCollider->AddIgnoreTag("Bullet");
	m_pHitCollider->AddIgnoreTag("Floor");
#ifdef _DEBUG
	m_pHitCollider->SetDebug();
#endif
	Sprite* _setSprite = nullptr;

	for (int i = 0; i < 2; i++)
	{
		WCHAR imageName[256] = _T("\0");

		/*
			swprintf_s(imageName, _T("ann_idle_sprite_%d.png"), i);
			m_IdleSprite.push_back(m_pAnimationController->AddSprite(_T("ann_idle.txt"), imageName, _T("Ann")));

			swprintf_s(imageName, _T("ann_move_sprite_%d.png"), i);
			m_MoveSprite.push_back(m_pAnimationController->AddSprite(_T("ann_move.txt"), imageName, _T("Ann")));

			swprintf_s(imageName, _T("ann_jump_sprite_%d.png"), i);
			m_JumpSprite.push_back(m_pAnimationController->AddSprite(_T("ann_jump.txt"), imageName, _T("Ann")));
			_setSprite = m_pAnimationController->GetSprite(m_JumpSprite[i]);
			_setSprite->m_IsPlayOnce = true;		// 한번만 실행할 애니메이션
			if (i == 1)	_setSprite->m_IsInversePlay = true;	// 역재생
			_setSprite->SetActive(false);
			_setSprite->StopAnimation();

			swprintf_s(imageName, _T("ann_fall_sprite_%d.png"), i);
			m_FallSprite.push_back(m_pAnimationController->AddSprite(_T("ann_fall.txt"), imageName, _T("Ann")));
			_setSprite = m_pAnimationController->GetSprite(m_FallSprite[i]);
			_setSprite->m_IsPlayOnce = true;		// 한번만 실행할 애니메이션
			if (i == 1)	_setSprite->m_IsInversePlay = true;	// 역재생
			_setSprite->SetActive(false);
			_setSprite->StopAnimation();

			swprintf_s(imageName, _T("ann_jumpbelow_sprite_%d.png"), i);
			m_JumpBelowSprite.push_back(m_pAnimationController->AddSprite(_T("ann_jumpbelow.txt"), imageName, _T("Ann")));
			_setSprite = m_pAnimationController->GetSprite(m_JumpBelowSprite[i]);
			_setSprite->m_IsPlayOnce = true;		// 한번만 실행할 애니메이션
			if (i == 1)	_setSprite->m_IsInversePlay = true;	// 역재생
			_setSprite->SetActive(false);
			_setSprite->StopAnimation();

			swprintf_s(imageName, _T("ann_dash_sprite_%d.png"), i);
			m_DashSprite.push_back(m_pAnimationController->AddSprite(_T("ann_dash.txt"), imageName, _T("Ann")));

			swprintf_s(imageName, _T("ann_hit_sprite_%d.png"), i);
			m_HitSprite.push_back(m_pAnimationController->AddSprite(_T("ann_hit.txt"), imageName, _T("Ann")));
			_setSprite = m_pAnimationController->GetSprite(m_HitSprite[i]);
			_setSprite->m_IsPlayOnce = true;		// 한번만 실행할 애니메이션
			if (i == 1)	_setSprite->m_IsInversePlay = true;	// 역재생
			_setSprite->SetActive(false);
			_setSprite->StopAnimation();

			swprintf_s(imageName, _T("ann_die_sprite_%d.png"), i);
			m_DieSprite.push_back(m_pAnimationController->AddSprite(_T("ann_die.txt"), imageName, _T("Ann")));
			_setSprite = m_pAnimationController->GetSprite(m_DieSprite[i]);
			_setSprite->m_IsPlayOnce = true;		// 한번만 실행할 애니메이션
			_setSprite->SetActive(false);
			_setSprite->StopAnimation();

			swprintf_s(imageName, _T("ann_convert_sprite_%d.png"), i);
			m_ConvertSprite.push_back(m_pAnimationController->AddSprite(_T("ann_convert.txt"), imageName, _T("Ann")));
			_setSprite = m_pAnimationController->GetSprite(m_ConvertSprite[i]);
			_setSprite->m_IsPlayOnce = true;	// 한번만 실행할 애니메이션
			_setSprite->m_IsInversePlay = true;	// 역재생
			_setSprite->SetActive(false);
			_setSprite->StopAnimation();

			swprintf_s(imageName, _T("ann_doubleJump_sprite_%d.png"), i);
			m_DoubleJumpSprite.push_back(m_pAnimationController->AddSprite(_T("ann_doubleJump.txt"), imageName, _T("Ann")));
			_setSprite = m_pAnimationController->GetSprite(m_DoubleJumpSprite[i]);
			_setSprite->m_IsPlayOnce = true;	// 한번만 실행할 애니메이션
			if (i == 1) _setSprite->m_IsInversePlay = true;	// 역재생
			_setSprite->SetActive(false);
			_setSprite->StopAnimation();
			*/

		m_IdleSprite.push_back(m_GameObject->AddComponent<Sprite>());
		swprintf_s(imageName, _T("ann_idle_sprite_%d.png"), i);
		m_IdleSprite[i]->LoadSprite(_T("ann_idle.txt"), imageName, _T("Ann"));
		if (i == 1) m_IdleSprite[i]->m_IsInversePlay = true;	// 역재생
		m_IdleSprite[i]->StopAnimation();
		m_IdleSprite[i]->SetActive(false);


		m_MoveSprite.push_back(m_GameObject->AddComponent<Sprite>());
		swprintf_s(imageName, _T("ann_move_sprite_%d.png"), i);
		m_MoveSprite[i]->LoadSprite(_T("ann_move.txt"), imageName, _T("Ann"));
		if (i == 1) m_MoveSprite[i]->m_IsInversePlay = true;	// 역재생
		m_MoveSprite[i]->StopAnimation();
		m_MoveSprite[i]->SetActive(false);

		m_JumpSprite.push_back(m_GameObject->AddComponent<Sprite>());
		swprintf_s(imageName, _T("ann_jump_sprite_%d.png"), i);
		m_JumpSprite[i]->LoadSprite(_T("ann_jump.txt"), imageName, _T("Ann"));
		if (i == 1) m_JumpSprite[i]->m_IsInversePlay = true;	// 역재생
		m_JumpSprite[i]->StopAnimation();
		m_JumpSprite[i]->m_IsPlayOnce = true;
		m_JumpSprite[i]->SetActive(false);

		m_FallSprite.push_back(m_GameObject->AddComponent<Sprite>());
		swprintf_s(imageName, _T("ann_fall_sprite_%d.png"), i);
		m_FallSprite[i]->LoadSprite(_T("ann_fall.txt"), imageName, _T("Ann"));
		if (i == 1) m_FallSprite[i]->m_IsInversePlay = true;	// 역재생
		m_FallSprite[i]->StopAnimation();
		m_FallSprite[i]->m_IsPlayOnce = true;
		m_FallSprite[i]->SetActive(false);

		m_JumpBelowSprite.push_back(m_GameObject->AddComponent<Sprite>());
		swprintf_s(imageName, _T("ann_jumpbelow_sprite_%d.png"), i);
		m_JumpBelowSprite[i]->LoadSprite(_T("ann_jumpbelow.txt"), imageName, _T("Ann"));
		if (i == 1) m_JumpBelowSprite[i]->m_IsInversePlay = true;	// 역재생
		m_JumpBelowSprite[i]->StopAnimation();
		m_JumpBelowSprite[i]->m_IsPlayOnce = true;
		m_JumpBelowSprite[i]->SetActive(false);

		m_DashSprite.push_back(m_GameObject->AddComponent<Sprite>());
		swprintf_s(imageName, _T("ann_dash_sprite_%d.png"), i);
		m_DashSprite[i]->LoadSprite(_T("ann_dash.txt"), imageName, _T("Ann"));
		if (i == 1) m_DashSprite[i]->m_IsInversePlay = true;	// 역재생
		m_DashSprite[i]->StopAnimation();
		m_DashSprite[i]->SetActive(false);

		m_HitSprite.push_back(m_GameObject->AddComponent<Sprite>());
		swprintf_s(imageName, _T("ann_hit_sprite_%d.png"), i);
		m_HitSprite[i]->LoadSprite(_T("ann_hit.txt"), imageName, _T("Ann"));
		if (i == 1) m_HitSprite[i]->m_IsInversePlay = true;	// 역재생
		m_HitSprite[i]->m_IsPlayOnce = true;
		m_HitSprite[i]->StopAnimation();
		m_HitSprite[i]->SetActive(false);

		m_DieSprite.push_back(m_GameObject->AddComponent<Sprite>());
		swprintf_s(imageName, _T("ann_die_sprite_%d.png"), i);
		m_DieSprite[i]->LoadSprite(_T("ann_die.txt"), imageName, _T("Ann"));
		m_DieSprite[i]->StopAnimation();
		m_DieSprite[i]->m_IsPlayOnce = true;
		m_DieSprite[i]->SetActive(false);

		m_ConvertSprite.push_back(m_GameObject->AddComponent<Sprite>());
		swprintf_s(imageName, _T("ann_convert_sprite_%d.png"), i);
		m_ConvertSprite[i]->LoadSprite(_T("ann_convert.txt"), imageName, _T("Ann"));
		m_ConvertSprite[i]->m_IsInversePlay = true;
		m_ConvertSprite[i]->m_IsPlayOnce = true;
		m_ConvertSprite[i]->StopAnimation();
		m_ConvertSprite[i]->SetActive(false);

		m_DoubleJumpSprite.push_back(m_GameObject->AddComponent<Sprite>());
		swprintf_s(imageName, _T("ann_doubleJump_sprite_%d.png"), i);
		m_DoubleJumpSprite[i]->LoadSprite(_T("ann_doubleJump.txt"), imageName, _T("Ann"));
		m_DoubleJumpSprite[i]->m_IsPlayOnce = true;	// 한번만 실행할 애니메이션
		m_DoubleJumpSprite[i]->SetActive(false);
		m_DoubleJumpSprite[i]->StopAnimation();
	}

	WCHAR imageName[256];

	m_InteractionSprite.push_back(m_GameObject->AddComponent<Sprite>());
	swprintf_s(imageName, _T("ann_interactionStart_sprite.png"));
	m_InteractionSprite[0]->LoadSprite(_T("ann_interactionStart.txt"), imageName, _T("Ann"));
	m_InteractionSprite[0]->StopAnimation();
	m_InteractionSprite[0]->SetActive(false);
	m_InteractionSprite[0]->m_IsPlayOnce = true;

	m_InteractionSprite.push_back(m_GameObject->AddComponent<Sprite>());
	swprintf_s(imageName, _T("ann_interaction_sprite.png"));
	m_InteractionSprite[1]->LoadSprite(_T("ann_interaction.txt"), imageName, _T("Ann"));
	m_InteractionSprite[1]->StopAnimation();
	m_InteractionSprite[1]->SetActive(false);

	/*
	}
	m_InteractionSprite.push_back(m_pAnimationController->AddSprite(_T("ann_interactionStart.txt"), _T("ann_interactionStart_sprite.png"), _T("Ann")));
	m_InteractionSprite.push_back(m_pAnimationController->AddSprite(_T("ann_interaction.txt"), _T("ann_interaction_sprite.png"), _T("Ann")));

	for (int i = 0; i < 2; i++)
	{
		//Idle -> Convert
		m_pAnimationController->AddTransition(m_IdleSprite[i], m_ConvertSprite[i]);
		m_pAnimationController->AddConditionToTransition(m_IdleSprite[i], m_ConvertSprite[i], "ConversionState", true);
		//Idle -> Move
		m_pAnimationController->AddTransition(m_IdleSprite[i], m_MoveSprite[i]);
		m_pAnimationController->AddConditionToTransition(m_IdleSprite[i], m_MoveSprite[i], "Move", true);
		// Idle-> Damage
		m_pAnimationController->AddTransition(m_IdleSprite[i], m_HitSprite[i]);
		m_pAnimationController->AddConditionToTransition(m_IdleSprite[i], m_HitSprite[i], "Damage", true);
		//Idle -> Dead
		m_pAnimationController->AddTransition(m_IdleSprite[i], m_DieSprite[i]);
		m_pAnimationController->AddConditionToTransition(m_IdleSprite[i], m_DieSprite[i], "Dead", true);
		if (i == 0)
		{
			//Idle -> Jump
			m_pAnimationController->AddTransition(m_IdleSprite[i], m_JumpSprite[i]);
			m_pAnimationController->AddConditionToTransition(m_IdleSprite[i], m_JumpSprite[i], "Jump", true);
			//Jump -> Fall
			m_pAnimationController->AddTransition(m_JumpSprite[i], m_FallSprite[i], true);
			m_pAnimationController->AddConditionToTransition(m_JumpSprite[i], m_FallSprite[i], "Fall", true);
			//Jump -> Double Jump
			m_pAnimationController->AddTransition(m_JumpSprite[i], m_DoubleJumpSprite[i], true);
			m_pAnimationController->AddConditionToTransition(m_JumpSprite[i], m_DoubleJumpSprite[i], "Double Jump", true);
			//Jump -> Dash
			m_pAnimationController->AddTransition(m_JumpSprite[i], m_DashSprite[i], true);
			m_pAnimationController->AddConditionToTransition(m_JumpSprite[i], m_DashSprite[i], "Dash", true);
			//Jump -> Fall
			m_pAnimationController->AddTransition(m_DoubleJumpSprite[i], m_FallSprite[i], true);
			m_pAnimationController->AddConditionToTransition(m_DoubleJumpSprite[i], m_FallSprite[i], "Fall", true);
			//Double Jump -> Dash
			m_pAnimationController->AddTransition(m_DoubleJumpSprite[i], m_DashSprite[i], true);
			m_pAnimationController->AddConditionToTransition(m_DoubleJumpSprite[i], m_DashSprite[i], "Dash", true);
			//Fall -> Jump
			m_pAnimationController->AddTransition(m_FallSprite[i], m_JumpSprite[i], true);
			m_pAnimationController->AddConditionToTransition(m_FallSprite[i], m_JumpSprite[i], "Jump", true);
			//Jump -> Idle
			m_pAnimationController->AddTransition(m_JumpSprite[i], m_IdleSprite[i], true);
			m_pAnimationController->AddConditionToTransition(m_JumpSprite[i], m_IdleSprite[i], "Jump", false);
			//Fall -> Dash
			m_pAnimationController->AddTransition(m_FallSprite[i], m_DashSprite[i], true);
			m_pAnimationController->AddConditionToTransition(m_FallSprite[i], m_DashSprite[i], "Dash", true);
			//Fall -> Idle
			m_pAnimationController->AddTransition(m_FallSprite[i], m_IdleSprite[i], true);
			m_pAnimationController->AddConditionToTransition(m_FallSprite[i], m_IdleSprite[i], "Fall", false);
			m_pAnimationController->AddConditionToTransition(m_FallSprite[i], m_IdleSprite[i], "Jump", false);
			m_pAnimationController->AddConditionToTransition(m_FallSprite[i], m_IdleSprite[i], "Double Jump", false);
			//Idle -> Down Jump
			m_pAnimationController->AddTransition(m_IdleSprite[i], m_JumpBelowSprite[i]);
			m_pAnimationController->AddConditionToTransition(m_IdleSprite[i], m_JumpBelowSprite[i], "Down Jump", true);
		}
		else
		{
			//Idle -> Jump
			m_pAnimationController->AddTransition(m_IdleSprite[i], m_JumpSprite[i]);
			m_pAnimationController->AddConditionToTransition(m_IdleSprite[i], m_JumpSprite[i], "Fall", true);
			//Idle -> Down Jump
			m_pAnimationController->AddConditionToTransition(m_IdleSprite[i], m_JumpSprite[i], "Down Jump", true);
			//Fall -> Jump
			m_pAnimationController->AddTransition(m_FallSprite[i], m_JumpSprite[i], true);
			m_pAnimationController->AddConditionToTransition(m_FallSprite[i], m_JumpSprite[i], "Fall", true);
			//Jump -> Idle
			m_pAnimationController->AddTransition(m_JumpSprite[i], m_IdleSprite[i], true);
			m_pAnimationController->AddConditionToTransition(m_JumpSprite[i], m_IdleSprite[i], "Jump", false);
			m_pAnimationController->AddConditionToTransition(m_JumpSprite[i], m_IdleSprite[i], "Fall", false);
			m_pAnimationController->AddConditionToTransition(m_JumpSprite[i], m_IdleSprite[i], "Double Jump", false);
		}

		//Down Jump -> Idle
		m_pAnimationController->AddTransition(m_JumpBelowSprite[i], m_IdleSprite[i]);
		m_pAnimationController->AddConditionToTransition(m_JumpBelowSprite[i], m_IdleSprite[i], "Down Jump", false);

		//Move -> Idle
		m_pAnimationController->AddTransition(m_MoveSprite[i], m_IdleSprite[i]);
		m_pAnimationController->AddConditionToTransition(m_MoveSprite[i], m_IdleSprite[i], "Move", false);
		//Move -> Dash
		m_pAnimationController->AddTransition(m_MoveSprite[i], m_DashSprite[i]);
		m_pAnimationController->AddConditionToTransition(m_MoveSprite[i], m_DashSprite[i], "Dash", true);
		//Move -> Convert
		m_pAnimationController->AddTransition(m_MoveSprite[i], m_ConvertSprite[i]);
		m_pAnimationController->AddConditionToTransition(m_MoveSprite[i], m_ConvertSprite[i], "ConversionState", true);
		// Move -> Damage
		m_pAnimationController->AddTransition(m_MoveSprite[i], m_HitSprite[i]);
		m_pAnimationController->AddConditionToTransition(m_MoveSprite[i], m_HitSprite[i], "Damage", true);

		if (i == 0)
		{
			//Move -> Fall
			m_pAnimationController->AddTransition(m_MoveSprite[i], m_FallSprite[i]);
			m_pAnimationController->AddConditionToTransition(m_MoveSprite[i], m_FallSprite[i], "Fall", true);
			//Move -> Jump
			m_pAnimationController->AddTransition(m_MoveSprite[i], m_JumpSprite[i]);
			m_pAnimationController->AddConditionToTransition(m_MoveSprite[i], m_JumpSprite[i], "Jump", true);
			//Move -> Down Jump
			m_pAnimationController->AddTransition(m_MoveSprite[i], m_JumpBelowSprite[i], true);
			m_pAnimationController->AddConditionToTransition(m_MoveSprite[i], m_JumpBelowSprite[i], "Down Jump", true);
		}
		else
		{
			// Move -> Jump
			m_pAnimationController->AddTransition(m_MoveSprite[i], m_JumpSprite[i], true);
			m_pAnimationController->AddConditionToTransition(m_MoveSprite[i], m_JumpSprite[i], "Fall", true);
			// Move -> Fall
			m_pAnimationController->AddTransition(m_MoveSprite[i], m_FallSprite[i]);
			m_pAnimationController->AddConditionToTransition(m_MoveSprite[i], m_FallSprite[i], "Jump", true);
			// Move -> Down Jump
			m_pAnimationController->AddConditionToTransition(m_IdleSprite[i], m_JumpSprite[i], "Down Jump", true);
		}

		//Dash -> Move
		m_pAnimationController->AddTransition(m_DashSprite[i], m_MoveSprite[i]);
		m_pAnimationController->AddConditionToTransition(m_DashSprite[i], m_MoveSprite[i], "Dash", false);
		//Dash -> Convert
		m_pAnimationController->AddTransition(m_DashSprite[i], m_ConvertSprite[i]);
		m_pAnimationController->AddConditionToTransition(m_DashSprite[i], m_ConvertSprite[i], "ConversionState", true);
		// Dash -> Damage
		m_pAnimationController->AddTransition(m_DashSprite[i], m_HitSprite[i]);
		m_pAnimationController->AddConditionToTransition(m_DashSprite[i], m_HitSprite[i], "Damage", true);

		if (i == 0)
		{
			// Dash - > Fall
			m_pAnimationController->AddTransition(m_DashSprite[i], m_FallSprite[i]);
			m_pAnimationController->AddConditionToTransition(m_DashSprite[i], m_FallSprite[i], "Fall", true);
			//Dash -> Jump
			m_pAnimationController->AddTransition(m_DashSprite[i], m_JumpSprite[i]);
			m_pAnimationController->AddConditionToTransition(m_DashSprite[i], m_JumpSprite[i], "Jump", true);
			//Dash -> Down Jump
			m_pAnimationController->AddTransition(m_DashSprite[i], m_JumpBelowSprite[i], true);
			m_pAnimationController->AddConditionToTransition(m_DashSprite[i], m_JumpBelowSprite[i], "Down Jump", true);
		}
		else
		{
			//Dash - > Jump
			m_pAnimationController->AddTransition(m_DashSprite[i], m_JumpSprite[i]);
			m_pAnimationController->AddConditionToTransition(m_DashSprite[i], m_JumpSprite[i], "Fall", true);
			//Dash -> Fall
			m_pAnimationController->AddTransition(m_DashSprite[i], m_FallSprite[i]);
			m_pAnimationController->AddConditionToTransition(m_DashSprite[i], m_FallSprite[i], "Jump", true);
			// Move -> Down Jump
			m_pAnimationController->AddTransition(m_DashSprite[i], m_JumpSprite[i], true);
			m_pAnimationController->AddConditionToTransition(m_DashSprite[i], m_JumpSprite[i], "Down Jump", true);
		}

		//Jump -> Dash
		m_pAnimationController->AddTransition(m_JumpSprite[i], m_DashSprite[i]);
		m_pAnimationController->AddConditionToTransition(m_JumpSprite[i], m_DashSprite[i], "Dash", true);

		//Fall -> Dash
		m_pAnimationController->AddTransition(m_FallSprite[i], m_DashSprite[i]);
		m_pAnimationController->AddConditionToTransition(m_FallSprite[i], m_DashSprite[i], "Dash", true);

		if (i == 0)
		{
			//Jump -> Double Jump
			m_pAnimationController->AddTransition(m_JumpSprite[i], m_DoubleJumpSprite[i]);
			m_pAnimationController->AddConditionToTransition(m_JumpSprite[i], m_DoubleJumpSprite[i], "Double Jump", true);
			//Double Jump -> Fall
			m_pAnimationController->AddTransition(m_DoubleJumpSprite[i], m_FallSprite[i]);
			m_pAnimationController->AddConditionToTransition(m_DoubleJumpSprite[i], m_FallSprite[i], "Fall", true);
		}
		else
		{
			//Jump -> Double Jump
			m_pAnimationController->AddTransition(m_FallSprite[i], m_DoubleJumpSprite[i]);
			m_pAnimationController->AddConditionToTransition(m_FallSprite[i], m_DoubleJumpSprite[i], "Double Jump", true);
			m_pAnimationController->AddConditionToTransition(m_FallSprite[i], m_DoubleJumpSprite[i], "Fall", true);
		}

		//별다른 조건 없음
		m_pAnimationController->AddTransition(m_ConvertSprite[i], m_IdleSprite[(i + 1) % 2]);

		//All -> Dead
		m_pAnimationController->AddTransition(-1, m_DieSprite[i]);
		m_pAnimationController->AddConditionToTransition(-1, m_DieSprite[i], "Dead", true);

		// Interaction -> Idle
		m_pAnimationController->AddTransition(m_InteractionSprite[1], m_IdleSprite[i]);
		m_pAnimationController->AddConditionToTransition(m_InteractionSprite[1], m_IdleSprite[i], "Interaction", false);

		//Idle -> Interaction
		m_pAnimationController->AddTransition(m_IdleSprite[i], m_InteractionSprite[0]);
		m_pAnimationController->AddConditionToTransition(-1, m_InteractionSprite[0], "Interaction", true);
	}


	m_pAnimationController->AddTransition(m_InteractionSprite[0], m_InteractionSprite[1]);

	m_pAnimationController->GetSprite(0)->SetActive(true);
	m_pAnimationController->GetSprite(0)->StartAnimation();
	*/
	//Gun setting
	m_Gun = ObjectFactory::CreateObject<GameObject>();
	m_Gun->AddComponent<Weapon>();
	m_Gun->SetStringTag("Weapon");
	//m_Gun->m_pTransform->SetPos(D2D1::Point2F(this->m_GameObject->GetPos().x , this->m_GameObject->GetPos().y- 20));
}

void Player::Update()
{
	if (InputManager::InputKeyDown(VK_F7))
	{
		m_IsMusuk = !m_IsMusuk;
	}

	StopAllAnimation();

	if (m_GameObject->GetPos().x - m_pHitCollider->m_Size.width / 2 <= 0)
	{
		m_GameObject->SetPos(D2D1::Point2F(m_pHitCollider->m_Size.width / 2, m_GameObject->GetPos().y));
	}
	else if (m_GameObject->GetPos().x + m_pHitCollider->m_Size.width / 2 >= _GameManager->pSceneManager->GetActiveScene()->m_Data.MapSize.width * TILE_SIZE)
	{
		m_GameObject->SetPos(D2D1::Point2F(_GameManager->pSceneManager->GetActiveScene()->m_Data.MapSize.width * TILE_SIZE - m_pHitCollider->m_Size.width / 2, m_GameObject->GetPos().y));
	}

	if (m_GameObject->GetPos().y + m_pHitCollider->m_Size.height / 2 >= (_GameManager->pSceneManager->GetActiveScene()->m_Data.MapSize.height - 2) * TILE_SIZE)
	{
		m_GameObject->SetPos(D2D1::Point2F(m_GameObject->GetPos().x, (_GameManager->pSceneManager->GetActiveScene()->m_Data.MapSize.height - 2) * TILE_SIZE - m_pHitCollider->m_Size.height / 2));
	}

	if (m_IsDead == true)
	{
		m_JumpSprite[_GameManager->m_CurrentState]->SetActive(false);
		m_FallSprite[_GameManager->m_CurrentState]->SetActive(false);
		m_JumpBelowSprite[_GameManager->m_CurrentState]->SetActive(false);
		m_ConvertSprite[_GameManager->m_CurrentState]->SetActive(false);
		m_DoubleJumpSprite[_GameManager->m_CurrentState]->SetActive(false);

		m_DieSprite[_GameManager->m_CurrentState]->SetActive(true);
		m_DieSprite[_GameManager->m_CurrentState]->StartAnimation();

		SceneObjectManager::GetInstance()->SetDeadScene();

		_GameManager->m_IsGameOver = true;

		SetActive(false);

		return;
	}
	else
	{
		m_DieSprite[_GameManager->m_CurrentState]->SetActive(false);
		m_DieSprite[_GameManager->m_CurrentState]->StopAnimation();
	}

	if (m_IsHit == true)
	{
		bool isCheck = true;

		for (auto col : m_pHitCollider->m_Target)
		{
			if (col->m_IsActive == true && col->GetStringTag()._Equal("EnemyBullet"))
			{
				isCheck = false;
				break;
			}
		}

		m_FallSprite[_GameManager->m_CurrentState]->SetActive(false);
		m_JumpSprite[_GameManager->m_CurrentState]->SetActive(false);
		m_JumpBelowSprite[_GameManager->m_CurrentState]->SetActive(false);
		m_DieSprite[_GameManager->m_CurrentState]->SetActive(false);
		m_ConvertSprite[_GameManager->m_CurrentState]->SetActive(false);
		m_DoubleJumpSprite[_GameManager->m_CurrentState]->SetActive(false);

		m_HitSprite[_GameManager->m_CurrentState]->SetActive(true);
		m_HitSprite[_GameManager->m_CurrentState]->StartAnimation();

		if (isCheck == true && (m_HitSprite[_GameManager->m_CurrentState]->m_SheetIndex == 0 || m_HitSprite[_GameManager->m_CurrentState]->m_IsFinish == true || m_pHitCollider->m_Target.size() <= 0))
		{
			m_HitSprite[_GameManager->m_CurrentState]->StopAnimation();
			m_IsHit = false;
			return;
		}
	}
	else
	{
		if (m_pHitCollider->m_Target.size() > 0)
		{
			for (auto col : m_pHitCollider->m_Target)
			{
				if (col->m_IsActive == true && col->GetStringTag()._Equal("EnemyBullet"))
				{
					Damage(col->m_GameObject->GetComponent<Bullet>()->m_Damage);
					
					return;
				}
			}
		}
		m_HitSprite[_GameManager->m_CurrentState]->StopAnimation();
		m_HitSprite[_GameManager->m_CurrentState]->SetActive(false);
	}



	m_GameObject->m_pTransform->SetScale(fabs(m_GameObject->m_pTransform->GetScale().width) * m_Direction, m_GameObject->m_pTransform->GetScale().height);

	if (_GameManager->m_CurrentState == 1)
	{
		m_GameObject->m_pTransform->SetScale(fabs(m_GameObject->m_pTransform->GetScale().width) * -m_Direction, m_GameObject->m_pTransform->GetScale().height);
	}

	/*
	for (int i = 0; i < 2; i++)
	{
		if (m_pAnimationController->m_CurrIndex == m_ConvertSprite[i])
		{
			m_IsConvert = true;
			break;
		}
		else
		{
			m_IsConvert = false;
		}
	}


	if (m_GameObject->GetComponent<Rigidbody>()->GetFallSpeed() > 0)
	{
		m_pAnimationController->SetCondition("Fall", true);
	}
	else if (m_GameObject->GetComponent<Rigidbody>()->GetFallSpeed() == 0)
	{
		m_pAnimationController->SetCondition("Down Jump", false);
		m_pAnimationController->SetCondition("Jump", false);
		m_pAnimationController->SetCondition("Fall", false);
	}
	else
	{
		m_pAnimationController->SetCondition("Fall", false);
	}

	if (m_IsMoving == true)
	{
		m_pAnimationController->SetCondition("Move", true);
	}
	else
	{
		m_pAnimationController->SetCondition("Move", false);
	}

	if (m_IsDash == true)
	{
		m_pAnimationController->SetCondition("Dash", true);
	}
	else
	{
		m_pAnimationController->SetCondition("Dash", false);
	}

	if (m_IsInteraction == true)
	{
		m_pAnimationController->SetCondition("Interaction", true);
	}
	else
	{
		m_pAnimationController->SetCondition("Interaction", false);
	}
	*/

	int currState = GameManager::GetInstance()->m_CurrentState;

	if (InputManager::InputKeyUp(VK_W))
	{
		m_IsInteraction = false;
	}

	if (m_IsDead == false && m_IsHit == false)
	{

		if (m_IsInteraction == true)
		{
			m_JumpState = -1;

			for (int i = 0; i < 2; i++)
			{
				m_HitSprite[i]->SetActive(false);
				m_FallSprite[i]->SetActive(false);
				m_JumpSprite[i]->SetActive(false);
				m_JumpBelowSprite[i]->SetActive(false);
				m_DieSprite[i]->SetActive(false);
				m_ConvertSprite[i]->SetActive(false);
				m_DoubleJumpSprite[i]->SetActive(false);
			}

			if (m_InteractionSprite[0]->m_IsFinish == false)
			{
				m_InteractionSprite[0]->SetActive(true);
				m_InteractionSprite[0]->StartAnimation();
			}
			else
			{
				m_InteractionSprite[0]->SetActive(false);
				//m_InteractionSprite[0]->StopAnimation();

				m_InteractionSprite[1]->SetActive(true);
				m_InteractionSprite[1]->StartAnimation();
			}
		}
		else
		{
			m_InteractionSprite[0]->SetActive(false);
			m_InteractionSprite[0]->StopAnimation();
			m_InteractionSprite[1]->SetActive(false);
			m_InteractionSprite[1]->StopAnimation();

			if (currState == 0)
			{
				m_ConvertSprite[1]->StopAnimation();
				m_ConvertSprite[1]->SetActive(false);

				if (m_JumpState == -1)
				{
					m_DoubleJumpSprite[0]->SetActive(false);

					if (m_IsConvert == true)
					{
						if (m_ConvertSprite[0]->m_IsFinish == false)
						{
							m_ConvertSprite[0]->StartAnimation();
							m_ConvertSprite[0]->SetActive(true);
						}
						else
						{
							m_ConvertSprite[0]->StopAnimation();
							m_ConvertSprite[0]->SetActive(false);
							m_IsConvert = false;
						}
					}
					else
					{
						if (m_IsMoving == false)
						{
							for (int i = 0; i < 2; i++)
							{
								m_DieSprite[i]->SetActive(false);
								m_HitSprite[i]->SetActive(false);
								m_JumpSprite[i]->StopAnimation();
								m_JumpBelowSprite[i]->StopAnimation();
								m_FallSprite[i]->StopAnimation();
								m_JumpSprite[i]->SetActive(false);
								m_JumpBelowSprite[i]->SetActive(false);
								m_FallSprite[i]->SetActive(false);
							}

							if (m_GameObject->GetComponent<Rigidbody>()->GetFallSpeed() > 0)
							{
								m_FallSprite[currState]->SetActive(true);
								m_FallSprite[currState]->StartAnimation();
							}
							else
							{
								for (int i = 0; i < 2; i++)
								{
									m_DieSprite[i]->SetActive(false);
									m_HitSprite[i]->SetActive(false);
									m_JumpSprite[i]->StopAnimation();
									m_JumpBelowSprite[i]->StopAnimation();
									m_FallSprite[i]->StopAnimation();
									m_JumpSprite[i]->SetActive(false);
									m_JumpBelowSprite[i]->SetActive(false);
									m_FallSprite[i]->SetActive(false);
								}

								m_IdleSprite[currState]->SetActive(true);
								m_IdleSprite[currState]->StartAnimation();
							}
						}
						else
						{
							if (m_IsDash == true)
							{
								m_DashSprite[currState]->SetActive(true);
								m_DashSprite[currState]->StartAnimation();
							}
							else if (m_GameObject->GetComponent<Rigidbody>()->GetFallSpeed() > 0)
							{
								m_FallSprite[currState]->SetActive(true);
								m_FallSprite[currState]->StartAnimation();
							}
							else
							{
								for (int i = 0; i < 2; i++)
								{
									m_DieSprite[i]->SetActive(false);
									m_HitSprite[i]->SetActive(false);
									m_JumpSprite[i]->StopAnimation();
									m_JumpBelowSprite[i]->StopAnimation();
									m_FallSprite[i]->StopAnimation();
									m_JumpSprite[i]->SetActive(false);
									m_JumpBelowSprite[i]->SetActive(false);
									m_FallSprite[i]->SetActive(false);
								}
								m_MoveSprite[currState]->SetActive(true);
								m_MoveSprite[currState]->StartAnimation();
							}
						}
					}
				}
				else if (m_JumpState == 0)
				{
					m_JumpSprite[1]->StopAnimation();
					m_JumpBelowSprite[1]->StopAnimation();
					m_FallSprite[1]->StopAnimation();
					m_JumpSprite[1]->SetActive(false);
					m_JumpBelowSprite[1]->SetActive(false);
					m_FallSprite[1]->SetActive(false);

					if (m_IsDoubleJump == true)
					{
						m_JumpSprite[0]->SetActive(false);
						m_JumpSprite[1]->SetActive(false);
						m_FallSprite[0]->SetActive(false);
						m_FallSprite[1]->SetActive(false);
						m_DoubleJumpSprite[1]->SetActive(false);

						m_DoubleJumpSprite[0]->SetActive(true);
						m_DoubleJumpSprite[0]->StartAnimation();
					}

					if (m_IsDash == true)
					{
						m_JumpSprite[0]->SetActive(false);
						m_FallSprite[0]->SetActive(false);
						m_DoubleJumpSprite[0]->SetActive(false);

						m_DashSprite[0]->SetActive(true);
						m_DashSprite[0]->StartAnimation();
					}
					else
					{
						m_DashSprite[0]->SetActive(false);
						m_DashSprite[0]->StopAnimation();

						if (m_JumpSprite[0]->m_IsFinish == false)
						{
							if (SceneObjectManager::GetInstance()->GetActiveScene()->m_ChangeState == false)
							{
								m_FallSprite[0]->SetActive(false);
								m_JumpBelowSprite[0]->SetActive(false);
							}
							m_JumpSprite[0]->SetActive(true);
							m_JumpSprite[0]->StartAnimation();
						}
						else
						{
							if (m_GameObject->GetComponent<Rigidbody>()->GetFallSpeed() > 0)
							{
								m_JumpSprite[0]->SetActive(false);
								m_DoubleJumpSprite[0]->SetActive(false);
								m_JumpSprite[1]->SetActive(false);
								m_FallSprite[0]->SetActive(true);
								m_FallSprite[0]->StartAnimation();
							}
						}
					}
				}
				else if (m_JumpState == 1)
				{
					m_JumpBelowSprite[1]->StopAnimation();
					m_FallSprite[1]->SetActive(false);

					if (m_IsDash == true)
					{
						m_JumpBelowSprite[0]->SetActive(false);

						m_DashSprite[0]->SetActive(true);
						m_DashSprite[0]->StartAnimation();
					}
					else
					{
						m_DashSprite[0]->SetActive(false);
						m_DashSprite[0]->StopAnimation();

						m_JumpBelowSprite[0]->SetActive(true);
						m_JumpBelowSprite[0]->StartAnimation();
					}
				}
			}

			//Weapon* weapon = m_Gun->GetComponent<Weapon>();

			//if (m_Direction == 1)
			//{
			//	weapon->m_PlayerPos = D2D1::Point2F(this->m_GameObject->GetPos().x - 120, this->m_GameObject->GetPos().y);
			//}
			//else if (m_Direction == -1)
			//{
			//	weapon->m_PlayerPos = D2D1::Point2F(this->m_GameObject->GetPos().x + 120, this->m_GameObject->GetPos().y);
			//}

			else
			{
				m_ConvertSprite[0]->StopAnimation();
				m_ConvertSprite[0]->SetActive(false);

				if (m_JumpState == -1)
				{
					m_DoubleJumpSprite[1]->SetActive(false);

					if (m_IsConvert == true)
					{
						if (m_ConvertSprite[1]->m_IsFinish == false)
						{
							m_ConvertSprite[1]->StartAnimation();
							m_ConvertSprite[1]->SetActive(true);
						}
						else
						{
							m_ConvertSprite[1]->StopAnimation();
							m_ConvertSprite[1]->SetActive(false);
							m_IsConvert = false;
						}
					}
					else
					{
						if (m_IsMoving == false)
						{

							for (int i = 0; i < 2; i++)
							{
								m_DieSprite[i]->SetActive(false);
								m_HitSprite[i]->SetActive(false);
								m_JumpSprite[i]->StopAnimation();
								m_JumpBelowSprite[i]->StopAnimation();
								m_FallSprite[i]->StopAnimation();
								m_JumpSprite[i]->SetActive(false);
								m_JumpBelowSprite[i]->SetActive(false);
								m_FallSprite[i]->SetActive(false);
							}

							if (m_GameObject->GetComponent<Rigidbody>()->GetFallSpeed() > 0)
							{
								m_JumpSprite[1]->SetActive(true);
								m_JumpSprite[1]->StartAnimation();
							}
							else
							{
								for (int i = 0; i < 2; i++)
								{
									m_DieSprite[i]->SetActive(false);
									m_HitSprite[i]->SetActive(false);
									m_JumpSprite[i]->StopAnimation();
									m_JumpBelowSprite[i]->StopAnimation();
									m_FallSprite[i]->StopAnimation();
									m_JumpSprite[i]->SetActive(false);
									m_JumpBelowSprite[i]->SetActive(false);
									m_FallSprite[i]->SetActive(false);
								}

								m_IdleSprite[currState]->SetActive(true);
								m_IdleSprite[currState]->StartAnimation();
							}

						}
						else
						{
							if (m_IsDash == true)
							{
								m_DashSprite[currState]->SetActive(true);
								m_DashSprite[currState]->StartAnimation();
							}
							else if (m_GameObject->GetComponent<Rigidbody>()->GetFallSpeed() > 0)
							{
								m_JumpSprite[1]->SetActive(true);
								m_JumpSprite[1]->StartAnimation();
							}
							else
							{
								for (int i = 0; i < 2; i++)
								{
									m_HitSprite[i]->SetActive(false);
									m_JumpSprite[i]->StopAnimation();
									m_JumpBelowSprite[i]->StopAnimation();
									m_FallSprite[i]->StopAnimation();
									m_JumpSprite[i]->SetActive(false);
									m_JumpBelowSprite[i]->SetActive(false);
									m_FallSprite[i]->SetActive(false);
								}

								m_MoveSprite[currState]->SetActive(true);
								m_MoveSprite[currState]->StartAnimation();
							}
						}
					}
				}
				else if (m_JumpState == 0)
				{
					m_JumpSprite[0]->StopAnimation();
					m_JumpBelowSprite[0]->StopAnimation();
					m_FallSprite[0]->StopAnimation();
					m_JumpSprite[0]->SetActive(false);
					m_JumpBelowSprite[0]->SetActive(false);
					m_FallSprite[0]->SetActive(false);

					if (m_IsDoubleJump == true)
					{
						m_JumpSprite[0]->SetActive(false);
						m_JumpSprite[1]->SetActive(false);
						m_FallSprite[0]->SetActive(false);
						m_FallSprite[1]->SetActive(false);
						m_DoubleJumpSprite[0]->SetActive(false);

						m_DoubleJumpSprite[1]->SetActive(true);
						m_DoubleJumpSprite[1]->StartAnimation();
					}

					if (m_IsDash == true)
					{
						m_FallSprite[1]->SetActive(false);
						m_JumpSprite[1]->SetActive(false);
						m_DoubleJumpSprite[1]->SetActive(false);

						m_DashSprite[1]->SetActive(true);
						m_DashSprite[1]->StartAnimation();
					}
					else
					{
						m_DashSprite[1]->SetActive(false);
						m_DashSprite[1]->StopAnimation();

						if (m_FallSprite[1]->m_IsFinish == false)
						{
							if (SceneObjectManager::GetInstance()->GetActiveScene()->m_ChangeState == false)
							{
								m_JumpSprite[1]->SetActive(false);
							}

							m_FallSprite[1]->SetActive(true);
							m_FallSprite[1]->StartAnimation();
						}
						else
						{
							if (m_GameObject->GetComponent<Rigidbody>()->GetFallSpeed() > 0)
							{
								m_DoubleJumpSprite[1]->SetActive(false);
								m_FallSprite[0]->SetActive(false);
								m_FallSprite[1]->SetActive(false);
								m_JumpSprite[1]->SetActive(true);
								m_JumpSprite[1]->StartAnimation();
							}
						}
					}
				}
				else if (m_JumpState == 1)
				{
					m_JumpBelowSprite[0]->StopAnimation();
					m_JumpBelowSprite[0]->SetActive(false);

					if (m_IsDash == true)
					{
						m_JumpBelowSprite[1]->SetActive(false);

						m_DashSprite[1]->SetActive(true);
						m_DashSprite[1]->StartAnimation();
					}
					else
					{
						m_DashSprite[1]->SetActive(false);
						m_DashSprite[1]->StopAnimation();

						m_JumpBelowSprite[1]->SetActive(true);
						m_JumpBelowSprite[1]->StartAnimation();
					}
				}

				//Weapon* weapon = m_Gun->GetComponent<Weapon>();


				//if (m_Direction == 1)
				//{
				//	weapon->m_PlayerPos = D2D1::Point2F(this->m_GameObject->GetPos().x + 120, this->m_GameObject->GetPos().y);
				//}
				//else if (m_Direction == -1)
				//{
				//	weapon->m_PlayerPos = D2D1::Point2F(this->m_GameObject->GetPos().x - 120, this->m_GameObject->GetPos().y);
				//}
			}
		}
	}
	else
	{
		m_DieSprite[currState]->SetActive(true);

		m_DieSprite[currState]->StartAnimation();
	}

	Weapon* weapon = m_Gun->GetComponent<Weapon>();

	weapon->m_PlayerPos = D2D1::Point2F(this->m_GameObject->GetPos().x - 120, this->m_GameObject->GetPos().y - 50);
}

void Player::OnRender()
{
#ifdef _DEBUG
	_GraphicEngine->D2DXDrawText(m_GameObject->GetPos().x, m_GameObject->GetPos().y, 50, 50, ColorF::AliceBlue, _T("HP : %.2f"), Status.HP);
#endif
}

void Player::OnDestroy()
{
	ObjectManager::GetInstance()->Destroy(&m_Gun);

	for (int i = 0; i < 2; i++)
	{
		SafeRelease(&m_MoveSprite[i]);
		SafeRelease(&m_IdleSprite[i]);
		SafeRelease(&m_JumpSprite[i]);
		SafeRelease(&m_FallSprite[i]);
		SafeRelease(&m_JumpBelowSprite[i]);
		SafeRelease(&m_DashSprite[i]);
		SafeRelease(&m_HitSprite[i]);
		SafeRelease(&m_InteractionSprite[i]);
		SafeRelease(&m_DieSprite[i]);
		SafeRelease(&m_ConvertSprite[i]);
		SafeRelease(&m_DoubleJumpSprite[i]);
	}
}

void Player::Init()
{
	StopAllAnimation();

	for (int i = 0; i < 2; i++)
	{
		m_JumpSprite[i]->SetActive(false);
		m_JumpBelowSprite[i]->SetActive(false);
		m_DieSprite[i]->SetActive(false);
		m_ConvertSprite[i]->SetActive(false);
		m_DoubleJumpSprite[i]->SetActive(false);
	}

	m_IsMoving = false;
	m_Direction = 1;
	m_JumpState = -1;
	m_IsDash = false;
	m_IsDoubleJump = false;
	m_IsHit = false;
	m_IsInteraction = false;
	m_IsDead = false;
	m_IsConvert = false;
	Status.HP = 5;

	SetActive(true);
}

void Player::Damage(float _damage)
{
	if (m_IsMusuk == true) return;

	if (m_IsHit == false)
	{
		Status.HP -= _damage;
		_FAudio->FPlaySound(ANN_HIT);

#ifdef _DEBUG
		Debug::LogFormat("Player Damage : %.2f", _damage);
#endif
		if (Status.HP <= 0)
		{
			m_IsDead = true;
			_FAudio->FPlaySound(ANN_DIE);
			_FAudio->FPlaySound(GAMEOVER_SFX);
		}
		m_IsHit = true;
	}
}

void Player::StopAllAnimation()
{

	for (int i = 0; i < 2; i++)
	{
		m_MoveSprite[i]->SetActive(false);
		m_IdleSprite[i]->SetActive(false);
		//m_JumpSprite[i]->SetActive(false);
		//m_JumpBelowSprite[i]->SetActive(false);
		m_DashSprite[i]->SetActive(false);
		m_HitSprite[i]->SetActive(false);
		m_InteractionSprite[1]->SetActive(false);
		//m_DieSprite[i]->SetActive(false);
		//m_ConvertSprite[i]->SetActive(false);
		//m_DoubleJumpSprite[i]->SetActive(false);

		m_MoveSprite[i]->StopAnimation();
		m_IdleSprite[i]->StopAnimation();
		//m_JumpSprite[i]->StopAnimation();
		//m_JumpBelowSprite[i]->StopAnimation();
		m_DashSprite[i]->StopAnimation();
		m_HitSprite[i]->StopAnimation();
		m_InteractionSprite[1]->StopAnimation();
		//m_DieSprite[i]->StopAnimation();
		//m_ConvertSprite[i]->StopAnimation();
		//m_DoubleJumpSprite[i]->StopAnimation();
	}
}
