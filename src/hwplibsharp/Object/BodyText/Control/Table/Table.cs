namespace HwpLib.Object.BodyText.Control.Table;

/// <summary>
/// 테이블 정보를 포함하는 레코드
/// </summary>
public class Table
{
    /// <summary>
    /// 속성
    /// </summary>
    private readonly TableProperty _property;

    /// <summary>
    /// 행의 개수
    /// </summary>
    private int _rowCount;

    /// <summary>
    /// 열의 개수
    /// </summary>
    private int _columnCount;

    /// <summary>
    /// 셀 사이의 공간
    /// </summary>
    private int _cellSpacing;

    /// <summary>
    /// 왼쪽 안쪽 여백
    /// </summary>
    private int _leftInnerMargin;

    /// <summary>
    /// 오른쪽 안쪽 여백
    /// </summary>
    private int _rightInnerMargin;

    /// <summary>
    /// 위쪽 안쪽 여백
    /// </summary>
    private int _topInnerMargin;

    /// <summary>
    /// 아래쪽 안쪽 여백
    /// </summary>
    private int _bottomInnerMargin;

    /// <summary>
    /// 각 행의 셀의 개수를 저장하는 리스트
    /// </summary>
    private readonly List<int> _cellCountOfRowList;

    /// <summary>
    /// 참조된 테두리/배경 id
    /// </summary>
    private int _borderFillId;

    /// <summary>
    /// 영역 속성 리스트 (5.0.1.0 이상)
    /// </summary>
    private readonly List<ZoneInfo> _zoneInfoList;

    /// <summary>
    /// 생성자
    /// </summary>
    public Table()
    {
        _property = new TableProperty();
        _cellCountOfRowList = new List<int>();
        _zoneInfoList = new List<ZoneInfo>();
    }

    /// <summary>
    /// 속성 객체를 반환한다.
    /// </summary>
    public TableProperty Property => _property;

    /// <summary>
    /// 행의 개수를 반환하거나 설정한다.
    /// </summary>
    public int RowCount
    {
        get => _rowCount;
        set => _rowCount = value;
    }

    /// <summary>
    /// 열의 개수를 반환하거나 설정한다.
    /// </summary>
    public int ColumnCount
    {
        get => _columnCount;
        set => _columnCount = value;
    }

    /// <summary>
    /// 셀 사이의 공간의 크기를 반환하거나 설정한다.
    /// </summary>
    public int CellSpacing
    {
        get => _cellSpacing;
        set => _cellSpacing = value;
    }

    /// <summary>
    /// 왼쪽 안쪽 여백의 크기를 반환하거나 설정한다.
    /// </summary>
    public int LeftInnerMargin
    {
        get => _leftInnerMargin;
        set => _leftInnerMargin = value;
    }

    /// <summary>
    /// 오른쪽 안쪽 여백의 크기를 반환하거나 설정한다.
    /// </summary>
    public int RightInnerMargin
    {
        get => _rightInnerMargin;
        set => _rightInnerMargin = value;
    }

    /// <summary>
    /// 위쪽 안쪽 여백의 크기를 반환하거나 설정한다.
    /// </summary>
    public int TopInnerMargin
    {
        get => _topInnerMargin;
        set => _topInnerMargin = value;
    }

    /// <summary>
    /// 아래쪽 안쪽 여백의 크기를 반환하거나 설정한다.
    /// </summary>
    public int BottomInnerMargin
    {
        get => _bottomInnerMargin;
        set => _bottomInnerMargin = value;
    }

    /// <summary>
    /// 행의 셀 개수를 추가한다.
    /// </summary>
    /// <param name="cellCountOfRow">특정 행의 셀 개수</param>
    public void AddCellCountOfRow(int cellCountOfRow)
    {
        _cellCountOfRowList.Add(cellCountOfRow);
    }

    /// <summary>
    /// 행의 셀 개수를 삭제한다.
    /// </summary>
    /// <param name="index">삭제할 인덱스</param>
    public void RemoveCellCountOfRow(int index)
    {
        _cellCountOfRowList.RemoveAt(index);
    }

    /// <summary>
    /// 행의 셀 개수 리스트를 지운다.
    /// </summary>
    public void ClearCellCountOfRowList()
    {
        _cellCountOfRowList.Clear();
    }

    /// <summary>
    /// 각 행의 셀의 개수를 저장하는 리스트를 반환한다.
    /// </summary>
    public IReadOnlyList<int> CellCountOfRowList => _cellCountOfRowList;

    /// <summary>
    /// 참조된 테두리/배경 id를 반환하거나 설정한다.
    /// </summary>
    public int BorderFillId
    {
        get => _borderFillId;
        set => _borderFillId = value;
    }

    /// <summary>
    /// 새로운 영역 속성 객체를 생성하고 리스트에 추가한다. (5.0.1.0 이상)
    /// </summary>
    /// <returns>새로 생성된 영역 속성 객체</returns>
    public ZoneInfo AddNewZoneInfo()
    {
        ZoneInfo zi = new ZoneInfo();
        _zoneInfoList.Add(zi);
        return zi;
    }

    /// <summary>
    /// 영역 속성 리스트를 반환한다. (5.0.1.0 이상)
    /// </summary>
    public IReadOnlyList<ZoneInfo> ZoneInfoList => _zoneInfoList;

    /// <summary>
    /// 다른 객체에서 값을 복사한다.
    /// </summary>
    /// <param name="from">복사할 원본 객체</param>
    public void Copy(Table from)
    {
        _property.Copy(from._property);
        _rowCount = from._rowCount;
        _columnCount = from._columnCount;
        _cellSpacing = from._cellSpacing;
        _leftInnerMargin = from._leftInnerMargin;
        _rightInnerMargin = from._rightInnerMargin;
        _topInnerMargin = from._topInnerMargin;
        _bottomInnerMargin = from._bottomInnerMargin;

        _cellCountOfRowList.Clear();
        foreach (var cellCount in from._cellCountOfRowList)
        {
            _cellCountOfRowList.Add(cellCount);
        }

        _borderFillId = from._borderFillId;

        _zoneInfoList.Clear();
        foreach (var zoneInfo in from._zoneInfoList)
        {
            _zoneInfoList.Add(zoneInfo.Clone());
        }
    }
}
