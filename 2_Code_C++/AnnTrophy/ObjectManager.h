#pragma once
class ObjectManager : public SingletonManager<ObjectManager>
{
public:
	std::vector<GameObject*> m_Objects;
	void Destroy(GameObject** obj);

	GameObject* FindGameObjectByTag(std::string tag);
};

