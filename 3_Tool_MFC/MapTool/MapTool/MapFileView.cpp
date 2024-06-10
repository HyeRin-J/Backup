// MapFileView.cpp: 구현 파일
//

#include "pch.h"
#include "MapTool.h"
#include "MapToolDoc.h"
#include "TileViewerDialog.h"
#include "MapFileView.h"
#include "MainFrm.h"
#include "MapToolView.h"
#include "MiniMapView.h"


// CMapFileView

IMPLEMENT_DYNCREATE(CMapFileView, CScrollView)

CMapFileView::CMapFileView()
{

}

CMapFileView::~CMapFileView()
{
}

BEGIN_MESSAGE_MAP(CMapFileView, CScrollView)
	ON_WM_LBUTTONDOWN()
	ON_WM_LBUTTONUP()
	ON_WM_MOUSEMOVE()
	ON_COMMAND(ID_EDIT_REDO, &CMapFileView::OnClickRedo)
	ON_COMMAND(ID_EDIT_UNDO, &CMapFileView::OnClickUndo)
	ON_WM_RBUTTONDOWN()
	ON_WM_RBUTTONUP()
END_MESSAGE_MAP()


// CMapFileView 그리기

void CMapFileView::OnDraw(CDC* pDC)
{
	//연결된 문서를 가져온다.
	CMapToolDoc* pDoc = static_cast<CMapToolDoc*>(GetDocument());

	// TODO: 여기에 그리기 코드를 추가합니다.
	// 클립 영역(그리기 영역)의 정보를 RECT에 저장
	RECT rc;
	pDC->GetClipBox(&rc);

	// 맵의 백그라운드 타일
	CDC tileBackDC;
	tileBackDC.CreateCompatibleDC(pDC);
	tileBackDC.SelectObject(theApp.m_MapTileAttribute.m_MapBitmap);

	// 새 브러쉬랑 펜을 만든다
	CBrush gridBrush, * oldBrush;
	CPen gridPen, rectPen, * oldPen;

	gridBrush.CreateStockObject(NULL_BRUSH);
	gridPen.CreatePen(PS_SOLID, 1, RGB(100, 100, 100));

	// 새로 만든 브러쉬랑 펜을 선택
	oldBrush = m_BackDC.SelectObject(&gridBrush);
	oldPen = m_BackDC.SelectObject(&gridPen);

	if (pDoc->m_CustomMap.m_BackgroundImage != nullptr)
	{
		pDoc->m_CustomMap.m_BackgroundImage.BitBlt(m_BackDC, 0, 0, SRCCOPY);
	}

	// 내가 만든 맵의 세로, 가로 길이만큼 반복
	for (int y = 0; y < pDoc->m_CustomMap.m_Height; ++y)
	{
		for (int x = 0; x < pDoc->m_CustomMap.m_Width; ++x)
		{
			// 내가 x, y 위치에 지정한 타일의 정보를 가져온다.
			MapTile tempTile = pDoc->m_CustomMap.m_Background[pDoc->m_CustomMap.m_Width * y + x];

			//그 지점에서 선택된 타일의 인덱스
			int selectX = tempTile.m_NumberX;
			int selectY = tempTile.m_NumberY;

			if (selectX != -1 && selectY != -1)
			{
				// x, y 타일의 위치에 배경 정보를 출력한다.
				m_BackDC.StretchBlt(x * CELL_SIZE, y * CELL_SIZE, CELL_SIZE, CELL_SIZE, &tileBackDC, TILE_SIZE * selectX, TILE_SIZE * selectY, TILE_SIZE, TILE_SIZE, SRCCOPY);
			}

			if (pDoc->m_pTileViewerDlg->m_IsShowAttributes)
			{
				CString attribute;

				attribute.Format(_T("%d"), pDoc->m_CustomMap.m_Background[pDoc->m_CustomMap.m_Width * y + x].m_Attribute);

				m_BackDC.TextOut(x * CELL_SIZE, y * CELL_SIZE, attribute);
			}

			// 선택된 건물 비트맵 타일 인덱스
			selectX = tempTile.m_NumberX;
			selectY = tempTile.m_NumberY;

			//격자 출력
			m_BackDC.MoveTo(x * CELL_SIZE, y * CELL_SIZE);
			m_BackDC.LineTo(pDoc->m_CustomMap.m_Width * CELL_SIZE, y * CELL_SIZE);

			m_BackDC.MoveTo(x * CELL_SIZE, y * CELL_SIZE);
			m_BackDC.LineTo(x * CELL_SIZE, pDoc->m_CustomMap.m_Height * CELL_SIZE);
		}
	}

	//선택 사각형을 위해서
	rectPen.CreatePen(PS_SOLID, 2, RGB(200, 20, 20));
	m_BackDC.SelectObject(rectPen);

	m_BackDC.Rectangle(
		m_MousePos.x * CELL_SIZE,
		m_MousePos.y * CELL_SIZE,
		(m_MousePos.x + pDoc->m_pTileViewerDlg->m_SelectedTiles.m_Width) * CELL_SIZE,
		(m_MousePos.y + pDoc->m_pTileViewerDlg->m_SelectedTiles.m_Height) * CELL_SIZE);

	// backDC를 mainDC로 출력
	pDC->BitBlt(0, 0, rc.right, rc.bottom, &m_BackDC, 0, 0, SRCCOPY);

	// 이전 브러쉬랑 펜으로 설정
	m_BackDC.SelectObject(oldBrush);
	m_BackDC.SelectObject(oldPen);

	// 미니맵 뷰도 같이 갱신
	((CMainFrame*)AfxGetMainWnd())->m_pMiniMapView->Invalidate(FALSE);
}


