// MapSizeDialog.cpp: 구현 파일
//

#include "pch.h"
#include "MapTool.h"
#include "MapToolView.h"
#include "MainFrm.h"
#include "MapSizeDialog.h"
#include "afxdialogex.h"


// MapSizeDialog 대화 상자

IMPLEMENT_DYNAMIC(MapSizeDialog, CDialogEx)

MapSizeDialog::MapSizeDialog(CWnd* pParent /*=nullptr*/)
	: CDialogEx(IDD_DLG_MAPSIZE, pParent)
{

}

MapSizeDialog::~MapSizeDialog()
{
}

void MapSizeDialog::DoDataExchange(CDataExchange* pDX)
{
	CDialogEx::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_SPIN1, m_WidthSpinCtrl);
	DDX_Control(pDX, IDC_SPIN2, m_HeightSpinCtrl);
	DDX_Control(pDX, IDC_EDIT1, m_EditWidth);
	DDX_Control(pDX, IDC_EDIT2, m_EditHeight);

	// 사이즈는 1부터 30까지
	m_WidthSpinCtrl.SetRange(1, 30);
	m_WidthSpinCtrl.SetPos(0);
	m_HeightSpinCtrl.SetRange(1, 17);
	m_HeightSpinCtrl.SetPos(0);

	//기본값
	m_EditWidth.SetWindowText(_T("1"));
	m_EditHeight.SetWindowText(_T("1"));
}


BEGIN_MESSAGE_MAP(MapSizeDialog, CDialogEx)
	ON_NOTIFY(UDN_DELTAPOS, IDC_SPIN1, &MapSizeDialog::OnDeltaposSpin1)
	ON_NOTIFY(UDN_DELTAPOS, IDC_SPIN2, &MapSizeDialog::OnDeltaposSpin2)
	ON_BN_CLICKED(IDOK, &MapSizeDialog::OnBnClickedOk)
	ON_BN_CLICKED(IDCANCEL, &MapSizeDialog::OnBnClickedCancel)
	ON_EN_CHANGE(IDC_EDIT1, &MapSizeDialog::OnEnChangeEdit1)
	ON_EN_CHANGE(IDC_EDIT2, &MapSizeDialog::OnEnChangeEdit2)
END_MESSAGE_MAP()


// MapSizeDialog 메시지 처리기


void MapSizeDialog::OnDeltaposSpin1(NMHDR* pNMHDR, LRESULT* pResult)
{
	LPNMUPDOWN pNMUpDown = reinterpret_cast<LPNMUPDOWN>(pNMHDR);
	// TODO: 여기에 컨트롤 알림 처리기 코드를 추가합니다.
	CString strTemp;

	int iVal = pNMUpDown->iPos + pNMUpDown->iDelta;

	//범위 제한
	if (iVal >= 0 && iVal <= 60)
	{
		strTemp.Format(_T("%d"), iVal);
		m_EditWidth.SetWindowText(strTemp);
	}

	*pResult = 0;
}


void MapSizeDialog::OnDeltaposSpin2(NMHDR* pNMHDR, LRESULT* pResult)
{
	LPNMUPDOWN pNMUpDown = reinterpret_cast<LPNMUPDOWN>(pNMHDR);
	// TODO: 여기에 컨트롤 알림 처리기 코드를 추가합니다.
	CString strTemp;

	int iVal = pNMUpDown->iPos + pNMUpDown->iDelta;

	//범위 제한
	if (iVal >= 0 && iVal <= 34)
	{
		strTemp.Format(_T("%d"), iVal);
		m_EditHeight.SetWindowText(strTemp);
	}

	*pResult = 0;
}


void MapSizeDialog::OnBnClickedOk()
{
	// TODO: 여기에 컨트롤 알림 처리기 코드를 추가합니다.
	CString strTemp;

	// 내가 입력한 정보를 가져와서
	m_EditWidth.GetWindowText(strTemp);
	m_Width = _ttoi(strTemp.GetBuffer());

	// width를 저장하자
	theApp.m_MapWidth = m_Width;

	m_EditHeight.GetWindowText(strTemp);
	m_Height = _ttoi(strTemp.GetBuffer());

	theApp.m_MapHeight = m_Height;

	CDialogEx::OnOK();
}


void MapSizeDialog::OnBnClickedCancel()
{
	// TODO: 여기에 컨트롤 알림 처리기 코드를 추가합니다.
	exit(0); // 프로그램 종료
}


void MapSizeDialog::OnEnChangeEdit1()
{
	// TODO:  RICHEDIT 컨트롤인 경우, 이 컨트롤은
	// CDialogEx::OnInitDialog() 함수를 재지정 
	//하고 마스크에 OR 연산하여 설정된 ENM_CHANGE 플래그를 지정하여 CRichEditCtrl().SetEventMask()를 호출하지 않으면
	// 이 알림 메시지를 보내지 않습니다.

	// TODO:  여기에 컨트롤 알림 처리기 코드를 추가합니다.
	CString strTemp;

	m_EditWidth.GetWindowText(strTemp);
	m_Width = _ttoi(strTemp.GetBuffer());

	// 범위 제한
	if (m_Width > 60)
	{
		m_Width = 60;
		strTemp.Format(_T("%d"), m_Width);
		m_EditWidth.SetWindowText(strTemp);
	}
	else if (m_Width < 1)
	{
		m_Width = 1;
		strTemp.Format(_T("%d"), m_Width);
		m_EditWidth.SetWindowText(strTemp);
	}
}


void MapSizeDialog::OnEnChangeEdit2()
{
	// TODO:  RICHEDIT 컨트롤인 경우, 이 컨트롤은
	// CDialogEx::OnInitDialog() 함수를 재지정 
	//하고 마스크에 OR 연산하여 설정된 ENM_CHANGE 플래그를 지정하여 CRichEditCtrl().SetEventMask()를 호출하지 않으면
	// 이 알림 메시지를 보내지 않습니다.

	// TODO:  여기에 컨트롤 알림 처리기 코드를 추가합니다.
	CString strTemp;

	m_EditHeight.GetWindowText(strTemp);
	m_Height = _ttoi(strTemp.GetBuffer());

	// 범위 제한
	if (m_Height > 34)
	{
		m_Height = 34;
		strTemp.Format(_T("%d"), m_Height);
		m_EditHeight.SetWindowText(strTemp);
	}
	else if (m_Height < 1)
	{
		m_Height = 1;
		strTemp.Format(_T("%d"), m_Height);
		m_EditHeight.SetWindowText(strTemp);
	}
}
