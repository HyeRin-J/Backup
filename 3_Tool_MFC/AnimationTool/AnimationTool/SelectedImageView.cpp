// SelectedImageView.cpp: 구현 파일
//

#include "pch.h"
#include "AnimationTool.h"
#include "MainFrm.h"
#include "SetPivotView.h"
#include "AnimationToolDoc.h"
#include "SelectedImageView.h"


// CSelectedImageView

IMPLEMENT_DYNCREATE(CSelectedImageView, CListView)

CSelectedImageView::CSelectedImageView()
{

}

CSelectedImageView::~CSelectedImageView()
{
	m_ImageList.DeleteImageList();
	DeleteObject(m_ImageList);
	DeleteObject(m_PopUpMenu);
}

BEGIN_MESSAGE_MAP(CSelectedImageView, CListView)
	ON_NOTIFY_REFLECT(NM_CLICK, &CSelectedImageView::OnNMClick)
	ON_NOTIFY_REFLECT(NM_RCLICK, &CSelectedImageView::OnNMRClick)
	ON_BN_CLICKED(ID_DELETE_IMAGE, &CSelectedImageView::OnClickDeleteButton)
END_MESSAGE_MAP()


// CSelectedImageView 진단

#ifdef _DEBUG
void CSelectedImageView::AssertValid() const
{
	CListView::AssertValid();
}

#ifndef _WIN32_WCE
void CSelectedImageView::Dump(CDumpContext& dc) const
{
	CListView::Dump(dc);
}
#endif
#endif //_DEBUG


// CSelectedImageView 메시지 처리기


void CSelectedImageView::OnInitialUpdate()
{
	CListView::OnInitialUpdate();

	// TODO: 여기에 특수화된 코드를 추가 및/또는 기본 클래스를 호출합니다.

	CListCtrl& listCtrl = GetListCtrl();

	if(listCtrl.GetSafeHwnd() == nullptr)
	{
		listCtrl.SetExtendedStyle(LVS_SHOWSELALWAYS | LVS_TYPEMASK | LVS_SINGLESEL | LVS_ICON);
		ListView_SetExtendedListViewStyle(listCtrl, LVS_EX_COLUMNSNAPPOINTS | LVS_EX_HEADERDRAGDROP);

		if (m_ImageList.GetSafeHandle() != nullptr)
		{
			DeleteObject(m_ImageList.Detach());
		}
		m_ImageList.Create(128, 128, ILC_COLOR24 | ILC_MASK, 2, 0);

		listCtrl.SetImageList(&m_ImageList, LVSIL_NORMAL);
	}
	else
	{
		UpdateImageList();
	}

	if (m_PopUpMenu.GetSafeHmenu() == nullptr)
	{
		m_PopUpMenu.LoadMenu(IDR_POPUP_MENU);
	}
	//칼럼 추가
	//listCtrl.InsertColumn(0, _T("이름"), LVCFMT_LEFT, 300);
}

void CSelectedImageView::UpdateImageList()
{
	CListCtrl& listCtrl = GetListCtrl();
	listCtrl.DeleteAllItems();

	m_ImageList.DeleteImageList();
	m_ImageList.Create(128, 128, ILC_COLOR24 | ILC_MASK, 0, 0);


	CAnimationToolDoc* pDoc = (CAnimationToolDoc*)GetDocument();

	for (int i = 0; i < pDoc->m_ImageList.size(); i++)
	{
		CImage* hImg = pDoc->m_ImageList[i];

		HBITMAP hbmMask = CreateCompatibleBitmap(::GetDC(NULL), hImg->GetWidth(), hImg->GetHeight());

		ICONINFO iInfo = { 0 };
		iInfo.fIcon = TRUE;
		iInfo.hbmColor = *hImg;
		iInfo.hbmMask = hbmMask;

		HICON hIcon = ::CreateIconIndirect(&iInfo);

		m_ImageList.Add(hIcon);
	}

	listCtrl.SetImageList(&m_ImageList, LVSIL_NORMAL);

	for (int i = 0; i < pDoc->m_ImageList.size(); i++)
	{
		CString name;
		name.Format(_T("%d"), listCtrl.GetItemCount());

		int index = listCtrl.InsertItem(i + 1, name, i);
		//listCtrl.Update(index);
	}

	for (int i = 0; i < pDoc->m_ImageList.size(); i++)
	{
		//CString name;
		//name.Format(_T("%d"), listCtrl.GetItemCount());
		//
		//int index = listCtrl.InsertItem(i + 1, name, i);
		listCtrl.Update(i);
	}
}

void CSelectedImageView::OnClickDeleteButton()
{
	CListCtrl& listCtrl = GetListCtrl();
	listCtrl.DeleteItem(m_SelectIndex);

	CAnimationToolDoc* pDoc = (CAnimationToolDoc*)(GetDocument());

	auto it = pDoc->m_ImageList.begin();
	auto it2 = pDoc->m_SaveData.begin();

	for (int i = 0; i < m_SelectIndex; i++, it++, it2++);

	CImage* img = pDoc->m_ImageList[m_SelectIndex];
	img->ReleaseDC();
	img->Destroy();
	delete img;
	img = nullptr;

	pDoc->m_ImageList.erase(it);
	pDoc->m_SaveData.erase(it2);

	--pDoc->m_SelectIndex;

	if (pDoc->m_SelectIndex < 0) pDoc->m_SelectIndex = 0;

	((CMainFrame*)AfxGetMainWnd())->m_pPivotView->Invalidate(TRUE);

	UpdateImageList();
}


void CSelectedImageView::OnNMClick(NMHDR* pNMHDR, LRESULT* pResult)
{
	LPNMITEMACTIVATE pNMItemActivate = reinterpret_cast<LPNMITEMACTIVATE>(pNMHDR);
	// TODO: 여기에 컨트롤 알림 처리기 코드를 추가합니다.
	int index = pNMItemActivate->iItem;

	if (index != -1)
	{
		CAnimationToolDoc* pDoc = (CAnimationToolDoc*)(GetDocument());
		pDoc->m_SelectIndex = index;
	}

	((CMainFrame*)AfxGetMainWnd())->m_pPivotView->Invalidate(TRUE);

	*pResult = 0;
}


void CSelectedImageView::OnNMRClick(NMHDR* pNMHDR, LRESULT* pResult)
{
	LPNMITEMACTIVATE pNMItemActivate = reinterpret_cast<LPNMITEMACTIVATE>(pNMHDR);
	// TODO: 여기에 컨트롤 알림 처리기 코드를 추가합니다.

	DWORD dwPos = GetMessagePos();
	CPoint pt(GET_X_LPARAM(dwPos), GET_Y_LPARAM(dwPos));

	int index = pNMItemActivate->iItem;
	m_SelectIndex = index;

	if (index != -1)
	{
		m_PopUpMenu.GetSubMenu(0)->TrackPopupMenu(TPM_LEFTALIGN, pt.x, pt.y, this);
	}
	*pResult = 0;
}
