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
	//상속받을 객체의 이름을 나타내는 순수 가상함수
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
	//삭제함수
	void Destroy();
	void Destroy(Object* _obj);
	void Destroy(Object* _obj, float _deleteTime);

public: // Operators

	//논리 연산자 오버로딩
	//bool operator!=(Object& _another);	// a,b오브젝트가 서로다른가?
	//bool operator==(Object& _another);		// 같은가?
	//bool friend operator == (Object _a, Object _b); 
};