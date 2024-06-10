#include "GamePCH.h"
#include "SceneObjectManager.h"

SceneObjectManager::SceneObjectManager()
{
	LoadImages();
}

SceneObjectManager::~SceneObjectManager()
{
	DeleteImages();
	DeletePlayer();
	DeleteGateAndSwitch();
	DeleteBoss();
}

void SceneObjectManager::LoadImages()
{
	std::wstring path = PATH_IMAGE_UNI;
	path += _T("Sprite/boss/bossStart_sprite.png");

	if (m_BossStartBitmap == nullptr)
	{
		D2DGraphics::GetInstance()->D2DLoadBitmap(path.c_str(), &m_BossStartBitmap);
	}

	for (int j = 0; j < 4; j++)
	{
		for (int i = 0; i < 5; i++)
		{
			path = PATH_IMAGE_UNI;

			WCHAR imageName[256];
			swprintf_s(imageName, _T("Cartoon_%d_%d.png"), i, j);

			path += _T("Cartoon/");
			path += imageName;

			_GraphicEngine->D2DLoadBitmap(path.c_str(), &m_CartoonImage[i][j]);
		}
	}

	path = PATH_IMAGE_UNI;

	WCHAR imageName[256];

	swprintf_s(imageName, _T("EndingCredit.png"));

	path += _T("Cartoon/");
	path += imageName;


	_GraphicEngine->D2DLoadBitmap(path.c_str(), &m_EndingCreditImage);

	path = PATH_IMAGE_UNI;

	swprintf_s(imageName, _T("tuto_0.png"));

	path += _T("Sprite/UI/");
	path += imageName;

	_GraphicEngine->D2DLoadBitmap(path.c_str(), &m_TutorialImage[0]);

	path = PATH_IMAGE_UNI;

	swprintf_s(imageName, _T("tuto_1.png"));

	path += _T("Sprite/UI/");
	path += imageName;

	_GraphicEngine->D2DLoadBitmap(path.c_str(), &m_TutorialImage[1]);
}

void SceneObjectManager::DeleteImages()
{
	SafeRelease(&m_BossStartBitmap);

	for (int j = 0; j < 4; j++)
	{
		for (int i = 0; i < 1; i++)
		{
			SafeRelease(&m_CartoonImage[i][j]);
		}
	}

	SafeRelease(&m_EndingCreditImage);
	SafeRelease(&m_TutorialImage[0]);
	SafeRelease(&m_TutorialImage[1]);
}

void SceneObjectManager::InitializeVariable()
{
	m_TutorialIndex = 0;

	m_CartoonIndex = 0;
	m_CartoonCutIndex = 0;
	m_EndingCreditOffset = 0;

	m_BossStartIndex = 0;
	m_BossStartTime = 0;

	m_EndingTimer = 0;

	m_BossStage = false;
	m_EndingCredit = false;
	m_EndingCreditFinish = false;
	m_EndingCreditStart = false;


	m_GameStart = false;
	m_CartoonOn = false;
	m_CurrIndex = 0;
	m_DeadSceneNum = 0;
}

void SceneObjectManager::PostLoadScene()
{
	CreatePlatformAndFloor();
	bool isBoss = false;
	m_IsTutorial = false;

	if (m_pActiveScene->m_Name._Equal(_T("Boss")))
	{
		m_BossStartIndex = 0;
		_GameManager->m_CurrentState = 1;

		CreateBoss();
		DeleteGateAndSwitch();

		m_BossStage = true;
		isBoss = true;

		m_pPlayerObject->SetPos(D2D1::Point2F(1920 / 2.f, 1080 * 2 / 3));
	}
	else if (m_pActiveScene->m_Name._Equal(_T("Ending")) || m_pActiveScene->m_Name._Equal(_T("GameOver")) || m_pActiveScene->m_Name._Equal(_T("title")))
	{
		DeleteGateAndSwitch();
		if (m_pActiveScene->m_Name._Equal(_T("GameOver")) == false)
			DeletePlayer();

		if (m_pActiveScene->m_Name._Equal(_T("Ending")) == true)
		{		
			m_pActiveScene->m_IsLoadFinish = true;
		}

		DeletePlatformAndFloor();
	}
	else
	{
		if (m_pActiveScene->m_Name._Equal(_T("tutorial")) == true)
		{
			CreatePlayer();
			CreateGateAndSwitch();
			m_IsTutorial = true;
			//m_pEnemy.push_back(ObjectFactory::CreateObject<GameObject>());
			//m_pEnemy[0]->AddComponent<DashMeleeMonster>();
			//m_pEnemy[0]->AddComponent<RangedMonster>();
			//m_pEnemy[0]->AddComponent<MeleeMonster>();
			//m_pEnemy[0]->AddComponent<JumpMeleeMonster>();
		}

		if (m_pActiveScene->m_Name._Equal(_T("sector 1-1")) == true)
		{
			m_IsTutorial = true;
			m_TutorialIndex++;
		}
	}

	CheckTile();
}

