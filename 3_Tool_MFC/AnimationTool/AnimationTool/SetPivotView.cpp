// CSetPivotView.cpp: 구현 파일
//

#include "pch.h"
#include "AnimationTool.h"
#include "AnimationToolDoc.h"
#include "AnimationToolView.h"
#include "MainFrm.h"
#include "SetPivotView.h"


// CSetPivotView

IMPLEMENT_DYNCREATE(CSetPivotView, CView)

CSetPivotView::CSetPivotView()
{

}

CSetPivotView::~CSetPivotView()
{
	DeleteObject(m_BackBitmap);
	DeleteObject(m_BackDC);
}

BEGIN_MESSAGE_MAP(CSetPivotView, CView)
	ON_WM_LBUTTONDOWN()
	ON_WM_KEYDOWN()
	ON_WM_LBUTTONUP()
	ON_WM_KEYUP()
END_MESSAGE_MAP()


// CSetPivotView 그리기

void CSetPivotView::OnDraw(CDC* pDC)
{
	// TODO: 여기에 그리기 코드를 추가합니다.
	CAnimationToolDoc* pDoc = ((CAnimationToolDoc*)GetDocument());

	if (pDoc->m_ImageList.size() > 0)
	{
		m_Pivot = pDoc->m_SaveData[pDoc->m_SelectIndex].pivot;
		m_Boundary = pDoc->m_SaveData[pDoc->m_SelectIndex].boundary;
		m_Delay = pDoc->m_SaveData[pDoc->m_SelectIndex].delay;

		CImage* img = pDoc->m_ImageList[pDoc->m_SelectIndex];

		RECT rc;
		pDC->GetClipBox(&rc);

		m_BackDC.Rectangle(rc.left - 1, rc.top - 1, rc.right + 1, rc.bottom + 1);

		CBrush pivotBrush, boundBrush, * oldBrush;
		CPen pivotPen, boundPen, * oldPen;
		pivotBrush.CreateSolidBrush(RGB(255, 0, 255));
		pivotPen.CreateStockObject(NULL_PEN);

		boundBrush.CreateStockObject(HOLLOW_BRUSH);
		boundPen.CreatePen(BS_SOLID, 1, RGB(0, 255, 0));
		img->BitBlt(m_BackDC.GetSafeHdc(), 0, 0, SRCCOPY);

		oldBrush = (CBrush*)m_BackDC.SelectObject(&pivotBrush);
		oldPen = (CPen*)m_BackDC.SelectObject(&pivotPen);

		m_BackDC.Rectangle(m_Pivot.x - 3, m_Pivot.y - 3, m_Pivot.x + 3, m_Pivot.y + 3);

		m_BackDC.SelectObject(&boundBrush);
		m_BackDC.SelectObject(&boundPen);

		m_BackDC.Rectangle(m_Boundary.left, m_Boundary.top, m_Boundary.right, m_Boundary.bottom);

		CString str;
		str.Format(_T("Bitmap Pos(x : %d, y : %d)"), pDoc->m_SaveData[pDoc->m_SelectIndex].bitmapStartPoint.x, pDoc->m_SaveData[pDoc->m_SelectIndex].bitmapStartPoint.y);
		m_BackDC.TextOut(rc.right - str.GetLength() * 8, 30, str);

		str.Format(_T("Size(Width : %d, Height : %d)"), img->GetWidth(), img->GetHeight());
		m_BackDC.TextOut(rc.right - str.GetLength() * 8, 50, str);

		str.Format(_T("pivot(x : %d, y : %d)"), m_Pivot.x, m_Pivot.y);
		m_BackDC.TextOut(rc.right - str.GetLength() * 8, 70, str);

		str.Format(_T("boundary(L : %d, T : %d, R : %d, B : %d)"), m_Boundary.left, m_Boundary.top, m_Boundary.right, m_Boundary.bottom);
		m_BackDC.TextOut(rc.right - str.GetLength() * 8, 90, str);

		str.Format(_T("delay : %f"), m_Delay);
		m_BackDC.TextOut(rc.right - str.GetLength() * 8, 110, str);

		pDC->BitBlt(0, 0, rc.right, rc.bottom, &m_BackDC, 0, 0, SRCCOPY);
		m_BackDC.SelectObject(oldBrush);
		m_BackDC.SelectObject(oldPen);
	}
}


// CSetPivotView 진단

#ifdef _DEBUG
void CSetPivotView::AssertValid() const
{
	CView::AssertValid();
}

#ifndef _WIN32_WCE
void CSetPivotView::Dump(CDumpContext& dc) const
{
	CView::Dump(dc);
}
#endif
#endif //_DEBUG


// CSetPivotView 메시지 처리기


