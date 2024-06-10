// AnimationPlayView.cpp: 구현 파일
//

#include "pch.h"
#include "AnimationTool.h"
#include "AnimationToolDoc.h"
#include "AnimationPlayView.h"


// CAnimationPlayView

IMPLEMENT_DYNCREATE(CAnimationPlayView, CView)

CAnimationPlayView::CAnimationPlayView()
{

}

CAnimationPlayView::~CAnimationPlayView()
{
	DeleteObject(m_BackBitmap);
	DeleteObject(m_BackDC);
}

BEGIN_MESSAGE_MAP(CAnimationPlayView, CView)
	ON_WM_TIMER()
END_MESSAGE_MAP()


// CAnimationPlayView 그리기

void CAnimationPlayView::OnDraw(CDC* pDC)
{
	CAnimationToolDoc* pDoc = (CAnimationToolDoc*)GetDocument();
	// TODO: 여기에 그리기 코드를 추가합니다.

	CRect rc;
	pDC->GetClipBox(&rc);

	m_BackBitmap.CreateCompatibleBitmap(pDC, rc.Width(), rc.Height());
	m_BackDC.SelectObject(m_BackBitmap);

	m_BackDC.Rectangle(rc.left - 1, rc.top - 1, rc.right + 1, rc.bottom + 1);

	CPen gridXPen, gridYPen, * oldPen;
	gridXPen.CreatePen(PS_SOLID, 1, RGB(255, 0, 0));
	gridYPen.CreatePen(PS_SOLID, 1, RGB(0, 255, 0));

	if (pDoc->m_ImageList.size() > 0)
	{
		CImage* img = pDoc->m_ImageList[m_CurrIndex];
		SaveData& data = pDoc->m_SaveData[m_CurrIndex];
		img->BitBlt(m_BackDC.GetSafeHdc(), rc.Width() / 2 - data.pivot.x, rc.Height() / 2 - data.pivot.y, SRCCOPY);
	}

	oldPen = (CPen*)m_BackDC.SelectObject(&gridXPen);
	m_BackDC.MoveTo(0, rc.Height() / 2);
	m_BackDC.LineTo(rc.Width(), rc.Height() / 2);

	m_BackDC.SelectObject(&gridYPen);
	m_BackDC.MoveTo(rc.Width() / 2, 0);
	m_BackDC.LineTo(rc.Width() / 2, rc.Height());

	pDC->BitBlt(0, 0, rc.right - rc.left, rc.bottom - rc.top, &m_BackDC, 0, 0, SRCCOPY);

	m_BackDC.SelectObject(oldPen);
	m_BackBitmap.DeleteObject();
}


// CAnimationPlayView 진단

#ifdef _DEBUG
void CAnimationPlayView::AssertValid() const
{
	CView::AssertValid();
}

#ifndef _WIN32_WCE
void CAnimationPlayView::Dump(CDumpContext& dc) const
{
	CView::Dump(dc);
}
#endif
#endif //_DEBUG

// CAnimationPlayView 메시지 처리기


void CAnimationPlayView::Drawing()
{
	if (((CAnimationToolDoc*)GetDocument())->m_ImageList.size() == 0) return;
	if (m_IsPlaying == true)
	{
		SetTimer(0, 1, nullptr);
	}
}

void CAnimationPlayView::OnInitialUpdate()
{
	// TODO: 여기에 특수화된 코드를 추가 및/또는 기본 클래스를 호출합니다.
		// TODO: 여기에 특수화된 코드를 추가 및/또는 기본 클래스를 호출합니다.
	CDC* pDC = GetDC();

	// 만약 m_BackDC가 이미 있으면 다시 만들 필요 없다
	if (!m_BackDC.GetSafeHdc())
	{
		m_BackDC.CreateCompatibleDC(pDC);
	}

	if (m_IsPlaying == true)
	{
		KillTimer(0);
	}
	// Bitmap이 만들어져 있는 상태면 삭제한다

	Invalidate(TRUE);
}


void CAnimationPlayView::OnTimer(UINT_PTR nIDEvent)
{
	static int Time = 0;
	// TODO: 여기에 메시지 처리기 코드를 추가 및/또는 기본값을 호출합니다.
	Time++;

	if (Time >= ((CAnimationToolDoc*)GetDocument())->m_SaveData[m_CurrIndex].delay * 100)
	{
		Invalidate(FALSE);
		m_CurrIndex++;
		m_CurrIndex %= ((CAnimationToolDoc*)GetDocument())->m_ImageList.size();
		
		Time = 0;
	}
	CView::OnTimer(nIDEvent);
}
