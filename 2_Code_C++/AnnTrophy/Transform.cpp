#include "pch.h"
#include "Vector.h"
#include "Object.h"
#include "Component.h"
#include "Transform.h"
#include "SpriteData.h"
#include "SingletonManager.h"
#include "D2DGraphics.h"
#include "Debug.h"

Transform::Transform(float _x, float _y)
{
	m_Position.x = _x;
	m_Position.y = _y;
}

Transform::Transform()
{
}

Transform::~Transform()
{
}



void Transform::SetTransform(const D2D1_MATRIX_3X2_F& transformMatrix)
{
	D2DGraphics::GetInstance()->m_pRenderTarget->SetTransform(transformMatrix);
}

void Transform::SetPos(float x, float y)
{
	m_Position.x = x;
	m_Position.y = y;
}

void Transform::SetPos(D2D1_POINT_2F _pos)
{
	m_Position = _pos;
}


void Transform::Translate(float _x, float _y)
{
	m_Position.x += _x;
	m_Position.y += _y;

}

void Transform::Translate(Vector2 _vec)
{
	m_Position.x += _vec.x;
	m_Position.y += _vec.y;
}

void Transform::Rotate()
{
	D2D1_MATRIX_3X2_F rotateMatrix = MakeRotationMatrix(m_Rotate, m_Position);

	SetTransform(rotateMatrix);
}

void Transform::Scale()
{
	D2D1_MATRIX_3X2_F scaleMatrix = MakeScaleMatrix(m_Scale, m_Position);

	SetTransform(scaleMatrix);
}


D2D1_MATRIX_3X2_F Transform::MakeTranslationMatrix(D2D1_SIZE_F size)
{
	// DX는 보통 생각하는 행렬 * 벡터 형태의 변환행렬이 아니고
	// 벡터 * 행렬 형태이므로 전치행렬(transposed)의 형태로 넣어줘야 한다. 
	D2D1_MATRIX_3X2_F _translation;

	_translation._11 = 1.0f; _translation._12 = 0.0f;
	_translation._21 = 0.0f; _translation._22 = 1.0f;
	_translation._31 = size.width; _translation._32 = size.height;

	return _translation;
}

D2D1_MATRIX_3X2_F Transform::MakeRotationMatrix(FLOAT angle, D2D1_POINT_2F center /*= D2D1::Point2F()*/)
{
	///D2D1_MATRIX_3X2_F _rotationOnAxis = D2D1::Matrix3x2F::Rotation(45.0f, D2D1::Point2F(200.f, 200.f));

	D2D1_MATRIX_3X2_F _translateToOrigin = MakeTranslationMatrix(D2D1::Size(-center.x, -center.y));
	D2D1_MATRIX_3X2_F _rotateOnOrigin = MakeRotationMatrix_Origin(angle);
	D2D1_MATRIX_3X2_F _translateToCenter = MakeTranslationMatrix(D2D1::Size(center.x, center.y));

	D2D1_MATRIX_3X2_F _resultTM = _translateToOrigin * _rotateOnOrigin * _translateToCenter;

	return _resultTM;
}

D2D1_MATRIX_3X2_F Transform::MakeScaleMatrix(D2D1_SIZE_F size, D2D1_POINT_2F center /*= D2D1::Point2F()*/)
{
	D2D1_MATRIX_3X2_F _translateToOrigin = MakeTranslationMatrix(D2D1::Size(-center.x, -center.y));
	D2D1_MATRIX_3X2_F _scaleOnOrigin = MakeScaleMatrix_Origin(size);
	D2D1_MATRIX_3X2_F _translateToCenter = MakeTranslationMatrix(D2D1::Size(center.x, center.y));

	D2D1_MATRIX_3X2_F _resultTM = _translateToOrigin * _scaleOnOrigin * _translateToCenter;

	return _resultTM;
}

D2D1_MATRIX_3X2_F Transform::MakeRotationMatrix_Origin(FLOAT angle)
{
	float _rad = ToRadian(angle);

	D2D1_MATRIX_3X2_F _rotation;

	_rotation._11 = cos(_rad); _rotation._21 = -sin(_rad); _rotation._31 = 0;
	_rotation._12 = sin(_rad); _rotation._22 = cos(_rad); _rotation._32 = 0;

	return _rotation;
}

D2D1_MATRIX_3X2_F Transform::MakeScaleMatrix_Origin(D2D1_SIZE_F size)
{
	D2D1_MATRIX_3X2_F _scale;

	_scale._11 = size.width * 1.0f; _scale._12 = 0.0f;
	_scale._21 = 0.0f; _scale._22 = size.height * 1.0f;
	_scale._31 = 0.0f; _scale._32 = 0.0f;

	return _scale;
}

void Transform::SetIdentity()
{
	SetTransform(D2D1::Matrix3x2F::Identity());
}