void SceneObjectManager::Update()
{
	if (m_CartoonOn == true)
	{
		NextCartoon();

		return;
	}

	m_pActiveScene->Update();

	if (m_BossStage == true)
	{
		NextBossAnimationIndex();
	}

	if (m_pActiveScene->m_IsLoadFinish == true)
	{
		if (m_pActiveScene->m_Name._Equal(_T("title")) == true)
		{
			InTitle();
		}
		else if (m_pActiveScene->m_Name._Equal(_T("Ending")) == true)
		{			
			InEnding();
		}
		else if (m_pActiveScene->m_Name._Equal(_T("GameOver")) == true)
		{
			if (InputManager::InputKeyDown(VK_LBUTTON))
			{
				if (_FAudio->FIsPlaying(OVERLOAD_END_SFX) == false)
				{
					_FAudio->FPlaySound(OVERLOAD_END_SFX);
					m_pActiveScene->m_ChangeState = true;
					m_pActiveScene->m_ChangeFinish = false;
					_GameManager->m_IsGameOver = false;
				}
			}

			if (m_pActiveScene->m_ChangeFinish == true)
			{
				for (auto bullet : m_pPlayerObject->GetComponent<Player>()->m_Gun->GetComponent<Weapon>()->m_Bullets)
				{
					bullet->GetComponent<Bullet>()->Init();
				}
				m_pPlayerObject->GetComponent<Player>()->m_Gun->GetComponent<Weapon>()->Init();
				m_pPlayerObject->GetComponent<Player>()->Init();
				m_pPlayerObject->GetComponent<Player>()->m_DieSprite[1]->SetActive(true);
				m_pPlayerObject->GetComponent<Player>()->m_DieSprite[1]->StartAnimation();
				m_CurrIndex = m_DeadSceneNum;
				LoadSceneManager::GetInstance()->LoadScene(m_CurrIndex);
				SetActiveScene(LoadSceneManager::GetInstance()->GetActiveScene());
				
				if (m_pActiveScene->m_Name._Equal(_T("Boss")) == false)
				{
					CreateGateAndSwitch();
				}
				
				PostLoadScene();
			}
		}
		else
		{
			UIManager::GetInstance()->Update();
		}
	}

	if (InputManager::InputKeyDown(VK_F2))
	{
		Release();
		LoadCurrScene();
	}

	if (InputManager::InputKeyDown(VK_F3))
	{
		Release();
		LoadNextScene();
	}
	if (InputManager::InputKeyDown(VK_F1))
	{
		Release();
		LoadPrevScene();
	}

	if (InputManager::InputKeyDown(VK_F4))
	{
		Release();
		LoadBossScene();
	}
}

