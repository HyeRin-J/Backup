
// MainFrm.h: CMainFrame 클래스의 인터페이스
//

#pragma once
#include "CustomSplitterWnd.h"

class CMapFileView;
class CMiniMapView;
class CMapItemTreeView;

class CMainFrame : public CFrameWnd
{
	
protected: // serialization에서만 만들어집니다.
	CMainFrame() noexcept;
	DECLARE_DYNCREATE(CMainFrame)

// 특성입니다.
public:

// 작업입니다.
public:

// 재정의입니다.
public:
	virtual BOOL PreCreateWindow(CREATESTRUCT& cs);

// 구현입니다.
public:
	virtual ~CMainFrame();
#ifdef _DEBUG
	virtual void AssertValid() const;
	virtual void Dump(CDumpContext& dc) const;
#endif

protected:  // 컨트롤 모음이 포함된 멤버입니다.
	CToolBar			m_wndToolBar;			//툴바
	CCustomSplitterWnd		m_wndSplitter[2];	//커스텀 윈도우 스플리터

// 생성된 메시지 맵 함수
protected:
	afx_msg int OnCreate(LPCREATESTRUCT lpCreateStruct);	//생성될때
	DECLARE_MESSAGE_MAP()

	//윈도우 창 분할
	virtual BOOL OnCreateClient(LPCREATESTRUCT lpcs, CCreateContext* pContext);
public:
	//분할한 윈도우들을 가르키는 포인터
	CMapFileView* m_pMapFileView;
	CMiniMapView* m_pMiniMapView;
	CMapItemTreeView* m_pMapItemTreeView;
};


