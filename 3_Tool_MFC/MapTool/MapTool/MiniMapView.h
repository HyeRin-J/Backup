#pragma once

#define MINIMAP_SIZE 8

// CMiniMapView 폼 보기

class CMiniMapView : public CFormView
{
	DECLARE_DYNCREATE(CMiniMapView)

protected:
	CMiniMapView();           // 동적 만들기에 사용되는 protected 생성자입니다.
	virtual ~CMiniMapView();

public:
#ifdef AFX_DESIGN_TIME
	enum { IDD = IDD_CMiniMapView };
#endif
#ifdef _DEBUG
	virtual void AssertValid() const;
#ifndef _WIN32_WCE
	virtual void Dump(CDumpContext& dc) const;
#endif
#endif

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV 지원입니다.

	DECLARE_MESSAGE_MAP()
	virtual void OnDraw(CDC* pDC);
};


