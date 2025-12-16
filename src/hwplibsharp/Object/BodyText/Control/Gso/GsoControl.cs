using HwpLib.Object.BodyText.Control.CtrlHeader;
using HwpLib.Object.BodyText.Control.Gso.Caption;
using HwpLib.Object.BodyText.Control.Gso.ShapeComponent;

namespace HwpLib.Object.BodyText.Control.Gso;

/// <summary>
/// 그리기 개체 컨트롤
/// </summary>
public abstract class GsoControl : Control
{
    /// <summary>
    /// 캡션 정보
    /// </summary>
    private Caption.Caption? _caption;

    /// <summary>
    /// 그리기 개체의 공통 요소
    /// </summary>
    protected ShapeComponent.ShapeComponent ShapeComponentInternal;

    /// <summary>
    /// 생성자
    /// </summary>
    protected GsoControl()
        : this(new CtrlHeaderGso())
    {
    }

    /// <summary>
    /// 생성자
    /// </summary>
    /// <param name="header">그리기 개체를 위한 컨트롤 헤더</param>
    protected GsoControl(CtrlHeaderGso header)
        : base(header)
    {
        _caption = null;
        ShapeComponentInternal = new ShapeComponentNormal();
    }

    /// <summary>
    /// 그리기 개체를 위한 컨트롤 헤더 객체를 반환한다.
    /// </summary>
    public new CtrlHeaderGso? GetHeader()
    {
        return Header as CtrlHeaderGso;
    }

    /// <summary>
    /// 그리기 개체 아이디를 반환한다.
    /// </summary>
    public uint GsoId => ShapeComponentInternal.GsoId;

    /// <summary>
    /// 그리기 개체 아이디를 설정한다.
    /// </summary>
    /// <param name="gsoId">그리기 개체 아이디</param>
    protected void SetGsoId(uint gsoId)
    {
        ShapeComponentInternal.GsoId = gsoId;
    }

    /// <summary>
    /// 그리기 개체 타입을 반환한다.
    /// </summary>
    public GsoControlType GsoType => GsoControlTypeExtensions.IdOf((long)GsoId);

    /// <summary>
    /// 캡션 객체를 생성한다.
    /// </summary>
    public void CreateCaption()
    {
        _caption = new Caption.Caption();
    }

    /// <summary>
    /// 캡션 객체를 삭제한다.
    /// </summary>
    public void DeleteCaption()
    {
        _caption = null;
    }

    /// <summary>
    /// 캡션 정보 객체를 반환한다.
    /// </summary>
    public Caption.Caption? Caption => _caption;

    /// <summary>
    /// 캡션 정보 객체를 설정한다.
    /// </summary>
    /// <param name="caption">캡션 정보 객체</param>
    public void SetCaption(Caption.Caption? caption)
    {
        _caption = caption;
    }

    /// <summary>
    /// 그리기 개체의 공통 요소를 나타내는 객체를 반환한다.
    /// </summary>
    public ShapeComponent.ShapeComponent ShapeComponent => ShapeComponentInternal;

    /// <summary>
    /// GsoControl 부분을 복사한다.
    /// </summary>
    /// <param name="from">복사할 원본 객체</param>
    public void CopyGsoControlPart(GsoControl from)
    {
        CopyControlPart(from);

        if (from._caption != null)
        {
            CreateCaption();
            _caption?.Copy(from._caption);
        }

        ShapeComponentInternal.Copy(from.ShapeComponentInternal);
    }
}
