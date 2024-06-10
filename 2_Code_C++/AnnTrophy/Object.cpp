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
	Debug::LogFormat("%s 해당 객체를 삭제합니다",m_name);
#endif
	
	delete _obj;
	_obj = nullptr;
}

void Object::Destroy(Object* _obj, float _deleteTime)
{
#ifdef _DEBUG
	Debug::LogFormat("%f 만큼 시간이 경과되어 삭제합니다", _deleteTime);
	Debug::LogFormat("%s 해당 객체를 삭제합니다", m_name);
#endif
	delete _obj;
	_obj = nullptr;
}


void Object::Release()
{

}
