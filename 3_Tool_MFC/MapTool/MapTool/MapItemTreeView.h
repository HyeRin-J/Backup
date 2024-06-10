#pragma once


// CMapItemTreeView 보기
#include "vector"
#include "algorithm"

struct SelectTiles;

class CMapItemTreeView : public CTreeView
{
	DECLARE_DYNCREATE(CMapItemTreeView)

protected:
	CMapItemTreeView();           // 동적 만들기에 사용되는 protected 생성자입니다.
	virtual ~CMapItemTreeView();

public:
#ifdef _DEBUG
	virtual void AssertValid() const;
#ifndef _WIN32_WCE
	virtual void Dump(CDumpContext& dc) const;
#endif
#endif

protected:
	DECLARE_MESSAGE_MAP()
public:
	CImageList m_ImageList;					// 트리에 표시할 이미지 리스트
	CMenu m_PopUpMenu;						// 팝업 메뉴
	HWND m_pTreeCtrl;						// 현재 트리 컨트롤

	HTREEITEM m_BackGroundRoot, m_StructureRoot;	// 노드의 루트 2개
	HTREEITEM m_LastSelectItem;				// 내가 마지막으로 선택한 아이템 포인터

	std::vector<SelectTiles*> m_Presets;	// 저장된 프리셋들의 포인터

	void AddItem(SelectTiles& tiles, CString name);		// 트리에 아이템 추가
	void DeleteItemInPreset(SelectTiles* tiles);			// 트리에서 아이템 삭제

	virtual void OnInitialUpdate();			// 현재 View가 처음으로 문서와 연결될 때 호출

	void OnClickDeleteButton();			// 팝업 메뉴에서 삭제 버튼을 눌렀을 경우

	afx_msg void OnTvnSelchanged(NMHDR* pNMHDR, LRESULT* pResult);		// 아이템 선택이 변경되었을 경우
	afx_msg void OnNMRClick(NMHDR* pNMHDR, LRESULT* pResult);			// 오른쪽 버튼 클릭
	afx_msg void OnDestroy();			// 뷰가 삭제되는 시점에 호출
};


