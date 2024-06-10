// TileViewerDialog.cpp: 구현 파일
//

#include "pch.h"
#include "MapTool.h"
#include "TileViewerDialog.h"
#include "MapToolDoc.h"
#include "afxdialogex.h"

#include "PresetSaveDialog.h"


// CTileViewerDialog 대화 상자

IMPLEMENT_DYNAMIC(CTileViewerDialog, CDialogEx)

CTileViewerDialog::CTileViewerDialog(CWnd* pParent /*=nullptr*/)
	: CDialogEx(IDD_DLG_TILE_VIEWER, pParent)
{

}

CTileViewerDialog::~CTileViewerDialog()
{
}

void CTileViewerDialog::DoDataExchange(CDataExchange* pDX)
{
	CDialogEx::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_COMBO2, m_TileAttributeSelectCombo);

	//초기값
	m_TileAttributeSelectCombo.SetCurSel(0);

	// 팝업 메뉴 로딩
	m_PopUpMenu.LoadMenu(IDR_POPUP_MENU);

	this->SetWindowPos(nullptr, 0, 0, theApp.m_MapTileAttribute.m_MapBitmap.GetWidth() + TILE_SIZE, theApp.m_MapTileAttribute.m_MapBitmap.GetHeight() + OFFSET_Y + TILE_SIZE * 2, SWP_NOMOVE | SWP_NOZORDER);
}

void CTileViewerDialog::OnDraw(CDC* pDC)
{
	// 더블 버퍼링용 dc
	CDC backDC;
	backDC.CreateCompatibleDC(pDC);

	// 클립영역 정보
	RECT rc;
	pDC->GetClipBox(&rc);

	//더블 버퍼링용 bitmap
	CBitmap backBitmap;
	backBitmap.CreateCompatibleBitmap(pDC, rc.right, rc.bottom);

	// dc랑 비트맵 연결
	backDC.SelectObject(backBitmap);

	//tilemap의 DC
	CDC tileMapDC;
	tileMapDC.CreateCompatibleDC(pDC);


	tileMapDC.SelectObject(theApp.m_MapTileAttribute.m_MapBitmap);
	

	// 새 브러쉬랑 펜을 만든다
	CBrush brush, * oldBrush;
	CPen pen, * oldPen;

	//화면 초기화
	backDC.Rectangle(rc.left - 1, rc.top - 1, rc.right + 1, rc.bottom + 1);

	//선택 사각형을 위해서
	brush.CreateStockObject(NULL_BRUSH);
	pen.CreatePen(PS_SOLID, 2, RGB(200, 20, 20));

	// 새로 만든 브러쉬랑 펜을 선택
	oldBrush = backDC.SelectObject(&brush);
	oldPen = backDC.SelectObject(&pen);

	//타일맵 출력
	backDC.BitBlt(OFFSET_X, OFFSET_Y, theApp.m_MapTileAttribute.m_MapBitmap.GetWidth(), theApp.m_MapTileAttribute.m_MapBitmap.GetHeight(), &tileMapDC, 0, 0, SRCCOPY);

	//선택한 위치에 사각형 출력
	backDC.Rectangle(
		m_FirstClickPoint.x * TILE_SIZE + OFFSET_X,
		m_FirstClickPoint.y * TILE_SIZE + OFFSET_Y,
		(m_LastClickPoint.x + 1) * TILE_SIZE + OFFSET_X,
		(m_LastClickPoint.y + 1) * TILE_SIZE + OFFSET_Y);

	//속성 출력
	if (m_IsShowAttributes)
	{
		for (int y = 0; y < theApp.m_MapTileAttribute.m_Height; ++y)
		{
			for (int x = 0; x < theApp.m_MapTileAttribute.m_Width; ++x)
			{
				CString attribute;

				attribute.Format(_T("%d"), theApp.m_MapTileAttribute.m_Attributes[theApp.m_MapTileAttribute.m_Width * y + x].m_Attribute);

				backDC.TextOut(x * TILE_SIZE + OFFSET_X, y * TILE_SIZE + OFFSET_Y, attribute);
			}
		}
	}

	// 이전 브러쉬랑 펜으로 설정
	backDC.SelectObject(oldBrush);
	backDC.SelectObject(oldPen);

	// mainDC에 출력
	pDC->BitBlt(0, 0, rc.right, rc.bottom, &backDC, 0, 0, SRCCOPY);
}


