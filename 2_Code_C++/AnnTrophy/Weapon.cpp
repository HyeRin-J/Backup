#include "GamePCH.h"
#include "Weapon.h"

Weapon::~Weapon()
{
	OnDestroy();
}

void Weapon::Awake()
{
	this->m_GameObject->m_pTransform->SetScale(0.5f, 0.5f);

	this->m_GameObject->AddComponent<BoxCollider>()->SetRect(D2D1::Point2F(m_GameObject->GetScale().width / 2, m_GameObject->GetScale().height / 2), D2D1::SizeF(96, 96));
#ifdef _DEBUG
	this->m_GameObject->GetComponent<Collider>()->SetDebugDraw();
#endif
	this->m_GameObject->GetComponent<Collider>()->AddIgnoreTag("Player");
	this->m_GameObject->GetComponent<Collider>()->AddIgnoreTag("Platform");
	this->m_GameObject->GetComponent<Collider>()->AddIgnoreTag("Bullet");
	this->m_GameObject->GetComponent<Collider>()->AddIgnoreTag("Jump Collider");
	this->m_GameObject->GetComponent<Collider>()->AddIgnoreTag("Move Collider");
	this->m_GameObject->GetComponent<Collider>()->AddIgnoreTag("Head Collider");
	this->m_GameObject->GetComponent<Collider>()->SetTrigger(true);

	std::wstring dataPath = _T("tropy36.txt");

	for (int i = 0; i < 2; i++)
	{
		WCHAR imageName[256];
		m_pSprite_0.push_back(m_GameObject->AddComponent<Sprite>());
		swprintf_s(imageName, _T("tropy0_sprite_%d.png"), i);
		m_pSprite_0[i]->LoadSprite(dataPath.c_str(), imageName, _T("tropy"));
		m_pSprite_0[i]->StopAnimation();
		m_pSprite_0[i]->SetActive(false);

		m_pSprite_1.push_back(m_GameObject->AddComponent<Sprite>());
		swprintf_s(imageName, _T("tropy1_sprite_%d.png"), i);
		m_pSprite_1[i]->LoadSprite(dataPath.c_str(), imageName, _T("tropy"));
		m_pSprite_1[i]->StopAnimation();
		m_pSprite_1[i]->SetActive(false);

		m_pSprite_2.push_back(m_GameObject->AddComponent<Sprite>());
		swprintf_s(imageName, _T("tropy2_sprite_%d.png"), i);
		m_pSprite_2[i]->LoadSprite(dataPath.c_str(), imageName, _T("tropy"));
		m_pSprite_2[i]->StopAnimation();
		m_pSprite_2[i]->SetActive(false);

		m_pSprite_3.push_back(m_GameObject->AddComponent<Sprite>());
		swprintf_s(imageName, _T("tropy3_sprite_%d.png"), i);
		m_pSprite_3[i]->LoadSprite(dataPath.c_str(), imageName, _T("tropy"));
		m_pSprite_3[i]->StopAnimation();
		m_pSprite_3[i]->SetActive(false);

		m_pSprite_12.push_back(m_GameObject->AddComponent<Sprite>());
		swprintf_s(imageName, _T("tropy12_sprite_%d.png"), i);
		m_pSprite_12[i]->LoadSprite(dataPath.c_str(), imageName, _T("tropy"));
		m_pSprite_12[i]->StopAnimation();
		m_pSprite_12[i]->SetActive(false);

		m_pSprite_13.push_back(m_GameObject->AddComponent<Sprite>());
		swprintf_s(imageName, _T("tropy13_sprite_%d.png"), i);
		m_pSprite_13[i]->LoadSprite(dataPath.c_str(), imageName, _T("tropy"));
		m_pSprite_13[i]->StopAnimation();
		m_pSprite_13[i]->SetActive(false);

		m_pSprite_23.push_back(m_GameObject->AddComponent<Sprite>());
		swprintf_s(imageName, _T("tropy23_sprite_%d.png"), i);
		m_pSprite_23[i]->LoadSprite(dataPath.c_str(), imageName, _T("tropy"));
		m_pSprite_23[i]->StopAnimation();
		m_pSprite_23[i]->SetActive(false);

		m_pSprite_123.push_back(m_GameObject->AddComponent<Sprite>());
		swprintf_s(imageName, _T("tropy123_sprite_%d.png"), i);
		m_pSprite_123[i]->LoadSprite(dataPath.c_str(), imageName, _T("tropy"));
		m_pSprite_123[i]->StopAnimation();
		m_pSprite_123[i]->SetActive(false);
	}

	for (int i = 0; i < 10; i++)
	{
		m_Bullets.push_back(ObjectFactory::CreateObject<GameObject>());
		m_Bullets[i]->AddComponent<Bullet>()->SetActive(false);
		m_Bullets[i]->SetStringTag("Bullet");
		m_Bullets[i]->GetComponent<Bullet>()->m_Damage = m_AttackDamage[_GameManager->m_CurrentState];
		m_Bullets[i]->GetComponent<Bullet>()->m_IsEffectStateScale = true;
	}
}

