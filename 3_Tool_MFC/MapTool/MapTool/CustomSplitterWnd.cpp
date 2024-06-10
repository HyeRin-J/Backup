#include "pch.h"
#include "CustomSplitterWnd.h"
BEGIN_MESSAGE_MAP(CCustomSplitterWnd, CSplitterWnd)
	ON_WM_MOUSEMOVE()
	ON_WM_LBUTTONDOWN()
	ON_WM_LBUTTONUP()
	ON_WM_LBUTTONDBLCLK()
END_MESSAGE_MAP()

//
// 마우스 관련 함수들 전부 재정의하여, 아무것도 못하게 한다
//

void CCustomSplitterWnd::OnMouseMove(UINT nFlags, CPoint point)
{
	// TODO: 여기에 메시지 처리기 코드를 추가 및/또는 기본값을 호출합니다.

	//CSplitterWnd::OnMouseMove(nFlags, point);
}


void CCustomSplitterWnd::OnLButtonDown(UINT nFlags, CPoint point)
{
	// TODO: 여기에 메시지 처리기 코드를 추가 및/또는 기본값을 호출합니다.

	//CSplitterWnd::OnLButtonDown(nFlags, point);
}


void CCustomSplitterWnd::OnLButtonUp(UINT nFlags, CPoint point)
{
	// TODO: 여기에 메시지 처리기 코드를 추가 및/또는 기본값을 호출합니다.

	//CSplitterWnd::OnLButtonUp(nFlags, point);
}


void CCustomSplitterWnd::OnLButtonDblClk(UINT nFlags, CPoint point)
{
	// TODO: 여기에 메시지 처리기 코드를 추가 및/또는 기본값을 호출합니다.

	//CSplitterWnd::OnLButtonDblClk(nFlags, point);
}
