<div align="center">
  <h1 style="border-bottom: 1px solid #21262d; color: #c9d1d9;"> 💻 Code_C++ </h1>
  <div style="font-weight: 700; font-size: 15px; text-align: center; color: #c9d1d9;"> c++로 작성한 코드 모음입니다. </div>
   <br>
</div>

<details open><summary>
<h2 style="border-bottom: 1px solid #21262d; color: #c9d1d9;"> ⭐ AnnTrophy </h2></summary>

<h3 style="border-bottom: 1px solid #21262d; color: #c9d1d9;">영상</h3>

[![AnnTrophy](http://img.youtube.com/vi/V1SOQquYiwc/0.jpg)](https://youtu.be/V1SOQquYiwc)
<h3 style="border-bottom: 1px solid #21262d; color: #c9d1d9;"> 세부 사항 </h3>
  <h5 style="border-bottom: 1px solid #21262d; color: #c9d1d9;"> 영화 테넷에서 아이디어를 착안하여, 총알을 발사했다가 다시 되돌아오는 공격 시스템을 가진 게임 </h5>
  <br>
  <ul style="display: table;  margin: auto;">
    <li>🎮 장르 : 플랫포머 </li>
    <li>📅 개발 기간 : 3주 </li>
    <li>🙋 개발 인원 : 9명    
      <ul style="border-bottom: 1px;">
        <li>프로그래머 3</li>
        <li>기획 3</li>
        <li>아트 3</li>
      </ul>
    </li>
    <li>📃 개발 환경 : Direct2D, C++, MFC(툴 제작) </li>
    <li>🛠️ 사용 기술</li>
     <ul style="border-bottom: 1px;">
        <li>Component Based Engine Framework</li>
        <li>AABB Collision check / Physics</li>
        <li>FSM(Finite-state Machine)</li>
        <li>BT(Behaviour Tree)</li>
        <li>STL</li>
      </ul>
  </ul>
데모 : 

[![Google Drive](https://img.shields.io/badge/Google%20Drive-4285F4?style=for-the-badge&logo=googledrive&logoColor=white)](https://drive.google.com/file/d/1n-ZXNZuGuirVmiB3_6Pl3WyGDqLP8sbU/view?usp=sharing)
<br>
Full Source Code :

[Engine](https://github.com/HyeRin-J/AnnTrophy)
[Tool](https://github.com/HyeRin-J/Portfolio/tree/main/3_Tool_MFC)

<h3 style="border-bottom: 1px solid #21262d; color: #c9d1d9;"> Source Code </h3>

<li>Component</li>

```cpp
Component::Component()
{
	EventExcuteSystem* eventSystem = EventExcuteSystem::GetInstance();

	std::function<void()> awake = std::bind(&Component::Awake, this);
	eventSystem->AttachEvent(this, EnumEvent::Awake, awake);

	std::function<void()> enable = std::bind(&Component::OnEnable, this);
	eventSystem->AttachEvent(this, EnumEvent::OnEnable, enable);

	std::function<void()> start = std::bind(&Component::Start, this);
	eventSystem->AttachEvent(this, EnumEvent::Start, start);

	std::function<void()> fixedUpdate = std::bind(&Component::FixedUpdate, this);
	eventSystem->AttachEvent(this, EnumEvent::FixedUpdate, fixedUpdate);

	std::function<void()> update = std::bind(&Component::Update, this);
	eventSystem->AttachEvent(this, EnumEvent::Update, update);

	std::function<void()> render = std::bind(&Component::OnRender, this);
	eventSystem->AttachEvent(this, EnumEvent::OnRender, render);

	std::function<void()> disable = std::bind(&Component::OnDisable, this);
	eventSystem->AttachEvent(this, EnumEvent::OnDisable, disable);

	std::function<void()> destroy = std::bind(&Component::OnDestroy, this);
	eventSystem->AttachEvent(this, EnumEvent::OnDestroy, destroy);
}
/// <summary>
/// 컴포넌트가 삭제 될 때
/// 붙어있는 메세지 함수들을 전부 때어내는 기능이다.
/// </summary>
Component::~Component()
{
	EventExcuteSystem* eventSystem = EventExcuteSystem::GetInstance();

	std::function<void()> awake = std::bind(&Component::Awake, this);
	eventSystem->DetachEvent(this, EnumEvent::Awake, awake);

	std::function<void()> enable = std::bind(&Component::OnEnable, this);
	eventSystem->DetachEvent(this, EnumEvent::OnEnable, enable);

	std::function<void()> start = std::bind(&Component::Start, this);
	eventSystem->DetachEvent(this, EnumEvent::Start, start);

	std::function<void()> fixedUpdate = std::bind(&Component::FixedUpdate, this);
	eventSystem->DetachEvent(this, EnumEvent::FixedUpdate, fixedUpdate);

	std::function<void()> update = std::bind(&Component::Update, this);
	eventSystem->DetachEvent(this, EnumEvent::Update, update);

	std::function<void()> render = std::bind(&Component::OnRender, this);
	eventSystem->DetachEvent(this, EnumEvent::OnRender, render);

	std::function<void()> disable = std::bind(&Component::OnDisable, this);
	eventSystem->DetachEvent(this, EnumEvent::OnDisable, disable);

	std::function<void()> destroy = std::bind(&Component::OnDestroy, this);
	eventSystem->DetachEvent(this, EnumEvent::OnDestroy, destroy);
}
/// <summary>
/// 컴포넌트를 활성화 여부를 가지며 
/// 컴포넌트가 가진 메세지들을 가동의 여부를지님.
/// </summary>
/// <param name="_active"></param>
void Component::SetActive(bool _active)
{
	if (this != nullptr)
	{
		if (_active)
		{
			EventExcuteSystem::GetInstance()->SendEventMessage(this, EnumEvent::OnEnable);
		}
		else
		{
			EventExcuteSystem::GetInstance()->SendEventMessage(this, EnumEvent::OnDisable);
		}
		m_IsActive = _active;
	}
}
```

<li>EventSystem</li>

```cpp
EventExcuteSystem::EventExcuteSystem()
{
	m_EventDictionary = new std::map<Component*, std::map<EnumEvent, std::function<void()>>>();
	m_EventQueue = new std::deque<std::function<void()>>();
}

bool EventExcuteSystem::AttachEvent(Component* _component, EnumEvent _eventName, std::function<void()> _func)
{
	//컴포넌트 객체가 없거나(그럴 일은 없지만)
	//이벤트 이름이 정상적이지 않을 경우
	if (_component == nullptr || _eventName == EnumEvent::None)
	{
#ifdef _DEBUG
		std::cout << "EventExcute : component is nullptr or event name is empty" << std::endl;
#endif
		return false;
	}

	//만약 현재 객체로 등록된 이벤트 함수 테이블이 없을 경우
	if (m_EventDictionary->count(_component) == 0)
	{
		//함수 테이블을 새로 만든다
		m_EventDictionary->emplace(make_pair(_component, std::map < EnumEvent, std::function<void()>>()));
	}

	//현재 컴포넌트 객체를 키로 하는 테이블을 가져온다.
	auto it = m_EventDictionary->find(_component);

	//map이므로 테이블은 value에 해당한다
	std::map<EnumEvent, std::function<void()>>& eventList = it->second;

	bool hasEventFunc = false;

	// check handler
	for (auto it : eventList)
	{
		//이미 등록된 이벤트 함수가 있으면 체크해준다
		if (it.first == _eventName)
		{
			hasEventFunc = true;
			break;
		}
	}

	//등록이 되어 있으면 함수 종료
	if (hasEventFunc == true)
	{
		return false;
	}

	//등록이 안 되어 있으면 새로 등록한다.
	eventList.emplace(make_pair(_eventName, _func));
	return true;
}

bool EventExcuteSystem::DetachEvent(Component* _component, EnumEvent _eventName, std::function<void()> _func)
{
	//컴포넌트 객체가 없거나(그럴 일은 없지만)
	//이벤트 이름이 정상적이지 않을 경우
	if (_component == nullptr || _eventName == EnumEvent::None)
	{
#ifdef _DEBUG
		std::cout << "EventExcute : component is nullptr or event name is empty" << std::endl;
#endif
		return false;
	}

	//만약 등록된 이벤트 함수가 없으면 종료
	if (m_EventDictionary->count(_component) == 0) 
	{
		return false;
	}

	//이벤트 함수 테이블을 가져오자
	auto it = m_EventDictionary->find(_component);
	std::map<EnumEvent, std::function<void()>>& EventList = it->second;

	bool hasEventFunc = false;

	// check handler
	for (auto it : EventList) 
	{
		if (it.first == _eventName)//EventList(first) = EnumEvent
		{
			hasEventFunc = true;
			break;
		}
	}

	if (hasEventFunc == false)
	{
		return false;
	}

	//등록된 이벤트 함수를 삭제한다.
	for (auto it = EventList.begin(); it != EventList.end(); it++)
	{
		if ((*it).first == _eventName)
		{
			EventList.erase(it);
			break;
		}
	}

	if (EventList.size() == 0)
	{
		m_EventDictionary->erase(_component);
	}

	return true;
}

bool EventExcuteSystem::QueueEvent(std::function<void()> _func)
{
	//큐에 함수를 등록한다
	m_EventQueue->push_back(_func);

	return true;
}

void EventExcuteSystem::SendEventMessage(EnumEvent _event)
{
	//이벤트 함수 테이블을 돌면서
	for (auto it = m_EventDictionary->begin() ; it != m_EventDictionary->end(); it++)
	{
		//만약 현재 컴포넌트가 비활성화 상태이면 지나간다
		if (it->first->IsActive() == false || 
			(it->first->m_GameObject)->GetStringTag()._Equal("Player") || 
			(it->first->m_GameObject)->GetStringTag()._Equal("UI") || 
			(it->first->m_GameObject)->GetStringTag()._Equal("Bullet"))
			continue;
		
		//메시지를 받은 이벤트 함수를 큐에 넣는다.
		QueueEvent((it->second)[_event]);//---
	}

	for (auto it = m_EventDictionary->begin(); it != m_EventDictionary->end(); it++)
	{
		//만약 현재 컴포넌트가 비활성화 상태이면 지나간다
		if ((it->first->m_GameObject)->GetStringTag()._Equal("Player") && it->first->IsActive() == true)
		//메시지를 받은 이벤트 함수를 큐에 넣는다.
		QueueEvent((it->second)[_event]);//---
	}

	for (auto it = m_EventDictionary->begin(); it != m_EventDictionary->end(); it++)
	{
		//만약 현재 컴포넌트가 비활성화 상태이면 지나간다
		if ((it->first->m_GameObject)->GetStringTag()._Equal("Bullet") && it->first->IsActive() == true)
			//메시지를 받은 이벤트 함수를 큐에 넣는다.
			QueueEvent((it->second)[_event]);//---
	}

	for (auto it = m_EventDictionary->begin(); it != m_EventDictionary->end(); it++)
	{
		//만약 현재 컴포넌트가 비활성화 상태이면 지나간다
		if ((it->first->m_GameObject)->GetStringTag()._Equal("UI") && it->first->IsActive() == true)
		//메시지를 받은 이벤트 함수를 큐에 넣는다.
		QueueEvent((it->second)[_event]);//---
	}
}

void EventExcuteSystem::SendEventMessage(Component* _component, EnumEvent _event)
{
	//맵에 등록된 컴포넌트를 찾는다
	auto it = m_EventDictionary->find(_component);

	// 그 객체의 이벤트 함수 테이블을 가져온다
	//it->second = std::map<EnumEvent, std::function<void()>>

	std::map<EnumEvent, std::function<void()>>& EventList = it->second;

	//함수 테이블을 찾는다.
	auto func = EventList[_event];

	//이벤트 함수를 큐에 넣는다
	QueueEvent(func);
}
/// <summary>
/// 중요한 부분!
/// 
/// </summary>
void EventExcuteSystem::Update()
{
	//큐에 들어있는 이벤트가 있으면
	while (m_EventQueue->empty() == false)
	{
		//제일 앞의 것(함수)을 가져와서
		std::function<void()>& func = m_EventQueue->front();

		//함수를 실행하고
		if (func != nullptr)
		{
			func();
		}
		//큐에서 제거한다.
		//큐에서 제거해야지 쌓여있던 메세지들이 실행된다~
		if (m_EventQueue->empty() == true) break;

		m_EventQueue->pop_front();
	}
}

void EventExcuteSystem::DeleteAllMessage()
{
	m_EventQueue->clear();
}
```

<li>PostRender</li>

```cpp
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

```
</details>
