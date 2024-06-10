#pragma once


// MapSizeDialog 대화 상자

class MapSizeDialog : public CDialogEx
{
	DECLARE_DYNAMIC(MapSizeDialog)

public:
	MapSizeDialog(CWnd* pParent = nullptr);   // 표준 생성자입니다.
	virtual ~MapSizeDialog();

// 대화 상자 데이터입니다.
#ifdef AFX_DESIGN_TIME
	enum { IDD = IDD_DLG_MAPSIZE };
#endif

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV 지원입니다.

	DECLARE_MESSAGE_MAP()
public:
	int m_Width, m_Height;		// 현재 타일 비트맵의 Width, Height

	// 컨트롤들
	CSpinButtonCtrl m_WidthSpinCtrl;
	CSpinButtonCtrl m_HeightSpinCtrl;
	CEdit m_EditWidth;
	CEdit m_EditHeight;

	// 컨트롤에 연결한 함수
	afx_msg void OnDeltaposSpin1(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnDeltaposSpin2(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnBnClickedOk();
	afx_msg void OnBnClickedCancel();
	afx_msg void OnEnChangeEdit1();
	afx_msg void OnEnChangeEdit2();
};