void SceneObjectManager::Render()
{
	if (m_CartoonOn == true)
	{
		DrawCartoon();
		return;
	}

	if (m_BossStage == true)
	{
		_GraphicEngine->DrawBitmap(m_pActiveScene->m_BackGroundBitmap[1], 0, 0);

		SpriteData data;
		data.m_BitmapPos = D2D1::Point2F(m_BossStartIndex % 5 * 1920, m_BossStartIndex / 5 * 1080);
		data.m_BitmapSize = D2D1::SizeF(1920, 1080);
		data.m_Pivot = D2D1::Point2F(1920 / 2, 1080 / 2);

		_GraphicEngine->DrawBitmap(D2D1::Point2F(1920 / 2, 1080 / 2), data, m_BossStartBitmap, data.m_BitmapSize, 1.0f);
	}
	else if (m_EndingCredit == true)
	{
		
		if (m_EndingCreditStart == false)
		{
			_GraphicEngine->DrawBitmap(m_pActiveScene->m_BackGroundBitmap[0], 0, 0);
		}
		else
		{
			_GraphicEngine->m_pRenderTarget->Clear(D2D1::ColorF(D2D1::ColorF::Black, 1));

			SpriteData data;
			data.m_BitmapPos = D2D1::Point2F(m_EndingCreditOffset, 0);
			data.m_BitmapSize = D2D1::SizeF(1920, 1080);
			data.m_Pivot = D2D1::Point2F(1920 / 2, 1080 / 2);

			_GraphicEngine->DrawBitmap(D2D1::Point2F(1920 / 2, 1080 / 2), data, m_EndingCreditImage, data.m_BitmapSize, 1.0f);
		}
	}
	else
	{
		m_pActiveScene->Render();

		if (m_pActiveScene->m_IsLoadFinish == true)
		{
			for (int y = 0; y < m_pActiveScene->m_Data.MapSize.height; ++y)
			{
				for (int x = 0; x < m_pActiveScene->m_Data.MapSize.width; ++x)
				{
					int index = m_pActiveScene->m_Data.MapSize.width * y + x;
					if (m_pActiveScene->m_Data.Attribute[index] == (int)TileType::PLATFORM || m_pActiveScene->m_Data.Attribute[index] == (int)TileType::FLOOR)
					{
						_GraphicEngine->DrawBitmap(m_pActiveScene->m_TileBitmap, D2D1::Point2F(m_pActiveScene->m_Data.Index[index].x * TILE_SIZE, m_pActiveScene->m_Data.Index[index].y * TILE_SIZE),
							D2D1::SizeF(TILE_SIZE, TILE_SIZE), D2D1::Point2F(TILE_SIZE * x, TILE_SIZE * y));
					}
				}
			}

			if (m_IsTutorial == true)
			{
				_GraphicEngine->DrawBitmap(m_TutorialImage[m_TutorialIndex], 0, 0, 1);
			}
		}
	}
}

void SceneObjectManager::PostRender()
{
	//우선 순위를 둔 렌더이다.
   /*
	   1.탄환소모 ui
	   2.오버로딩 배경 ui
	   3.오버로딩 에너지 ui
	   4.오버로딩 테두리 ui
	   5.강화창 ui
	   6.강화 카드 ui
	   7.사거리 증가 ui
	   8.데미지 증가 ui
	   9.오버로딩 증가 ui
	   10.공격속도 증가 ui
	   11.발사체 개수 증가 ui
	   12.투명한 뒷배경  ui
	   13.스탯창 패널  ui
   */
	if (m_CartoonOn == false && m_pActiveScene->m_IsLoadFinish == true && m_CurrIndex != 0 && m_CurrIndex != 15 && m_CurrIndex != 16)
	{
		UIManager::GetInstance()->DrawHealthPannel
		(
			m_pPlayerObject->GetComponent<Player>()->m_MAX_HP
		);
		UIManager::GetInstance()->DrawBossPannel(100, 50, 3);
		UIManager::GetInstance()->DrawBulletPannel(80, 30);
		UIManager::GetInstance()->DrawSelectPannel();
		UIManager::GetInstance()->DrawStatusPannel(234, 122);
		UIManager::GetInstance()->DrawOverloadingEffectPannel(137, 100);
	}

	for (auto spr : UIManager::GetInstance()->m_OverloadingEffectSprite)
	{
		if (spr->IsActive())
		{
			EventExcuteSystem::GetInstance()->SendEventMessage(spr, EnumEvent::OnRender);
			EventExcuteSystem::GetInstance()->Update();
		}
	}
}

