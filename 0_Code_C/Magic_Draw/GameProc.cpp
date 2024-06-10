#include "../RRYEngine/framework.h"
#include "ynMMedia.h"			   //예나 미디어 헤더.★
#include "UnitManager.h"
#include "System.h"
#include <vector>
#include "MagicManager.h"
#include "Input.h"
#include "SoundManager.h"
#include "GameProc.h"
#include "resource.h"

Stage StageInfo = Stage::title;
int WaveCount = 0;
int g_Score = 0;

HWND g_ReplayBtn;
extern HANDLE buttonImage;

void CreateEngine()
{
	::CreateEngine(g_hWnd, System::GetInstance()->m_ScreenSize.cx, System::GetInstance()->m_ScreenSize.cy);
	srand(GetTickCount64());
}

void DataLoading()
{
	System::GetInstance()->DataLoading();
	UnitManager::GetInstance()->CreateInitEnemy();
	UnitManager::GetInstance()->CreateHero();
	UnitManager::GetInstance()->CreateBoss();
	MagicManager::GetInstance()->Initialize();
	SoundManager::GetInstance()->LoadData();
}

void Title()
{
	ShowWindow(g_ReplayBtn, SW_HIDE);

	UnitManager::GetInstance()->Init();
	MagicManager::GetInstance()->Init();

	g_Score = 0;
	WaveCount = 0;

	System::GetInstance()->SystemUpdate();
	System::GetInstance()->RenderAll();
}

void Loading()
{
	ShowWindow(g_ReplayBtn, SW_HIDE);

	UnitManager::GetInstance()->Init();
	MagicManager::GetInstance()->Init();

	static float StartTime = 0;

	System::GetInstance()->SystemUpdate();

	StartTime += System::GetInstance()->m_DTime;

	System::GetInstance()->RenderAll();

	if (StartTime > 2)
	{
		StartTime = 0;
		StageInfo = Stage::tutorial;
		SoundManager::GetInstance()->PlayBackGround();
	}
}

void Tutorial()
{
	ShowWindow(g_ReplayBtn, SW_HIDE);

	System::GetInstance()->SystemUpdate();

	System::GetInstance()->RenderAll();
}

void NormalStage()
{
	ShowWindow(g_ReplayBtn, SW_HIDE);

	System::GetInstance()->SystemUpdate();

	MagicManager::GetInstance()->CreateMagic();
	MagicManager::GetInstance()->MoveMagic();

	UnitManager::GetInstance()->CheckCreateTime(System::GetInstance()->m_DTime);
	UnitManager::GetInstance()->MoveMonster();

	System::GetInstance()->RenderAll();

	UnitManager::GetInstance()->CheckHeroCollision();
	MagicManager::GetInstance()->CheckCollision();
}

void IntroBossStage()
{
	ShowWindow(g_ReplayBtn, SW_HIDE);

	static float StartTime = 0;

	if (StartTime == 0)
		SoundManager::GetInstance()->PlayOnce(SoundList::BOSS_COMMING);

	System::GetInstance()->SystemUpdate();
	MagicManager::GetInstance()->Init();

	StartTime += System::GetInstance()->m_DTime;

	if (StartTime > 1)
		UnitManager::GetInstance()->MoveBoss();

	System::GetInstance()->RenderAll();

	if (StartTime > 2)
	{
		StageInfo = Stage::boss;
		StartTime = 0;
	}
}

void BossStage()
{
	ShowWindow(g_ReplayBtn, SW_HIDE);

	System::GetInstance()->SystemUpdate();

	MagicManager::GetInstance()->CreateMagic();
	MagicManager::GetInstance()->MoveMagic();

	UnitManager::GetInstance()->MoveBoss();
	UnitManager::GetInstance()->MoveMonster();

	System::GetInstance()->RenderAll();

	UnitManager::GetInstance()->CheckHeroCollision();
	MagicManager::GetInstance()->CheckCollision();
}

void StageClear()
{
	System::GetInstance()->SystemUpdate();

	UnitManager::GetInstance()->CheckCreateTime(System::GetInstance()->m_DTime);

	System::GetInstance()->RenderAll();

	MagicManager::GetInstance()->CreateMagic();
	MagicManager::GetInstance()->MoveMagic();

	MagicManager::GetInstance()->CheckCollision();

	MagicManager::GetInstance()->Init();

	ShowWindow(g_ReplayBtn, SW_SHOW);
}

void GameOver()
{
	System::GetInstance()->SystemUpdate();

	System::GetInstance()->RenderAll();

	ShowWindow(g_ReplayBtn, SW_SHOW);
}

void Release()
{
	UnitManager::GetInstance()->Release();
	System::GetInstance()->DataRelease();
	MagicManager::GetInstance()->Release();
	SoundManager::GetInstance()->ReleaseData();

	DestroyWindow(g_ReplayBtn);
	g_ReplayBtn = nullptr;

	DeleteObject(buttonImage);

	buttonImage = nullptr;

	ReleaseEngine();
}