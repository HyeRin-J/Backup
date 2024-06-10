
// MapTool.h: MapTool 애플리케이션의 기본 헤더 파일
//
#pragma once

#ifndef __AFXWIN_H__
	#error "PCH에 대해 이 파일을 포함하기 전에 'pch.h'를 포함합니다."
#endif

#include "resource.h"       // 주 기호입니다.
#include "MapSizeDialog.h"
#include "vector"

// CMapToolApp:
// 이 클래스의 구현에 대해서는 MapTool.cpp을(를) 참조하세요.
//

// 타일 1개의 정보
struct MapTile
{
	int m_NumberX, m_NumberY;
	int m_Attribute;
};

// 기본 비트맵 타일들의 정보
struct MapFileAttributes
{
	int m_Width = 0, m_Height = 0;
	CImage m_MapBitmap;
	std::vector<MapTile> m_Attributes;
};

class CMapToolApp : public CWinApp
{
public:
	CMapToolApp() noexcept;


// 재정의입니다.
public:
	virtual BOOL InitInstance();

// 구현입니다.
	afx_msg void OnAppAbout();

	int m_MapWidth, m_MapHeight;

	// 불러온 비트맵 타일의 정보들
	MapFileAttributes m_MapTileAttribute;

	DECLARE_MESSAGE_MAP()
};

extern CMapToolApp theApp;