void SceneObjectManager::Release()
{
	PrevLoadScene();

	if (_GameManager->m_IsGameOver == false)
	{
		if (m_pActiveScene->m_Name._Equal(_T("tutorial"))
			|| m_pActiveScene->m_Name._Equal(_T("sector 1-4")) == true
			|| m_pActiveScene->m_Name._Equal(_T("Sector 3-4")) == true)
		{
			m_CartoonOn = true;
		}
		else
		{
			m_CartoonOn = false;
		}
	}

	m_pPlayerObject->GetComponent<Player>()->m_pHitCollider->m_Target.clear();
	m_pPlayerObject->GetComponent<Player>()->m_Gun->GetComponent<Weapon>()->m_CurrBulletIndex = -1;
	m_pPlayerObject->GetComponent<MovePlayer>()->m_JumpCollider->m_Target.clear();

	for (auto enemy : m_pEnemy)
	{
		ObjectManager::GetInstance()->Destroy(&enemy);
	}
	m_pEnemy.clear();

	DeleteBoss();

	if (_GameManager->m_IsGameOver == false)
	{
		if (m_pActiveScene->m_Name._Equal(_T("Boss")) == true)
		{
			DeletePlayer();
			m_pActiveScene->m_IsLoadFinish = true;
			_GameManager->m_CurrentState = 1;
		}
	}

	EventExcuteSystem::GetInstance()->DeleteAllMessage();
	_GameManager->m_CurrentState = 0;
}

void SceneObjectManager::LoadNextScene()
{
	m_CurrIndex = LoadSceneManager::GetInstance()->GetSceneAt();

	if (m_CurrIndex >= LoadSceneManager::GetInstance()->SceneMax() - 1)
		m_CurrIndex = -1;

	LoadSceneManager::GetInstance()->LoadScene(m_CurrIndex + 1);
	m_CurrIndex++;
	SetActiveScene(LoadSceneManager::GetInstance()->GetActiveScene());

	for (int i = 0; i < 6; i++)
	{
		GameManager::m_pBackgroundChannel[i]->setPaused(true);
	}
	if (m_CurrIndex <= 1 || m_pActiveScene->m_Name._Equal(_T("Ending")))
	{		
		GameManager::m_pBackgroundChannel[5]->setPaused(false);
		GameManager::m_BgmIndex = 5;
	}
	else if (m_CurrIndex <= 4)
	{
		GameManager::m_pBackgroundChannel[0]->setPaused(false);
		GameManager::m_BgmIndex = 0;
	}
	else if (m_CurrIndex <= 7)
	{
		GameManager::m_pBackgroundChannel[1]->setPaused(false);
		GameManager::m_BgmIndex = 1;
	}
	else if (m_CurrIndex <= 10)
	{
		GameManager::m_pBackgroundChannel[2]->setPaused(false);
		GameManager::m_BgmIndex = 2;
	}
	else if (m_CurrIndex <= 13)
	{
		GameManager::m_pBackgroundChannel[3]->setPaused(false);
		GameManager::m_BgmIndex = 3;
	}
	else if (m_pActiveScene->m_Name._Equal(_T("Boss")))
	{
		GameManager::m_pBackgroundChannel[4]->setPaused(false);
		GameManager::m_BgmIndex = 4;
		m_BossStage = true;
	}

	PostLoadScene();

}

void SceneObjectManager::LoadPrevScene()
{
	m_CurrIndex = LoadSceneManager::GetInstance()->GetSceneAt();

	if (m_CurrIndex < 0)
	{
		m_CurrIndex = LoadSceneManager::GetInstance()->SceneMax();
	}

	LoadSceneManager::GetInstance()->LoadScene(m_CurrIndex - 1);
	m_CurrIndex--;
	SetActiveScene(LoadSceneManager::GetInstance()->GetActiveScene());

	PostLoadScene();

}

void SceneObjectManager::LoadCurrScene()
{
	m_CurrIndex = LoadSceneManager::GetInstance()->GetSceneAt();
	LoadSceneManager::GetInstance()->LoadScene(LoadSceneManager::GetInstance()->GetSceneAt());
	SetActiveScene(LoadSceneManager::GetInstance()->GetActiveScene());

	PostLoadScene();

}

void SceneObjectManager::LoadBossScene()
{
	m_CurrIndex = 14;
	LoadSceneManager::GetInstance()->LoadScene(m_CurrIndex);
	SetActiveScene(LoadSceneManager::GetInstance()->GetActiveScene());

	PostLoadScene();
}

