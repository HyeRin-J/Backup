#include "pch.h"
#include "SingletonManager.h"
#include "Object.h"
#include "Component.h"
#include "EventExcuteSystem.h"
#include "Transform.h"
#include "GameObject.h"
#include "Debug.h"


using namespace std;

GameObject::GameObject()
{
	m_pTransform = new Transform;
	m_pTransform->m_GameObject = this;
#ifdef _DEBUG
	Debug::LogFormat("게임오브젝트 생성함 : %d ", 1);
#endif

}

GameObject::GameObject(float _x, float _y, float _width, float _height) :GameObject()
{
	m_pTransform->SetPos(_x, _y);
}


GameObject::~GameObject()
{
	delete m_pTransform;
	m_pTransform = nullptr;

	for (auto _com : m_Components)
	{
		delete _com;
		_com = nullptr;
	}

	m_Components.clear();
}
GameObject::GameObject(string _name) 
{
	Object::SetStringName(_name);
}