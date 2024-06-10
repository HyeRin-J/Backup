#pragma once
class Object;
class GameObject;
class Transform;

/// <summary>
/// ������Ʈ ������ �������
/// ������Ʈ���� �޼����� ���� ���־���Ѵٰ� �ô�
/// ���� �� ������ �ƴѰŰ����� 
/// ���ϱ� ������ ���ɻ������� �˰Ű���. ����͸�..����͸�..
/// </summary>
class Component : public Object
{
public:
	Component();
	virtual ~Component();
public:
	//������Ʈ�� ���ӿ�����Ʈ�� ���������� ��.
	//�׷��� ���ӿ�����Ʈ�� ������ ������ �˰��־����
	GameObject* m_GameObject;
	//���� ������Ʈ�� Ȱ��ȭ �Ұ��ΰ��� ���� ���� ex)transform , collider ��.
	bool m_IsActive = true;
public:
	void	SetActive(bool _active);
	bool	IsActive() { return m_IsActive; }

	//������Ʈ�� ���� �ڽ� ��ü���� �������ִ�
	//�޼������̴� .
	//[�̺�Ʈ ť]�� �̿��� �޼��� ó����
	//������Ʈ���� ���� ������ �޼����� �������ִ�.
	//������Ʈ�� �������� ȣ������ �ʾƵ� �ݺ��� ������ �̺�Ʈ ť ���
	//Ȱ��ȭ �������μ� �� ����� ���� ����ϰ� �ϰԲ� �ϴ� ����̴�.

	virtual void	Awake() {}; //ó�� ������ �� �ѹ���
	virtual void	OnEnable() {};//Ȱ��
	virtual void	Start() {};//�� ��ü�� ó�� Ȱ��ȭ �ɶ�
	virtual void	FixedUpdate() {};//������ ����o
	virtual void	Update() {};//������ ����x
	virtual void	OnRender() {};//�׷��ִ� ���
	virtual void	OnDisable() {};//��Ȱ��
	virtual void	OnDestroy() {};//��ü ����

};

