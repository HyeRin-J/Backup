#include "GamePCH.h"
#include "GameManager.h"

FMOD::Channel* GameManager::m_pBackgroundChannel[6];
int GameManager::m_BgmIndex = 0;

void GameManager::Initialize()
{
	pEventSystem = EventExcuteSystem::GetInstance();
	pSceneObjectManager = SceneObjectManager::GetInstance();
	pSceneManager = LoadSceneManager::GetInstance();
	pSceneManager->Init();

	pSceneManager->CreateScene(_T("title"), _T("None"));

	std::wstring mapPath = PATH_MAP_UNI;
	mapPath.append(_T("data/tutorial.json"));
	pSceneManager->LoadData(mapPath.c_str());

	for (int j = 1; j <= 3; j++)
	{
		for (int i = 1; i <= 4; i++)
		{
			WCHAR path[256];
			swprintf_s(path, _T("%sdata/sector %d-%d.json"), PATH_MAP_UNI, j, i);

			mapPath = path;

			pSceneManager->LoadData(mapPath.c_str());
		}
	}

	WCHAR path[256];
	swprintf_s(path, _T("%sdata/Boss.json"), PATH_MAP_UNI);
	pSceneManager->LoadData(path);

	pSceneManager->CreateScene(_T("Ending"), _T("None"));
	pSceneManager->CreateScene(_T("GameOver"), _T("None"));

	if (pSceneManager->LoadScene(0))
	{
		pSceneObjectManager->SetActiveScene(pSceneManager->GetActiveScene());
		pSceneObjectManager->PostLoadScene();
	}

	//원하는 폰트 글씨체를 사용하기전 해당 폰트의 타입과 사이즈를 등록합니다
	//UIManager::GetInstance()->Initialize();

}

void GameManager::Render()
{
	BeginRender();

	pSceneObjectManager->Render();

	if (pSceneObjectManager->GetActiveScene()->m_IsLoadFinish == true && SceneObjectManager::GetInstance()->m_CartoonOn == false)
	{
		pEventSystem->SendEventMessage(EnumEvent::OnRender);
	}
	pEventSystem->Update();

	if (m_IsGameOver == false)
	{			
		pSceneObjectManager->PostRender();
	}

	float _fps = _FTime->GetFPS();

	_GraphicEngine->D2DXDrawText(20, 0, 10, 10, D2D1::ColorF::White, L"FPS:");
	_GraphicEngine->D2DXDrawText(50, 0, 10, 10, D2D1::ColorF::White, L"%d", (short)_fps);
#ifdef _DEBUG

	//float _dtime = _FTime->GetDeltaTimeMS() / 1000.f;

	float _Dtime = _FTime->GetRenderFixedDeltaTimeSec(); // real Delta time

	float _ObjectsScale = _FTime->m_ObjectDeltaTimeScale;
	float _ObjectsDtime = _FTime->GetObjectFixedDeltaTimeSec();

	//UIManager::GetInstance()->UIDrawStyle_0();

	//UIManager::GetInstance()->UIDrawStyle_0();


	_GraphicEngine->D2DXDrawText(20, 50, 10, 10, D2D1::ColorF::White, L"DeltaTime:");
	_GraphicEngine->D2DXDrawText(80, 50, 10, 10, D2D1::ColorF::White, L"%.3f", _Dtime);

	_GraphicEngine->D2DXDrawText(20, 65, 10, 10, D2D1::ColorF::White, L"Objects time scale:");
	_GraphicEngine->D2DXDrawText(140, 65, 10, 10, D2D1::ColorF::White, L"%.3f", _ObjectsScale);
	_GraphicEngine->D2DXDrawText(20, 75, 10, 10, D2D1::ColorF::White, L"Objects DeltaTime:");
	_GraphicEngine->D2DXDrawText(140, 75, 10, 10, D2D1::ColorF::White, L"%.3f", _ObjectsDtime);



	mousePos = InputManager::GetMousePos(AppManager::GetInstance()->m_hwnd);
	//mouse draw
	_GraphicEngine->D2DXDrawText(20, 100, 10, 10, D2D1::ColorF::White, L"mouse : %.3f", mousePos.x);
	_GraphicEngine->D2DXDrawText(20, 150, 10, 10, D2D1::ColorF::White, L"mouse : %.3f", mousePos.y);
#endif

	EndRender();
}


