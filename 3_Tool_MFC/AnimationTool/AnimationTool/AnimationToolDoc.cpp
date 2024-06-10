
// AnimationToolDoc.cpp: CAnimationToolDoc 클래스의 구현
//

#include "pch.h"
#include "framework.h"
// SHARED_HANDLERS는 미리 보기, 축소판 그림 및 검색 필터 처리기를 구현하는 ATL 프로젝트에서 정의할 수 있으며
// 해당 프로젝트와 문서 코드를 공유하도록 해 줍니다.
#ifndef SHARED_HANDLERS
#include "AnimationTool.h"
#endif

#include "MainFrm.h"
#include "AnimationToolView.h"
#include "SelectedImageView.h"
#include "AnimationToolDoc.h"

#include <propkey.h>

#ifdef _DEBUG
#define new DEBUG_NEW
#endif

// CAnimationToolDoc

IMPLEMENT_DYNCREATE(CAnimationToolDoc, CDocument)

BEGIN_MESSAGE_MAP(CAnimationToolDoc, CDocument)
END_MESSAGE_MAP()


// CAnimationToolDoc 생성/소멸

CAnimationToolDoc::CAnimationToolDoc() noexcept
{
	// TODO: 여기에 일회성 생성 코드를 추가합니다.

}

CAnimationToolDoc::~CAnimationToolDoc()
{
	m_ShowImage.Destroy();

	if (m_SnapImage != nullptr)
	{
		m_SnapImage->Destroy();
		m_SnapImage = nullptr;
	}

	for (auto img : m_ImageList)
	{
		if(img->GetDC() != nullptr)	img->ReleaseDC();
		img->Destroy();
		delete img;
		img = nullptr;
	}

	m_ImageList.clear();
	m_SaveData.clear();
}

BOOL CAnimationToolDoc::OnNewDocument()
{
	if (!CDocument::OnNewDocument())
		return FALSE;

	// TODO: 여기에 재초기화 코드를 추가합니다.
	// SDI 문서는 이 문서를 다시 사용합니다.

	// TODO: Add your control notification handler code here
	static TCHAR BASED_CODE szFilter[] = _T("이미지 파일(*.BMP, *.GIF, *.JPG, *.PNG) | *.BMP;*.GIF;*.JPG;*.PNG;*.bmp;*.jpg;*.gif;*.png |모든파일(*.*)|*.*||");

	CFileDialog dlg(TRUE, _T("*.jpg"), _T("image"), OFN_HIDEREADONLY, szFilter);

	if (IDOK == dlg.DoModal())
	{
		CString pathName = dlg.GetPathName();

		m_FileName = pathName.Right(pathName.GetLength() - pathName.ReverseFind('\\') - 1);

		if (m_ShowImage)
		{
			DeleteObject(m_ShowImage.Detach());
		}

		m_ShowImage.Load(pathName);

		//HBITMAP hBitmap = (HBITMAP)LoadImage(NULL,"c:\\image.bmp", 
		//IMAGE_BITMAP, 0, 0, LR_LOADFROMFILE);
		//if (hBitmap != NULL)
		//	bitmap.Attach(hBitmap);
	}
	else
	{
		return FALSE;
	}

	if (m_ImageList.size() != 0)
	{
		for (auto img : m_ImageList)
		{
			img->ReleaseDC();
			img->Destroy();
			img = nullptr;
		}

		m_ImageList.clear();
		m_SaveData.clear();
	}

	//m_ImageList.Create(1024, 1024, ILC_COLOR, 0, 4);

	return TRUE;
}




// CAnimationToolDoc serialization

