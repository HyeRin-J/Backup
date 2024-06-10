#pragma once


// CAnimationPlayView 보기

class CAnimationPlayView : public CView
{
	DECLARE_DYNCREATE(CAnimationPlayView)

protected:
	CAnimationPlayView();           // 동적 만들기에 사용되는 protected 생성자입니다.
	virtual ~CAnimationPlayView();

public:
	virtual void OnDraw(CDC* pDC);      // 이 뷰를 그리기 위해 재정의되었습니다.
#ifdef _DEBUG
	virtual void AssertValid() const;
#ifndef _WIN32_WCE
	virtual void Dump(CDumpContext& dc) const;
#endif
#endif

protected:
	int		m_CurrIndex = 0;

	CDC		m_BackDC;
	CBitmap m_BackBitmap;
	DECLARE_MESSAGE_MAP()
public:
	bool	m_IsPlaying = false;


	void		Drawing();
	virtual void OnInitialUpdate();
	afx_msg void OnTimer(UINT_PTR nIDEvent);
};


