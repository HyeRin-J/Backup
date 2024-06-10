
// AnimationToolView.cpp: CAnimationToolView 클래스의 구현
//

#include "pch.h"
#include "framework.h"
// SHARED_HANDLERS는 미리 보기, 축소판 그림 및 검색 필터 처리기를 구현하는 ATL 프로젝트에서 정의할 수 있으며
// 해당 프로젝트와 문서 코드를 공유하도록 해 줍니다.
#ifndef SHARED_HANDLERS
#include "AnimationTool.h"
#endif
#include "MainFrm.h"
#include "SelectedImageView.h"
#include "SetPivotView.h"
#include "AnimationToolDoc.h"
#include "AnimationToolView.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// CAnimationToolView

IMPLEMENT_DYNCREATE(CAnimationToolView, CScrollView)

BEGIN_MESSAGE_MAP(CAnimationToolView, CScrollView)
	// 표준 인쇄 명령입니다.
	ON_COMMAND(ID_FILE_PRINT, &CView::OnFilePrint)
	ON_COMMAND(ID_FILE_PRINT_DIRECT, &CView::OnFilePrint)
	ON_COMMAND(ID_FILE_PRINT_PREVIEW, &CView::OnFilePrintPreview)
	ON_WM_LBUTTONDOWN()
	ON_WM_LBUTTONUP()
	ON_WM_MOUSEMOVE()
//	ON_WM_MOUSEHWHEEL()
END_MESSAGE_MAP()

// CAnimationToolView 생성/소멸

CAnimationToolView::CAnimationToolView() noexcept
{
	// TODO: 여기에 생성 코드를 추가합니다.

}

CAnimationToolView::~CAnimationToolView()
{
	DeleteObject(m_BackBitmap);
	DeleteObject(m_BackDC);
}

BOOL CAnimationToolView::PreCreateWindow(CREATESTRUCT& cs)
{
	// TODO: CREATESTRUCT cs를 수정하여 여기에서
	//  Window 클래스 또는 스타일을 수정합니다.

	return CView::PreCreateWindow(cs);
}

// CAnimationToolView 그리기

void CAnimationToolView::OnDraw(CDC* pDC)
{
	CAnimationToolDoc* pDoc = GetDocument();
	ASSERT_VALID(pDoc);
	if (!pDoc)
		return;

	// TODO: 여기에 그리기 코드를 추가합니다.
	// 클립 영역(그리기 영역)의 정보를 RECT에 저장
	RECT rc;
	pDC->GetClipBox(&rc);

	m_BackDC.Rectangle(rc.left - 1, rc.top - 1, rc.right + 1, rc.bottom + 1);

	// 새 브러쉬랑 펜을 만든다
	CBrush gridBrush, * oldBrush;
	CPen gridPen, rectPen, * oldPen;

	gridBrush.CreateStockObject(NULL_BRUSH);
	gridPen.CreatePen(PS_SOLID, 1, RGB(100, 100, 100));

	// 새로 만든 브러쉬랑 펜을 선택
	oldBrush = m_BackDC.SelectObject(&gridBrush);
	oldPen = m_BackDC.SelectObject(&gridPen);

	//선택 사각형을 위해서
	rectPen.CreatePen(PS_SOLID, 2, RGB(200, 20, 20));
	m_BackDC.SelectObject(rectPen);

	pDoc->m_ShowImage.StretchBlt(m_BackDC.GetSafeHdc(), 0, 0,
		pDoc->m_ShowImage.GetWidth() * m_ZoomScale, pDoc->m_ShowImage.GetHeight() * m_ZoomScale,
		SRCCOPY);

	m_BackDC.Rectangle(
		m_MousePos[0].x - 2,
		m_MousePos[0].y - 2,
		m_MousePos[1].x + 2,
		m_MousePos[1].y + 2);

	// backDC를 mainDC로 출력
	pDC->BitBlt(0, 0, rc.right, rc.bottom, &m_BackDC, 0, 0, SRCCOPY);

	// 이전 브러쉬랑 펜으로 설정
	m_BackDC.SelectObject(oldBrush);
	m_BackDC.SelectObject(oldPen);

	// 미니맵 뷰도 같이 갱신
	//((CMainFrame*)AfxGetMainWnd())->m_pMiniMapView->Invalidate(FALSE);
	// TODO: 여기에 원시 데이터에 대한 그리기 코드를 추가합니다.
}


// CAnimationToolView 인쇄

BOOL CAnimationToolView::OnPreparePrinting(CPrintInfo* pInfo)
{
	// 기본적인 준비
	return DoPreparePrinting(pInfo);
}

