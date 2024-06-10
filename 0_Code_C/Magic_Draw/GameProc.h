#pragma once
enum class Stage
{
	title, loading, tutorial, stage, stageClear, introBoss, boss, gameover, STAGE_COUNT
};

extern int g_Score;

extern Stage StageInfo;
extern int WaveCount;

void CreateEngine();
void DataLoading();
void Title();
void Loading();
void Tutorial();
void NormalStage();
void IntroBossStage();
void BossStage();
void StageClear();
void GameOver();
void Release();


extern HWND g_hWnd;
extern HWND g_ReplayBtn;
extern HINSTANCE hInst;