void CAnimationToolDoc::Serialize(CArchive& ar)
{
	if (ar.IsStoring())
	{
		// TODO: 여기에 저장 코드를 추가합니다.
		rapidjson::GenericDocument<rapidjson::UTF16<>> Json;
		Json.SetObject();
			
		rapidjson::GenericValue< rapidjson::UTF16<>> Array;
		Array.SetArray();

		rapidjson::GenericValue< rapidjson::UTF16<> > key_;
		key_.SetString(m_FileName.GetBuffer(), m_FileName.GetLength());

		for (int i = 0; i < m_SaveData.size(); i++)
		{
			rapidjson::GenericValue<rapidjson::UTF16<>> Frame;
			Frame.SetObject();

			rapidjson::GenericValue<rapidjson::UTF16<>> key, value;
			rapidjson::GenericValue<rapidjson::UTF16<>> arr;

			key.SetString(_T("Pos"));
			arr.SetArray();
			arr.PushBack(m_SaveData[i].bitmapStartPoint.x, Json.GetAllocator());
			arr.PushBack(m_SaveData[i].bitmapStartPoint.y, Json.GetAllocator());
			Frame.AddMember(key, arr, Json.GetAllocator());
	
			key.SetString(_T("Size"));
			arr.SetArray();
			arr.PushBack(m_SaveData[i].bitmapSize.cx, Json.GetAllocator());
			arr.PushBack(m_SaveData[i].bitmapSize.cy, Json.GetAllocator());
			Frame.AddMember(key, arr, Json.GetAllocator());

			key.SetString(_T("Pivot"));
			arr.SetArray();
			arr.PushBack(m_SaveData[i].pivot.x, Json.GetAllocator());
			arr.PushBack(m_SaveData[i].pivot.y, Json.GetAllocator());
			Frame.AddMember(key, arr, Json.GetAllocator());

			key.SetString(_T("Boundary"));
			arr.SetArray();
			arr.PushBack(m_SaveData[i].boundary.left, Json.GetAllocator());
			arr.PushBack(m_SaveData[i].boundary.top, Json.GetAllocator());
			arr.PushBack(m_SaveData[i].boundary.right, Json.GetAllocator());
			arr.PushBack(m_SaveData[i].boundary.bottom, Json.GetAllocator());
			Frame.AddMember(key, arr, Json.GetAllocator());
			
			key.SetString(_T("Delay"));
			value.SetFloat(m_SaveData[i].delay);
			Frame.AddMember(key, value, Json.GetAllocator());

			Array.PushBack(Frame, Json.GetAllocator());
		}

		Json.AddMember(key_, Array, Json.GetAllocator());
		
		rapidjson::GenericStringBuffer< rapidjson::UTF16<>> strbuf;
		rapidjson::Writer < rapidjson::GenericStringBuffer< rapidjson::UTF16<> >, rapidjson::UTF16<>, rapidjson::UTF16<> > writer(strbuf);
		Json.Accept(writer);

		CString str = strbuf.GetString();

		ar.WriteString(str);
	}
	else
	{
		// TODO: 여기에 로딩 코드를 추가합니다.
		// ar >> json 문자열
		m_SelectIndex = -1;
		m_SaveData.clear();
		m_ShowImage.Destroy();

		if (m_ImageList.size() != 0)
		{
			for (auto img : m_ImageList)
			{
				img->ReleaseDC();
				img->Destroy();
				img = nullptr;
			}

			m_ImageList.clear();
		}

		CString str;
		ar.ReadString(str);

		rapidjson::GenericDocument< rapidjson::UTF16<> > Json;
		Json.Parse(str.GetBuffer());
		assert(Json.IsObject());

		CString tFileName = _T("./image/");
		m_FileName = Json.MemberBegin()->name.GetString();
		tFileName += m_FileName;
		m_ShowImage.Load(tFileName);

		SearchObject(Json);

		CAnimationToolView* mainView = ((CMainFrame*)AfxGetMainWnd())->m_pMainView;
		mainView->Invalidate(TRUE);

		CMFCColorDialog colorDlg;
		colorDlg.DoModal();
		m_ColorKey = colorDlg.GetColor();
		((CMainFrame*)AfxGetMainWnd())->ChangeColorKey();

		for (int i = 0; i < m_SaveData.size(); i++)
		{
			mainView->m_MousePos[0] = m_SaveData[i].bitmapStartPoint;
			mainView->m_MousePos[1] = m_SaveData[i].bitmapStartPoint + m_SaveData[i].bitmapSize;

			mainView->AddImageList(false);
		}
	}
}

