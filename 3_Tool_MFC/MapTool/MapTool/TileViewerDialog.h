#pragma once
#include "vector"

#define OFFSET_X 10
#define OFFSET_Y 50
#define TILE_SIZE 32

// 선택하고 있는 타일 정보를 저장할 구조체
struct SelectTiles
{
	int m_Width = 1, m_Height = 1;
	int m_TileType = 0;
	std::vector<CPoint> selectedPoints = { CPoint(0, 0) };
};

// CTileViewerDialog 대화 상자
class CTileViewerDialog : public CDialogEx
{
	DECLARE_DYNAMIC(CTileViewerDialog)

public:
	CTileViewerDialog(CWnd* pParent = nullptr);   // 표준 생성자입니다.
	virtual ~CTileViewerDialog();

// 대화 상자 데이터입니다.
#ifdef AFX_DESIGN_TIME
	enum { IDD = IDD_DLG_TILE_VIEWER };
#endif

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV 지원입니다.

	void OnDraw(CDC* pDC);		//Custom OnDraw

	DECLARE_MESSAGE_MAP()
public:
	CComboBox m_TileAttributeSelectCombo;	// 타일의 속성을 변경하는 콤보박스
	CMenu m_PopUpMenu;				// 팝업 메뉴

	CPoint m_FirstClickPoint;		// 첫번째로 클릭한 point
	CPoint m_LastClickPoint;		// 마지막으로 클릭한 point
	CPoint m_SelectTileCount;		// 현재 처리 중인 point
	
	SelectTiles m_SelectedTiles;	// 현재 선택해놓은 타일들의 정보

	bool m_IsShowAttributes = false;	// 속성 보기
	bool m_IsLButtonDown = false;		// 드래그를 위해서

	virtual void OnOK();
	virtual void OnCancel();
	void OnClickSavePreset();

	afx_msg void OnPaint();
	afx_msg void OnLButtonDown(UINT nFlags, CPoint point);
	afx_msg void OnLButtonUp(UINT nFlags, CPoint point);
	afx_msg void OnMouseMove(UINT nFlags, CPoint point);
	afx_msg void OnRButtonUp(UINT nFlags, CPoint point);

	afx_msg void OnBnClickedCheck1();
	afx_msg void OnCbnSelchangeCombo2();
};
