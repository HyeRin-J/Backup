#pragma once

struct BBOX
{
	int	Left,				//충돌영역. : 이미지 좌측상단 기준좌표.
		Right,
		Top,
		Bottom;

	int Hx, Hy;			//충돌영역, 각 변의 1/2 길이.
	int Cx, Cy;			//중심점 : 이미지 좌측상단 기준좌표.
};

struct POSITION
{
	float x;
	float y;
};

#define IS_DEAD			0x00000001
#define IS_COLLISION	0x00000010

class Unit
{
protected:
	int m_Hp = 1;

	POSITION m_Pos = POSITION();		//출력좌표

	float m_Velo = 10;	//이동 속도
	float m_ScaleX = 1, m_ScaleY = 1; //스프라이트 스케일

	Vector2 m_Dir = { -1, 0 };	//이동할 방향
	PSPRITE m_Spr = NULL;	//출력할 스프라이트

	BBOX m_BBox;

public:
	unsigned int m_State = 0x00000000;

	Unit();

	int GetHP() { return m_Hp; }

	POSITION GetPos();

	bool SetSprite(SPRITE spr);
	SPRITE GetSprite();

	void SetScale(float scaleX, float scaleY) { m_ScaleX = scaleX; m_ScaleY = scaleY; }

	void SetBBox(int Left, int Width, int Top, int Height);
	void SetBBox(BBOX bBox);

	BBOX GetBBox() { return m_BBox; }
	POSITION GetBBoxCenter();
	POSITION GetCenter();

	bool IsDead() { return m_State & IS_DEAD; }
	bool IsCollision() { return m_State & IS_COLLISION; }

	virtual int DrawBBox();

	virtual bool Hit(int pattern);

	virtual bool Init(int hp, POSITION pos, float velo, Vector2 dir);
	virtual bool Move(float dTime) = 0;
	virtual bool Draw(float dTime, float alpha = 1.0f) = 0;
	virtual void Release();
};