#ifdef SHARED_HANDLERS

// 축소판 그림을 지원합니다.
void CAnimationToolDoc::OnDrawThumbnail(CDC& dc, LPRECT lprcBounds)
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
void CAnimationToolDoc::InitializeSearchContent()
{
	CString strSearchContent;
	// 문서의 데이터에서 검색 콘텐츠를 설정합니다.
	// 콘텐츠 부분은 ";"로 구분되어야 합니다.

	// 예: strSearchContent = _T("point;rectangle;circle;ole object;");
	SetSearchContent(strSearchContent);
}

void CAnimationToolDoc::SetSearchContent(const CString& value)
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

// CAnimationToolDoc 진단

#ifdef _DEBUG
void CAnimationToolDoc::AssertValid() const
{
	CDocument::AssertValid();
}

void CAnimationToolDoc::Dump(CDumpContext& dc) const
{
	CDocument::Dump(dc);
}
#endif //_DEBUG


// CAnimationToolDoc 명령
void CAnimationToolDoc::SearchObject(const rapidjson::GenericValue<rapidjson::UTF16<>>& Json)
{
	for (rapidjson::GenericValue< rapidjson::UTF16<> >::ConstMemberIterator itr = Json.MemberBegin(); itr != Json.MemberEnd(); ++itr)
	{
		switch (itr->value.GetType())
		{
		case rapidjson::kObjectType:
		{
			SearchObject(itr->value.GetObject());
			break;	// Object 타입의 노드 검색
		}
		case rapidjson::kArrayType:
		{
			CString  name = itr->name.GetString();
			auto it = itr->value.GetArray().Begin();

			if (name == _T("Pos"))
			{
				m_SaveData[m_SelectIndex].bitmapStartPoint.x = it++->GetInt();
				m_SaveData[m_SelectIndex].bitmapStartPoint.y = it->GetInt();
			}
			else if (name == _T("Size"))
			{
				m_SaveData[m_SelectIndex].bitmapSize.cx = it++->GetInt();
				m_SaveData[m_SelectIndex].bitmapSize.cy = it->GetInt();
			}
			else if (name == _T("Pivot"))
			{
				m_SaveData[m_SelectIndex].pivot.x = it++->GetInt();
				m_SaveData[m_SelectIndex].pivot.y = it->GetInt();
			}
			else if (name == _T("Boundary"))
			{
				
				m_SaveData[m_SelectIndex].boundary.left = it++->GetInt();
				m_SaveData[m_SelectIndex].boundary.top = it++->GetInt();
				m_SaveData[m_SelectIndex].boundary.right = it++->GetInt();
				m_SaveData[m_SelectIndex].boundary.bottom = it->GetInt();
			}
			else
			{
				SearchArray(itr->value.GetArray());
			}
			break;	// Array 타입의 노드 검색
		}
		case rapidjson::kNumberType:
		{
			// 노드 추가
			m_SaveData[m_SelectIndex].delay = Json[_T("Delay")].GetFloat();
		}
		break;
		default:
			break;
		}
	}
}

void CAnimationToolDoc::SearchArray(const rapidjson::GenericValue<rapidjson::UTF16<>>& Json)
{
	for (rapidjson::GenericValue< rapidjson::UTF16<> >::ConstValueIterator itr = Json.Begin(); itr != Json.End(); ++itr)
	{
		switch (itr->GetType())
		{
		case rapidjson::kObjectType:
		{
			SaveData tempData;
			++m_SelectIndex;
			m_SaveData.push_back(tempData);
			SearchObject(itr->GetObject());
			break;	// Object 타입의 노드 검색
		}
		case rapidjson::kArrayType:
		{
			SearchArray(itr->GetArray());
			break;	// Array 타입의 노드 검색
		}
		default:
			break;
		}
	}
}