// CMapFileView 진단

#ifdef _DEBUG
void CMapFileView::AssertValid() const
{
	CView::AssertValid();
}

#ifndef _WIN32_WCE
void CMapFileView::Dump(CDumpContext& dc) const
{
	CView::Dump(dc);
}
#endif
#endif //_DEBUG


// CMapFileView 메시지 처리기


void CMapFileView::OnLButtonDown(UINT nFlags, CPoint point)
{
	// TODO: 여기에 메시지 처리기 코드를 추가 및/또는 기본값을 호출합니다.

	// WM_MOUSEMOVE에서 사용한다.
	// 드래그를 처리하기 위해서
	m_IsLButtonDown = true;

	CPoint scrPoint = GetScrollPosition();

	// 내가 선택한 지점
	int clickX = (point.x + scrPoint.x) / CELL_SIZE;
	int clickY = (point.y + scrPoint.y) / CELL_SIZE;

	// 연결된 Document 포인터
	// GetDocument()도 가능
	CMapToolDoc* pDoc = ((CMapToolDoc*)m_pDocument);

	//생성된 맵을 벗어나게 클릭하는 경우 아무것도 하지 않는다.
	if (clickX >= pDoc->m_CustomMap.m_Width || clickY >= pDoc->m_CustomMap.m_Height)
		return;

	//타일 정보를 저장한다.
	SetTile(clickX, clickY);

	//화면을 갱신한다.
	Invalidate(FALSE);
}


