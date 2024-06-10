#pragma once

class Transform;
class Component;
//���� ������Ʈ�� (�ֻ���)������Ʈ�� ��ӹ޴´�
class GameObject : public Object
{
public:
	Transform* m_pTransform;
private:
	//���� �� ��ġ ������ transform �����ͺ���
	std::vector<Component*> m_Components;

public:
	//�Ϲ� ����
	GameObject();
	~GameObject();
	//�̸��� ������ ä�� ����
	GameObject(std::string _name);
	//����� ������ ä��  ����
	GameObject(float _x, float _y, float _width, float _height);

	D2D1_POINT_2F GetPos() { return m_pTransform->GetPos(); };
	void SetPos(D2D1_POINT_2F point) { m_pTransform->m_Position.x = point.x; m_pTransform->m_Position.y = point.y; };

	D2D1_SIZE_F GetScale() { return m_pTransform->m_Scale; };
	void SetScale(D2D1_SIZE_F _scale) { m_pTransform->m_Scale = _scale; };

	void ReleaseComponents() { m_Components.clear(); }
public:
	//���ø��� �޴� ������
	//��� ���·� ������ �𸣰�����
	//�ϴ� �ް���, Ÿ��id������ �ڷ����¸� �˾Ƴ��� �װͿ� �´� ������Ʈ�� �߰��ϱ� ����
	//���ø����� �����Ͽ���.
	//���� �� �����δ� ��� ���°� ���� �𸣱⋚���� nullüũ�� �ݵ�� ���־���Ѵ�.
	template<typename T>
	T* AddComponent();

	template<typename T>
	T* GetComponent();

	std::vector<Component*> GetAllComponents() { return m_Components; }

	template<typename T>
	inline T* GetComponentByTag(std::string _tag)
	{
		for (auto _com : m_Components)
		{
			if (_com->GetStringTag()._Equal(_tag) == 0)
			{
				return _com;
			}
		}

		return nullptr;
	}
public:
	virtual void	Awake() {};
	virtual void	OnEnable() {};
	virtual void	Start() {};
	virtual void	FixedUpdate() {};
	virtual void	Update() {};
	virtual void	OnRender() {};
	virtual void	OnDisable() {};
	virtual void	OnDestroy() {};	
};

//��� �ڷḦ ���� �� ��� Ÿ���� �޴��� �𸣰����� ���ϴ� Ÿ������ ĳ������ ���Ҷ�
//���ø����� �޾��ְ� , �ش�Ÿ�Կ� ���� ��ü�� �����Ѵ�.
template<typename T>
inline T* GameObject::AddComponent()
{
	//���ϴ� Ÿ���� ��ü�� ����(������ �ܰ迡�� ������)
	T* _com = new T();
	m_Components.push_back(_com);

	//�ϴ� ������Ʈ�� �� ��ü�� �𸥴�.
	//�� ���� ������Ʈ���� �񱳸� ���� �� ����� �Ͼ�� ��.
	//�׷��⸦ �����ϱ����� ������Ʈ�� ��ü�� �˾ƾ��� �ʿ䰡����
	//�׷��� ���ؼ� �ش� ��ü�� ���� (�ڽ���)�ּҸ� �־������ν�
	//�ش� ������Ʈ�� �˼��ְ�
	//�� ������Ʈ�� �ش簴ü�� ���ӵȴ�.
	_com->m_GameObject = this;
	//_com->m_pTransform = this->m_pTransform;
	//��ü�� �ʱ���¸� ȣ��
	_com->Awake();

	//������Ʈ�� �߰��ɶ�����
	//�ش� ���ӿ�����Ʈ ��ü�� ������Ʈ�� �߰��Ѵ�.

	return _com;
}

template<typename T>
inline T* GameObject::GetComponent()
{
	//m_Components�� �����ŭ com�� �ִ´ٴ� �ǹ̷�..
	// com <= m_Components 
	//������Ʈ�� ������ ���Ϳ��� 
	//autoŰ����� �ش��ڷḦ �о���δ�.
	//auto�� ������� ���� Components*������ ���� ������ ���ʹϱ�
	//Components* com�� �Ǵµ� ���� ���ϴ°� Components*�� �����Ͱ��ƴ�
	//T*Ÿ���� ������ ���ϴ� �ؿ��� �ѹ� �� dynamic_cast<> �� �����Ѵ�.

	for (auto com : m_Components)
	{
		//���� ���ø� Ÿ�Կ� �´� �ٿ�ĳ������ �����Ѵ�.
		//���̳��� ĳ��Ʈ�� ĳ���� ���� �� ���
		//nullptr�� ��ȯ�Ѵ�.
		T* c = dynamic_cast<T*>(com);
		//nullptr�� �ƴѰ�� �ش� �ڷḦ ��ȯ�Ѵ�.
		if (c != nullptr)
		{
			return c;
		}
	}
	return nullptr;
}
