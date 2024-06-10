#include "pch.h"
#include "SingletonManager.h"
#include "Object.h"
#include "Component.h"
#include "Transform.h"
#include "GameObject.h"
#include "EventExcuteSystem.h"

using namespace std;
/// <summary>
/// 해당 컴포넌트를 가진 객체는 
/// 해당 메세지함수들을 가지고있게 해주는 기능이다.
/// 현재 아래 기본 생성자는
/// 전부 메세지 함수를 가지고있다.
/// </summary>
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


