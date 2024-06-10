// MiniMapView.cpp: 구현 파일
//

#include "pch.h"
#include "MapTool.h"
#include "MapToolDoc.h"
#include "MainFrm.h"
#include "MiniMapView.h"
#include "MapFileView.h"


// CMiniMapView

IMPLEMENT_DYNCREATE(CMiniMapView, CFormView)

CMiniMapView::CMiniMapView()
	: CFormView(IDD_CMiniMapView)
{

}

CMiniMapView::~CMiniMapView()
{
}

void CMiniMapView::DoDataExchange(CDataExchange* pDX)
{
	CFormView::DoDataExchange(pDX);

	//스크롤 제거
	CSize scrollSize(0, 0);
	SetScrollSizes(MM_TEXT, scrollSize);
}

BEGIN_MESSAGE_MAP(CMiniMapView, CFormView)
END_MESSAGE_MAP()


// CMiniMapView 진단

#ifdef _DEBUG
void CMiniMapView::AssertValid() const
{
	CFormView::AssertValid();
}

#ifndef _WIN32_WCE
void CMiniMapView::Dump(CDumpContext& dc) const
{
	CFormView::Dump(dc);
}
#endif
#endif //_DEBUG


// CMiniMapView 메시지 처리기

// 화면 갱신
void CMiniMapView::OnDraw(CDC* pDC)
{
	// 포인터
	CMainFrame* main = (CMainFrame*)AfxGetMainWnd();
	CMapToolDoc* pDoc = (CMapToolDoc*)main->GetActiveDocument();

	// 내가 여태까지 편집한 맵의 비트맵 그 자체
	BITMAP info;
	main->m_pMapFileView->m_BackBitmap.GetBitmap(&info);	

	// 백버퍼 DC
	CDC backDC;
	backDC.CreateCompatibleDC(pDC);

	// 미니 Bitmap
	CBitmap backBitmap;
	backBitmap.CreateCompatibleBitmap(pDC, pDoc->m_CustomMap.m_Width * MINIMAP_SIZE, pDoc->m_CustomMap.m_Height * MINIMAP_SIZE);
	
	// 연결
	backDC.SelectObject(backBitmap);

	// 클립 영역 정보
	RECT rc;
	pDC->GetClipBox(&rc);

	// 화면 지우기
	backDC.Rectangle(rc.left - 1, rc.top - 1, rc.right + 1, rc.bottom + 1);

	// 내가 편집중인 맵의 DC를 그대로 가져와서 StretchBlt으로 줄여서 출력한다.
	backDC.StretchBlt(0, 0, pDoc->m_CustomMap.m_Width * MINIMAP_SIZE, pDoc->m_CustomMap.m_Height * MINIMAP_SIZE,
		&main->m_pMapFileView->m_BackDC, 0, 0, info.bmWidth, info.bmHeight, SRCCOPY);

	// mainDC에 출력한다.
	pDC->BitBlt(0, 0, rc.right, rc.bottom, &backDC, 0, 0, SRCCOPY);
}
