// MapItemTreeView.cpp: 구현 파일
//

#include "pch.h"
#include "MapTool.h"
#include "TileViewerDialog.h"
#include "MapItemTreeView.h"

#include "MainFrm.h"
#include "MapFileView.h"
#include "TileViewerDialog.h"
#include "MapToolDoc.h"

// CMapItemTreeView

IMPLEMENT_DYNCREATE(CMapItemTreeView, CTreeView)

CMapItemTreeView::CMapItemTreeView()
{

}

CMapItemTreeView::~CMapItemTreeView()
{
}

BEGIN_MESSAGE_MAP(CMapItemTreeView, CTreeView)
	ON_NOTIFY_REFLECT(TVN_SELCHANGED, &CMapItemTreeView::OnTvnSelchanged)
	ON_NOTIFY_REFLECT(NM_RCLICK, &CMapItemTreeView::OnNMRClick)
	ON_BN_CLICKED(ID_PRESET_DELETE, &CMapItemTreeView::OnClickDeleteButton)
	ON_WM_DESTROY()
END_MESSAGE_MAP()


// CMapItemTreeView 진단

#ifdef _DEBUG
void CMapItemTreeView::AssertValid() const
{
	CTreeView::AssertValid();
}

#ifndef _WIN32_WCE
void CMapItemTreeView::Dump(CDumpContext& dc) const
{
	CTreeView::Dump(dc);
}
#endif
#endif //_DEBUG


// CMapItemTreeView 메시지 처리기

/// <summary>
/// 트리에 아이템을 추가한다.
/// </summary>
/// <param name="tiles">추가할 선택한 타일들의 정보</param>
/// <param name="name">추가할 이름</param>
void CMapItemTreeView::AddItem(SelectTiles& tiles, CString name)
{
	// 트리 컨트롤을 가져온다
	CTreeCtrl& rTreeCtrl = GetTreeCtrl();

	// 추가한 아이템의 포인터를 저장할 때 사용할 변수
	HTREEITEM item;

	// 가져온 정보를 토대로 새로운 객체를 동적으로 생성
	SelectTiles* tile = new SelectTiles(tiles);
	// 프리셋포인터 추가
	m_Presets.push_back(tile);

	// 타입에 따라 다르게 처리
	if (tiles.m_TileType == 0)
	{
		// 배경 노드에 추가
		item = rTreeCtrl.InsertItem(name, 1, 1, m_BackGroundRoot, TVI_LAST);

		// 아이템 데이터에 동적 할당한 SelectTiles의 포인터를 저장한다.
		// 나중에 GetItemData를 통해서 포인터를 가져와서 정보를 가져올 수 있다.
		rTreeCtrl.SetItemData(item, (DWORD_PTR)m_Presets[m_Presets.size() - 1]);
		// 확장 형태
		rTreeCtrl.Expand(m_BackGroundRoot, TVE_EXPAND);
	}
	else
	{
		item = rTreeCtrl.InsertItem(name, 1, 1, m_StructureRoot, TVI_LAST);

		rTreeCtrl.SetItemData(item, (DWORD_PTR)m_Presets[m_Presets.size() - 1]);
		rTreeCtrl.Expand(m_StructureRoot, TVE_EXPAND);
	}
}

/// <summary>
/// 프리셋에서 아이템을 삭제한다
/// </summary>
/// <param name="tiles"></param>
void CMapItemTreeView::DeleteItemInPreset(SelectTiles* tiles)
{
	// 프리셋의 시작 지점부터, 끝지점까지 같은 주소값을 찾아서 그 위치를 반환
	auto pos = std::find(m_Presets.begin(), m_Presets.end(), tiles);

	delete *pos; // 그 지점을 삭제

	// 프리셋의 pos 위치 제거
	m_Presets.erase(pos);
}

void CMapItemTreeView::OnInitialUpdate()
{
	CTreeView::OnInitialUpdate();

	// TODO: 여기에 특수화된 코드를 추가 및/또는 기본 클래스를 호출합니다.

	// 이미 만들어진 컨트롤이 있으면 또 할 필요가 없다
	if (m_pTreeCtrl != NULL) return;

	// 팝업 메뉴 로딩
	m_PopUpMenu.LoadMenu(IDR_POPUP_MENU);

	// 컨트롤 가져오기
	CTreeCtrl& rTreeCtrl = GetTreeCtrl();
	// 컨트롤 저장
	m_pTreeCtrl = rTreeCtrl;
	//컨트롤의 스타일 지정
	rTreeCtrl.ModifyStyle(0, TVS_HASBUTTONS | TVS_HASLINES, SWP_FRAMECHANGED);

	// 이미지 리스트를 만든다.
	m_ImageList.Create(15, 15, ILC_COLOR24 | ILC_MASK, 2, 0);

	CBitmap bmp;
	bmp.LoadBitmap(IDB_FILE_BITMAP);

	m_ImageList.Add(&bmp, RGB(255, 0, 255));

	rTreeCtrl.SetImageList(&m_ImageList, TVSIL_NORMAL);

	// 루트노드 추가
	m_BackGroundRoot = rTreeCtrl.InsertItem(_T("배경"), 0, 0, TVI_ROOT, TVI_LAST);
	m_StructureRoot = rTreeCtrl.InsertItem(_T("건물"), 0, 0, TVI_ROOT, TVI_LAST);
}