void Weapon::Update()
{
	//¿£Áø ½Ã°£ÀÌ ¸ØÃß¸é µå·Ðµµ ¸ØÃá´Ù!
	if (FTime::GetInstance()->m_EngineDeltaTimeScale <= 0.f) { return; };

	double angle = 0;

	D2D1_POINT_2F mousePos = InputManager::GetMousePos(AppManager::GetInstance()->m_hwnd);

	Vector2 mousePosToWeapon = Vector2(m_GameObject->GetPos(), mousePos);
	Vector2 targetVec = Vector2(m_GameObject->GetPos(), D2D1::Point2F(m_GameObject->GetPos().x, m_GameObject->GetPos().y + 100));

	angle = acos(Vector2::Dot(mousePosToWeapon, targetVec) / mousePosToWeapon.Length() / targetVec.Length());

	angle = ToDegree(angle);

	if (mousePos.x < m_GameObject->GetPos().x)
	{
		angle *= (-1);
		angle = 360 + angle;
	}

	if ((m_Upgrade & ONE_TWO_THREE) == ONE_TWO_THREE)
	{
		m_pSprite_123[_GameManager->m_CurrentState ^ 0x01]->SetActive(false);
		m_pSprite_123[_GameManager->m_CurrentState]->SetActive(true);
		m_pSprite_123[_GameManager->m_CurrentState]->m_SheetIndex = angle / 10;
	}
	else if ((m_Upgrade & TWO_THREE) == TWO_THREE)
	{
		m_pSprite_23[_GameManager->m_CurrentState ^ 0x01]->SetActive(false);
		m_pSprite_23[_GameManager->m_CurrentState]->SetActive(true);
		m_pSprite_23[_GameManager->m_CurrentState]->m_SheetIndex = angle / 10;
	}
	else if ((m_Upgrade & ONE_THREE) == ONE_THREE)
	{
		m_pSprite_13[_GameManager->m_CurrentState ^ 0x01]->SetActive(false);
		m_pSprite_13[_GameManager->m_CurrentState]->SetActive(true);
		m_pSprite_13[_GameManager->m_CurrentState]->m_SheetIndex = angle / 10;
	}
	else if ((m_Upgrade & ONE_TWO) == ONE_TWO)
	{
		m_pSprite_12[_GameManager->m_CurrentState ^ 0x01]->SetActive(false);
		m_pSprite_12[_GameManager->m_CurrentState]->SetActive(true);
		m_pSprite_12[_GameManager->m_CurrentState]->m_SheetIndex = angle / 10;
	}
	else if ((m_Upgrade & TWO) == TWO)
	{
		m_pSprite_2[_GameManager->m_CurrentState ^ 0x01]->SetActive(false);
		m_pSprite_2[_GameManager->m_CurrentState]->SetActive(true);
		m_pSprite_2[_GameManager->m_CurrentState]->m_SheetIndex = angle / 10;
	}
	else if ((m_Upgrade & ONE) == ONE)
	{
		m_pSprite_1[_GameManager->m_CurrentState ^ 0x01]->SetActive(false);
		m_pSprite_1[_GameManager->m_CurrentState]->SetActive(true);
		m_pSprite_1[_GameManager->m_CurrentState]->m_SheetIndex = angle / 10;
	}
	else if ((m_Upgrade & THREE) == THREE)
	{
		m_pSprite_3[_GameManager->m_CurrentState ^ 0x01]->SetActive(false);
		m_pSprite_3[_GameManager->m_CurrentState]->SetActive(true);
		m_pSprite_3[_GameManager->m_CurrentState]->m_SheetIndex = angle / 10;
	}
	else
	{
		m_pSprite_0[_GameManager->m_CurrentState ^ 0x01]->SetActive(false);
		m_pSprite_0[_GameManager->m_CurrentState]->SetActive(true);
		m_pSprite_0[_GameManager->m_CurrentState]->m_SheetIndex = angle / 10;
	}

#ifdef _DEBUG
	if (InputManager::InputKeyDown(VK_NUM_1))
	{
		Upgrade(0);
	}
	if (InputManager::InputKeyDown(VK_NUM_2))
	{
		Upgrade(1);
	}
	if (InputManager::InputKeyDown(VK_NUM_3))
	{
		Upgrade(2);
	}
	if (InputManager::InputKeyDown(VK_NUM_4))
	{
		Upgrade(3);
	}
	if (InputManager::InputKeyDown(VK_NUM_5))
	{
		Upgrade(4);
	}
#endif

	if (m_IsOverloadingDelay == false && InputManager::InputKeyDown(VK_R))
	{
		m_IsOverloading = true;

		m_OverloadingInversionIndex =  m_MaxBullet;

		GameManager::m_pBackgroundChannel[GameManager::m_BgmIndex]->setPaused(true);
		_FAudio->FPlaySound(OVERLOAD_START_SFX);
	}

	if (m_IsOverloading == true)
	{
		m_OverloadingDuration -= _FTime->GetObjectFixedDeltaTimeSec();

		if (m_OverloadingDuration <= 0.6f)
		{
			if (m_IsOverloadingEndBGMPlay == false)
			{
				_FAudio->FPlaySound(OVERLOAD_END_SFX);
				m_IsOverloadingEndBGMPlay = true;
			}
		}
		else if (m_OverloadingDuration <= 7.4f)
		{
			if (_FAudio->FIsPlaying(OVERLOAD_SFX) == false)
			{
				_FAudio->FPlaySound(OVERLOAD_SFX);
			}

			if (_GameManager->m_CurrentState == 1)
			{
				m_OverloadingInversionDelay += _FTime->GetEngineFixedDeltaTimeSec();
				if (m_OverloadingInversionDelay >= 0.15f)
				{
					m_OverloadingInversionDelay = 0;
					if (m_OverloadingInversionIndex >= 0)
						m_OverloadingInversionIndex--;
				}
			}
			else
			{
				m_OverloadingInversionIndex = m_MaxBullet;
			}
		}
		else
		{
			m_IsOverloadingEndBGMPlay = false;
			GameManager::m_pBackgroundChannel[GameManager::m_BgmIndex]->setPaused(false);
		}
		if (m_OverloadingDuration <= 0)
		{
			m_IsFire = false;
			m_IsOverloading = false;
			m_IsOverloadingDelay = true;
			m_OverloadingDuration = m_InitOverloadingDuration;
		}
	}

	if (m_IsOverloadingDelay == true)
	{
		m_OverloadingDelayDuration -= _FTime->GetObjectFixedDeltaTimeSec();

		if (m_OverloadingDelayDuration <= 0)
		{
			m_OverloadingDelayDuration = 20.0f;
			m_IsOverloadingDelay = false;
		}
	}

	if (m_IsFire == false && InputManager::InputKeyDown(VK_LBUTTON))
	{
		D2D1_POINT_2F mousePos = InputManager::GetMousePos(AppManager::GetInstance()->m_hwnd);

		//m_IsDelayFinish = false;
		m_IsFire = true;
		m_FireTime = 0;

		if (_GameManager->m_CurrentState == 0)
		{
			if (m_CurrBulletIndex < m_MaxBullet)
			{
				if (m_CurrBulletIndex < 0) m_CurrBulletIndex = 0;

				Bullet* m_pBullet = (m_Bullets[m_CurrBulletIndex]->GetComponent<Bullet>());
				m_pBullet->SetStartPosition(m_GameObject->GetPos());

				Vector2 distance = Vector2(mousePos.x, mousePos.y) - Vector2(m_GameObject->GetPos().x, m_GameObject->GetPos().y);
				distance.Normalize();

				Vector2 dest = Vector2(m_GameObject->GetPos().x, m_GameObject->GetPos().y) + distance * m_AttackRange;

				m_pBullet->SetDestination(D2D1::Point2F(dest.x, dest.y));
				m_pBullet->m_GameObject->m_pTransform->SetRotation(angle);
				m_pBullet->MoveStart();

				m_pBullet->m_Damage = m_AttackDamage[_GameManager->m_CurrentState];

				if (m_IsOverloading == true)
				{
					_FAudio->FPlaySound(ANN_ATTK_OVERLOAD_NORMAL);
				}
				else
				{
					int r = rand() % 3;
					switch (r)
					{
					case 0:
						_FAudio->FPlaySound(ANN_ATTK_0);
						break;
					case 1:
						_FAudio->FPlaySound(ANN_ATTK_1);
						break;
					case 2:
						_FAudio->FPlaySound(ANN_ATTK_2);
						break;
					}
				}

				++m_CurrBulletIndex;
			}
		}
		else if (_GameManager->m_CurrentState == 1)
		{
			if (m_CurrBulletIndex > 0)
			{
				m_CurrBulletIndex--;

				if (m_IsOverloading == true)
				{
					_FAudio->FPlaySound(ANN_ATTK_OVERLOAD_INVERSE);
				}
				else
				{
					int r = rand() % 2;
					switch (r)
					{
					case 0:
						_FAudio->FPlaySound(ANN_ATTK_INVERSE_0);
						break;
					case 1:
						_FAudio->FPlaySound(ANN_ATTK_INVERSE_1);
						break;
					}
				}

				Bullet* m_pBullet = m_Bullets[m_CurrBulletIndex]->GetComponent<Bullet>();
				m_pBullet->SetStartPosition(m_Bullets[m_CurrBulletIndex]->GetPos());
				m_pBullet->MoveStart();
				m_pBullet->m_IsMoveToWeapon = true;
			}
		}
	}

	if (_GameManager->m_CurrentState == 1)
	{
		if (m_CurrBulletIndex != -1)
		{
			if (m_IsOverloading == true)
			{
				for (int i = m_MaxBullet - 1; i > m_OverloadingInversionIndex; i--)
				{
					Bullet* m_pBullet = m_Bullets[i]->GetComponent<Bullet>();
					m_pBullet->SetDestination(m_GameObject->GetPos());

					m_pBullet->SetStartPosition(m_pBullet->m_GameObject->GetPos());
					m_pBullet->MoveStart();
					m_pBullet->m_IsMoveToWeapon = true;

					m_CurrBulletIndex = i;		

					m_pBullet->m_Damage = m_AttackDamage[_GameManager->m_CurrentState];

					if (m_pBullet->IsMoving() == false)
					{
						m_pBullet->m_pCollider->SetActive(false);
					}
				}
			}
			else
			{
				for (int i = m_MaxBullet - 1; i >= (m_IsOverloading ? 0 : m_CurrBulletIndex); i--)
				{
					Bullet* m_pBullet = m_Bullets[i]->GetComponent<Bullet>();
					m_pBullet->SetDestination(m_GameObject->GetPos());

					m_pBullet->m_Damage = m_AttackDamage[_GameManager->m_CurrentState];

					if (m_pBullet->IsMoving() == false)
					{
						m_pBullet->m_pCollider->SetActive(false);
					}
				}
			}

		}
	}

	if (m_IsFire)
	{
		m_FireTime += _FTime->GetEngineFixedDeltaTimeSec();

		if (m_FireTime >= m_FireDelay * (m_IsOverloading ? m_OverloadingReduceDelay : 1.0f))
		{
			m_IsFire = false;
		}
	}

	m_GameObject->SetPos(m_PlayerPos);

	//m_GameObject->m_pTransform->SetRotation(angle);
	//Debug::LogFormat("MousePos : %.2f, %.2f, MousePosVec : %.2f, %.2f, m_GameObjectPos : %.2f, %.2f, Weapon angle : %.2f", mousePos.x, mousePos.y, mousePosToWeapon.x, mousePosToWeapon.y, m_GameObject->GetPos().x, m_GameObject->GetPos().y, angle);

	}

