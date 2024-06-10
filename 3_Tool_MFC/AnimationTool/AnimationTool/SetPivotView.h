#pragma once


// CSetPivotView 보기

class CSetPivotView : public CView
{
	DECLARE_DYNCREATE(CSetPivotView)

protected:
	CSetPivotView();           // 동적 만들기에 사용되는 protected 생성자입니다.
	virtual ~CSetPivotView();

public:
	virtual void OnDraw(CDC* pDC);      // 이 뷰를 그리기 위해 재정의되었습니다.
#ifdef _DEBUG
	virtual void AssertValid() const;
#ifndef _WIN32_WCE
	virtual void Dump(CDumpContext& dc) const;
#endif
#endif

protected:
	DECLARE_MESSAGE_MAP()
public:
	virtual void OnInitialUpdate();
	afx_msg void OnLButtonDown(UINT nFlags, CPoint point);
	afx_msg void OnKeyDown(UINT nChar, UINT nRepCnt, UINT nFlags);

	CDC		m_BackDC;
	CBitmap m_BackBitmap;

	bool	m_IsPivotMode = true;
	bool	m_IsCTRLDown = false;

	CPoint	m_Pivot = { 0, 0 };
	CRect	m_Boundary = { 0, 0, 0, 0 };

	float	m_Delay = 1.f;
	afx_msg void OnLButtonUp(UINT nFlags, CPoint point);
	afx_msg void OnKeyUp(UINT nChar, UINT nRepCnt, UINT nFlags);
};


