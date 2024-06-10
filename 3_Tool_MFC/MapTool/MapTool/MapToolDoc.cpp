
// MapToolDoc.cpp: CMapToolDoc 클래스의 구현
//

#include "pch.h"
#include "framework.h"
#include "MainFrm.h"
#include "TileViewerDialog.h"

// SHARED_HANDLERS는 미리 보기, 축소판 그림 및 검색 필터 처리기를 구현하는 ATL 프로젝트에서 정의할 수 있으며
// 해당 프로젝트와 문서 코드를 공유하도록 해 줍니다.
#ifndef SHARED_HANDLERS
#include "MapTool.h"
#endif

#include "MapToolDoc.h"
#include "MapFileView.h"

#include <propkey.h>

#ifdef _DEBUG
#define new DEBUG_NEW
#endif

// CMapToolDoc

IMPLEMENT_DYNCREATE(CMapToolDoc, CDocument)

BEGIN_MESSAGE_MAP(CMapToolDoc, CDocument)
	ON_COMMAND(ID_CHANGE_TILEMAP, &CMapToolDoc::ChangeTilemap)
END_MESSAGE_MAP()


// CMapToolDoc 생성/소멸

CMapToolDoc::CMapToolDoc() noexcept
{
	// TODO: 여기에 일회성 생성 코드를 추가합니다.

}

CMapToolDoc::~CMapToolDoc()
{
}

//새 문서를 만들기
BOOL CMapToolDoc::OnNewDocument()
{
	if (!CDocument::OnNewDocument())
		return FALSE;

	// TODO: 여기에 재초기화 코드를 추가합니다.
	// SDI 문서는 이 문서를 다시 사용합니다.

	//사이즈 조절 다이얼로그 호출
	MapSizeDialog sizeDlg;
	sizeDlg.DoModal();

	m_CustomMap.m_Width = theApp.m_MapWidth;
	m_CustomMap.m_Height = theApp.m_MapHeight;

	static TCHAR BASED_CODE szFilter[] = _T("이미지 파일(*.BMP, *.GIF, *.JPG, *.PNG) | *.BMP;*.GIF;*.JPG;*.PNG;*.bmp;*.jpg;*.gif;*.png |모든파일(*.*)|*.*||");

	CFileDialog dlg(TRUE, _T("*.jpg"), _T("image"), OFN_HIDEREADONLY, szFilter);

	if (IDOK == dlg.DoModal())
	{
		CString pathName = dlg.GetPathName();

		m_TileMapPath = pathName.Right(pathName.GetLength() - pathName.ReverseFind('\\') - 1);
	}
	else
	{
		return FALSE;
	}

	CFileDialog dlg2(TRUE, _T("*.png"), _T("image"), OFN_HIDEREADONLY, szFilter);

	if (IDOK == dlg2.DoModal())
	{
		CString pathName = dlg2.GetPathName();

		m_CustomMap.m_BackgroundImage.Load(pathName);
	}
	else
	{
		return FALSE;
	}

	CMainFrame* mainFrm = ((CMainFrame*)AfxGetMainWnd());
	if (mainFrm != nullptr)
	{
		//스택 제거
		mainFrm->m_pMapFileView->m_MapBackGroundStack.clear();
		mainFrm->m_pMapFileView->m_StructureStack.clear();
		mainFrm->m_pMapFileView->m_StackIndex = 0;
	}

	//Bitmap이 이미 있으면 로딩하지 않는다
	if (theApp.m_MapTileAttribute.m_MapBitmap == nullptr)
	{
		theApp.m_MapTileAttribute.m_MapBitmap.Load(dlg.GetPathName());
	}

	theApp.m_MapTileAttribute.m_Width = theApp.m_MapTileAttribute.m_MapBitmap.GetWidth() / TILE_SIZE;
	theApp.m_MapTileAttribute.m_Height = theApp.m_MapTileAttribute.m_MapBitmap.GetHeight() / TILE_SIZE;

	//타일 정보 전체 삭제
	m_CustomMap.m_Background.clear();

	// 내가 만들려고 하는 Height와 Width 만큼 초기값 설정
	for (int y = 0; y < m_CustomMap.m_Height; ++y)
	{
		for (int x = 0; x < m_CustomMap.m_Width; ++x)
		{
			//타일 초기값
			MapTile tile;
			tile.m_NumberX = -1;
			tile.m_NumberY = -1;
			tile.m_Attribute = 0;
			m_CustomMap.m_Background.push_back(tile);
		}
	}

	//전체 타일에 대한 정보
	for (int y = 0; y < theApp.m_MapTileAttribute.m_Height; ++y)
	{
		for (int x = 0; x < theApp.m_MapTileAttribute.m_Width; ++x)
		{
			MapTile temp;
			temp.m_NumberX = x;
			temp.m_NumberY = y;
			temp.m_Attribute = 0;
			theApp.m_MapTileAttribute.m_Attributes.push_back(temp);
		}
	}

	return TRUE;
}

