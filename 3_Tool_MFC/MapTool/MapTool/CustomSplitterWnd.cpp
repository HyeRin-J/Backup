#include "pch.h"
#include "CustomSplitterWnd.h"
BEGIN_MESSAGE_MAP(CCustomSplitterWnd, CSplitterWnd)
	ON_WM_MOUSEMOVE()
	ON_WM_LBUTTONDOWN()
	ON_WM_LBUTTONUP()
	ON_WM_LBUTTONDBLCLK()
END_MESSAGE_MAP()

//
// ���콺 ���� �Լ��� ���� �������Ͽ�, �ƹ��͵� ���ϰ� �Ѵ�
//

void CCustomSplitterWnd::OnMouseMove(UINT nFlags, CPoint point)
{
	// TODO: ���⿡ �޽��� ó���� �ڵ带 �߰� ��/�Ǵ� �⺻���� ȣ���մϴ�.

	//CSplitterWnd::OnMouseMove(nFlags, point);
}


void CCustomSplitterWnd::OnLButtonDown(UINT nFlags, CPoint point)
{
	// TODO: ���⿡ �޽��� ó���� �ڵ带 �߰� ��/�Ǵ� �⺻���� ȣ���մϴ�.

	//CSplitterWnd::OnLButtonDown(nFlags, point);
}


void CCustomSplitterWnd::OnLButtonUp(UINT nFlags, CPoint point)
{
	// TODO: ���⿡ �޽��� ó���� �ڵ带 �߰� ��/�Ǵ� �⺻���� ȣ���մϴ�.

	//CSplitterWnd::OnLButtonUp(nFlags, point);
}


void CCustomSplitterWnd::OnLButtonDblClk(UINT nFlags, CPoint point)
{
	// TODO: ���⿡ �޽��� ó���� �ڵ带 �߰� ��/�Ǵ� �⺻���� ȣ���մϴ�.

	//CSplitterWnd::OnLButtonDblClk(nFlags, point);
}
