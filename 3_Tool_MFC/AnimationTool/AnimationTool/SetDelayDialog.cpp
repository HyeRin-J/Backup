// SetDelayDialog.cpp: 구현 파일
//

#include "pch.h"
#include "AnimationTool.h"
#include "SetDelayDialog.h"
#include "afxdialogex.h"
#include "AnimationToolDoc.h"
#include "SetPivotView.h"
#include "MainFrm.h"


// SetDelayDialog 대화 상자

IMPLEMENT_DYNAMIC(SetDelayDialog, CDialogEx)

SetDelayDialog::SetDelayDialog(CWnd* pParent /*=nullptr*/)
	: CDialogEx(IDD_DIALOG1, pParent)
{

}

SetDelayDialog::~SetDelayDialog()
{
}

void SetDelayDialog::DoDataExchange(CDataExchange* pDX)
{
	CDialogEx::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_SPIN1, m_TimerSpin);
	m_TimerSpin.SetRange(0, 100);
	DDX_Control(pDX, IDC_EDIT1, m_DelayEdit);
}


BEGIN_MESSAGE_MAP(SetDelayDialog, CDialogEx)
	ON_NOTIFY(UDN_DELTAPOS, IDC_SPIN1, &SetDelayDialog::OnDeltaposSpin1)
	ON_BN_CLICKED(IDOK, &SetDelayDialog::OnBnClickedOk)
END_MESSAGE_MAP()


// SetDelayDialog 메시지 처리기


void SetDelayDialog::OnDeltaposSpin1(NMHDR* pNMHDR, LRESULT* pResult)
{
	LPNMUPDOWN pNMUpDown = reinterpret_cast<LPNMUPDOWN>(pNMHDR);
	// TODO: 여기에 컨트롤 알림 처리기 코드를 추가합니다.

	int val = pNMUpDown->iPos + pNMUpDown->iDelta;

	if (val >= 0 && val <= 100)
	{
		CString str;
		str.Format(_T("%.2f"), val / 100.f);
		m_DelayEdit.SetWindowText(str);
	}

	*pResult = 0;
}


void SetDelayDialog::OnBnClickedOk()
{
	// TODO: 여기에 컨트롤 알림 처리기 코드를 추가합니다.
	//CDialogEx::OnOK();
	CAnimationToolDoc* pDoc = (CAnimationToolDoc*)((CMainFrame*)AfxGetMainWnd())->GetActiveDocument();
	
	CString str;

	m_DelayEdit.GetWindowText(str);
	pDoc->m_SaveData[pDoc->m_SelectIndex].delay = _ttof(str);

	((CMainFrame*)AfxGetMainWnd())->m_pPivotView->Invalidate(TRUE);
}