// CMapToolDoc serialization

void CMapToolDoc::Serialize(CArchive& ar)
{
	if (ar.IsStoring())
	{
		// TODO: 여기에 저장 코드를 추가합니다.
		rapidjson::GenericDocument<rapidjson::UTF16<>> Json;
		Json.SetObject();

		rapidjson::GenericValue<rapidjson::UTF16<>> MapData, FilePath;
		MapData.SetObject();

		rapidjson::GenericValue<rapidjson::UTF16<>> SizeArray;
		SizeArray.SetArray();

		rapidjson::GenericValue< rapidjson::UTF16<> > key_;

		key_.SetString(_T("TileMapName"));
		FilePath.SetString(m_TileMapPath.GetBuffer(), m_TileMapPath.GetLength(), Json.GetAllocator());

		MapData.AddMember(key_, FilePath, Json.GetAllocator());

		key_.SetString(_T("Size"));
		SizeArray.PushBack(m_CustomMap.m_Width, Json.GetAllocator());
		SizeArray.PushBack(m_CustomMap.m_Height, Json.GetAllocator());
		MapData.AddMember(key_, SizeArray, Json.GetAllocator());

		rapidjson::GenericValue<rapidjson::UTF16<>> BackGroundArray, AttributeArray;
		BackGroundArray.SetArray();
		AttributeArray.SetArray();

		// 배경 저장
		for (int i = 0; i < m_CustomMap.m_Background.size(); ++i)
		{
			rapidjson::GenericValue<rapidjson::UTF16<>> data;
			data.SetArray();

			MapTile& tile = m_CustomMap.m_Background[i];

			data.PushBack(tile.m_NumberX, Json.GetAllocator());
			data.PushBack(tile.m_NumberY, Json.GetAllocator());
			BackGroundArray.PushBack(data, Json.GetAllocator());

			AttributeArray.PushBack(tile.m_Attribute, Json.GetAllocator());
		}

		key_.SetString(_T("BackGroundIndex"));
		MapData.AddMember(key_, BackGroundArray, Json.GetAllocator());

		key_.SetString(_T("Attribute"));
		MapData.AddMember(key_, AttributeArray, Json.GetAllocator());

		CString fileName = ar.m_strFileName.GetBuffer();
		fileName = fileName.Right(fileName.GetLength() - fileName.ReverseFind('\\') - 1);
		fileName = fileName.Left(fileName.ReverseFind('.'));

		key_.SetString(fileName.GetBuffer(), fileName.GetLength());
		Json.AddMember(key_, MapData, Json.GetAllocator());

		rapidjson::GenericStringBuffer< rapidjson::UTF16<>> strbuf;
		rapidjson::Writer < rapidjson::GenericStringBuffer< rapidjson::UTF16<> >, rapidjson::UTF16<>, rapidjson::UTF16<> > writer(strbuf);
		Json.Accept(writer);

		CString str = strbuf.GetString();

		ar.WriteString(str);
	}
	else
	{
		// TODO: 여기에 로딩 코드를 추가합니다.
		// vector 초기화
		m_CustomMap.m_Background.clear();

		CMainFrame* mainFrm = ((CMainFrame*)AfxGetMainWnd());

		//스택 초기화
		mainFrm->m_pMapFileView->m_MapBackGroundStack.clear();
		mainFrm->m_pMapFileView->m_StructureStack.clear();

		CString str;
		ar.ReadString(str);

		rapidjson::GenericDocument< rapidjson::UTF16<> > Json;
		Json.Parse(str.GetBuffer());
		assert(Json.IsObject());

		SearchObject(Json);

		CString tFileName = _T("./image/");
		m_StageName = Json.MemberBegin()->name.GetString();
		tFileName += m_StageName;
		tFileName += _T(".png");
		m_CustomMap.m_BackgroundImage.Load(tFileName);

		// 스택에 제일 초기 정보를 넣는다
		mainFrm->m_pMapFileView->m_MapBackGroundStack.push_back(m_CustomMap.m_Background);
	}
}

#ifdef SHARED_HANDLERS

// 축소판 그림을 지원합니다.
void CMapToolDoc::OnDrawThumbnail(CDC& dc, LPRECT lprcBounds)
{
	// 문서의 데이터를 그리려면 이 코드를 수정하십시오.
	dc.FillSolidRect(lprcBounds, RGB(255, 255, 255));

	CString strText = _T("TODO: implement thumbnail drawing here");
	LOGFONT lf;

	CFont* pDefaultGUIFont = CFont::FromHandle((HFONT)GetStockObject(DEFAULT_GUI_FONT));
	pDefaultGUIFont->GetLogFont(&lf);
	lf.lfHeight = 36;

	CFont fontDraw;
	fontDraw.CreateFontIndirect(&lf);

	CFont* pOldFont = dc.SelectObject(&fontDraw);
	dc.DrawText(strText, lprcBounds, DT_CENTER | DT_WORDBREAK);
	dc.SelectObject(pOldFont);
}