void CMapFileView::OnLButtonUp(UINT nFlags, CPoint point)
{
	// TODO: 여기에 메시지 처리기 코드를 추가 및/또는 기본값을 호출합니다.

	// 마우스 다운 상태 해제
	m_IsLButtonDown = false;

	// 연결된 Document 포인터
	CMapToolDoc* pDoc = ((CMapToolDoc*)m_pDocument);

	// 스택에 추가
	// 현재 index가 1 이상이고, size 보다 작을 경우
	// 실행 취소를 1번 이상 한 상태이므로,
	// 현재 지점보다 더 나중의 데이터를 초기화한다.
	if (m_StackIndex >= 0 && m_StackIndex < m_MapBackGroundStack.size())
	{
		// 시작 지점
		auto iter = m_MapBackGroundStack.begin();

		// 시작지점부터, 내가 현재 위치한 index의 지점까지 iter를 이동
		for (int i = 0; i < m_StackIndex + 1 && iter != m_MapBackGroundStack.end(); i++)
			iter++;

		//iter 위치부터, end 요소까지 전부 삭제
		m_MapBackGroundStack.erase(iter, m_MapBackGroundStack.end());
	}

	// 제일 마지막에 현재의 타일 정보를 집어넣는다.
	m_MapBackGroundStack.push_back(pDoc->m_CustomMap.m_Background);

	// m_BackGroundStack과 같은 원리
	if (m_StackIndex >= 0 && m_StackIndex < m_StructureStack.size() - 1)
	{
		auto iter = m_StructureStack.begin();

		for (int i = 0; i < m_StackIndex + 1 && iter != m_StructureStack.end(); i++)
			iter++;

		m_StructureStack.erase(iter, m_StructureStack.end());
	}

	// 현재 index를 한칸 증가
	++m_StackIndex;
}


void CMapFileView::OnMouseMove(UINT nFlags, CPoint point)
{
	// TODO: 여기에 메시지 처리기 코드를 추가 및/또는 기본값을 호출합니다.
	CPoint scrPoint = GetScrollPosition();

	//현재 움직이고 있는 지점
	m_MousePos.x = (point.x + scrPoint.x) / CELL_SIZE;
	m_MousePos.y = (point.y + scrPoint.y) / CELL_SIZE;

	CMapToolDoc* pDoc = ((CMapToolDoc*)m_pDocument);

	// 드래그 중이 아니면 아무것도 하지 않음
	if (m_IsLButtonDown)
	{
		// 연결된 Document 포인터

		//생성된 맵을 벗어나게 클릭하는 경우 아무것도 하지 않는다.
		if (m_MousePos.x >= pDoc->m_CustomMap.m_Width || m_MousePos.y >= pDoc->m_CustomMap.m_Height)
			return;

		// 타일 정보 갱신
		SetTile(m_MousePos.x, m_MousePos.y);
	}

	else if (m_IsRButtonDown)
	{
		// 내가 선택한 타일의 높이와 너비 정보
		int selectTileHeight = pDoc->m_pTileViewerDlg->m_SelectedTiles.m_Height;
		int selectTileWidth = pDoc->m_pTileViewerDlg->m_SelectedTiles.m_Width;

		// selectedPoints가 1차원 벡터라서 index가 필요하다
		int index = 0;

		// 클릭한 y부터, y + selectHeight까지
		// 클릭한 x부터, x + selectWidth까지
		for (int y = m_MousePos.y; y < m_MousePos.y + selectTileHeight && y < pDoc->m_CustomMap.m_Height; ++y)
		{
			for (int x = m_MousePos.x; x < m_MousePos.x + selectTileWidth && x < pDoc->m_CustomMap.m_Width; ++x)
			{
				//타일 타입에 따라서 다르게 처리
				if (pDoc->m_pTileViewerDlg->m_SelectedTiles.m_TileType == 0)
				{
					//타일 객체 레퍼런스
					MapTile& tile = pDoc->m_CustomMap.m_Background[(pDoc->m_CustomMap.m_Width * y) + x];

					// 갱신
					tile.m_NumberX = -1;
					tile.m_NumberY = -1;
					tile.m_Attribute = 0;
				}
			}
		}
	}
	//화면 갱신
	Invalidate(FALSE);
}

