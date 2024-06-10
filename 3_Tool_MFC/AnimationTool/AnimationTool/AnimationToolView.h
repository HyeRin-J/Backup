
// AnimationToolView.h: CAnimationToolView 클래스의 인터페이스
//

#pragma once

class CAnimationToolDoc;
struct SaveData;

class CAnimationToolView : public CScrollView
{
protected: // serialization에서만 만들어집니다.
	CAnimationToolView() noexcept;
	DECLARE_DYNCREATE(CAnimationToolView)

	// 특성입니다.
public:
	CAnimationToolDoc* GetDocument() const;

	// 작업입니다.
public:

	// 재정의입니다.
public:
	virtual void OnDraw(CDC* pDC);  // 이 뷰를 그리기 위해 재정의되었습니다.
	virtual BOOL PreCreateWindow(CREATESTRUCT& cs);
protected:
	virtual BOOL OnPreparePrinting(CPrintInfo* pInfo);
	virtual void OnBeginPrinting(CDC* pDC, CPrintInfo* pInfo);
	virtual void OnEndPrinting(CDC* pDC, CPrintInfo* pInfo);

	// 구현입니다.
public:
	virtual ~CAnimationToolView();
#ifdef _DEBUG
	virtual void AssertValid() const;
	virtual void Dump(CDumpContext& dc) const;
#endif

protected:

	// 생성된 메시지 맵 함수
protected:
	DECLARE_MESSAGE_MAP()

	bool		m_IsLButtonDown = false;

	float		m_ZoomScale = 1.f;

	CDC			m_BackDC;
	CBitmap		m_BackBitmap;
public:
	CPoint		m_MousePos[2] = { { 50, 50 }, {100, 100} };

	void		AddImageList(bool isAdd = true);

	virtual void OnInitialUpdate();
	afx_msg void OnLButtonDown(UINT nFlags, CPoint point);
	afx_msg void OnLButtonUp(UINT nFlags, CPoint point);
	afx_msg void OnMouseMove(UINT nFlags, CPoint point);
//	afx_msg void OnMouseHWheel(UINT nFlags, short zDelta, CPoint pt);

};

#ifndef _DEBUG  // AnimationToolView.cpp의 디버그 버전
inline CAnimationToolDoc* CAnimationToolView::GetDocument() const
{
	return reinterpret_cast<CAnimationToolDoc*>(m_pDocument);
}
#endif

