#pragma once
#include <afxext.h>

//사이즈 조절이 불가능하게 하기 위한 커스텀 스플리터 윈도우
class CCustomSplitterWnd : public CSplitterWnd
{
public:
    DECLARE_MESSAGE_MAP()

    // 마우스 관련 함수들 전부 재정의하여, 아무것도 못하게 한다
    afx_msg void OnMouseMove(UINT nFlags, CPoint point);
    afx_msg void OnLButtonDown(UINT nFlags, CPoint point);
    afx_msg void OnLButtonUp(UINT nFlags, CPoint point);
    afx_msg void OnLButtonDblClk(UINT nFlags, CPoint point);
};