/// <summary>
/// 타일 정보를 갱신하는 함수이다.
/// </summary>
/// <param name="clickX">선택한 x 지점</param>
/// <param name="clickY">선택한 y 지점</param>
void CMapFileView::SetTile(int clickX, int clickY)
{
	// 연결된 Document 포인터
	CMapToolDoc* pDoc = ((CMapToolDoc*)m_pDocument);

	// 만약에 선택해놓은 타일이 아무것도 없으면 아무것도 하지 않음
	if (pDoc->m_pTileViewerDlg->m_SelectedTiles.selectedPoints.size() <= 0)
		return;

	// 내가 선택한 타일의 높이와 너비 정보
	int selectTileHeight = pDoc->m_pTileViewerDlg->m_SelectedTiles.m_Height;
	int selectTileWidth = pDoc->m_pTileViewerDlg->m_SelectedTiles.m_Width;

	// selectedPoints가 1차원 벡터라서 index가 필요하다
	int index = 0;

	// 클릭한 y부터, y + selectHeight까지
	// 클릭한 x부터, x + selectWidth까지
	for (int y = clickY; y < clickY + selectTileHeight && y < pDoc->m_CustomMap.m_Height; ++y)
	{
		for (int x = clickX; x < clickX + selectTileWidth && x < pDoc->m_CustomMap.m_Width; ++x)
		{
			// 선택해 놓았던 위치의 point 정보를 가져온다
			CPoint& point = pDoc->m_pTileViewerDlg->m_SelectedTiles.selectedPoints[index++];
			int attribute = theApp.m_MapTileAttribute.m_Attributes[(theApp.m_MapTileAttribute.m_Width * point.y) + point.x].m_Attribute;

			//타일 타입에 따라서 다르게 처리
			if (pDoc->m_pTileViewerDlg->m_SelectedTiles.m_TileType == 0)
			{
				//타일 객체 레퍼런스
				MapTile& tile = pDoc->m_CustomMap.m_Background[(pDoc->m_CustomMap.m_Width * y) + x];

				// 갱신
				tile.m_NumberX = point.x;
				tile.m_NumberY = point.y;
				tile.m_Attribute = attribute;
			}
		}
	}
}


void CMapFileView::OnInitialUpdate()
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
		m_BackBitmap.DeleteObject();

	CMapToolDoc* pDoc = (CMapToolDoc*)GetDocument();

	// 맵의 사이즈 크기의 비트맵을 새로 만들고, backDC에 연결한다.
	m_BackBitmap.CreateCompatibleBitmap(pDC, CELL_SIZE * pDoc->m_CustomMap.m_Width, CELL_SIZE * pDoc->m_CustomMap.m_Height);
	m_BackDC.SelectObject(m_BackBitmap);

	//타일 선택 다이얼로그 출력
	static CTileViewerDialog tileViewer;

	// 만들어진 tileViewer가 없으면
	// 새로 생성하도록 하자
	if (!tileViewer.GetSafeHwnd())
	{
		tileViewer.Create(IDD_DLG_TILE_VIEWER, this);
		tileViewer.ModifyStyle(WS_POPUP, WS_CHILD);
		tileViewer.SetParent(this);
	}

	// Document에 포인터 지정
	pDoc->m_pTileViewerDlg = &tileViewer;

	CView::ModifyStyle(0, WS_CLIPCHILDREN);

	//화면에 표시
	tileViewer.ShowWindow(SW_SHOW);

	CSize sizeViewPageTotal;
	sizeViewPageTotal.cx = CELL_SIZE * pDoc->m_CustomMap.m_Width;
	sizeViewPageTotal.cy = CELL_SIZE * pDoc->m_CustomMap.m_Height;

	SetScrollSizes(MM_TEXT, sizeViewPageTotal);

	//초기 상태를 스택에 추가
	m_MapBackGroundStack.push_back(pDoc->m_CustomMap.m_Background);

	m_StackIndex++;


	CView::OnInitialUpdate();
}

