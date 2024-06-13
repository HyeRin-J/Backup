<div align="center">
  <h1 style="border-bottom: 1px solid #21262d; color: #c9d1d9;"> 💻 Code_C </h1>
  <div style="font-weight: 700; font-size: 15px; text-align: center; color: #c9d1d9;"> c언어로 작성한 코드 모음입니다. </div>
   <br>
</div>

<details open> <summary>
<h2 style="border-bottom: 1px solid #21262d; color: #c9d1d9;"> ⭐ Magic Draw </h2> </summary>

<h3 style="border-bottom: 1px solid #21262d; color: #c9d1d9;">영상</h3>

[![MagicDraw](http://img.youtube.com/vi/qgwIbG5afvg/0.jpg)](https://youtu.be/qgwIbG5afvg)
<h3 style="border-bottom: 1px solid #21262d; color: #c9d1d9;"> 세부 사항 </h3>
  <h5 style="border-bottom: 1px solid #21262d; color: #c9d1d9;"> 몬스터의 모양을 보고 모양과 맞는 도형을 마우스로 그려서 몬스터를 처치하는 게임 </h5>
  <br>
  <ul style="display: table;  margin: auto;">
    <li>🎮 장르 : 퍼즐 디펜스 </li>
    <li>📅 개발 기간 : 2주 </li>
    <li>🙋 개발 인원 : 5명    
      <ul style="border-bottom: 1px;">
        <li>프로그래머 1</li>
        <li>기획 1</li>
        <li>아트 3</li>
      </ul>
    </li>
    <li>📃 개발 환경 : WinAPI, C++ </li>
    <li>🛠️ 사용 기술</li>
     <ul style="border-bottom: 1px;">
        <li>Vector2D 내적 각도 계산을 통한 도형 인식</li>
        <li>TransparentBlt</li>
        <li>AlphaBlend Sprite Animation</li>
        <li>Double Buffering</li>
        <li>AABB Collision Check</li>
      </ul>
  </ul>
데모 : 

[![Google Drive](https://img.shields.io/badge/Google%20Drive-4285F4?style=for-the-badge&logo=googledrive&logoColor=white)](https://drive.google.com/file/d/1b6BMPeU7mA5v1ZF33V7YpUNQHKAA1mXw/view?usp=sharing)

[Full Source Code](https://github.com/HyeRin-J/MagicDraw/)</li>

<h3 style="border-bottom: 1px solid #21262d; color: #c9d1d9;"> Source Code </h3>

<li>Check Shape</li>

```cpp
int MagicManager::CheckShape()
{
	//사이즈가 적으면 판단이 안 되서 리턴
	if (m_DrawPoints.size() < 3) return m_DrawPoints.size() - 1;

	//생긴 벡터의 개수
	std::vector<Vector2> vectorList;

	//노드 지정
	POINT firstPoint = m_DrawPoints[0];
	POINT curr = m_DrawPoints[1];
	POINT next = m_DrawPoints[2];

	//처음 벡터
	Vector2 firstVector = { (double)curr.x - firstPoint.x, (double)curr.y - firstPoint.y };
	firstVector.Normalize();

	//다음 벡터
	Vector2 v2 = { (double)next.x - curr.x, (double)next.y - curr.y };
	v2.Normalize();

	//벡터의 끝지점까지
	for (unsigned int i = 3; i < m_DrawPoints.size(); i++)
	{
		//각도 계산
		double angle = Vector2::Angle(firstVector, v2);

		//각도가 적으면 무시..
		if (angle < 28)
		{
			if (curr.x != firstPoint.x && curr.y != firstPoint.y)
			{
				firstVector = { (double)curr.x - firstPoint.x, (double)curr.y - firstPoint.y };
				firstVector.Normalize();
			}			
			do 
			{
				curr = next;
				next = m_DrawPoints[i];
				v2 = { (double)next.x - curr.x, (double)next.y - curr.y };
				i++;
				if (i >= m_DrawPoints.size()) break;
			} while (v2.Length() < 20);
			v2.Normalize();
		}
		//각도가 크면 이전 벡터를 넣어놓고 새로운 벡터를 생성
		else
		{
			if (curr.x != firstPoint.x && curr.y != firstPoint.y)
			{
				firstVector = { (double)curr.x - firstPoint.x, (double)curr.y - firstPoint.y };
				firstVector.Normalize();
				vectorList.push_back(firstVector);
			}

			POINT prev = curr;

			//새 벡터 생성하는 부분
			do
			{
				prev = curr;
				curr = next;
				next = m_DrawPoints[i];

				Vector2 v1 = { (double)curr.x - prev.x, (double)curr.y - prev.y };
				v1.Normalize();

				Vector2 v2 = { (double)next.x - curr.x, (double)next.y - curr.y };
				v2.Normalize();

				angle = Vector2::Angle(v1, v2);
			} while (angle > 28);	//각도가 큰 동안 계속 반복한다

			//새 벡터를 생성하기
			firstPoint = prev;
			if (curr.x != firstPoint.x && curr.y != firstPoint.y)
			{
				firstVector = { (double)curr.x - firstPoint.x, (double)curr.y - firstPoint.y };
				firstVector.Normalize();
			}
		}
	}

	//벡터의 사이즈 + 마지막 벡터(+1)
	System::GetInstance()->m_ShapeNum = vectorList.size() + 1;
	return vectorList.size() + 1;
}

```

<li>Check Collision</li>

```cpp
bool Magic::CheckCollision()
{
	for (int i = 0; i < ENEMY_MAX_; i++)
	{
		Monster* mon = UnitManager::GetInstance()->m_Monsters[i];

		if (!mon->IsCreated() || mon->IsDead()) continue;
		
		if (UnitManager::GetInstance()->CheckCollision(GetBBox(), mon->GetBBox()))
		{
			m_CollTime = GetTickCount64();
			if (mon->Hit(m_Type) && StageInfo == Stage::stage)
			{
				g_Score += UnitManager::m_Score[i / 20][(i / 10) % 2];
				UnitManager::GetInstance()->CheckStage();
			}
			return true;
		}
	}
	if (StageInfo == Stage::boss)
	{
		BossMonster* boss = UnitManager::GetInstance()->m_BossMonster;

		if (UnitManager::GetInstance()->CheckCollision(GetBBox(), boss->GetBBox()))
		{
			m_CollTime = GetTickCount64();
			boss->Hit(m_Type);

			return true;
		}
	}
	return false;
}
```

<li>RenderAll</li>

```cpp
void System::RenderAll()
{
	BeginRendering();	//렌더링 시작

	//백그라운드 출력
	DrawSprite(backBufferDC, 0, 0, &m_BackgroundSprites[(int)BK_SPR_NUM::BACKGROUND], 10, 10);

	//일반 스테이지와 보스 스테이지 판단
	switch (StageInfo)
	{
	case Stage::title:
		m_BossGuideTimer = 0;
		m_State = 0;
		DrawSprite(backBufferDC, 0, 0, &System::GetInstance()->m_BackgroundSprites[(int)BK_SPR_NUM::TITLE]);
		break;
	case Stage::loading:
		DrawAnimation(m_DTime, 4, 0, 0, &m_LoadingSprite);
		break;

	case Stage::tutorial:
		DrawAlphaSprite(backBufferDC, 0, 0,
			m_BackgroundSprites[(int)BK_SPR_NUM::TUTORIAL].Width, m_BackgroundSprites[(int)BK_SPR_NUM::TUTORIAL].Height,
			&m_BackgroundSprites[(int)BK_SPR_NUM::TUTORIAL]);

		break;
	case Stage::stage:
		UnitManager::GetInstance()->DrawMonster();

		PrintText(550, 120, RGB(255, 255, 255), "WAVE %d / 3", WaveCount + 1);

		break;
	case Stage::stageClear:
	{
		DrawSprite(backBufferDC, 0, 0, &m_BackgroundSprites[(int)BK_SPR_NUM::ENDING], 0, 0);
		DrawAlphaSprite(backBufferDC, 450, 200,
			m_BackgroundSprites[(int)BK_SPR_NUM::CLEAR].Width, m_BackgroundSprites[(int)BK_SPR_NUM::CLEAR].Height,
			&m_BackgroundSprites[(int)BK_SPR_NUM::CLEAR]);

		PrintText(520, 400, RGB(255, 255, 255), "Total Score : %d", g_Score);
	}
	break;
	case Stage::introBoss:
		UnitManager::GetInstance()->DrawBoss();
		UnitManager::GetInstance()->DrawMonster();
		DrawAlphaSprite(backBufferDC, 0, 0, m_ScreenSize.cx, m_ScreenSize.cy, &m_BackgroundSprites[(int)BK_SPR_NUM::INTROBOSS]);
		break;
	case Stage::boss:

		m_BossGuideTimer += m_DTime;

		if (UnitManager::GetInstance()->m_BossMonster->IsAttacking())
		{
			if (UnitManager::GetInstance()->m_BossMonster->m_AttackSprite->aniFrame >= 5)
			{
				ShakeBackGround();
				SoundManager::GetInstance()->PlayOnce(SoundList::BOSS_ATTACK);
				UnitManager::GetInstance()->m_IsHeroHit = true;
				if (UnitManager::GetInstance()->m_Hero->BossHit())
				{
					StageInfo = Stage::gameover;
				}
			}
		}

		if (!IsBossAttackStart())
		{
			DrawAlphaSprite(backBufferDC, 0, 0,
				m_BackgroundSprites[(int)BK_SPR_NUM::TUTORIAL_BOSS].Width, m_BackgroundSprites[(int)BK_SPR_NUM::TUTORIAL_BOSS].Height,
				&m_BackgroundSprites[(int)BK_SPR_NUM::TUTORIAL_BOSS]);
		}

		UnitManager::GetInstance()->DrawBoss();
		UnitManager::GetInstance()->DrawMonster();

		if (m_BossGuideTimer > 3.0f && UnitManager::GetInstance()->m_BossMonster->IsInitMoveFinish() && IsKeyDown(VK_LBUTTON))
			m_State |= BOSS_ATTACK_START;
		
		break;
	case Stage::gameover:
		DrawSprite(backBufferDC, 0, 0, &m_BackgroundSprites[(int)BK_SPR_NUM::ENDING], 0, 0);
		DrawAlphaSprite(backBufferDC, 430, 200,
			m_BackgroundSprites[(int)BK_SPR_NUM::GAME_OVER].Width, m_BackgroundSprites[(int)BK_SPR_NUM::GAME_OVER].Height,
			&m_BackgroundSprites[(int)BK_SPR_NUM::GAME_OVER]);

		PrintText(520, 400, RGB(255, 255, 255), "Total Score : %d", g_Score);
		break;
	default:
		break;
	}

	if (StageInfo == Stage::tutorial || StageInfo == Stage::stage || StageInfo == Stage::introBoss || StageInfo == Stage::boss)
	{
		DrawAlphaSprite(backBufferDC, 310, 0,
			m_BackgroundSprites[(int)BK_SPR_NUM::GUIDE].Width, m_BackgroundSprites[(int)BK_SPR_NUM::GUIDE].Height,
			&m_BackgroundSprites[(int)BK_SPR_NUM::GUIDE]);

		//주인공 그리기
		UnitManager::GetInstance()->DrawHero();

		//만들어진 마법을 그린다
		MagicManager::GetInstance()->DrawMagic();

		PrintText(1180, 20, RGB(0, 255, 255), "Score %4d", g_Score);
	}

	//도움말 출력
#if DEBUG_
	ShowHelp();
#endif

	//배경과 OR 연산
	GdiTransparentBlt(backBufferDC, 0, 0, 1440, 900, m_ShapeBufferDC, 0, 0, 1440, 900, RGB(0, 0, 0));

	//렌더 끝
	EndRendering();

	//메인 DC에 출력
	Flip();
}```
</details>
