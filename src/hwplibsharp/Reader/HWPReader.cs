using HwpLib.CompoundFile;
using HwpLib.Object;
using HwpLib.Object.DocInfo.BinData;
using HwpLib.Object.FileHeader;
using HwpLib.Reader.DocInfo;

namespace HwpLib.Reader;

/// <summary>
/// 한글 파일을 읽기 위한 객체
/// </summary>
public class HWPReader : IDisposable
{
    /// <summary>
    /// HWP파일을 나타내는 객체
    /// </summary>
    private HWPFile? _hwpFile;

    /// <summary>
    /// MS Compound 파일을 읽기 위한 리더 객체
    /// </summary>
    private CompoundFileReader? _cfr;

    /// <summary>
    /// 리소스 해제 여부
    /// </summary>
    private bool _disposed;

    /// <summary>
    /// 생성자
    /// </summary>
    private HWPReader()
    {
    }

    /// <summary>
    /// hwp 파일을 읽는다.
    /// </summary>
    /// <param name="filePath">hwp파일의 경로</param>
    /// <returns>HWPFile 객체</returns>
    public static HWPFile FromFile(string filePath)
    {
        using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        return FromStream(stream);
    }

    /// <summary>
    /// hwp 파일을 스트림에서 읽는다.
    /// </summary>
    /// <param name="stream">hwp파일을 가리키는 스트림</param>
    /// <returns>HWPFile 객체</returns>
    public static HWPFile FromStream(Stream stream)
    {
        using var reader = new HWPReader();
        reader._hwpFile = new HWPFile();
        reader._cfr = new CompoundFileReader(stream);

        reader.ReadFileHeader();
        if (reader.HasPassword())
        {
            throw new NotSupportedException("Files with passwords are not supported.");
        }

        reader.ReadDocInfo();
        reader.ReadBodyText();
        reader.ReadBinData();
        // reader.ReadSummaryInformation();
        // reader.ReadScripts();

        return reader._hwpFile;
    }

    /// <summary>
    /// Base64 문자열에서 hwp 파일을 읽는다.
    /// </summary>
    /// <param name="base64">Base64로 인코딩된 hwp 파일</param>
    /// <returns>HWPFile 객체</returns>
    public static HWPFile FromBase64String(string base64)
    {
        byte[] binary = Convert.FromBase64String(base64);
        using var stream = new MemoryStream(binary);
        return FromStream(stream);
    }

    /// <summary>
    /// URL에서 hwp 파일을 읽는다.
    /// </summary>
    /// <param name="url">hwp 파일의 URL</param>
    /// <returns>HWPFile 객체</returns>
    public static HWPFile FromUrl(string url)
    {
        using var httpClient = new HttpClient();
        var bytes = httpClient.GetByteArrayAsync(url).GetAwaiter().GetResult();
        using var stream = new MemoryStream(bytes);
        return FromStream(stream);
    }

