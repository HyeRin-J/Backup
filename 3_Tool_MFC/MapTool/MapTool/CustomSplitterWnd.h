#pragma once
#include <afxext.h>

//������ ������ �Ұ����ϰ� �ϱ� ���� Ŀ���� ���ø��� ������
class CCustomSplitterWnd : public CSplitterWnd
{
public:
    DECLARE_MESSAGE_MAP()

    // ���콺 ���� �Լ��� ���� �������Ͽ�, �ƹ��͵� ���ϰ� �Ѵ�
    afx_msg void OnMouseMove(UINT nFlags, CPoint point);
    afx_msg void OnLButtonDown(UINT nFlags, CPoint point);
    afx_msg void OnLButtonUp(UINT nFlags, CPoint point);
    afx_msg void OnLButtonDblClk(UINT nFlags, CPoint point);
};