void SceneObjectManager::CreatePlayer()
{
	if (m_pPlayerObject == nullptr)
	{
		m_pPlayerObject = ObjectFactory::CreateObject<GameObject>();
		_GameManager->m_pPlayer = m_pPlayerObject;

		Player* pPlayer = m_pPlayerObject->AddComponent<Player>();
		m_pPlayerObject->AddComponent<MovePlayer>()->m_pPlayer = pPlayer;
	}
}

void SceneObjectManager::DeletePlayer()
{
	if (m_pPlayerObject != nullptr)
	{
		ObjectManager::GetInstance()->Destroy(&m_pPlayerObject);
		m_pPlayerObject = nullptr;
	}
}

void SceneObjectManager::CreateGateAndSwitch()
{
	if (m_Gate == nullptr)
	{
		m_Gate = ObjectFactory::CreateObject<GameObject>();
		m_Gate->SetStringTag("Gate");
		m_Gate->AddComponent<Gate>();
	}
	if (m_Switch == nullptr)
	{
		m_Switch = ObjectFactory::CreateObject<GameObject>();
		m_Switch->AddComponent<Switch>()->m_Gate = m_Gate;
		m_Switch->SetStringTag("Switch");
	}
}

void SceneObjectManager::DeleteGateAndSwitch()
{
	if (m_Gate != nullptr)
	{
		ObjectManager::GetInstance()->Destroy(&m_Gate);
		m_Gate = nullptr;
	}
	if (m_Switch != nullptr)
	{
		ObjectManager::GetInstance()->Destroy(&m_Switch);
		m_Switch = nullptr;
	}
}

void SceneObjectManager::PrevLoadScene()
{
	DeletePlatformAndFloor();
	InitBullets();
}

void SceneObjectManager::InitBullets()
{
	for (GameObject* bullet : m_pPlayerObject->GetComponent<Player>()->m_Gun->GetComponent<Weapon>()->m_Bullets)
	{
		bullet->GetComponent<Bullet>()->Init();
		bullet->GetComponent<Collider>()->SetActive(false);
	}
}

void SceneObjectManager::CreatePlatformAndFloor()
{
	m_Platform = ObjectFactory::CreateObject<GameObject>();
	m_Platform->SetStringTag("Platform");

	m_Floor = ObjectFactory::CreateObject<GameObject>();
	m_Floor->SetStringTag("Floor");
}

void SceneObjectManager::DeletePlatformAndFloor()
{
	if (m_Platform != nullptr)
	{
		ObjectManager::GetInstance()->Destroy(&m_Platform);
		m_Platform = nullptr;
	}

	if (m_Floor != nullptr)
	{
		ObjectManager::GetInstance()->Destroy(&m_Floor);
		m_Floor = nullptr;
	}
}

void SceneObjectManager::CreateBoss()
{
	if (m_Boss == nullptr)
	{
		m_Boss = ObjectFactory::CreateObject<GameObject>();
		m_Boss->AddComponent<Boss>();
		m_Boss->SetPos(D2D1::Point2F(1920 / 2.f, 1080 / 3.f));
	}
	else
	{
		m_Boss->GetComponent<Boss>()->Init();
	}
}

void SceneObjectManager::DeleteBoss()
{
	if (m_Boss != nullptr)
	{
		ObjectManager::GetInstance()->Destroy(&m_Boss);
		m_Boss = nullptr;
	}
}