void CAnimationToolView::OnBeginPrinting(CDC* /*pDC*/, CPrintInfo* /*pInfo*/)
{
	// TODO: 인쇄하기 전에 추가 초기화 작업을 추가합니다.
}

void CAnimationToolView::OnEndPrinting(CDC* /*pDC*/, CPrintInfo* /*pInfo*/)
{
	// TODO: 인쇄 후 정리 작업을 추가합니다.
}


// CAnimationToolView 진단

#ifdef _DEBUG
void CAnimationToolView::AssertValid() const
{
	CView::AssertValid();
}

void CAnimationToolView::Dump(CDumpContext& dc) const
{
	CView::Dump(dc);
}

CAnimationToolDoc* CAnimationToolView::GetDocument() const // 디버그되지 않은 버전은 인라인으로 지정됩니다.
{
	ASSERT(m_pDocument->IsKindOf(RUNTIME_CLASS(CAnimationToolDoc)));
	return (CAnimationToolDoc*)m_pDocument;
}
#endif //_DEBUG


// CAnimationToolView 메시지 처리기


void CAnimationToolView::OnInitialUpdate()
{
	// TODO: 여기에 특수화된 코드를 추가 및/또는 기본 클래스를 호출합니다.
	CDC* pDC = GetDC();

	// 만약 m_BackDC가 이미 있으면 다시 만들 필요 없다
	if (!m_BackDC.GetSafeHdc())
	{
		m_BackDC.CreateCompatibleDC(pDC);
	}

	// Bitmap이 만들어져 있는 상태면 삭제한다
	if (m_BackBitmap.GetSafeHandle())
	{
		m_BackBitmap.DeleteObject();
	}

	CAnimationToolDoc* pDoc = (CAnimationToolDoc*)GetDocument();
	CImage& img = pDoc->m_ShowImage;

	m_BackBitmap.CreateCompatibleBitmap(pDC, img.GetWidth(), img.GetHeight());
	CBitmap* oldBitmap = (CBitmap*)m_BackDC.SelectObject(m_BackBitmap);

	Invalidate(TRUE);

	CSize sizeViewPageTotal;
	sizeViewPageTotal.cx = img.GetWidth();
	sizeViewPageTotal.cy = img.GetHeight();

	SetScrollSizes(MM_TEXT, sizeViewPageTotal);

	CView::OnInitialUpdate();
}


void CAnimationToolView::OnLButtonDown(UINT nFlags, CPoint point)
{
	// TODO: 여기에 메시지 처리기 코드를 추가 및/또는 기본값을 호출합니다.
	m_IsLButtonDown = true;

	CPoint scrPoint = GetScrollPosition();

	//타일 정보를 저장한다.
	m_MousePos[0] = point + scrPoint;
	m_MousePos[1] = point + scrPoint;

	// 내가 선택한 지점
	int clickX = point.x + scrPoint.x;
	int clickY = point.y + scrPoint.y;

	// 연결된 Document 포인터
	// GetDocument()도 가능
	CAnimationToolDoc* pDoc = ((CAnimationToolDoc*)m_pDocument);

	//화면을 갱신한다.
	Invalidate(FALSE);
}


