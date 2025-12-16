using HwpLib.CompoundFile;
using HwpLib.Object.BodyText.Control;
using HwpLib.Object.Etc;
using HwpLib.Reader.BodyText.Control.Secd;

namespace HwpLib.Reader.BodyText.Control;

/// <summary>
/// 구역 정의 컨트롤을 읽기 위한 객체
/// </summary>
public class ForControlSectionDefine
{
    /// <summary>
    /// 구역 정의 컨트롤
    /// </summary>
    private ControlSectionDefine? _secd;

    /// <summary>
    /// 스트림 리더
    /// </summary>
    private CompoundStreamReader? _sr;

    /// <summary>
    /// 컨트롤헤더의 레벨
    /// </summary>
    private short _ctrlHeaderLevel;

    /// <summary>
    /// 미/각주모양 레코드 인덱스
    /// </summary>
    private int _endFootnoteShapeIndex;

    /// <summary>
    /// 쪽 테두리/배경 레코드 인덱스
    /// </summary>
    private int _pageBorderFillIndex;

    /// <summary>
    /// 생성자
    /// </summary>
    public ForControlSectionDefine()
    {
        _endFootnoteShapeIndex = 0;
        _pageBorderFillIndex = 0;
    }

    /// <summary>
    /// 구역 정의 컨트롤을 읽는다.
    /// </summary>
    /// <param name="secd">구역 정의 컨트롤 객체</param>
    /// <param name="sr">스트림 리더</param>
    public void Read(ControlSectionDefine secd, CompoundStreamReader sr)
    {
        _secd = secd;
        _sr = sr;
        _ctrlHeaderLevel = (short)sr.CurrentRecordHeader!.Level;

        CtrlHeader();

        while (!sr.IsEndOfStream())
        {
            if (!sr.IsImmediatelyAfterReadingHeader)
            {
                sr.ReadRecordHeader();
            }

            if (_ctrlHeaderLevel >= sr.CurrentRecordHeader!.Level)
            {
                break;
            }
            ReadBody();
        }
    }

    /// <summary>
    /// 구역 정의 컨트롤의 컨트롤 헤더 레코드를 읽는다.
    /// </summary>
    private void CtrlHeader()
    {
        ForCtrlHeaderSecd.Read(_secd!.Header, _sr!);
    }

    /// <summary>
    /// 이미 읽은 레코드 헤더에 따른 레코드 내용을 읽는다.
    /// </summary>
    private void ReadBody()
    {
        var tagId = _sr!.CurrentRecordHeader!.TagId;

        if (tagId == HWPTag.PageDef)
        {
            PageDef();
        }
        else if (tagId == HWPTag.FootnoteShape)
        {
            EndFootnoteShapes();
        }
        else if (tagId == HWPTag.PageBorderFill)
        {
            PageBorderFills();
        }
        else if (tagId == HWPTag.ListHeader)
        {
            BatangPageInfo();
        }
        else if (tagId == HWPTag.CtrlData)
        {
            CtrlData();
        }
        else
        {
            _sr.SkipToEndRecord();
        }
    }

    /// <summary>
    /// 용지 설정 레코드를 읽는다.
    /// </summary>
    private void PageDef()
    {
        ForPageDef.Read(_secd!.PageDef, _sr!);
    }

    /// <summary>
    /// 각주/미주 모양 레코드를 읽는다.
    /// </summary>
    private void EndFootnoteShapes()
    {
        if (_endFootnoteShapeIndex == 0)
        {
            FootNoteShape();
        }
        else if (_endFootnoteShapeIndex == 1)
        {
            EndNoteShape();
        }
        _endFootnoteShapeIndex++;
    }

    /// <summary>
    /// 각주 모양 레코드를 읽는다.
    /// </summary>
    private void FootNoteShape()
    {
        ForFootEndNoteShape.Read(_secd!.FootNoteShape, _sr!);
    }

    /// <summary>
    /// 미주 모양 레코드를 읽는다.
    /// </summary>
    private void EndNoteShape()
    {
        ForFootEndNoteShape.Read(_secd!.EndNoteShape, _sr!);
    }

    /// <summary>
    /// 쪽 테두리/배경 레코드를 읽는다.
    /// </summary>
    private void PageBorderFills()
    {
        if (_pageBorderFillIndex == 0)
        {
            BothPageBorderFill();
        }
        else if (_pageBorderFillIndex == 1)
        {
            EvenPageBorderFill();
        }
        else if (_pageBorderFillIndex == 2)
        {
            OddPageBorderFill();
        }
        _pageBorderFillIndex++;
    }

    /// <summary>
    /// 양쪽 페이지를 위한 쪽 테두리/배경 레코드를 읽는다.
    /// </summary>
    private void BothPageBorderFill()
    {
        ForPageBorderFill.Read(_secd!.BothPageBorderFill, _sr!);
    }

    /// <summary>
    /// 짝수쪽 페이지를 위한 쪽 테두리/배경 레코드를 읽는다.
    /// </summary>
    private void EvenPageBorderFill()
    {
        ForPageBorderFill.Read(_secd!.EvenPageBorderFill, _sr!);
    }

    /// <summary>
    /// 홀수쪽 페이지를 위한 쪽 테두리/배경 레코드를 읽는다.
    /// </summary>
    private void OddPageBorderFill()
    {
        ForPageBorderFill.Read(_secd!.OddPageBorderFill, _sr!);
    }

    /// <summary>
    /// 바탕쪽 정보를 읽는다.
    /// </summary>
    private void BatangPageInfo()
    {
        ForBatangPageInfo.Read(_secd!.AddNewBatangPageInfo(), _sr!);
    }

    /// <summary>
    /// 컨트롤 데이터를 읽는다.
    /// </summary>
    private void CtrlData()
    {
        _secd!.CreateCtrlData();
        var ctrlData = ForCtrlData.Read(_sr!);
        _secd.SetCtrlData(ctrlData);
    }
}
