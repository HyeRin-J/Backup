#pragma once


// SetDelayDialog 대화 상자

class SetDelayDialog : public CDialogEx
{
	DECLARE_DYNAMIC(SetDelayDialog)

public:
	SetDelayDialog(CWnd* pParent = nullptr);   // 표준 생성자입니다.
	virtual ~SetDelayDialog();

// 대화 상자 데이터입니다.
#ifdef AFX_DESIGN_TIME
	enum { IDD = IDD_DIALOG1 };
#endif

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV 지원입니다.

	DECLARE_MESSAGE_MAP()
public:
	afx_msg void OnDeltaposSpin1(NMHDR* pNMHDR, LRESULT* pResult);
	CSpinButtonCtrl m_TimerSpin;
	CEdit m_DelayEdit;
	afx_msg void OnBnClickedOk();
};

float GetSpinRangeFloatToInt(float lower, float upper, float delta);