BEGIN_MESSAGE_MAP(CTileViewerDialog, CDialogEx)
	ON_WM_PAINT()
	ON_WM_LBUTTONDOWN()
	ON_BN_CLICKED(IDC_CHECK1, &CTileViewerDialog::OnBnClickedCheck1)
	ON_BN_CLICKED(ID_PRESET_SAVE, &CTileViewerDialog::OnClickSavePreset)
	ON_CBN_SELCHANGE(IDC_COMBO2, &CTileViewerDialog::OnCbnSelchangeCombo2)
	ON_WM_LBUTTONUP()
	ON_WM_MOUSEMOVE()
	ON_WM_RBUTTONUP()
END_MESSAGE_MAP()


// CTileViewerDialog 메시지 처리기

// 다이얼로그에는 onDraw가 사실 없어서 onPaint로 사용해야 하는데
// onDraw가 익숙해서 그냥 커스텀으로 만들었다.
void CTileViewerDialog::OnPaint()
{
	if (IsIconic())
	{
		CPaintDC dc(this); // device context for painting

		SendMessage(WM_ICONERASEBKGND, (WPARAM)dc.GetSafeHdc(), 0);

		// Center icon in client rectangle
		int cxIcon = GetSystemMetrics(SM_CXICON);
		int cyIcon = GetSystemMetrics(SM_CYICON);

		CRect rect;
		GetClientRect(&rect);
		int x = (rect.Width() - cxIcon + 1) / 2;
		int y = (rect.Height() - cyIcon + 1) / 2;
	}
	else
	{
		CPaintDC dc(this);

		OnDraw(&dc);

		CDialog::OnPaint();
	}
}


void CTileViewerDialog::OnLButtonDown(UINT nFlags, CPoint point)
{
	// TODO: 여기에 메시지 처리기 코드를 추가 및/또는 기본값을 호출합니다.

	//선택한 x, y 좌표 계산
	if (point.x - OFFSET_X < 0 || point.y - OFFSET_Y < 0) return;

	m_SelectTileCount.x = (point.x - OFFSET_X) / TILE_SIZE;
	m_SelectTileCount.y = (point.y - OFFSET_Y) / TILE_SIZE;

	// 현재 선택중인 타일의 속성 정보를 저장
	int attribute = 0;

	// 편집중인 타입

		// 범위 초과
	if (m_SelectTileCount.x >= theApp.m_MapTileAttribute.m_Width ||
		m_SelectTileCount.y >= theApp.m_MapTileAttribute.m_Height)
		return;

	// 현재 선택중인 타일의 속성 정보를 저장
	attribute = theApp.m_MapTileAttribute.m_Attributes[theApp.m_MapTileAttribute.m_Width * m_SelectTileCount.y + m_SelectTileCount.x].m_Attribute;


	// 타일 새로 선택하기 때문에 클리어
	m_SelectedTiles.selectedPoints.clear();

	// 드래그 상태
	m_IsLButtonDown = true;

	// 시작지점
	m_FirstClickPoint.x = m_SelectTileCount.x;
	m_FirstClickPoint.y = m_SelectTileCount.y;

	// 끝지점
	m_LastClickPoint.x = m_SelectTileCount.x;
	m_LastClickPoint.y = m_SelectTileCount.y;

	// 선택한 타일의 속성 정보를 갱신
	m_TileAttributeSelectCombo.SetCurSel(attribute);

	// 화면 갱신
	Invalidate(FALSE); // WM_PAINT 메시지 호출 -> OnPaint -> 안에 내가 만든 OnDraw -> 자동으로 화면 갱신
}

// 취소버튼 아무것도 안 함
void CTileViewerDialog::OnCancel()
{
	// TODO: 여기에 특수화된 코드를 추가 및/또는 기본 클래스를 호출합니다.

	//CDialogEx::OnCancel();
}

void CTileViewerDialog::OnOK()
{
	// TODO: 여기에 특수화된 코드를 추가 및/또는 기본 클래스를 호출합니다.

	//CDialogEx::OnOK();
}


void CTileViewerDialog::OnBnClickedCheck1()
{
	// TODO: 여기에 컨트롤 알림 처리기 코드를 추가합니다.
	// 속성 보기 모드
	m_IsShowAttributes = !m_IsShowAttributes;

	//속성 변경 콤보 박스 활성화/비활성화
	if (m_IsShowAttributes == TRUE)
	{
		m_TileAttributeSelectCombo.EnableWindow(TRUE);
	}
	else
	{
		m_TileAttributeSelectCombo.EnableWindow(FALSE);
	}

	// 화면 갱신
	Invalidate(FALSE);
}

