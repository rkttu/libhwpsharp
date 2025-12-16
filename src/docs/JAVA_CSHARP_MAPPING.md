# Java-C# 매핑 문서

> 원본 Java 라이브러리: [neolord0/hwplib](https://github.com/neolord0/hwplib)
> C# 포팅: [rkttu/libhwpsharp](https://github.com/rkttu/libhwpsharp)

## ?? 개요

이 문서는 Java 버전 hwplib과 C# 버전 hwplibsharp 간의 클래스, 메서드, 속성 매핑을 정리합니다.

---

## ?? 네이밍 컨벤션

| 항목 | Java | C# |
|------|------|-----|
| 패키지/네임스페이스 | `kr.dogfoot.hwplib.*` | `HwpLib.*` |
| 클래스명 | PascalCase | PascalCase (동일) |
| 메서드명 | camelCase | PascalCase |
| Getter | `getXxx()` | `Xxx` (Property) |
| Setter | `setXxx(value)` | `Xxx { set; }` (Property) |
| 필드명 | camelCase | `_camelCase` (private) |
| 상수 | UPPER_SNAKE_CASE | PascalCase |

---

## ?? 패키지/네임스페이스 매핑

| Java 패키지 | C# 네임스페이스 |
|-------------|-----------------|
| `kr.dogfoot.hwplib.object` | `HwpLib.Object` |
| `kr.dogfoot.hwplib.object.bindata` | `HwpLib.Object.BinData` |
| `kr.dogfoot.hwplib.object.bodytext` | `HwpLib.Object.BodyText` |
| `kr.dogfoot.hwplib.object.bodytext.control` | `HwpLib.Object.BodyText.Control` |
| `kr.dogfoot.hwplib.object.bodytext.control.bookmark` | `HwpLib.Object.BodyText.Control.Bookmark` |
| `kr.dogfoot.hwplib.object.bodytext.control.ctrlheader` | `HwpLib.Object.BodyText.Control.CtrlHeader` |
| `kr.dogfoot.hwplib.object.bodytext.control.equation` | `HwpLib.Object.BodyText.Control.Equation` |
| `kr.dogfoot.hwplib.object.bodytext.control.form` | `HwpLib.Object.BodyText.Control.Form` |
| `kr.dogfoot.hwplib.object.bodytext.control.gso` | `HwpLib.Object.BodyText.Control.Gso` |
| `kr.dogfoot.hwplib.object.bodytext.control.table` | `HwpLib.Object.BodyText.Control.Table` |
| `kr.dogfoot.hwplib.object.bodytext.paragraph` | `HwpLib.Object.BodyText.Paragraph` |
| `kr.dogfoot.hwplib.object.docinfo` | `HwpLib.Object.DocInfo` |
| `kr.dogfoot.hwplib.object.fileheader` | `HwpLib.Object.FileHeader` |
| `kr.dogfoot.hwplib.reader` | `HwpLib.Reader` |
| `kr.dogfoot.hwplib.reader.bodytext` | `HwpLib.Reader.BodyText` |
| `kr.dogfoot.hwplib.reader.docinfo` | `HwpLib.Reader.DocInfo` |
| `kr.dogfoot.hwplib.writer` | `HwpLib.Writer` |
| `kr.dogfoot.hwplib.writer.bodytext` | `HwpLib.Writer.BodyText` |
| `kr.dogfoot.hwplib.writer.docinfo` | `HwpLib.Writer.DocInfo` |
| `kr.dogfoot.hwplib.tool.blankfilemaker` | `HwpLib.Tool.BlankFileMaker` |
| `kr.dogfoot.hwplib.tool.objectfinder` | `HwpLib.Tool.ObjectFinder` |
| `kr.dogfoot.hwplib.tool.paragraphadder` | `HwpLib.Tool.ParagraphAdder` |
| `kr.dogfoot.hwplib.tool.textextractor` | `HwpLib.Tool.TextExtractor` |
| `kr.dogfoot.hwplib.util.binary` | `HwpLib.Util.Binary` |
| `kr.dogfoot.hwplib.util.compoundFile` | `HwpLib.CompoundFile` |

---

## ??? 핵심 클래스 매핑

### HWPFile

| Java | C# | 설명 |
|------|-----|------|
| `HWPFile` | `HWPFile` | HWP 파일 객체 |
| `getFileHeader()` | `FileHeader` | 파일 헤더 속성 |
| `getDocInfo()` | `DocInfo` | 문서 정보 속성 |
| `getBodyText()` | `BodyText` | 본문 속성 |
| `getBinData()` | `BinData` | 바이너리 데이터 속성 |
| `getScripts()` | `Scripts` | 스크립트 속성 |
| `getSummaryInformation()` | `SummaryInformation` | 요약 정보 속성 |
| `setSummaryInformation(v)` | `SummaryInformation { set; }` | 요약 정보 설정 |
| `clone(deepCopyImage)` | `Clone(deepCopyImage)` | 복제 메서드 |
| `copy(from, deepCopyImage)` | `Copy(from, deepCopyImage)` | 복사 메서드 |

### HWPReader

| Java | C# | 설명 |
|------|-----|------|
| `HWPReader.fromFile(filepath)` | `HWPReader.FromFile(filePath)` | 파일에서 읽기 |
| `HWPReader.fromInputStream(is)` | `HWPReader.FromStream(stream)` | 스트림에서 읽기 |
| `HWPReader.fromURL(url)` | *(미구현)* | URL에서 읽기 |
| `HWPReader.fromBase64String(base64)` | `HWPReader.FromBase64String(base64)` | Base64에서 읽기 |

### HWPWriter

| Java | C# | 설명 |
|------|-----|------|
| `HWPWriter.toFile(hwpFile, filepath)` | `HWPWriter.ToFile(hwpFile, filePath)` | 파일로 쓰기 |
| `HWPWriter.toStream(hwpFile)` | `HWPWriter.ToStream(hwpFile)` | 스트림으로 쓰기 |

---

## ?? FileHeader 매핑

| Java | C# | 설명 |
|------|-----|------|
| `FileHeader` | `FileHeader` | 파일 헤더 클래스 |
| `getVersion()` | `Version` | 버전 정보 |
| `isCompressed()` | `Compressed` | 압축 여부 |
| `hasPassword()` | `HasPassword` | 암호 여부 |
| `isDistribution()` | `IsDistribution` | 배포용 문서 여부 |
| `isScript()` | `IsScript` | 스크립트 포함 여부 |
| `isDRM()` | `IsDRM` | DRM 여부 |
| `isXMLTemplate()` | `IsXMLTemplate` | XML 템플릿 여부 |
| `isDocumentHistory()` | `IsDocumentHistory` | 문서 이력 여부 |
| `isVCS()` | `IsVCS` | 버전 관리 여부 |

### FileVersion

| Java | C# | 설명 |
|------|-----|------|
| `FileVersion` | `FileVersion` | 파일 버전 클래스 |
| `getMM()` | `MM` | Major 버전 |
| `getNN()` | `NN` | Minor 버전 |
| `getPP()` | `PP` | Patch 버전 |
| `getRR()` | `RR` | Release 버전 |
| `setMM(v)` | `MM { set; }` | Major 버전 설정 |
| `setNN(v)` | `NN { set; }` | Minor 버전 설정 |
| `setPP(v)` | `PP { set; }` | Patch 버전 설정 |
| `setRR(v)` | `RR { set; }` | Release 버전 설정 |

---

## ?? DocInfo 매핑

| Java | C# | 설명 |
|------|-----|------|
| `DocInfo` | `DocInfo` | 문서 정보 클래스 |
| `getDocumentProperties()` | `DocumentProperties` | 문서 속성 |
| `getIDMappings()` | `IDMappings` | ID 매핑 |
| `getBinDataList()` | `BinDataList` | 바이너리 데이터 리스트 |
| `getFaceNameList()` | `FaceNameList` | 글꼴 리스트 |
| `getBorderFillList()` | `BorderFillList` | 테두리/채우기 리스트 |
| `getCharShapeList()` | `CharShapeList` | 글자 모양 리스트 |
| `getTabDefList()` | `TabDefList` | 탭 정의 리스트 |
| `getNumberingList()` | `NumberingList` | 번호 매기기 리스트 |
| `getBulletList()` | `BulletList` | 글머리표 리스트 |
| `getParaShapeList()` | `ParaShapeList` | 문단 모양 리스트 |
| `getStyleList()` | `StyleList` | 스타일 리스트 |
| `getCompatibleDocument()` | `CompatibleDocument` | 호환 문서 |
| `getLayoutCompatibility()` | `LayoutCompatibility` | 레이아웃 호환성 |

---

## ?? BodyText 매핑

| Java | C# | 설명 |
|------|-----|------|
| `BodyText` | `BodyText` | 본문 클래스 |
| `getSectionList()` | `SectionList` | 섹션 리스트 |
| `addNewSection()` | `AddNewSection()` | 새 섹션 추가 |
| `getMemoList()` | `MemoList` | 메모 리스트 |
| `addNewMemo()` | `AddNewMemo()` | 새 메모 추가 |

### Section

| Java | C# | 설명 |
|------|-----|------|
| `Section` | `Section` | 섹션 클래스 |
| `getParagraphList()` | `Paragraphs` | 문단 리스트 |
| `getParagraph(index)` | `Paragraphs[index]` | 문단 가져오기 |
| `countOfParagraph()` | `Paragraphs.Count` | 문단 개수 |

### Paragraph

| Java | C# | 설명 |
|------|-----|------|
| `Paragraph` | `Paragraph` | 문단 클래스 |
| `getHeader()` | `Header` | 문단 헤더 |
| `getText()` | `Text` | 문단 텍스트 |
| `getCharShape()` | `CharShape` | 글자 모양 |
| `getLineSeg()` | `LineSeg` | 줄 정보 |
| `getRangeTag()` | `RangeTag` | 범위 태그 |
| `getControlList()` | `ControlList` | 컨트롤 리스트 |
| `getMemoList()` | `MemoList` | 메모 리스트 |

---

## ??? Control 매핑

### 기본 Control

| Java | C# | 설명 |
|------|-----|------|
| `Control` | `Control` | 컨트롤 기본 클래스 |
| `getType()` | `Type` | 컨트롤 타입 |
| `getHeader()` | `Header` | 컨트롤 헤더 |

### ControlType

| Java | C# | 설명 |
|------|-----|------|
| `ControlType.SectionDefine` | `ControlType.SectionDefine` | 섹션 정의 |
| `ControlType.ColumnDefine` | `ControlType.ColumnDefine` | 다단 정의 |
| `ControlType.Table` | `ControlType.Table` | 표 |
| `ControlType.Equation` | `ControlType.Equation` | 수식 |
| `ControlType.Header` | `ControlType.Header` | 머리말 |
| `ControlType.Footer` | `ControlType.Footer` | 꼬리말 |
| `ControlType.Footnote` | `ControlType.Footnote` | 각주 |
| `ControlType.Endnote` | `ControlType.Endnote` | 미주 |
| `ControlType.AutoNumber` | `ControlType.AutoNumber` | 자동 번호 |
| `ControlType.NewNumber` | `ControlType.NewNumber` | 새 번호 |
| `ControlType.PageHide` | `ControlType.PageHide` | 페이지 숨기기 |
| `ControlType.PageNumberPosition` | `ControlType.PageNumberPosition` | 페이지 번호 위치 |
| `ControlType.IndexMark` | `ControlType.IndexMark` | 찾아보기 표시 |
| `ControlType.Bookmark` | `ControlType.Bookmark` | 책갈피 |
| `ControlType.OverlappingLetter` | `ControlType.OverlappingLetter` | 겹침 글자 |
| `ControlType.AdditionalText` | `ControlType.AdditionalText` | 덧말 |
| `ControlType.HiddenComment` | `ControlType.HiddenComment` | 숨은 설명 |
| `ControlType.Form` | `ControlType.Form` | 양식 |
| `ControlType.Field` | `ControlType.Field` | 필드 |
| `ControlType.Gso` | `ControlType.Gso` | 그리기 개체 |

### GSO Controls

| Java | C# | 설명 |
|------|-----|------|
| `ControlPicture` | `ControlPicture` | 그림 |
| `ControlLine` | `ControlLine` | 선 |
| `ControlRectangle` | `ControlRectangle` | 사각형 |
| `ControlEllipse` | `ControlEllipse` | 타원 |
| `ControlArc` | `ControlArc` | 호 |
| `ControlPolygon` | `ControlPolygon` | 다각형 |
| `ControlCurve` | `ControlCurve` | 곡선 |
| `ControlOLE` | `ControlOLE` | OLE 개체 |
| `ControlContainer` | `ControlContainer` | 묶음 개체 |
| `ControlTextArt` | `ControlTextArt` | 글맵시 |
| `ControlObjectLinkLine` | `ControlObjectLinkLine` | 연결선 |

### Table

| Java | C# | 설명 |
|------|-----|------|
| `ControlTable` | `ControlTable` | 표 컨트롤 |
| `getTable()` | `Table` | 표 객체 |
| `getRowList()` | `RowList` | 행 리스트 |
| `getCellList()` | `CellList` | 셀 리스트 |

---

## ?? Tool 매핑

### BlankFileMaker

| Java | C# | 설명 |
|------|-----|------|
| `BlankFileMaker.make()` | `BlankFileMaker.Make()` | 빈 파일 생성 |

### TextExtractor

| Java | C# | 설명 |
|------|-----|------|
| `TextExtractor.extract(hwpFile, tem)` | `TextExtractor.Extract(hwpFile, tem)` | 텍스트 추출 |
| `TextExtractMethod.InsertControlTextBetweenParagraphText` | `TextExtractMethod.InsertControlTextBetweenParagraphText` | 추출 방법 |
| `TextExtractMethod.AppendControlTextAfterParagraphText` | `TextExtractMethod.AppendControlTextAfterParagraphText` | 추출 방법 |

### ParagraphAdder

| Java | C# | 설명 |
|------|-----|------|
| `ParagraphAdder.add(target, source)` | `ParagraphAdder.Add(target, source)` | 문단 추가 |

### ObjectFinder

| Java | C# | 설명 |
|------|-----|------|
| `ControlFinder.find(hwpFile, filter)` | `ControlFinder.Find(hwpFile, filter)` | 컨트롤 찾기 |
| `FieldFinder.find(hwpFile, fieldName)` | `FieldFinder.Find(hwpFile, fieldName)` | 필드 찾기 |
| `CellFinder.find(hwpFile, row, col)` | `CellFinder.Find(hwpFile, row, col)` | 셀 찾기 |

---

## ?? CompoundFile 매핑

### CompoundFileReader

| Java | C# | 설명 |
|------|-----|------|
| `CompoundFileReader(file)` | `CompoundFileReader(stream)` | 생성자 |
| `close()` | `Dispose()` | 리소스 해제 |
| `getChildStreamReader(name, compressed, version)` | `GetChildStreamReader(name, compressed, version)` | 스트림 리더 가져오기 |
| `moveChildStorage(name)` | `MoveChildStorage(name)` | 하위 스토리지로 이동 |
| `moveParentStorage()` | `MoveParentStorage()` | 상위 스토리지로 이동 |
| `isChildStorage(name)` | `IsChildStorage(name)` | 하위 스토리지 존재 여부 |
| `listChildNames()` | `ListChildNames()` | 하위 항목 이름 목록 |

### StreamReader

| Java | C# | 설명 |
|------|-----|------|
| `StreamReader` | `CompoundStreamReader` | 스트림 리더 클래스 |
| `readBytes(buffer)` | `ReadBytes(count)` | 바이트 읽기 |
| `readUInt1()` | `ReadUInt8()` | 1바이트 부호없는 정수 |
| `readSInt1()` | `ReadInt8()` | 1바이트 부호있는 정수 |
| `readUInt2()` | `ReadUInt16()` | 2바이트 부호없는 정수 |
| `readSInt2()` | `ReadInt16()` | 2바이트 부호있는 정수 |
| `readUInt4()` | `ReadUInt32()` | 4바이트 부호없는 정수 |
| `readSInt4()` | `ReadInt32()` | 4바이트 부호있는 정수 |
| `readHWPString()` | `ReadHWPString()` | HWP 문자열 |
| `getSize()` | `Size` | 스트림 크기 |
| `isEndOfStream()` | `IsEndOfStream` | 스트림 끝 여부 |
| `readRecordHeader()` | `ReadRecordHeader()` | 레코드 헤더 읽기 |
| `getCurrentRecordHeader()` | `CurrentRecordHeader` | 현재 레코드 헤더 |

### CompoundFileWriter

| Java | C# | 설명 |
|------|-----|------|
| `CompoundFileWriter()` | `CompoundFileWriter()` | 생성자 |
| `close()` | `Dispose()` | 리소스 해제 |
| `openCurrentStorage(name)` | `CreateStorage(name)` | 스토리지 생성 |
| `openCurrentStream(name)` | `CreateStream(name)` | 스트림 생성 |
| `writeCurrentStream(data)` | `WriteStream(data)` | 스트림 쓰기 |
| `closeCurrentStream()` | `CloseStream()` | 스트림 닫기 |
| `write(filepath)` | `Save(filePath)` | 파일로 저장 |
| `write(os)` | `Save(stream)` | 스트림으로 저장 |

---

## ?? 타입 매핑

### 기본 타입

| Java | C# | 설명 |
|------|-----|------|
| `byte` | `sbyte` | 부호있는 1바이트 |
| `short` | `short` | 부호있는 2바이트 |
| `int` | `int` | 부호있는 4바이트 |
| `long` | `long` | 부호있는 8바이트 |
| `boolean` | `bool` | 불리언 |
| `String` | `string` | 문자열 |
| `byte[]` | `byte[]` | 바이트 배열 |

### 부호없는 정수 (HWP에서 자주 사용)

| Java | C# | 설명 |
|------|-----|------|
| `int` (unsigned로 사용) | `uint` | 부호없는 4바이트 |
| `short` (unsigned로 사용) | `ushort` | 부호없는 2바이트 |
| `byte` (unsigned로 사용) | `byte` | 부호없는 1바이트 |

### 컬렉션

| Java | C# | 설명 |
|------|-----|------|
| `ArrayList<T>` | `List<T>` | 리스트 |
| `Set<String>` | `ISet<string>` | 집합 |
| `Iterator<T>` | `IEnumerator<T>` | 반복자 |

---

## ?? Enum 매핑 예시

### BinDataCompress

| Java | C# |
|------|-----|
| `BinDataCompress.ByStorageDefault` | `BinDataCompress.ByStorageDefault` |
| `BinDataCompress.Compress` | `BinDataCompress.Compress` |
| `BinDataCompress.NoCompress` | `BinDataCompress.NoCompress` |

### ControlType

| Java | C# |
|------|-----|
| `ControlType.SectionDefine` | `ControlType.SectionDefine` |
| `ControlType.Table` | `ControlType.Table` |
| `ControlType.Gso` | `ControlType.Gso` |
| *(등등...)* | *(등등...)* |

---

## ?? 파일 구조 매핑

### Java 디렉토리 구조
```
hwplib/src/main/java/kr/dogfoot/hwplib/
├── object/          # 데이터 객체들
├── reader/          # 읽기 로직
├── writer/          # 쓰기 로직
├── tool/            # 유틸리티 도구들
├── util/            # 일반 유틸리티
└── org/apache/poi/  # POI 라이브러리 (Compound File)
```

### C# 디렉토리 구조
```
hwplibsharp/
├── Object/          # 데이터 객체들
├── Reader/          # 읽기 로직
├── Writer/          # 쓰기 로직
├── Tool/            # 유틸리티 도구들
├── Util/            # 일반 유틸리티
├── Binary/          # 바이너리 유틸리티
└── CompoundFile/    # OpenMcdf 기반 Compound File
```

---

## ?? 주요 변환 패턴

### 1. Getter/Setter → Property

**Java:**
```java
public FileHeader getFileHeader() { return fileHeader; }
public void setFileHeader(FileHeader fh) { this.fileHeader = fh; }
```

**C#:**
```csharp
public FileHeader FileHeader { get; set; }
// 또는 readonly property:
public FileHeader FileHeader => _fileHeader;
```

### 2. 예외 처리

**Java:**
```java
public static HWPFile fromFile(String filepath) throws Exception {
    // ...
}
```

**C#:**
```csharp
public static HWPFile FromFile(string filePath)
{
    // throws 선언 불필요, 필요시 try-catch 사용
}
```

### 3. 리소스 관리

**Java:**
```java
StreamReader sr = cfr.getChildStreamReader(name);
// ... use sr ...
sr.close();
```

**C#:**
```csharp
using var sr = _cfr.GetChildStreamReader(name);
// ... use sr, 자동 Dispose
```

### 4. 반복자 패턴

**Java:**
```java
Iterator<String> it = ss.iterator();
while (it.hasNext()) {
    String name = it.next();
}
```

**C#:**
```csharp
foreach (var name in names)
{
    // ...
}
```

### 5. 정적 메서드

**Java:**
```java
public static HWPFile fromFile(String filepath) throws Exception { }
```

**C#:**
```csharp
public static HWPFile FromFile(string filePath) { }
```

---

## ?? 미구현/차이점 목록

| 항목 | Java | C# | 상태 |
|------|------|-----|------|
| URL에서 읽기 | `fromURL(url)` | - | ? 미구현 |
| SummaryInformation | POI PropertySet | byte[] | ?? 단순화됨 |
| Compound File | 내장 POI | OpenMcdf | ? 외부 라이브러리 |
| 텍스트 추출 | `forExtractText()` | - | ?? 부분 구현 |
| GSO Reader | 완전 구현 | - | ? 미구현 |
| Table Reader | 완전 구현 | - | ? 미구현 |

---

## ?? 참조

- **원본 Java 라이브러리**: [neolord0/hwplib](https://github.com/neolord0/hwplib)
- **C# 포팅**: [rkttu/libhwpsharp](https://github.com/rkttu/libhwpsharp)
- **OpenMcdf 라이브러리**: [ironfede/openmcdf](https://github.com/ironfede/openmcdf)

---

## ?? 업데이트 이력

| 날짜 | 내용 |
|------|------|
| 2025-12-16 | 최초 문서 작성 |
