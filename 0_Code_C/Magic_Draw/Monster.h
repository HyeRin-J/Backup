#pragma once
#include "Unit.h"

#define IS_CREATED 0x00000004

class Monster :
    public Unit
{
protected:
    int m_Type = 1;

    float m_DeadTime = 0;
public:
    PANISPRITE m_AniSprite = nullptr;
    PANISPRITE m_DeadAniSprite = nullptr;

    Monster();

    bool    IsCreated() {  return m_State & IS_CREATED; }

    int GetType() { return m_Type; }

    void SetDeadTime(float time) { m_DeadTime = time; }

    bool SetSprites(ANISPRITE spr, ANISPRITE deadSpr);

    void Init();
    bool Init(int hp, POSITION pos, float velo, Vector2 dir, int type);
   // bool Init(int hp, POSITION pos, float velo, Vector2 dir, SPRITE spr, int type);

    bool DeadAnimation(float dTime);

    virtual bool Hit(int pattern);         //맞았는가
    virtual bool Draw(float dTime, float alpha = 1.0f);		//유닛 그리기
    virtual bool Move(float dTime);	    //유닛 움직이기
    
    virtual void Release();
};