    /// <summary>
    /// URL에서 hwp 파일을 비동기로 읽는다.
    /// </summary>
    /// <param name="url">hwp 파일의 URL</param>
    /// <param name="cancellationToken">취소 토큰</param>
    /// <returns>HWPFile 객체</returns>
    public static async Task<HWPFile> FromUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        using var httpClient = new HttpClient();
        var bytes = await httpClient.GetByteArrayAsync(url, cancellationToken);
        using var stream = new MemoryStream(bytes);
        return FromStream(stream);
    }

    /// <summary>
    /// FileHeader 스트림을 읽는다.
    /// </summary>
    private void ReadFileHeader()
    {
        using var sr = _cfr!.GetChildStreamReader("FileHeader", false, null);
        ForFileHeader.Read(_hwpFile!.FileHeader, sr);
    }

    /// <summary>
    /// 암호화된 파일인지 여부를 반환한다.
    /// </summary>
    /// <returns>암호화된 파일인지 여부</returns>
    private bool HasPassword()
    {
        return _hwpFile!.FileHeader.HasPassword;
    }

    /// <summary>
    /// 배포용 문서 파일인지 여부를 반환한다.
    /// </summary>
    /// <returns>배포용 문서 파일인지 여부</returns>
    private bool IsDistribution()
    {
        return _hwpFile!.FileHeader.IsDistribution;
    }

    /// <summary>
    /// 압축된 파일인지 여부를 반환한다.
    /// </summary>
    /// <returns>압축된 파일인지 여부</returns>
    private bool IsCompressed()
    {
        return _hwpFile!.FileHeader.Compressed;
    }

    /// <summary>
    /// 파일의 버전을 반환한다.
    /// </summary>
    /// <returns>파일의 버전</returns>
    private FileVersion GetVersion()
    {
        return _hwpFile!.FileHeader.Version;
    }

    /// <summary>
    /// DocInfo 스트림을 읽는다.
    /// </summary>
    private void ReadDocInfo()
    {
        using var sr = _cfr!.GetChildStreamReader("DocInfo", IsCompressed(), GetVersion());
        new ForDocInfo().Read(_hwpFile!.DocInfo, sr);
    }

    /// <summary>
    /// BodyText 스토리지를 읽는다.
    /// </summary>
    private void ReadBodyText()
    {
        if (_cfr!.IsChildStorage("BodyText"))
        {
            _cfr.MoveChildStorage("BodyText");

            // Section 스트림 이름들 찾기 (Section0, Section1, ...)
            var sectionNames = _cfr.ListChildNames()
                .Where(name => name.StartsWith("Section"))
                .OrderBy(name => 
                {
                    // Section 이름에서 숫자 추출
                    string numPart = name.Substring("Section".Length);
                    return int.TryParse(numPart, out int num) ? num : 0;
                })
                .ToList();

            foreach (var sectionName in sectionNames)
            {
                var section = _hwpFile!.BodyText.AddNewSection();
                ReadSection(sectionName, section);
            }

            _cfr.MoveParentStorage();
        }
    }

    /// <summary>
    /// Section 스트림을 읽는다.
    /// </summary>
    /// <param name="sectionName">섹션 스트림 이름</param>
    /// <param name="section">섹션 객체</param>
    private void ReadSection(string sectionName, HwpLib.Object.BodyText.Section section)
    {
        using var sr = _cfr!.GetChildStreamReader(sectionName, IsCompressed(), GetVersion());
        
        // Section 스트림 읽기
        new BodyText.ForSection().Read(section, sr);
    }

    /// <summary>
    /// BinData 스토리지를 읽는다.
    /// </summary>
    private void ReadBinData()
    {
        if (_cfr!.IsChildStorage("BinData"))
        {
            _cfr.MoveChildStorage("BinData");

            var names = _cfr.ListChildNames();
            foreach (var name in names)
            {
                int id = NameToID(name);
                BinDataCompress compressMethod = GetCompressMethod(id);
                byte[] data = ReadEmbeddedBinaryData(name, compressMethod);
                _hwpFile!.BinData.AddNewEmbeddedBinaryData(name, data, compressMethod);
            }

            _cfr.MoveParentStorage();
        }
    }

    /// <summary>
    /// BinData 이름에서 ID를 추출한다.
    /// </summary>
    /// <param name="name">BinData 스트림 이름</param>
    /// <returns>ID</returns>
    private static int NameToID(string name)
    {
        string id = name.Substring(3, 4);
        return int.Parse(id, System.Globalization.NumberStyles.HexNumber);
    }

    /// <summary>
    /// 압축 방법을 가져온다.
    /// </summary>
    /// <param name="id">BinData ID</param>
    /// <returns>압축 방법</returns>
    private BinDataCompress GetCompressMethod(int id)
    {
        var binDataList = _hwpFile!.DocInfo.BinDataList;
        if (id > 0 && id <= binDataList.Count)
        {
            return binDataList[id - 1].Property.Compress;
        }
        return BinDataCompress.ByStorageDefault;
    }

    /// <summary>
    /// BinData 스토리지 아래의 스트림을 읽는다.
    /// </summary>
    /// <param name="name">스트림 이름</param>
    /// <param name="compressMethod">압축 방법</param>
    /// <returns>스트림에 저장된 데이터</returns>
    private byte[] ReadEmbeddedBinaryData(string name, BinDataCompress compressMethod)
    {
        bool isCompress = IsCompressBinData(compressMethod);
        using var sr = _cfr!.GetChildStreamReader(name, isCompress, null);
        return sr.ReadBytes((int)sr.Size);
    }

    /// <summary>
    /// BinData의 압축 여부를 반환한다.
    /// </summary>
    /// <param name="compressMethod">압축 방법</param>
    /// <returns>BinData의 압축 여부</returns>
    private bool IsCompressBinData(BinDataCompress compressMethod)
    {
        return compressMethod switch
        {
            BinDataCompress.ByStorageDefault => IsCompressed(),
            BinDataCompress.Compress => true,
            BinDataCompress.NoCompress => false,
            _ => false
        };
    }

    /// <summary>
    /// 리소스를 해제한다.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// 리소스를 해제한다.
    /// </summary>
    /// <param name="disposing">관리 리소스 해제 여부</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _cfr?.Dispose();
            }
            _disposed = true;
        }
    }

    /// <summary>
    /// 소멸자
    /// </summary>
    ~HWPReader()
    {
        Dispose(false);
    }
}
