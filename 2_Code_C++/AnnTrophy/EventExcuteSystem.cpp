#pragma once
#include "pch.h"
#include <iostream>
#include "Object.h"
#include "Component.h"
#include "Transform.h"
#include "GameObject.h"
#include "SingletonManager.h"
#include "EventExcuteSystem.h"

EventExcuteSystem::EventExcuteSystem()
{
	m_EventDictionary = new std::map<Component*, std::map<EnumEvent, std::function<void()>>>();
	m_EventQueue = new std::deque<std::function<void()>>();
}

bool EventExcuteSystem::AttachEvent(Component* _component, EnumEvent _eventName, std::function<void()> _func)
{
	//������Ʈ ��ü�� ���ų�(�׷� ���� ������)
	//�̺�Ʈ �̸��� ���������� ���� ���
	if (_component == nullptr || _eventName == EnumEvent::None)
	{
#ifdef _DEBUG
		std::cout << "EventExcute : component is nullptr or event name is empty" << std::endl;
#endif
		return false;
	}

	//���� ���� ��ü�� ��ϵ� �̺�Ʈ �Լ� ���̺��� ���� ���
	if (m_EventDictionary->count(_component) == 0)
	{
		//�Լ� ���̺��� ���� �����
		m_EventDictionary->emplace(make_pair(_component, std::map < EnumEvent, std::function<void()>>()));
	}

	//���� ������Ʈ ��ü�� Ű�� �ϴ� ���̺��� �����´�.
	auto it = m_EventDictionary->find(_component);

	//map�̹Ƿ� ���̺��� value�� �ش��Ѵ�
	std::map<EnumEvent, std::function<void()>>& eventList = it->second;

	bool hasEventFunc = false;

	// check handler
	for (auto it : eventList)
	{
		//�̹� ��ϵ� �̺�Ʈ �Լ��� ������ üũ���ش�
		if (it.first == _eventName)
		{
			hasEventFunc = true;
			break;
		}
	}

	//����� �Ǿ� ������ �Լ� ����
	if (hasEventFunc == true)
	{
		return false;
	}

	//����� �� �Ǿ� ������ ���� ����Ѵ�.
	eventList.emplace(make_pair(_eventName, _func));
	return true;
}

bool EventExcuteSystem::DetachEvent(Component* _component, EnumEvent _eventName, std::function<void()> _func)
{
	//������Ʈ ��ü�� ���ų�(�׷� ���� ������)
	//�̺�Ʈ �̸��� ���������� ���� ���
	if (_component == nullptr || _eventName == EnumEvent::None)
	{
#ifdef _DEBUG
		std::cout << "EventExcute : component is nullptr or event name is empty" << std::endl;
#endif
		return false;
	}

	//���� ��ϵ� �̺�Ʈ �Լ��� ������ ����
	if (m_EventDictionary->count(_component) == 0) 
	{
		return false;
	}

	//�̺�Ʈ �Լ� ���̺��� ��������
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

	//��ϵ� �̺�Ʈ �Լ��� �����Ѵ�.
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
	//ť�� �Լ��� ����Ѵ�
	m_EventQueue->push_back(_func);

	return true;
}

void EventExcuteSystem::SendEventMessage(EnumEvent _event)
{
	//�̺�Ʈ �Լ� ���̺��� ���鼭
	for (auto it = m_EventDictionary->begin() ; it != m_EventDictionary->end(); it++)
	{
		//���� ���� ������Ʈ�� ��Ȱ��ȭ �����̸� ��������
		if (it->first->IsActive() == false || 
			(it->first->m_GameObject)->GetStringTag()._Equal("Player") || 
			(it->first->m_GameObject)->GetStringTag()._Equal("UI") || 
			(it->first->m_GameObject)->GetStringTag()._Equal("Bullet"))
			continue;
		
		//�޽����� ���� �̺�Ʈ �Լ��� ť�� �ִ´�.
		QueueEvent((it->second)[_event]);//---
	}

	for (auto it = m_EventDictionary->begin(); it != m_EventDictionary->end(); it++)
	{
		//���� ���� ������Ʈ�� ��Ȱ��ȭ �����̸� ��������
		if ((it->first->m_GameObject)->GetStringTag()._Equal("Player") && it->first->IsActive() == true)
		//�޽����� ���� �̺�Ʈ �Լ��� ť�� �ִ´�.
		QueueEvent((it->second)[_event]);//---
	}

	for (auto it = m_EventDictionary->begin(); it != m_EventDictionary->end(); it++)
	{
		//���� ���� ������Ʈ�� ��Ȱ��ȭ �����̸� ��������
		if ((it->first->m_GameObject)->GetStringTag()._Equal("Bullet") && it->first->IsActive() == true)
			//�޽����� ���� �̺�Ʈ �Լ��� ť�� �ִ´�.
			QueueEvent((it->second)[_event]);//---
	}

	for (auto it = m_EventDictionary->begin(); it != m_EventDictionary->end(); it++)
	{
		//���� ���� ������Ʈ�� ��Ȱ��ȭ �����̸� ��������
		if ((it->first->m_GameObject)->GetStringTag()._Equal("UI") && it->first->IsActive() == true)
		//�޽����� ���� �̺�Ʈ �Լ��� ť�� �ִ´�.
		QueueEvent((it->second)[_event]);//---
	}
}

void EventExcuteSystem::SendEventMessage(Component* _component, EnumEvent _event)
{
	//�ʿ� ��ϵ� ������Ʈ�� ã�´�
	auto it = m_EventDictionary->find(_component);

	// �� ��ü�� �̺�Ʈ �Լ� ���̺��� �����´�
	//it->second = std::map<EnumEvent, std::function<void()>>

	std::map<EnumEvent, std::function<void()>>& EventList = it->second;

	//�Լ� ���̺��� ã�´�.
	auto func = EventList[_event];

	//�̺�Ʈ �Լ��� ť�� �ִ´�
	QueueEvent(func);
}
/// <summary>
/// �߿��� �κ�!
/// 
/// </summary>
void EventExcuteSystem::Update()
{
	//ť�� ����ִ� �̺�Ʈ�� ������
	while (m_EventQueue->empty() == false)
	{
		//���� ���� ��(�Լ�)�� �����ͼ�
		std::function<void()>& func = m_EventQueue->front();

		//�Լ��� �����ϰ�
		if (func != nullptr)
		{
			func();
		}
		//ť���� �����Ѵ�.
		//ť���� �����ؾ��� �׿��ִ� �޼������� ����ȴ�~
		if (m_EventQueue->empty() == true) break;

		m_EventQueue->pop_front();
	}
}

void EventExcuteSystem::DeleteAllMessage()
{
	m_EventQueue->clear();
}