void CAnimationToolView::OnLButtonUp(UINT nFlags, CPoint point)
{
	// TODO: 여기에 메시지 처리기 코드를 추가 및/또는 기본값을 호출합니다.
	if (m_IsLButtonDown == false) return;

	m_IsLButtonDown = false;

	CPoint scrPoint = GetScrollPosition();

	m_MousePos[1] = point + scrPoint;
	CAnimationToolDoc* pDoc = (CAnimationToolDoc*)GetDocument();
	CImage& img = pDoc->m_ShowImage;

	if (m_MousePos[0].x < 0 || m_MousePos[0].x > img.GetWidth()) return;
	if (m_MousePos[0].y < 0 || m_MousePos[0].y > img.GetHeight()) return;

	int x1 = m_MousePos[0].x >= 0 && m_MousePos[0].x < img.GetWidth() ? m_MousePos[0].x : 0;
	int y1 = m_MousePos[0].y >= 0 && m_MousePos[0].y < img.GetHeight() ? m_MousePos[0].y : 0;
	int x2 = m_MousePos[1].x >= 0 && m_MousePos[1].x < img.GetWidth() ? m_MousePos[1].x : img.GetWidth() - 1;
	int y2 = m_MousePos[1].y >= 0 && m_MousePos[1].y < img.GetHeight() ? m_MousePos[1].y : img.GetHeight() - 1;

	if (m_MousePos[0].x > m_MousePos[1].x)
	{
		int temp = m_MousePos[1].x;
		m_MousePos[1].x = m_MousePos[0].x;
		m_MousePos[0].x = temp;
	}

	if (m_MousePos[0].y > m_MousePos[1].y)
	{
		int temp = m_MousePos[1].y;
		m_MousePos[1].y = m_MousePos[0].y;
		m_MousePos[0].y = temp;
	}

	int y = m_MousePos[0].y;

	while (img.GetPixel(x1, y) == pDoc->m_ColorKey)
	{
		y++;

		if (y >= y2)
		{
			y = 0;
			x1++;
		}
		if (x1 >= x2)
		{
			break;
		}
	}

	int x = m_MousePos[0].x;

	while (img.GetPixel(x, y1) == pDoc->m_ColorKey)
	{
		x++;

		if (x >= x2)
		{
			x = 0;
			y1++;
		}
		if (y1 >= img.GetHeight())
		{
			break;
		}
	}

	y = m_MousePos[0].y;
	while (img.GetPixel(x2, y) == pDoc->m_ColorKey)
	{
		y++;

		if (y >= y2)
		{
			y = 0;
			x2--;
		}
		if (x2 < 0)
		{
			break;
		}
	}

	x = m_MousePos[0].x;

	while (img.GetPixel(x, y2) == pDoc->m_ColorKey)
	{
		x++;

		if (x >= x2)
		{
			x = 0;
			y2--;
		}
		if (y2 < 0)
		{
			break;
		}
	}

	m_MousePos[0].x = x1;
	m_MousePos[0].y = y1;
	m_MousePos[1].x = x2;
	m_MousePos[1].y = y2;

	Invalidate(FALSE);

	AddImageList(true);
}


void CAnimationToolView::OnMouseMove(UINT nFlags, CPoint point)
{
	// TODO: 여기에 메시지 처리기 코드를 추가 및/또는 기본값을 호출합니다.
	if (!m_IsLButtonDown) return;

	CPoint scrPoint = GetScrollPosition();

	m_MousePos[1] = point + scrPoint;

	Invalidate(FALSE);
}


//void CAnimationToolView::OnMouseHWheel(UINT nFlags, short zDelta, CPoint pt)
//{
//	// 이 기능을 사용하려면 Windows Vista 이상이 있어야 합니다.
//	// _WIN32_WINNT 기호는 0x0600보다 크거나 같아야 합니다.
//	// TODO: 여기에 메시지 처리기 코드를 추가 및/또는 기본값을 호출합니다.
//
//	CView::OnMouseHWheel(nFlags, zDelta, pt);
//}

void CAnimationToolView::AddImageList(bool isAdd)
{
	CAnimationToolDoc* pDoc = (CAnimationToolDoc*)GetDocument();

	int width = m_MousePos[1].x - m_MousePos[0].x;
	int height = m_MousePos[1].y - m_MousePos[0].y;

	CDC tempDC;
	tempDC.CreateCompatibleDC(GetDC());

	CBitmap tempBitmap;
	tempBitmap.CreateCompatibleBitmap(GetDC(), width, height);

	if (tempBitmap.GetSafeHandle() == nullptr) return;

	tempDC.SelectObject(tempBitmap);

	CBrush backBrush;
	backBrush.CreateSolidBrush(RGB(255, 255, 255));
	tempDC.SelectObject(backBrush);
	tempDC.Rectangle(-1, -1, width + 1, height + 1);

	TransparentBlt(tempDC, 0, 0, width, height,	pDoc->m_ShowImage.GetDC(), m_MousePos[0].x, m_MousePos[0].y, width, height, pDoc->m_ColorKey);

	pDoc->m_SnapImage = new CImage();
	pDoc->m_SnapImage->Create(width, height, 32, 0);

	if ((HBITMAP)pDoc->m_SnapImage == nullptr) return;

	BitBlt(pDoc->m_SnapImage->GetDC(), 0, 0, width, height, tempDC, 0, 0, SRCCOPY);

	pDoc->m_ImageList.push_back(pDoc->m_SnapImage);

	if (isAdd)
	{
		SaveData data = { m_MousePos[0], {width, height}, {width / 2, height / 2}, {0, 0, width, height}, .16f };

		pDoc->m_SelectIndex = pDoc->m_ImageList.size() - 1;

		pDoc->m_SaveData.push_back(data);
	}

	pDoc->m_SnapImage = nullptr;

	((CMainFrame*)AfxGetMainWnd())->m_pSelectedView->UpdateImageList();
	((CMainFrame*)AfxGetMainWnd())->m_pPivotView->Invalidate(FALSE);
}
