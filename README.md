# HwpLibSharp

[![NuGet](https://img.shields.io/nuget/v/HwpLibSharp.svg)](https://www.nuget.org/packages/HwpLibSharp/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/HwpLibSharp.svg)](https://www.nuget.org/packages/HwpLibSharp/)
[![CI](https://github.com/rkttu/libhwpsharp/actions/workflows/ci.yml/badge.svg)](https://github.com/rkttu/libhwpsharp/actions/workflows/ci.yml)
[![Release to NuGet](https://github.com/rkttu/libhwpsharp/actions/workflows/release.yml/badge.svg)](https://github.com/rkttu/libhwpsharp/actions/workflows/release.yml)

.NET용 HWP(한글) 파일 라이브러리

> ⚠️ **주의**: 이 프로젝트는 **실험적(Experimental)** 프로젝트입니다. 지속적으로 업데이트될 예정이며, API가 예고 없이 변경될 수 있습니다. 프로덕션 환경에서 사용 시 발생하는 문제에 대해서는 책임을 지지 않습니다. 이 소프트웨어는 **있는 그대로(AS-IS)** 제공됩니다.

## 소개

이 프로젝트는 [neolord0/hwplib](https://github.com/neolord0/hwplib)의 Java 코드를 .NET으로 포팅한 것입니다. AI 코딩 어시스턴트를 활용하여 [@rkttu](https://github.com/rkttu)가 .NET 개발자들을 위해 포팅 작업을 진행하였습니다.

## 기술적 특징

- **.NET 8.0 타겟**: 최신 .NET 8.0을 대상으로 빌드되었습니다.
- **OpenMcdf 사용**: 원본 Java 프로젝트에서 사용하던 Apache POI 대신, .NET 네이티브 라이브러리인 [OpenMcdf](https://github.com/ironfede/openmcdf)를 사용하여 OLE 복합 문서를 처리합니다.
- **AOT 빌드 지원**: Native AOT(Ahead-of-Time) 컴파일을 지원하도록 설계되어, 더 빠른 시작 시간과 낮은 메모리 사용량을 제공합니다.

## 원본 프로젝트

- **원본 리포지터리**: [neolord0/hwplib](https://github.com/neolord0/hwplib)
- **원작자**: [@neolord0](https://github.com/neolord0)
- **원본 언어**: Java

## 프로젝트 거버넌스

이 프로젝트의 의사결정 및 판단 우선권은 원본 프로젝트의 작성자인 **[@neolord0](https://github.com/neolord0)** 님의 의사를 우선으로 합니다.

- 기능 추가, 버그 수정, API 설계 등 주요 결정 사항에 대해서는 원본 프로젝트의 방향성을 따릅니다.
- 원본 프로젝트와의 호환성 및 일관성을 유지하는 것을 목표로 합니다.
- .NET 특화 기능이나 개선 사항은 원본 프로젝트의 철학을 존중하는 범위 내에서 추가됩니다.

## 코드 사용법

### 다양한 불러오기 방법

다양한 소스로부터 HWP 파일을 불러올 수 있습니다.

```csharp
// 파일에서 읽기
var hwpFile = HWPReader.FromFile("document.hwp");

// URL에서 읽기
var hwpFile = HWPReader.FromUrl("https://example.com/document.hwp");

// URL에서 비동기로 읽기
var hwpFile = await HWPReader.FromUrlAsync("https://example.com/document.hwp");

// Stream에서 읽기
using var stream = new FileStream("document.hwp", FileMode.Open);
var hwpFile = HWPReader.FromStream(stream);

// Base64 문자열에서 읽기
var hwpFile = HWPReader.FromBase64String(base64EncodedHwp);
```

### 다양한 저장 방법

다양한 대상으로 HWP 파일을 저장할 수 있습니다.

```csharp
// 파일로 저장
HWPWriter.ToFile(hwpFile, "output.hwp");

// Stream으로 저장
using var outputStream = new MemoryStream();
HWPWriter.ToStream(hwpFile, outputStream);
```

### 비어있는 HWP 파일 생성

비어있는 HWP 파일을 생성할 수 있습니다.

```csharp
using HwpLib.Tool.BlankFileMaker;

var hwpFile = BlankFileMaker.Make();
HWPWriter.ToFile(hwpFile, "blank.hwp");
```

### RAG를 위한 텍스트 추출

다음과 같이 HWP 파일에서 RAG를 위한 용도로 텍스트를 추출할 수 있습니다.

```csharp
var url = "https://raw.githubusercontent.com/rkttu/libhwpsharp/refs/heads/main/sample_hwp/source.hwp";
var hwpFile = HWPReader.FromUrl(url);

var option = new TextExtractOption();
option.SetMethod(TextExtractMethod.InsertControlTextBetweenParagraphText);
option.SetWithControlChar(false);
option.SetAppendEndingLF(true);

var extractedText = TextExtractor.Extract(hwpFile, option);
Console.WriteLine(extractedText);
```

### 문단별 텍스트 추출

다음과 같이 HWP 파일에서 문단별로 텍스트를 추출할 수 있습니다.

```csharp
foreach (var section in hwpFile.BodyText.SectionList)
{
    for (int i = 0; i < section.ParagraphCount; i++)
    {
        var paragraph = section.GetParagraph(i);
        var text = paragraph.GetNormalString();
        Console.WriteLine(text);
    }
}
```

### 표 찾기

다음과 같이 HWP 파일에서 표를 찾을 수 있습니다.

```csharp
using HwpLib.Tool.ObjectFinder;
using HwpLib.Object.BodyText.Control;

// 커스텀 필터 정의
class TableFilter : IControlFilter
{
    public bool IsMatched(Control control, Paragraph paragraph, Section section)
    {
        return control.Type == ControlType.Table;
    }
}

var tables = ControlFinder.Find(hwpFile, new TableFilter());
Console.WriteLine($"발견된 표 개수: {tables.Count}");
```

### 셀 내용 읽기

다음과 같이 HWP 파일에서 표의 셀 내용을 읽을 수 있습니다.

```csharp
using HwpLib.Object.BodyText.Control.Table;

// 표 컨트롤을 찾은 후
var table = (ControlTable)control;

foreach (var row in table.RowList)
{
    foreach (var cell in row.CellList)
    {
        var cellText = cell.ParagraphList.GetNormalString();
        Console.Write($"[{cellText}] ");
    }
    Console.WriteLine();
}
```

### 필드 및 누름틀 찾기

다음과 같이 HWP 파일에서 필드 및 누름틀을 찾을 수 있습니다.

```csharp
using HwpLib.Tool.ObjectFinder;

// 필드 텍스트 읽기
var fieldTexts = FieldFinder.GetAllClickHereText(
    hwpFile, 
    "필드명", 
    TextExtractMethod.OnlyMainParagraph);

// 필드 텍스트 쓰기
var textList = new List<string> { "값1", "값2", "값3" };
var result = FieldFinder.SetFieldText(
    hwpFile, 
    ControlType.FIELD_CLICKHERE, 
    "필드명", 
    textList);
```

## 기여자

- **포팅 작업**: [@rkttu](https://github.com/rkttu)
- **원본 개발**: [@neolord0](https://github.com/neolord0)

## 라이선스

이 프로젝트는 원본 프로젝트 [neolord0/hwplib](https://github.com/neolord0/hwplib)와 동일하게 **Apache License 2.0**을 따릅니다.

자세한 내용은 [LICENSE](LICENSE) 파일을 참조하세요.

## 감사의 말

이 프로젝트가 가능하도록 훌륭한 hwplib 라이브러리를 만들어주신 [@neolord0](https://github.com/neolord0) 님께 깊은 감사를 드립니다.
