#pragma once

#define TILE_SIZE 32

class SceneObjectManager : public SingletonManager<SceneObjectManager>
{
private:
	Scene* m_pActiveScene;
	GameObject* m_Platform;
	GameObject* m_Floor;
	std::vector<GameObject*> m_pEnemy;
	GameObject* m_Gate;
	GameObject* m_Switch;

	int			m_CartoonIndex = 0;
	int			m_CartoonCutIndex = 0;
	int			m_EndingCreditOffset = 0;

	ID2D1Bitmap* m_TutorialImage[2];
	bool	m_IsTutorial = false;
	int		m_TutorialIndex = 0;
	
	ID2D1Bitmap* m_EndingCreditImage;
	ID2D1Bitmap* m_CartoonImage[5][4];

	int         m_BossStartIndex = 0;
	float       m_BossStartTime = 0;

	float		m_EndingTimer = 0;

	ID2D1Bitmap* m_BossStartBitmap;
	bool            m_BossStage = false;
	bool            m_EndingCredit = false;
	bool			m_EndingCreditFinish = false;
	bool			m_EndingCreditStart = false;
public:
	SceneObjectManager();
	~SceneObjectManager();

	bool		m_GameStart = false;
	bool        m_CartoonOn = false;
	int         m_CurrIndex = 0;
	int         m_DeadSceneNum = 0;
	GameObject* m_Boss;
	GameObject* m_pPlayerObject;
	GameObject* m_pOverloading;

	void    SetDeadScene() { m_DeadSceneNum = m_CurrIndex; }
	void    SetActiveScene(Scene* _pScene) { m_pActiveScene = _pScene; }
	Scene* GetActiveScene() { return m_pActiveScene; }

	void LoadImages();
	void DeleteImages();

	void InitializeVariable();
	void Update();
	void Render();
	void PostRender();

	void Release();
	void LoadNextScene();
	void LoadPrevScene();
	void LoadCurrScene();
	void LoadBossScene();

	void CreatePlayer();
	void DeletePlayer();

	void CreateGateAndSwitch();
	void DeleteGateAndSwitch();

	void PostLoadScene();
	void PrevLoadScene();

	void InitBullets();

	void CreatePlatformAndFloor();
	void DeletePlatformAndFloor();

	void CreateBoss();
	void DeleteBoss();

	void CheckTile();

	void NextCartoon();
	void DrawCartoon();

	void NextBossAnimationIndex();

	void InTitle();
	void InEnding();
};