void SceneObjectManager::CheckTile()
{
	bool isCheckSwitch = false;
	bool isCheckGate = false;

	for (int y = 0; y < m_pActiveScene->m_Data.MapSize.height; ++y)
	{
		bool isCheckPlatform = false;
		bool isCheckFloor = false;
		int firstPlatformPointX = -1;
		int firstFloorPointX = -1;

		for (int x = 0; x < m_pActiveScene->m_Data.MapSize.width; ++x)
		{
			int index = m_pActiveScene->m_Data.MapSize.width * y + x;

			if (m_pActiveScene->m_Data.Attribute[index] == (int)TileType::PLATFORM)
			{
				if (isCheckPlatform == false)
				{
					isCheckPlatform = true;
					firstPlatformPointX = x;
				}
			}
			else if (m_pActiveScene->m_Data.Attribute[index] == (int)TileType::FLOOR)
			{
				if (isCheckFloor == false)
				{
					isCheckFloor = true;
					firstFloorPointX = x;
				}
			}
			else if (m_pActiveScene->m_Data.Attribute[index] == (int)TileType::SWITCH)
			{
				if (isCheckSwitch == false)
				{
					BoxCollider* collider = m_Switch->AddComponent<BoxCollider>();
					m_Switch->GetComponent<Switch>()->m_Collider = collider;
					m_Switch->SetPos(D2D1::Point2F(TILE_SIZE * (x + 1.5f), TILE_SIZE * (y + 3.5f)));

					collider->SetRect(D2D1::Point2F(0, 0), D2D1::SizeF(TILE_SIZE * 2.5f, TILE_SIZE * 5));
#ifdef _DEBUG
					collider->SetDebugDraw();
#endif
					collider->SetStringTag("Switch");
					collider->AddIgnoreTag("Weapon");
					collider->SetTrigger(true);

					isCheckSwitch = true;
					m_Switch->GetComponent<Switch>()->Init();
				}
			}
			else if (m_pActiveScene->m_Data.Attribute[index] == (int)TileType::GATE)
			{
				if (isCheckGate == false)
				{
					BoxCollider* collider = m_Gate->AddComponent<BoxCollider>();
					m_Gate->GetComponent<Gate>()->m_Collider = collider;
					m_Gate->SetPos(D2D1::Point2F(TILE_SIZE * (x + 1.5f), TILE_SIZE * (y + 3.3f)));
					collider->SetRect(D2D1::Point2F(0, 0), D2D1::SizeF(TILE_SIZE * 2.5f, TILE_SIZE * 5));
#ifdef _DEBUG
					collider->SetDebugDraw();
#endif
					collider->SetStringTag("Gate");
					collider->AddIgnoreTag("Weapon");
					collider->SetTrigger(true);

					m_Gate->GetComponent<Gate>()->Init();

					isCheckGate = true;
				}
			}
			else if (m_pActiveScene->m_Data.Attribute[index] == (int)TileType::SPAWN)
			{
				D2D1_POINT_2F m_PlayerSpawnPoint = D2D1::Point2F(TILE_SIZE * x + TILE_SIZE / 2, TILE_SIZE * y + TILE_SIZE / 2);
				m_pPlayerObject->SetPos(m_PlayerSpawnPoint);
			}
			else if (m_pActiveScene->m_Data.Attribute[index] == (int)TileType::BEAR)
			{
				D2D1_POINT_2F spawnPoint = D2D1::Point2F(TILE_SIZE * x + TILE_SIZE / 2, TILE_SIZE * y + TILE_SIZE / 2);
				m_pEnemy.push_back(ObjectFactory::CreateObject<GameObject>());
				//m_pEnemy[0]->AddComponent<DashMeleeMonster>();

				m_pEnemy[m_pEnemy.size() - 1]->AddComponent<MeleeMonster>();
				m_pEnemy[m_pEnemy.size() - 1]->SetPos(spawnPoint);
			}
			else if (m_pActiveScene->m_Data.Attribute[index] == (int)TileType::BIRD)
			{
				D2D1_POINT_2F spawnPoint = D2D1::Point2F(TILE_SIZE * x + TILE_SIZE / 2, TILE_SIZE * y + TILE_SIZE / 2);
				m_pEnemy.push_back(ObjectFactory::CreateObject<GameObject>());
				//m_pEnemy[0]->AddComponent<DashMeleeMonster>();
				m_pEnemy[m_pEnemy.size() - 1]->AddComponent<JumpMeleeMonster>();
				m_pEnemy[m_pEnemy.size() - 1]->SetPos(spawnPoint);
			}
			else if (m_pActiveScene->m_Data.Attribute[index] == (int)TileType::TURTLE)
			{
				D2D1_POINT_2F spawnPoint = D2D1::Point2F(TILE_SIZE * x + TILE_SIZE / 2, TILE_SIZE * y + TILE_SIZE / 2);
				m_pEnemy.push_back(ObjectFactory::CreateObject<GameObject>());
				//m_pEnemy[0]->AddComponent<DashMeleeMonster>();
				m_pEnemy[m_pEnemy.size() - 1]->AddComponent<DashMeleeMonster>();
				m_pEnemy[m_pEnemy.size() - 1]->SetPos(spawnPoint);
			}
			else if (m_pActiveScene->m_Data.Attribute[index] == (int)TileType::TANK)
			{
				D2D1_POINT_2F spawnPoint = D2D1::Point2F(TILE_SIZE * x + TILE_SIZE / 2, TILE_SIZE * y + TILE_SIZE / 2);
				m_pEnemy.push_back(ObjectFactory::CreateObject<GameObject>());
				//m_pEnemy[0]->AddComponent<DashMeleeMonster>();
				m_pEnemy[m_pEnemy.size() - 1]->AddComponent<RangedMonster>();
				m_pEnemy[m_pEnemy.size() - 1]->SetPos(spawnPoint);
			}

			if (m_pActiveScene->m_Data.Attribute[index] != (int)TileType::PLATFORM)
			{
				if (isCheckPlatform == true && firstPlatformPointX != -1)
				{
					isCheckPlatform = false;

					BoxCollider* collider = m_Platform->AddComponent<BoxCollider>();
					collider->SetRect(D2D1::Point2F(TILE_SIZE * (firstPlatformPointX + ((x - 1) - firstPlatformPointX) / 2.f) + TILE_SIZE / 2, TILE_SIZE * y + TILE_SIZE / 2),
						D2D1::SizeF(TILE_SIZE * (x - firstPlatformPointX), TILE_SIZE));
					collider->SetTrigger(false);
					collider->SetStringTag("Platform");
					collider->AddIgnoreTag("Weapon");
#ifdef _DEBUG
					collider->SetDebugDraw();
#endif
				}
			}
			if (m_pActiveScene->m_Data.Attribute[index] != (int)TileType::FLOOR)
			{
				if (isCheckFloor == true && firstFloorPointX != -1)
				{
					isCheckFloor = false;

					BoxCollider* collider = m_Floor->AddComponent<BoxCollider>();
					collider->SetRect(D2D1::Point2F(TILE_SIZE * (firstFloorPointX + ((x - 1) - firstFloorPointX) / 2.f) + TILE_SIZE / 2, TILE_SIZE * y + TILE_SIZE / 2),
						D2D1::SizeF(TILE_SIZE * (x - firstFloorPointX), TILE_SIZE));
					collider->SetTrigger(false);
					collider->SetStringTag("Floor");
#ifdef _DEBUG
					collider->SetDebugDraw();
#endif
					collider->AddIgnoreTag("Weapon");
				}
			}
		}

		if (isCheckPlatform == true)
		{
			isCheckPlatform = false;
			BoxCollider* collider = m_Platform->AddComponent<BoxCollider>();
			collider->SetRect(D2D1::Point2F(TILE_SIZE * (firstPlatformPointX + ((m_pActiveScene->m_Data.MapSize.width - 1) - firstPlatformPointX) / 2.f) + TILE_SIZE / 2, TILE_SIZE * y + TILE_SIZE / 2),
				D2D1::SizeF(TILE_SIZE * (m_pActiveScene->m_Data.MapSize.width - firstPlatformPointX), TILE_SIZE));
			collider->SetTrigger(false);
			collider->SetStringTag("Platform");
#ifdef _DEBUG
			collider->SetDebugDraw();
#endif
			collider->AddIgnoreTag("Weapon");
		}

		if (isCheckFloor == true)
		{
			isCheckFloor = false;
			BoxCollider* collider = m_Floor->AddComponent<BoxCollider>();
			collider->SetRect(D2D1::Point2F(TILE_SIZE * (firstFloorPointX + ((m_pActiveScene->m_Data.MapSize.width - 1) - firstFloorPointX) / 2.f) + TILE_SIZE / 2, TILE_SIZE * y + TILE_SIZE / 2),
				D2D1::SizeF(TILE_SIZE * (m_pActiveScene->m_Data.MapSize.width - firstFloorPointX), TILE_SIZE));
			collider->SetTrigger(false);
			collider->SetStringTag("Floor");
#ifdef _DEBUG
			collider->SetDebugDraw();
#endif
			collider->AddIgnoreTag("Weapon");
		}
	}
}

