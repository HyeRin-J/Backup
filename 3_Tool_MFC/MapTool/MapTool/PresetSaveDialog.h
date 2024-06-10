#pragma once


// CPresetSaveDialog 대화 상자

class CPresetSaveDialog : public CDialogEx
{
	DECLARE_DYNAMIC(CPresetSaveDialog)

public:
	CPresetSaveDialog(CWnd* pParent = nullptr);   // 표준 생성자입니다.
	virtual ~CPresetSaveDialog();

// 대화 상자 데이터입니다.
#ifdef AFX_DESIGN_TIME
	enum { IDD = IDD_PRESET_DLG};
#endif

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV 지원입니다.

	DECLARE_MESSAGE_MAP()
public:
	afx_msg void OnBnClickedOk();
	CEdit m_NameEdit;
};
