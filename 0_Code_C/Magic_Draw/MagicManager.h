#pragma once

class Magic;

#define MAGIC_MAX_ 4

class MagicManager
{
	static MagicManager* m_Instance;

public:
	std::vector<POINT> m_DrawPoints;

	SPRITE m_MagicStarEffect;
	ANISPRITE m_MagicExploSprite;
	ANISPRITE m_MagicSprites[4];
	Magic* m_Magics[MAGIC_MAX_];

	MagicManager();

	void Init();
	void Initialize();

	static MagicManager* GetInstance();
	
	void CreateMagic();

	void CheckCollision();

	void MoveMagic();
	void DrawMagic();

	void Release();

	int CheckShape();
};

