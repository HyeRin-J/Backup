<div align="center">
  <h1 style="border-bottom: 1px solid #21262d; color: #c9d1d9;"> 💻 Code_MFC </h1>
  <div style="font-weight: 700; font-size: 15px; text-align: center; color: #c9d1d9;"> MFC로 작성한 코드 모음입니다. </div>
   <br>
</div>

<details open><summary>
<h2 style="border-bottom: 1px solid #21262d; color: #c9d1d9;"> ⭐ AnimationTool </h2></summary>

<h3 style="border-bottom: 1px solid #21262d; color: #c9d1d9;">이미지</h3>

![AnimationTool](https://github.com/HyeRin-J/Portfolio/blob/main/3_Tool_MFC/AnimationTool/Image/ToolMainImage.png)
<h3 style="border-bottom: 1px solid #21262d; color: #c9d1d9;"> 세부 사항 </h3>
  <h5 style="border-bottom: 1px solid #21262d; color: #c9d1d9;"> AnnTrophy 프로젝트에서 사용하기 위한 애니메이션 데이터 제작용 툴 </h5>

[AnnTrophy Project](https://github.com/HyeRin-J/Portfolio/tree/main/2_Code_C%2B%2B)

<h3 style="border-bottom: 1px solid #21262d; color: #c9d1d9;"> Source Code </h3>

<li>SelectedImageListView</li>

```cpp
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

```

<li>MouseDragAndUp(ImageSnap)</li>

```cpp
void CAnimationToolView::OnLButtonDown(UINT nFlags, CPoint point)
{
	// TODO: 여기에 메시지 처리기 코드를 추가 및/또는 기본값을 호출합니다.
	m_IsLButtonDown = true;

	CPoint scrPoint = GetScrollPosition();

	//타일 정보를 저장한다.
	m_MousePos[0] = point + scrPoint;
	m_MousePos[1] = point + scrPoint;

	// 내가 선택한 지점
	int clickX = point.x + scrPoint.x;
	int clickY = point.y + scrPoint.y;

	// 연결된 Document 포인터
	// GetDocument()도 가능
	CAnimationToolDoc* pDoc = ((CAnimationToolDoc*)m_pDocument);

	//화면을 갱신한다.
	Invalidate(FALSE);
}


void CAnimationToolView::OnLButtonUp(UINT nFlags, CPoint point)
{
	// TODO: 여기에 메시지 처리기 코드를 추가 및/또는 기본값을 호출합니다.
	if (m_IsLButtonDown == false) return;

	m_IsLButtonDown = false;

	CPoint scrPoint = GetScrollPosition();

	m_MousePos[1] = point + scrPoint;
	CAnimationToolDoc* pDoc = (CAnimationToolDoc*)GetDocument();
	CImage& img = pDoc->m_ShowImage;

	if (m_MousePos[0].x < 0 || m_MousePos[0].x > img.GetWidth()) return;
	if (m_MousePos[0].y < 0 || m_MousePos[0].y > img.GetHeight()) return;

	int x1 = m_MousePos[0].x >= 0 && m_MousePos[0].x < img.GetWidth() ? m_MousePos[0].x : 0;
	int y1 = m_MousePos[0].y >= 0 && m_MousePos[0].y < img.GetHeight() ? m_MousePos[0].y : 0;
	int x2 = m_MousePos[1].x >= 0 && m_MousePos[1].x < img.GetWidth() ? m_MousePos[1].x : img.GetWidth() - 1;
	int y2 = m_MousePos[1].y >= 0 && m_MousePos[1].y < img.GetHeight() ? m_MousePos[1].y : img.GetHeight() - 1;

	if (m_MousePos[0].x > m_MousePos[1].x)
	{
		int temp = m_MousePos[1].x;
		m_MousePos[1].x = m_MousePos[0].x;
		m_MousePos[0].x = temp;
	}

	if (m_MousePos[0].y > m_MousePos[1].y)
	{
		int temp = m_MousePos[1].y;
		m_MousePos[1].y = m_MousePos[0].y;
		m_MousePos[0].y = temp;
	}

	int y = m_MousePos[0].y;

	while (img.GetPixel(x1, y) == pDoc->m_ColorKey)
	{
		y++;

		if (y >= y2)
		{
			y = 0;
			x1++;
		}
		if (x1 >= x2)
		{
			break;
		}
	}

	int x = m_MousePos[0].x;

	while (img.GetPixel(x, y1) == pDoc->m_ColorKey)
	{
		x++;

		if (x >= x2)
		{
			x = 0;
			y1++;
		}
		if (y1 >= img.GetHeight())
		{
			break;
		}
	}

	y = m_MousePos[0].y;
	while (img.GetPixel(x2, y) == pDoc->m_ColorKey)
	{
		y++;

		if (y >= y2)
		{
			y = 0;
			x2--;
		}
		if (x2 < 0)
		{
			break;
		}
	}

	x = m_MousePos[0].x;

	while (img.GetPixel(x, y2) == pDoc->m_ColorKey)
	{
		x++;

		if (x >= x2)
		{
			x = 0;
			y2--;
		}
		if (y2 < 0)
		{
			break;
		}
	}

	m_MousePos[0].x = x1;
	m_MousePos[0].y = y1;
	m_MousePos[1].x = x2;
	m_MousePos[1].y = y2;

	Invalidate(FALSE);

	AddImageList(true);
}


void CAnimationToolView::OnMouseMove(UINT nFlags, CPoint point)
{
	// TODO: 여기에 메시지 처리기 코드를 추가 및/또는 기본값을 호출합니다.
	if (!m_IsLButtonDown) return;

	CPoint scrPoint = GetScrollPosition();

	m_MousePos[1] = point + scrPoint;

	Invalidate(FALSE);
}
```

<li>AddSnappedImageToList</li>

```cpp
void CAnimationToolView::AddImageList(bool isAdd)
{
	CAnimationToolDoc* pDoc = (CAnimationToolDoc*)GetDocument();

	int width = m_MousePos[1].x - m_MousePos[0].x;
	int height = m_MousePos[1].y - m_MousePos[0].y;

	CDC tempDC;
	tempDC.CreateCompatibleDC(GetDC());

	CBitmap tempBitmap;
	tempBitmap.CreateCompatibleBitmap(GetDC(), width, height);

	if (tempBitmap.GetSafeHandle() == nullptr) return;

	tempDC.SelectObject(tempBitmap);

	CBrush backBrush;
	backBrush.CreateSolidBrush(RGB(255, 255, 255));
	tempDC.SelectObject(backBrush);
	tempDC.Rectangle(-1, -1, width + 1, height + 1);

	TransparentBlt(tempDC, 0, 0, width, height,	pDoc->m_ShowImage.GetDC(), m_MousePos[0].x, m_MousePos[0].y, width, height, pDoc->m_ColorKey);

	pDoc->m_SnapImage = new CImage();
	pDoc->m_SnapImage->Create(width, height, 32, 0);

	if ((HBITMAP)pDoc->m_SnapImage == nullptr) return;

	BitBlt(pDoc->m_SnapImage->GetDC(), 0, 0, width, height, tempDC, 0, 0, SRCCOPY);

	pDoc->m_ImageList.push_back(pDoc->m_SnapImage);

	if (isAdd)
	{
		SaveData data = { m_MousePos[0], {width, height}, {width / 2, height / 2}, {0, 0, width, height}, .16f };

		pDoc->m_SelectIndex = pDoc->m_ImageList.size() - 1;

		pDoc->m_SaveData.push_back(data);
	}

	pDoc->m_SnapImage = nullptr;

	((CMainFrame*)AfxGetMainWnd())->m_pSelectedView->UpdateImageList();
	((CMainFrame*)AfxGetMainWnd())->m_pPivotView->Invalidate(FALSE);
}
```
</details>

<details open> <summary><h2 style="border-bottom: 1px solid #21262d; color: #c9d1d9;"> ⭐ MapTool </h2></summary>

<h3 style="border-bottom: 1px solid #21262d; color: #c9d1d9;">이미지</h3>

![MapTool](https://github.com/HyeRin-J/Portfolio/blob/main/3_Tool_MFC/MapTool/Image/Tool_Main.png)
<h3 style="border-bottom: 1px solid #21262d; color: #c9d1d9;"> 세부 사항 </h3>
  <h5 style="border-bottom: 1px solid #21262d; color: #c9d1d9;"> AnnTrophy 프로젝트에서 사용하기 위한 맵 데이터 제작용 툴 </h5>

[AnnTrophy Project](https://github.com/HyeRin-J/Portfolio/tree/main/2_Code_C%2B%2B)

<h3 style="border-bottom: 1px solid #21262d; color: #c9d1d9;"> Source Code </h3>

<li>MouseDragAndUp</li>

```cpp
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
```

<li>SetTileInfo</li>

```cpp
/// <summary>
/// 타일 정보를 갱신하는 함수이다.
/// </summary>
/// <param name="clickX">선택한 x 지점</param>
/// <param name="clickY">선택한 y 지점</param>
void CMapFileView::SetTile(int clickX, int clickY)
{
	// 연결된 Document 포인터
	CMapToolDoc* pDoc = ((CMapToolDoc*)m_pDocument);

	// 만약에 선택해놓은 타일이 아무것도 없으면 아무것도 하지 않음
	if (pDoc->m_pTileViewerDlg->m_SelectedTiles.selectedPoints.size() <= 0)
		return;

	// 내가 선택한 타일의 높이와 너비 정보
	int selectTileHeight = pDoc->m_pTileViewerDlg->m_SelectedTiles.m_Height;
	int selectTileWidth = pDoc->m_pTileViewerDlg->m_SelectedTiles.m_Width;

	// selectedPoints가 1차원 벡터라서 index가 필요하다
	int index = 0;

	// 클릭한 y부터, y + selectHeight까지
	// 클릭한 x부터, x + selectWidth까지
	for (int y = clickY; y < clickY + selectTileHeight && y < pDoc->m_CustomMap.m_Height; ++y)
	{
		for (int x = clickX; x < clickX + selectTileWidth && x < pDoc->m_CustomMap.m_Width; ++x)
		{
			// 선택해 놓았던 위치의 point 정보를 가져온다
			CPoint& point = pDoc->m_pTileViewerDlg->m_SelectedTiles.selectedPoints[index++];
			int attribute = theApp.m_MapTileAttribute.m_Attributes[(theApp.m_MapTileAttribute.m_Width * point.y) + point.x].m_Attribute;

			//타일 타입에 따라서 다르게 처리
			if (pDoc->m_pTileViewerDlg->m_SelectedTiles.m_TileType == 0)
			{
				//타일 객체 레퍼런스
				MapTile& tile = pDoc->m_CustomMap.m_Background[(pDoc->m_CustomMap.m_Width * y) + x];

				// 갱신
				tile.m_NumberX = point.x;
				tile.m_NumberY = point.y;
				tile.m_Attribute = attribute;
			}
		}
	}
}
```

<li>MainOnDraw</li>

```cpp
void CMapFileView::OnDraw(CDC* pDC)
{
	//연결된 문서를 가져온다.
	CMapToolDoc* pDoc = static_cast<CMapToolDoc*>(GetDocument());

	// TODO: 여기에 그리기 코드를 추가합니다.
	// 클립 영역(그리기 영역)의 정보를 RECT에 저장
	RECT rc;
	pDC->GetClipBox(&rc);

	// 맵의 백그라운드 타일
	CDC tileBackDC;
	tileBackDC.CreateCompatibleDC(pDC);
	tileBackDC.SelectObject(theApp.m_MapTileAttribute.m_MapBitmap);

	// 새 브러쉬랑 펜을 만든다
	CBrush gridBrush, * oldBrush;
	CPen gridPen, rectPen, * oldPen;

	gridBrush.CreateStockObject(NULL_BRUSH);
	gridPen.CreatePen(PS_SOLID, 1, RGB(100, 100, 100));

	// 새로 만든 브러쉬랑 펜을 선택
	oldBrush = m_BackDC.SelectObject(&gridBrush);
	oldPen = m_BackDC.SelectObject(&gridPen);

	if (pDoc->m_CustomMap.m_BackgroundImage != nullptr)
	{
		pDoc->m_CustomMap.m_BackgroundImage.BitBlt(m_BackDC, 0, 0, SRCCOPY);
	}

	// 내가 만든 맵의 세로, 가로 길이만큼 반복
	for (int y = 0; y < pDoc->m_CustomMap.m_Height; ++y)
	{
		for (int x = 0; x < pDoc->m_CustomMap.m_Width; ++x)
		{
			// 내가 x, y 위치에 지정한 타일의 정보를 가져온다.
			MapTile tempTile = pDoc->m_CustomMap.m_Background[pDoc->m_CustomMap.m_Width * y + x];

			//그 지점에서 선택된 타일의 인덱스
			int selectX = tempTile.m_NumberX;
			int selectY = tempTile.m_NumberY;

			if (selectX != -1 && selectY != -1)
			{
				// x, y 타일의 위치에 배경 정보를 출력한다.
				m_BackDC.StretchBlt(x * CELL_SIZE, y * CELL_SIZE, CELL_SIZE, CELL_SIZE, &tileBackDC, TILE_SIZE * selectX, TILE_SIZE * selectY, TILE_SIZE, TILE_SIZE, SRCCOPY);
			}

			if (pDoc->m_pTileViewerDlg->m_IsShowAttributes)
			{
				CString attribute;

				attribute.Format(_T("%d"), pDoc->m_CustomMap.m_Background[pDoc->m_CustomMap.m_Width * y + x].m_Attribute);

				m_BackDC.TextOut(x * CELL_SIZE, y * CELL_SIZE, attribute);
			}

			// 선택된 건물 비트맵 타일 인덱스
			selectX = tempTile.m_NumberX;
			selectY = tempTile.m_NumberY;

			//격자 출력
			m_BackDC.MoveTo(x * CELL_SIZE, y * CELL_SIZE);
			m_BackDC.LineTo(pDoc->m_CustomMap.m_Width * CELL_SIZE, y * CELL_SIZE);

			m_BackDC.MoveTo(x * CELL_SIZE, y * CELL_SIZE);
			m_BackDC.LineTo(x * CELL_SIZE, pDoc->m_CustomMap.m_Height * CELL_SIZE);
		}
	}

	//선택 사각형을 위해서
	rectPen.CreatePen(PS_SOLID, 2, RGB(200, 20, 20));
	m_BackDC.SelectObject(rectPen);

	m_BackDC.Rectangle(
		m_MousePos.x * CELL_SIZE,
		m_MousePos.y * CELL_SIZE,
		(m_MousePos.x + pDoc->m_pTileViewerDlg->m_SelectedTiles.m_Width) * CELL_SIZE,
		(m_MousePos.y + pDoc->m_pTileViewerDlg->m_SelectedTiles.m_Height) * CELL_SIZE);

	// backDC를 mainDC로 출력
	pDC->BitBlt(0, 0, rc.right, rc.bottom, &m_BackDC, 0, 0, SRCCOPY);

	// 이전 브러쉬랑 펜으로 설정
	m_BackDC.SelectObject(oldBrush);
	m_BackDC.SelectObject(oldPen);

	// 미니맵 뷰도 같이 갱신
	((CMainFrame*)AfxGetMainWnd())->m_pMiniMapView->Invalidate(FALSE);
}
```

<li>LeftClick(draw), RightClick(remove)</li>

```cpp
void CMapFileView::OnLButtonDown(UINT nFlags, CPoint point)
{
	// TODO: 여기에 메시지 처리기 코드를 추가 및/또는 기본값을 호출합니다.

	// WM_MOUSEMOVE에서 사용한다.
	// 드래그를 처리하기 위해서
	m_IsLButtonDown = true;

	CPoint scrPoint = GetScrollPosition();

	// 내가 선택한 지점
	int clickX = (point.x + scrPoint.x) / CELL_SIZE;
	int clickY = (point.y + scrPoint.y) / CELL_SIZE;

	// 연결된 Document 포인터
	// GetDocument()도 가능
	CMapToolDoc* pDoc = ((CMapToolDoc*)m_pDocument);

	//생성된 맵을 벗어나게 클릭하는 경우 아무것도 하지 않는다.
	if (clickX >= pDoc->m_CustomMap.m_Width || clickY >= pDoc->m_CustomMap.m_Height)
		return;

	//타일 정보를 저장한다.
	SetTile(clickX, clickY);

	//화면을 갱신한다.
	Invalidate(FALSE);
}

void CMapFileView::OnRButtonDown(UINT nFlags, CPoint point)
{
	// TODO: 여기에 메시지 처리기 코드를 추가 및/또는 기본값을 호출합니다.


	CPoint scrPoint = GetScrollPosition();

	// 내가 선택한 지점
	int clickX = (point.x + scrPoint.x) / CELL_SIZE;
	int clickY = (point.y + scrPoint.y) / CELL_SIZE;

	// 연결된 Document 포인터
	// GetDocument()도 가능
	CMapToolDoc* pDoc = ((CMapToolDoc*)m_pDocument);

	//생성된 맵을 벗어나게 클릭하는 경우 아무것도 하지 않는다.
	if (clickX >= pDoc->m_CustomMap.m_Width || clickY >= pDoc->m_CustomMap.m_Height)
		return;

	// 만약에 선택해놓은 타일이 아무것도 없으면 아무것도 하지 않음
	if (pDoc->m_pTileViewerDlg->m_SelectedTiles.selectedPoints.size() <= 0)
		return;

	m_IsRButtonDown = true;

	// 내가 선택한 타일의 높이와 너비 정보
	int selectTileHeight = pDoc->m_pTileViewerDlg->m_SelectedTiles.m_Height;
	int selectTileWidth = pDoc->m_pTileViewerDlg->m_SelectedTiles.m_Width;

	// selectedPoints가 1차원 벡터라서 index가 필요하다
	int index = 0;

	// 클릭한 y부터, y + selectHeight까지
	// 클릭한 x부터, x + selectWidth까지
	for (int y = clickY; y < clickY + selectTileHeight && y < pDoc->m_CustomMap.m_Height; ++y)
	{
		for (int x = clickX; x < clickX + selectTileWidth && x < pDoc->m_CustomMap.m_Width; ++x)
		{
			//타일 타입에 따라서 다르게 처리
			if (pDoc->m_pTileViewerDlg->m_SelectedTiles.m_TileType == 0)
			{
				//타일 객체 레퍼런스
				MapTile& tile = pDoc->m_CustomMap.m_Background[(pDoc->m_CustomMap.m_Width * y) + x];

				// 갱신
				tile.m_NumberX = -1;
				tile.m_NumberY = -1;
				tile.m_Attribute = 0;
			}
		}
	}

	//화면을 갱신한다.
	Invalidate(FALSE);
}
```

</details>