void CTileViewerDialog::OnCbnSelchangeCombo2()
{
	// TODO: 여기에 컨트롤 알림 처리기 코드를 추가합니다.

	// 타일 속성 정보 변경
	int attribute = m_TileAttributeSelectCombo.GetCurSel();


	CPoint point;

	// 내가 선택하고 있는 타일들의 속성을 다 변경
	for (int index = 0; index < m_SelectedTiles.selectedPoints.size(); ++index)
	{
		point = m_SelectedTiles.selectedPoints[index];

		// 속성 갱신
		MapTile& tile = theApp.m_MapTileAttribute.m_Attributes[theApp.m_MapTileAttribute.m_Width * point.y + point.x];
		tile.m_Attribute = attribute;
	}


	Invalidate(FALSE);
}


void CTileViewerDialog::OnLButtonUp(UINT nFlags, CPoint point)
{
	// TODO: 여기에 메시지 처리기 코드를 추가 및/또는 기본값을 호출합니다.

	if (point.x - OFFSET_X < 0 || point.y - OFFSET_Y < 0) return;

	// x, y 위치 계산
	int x = (point.x - OFFSET_X) / TILE_SIZE;
	int y = (point.y - OFFSET_Y) / TILE_SIZE;


	// 범위 계산
	if ((x < theApp.m_MapTileAttribute.m_Width &&
		y < theApp.m_MapTileAttribute.m_Height) == TRUE)
	{
		// 마지막 클릭 지점 갱신
		m_LastClickPoint.x = x;
		m_LastClickPoint.y = y;
	}

	// 드래그 모드 해제
	m_IsLButtonDown = false;

	// 선택 중인 타일들을 넣기 위해서 계산한다.
	int x1 = m_FirstClickPoint.x;
	int x2 = m_LastClickPoint.x;

	//둘 중 더 큰 값을 x2로 교체
	if (x1 > x2)
	{
		int temp = x1;
		x1 = x2;
		x2 = temp;
	}

	// 선택 중인 타일들을 넣기 위해서 계산한다.
	int y1 = m_FirstClickPoint.y;
	int y2 = m_LastClickPoint.y;

	//둘 중 더 큰 값을 y2로 교체
	if (y1 > y2)
	{
		int temp = y1;
		y1 = y2;
		y2 = temp;
	}

	// 선택 중인 타일들의 넓이, 높이 계산
	m_SelectedTiles.m_Width = x2 - x1 + 1;
	m_SelectedTiles.m_Height = y2 - y1 + 1;

	//선택한 타일들을 집어넣는다
	for (int y = y1; y <= y2; ++y)
	{
		for (int x = x1; x <= x2; ++x)
		{
			CPoint point;
			point.x = x;
			point.y = y;

			m_SelectedTiles.selectedPoints.push_back(point);
		}
	}

	Invalidate(FALSE);
}


void CTileViewerDialog::OnMouseMove(UINT nFlags, CPoint point)
{
	// TODO: 여기에 메시지 처리기 코드를 추가 및/또는 기본값을 호출합니다.

	// 드래그 상태가 아니면 아무것도 안 함
	if (m_IsLButtonDown == FALSE) return;

	// x, y 계산
	int x = (point.x - OFFSET_X) / TILE_SIZE;
	int y = (point.y - OFFSET_Y) / TILE_SIZE;

	if (x < 0 || x >= theApp.m_MapTileAttribute.m_Width ||
		y < 0 || y >= theApp.m_MapTileAttribute.m_Height)
		return;


	// 마지막 위치 갱신
	m_LastClickPoint.x = x;
	m_LastClickPoint.y = y;

	Invalidate(FALSE);
}

// 저장 버튼 클릭
void CTileViewerDialog::OnClickSavePreset()
{
	// 저장 다이얼로그 출력
	CPresetSaveDialog dialog;
	dialog.DoModal();
}

// 오른쪽 버튼 클릭
void CTileViewerDialog::OnRButtonUp(UINT nFlags, CPoint point)
{
	// TODO: 여기에 메시지 처리기 코드를 추가 및/또는 기본값을 호출합니다.

	// 클릭한 위치에
	ClientToScreen(&point);

	// 팝업 메뉴 호출
	m_PopUpMenu.GetSubMenu(1)->TrackPopupMenu(TPM_LEFTALIGN, point.x, point.y, this);
}