// 검색 처리기를 지원합니다.
void CMapToolDoc::InitializeSearchContent()
{
	CString strSearchContent;
	// 문서의 데이터에서 검색 콘텐츠를 설정합니다.
	// 콘텐츠 부분은 ";"로 구분되어야 합니다.

	// 예: strSearchContent = _T("point;rectangle;circle;ole object;");
	SetSearchContent(strSearchContent);
}

void CMapToolDoc::SetSearchContent(const CString& value)
{
	if (value.IsEmpty())
	{
		RemoveChunk(PKEY_Search_Contents.fmtid, PKEY_Search_Contents.pid);
	}
	else
	{
		CMFCFilterChunkValueImpl* pChunk = nullptr;
		ATLTRY(pChunk = new CMFCFilterChunkValueImpl);
		if (pChunk != nullptr)
		{
			pChunk->SetTextValue(PKEY_Search_Contents, value, CHUNK_TEXT);
			SetChunkValue(pChunk);
		}
	}
}



#endif // SHARED_HANDLERS

// CMapToolDoc 진단
void CMapToolDoc::ChangeTilemap()
{
	static TCHAR BASED_CODE szFilter[] = _T("이미지 파일(*.BMP, *.GIF, *.JPG, *.PNG) | *.BMP;*.GIF;*.JPG;*.PNG;*.bmp;*.jpg;*.gif;*.png |모든파일(*.*)|*.*||");

	CFileDialog dlg(TRUE, _T("*.jpg"), _T("image"), OFN_HIDEREADONLY, szFilter);

	if (IDOK == dlg.DoModal())
	{
		CString pathName = dlg.GetPathName();

		m_TileMapPath = pathName.Right(pathName.GetLength() - pathName.ReverseFind('\\') - 1);
		theApp.m_MapTileAttribute.m_MapBitmap.Load(pathName);
		m_pTileViewerDlg->Invalidate(TRUE);
	}

	CFileDialog dlg2(TRUE, _T("*.png"), _T("image"), OFN_HIDEREADONLY, szFilter);

	if (IDOK == dlg2.DoModal())
	{
		CString pathName = dlg2.GetPathName();

		m_CustomMap.m_BackgroundImage.Load(pathName);
	}
}

#ifdef _DEBUG
void CMapToolDoc::AssertValid() const
{
	CDocument::AssertValid();
}

void CMapToolDoc::Dump(CDumpContext& dc) const
{
	CDocument::Dump(dc);
}
#endif //_DEBUG


// CMapToolDoc 명령
void CMapToolDoc::SearchObject(const rapidjson::GenericValue<rapidjson::UTF16<>>& Json)
{
	for (rapidjson::GenericValue< rapidjson::UTF16<> >::ConstMemberIterator itr = Json.MemberBegin(); itr != Json.MemberEnd(); ++itr)
	{
		CString  name = itr->name.GetString();

		switch (itr->value.GetType())
		{
		case rapidjson::kObjectType:
		{
			SearchObject(itr->value.GetObject());
			break;	// Object 타입의 노드 검색
		}
		case rapidjson::kArrayType:
		{
			auto it = itr->value.GetArray().Begin();

			if (name == _T("Size"))
			{
				m_CustomMap.m_Width = it++->GetInt();
				m_CustomMap.m_Height = it->GetInt();

				theApp.m_MapWidth = m_CustomMap.m_Width;
				theApp.m_MapHeight = m_CustomMap.m_Height;

				for (int y = 0; y < m_CustomMap.m_Height; y++)
				{
					for (int x = 0; x < m_CustomMap.m_Width; x++)
					{
						MapTile tile;

						m_CustomMap.m_Background.push_back(tile);
					}
				}
			}
			else if (name == _T("BackGroundIndex"))
			{
				for (int index = 0; it != itr->value.GetArray().End(); it++, index++)
				{
					auto value = it->Begin();
					m_CustomMap.m_Background[index].m_NumberX = value++->GetInt();
					m_CustomMap.m_Background[index].m_NumberY = value->GetInt();
				}
			}
			else if (name == _T("Attribute"))
			{
				for (int index = 0; it != itr->value.GetArray().End(); it++, index++)
				{
					m_CustomMap.m_Background[index].m_Attribute = it->GetInt();
				}
			}
		}
		case rapidjson::kStringType:
		{
			if (name == _T("TileMapName"))
			{
				WCHAR dir[256];
				GetCurrentDirectory(256, dir);
				CString tFileName = dir;
				tFileName += _T("\\image\\");
				m_TileMapPath = Json.MemberBegin()->value.GetString();
				tFileName += m_TileMapPath;
				DeleteObject(theApp.m_MapTileAttribute.m_MapBitmap.Detach());
				theApp.m_MapTileAttribute.m_MapBitmap.Load(tFileName);
				m_pTileViewerDlg->Invalidate(TRUE);
			}
		}
		break;
		default:
			break;
		}
	}
}