#pragma once


// CSelectedImageView 보기

class CSelectedImageView : public CListView
{
	DECLARE_DYNCREATE(CSelectedImageView)

protected:
	CSelectedImageView();           // 동적 만들기에 사용되는 protected 생성자입니다.
	virtual ~CSelectedImageView();

public:
#ifdef _DEBUG
	virtual void AssertValid() const;
#ifndef _WIN32_WCE
	virtual void Dump(CDumpContext& dc) const;
#endif
#endif

protected:
	DECLARE_MESSAGE_MAP()

	int			m_SelectIndex = 0;
	CMenu		m_PopUpMenu;
	CImageList m_ImageList;
	
public:
	virtual void OnInitialUpdate();
	void UpdateImageList();
	void OnClickDeleteButton();

	afx_msg void OnNMClick(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnNMRClick(NMHDR* pNMHDR, LRESULT* pResult);
};


