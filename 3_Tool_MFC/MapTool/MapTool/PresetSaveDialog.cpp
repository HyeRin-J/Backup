// PresetSaveDialog.cpp: 구현 파일
//

#include "pch.h"
#include "MapTool.h"
#include "PresetSaveDialog.h"
#include "afxdialogex.h"

#include "MainFrm.h"
#include "MapItemTreeView.h"
#include "MapToolDoc.h"
#include "TileViewerDialog.h"

// CPresetSaveDialog 대화 상자

IMPLEMENT_DYNAMIC(CPresetSaveDialog, CDialogEx)

CPresetSaveDialog::CPresetSaveDialog(CWnd* pParent /*=nullptr*/)
	: CDialogEx(IDD_PRESET_DLG, pParent)
{

}

CPresetSaveDialog::~CPresetSaveDialog()
{
}

void CPresetSaveDialog::DoDataExchange(CDataExchange* pDX)
{
	CDialogEx::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_EDIT1, m_NameEdit);
}


BEGIN_MESSAGE_MAP(CPresetSaveDialog, CDialogEx)
	ON_BN_CLICKED(IDOK, &CPresetSaveDialog::OnBnClickedOk)
END_MESSAGE_MAP()


// CPresetSaveDialog 메시지 처리기
// OK 버튼 누를때
void CPresetSaveDialog::OnBnClickedOk()
{
	// TODO: 여기에 컨트롤 알림 처리기 코드를 추가합니다.	 
	CMainFrame* mainFrame = (CMainFrame*)AfxGetMainWnd();
	CMapToolDoc* pDoc = (CMapToolDoc*)mainFrame->GetActiveDocument();

	// 저장하기 위한 아이템 Edit에서 값 가져오기
	CString name;
	m_NameEdit.GetWindowText(name);
	
	// 트리뷰의 아이템 저장 함수 호출
	mainFrame->m_pMapItemTreeView->AddItem(pDoc->m_pTileViewerDlg->m_SelectedTiles, name);
	
	CDialogEx::OnOK();
}