/// <summary>
/// 팝업 메뉴에서 삭제를 눌렀을 때
/// </summary>
void CMapItemTreeView::OnClickDeleteButton()
{
	// 내가 마지막으로 선택한 아이템의 Data를 가져온다.
	// SelectTiles의 포인터
	SelectTiles* tile = (SelectTiles*)GetTreeCtrl().GetItemData(m_LastSelectItem);

	// 트리 컨트롤에서 마지막으로 선택한 아이템을 삭제한다.
	GetTreeCtrl().DeleteItem(m_LastSelectItem);
	m_LastSelectItem = NULL;	//포인터를 NULL로 초기화한다.

	//프리셋에서 제거한다.
	DeleteItemInPreset(tile);
}

void CMapItemTreeView::OnTvnSelchanged(NMHDR* pNMHDR, LRESULT* pResult)
{
	LPNMTREEVIEW pNMTreeView = reinterpret_cast<LPNMTREEVIEW>(pNMHDR);

	// 컨트롤 가져오기
	CTreeCtrl& rTreeCtrl = GetTreeCtrl();

	// TODO: 여기에 컨트롤 알림 처리기 코드를 추가합니다.
	// 메시지가 발생한 위치
	DWORD dwPos = GetMessagePos();

	// CPoint 형태로 변환
	CPoint pt(GET_X_LPARAM(dwPos), GET_Y_LPARAM(dwPos));
	CPoint spt;
	spt = pt;

	// Screen 좌표에서 View의 클라이언트 좌표로 변환해야 자연스럽다.
	ScreenToClient(&spt);
	UINT test;
	m_LastSelectItem = GetTreeCtrl().HitTest(spt, &test);	// spt 지점에서 아이템을 검출

	// 아이템 정보
	TVITEM tvi;
	tvi.hItem = m_LastSelectItem;
	tvi.mask = TVIF_PARAM;
	TreeView_GetItem(GetSafeHwnd(), &tvi);

	// 선택한 아이템이 있고, 루트 노드들이 아니라면
	if (m_LastSelectItem && m_LastSelectItem != m_BackGroundRoot && m_LastSelectItem != m_StructureRoot)
	{
		//저장된 정보를 가져온다.
		SelectTiles* tile = (SelectTiles*)(rTreeCtrl.GetItemData(m_LastSelectItem));

		//포인터들...
		CMainFrame* mainFrame = (CMainFrame*)AfxGetMainWnd();
		CMapToolDoc* pDoc = (CMapToolDoc*)mainFrame->GetActiveDocument();
		
		//현재 선택된 타일을 교체한다
		pDoc->m_pTileViewerDlg->m_SelectedTiles = *tile;

		// 저장된 정보로 초기화
		pDoc->m_pTileViewerDlg->m_FirstClickPoint = tile->selectedPoints[0];
		pDoc->m_pTileViewerDlg->m_LastClickPoint = tile->selectedPoints[tile->selectedPoints.size() - 1];

		//화면을 갱신한다
		pDoc->m_pTileViewerDlg->Invalidate(FALSE);
	}

	*pResult = 0;
}


void CMapItemTreeView::OnNMRClick(NMHDR* pNMHDR, LRESULT* pResult)
{
	// TODO: 여기에 컨트롤 알림 처리기 코드를 추가합니다.
	// 메시지 발생 위치
	DWORD dwPos = GetMessagePos();

	/* Convert the co-ords into a CPoint structure */
	CPoint pt(GET_X_LPARAM(dwPos), GET_Y_LPARAM(dwPos));
	CPoint spt;
	spt = pt;

	/* convert to screen co-ords for the hittesting to work */
	ScreenToClient(&spt);
	UINT test = 0;
	//선택한 아이템 반환
	m_LastSelectItem = GetTreeCtrl().HitTest(spt, &test);

	TVITEM tvi;
	tvi.hItem = m_LastSelectItem;
	tvi.mask = TVIF_PARAM;
	TreeView_GetItem(GetSafeHwnd(), &tvi);

	//팝업 메뉴 출력
	if (m_LastSelectItem != m_BackGroundRoot && m_LastSelectItem != m_StructureRoot)
	{
		m_PopUpMenu.GetSubMenu(0)->TrackPopupMenu(TPM_LEFTALIGN, pt.x, pt.y, this);
	}

	*pResult = 0;
}


void CMapItemTreeView::OnDestroy()
{
	auto iter = m_Presets.begin();

	//동적 생성된 데이터들 삭제
	for (int i = 0; i < m_Presets.size(); i++)
	{
		auto temp = iter++;
		delete* temp;
		m_Presets.erase(temp);
	}

	// TODO: 여기에 메시지 처리기 코드를 추가합니다.
	CTreeView::OnDestroy();
}
