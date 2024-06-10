

#pragma once
//스스로 이해를 할려고 노력은 해보았는데 이렇게 질질끌다보면
//내 자신의 발전도 팀의 노고도 물거품이라.
//성민이형 예제를 참고하면서 만들었다.
//
class Component;
struct Vector2;

class Transform : public Component
{
public:
	//멤버변수 
	D2D1_POINT_2F m_Position = { 0, 0 };
	double m_Rotate = 0; // angle 
	D2D1_SIZE_F m_Scale = { 1, 1 };

public:
	//생성자 관련
	//확장성을 고려해서 일단은..
	Transform(float _x, float _y);//2d
	Transform();//2d
	~Transform();

	virtual void	FixedUpdate() {};
	virtual void	Update() {};
	virtual void	OnDestroy() {};

public:

	void SetPos(float _x, float _y);
	void SetPos(D2D1_POINT_2F _pos);
	D2D1_POINT_2F GetPos() { return m_Position; };
	
	void SetRotation(double newAngle) { m_Rotate = newAngle; };
	double GetRotation() { return m_Rotate; };

	void SetScale(D2D1_SIZE_F newScale) { m_Scale = newScale; };
	void SetScale(float width, float height) { m_Scale.width = width; m_Scale.height = height; };

	D2D1_SIZE_F GetScale() { return m_Scale; };


	void Translate(float _x, float _y);
	void Translate(Vector2 _vec);

	void Rotate();
	void Scale();

//++static Func
public:

	static D2D1_MATRIX_3X2_F MakeTranslationMatrix(D2D1_SIZE_F size);
	static D2D1_MATRIX_3X2_F MakeRotationMatrix(FLOAT angle, D2D1_POINT_2F center = D2D1::Point2F());
	static D2D1_MATRIX_3X2_F MakeScaleMatrix(D2D1_SIZE_F size, D2D1_POINT_2F center = D2D1::Point2F());

	//origin rs
	static D2D1_MATRIX_3X2_F MakeRotationMatrix_Origin(FLOAT angle);
	static D2D1_MATRIX_3X2_F MakeScaleMatrix_Origin(D2D1_SIZE_F size);

	static void SetIdentity();

	static void SetTransform(const D2D1_MATRIX_3X2_F& transformMatrix);

};

