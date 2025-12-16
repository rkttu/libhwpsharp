using HwpLib.Object.BodyText.Control.CtrlHeader;
using HwpLib.Object.BodyText.Control.Gso.Caption;
using HwpLib.Object.BodyText.Control.Table;
using HwpLib.Object.Etc;

namespace HwpLib.Object.BodyText.Control;

/// <summary>
/// 표 컨트롤
/// </summary>
public class ControlTable : Control
{
    /// <summary>
    /// 캡션
    /// </summary>
    private Caption? _caption;

    /// <summary>
    /// 표 정보
    /// </summary>
    private readonly Table.Table _table;

    /// <summary>
    /// 행 리스트
    /// </summary>
    private readonly List<Row> _rowList;

    /// <summary>
    /// 생성자
    /// </summary>
    public ControlTable()
        : this(new CtrlHeaderGso(ControlType.Table))
    {
    }

    /// <summary>
    /// 생성자
    /// </summary>
    /// <param name="header">컨트롤 헤더</param>
    public ControlTable(CtrlHeader.CtrlHeader header)
        : base(header)
    {
        _caption = null;
        _table = new Table.Table();
        _rowList = new List<Row>();
    }

    /// <summary>
    /// 그리기 객체 용 컨트롤 헤더를 반환한다.
    /// </summary>
    public new CtrlHeaderGso Header => (CtrlHeaderGso)base.Header!;

    /// <summary>
    /// 캡션 객체를 생성한다.
    /// </summary>
    public void CreateCaption()
    {
        _caption = new Caption();
    }

    /// <summary>
    /// 캡션 객체를 삭제한다.
    /// </summary>
    public void DeleteCaption()
    {
        _caption = null;
    }

    /// <summary>
    /// 캡션 객체를 반환한다.
    /// </summary>
    public Caption? Caption => _caption;

    /// <summary>
    /// 표 정보 객체를 반환한다.
    /// </summary>
    public Table.Table Table => _table;

    /// <summary>
    /// 새로운 행 객체를 생성하고 리스트에 추가한다.
    /// </summary>
    /// <returns>새로 생성된 행 객체</returns>
    public Row AddNewRow()
    {
        var row = new Row();
        _rowList.Add(row);
        return row;
    }

    /// <summary>
    /// 행 리스트를 반환한다.
    /// </summary>
    public IReadOnlyList<Row> RowList => _rowList;

    /// <summary>
    /// 지정된 인덱스의 행을 삭제한다.
    /// </summary>
    /// <param name="index">삭제할 행의 인덱스</param>
    public void RemoveRow(int index)
    {
        _rowList.RemoveAt(index);
    }

    /// <summary>
    /// 객체를 복제한다.
    /// </summary>
    /// <returns>복제된 객체</returns>
    public override Control Clone()
    {
        var cloned = new ControlTable();
        cloned.CopyControlPart(this);

        if (_caption != null)
        {
            cloned.CreateCaption();
            cloned._caption!.Copy(_caption);
        }
        else
        {
            cloned._caption = null;
        }

        cloned._table.Copy(_table);

        foreach (var row in _rowList)
        {
            cloned._rowList.Add(row.Clone());
        }

        return cloned;
    }
}
