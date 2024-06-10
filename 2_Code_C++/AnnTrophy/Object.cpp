#include "pch.h"
#include "Object.h"
#include "Component.h"
#include "Transform.h"
#include "Debug.h"


Object::Object() : m_name("")
{
}

Object::Object(string _name)
{
	m_name = _name;
}

Object::~Object()
{
}

void Object::Destroy()
{
	delete this;
	
}

void Object::Destroy(Object* _obj)
{
#ifdef _DEBUG
	Debug::LogFormat("%s �ش� ��ü�� �����մϴ�",m_name);
#endif
	
	delete _obj;
	_obj = nullptr;
}

void Object::Destroy(Object* _obj, float _deleteTime)
{
#ifdef _DEBUG
	Debug::LogFormat("%f ��ŭ �ð��� ����Ǿ� �����մϴ�", _deleteTime);
	Debug::LogFormat("%s �ش� ��ü�� �����մϴ�", m_name);
#endif
	delete _obj;
	_obj = nullptr;
}


void Object::Release()
{

}