void Weapon::OnRender()
{

	//_GraphicEngine->D2DXDrawRectrangle(m_GameObject->GetPos().x - 2, m_GameObject->GetPos().y - 2, 4, 4, ColorF::Red, ColorF::Blue);
#ifdef _DEBUG
	if (m_IsOverloading == true)
		_GraphicEngine->D2DXDrawText(200, 200, 500, 500, ColorF::Red, _T("Overloading"));
#endif

	if (m_IsFire)
	{
		D2D1_POINT_2F point = m_GameObject->GetPos();
		D2D1_SIZE_F size = D2D1::SizeF(TILE_SIZE * 2.5f, TILE_SIZE / 3);

		_GraphicEngine->D2DXDrawRectrangle(point.x - size.width / 3, point.y - (m_GameObject->GetScale().height * TILE_SIZE * 2) - size.height / 2, size.width * (m_FireTime / m_FireDelay), size.height, ColorF::Black, ColorF::Yellow, true);
	}
}

void Weapon::OnDestroy()
{
	for (auto bullet : m_Bullets)
	{
		ObjectManager::GetInstance()->Destroy(&bullet);
	}
}

void Weapon::Init()
{
	m_FireTime = 0;
	m_FireDelay = 0.8f;

	m_IsDelayFinish = true;
	m_CurrBulletIndex = 0;
}