void GameManager::BeginRender()
{
	_GraphicEngine->BeginRender();

	_GraphicEngine->m_pRenderTarget->SetTransform(D2D1::Matrix3x2F::Identity());

	//_GraphicEngine->m_pRenderTarget->Clear(D2D1::ColorF(D2D1::ColorF::DarkGray));
}

HRESULT GameManager::EndRender()
{
	HRESULT hr = S_OK;

	if (SUCCEEDED(hr))
	{
		_GraphicEngine->EndRender();
	}

	if (hr == D2DERR_RECREATE_TARGET)
	{
		hr = S_OK;
		//_GraphicEngine->DiscardDeviceResources();
	}

	return hr;
}

void GameManager::Update()
{
	//InputManager::KeyUpdate();
	pSceneObjectManager->Update();
	Scene* pActiveScene = pSceneObjectManager->GetActiveScene();

	_FAudio->FUpdate();


	if (InputManager::InputKeyDown(VK_ESC))
	{
		SendMessage(AppManager::GetInstance()->m_hwnd, WM_DESTROY, 0, 0);
	}

	if (m_IsGameOver == true)
	{
		if (SceneObjectManager::GetInstance()->m_pPlayerObject != nullptr)
		{
			if (SceneObjectManager::GetInstance()->m_pPlayerObject->GetComponent<Player>()->m_DieSprite[_GameManager->m_CurrentState]->m_IsFinish == true)
			{
				pSceneObjectManager->Release();
				if (pSceneObjectManager->m_CurrIndex != 16 && pSceneManager->LoadScene(16))
				{
					pSceneObjectManager->SetActiveScene(pSceneManager->GetActiveScene());
					pSceneObjectManager->m_CurrIndex = 16;
					pSceneObjectManager->PostLoadScene();
				}
			}
		}
	}

	if (m_IsGameOver == false && pActiveScene->m_Name._Equal(_T("title")) == false && pActiveScene->m_Name._Equal(_T("Ending")) == false && pActiveScene->m_IsLoadFinish == true)
	{
		if (UIManager::GetInstance()->m_IsUIActive == false)
		{
			if (pActiveScene->m_ChangeState == false && InputManager::InputKeyDown(VK_RBUTTON))
			{
				if (m_CurrentState == 0)
				{
					_FAudio->FPlaySound(CONVERT_0_SFX);
				}
				else
				{
					_FAudio->FPlaySound(CONVERT_1_SFX);
				}
				m_CurrentState ^= 0x01;

				pActiveScene->m_BackgroundIndex = m_CurrentState;
				pActiveScene->m_ChangeState = true;
				//(SceneObjectManager::GetInstance()->m_pPlayerObject)->GetComponent<Player>()->m_pAnimationController->SetCondition("ConversionState", true);
				if (SceneObjectManager::GetInstance()->m_pPlayerObject->GetComponent<Player>()->m_JumpState == -1 && SceneObjectManager::GetInstance()->m_pPlayerObject->GetComponent<Rigidbody>()->GetFallSpeed() == 0)
				{
					(SceneObjectManager::GetInstance()->m_pPlayerObject)->GetComponent<Player>()->m_IsConvert = true;
				}
			}
			//Debug::LogFormat("Current State : %d", m_CurrentState);

			if (m_CurrentState == 1)
			{
				_FTime->m_ObjectDeltaTimeScale = 0.6f;
			}
			else
			{
				_FTime->m_ObjectDeltaTimeScale = 1.0f;
			}
		}
		if (SceneObjectManager::GetInstance()->m_CartoonOn == false)
			pEventSystem->SendEventMessage(EnumEvent::Update);
		pEventSystem->Update();
	}
}


void GameManager::MainLoop()
{
	Update();
	Render();
}