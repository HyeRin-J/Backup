#include "pch.h"
#include "Object.h"
#include "Component.h"
#include "Transform.h"
#include "GameObject.h"
#include "SingletonManager.h"
#include "ObjectManager.h"

void ObjectManager::Destroy(GameObject** obj)
{
	for (auto it = m_Objects.begin(); it <= m_Objects.end(); it++)
	{
		if (*it == *obj)
		{
			m_Objects.erase(it);
			break;
		}
	}

	delete* obj;
}

GameObject* ObjectManager::FindGameObjectByTag(std::string tag)
{
	for (auto gameObject : m_Objects)
	{
		if ((gameObject->GetStringTag())._Equal(tag) == true)
		{
			return gameObject;
		}
	}

	return nullptr;
}
