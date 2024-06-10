#pragma once


// CMapFileView 보기

#define CELL_SIZE 20

class CTileViewerDialog;

class CMapFileView : public CScrollView
{
	DECLARE_DYNCREATE(CMapFileView)

protected:
	CMapFileView();           // 동적 만들기에 사용되는 protected 생성자입니다.
	virtual ~CMapFileView();

public:
	virtual void OnDraw(CDC* pDC);      // 이 뷰를 그리기 위해 재정의되었습니다.
#ifdef _DEBUG
	virtual void AssertValid() const;
#ifndef _WIN32_WCE
	virtual void Dump(CDumpContext& dc) const;
#endif
#endif
	bool m_IsLButtonDown = false;
	bool m_IsRButtonDown = false;
protected:
	DECLARE_MESSAGE_MAP()
public:
	afx_msg void OnLButtonDown(UINT nFlags, CPoint point);	//왼쪽 마우스 버튼 클릭
	afx_msg void OnLButtonUp(UINT nFlags, CPoint point);	//왼쪽 마우스 버튼 뗄 때
	afx_msg void OnMouseMove(UINT nFlags, CPoint point);	//마우스 이동

	void SetTile(int clickX, int clickY);			//타일 정보 설정
	virtual void OnInitialUpdate();					//처음 문서랑 뷰가 연결될 때 호출된다.
	
	//미니맵 뷰에서 사용하기 위해 멤버변수화
	CDC m_BackDC;				
	CBitmap m_BackBitmap;
	CPoint m_MousePos;

	// vector의 index
	int m_StackIndex = 0;

	// undo, redo를 위한 스택
	// redo가 있어서 stack 대신 vector 사용
	std::vector<std::vector<MapTile>> m_MapBackGroundStack;
	std::vector<std::vector<MapTile>> m_StructureStack;

	void OnClickRedo();		// redo 클릭 or Ctrl + Z
	void OnClickUndo();		// undo 클릭 or Ctrl + Shift + Z

	afx_msg void OnRButtonDown(UINT nFlags, CPoint point);
	afx_msg void OnRButtonUp(UINT nFlags, CPoint point);
};


