#pragma once
class Object;
class GameObject;
class Transform;

/// <summary>
/// 컴포넌트 엔진을 만들려니
/// 컴포넌트간의 메세지를 연결 해주어야한다고 봤다
/// 아직 할 레벨은 아닌거같은데 
/// 보니깐 대충은 어케생긴지는 알거같다. 생긴것만..생긴것만..
/// </summary>
class Component : public Object
{
public:
	Component();
	virtual ~Component();
public:
	//컴포넌트는 게임오브젝트를 구분할줄을 모름.
	//그래서 게임오브젝트를 구별할 정보를 알고있어야함
	GameObject* m_GameObject;
	//현재 컴포넌트를 활성화 할것인가에 대한 변수 ex)transform , collider 등.
	bool m_IsActive = true;
public:
	void	SetActive(bool _active);
	bool	IsActive() { return m_IsActive; }

	//컴포넌트를 받은 자식 객체들이 가지고있는
	//메세지들이다 .
	//[이벤트 큐]를 이용한 메세지 처리로
	//컴포넌트들은 각자 실행할 메세지를 가지고있다.
	//오브젝트가 수동으로 호출하지 않아도 반복문 구문에 이벤트 큐 계속
	//활성화 해줌으로서 그 기능을 전부 사용하게 하게끔 하는 목록이다.

	virtual void	Awake() {}; //처음 실행할 때 한번만
	virtual void	OnEnable() {};//활성
	virtual void	Start() {};//이 객체가 처음 활성화 될때
	virtual void	FixedUpdate() {};//프레임 고정o
	virtual void	Update() {};//프레임 고정x
	virtual void	OnRender() {};//그려주는 기능
	virtual void	OnDisable() {};//비활성
	virtual void	OnDestroy() {};//객체 삭제

};