void SceneObjectManager::NextCartoon()
{
	if (InputManager::InputKeyDown(VK_LBUTTON))
	{
		m_CartoonCutIndex++;

		if (m_CartoonCutIndex >= 4)
		{
			m_CartoonOn = false;
			m_CartoonCutIndex = 0;
			m_CartoonIndex++;
			m_pActiveScene->m_IsLoadFinish = true;
		}
	}
}

void SceneObjectManager::DrawCartoon()
{
	m_pActiveScene->m_IsLoadFinish = true;

	_GraphicEngine->m_pRenderTarget->Clear(D2D1::ColorF(D2D1::ColorF::Black, 1));

	for (int i = 0; i < m_CartoonCutIndex + 1; i++)
	{
		if (m_CartoonImage[m_CartoonIndex][i] != nullptr)
		{
			SpriteData data;
			data.m_BitmapPos = D2D1::Point2F(0, 0);
			data.m_BitmapSize = m_CartoonImage[m_CartoonIndex][i]->GetSize();
			data.m_Pivot = D2D1::Point2F(0, 0);

			_GraphicEngine->DrawBitmap(data.m_BitmapPos, data, m_CartoonImage[m_CartoonIndex][i], data.m_BitmapSize, 1.0f);
		}
	}
}

void SceneObjectManager::NextBossAnimationIndex()
{
	if (m_BossStartIndex >= 30)
	{
		m_BossStage = false;
		m_pActiveScene->m_IsLoadFinish = true;
	}
	else
	{
		m_pActiveScene->m_IsLoadFinish = false;
	}

	m_BossStartTime += _FTime->GetObjectFixedDeltaTimeSec();
	if (m_BossStartTime >= 2.0f / 30.f)
	{
		m_BossStartIndex++;
		m_BossStartTime = 0;
	}
}

