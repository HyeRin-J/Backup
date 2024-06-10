#pragma once

//컴포넌트가 나타낼 이벤트를 나타내준다.
enum class EnumEvent
{
	None = -1, Awake, OnEnable, 
	Start, FixedUpdate, Update, OnRender, OnDisable, OnDestroy
};


class EventExcuteSystem : public SingletonManager<EventExcuteSystem>
{
	//등록한 이벤트 함수
	//		   컴포넌트 객체를 받고, 어떠한 메세지 상태인가, 어떠한 함수를 적용시킬까?
	//		   Enum과 함수명은 같다고 보면된다.
	std::map < Component*, std::map<EnumEvent, std::function<void()>>>* m_EventDictionary;
	
	
	//실행할 이벤트 큐
	std::deque< std::function<void()>>* m_EventQueue;

	//이벤트를 큐에 넣는다
	bool QueueEvent(std::function<void()> _func);


public:
	//기본 생성자
	EventExcuteSystem();

	/// <summary>
	/// 이벤트 함수를 등록한다
	/// </summary>
	/// <param name="_component">컴포넌트의 객체</param>
	/// <param name="_eventName">등록할 이벤트 이름</param>
	/// <param name="_func">등록할 이벤트 멤버 함수</param>
	/// <returns></returns>
	bool AttachEvent(Component* _component, EnumEvent _eventName, std::function<void()> _func);
	bool DetachEvent(Component* _component, EnumEvent _eventName, std::function<void()> _func);

	/// <summary>
	/// 이 함수는 compoment를 가진 객체들으 
	/// 들어온 EnumEvent 대한 전체 메시지를 실행한다.
	/// 
	/// SendEventMessage(EnumEvent::Awake)를 해주면.
	/// 컴포넌트를 가진 객체들은 전부 Awake를 실행한다는 뜻이다.
	/// </summary> 
	/// <param name="_event"></param>
	void SendEventMessage(EnumEvent _event);

	/// <summary>
	/// OnEnable or OnDisable
	/// </summary>
	/// <param name="_component"></param>
	/// <param name="_event"></param>
	void SendEventMessage(Component* _component, EnumEvent _event);

	/// <summary>
	/// 큐를 처리한다
	/// </summary>
	void Update();

	void DeleteAllMessage();
};

