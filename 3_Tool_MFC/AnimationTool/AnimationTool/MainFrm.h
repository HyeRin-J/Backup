
// MainFrm.h: CMainFrame 클래스의 인터페이스
//

#pragma once

class CAnimationPlayView;
class CAnimationToolView;
class CSelectedImageView;
class CSetPivotView;

class CMainFrame : public CFrameWnd
{
	
protected: // serialization에서만 만들어집니다.
	CMainFrame() noexcept;
	DECLARE_DYNCREATE(CMainFrame)

// 특성입니다.
public:

// 작업입니다.
public:

// 재정의입니다.
public:
	virtual BOOL PreCreateWindow(CREATESTRUCT& cs);

// 구현입니다.
public:
	virtual ~CMainFrame();
#ifdef _DEBUG
	virtual void AssertValid() const;
	virtual void Dump(CDumpContext& dc) const;
#endif

protected:  // 컨트롤 모음이 포함된 멤버입니다.
	CToolBar		m_wndToolBar;
	CSplitterWnd	m_wndSplitter[3];	//커스텀 윈도우 스플리터

// 생성된 메시지 맵 함수
protected:
	afx_msg int OnCreate(LPCREATESTRUCT lpCreateStruct);
	afx_msg void SetColorKey();
	afx_msg void OnClickPlayButton();
	afx_msg void OnClickStopButton();
	afx_msg void OnClickBoundButton();
	afx_msg void OnClickPivotButton();
	afx_msg void OnClickSetDelayButton();


	DECLARE_MESSAGE_MAP()

	virtual BOOL OnCreateClient(LPCREATESTRUCT lpcs, CCreateContext* pContext);
public:
	CAnimationPlayView* m_pPlayView;
	CAnimationToolView* m_pMainView;
	CSelectedImageView* m_pSelectedView;
	CSetPivotView*		m_pPivotView;
	
	void ChangeColorKey();
};


