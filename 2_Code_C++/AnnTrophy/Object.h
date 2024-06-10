#pragma once
class Object
{
private:
	std::string m_name;
	std::string m_tag;
public:
	Object();
	Object(std::string _name);
	virtual ~Object();

public: // getter setter
	std::string GetStringName() { return m_name; }
	void SetStringName(std::string newName) { m_name = newName; };
	std::string GetStringTag() { return m_tag; }
	void SetStringTag(std::string newTag) { m_tag = newTag; }
public:
	//��ӹ��� ��ü�� �̸��� ��Ÿ���� ���� �����Լ�
	virtual void	Awake() {};
	virtual void	OnEnable() {};
	virtual void	Start() {};
	virtual void	FixedUpdate() {};
	virtual void	Update() {};
	virtual void	OnRender() {};
	virtual void	OnDisable() {};
	virtual void	OnDestroy() {};
public:
	virtual void Release();


public:
	//�����Լ�
	void Destroy();
	void Destroy(Object* _obj);
	void Destroy(Object* _obj, float _deleteTime);

public: // Operators

	//�� ������ �����ε�
	//bool operator!=(Object& _another);	// a,b������Ʈ�� ���δٸ���?
	//bool operator==(Object& _another);		// ������?
	//bool friend operator == (Object _a, Object _b); 
};