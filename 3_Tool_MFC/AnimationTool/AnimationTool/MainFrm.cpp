
// MainFrm.cpp: CMainFrame 클래스의 구현
//

#include "pch.h"
#include "framework.h"
#include "AnimationToolDoc.h"
#include "AnimationToolView.h"
#include "AnimationPlayView.h"
#include "SelectedImageView.h"
#include "SetPivotView.h"
#include "SetDelayDialog.h"
#include "AnimationTool.h"

#include "MainFrm.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif

// CMainFrame

IMPLEMENT_DYNCREATE(CMainFrame, CFrameWnd)

BEGIN_MESSAGE_MAP(CMainFrame, CFrameWnd)
	ON_WM_CREATE()
	ON_COMMAND(ID_SET_COLOR_KEY, &CMainFrame::SetColorKey)
	ON_COMMAND(ID_PLAY_BTN, &CMainFrame::OnClickPlayButton)
	ON_COMMAND(ID_STOP_BTN, &CMainFrame::OnClickStopButton)
	ON_COMMAND(ID_SET_PIVOT, &CMainFrame::OnClickPivotButton)
	ON_COMMAND(ID_SET_BOUND, &CMainFrame::OnClickBoundButton)
	ON_COMMAND(ID_SET_DELAY, &CMainFrame::OnClickSetDelayButton)
END_MESSAGE_MAP()

static UINT indicators[] =
{
	ID_SEPARATOR,           // 상태 줄 표시기
	ID_INDICATOR_CAPS,
	ID_INDICATOR_NUM,
	ID_INDICATOR_SCRL,
};

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
	m_wndToolBar.SetButtonStyle(6, TBBS_CHECKGROUP);
	m_wndToolBar.SetButtonStyle(7, TBBS_CHECKGROUP);

	m_wndToolBar.SetButtonStyle(9, TBBS_CHECKGROUP);
	m_wndToolBar.SetButtonStyle(10, TBBS_CHECKGROUP);

	ChangeColorKey();

	return 0;
}


void CMainFrame::SetColorKey()
{
	CMFCColorDialog colorDlg;
	colorDlg.DoModal();

	CAnimationToolDoc* pDoc = (CAnimationToolDoc*)(m_pMainView->GetDocument());
	pDoc->m_ColorKey = colorDlg.GetColor();

	ChangeColorKey();
}

void CMainFrame::OnClickPlayButton()
{
	m_pPlayView->m_IsPlaying = true;
	m_pPlayView->Drawing();
}

void CMainFrame::OnClickStopButton()
{
	m_pPlayView->m_IsPlaying = false;
	m_pPlayView->KillTimer(0);
}

void CMainFrame::OnClickBoundButton()
{
	m_pPivotView->m_IsPivotMode = false;
}

void CMainFrame::OnClickPivotButton()
{
	m_pPivotView->m_IsPivotMode = true;
}

void CMainFrame::OnClickSetDelayButton()
{
	SetDelayDialog dlg;
	dlg.DoModal();
}

void CMainFrame::ChangeColorKey()
{
	CAnimationToolDoc* pDoc = (CAnimationToolDoc*)(m_pMainView->GetDocument());

	CToolBarCtrl& tbCtrl = m_wndToolBar.GetToolBarCtrl();
	CImageList* imageList = tbCtrl.GetImageList();
	IMAGEINFO info;
	imageList->GetImageInfo(8, &info);

	CDC dc;
	dc.CreateCompatibleDC(GetDC());

	CBitmap bitmap;
	bitmap.CreateCompatibleBitmap(GetDC(), info.rcImage.right - info.rcImage.left, info.rcImage.bottom - info.rcImage.top);

	dc.SelectObject(bitmap);

	CBrush newBrush;

	newBrush.CreateSolidBrush(pDoc->m_ColorKey);
	dc.SelectObject(newBrush);

	Rectangle(dc, 0, 0, info.rcImage.right, info.rcImage.bottom - info.rcImage.top);

	HBITMAP hbmMask = CreateCompatibleBitmap(::GetDC(NULL), info.rcImage.right, info.rcImage.bottom - info.rcImage.top);

	ICONINFO iInfo = { 0 };
	iInfo.fIcon = TRUE;
	iInfo.hbmColor = bitmap;
	iInfo.hbmMask = hbmMask;

	HICON hIcon = ::CreateIconIndirect(&iInfo);
	DeleteObject(hbmMask);

	imageList->Replace(3, hIcon);

	/*
	* CBitmap 저장
	CImage image;
	image.Attach(bitmap);
	image.Save(_T(".\\test.bmp"), Gdiplus::ImageFormatBMP);
	*/
	m_wndToolBar.Invalidate(TRUE);
}

BOOL CMainFrame::PreCreateWindow(CREATESTRUCT& cs)
{
	if (!CFrameWnd::PreCreateWindow(cs))
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
	m_wndSplitter[1].CreateStatic(this, 2, 1);
	//m_WndSplitter[1], 분할된 창의 뒤 쪽 창을 한 번 더 분할 한다.
	//세로로
	m_wndSplitter[0].CreateStatic(&m_wndSplitter[1], 1, 2, WS_CHILD | WS_VISIBLE | WS_BORDER, m_wndSplitter[1].IdFromRowCol(0, 0));
	m_wndSplitter[2].CreateStatic(&m_wndSplitter[1], 1, 2, WS_CHILD | WS_VISIBLE | WS_BORDER, m_wndSplitter[1].IdFromRowCol(1, 0));

	// 뷰(윈도우) 클래스 설정
	m_wndSplitter[0].CreateView(0, 0, RUNTIME_CLASS(CAnimationPlayView), CSize(), pContext);
	m_wndSplitter[0].CreateView(0, 1, RUNTIME_CLASS(CSelectedImageView), CSize(), pContext);
	m_wndSplitter[2].CreateView(0, 0, RUNTIME_CLASS(CSetPivotView), CSize(), pContext);
	m_wndSplitter[2].CreateView(0, 1, RUNTIME_CLASS(CAnimationToolView), CSize(), pContext);

	//가로, 세로 사이즈 지정
	m_wndSplitter[0].SetColumnInfo(0, 300, 0);
	m_wndSplitter[1].SetRowInfo(0, 300, 0);
	m_wndSplitter[2].SetColumnInfo(0, 500, 0);

	// 각 View들의 포인터를 받아놓고 사용
	m_pPlayView = static_cast<CAnimationPlayView*>(m_wndSplitter[0].GetPane(0, 0));
	m_pSelectedView = static_cast<CSelectedImageView*>(m_wndSplitter[0].GetPane(0, 1));

	m_pPivotView = static_cast<CSetPivotView*>(m_wndSplitter[2].GetPane(0, 0));
	m_pMainView = static_cast<CAnimationToolView*>(m_wndSplitter[2].GetPane(0, 1));

	return TRUE;
}
