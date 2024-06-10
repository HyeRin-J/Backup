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
