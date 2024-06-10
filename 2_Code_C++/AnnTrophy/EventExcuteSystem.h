#pragma once

//������Ʈ�� ��Ÿ�� �̺�Ʈ�� ��Ÿ���ش�.
enum class EnumEvent
{
	None = -1, Awake, OnEnable, 
	Start, FixedUpdate, Update, OnRender, OnDisable, OnDestroy
};


class EventExcuteSystem : public SingletonManager<EventExcuteSystem>
{
	//����� �̺�Ʈ �Լ�
	//		   ������Ʈ ��ü�� �ް�, ��� �޼��� �����ΰ�, ��� �Լ��� �����ų��?
	//		   Enum�� �Լ����� ���ٰ� ����ȴ�.
	std::map < Component*, std::map<EnumEvent, std::function<void()>>>* m_EventDictionary;
	
	
	//������ �̺�Ʈ ť
	std::deque< std::function<void()>>* m_EventQueue;

	//�̺�Ʈ�� ť�� �ִ´�
	bool QueueEvent(std::function<void()> _func);


public:
	//�⺻ ������
	EventExcuteSystem();

	/// <summary>
	/// �̺�Ʈ �Լ��� ����Ѵ�
	/// </summary>
	/// <param name="_component">������Ʈ�� ��ü</param>
	/// <param name="_eventName">����� �̺�Ʈ �̸�</param>
	/// <param name="_func">����� �̺�Ʈ ��� �Լ�</param>
	/// <returns></returns>
	bool AttachEvent(Component* _component, EnumEvent _eventName, std::function<void()> _func);
	bool DetachEvent(Component* _component, EnumEvent _eventName, std::function<void()> _func);

	/// <summary>
	/// �� �Լ��� compoment�� ���� ��ü���� 
	/// ���� EnumEvent ���� ��ü �޽����� �����Ѵ�.
	/// 
	/// SendEventMessage(EnumEvent::Awake)�� ���ָ�.
	/// ������Ʈ�� ���� ��ü���� ���� Awake�� �����Ѵٴ� ���̴�.
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
	/// ť�� ó���Ѵ�
	/// </summary>
	void Update();

	void DeleteAllMessage();
};