void SceneObjectManager::InTitle()
{
	if (m_GameStart == true && m_CartoonOn == false)
	{
		LoadNextScene();
		UIManager::GetInstance()->Initialize();
		return;
	}

	if (InputManager::InputKeyDown(VK_LBUTTON))
	{
		if (m_CartoonOn == false && _FAudio->FIsPlaying(OVERLOAD_START_SFX) == false)
		{
			_FAudio->FPlaySound(OVERLOAD_START_SFX);
			m_pActiveScene->m_ChangeState = true;
			m_pActiveScene->m_ChangeFinish = false;
		}
	}

	if (m_pActiveScene->m_ChangeFinish == true)
	{
		m_GameStart = true;
		m_CartoonOn = true;
	}
}

void SceneObjectManager::InEnding()
{
	m_EndingCredit = true;

	m_EndingTimer += _FTime->GetEngineFixedDeltaTimeSec();

	if (m_EndingCreditStart == false && m_EndingTimer >= 3.0)
	{
		m_EndingCreditStart = true;
	}

	if (m_EndingCreditFinish == false && m_EndingCreditStart == true)
	{
		m_EndingCreditOffset++;
		if (m_EndingCreditOffset >= 620)
		{
			m_EndingCreditFinish = true;
		}
	}

	if (m_EndingCreditFinish == true && InputManager::InputKeyDown(VK_LBUTTON))
	{
		if (_FAudio->FIsPlaying(OVERLOAD_END_SFX) == false)
		{
			_FAudio->FPlaySound(OVERLOAD_END_SFX);
			m_pActiveScene->m_ChangeState = true;
			m_pActiveScene->m_ChangeFinish = false;
			m_EndingCredit = false;
			_GameManager->m_IsGameOver = false;
			m_CurrIndex = 0;
			LoadSceneManager::GetInstance()->LoadScene(0);
			SetActiveScene(LoadSceneManager::GetInstance()->GetActiveScene());
			InitializeVariable();
		}
	}
}
