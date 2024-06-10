#pragma once
class UIManager;

class GameManager : public SingletonManager<GameManager>
{
private:
	EventExcuteSystem* pEventSystem;
	SceneObjectManager* pSceneObjectManager;
private:
	//D2D1_POINT_2F GetMousePos() { return mousePos; };
	//void SetMousePos(D2D1_POINT_2F _v) { mousePos = _v; };
	void BeginRender();
	HRESULT EndRender();
public:	
	static FMOD::Channel* m_pBackgroundChannel[6];
	static int	m_BgmIndex;

	GameObject* m_pPlayer;
	LoadSceneManager* pSceneManager;
	D2D1_POINT_2F mousePos;
	// 0 -> 순행, 1 -> 역행
	int			m_CurrentState = 0;
	bool		m_IsGameOver = false;

	void Initialize();

	void MainLoop();

	void Render();
	void Update();

};

