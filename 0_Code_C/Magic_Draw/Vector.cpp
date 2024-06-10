#include "Vector.h"
#include "math.h"
#include "windows.h"

#pragma region double2, Vector2
double2::double2()
{
	x = 0.f; y = 0.f;
}

double2::double2(double _x, double _y)
{
	x = _x; y = _y;
}

//생성자
Vector2::Vector2()
	: double2() {}

Vector2::Vector2(double x)
	: double2(x, x) {}

Vector2::Vector2(double x, double y)
	: double2(x, y) {}

Vector2::Vector2(double2& F)
{
	x = F.x;
	y = F.y;
}
/*
Vector2::Vector2(Vector2& V)
{
	x = V.x;
	y = V.y;
}
*/
//연산자 재정의
bool Vector2::operator== (Vector2& V)
{
	if (abs((long)(x - V.x)) < 0.1f && abs((long)(y - V.y)) < 0.1f) {
		return true;
	}
	return false;
}

bool Vector2::operator!= (Vector2& V)
{
	if (abs((long)(x - V.x)) < 0.1f && abs((long)(y == V.y)) < 0.1f) {
		return false;
	}
	return true;
}

Vector2 Vector2::operator+= (Vector2 V)
{
	Vector2 rv = { x += V.x, y += V.y };
	return rv;
}

Vector2 Vector2::operator-= (Vector2 V)
{
	Vector2 rv = { x -= V.x, y -= V.y };
	return rv;
}

Vector2 Vector2::operator*= (float S)
{
	Vector2 rv = { x *= S, y *= S };
	return rv;
}

Vector2 Vector2::operator/= (float S)
{
	Vector2 rv = { x /= S, y /= S };
	return rv;
}

Vector2 Vector2::operator+ (Vector2 V)
{
	Vector2 rv = { x + V.x, y + V.y };
	return rv;
}

Vector2 Vector2::operator- (Vector2 V)
{
	Vector2 rv = { x - V.x, y - V.y };
	return rv;
}

Vector2 Vector2::operator* (float S)
{
	Vector2 rv = { x * S, y * S };
	return rv;
}

Vector2 Vector2::operator/ (float S)
{
	Vector2 rv = { x / S, y / S };
	return rv;
}

Vector2 Vector2::operator+ ()
{
	return *this;
}

Vector2 Vector2::operator- ()
{
	return Vector2(-x, -y);
}

double Vector2::Length()
{
	return sqrtf(x * x + y * y);
}

double Vector2::LengthSquared()
{
	return x * x + y * y;
}

double Vector2::Dot(const Vector2 V)
{
	return x * V.x + y * V.y;
}

double Vector2::Angle(Vector2 V1, Vector2 V2)
{
	double dot = Dot(V1, V2);
	if (dot > 1) dot = 1;
	else if (dot < -1) dot = -1;
	double rad = acos(dot);

	return ToDegree(rad);
}

Vector2 Vector2::Normalize()
{
	Vector2 bf = *this;

	this->x /= Length();
	this->y /= Length();

	return bf;
}

void Vector2::Normalize(Vector2 result)
{
	result.x /= Length();
	result.y /= Length();
}

Vector2 Vector2::Clamp(const Vector2 min, const Vector2 max)
{
	Vector2 bf = *this;
	(x <= min.x) ? (x = min.x) : ((x > max.x) ? (x = max.x) : (x));
	(y <= min.y) ? (y = min.y) : ((y > max.y) ? (y = max.y) : (y));
	return bf;
}

void Vector2::Clamp(Vector2 min, Vector2 max, Vector2 ret)
{
	(ret.x <= min.x) ? (ret.x = min.x) : ((ret.x > max.x) ? (ret.x = max.x) : (ret.x));
	(ret.y <= min.y) ? (ret.y = min.y) : ((ret.y > max.y) ? (ret.y = max.y) : (ret.y));
}

//static function
Vector2 operator* (double S, Vector2 V)
{
	Vector2 rv = { V.x * S, V.y * S };
	return rv;
}

Vector2 operator/ (double S, Vector2 V)
{
	Vector2 rv = { V.x / S, V.y / S };
	return rv;
}

double Vector2::Dot(Vector2 v1, Vector2 v2)
{
	return v1.x * v2.x + v1.y * v2.y;
}

double Vector2::Distance(Vector2 v1, Vector2 v2)
{
	return (v2 - v1).Length();
}

double Vector2::DistanceSquared(Vector2 v1, Vector2 v2)
{
	return (v2 - v1).LengthSquared();
}

Vector2 Vector2::Min(Vector2 v1, Vector2 v2)
{
	Vector2 ret;
	(v1.x <= v2.x) ? (ret.x = v1.x) : (ret.x = v2.x);
	(v1.y <= v2.y) ? (ret.y = v1.y) : (ret.y = v2.y);
	return ret;
}

Vector2 Vector2::Max(Vector2 v1, Vector2 v2)
{
	Vector2 ret;
	(v1.x >= v2.x) ? (ret.x = v1.x) : (ret.x = v2.x);
	(v1.y >= v2.y) ? (ret.y = v1.y) : (ret.y = v2.y);
	return ret;
}

Vector2 Vector2::Lerp(Vector2 v1, Vector2 v2, double t)
{
	Vector2 ret;
	t = (t > 1.0f) ? 1.0f : ((t < 0.0f) ? 0.0f : t);
	ret.x = v1.x + t * (v2.x - v1.x);
	ret.y = v1.y + t * (v2.y - v1.y);
	return ret;
}

Vector2 Vector2::SmoothStep(Vector2 v1, Vector2 v2, double t)
{
	Vector2 ret;
	t = (t > 1.0f) ? 1.0f : ((t < 0.0f) ? 0.0f : t);
	t = t * t * (3.f - 2.f * t); //극솟값이 0, 극댓값이 1, 변곡점이 0.5. (이 조건을 만족한다면 다른 식을 써도 될 듯)
	ret.x = v1.x + t * (v2.x - v1.x);
	ret.y = v1.y + t * (v2.y - v1.y);
	return ret;
}

Vector2 Vector2::Reflect(Vector2 iV, Vector2 nV)
{
	return 2.0f * Dot(iV, nV) * (iV - iV);
}

Vector2 Vector2::Refract(Vector2 iV, Vector2 nV, double rI)
{
	double t = Dot(iV, nV);
	double r = 1.f - (rI * rI) * (1.f - (t * t));

	Vector2 rv;
	if (r < 0.f) {
		rv = { 0.f, 0.f };
	}
	else {
		double s = rI * t + sqrt(r);
		rv = { rI * iV.x - s * nV.x, rI * iV.y - s * nV.y };
	}
	return rv;
}

const Vector2 Vector2::Zero2 = { 0.f, 0.f };
const Vector2 Vector2::UnitX2 = { 1.f, 0.f };
const Vector2 Vector2::UnitY2 = { 0.f, 1.f };
#pragma endregion