// redo
void CMapFileView::OnClickRedo()
{
	// 인덱스가 범위를 벗어남
	if (m_StackIndex >= m_MapBackGroundStack.size() - 1) return;

	// 포인터들...
	CMainFrame* pFrame;
	pFrame = static_cast<CMainFrame*>(AfxGetMainWnd());

	CMapToolDoc* pDoc;
	pDoc = (CMapToolDoc*)GetDocument();

	// index를 증가하고
	++m_StackIndex;

	// 정보를 재설정
	pDoc->m_CustomMap.m_Background = m_MapBackGroundStack[m_StackIndex];

	// 화면 갱신
	pFrame->m_pMapFileView->Invalidate(FALSE);
}

// undo
void CMapFileView::OnClickUndo()
{
	// index가 0이면 되돌릴 게 없음
	if (m_StackIndex <= 0) return;

	// 포인터들
	CMainFrame* pFrame;
	pFrame = static_cast<CMainFrame*>(AfxGetMainWnd());

	CMapToolDoc* pDoc;
	pDoc = (CMapToolDoc*)GetDocument();

	// index를 감소하고
	--m_StackIndex;

	//정보 재 설정
	pDoc->m_CustomMap.m_Background = m_MapBackGroundStack[m_StackIndex];

	// 화면 갱신
	pFrame->m_pMapFileView->Invalidate(FALSE);
}

void CMapFileView::OnRButtonDown(UINT nFlags, CPoint point)
{
	// TODO: 여기에 메시지 처리기 코드를 추가 및/또는 기본값을 호출합니다.


	CPoint scrPoint = GetScrollPosition();

	// 내가 선택한 지점
	int clickX = (point.x + scrPoint.x) / CELL_SIZE;
	int clickY = (point.y + scrPoint.y) / CELL_SIZE;

	// 연결된 Document 포인터
	// GetDocument()도 가능
	CMapToolDoc* pDoc = ((CMapToolDoc*)m_pDocument);

	//생성된 맵을 벗어나게 클릭하는 경우 아무것도 하지 않는다.
	if (clickX >= pDoc->m_CustomMap.m_Width || clickY >= pDoc->m_CustomMap.m_Height)
		return;

	// 만약에 선택해놓은 타일이 아무것도 없으면 아무것도 하지 않음
	if (pDoc->m_pTileViewerDlg->m_SelectedTiles.selectedPoints.size() <= 0)
		return;

	m_IsRButtonDown = true;

	// 내가 선택한 타일의 높이와 너비 정보
	int selectTileHeight = pDoc->m_pTileViewerDlg->m_SelectedTiles.m_Height;
	int selectTileWidth = pDoc->m_pTileViewerDlg->m_SelectedTiles.m_Width;

	// selectedPoints가 1차원 벡터라서 index가 필요하다
	int index = 0;

	// 클릭한 y부터, y + selectHeight까지
	// 클릭한 x부터, x + selectWidth까지
	for (int y = clickY; y < clickY + selectTileHeight && y < pDoc->m_CustomMap.m_Height; ++y)
	{
		for (int x = clickX; x < clickX + selectTileWidth && x < pDoc->m_CustomMap.m_Width; ++x)
		{
			//타일 타입에 따라서 다르게 처리
			if (pDoc->m_pTileViewerDlg->m_SelectedTiles.m_TileType == 0)
			{
				//타일 객체 레퍼런스
				MapTile& tile = pDoc->m_CustomMap.m_Background[(pDoc->m_CustomMap.m_Width * y) + x];

				// 갱신
				tile.m_NumberX = -1;
				tile.m_NumberY = -1;
				tile.m_Attribute = 0;
			}
		}
	}

	//화면을 갱신한다.
	Invalidate(FALSE);
}


void CMapFileView::OnRButtonUp(UINT nFlags, CPoint point)
{
	// TODO: 여기에 메시지 처리기 코드를 추가 및/또는 기본값을 호출합니다.

	m_IsRButtonDown = false;

	CScrollView::OnRButtonUp(nFlags, point);
}
