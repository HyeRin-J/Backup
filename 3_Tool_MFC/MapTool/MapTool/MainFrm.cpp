
// MainFrm.cpp: CMainFrame 클래스의 구현
//

#include "pch.h"

#include "framework.h"
#include "MapTool.h"
#include "MapFileView.h"
#include "MapToolView.h"
#include "MiniMapView.h"
#include "MapItemTreeView.h"
#include "MapItemTreeView.h"
#include "MainFrm.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif

// CMainFrame

IMPLEMENT_DYNCREATE(CMainFrame, CFrameWnd)

BEGIN_MESSAGE_MAP(CMainFrame, CFrameWnd)
	ON_WM_CREATE()
END_MESSAGE_MAP()

// CMainFrame 생성/소멸

CMainFrame::CMainFrame() noexcept
{
	// TODO: 여기에 멤버 초기화 코드를 추가합니다.
}

CMainFrame::~CMainFrame()
{
}

int CMainFrame::OnCreate(LPCREATESTRUCT lpCreateStruct)
{
	if (CFrameWnd::OnCreate(lpCreateStruct) == -1)
		return -1;

	//툴바를 만든다
	if (!m_wndToolBar.CreateEx(this, TBSTYLE_FLAT, WS_CHILD | WS_VISIBLE | CBRS_TOP | CBRS_GRIPPER | CBRS_TOOLTIPS | CBRS_FLYBY | CBRS_SIZE_DYNAMIC) ||
		!m_wndToolBar.LoadToolBar(IDR_MAINFRAME))
	{
		TRACE0("도구 모음을 만들지 못했습니다.\n");
		return -1;      // 만들지 못했습니다.
	}

	// TODO: 도구 모음을 도킹할 수 없게 하려면 이 세 줄을 삭제하십시오.
	m_wndToolBar.EnableDocking(CBRS_ALIGN_ANY);
	EnableDocking(CBRS_ALIGN_ANY);
	DockControlBar(&m_wndToolBar);

	return 0;
}

BOOL CMainFrame::PreCreateWindow(CREATESTRUCT& cs)
{
	if( !CFrameWnd::PreCreateWindow(cs) )
		return FALSE;
	// TODO: CREATESTRUCT cs를 수정하여 여기에서
	//  Window 클래스 또는 스타일을 수정합니다.
	
	return TRUE;
}

// CMainFrame 진단

#ifdef _DEBUG
void CMainFrame::AssertValid() const
{
	CFrameWnd::AssertValid();
}

void CMainFrame::Dump(CDumpContext& dc) const
{
	CFrameWnd::Dump(dc);
}
#endif //_DEBUG


// CMainFrame 메시지 처리기



BOOL CMainFrame::OnCreateClient(LPCREATESTRUCT lpcs, CCreateContext* pContext)
{
	// TODO: 여기에 특수화된 코드를 추가 및/또는 기본 클래스를 호출합니다.

	//뷰를 나눈다
	//좌, 우로 먼저 분할
	//WndSpliiter.CreateStatic(부모 창 포인터, 가로, 세로, 창 스타일, 아이디)
	m_wndSplitter[1].CreateStatic(this, 1, 2);
	//m_WndSplitter[1], 분할된 창의 뒤 쪽 창을 한 번 더 분할 한다.
	//세로로
	m_wndSplitter[0].CreateStatic(&m_wndSplitter[1], 2, 1, WS_CHILD | WS_VISIBLE | WS_BORDER, m_wndSplitter[1].IdFromRowCol(0, 0));

	// 뷰(윈도우) 클래스 설정
	m_wndSplitter[0].CreateView(0, 0, RUNTIME_CLASS(CMiniMapView), CSize(), pContext);
	m_wndSplitter[0].CreateView(1, 0, RUNTIME_CLASS(CMapItemTreeView), CSize(), pContext);
	m_wndSplitter[1].CreateView(0, 1, RUNTIME_CLASS(CMapFileView), CSize(), pContext);

	//가로, 세로 사이즈 지정
	m_wndSplitter[0].SetRowInfo(0, 275, 0);
	m_wndSplitter[1].SetColumnInfo(0, 482, 0);

	// 각 View들의 포인터를 받아놓고 사용
	m_pMapFileView = (CMapFileView*)m_wndSplitter[1].GetPane(0, 1);
	m_pMiniMapView = (CMiniMapView*)m_wndSplitter[0].GetPane(0, 0);
	m_pMapItemTreeView = (CMapItemTreeView*)m_wndSplitter[0].GetPane(1, 0);

	return TRUE;
}
