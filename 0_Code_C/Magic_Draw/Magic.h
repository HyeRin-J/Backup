#pragma once

#define IS_FIRE 0x00000002

class Magic : public Unit
{
	int m_Type = 1;	//마법 타입

	long m_CollTime = 0;
public:
	PANISPRITE m_FireSprite;
	PANISPRITE m_ExploAniSprite;

	Magic();

	int GetType() { return m_Type; }

	virtual int DrawBBox();

	virtual bool Hit(BBOX colBox);

	bool Explosion(float dTime, float alpha);

	bool IsFire() { return m_State & IS_FIRE; }
	bool SetSprites(ANISPRITE fireSpr, ANISPRITE exploSpr);

	void Init();
	virtual bool Init(int hp, POSITION pos, float velo, Vector2 dir, int type = 0);
	virtual bool Move(float dTime);
	virtual bool Draw(float dTime, float alpha = 1.0f);

	virtual void Release();

	bool CheckCollision();

};