void CSetPivotView::OnLButtonDown(UINT nFlags, CPoint point)
{
	// TODO: 여기에 메시지 처리기 코드를 추가 및/또는 기본값을 호출합니다.
	CAnimationToolDoc* pDoc = ((CAnimationToolDoc*)GetDocument());

	if (pDoc->m_SaveData.size() > 0)
	{
		if (pDoc->m_SaveData[pDoc->m_SelectIndex].bitmapSize.cx < point.x ||
			pDoc->m_SaveData[pDoc->m_SelectIndex].bitmapSize.cy < point.y) return;

		if (m_IsPivotMode)
		{
			m_Pivot = point;
			pDoc->m_SaveData[pDoc->m_SelectIndex].pivot = point;
		}
		else
		{
			m_Boundary.left = point.x;
			m_Boundary.top = point.y;
			pDoc->m_SaveData[pDoc->m_SelectIndex].boundary = { point.x, point.y, point.x, point.y };
		}
	}
	Invalidate(FALSE);
}


void CSetPivotView::OnInitialUpdate()
{
	CView::OnInitialUpdate();

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

	CRect rc;
	GetClientRect(&rc);

	m_BackBitmap.CreateCompatibleBitmap(pDC, rc.Width(), rc.Height());
	CBitmap* oldBitmap = (CBitmap*)m_BackDC.SelectObject(m_BackBitmap);

	Invalidate(FALSE);
}


void CSetPivotView::OnKeyDown(UINT nChar, UINT nRepCnt, UINT nFlags)
{
	// TODO: 여기에 메시지 처리기 코드를 추가 및/또는 기본값을 호출합니다.

	CAnimationToolDoc* pDoc = ((CAnimationToolDoc*)GetDocument());

	if (pDoc->m_SaveData.size() > 0)
	{
		if (m_IsPivotMode == true)
		{
			switch (nChar)
			{
			case VK_LEFT:
				m_Pivot.x -= 1;
				break;
			case VK_RIGHT:
				m_Pivot.x += 1;
				break;
			case VK_UP:
				m_Pivot.y -= 1;
				break;
			case VK_DOWN:
				m_Pivot.y += 1;
				break;
			}

			if (pDoc->m_SaveData[pDoc->m_SelectIndex].bitmapSize.cx < m_Pivot.x)
			{
				m_Pivot.x = pDoc->m_SaveData[pDoc->m_SelectIndex].bitmapSize.cx;
			}
			else if (pDoc->m_SaveData[pDoc->m_SelectIndex].bitmapSize.cy < m_Pivot.y)
			{
				m_Pivot.y = pDoc->m_SaveData[pDoc->m_SelectIndex].bitmapSize.cy;
			}
			else if (m_Pivot.x < 0)
			{
				m_Pivot.x = 0;
			}
			else if (m_Pivot.y < 0)
			{
				m_Pivot.y = 0;
			}

			pDoc->m_SaveData[pDoc->m_SelectIndex].pivot = m_Pivot;
		}
		else
		{
			switch (nChar)
			{
			case VK_CONTROL:
				m_IsCTRLDown = true;
				break;
			case VK_LEFT:
				if (m_IsCTRLDown == true)
				{
					m_Boundary.right -= 1;
				}
				else
				{
					m_Boundary.left -= 1;
				}
				break;
			case VK_RIGHT:
				if (m_IsCTRLDown == true)
				{
					m_Boundary.left += 1;
				}
				else
				{
					m_Boundary.right += 1;
				}
				break;
			case VK_UP:
				if (m_IsCTRLDown == true)
				{
					m_Boundary.bottom -= 1;
				}
				else
				{
					m_Boundary.top -= 1;
				}
				break;
			case VK_DOWN:
				if (m_IsCTRLDown == true)
				{
					m_Boundary.top += 1;
				}
				else
				{
					m_Boundary.bottom += 1;
				}
				break;
			}

			pDoc->m_SaveData[pDoc->m_SelectIndex].boundary = m_Boundary;
		}
		Invalidate(FALSE);
	}
}


void CSetPivotView::OnLButtonUp(UINT nFlags, CPoint point)
{
	// TODO: 여기에 메시지 처리기 코드를 추가 및/또는 기본값을 호출합니다.
	CAnimationToolDoc* pDoc = ((CAnimationToolDoc*)GetDocument());

	if (pDoc->m_SaveData.size() > 0)
	{
		if (m_IsPivotMode == false)
		{
			m_Boundary.right = point.x;
			m_Boundary.bottom = point.y;

			pDoc->m_SaveData[pDoc->m_SelectIndex].boundary = m_Boundary;

			Invalidate(FALSE);
		}
	}

}


void CSetPivotView::OnKeyUp(UINT nChar, UINT nRepCnt, UINT nFlags)
{
	// TODO: 여기에 메시지 처리기 코드를 추가 및/또는 기본값을 호출합니다.

	switch (nChar)
	{
	case VK_CONTROL:
		m_IsCTRLDown = false;
		break;
	}
}