void Weapon::Upgrade(int index)
{
	_FAudio->FPlaySound(UPGRADE_CLEAR);

	++m_UpgradeLevel[index - 1];

	float allLevel = 0;

	for (int i = 0; i < 5; ++i)
	{
		allLevel += m_UpgradeLevel[i];
	}

	float size = 0.5f + 0.1f * allLevel;

	if (size >= 2.0f) size = 2.f;

	this->m_GameObject->m_pTransform->SetScale(size, size);

	for (int i = 0; i < 2; i++)
	{
		m_pSprite_0[i]->SetActive(false);
		m_pSprite_1[i]->SetActive(false);
		m_pSprite_2[i]->SetActive(false);
		m_pSprite_3[i]->SetActive(false);
		m_pSprite_12[i]->SetActive(false);
		m_pSprite_13[i]->SetActive(false);
		m_pSprite_23[i]->SetActive(false);
		m_pSprite_123[i]->SetActive(false);
	}

	switch (index)
	{
	case 1:
	{
		m_Upgrade |= UPGRADE_ATTACK_DAMAGE;
		if (m_UpgradeLevel[index - 1] == 1)
		{
			m_AttackDamage[0] += 20;
			m_AttackDamage[1] += 30;
		}
		else if (m_UpgradeLevel[index - 1] == 2)
		{
			m_AttackDamage[0] += 30;
			m_AttackDamage[1] += 60;
		}
		else
		{
			m_AttackDamage[0] += 50;
			m_AttackDamage[1] += 60;
		}
		break;
	}
	case 2:
	{
		m_Upgrade |= UPGRADE_ATTACK_RANGE;
		if (m_UpgradeLevel[index - 1] <= 2)
		{
			m_AttackRange += 300;
		}
		else
		{
			m_AttackRange += 400;
		}
		break;
	}
	case 3:
	{
		m_Upgrade |= UPGRADE_ATTACK_SPEED;
		m_FireDelay -= 0.1f;
		break;
	}
	case 4:
	{
		m_Upgrade |= UPGRADE_OVERLOADING_DURATION;
		m_InitOverloadingDuration += 0.15f;
		break;
	}
	case 5:
	{
		m_Upgrade |= UPGRADE_MAX_BULLET;
		m_MaxBullet += 2;
		break;
	}
	}

	for (int i = 0; i < 10; ++i)
	{
		m_Bullets[i]->GetComponent<Bullet>()->IncreaseScale();
	}
}
