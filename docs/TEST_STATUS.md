# 테스트 현황 문서

> 마지막 업데이트: 2025년 12월 16일

## 📊 테스트 결과 요약

| 항목 | 개수 | 비율 |
|------|------|------|
| **총 테스트** | 148개 | 100% |
| **성공** | 70개 | 47.3% |
| **실패** | 75개 | 50.7% |
| **건너뜀** | 3개 | 2.0% |

---

## 🔴 실패 원인 분류

### 1. HWP 파일 읽기 실패 - `ReadBasicFile_ShouldSucceed`

**테스트 파일**: `ReadingHwpFromFileTest.cs`

**증상**:
- `Assert.IsTrue` 실패: `hwpFile.BodyText.SectionList.Count > 0` 조건 불충족
- 디버그 메시지: `Unexpected minor version: 59`

**영향받는 HWP 파일들**:
- 책갈피.hwp
- 이미지추가.hwp
- 페이지숨김.hwp
- 표.hwp
- etc.hwp
- 숨은설명.hwp
- 선-사각형-타원.hwp
- 필드.hwp
- 수식.hwp
- 필드-누름틀.hwp
- 호-곡선.hwp

**근본 원인**:
- HWP 파일의 minor version 59가 현재 라이브러리에서 완전히 지원되지 않음
- BodyText Section 파싱 로직에서 새 버전 형식 처리 누락

---

### 2. HWP 파일 다시 쓰기 실패 - `RewriteFile_ShouldSucceed`

**테스트 파일**: `RewritingHwpFileTest.cs`

**증상 A - FAT Sector ID 오류**:
```
OpenMcdf.FileFormatException: Invalid FAT sector ID: 4294967295
```

**발생 위치**: 
- `src/hwplibsharp/CompoundFile/Wrappers.cs` Line 54 - `IsStream()` 메서드

**영향받는 파일들** (대다수):
- 각주미주.hwp, etc.hwp, 글자겹침.hwp, blank.hwp, 그림.hwp
- 글상자.hwp, 문단번호 1-10 수준.hwp, 바탕쪽.hwp, 새번호지정.hwp
- 선-사각형-타원.hwp, 묶음.hwp, 수식.hwp, 차트.hwp, 이미지추가.hwp
- 머리글꼬리글.hwp, 글상자-압축.hwp, 숨은설명.hwp, 필드-누름틀.hwp
- 필드.hwp, 호-곡선.hwp, 페이지숨김.hwp, 책갈피.hwp, 표.hwp
- 다각형.hwp, 덧말.hwp

**증상 B - Mini FAT 오류**:
```
OpenMcdf.FileFormatException: Mini FAT index not found: 0
```

**발생 위치**:
- `src/hwplibsharp/CompoundFile/Wrappers.cs` Line 129 - `GetData()` 메서드

**영향받는 파일들**:
- ole.hwp
- 구버전(5.0.2.2) Picture 컨트롤.hwp

**근본 원인**:
- HWP 파일 Writer가 생성한 result 파일들의 Compound File 구조가 손상됨
- OpenMcdf 라이브러리로 다시 읽을 때 FAT 테이블 불일치 발생

---

### 3. 단순 편집 실패 - `SimpleEdit_ShouldSucceed`

**테스트 파일**: `SimpleEditingHwpFileTest.cs`

**증상**: 위 2번과 동일한 FAT Sector ID 오류

---

## 🔧 수정 필요 영역

### 우선순위 1: Compound File Writer 수정
- **파일**: `src/hwplibsharp/CompoundFile/` 디렉토리
- **작업**: HWP 파일 쓰기 시 FAT 테이블 구조가 올바르게 생성되도록 수정
- **참고**: OpenMcdf 라이브러리 사용법 재검토 필요

### 우선순위 2: HWP 버전 호환성 확장
- **파일**: `src/hwplibsharp/Reader/` 디렉토리
- **작업**: minor version 59 이상의 HWP 파일 형식 지원
- **참고**: Java 버전 hwplib의 최신 구현 참조

### 우선순위 3: BodyText Section 파싱 로직
- **파일**: Body Text 관련 Reader 클래스들
- **작업**: 새 버전에서 Section 데이터가 올바르게 파싱되도록 수정

---

## 📁 관련 파일 목록

### 테스트 파일
- `src/hwplibsharp.test/ReadingHwpFromFileTest.cs`
- `src/hwplibsharp.test/RewritingHwpFileTest.cs`
- `src/hwplibsharp.test/SimpleEditingHwpFileTest.cs`

### 핵심 구현 파일
- `src/hwplibsharp/CompoundFile/Wrappers.cs`
- `src/hwplibsharp/CompoundFile/CompoundFileReader.cs`
- `src/hwplibsharp/CompoundFile/CompoundStreamReader.cs`
- `src/hwplibsharp/Reader/HWPReader.cs`

### 테스트 데이터
- `sample_hwp/basic/` - 원본 HWP 파일들
- `sample_hwp/result/` - 쓰기 테스트 결과 파일들

---

## 🔗 참조 자료

- **원본 Java 라이브러리**: [neolord0/hwplib](https://github.com/neolord0/hwplib)
- **OpenMcdf 라이브러리**: [ironfede/openmcdf](https://github.com/ironfede/openmcdf)
- **HWP 파일 형식 문서**: 한글과컴퓨터 공식 문서

---

## ✅ 성공하는 테스트 (참고용)

현재 70개의 테스트가 성공하고 있으며, 주로 다음 기능들이 작동합니다:
- 일부 HWP 파일 읽기
- 기본적인 파일 헤더 파싱
- 일부 메타데이터 추출

---

## 📝 변경 이력

| 날짜 | 변경 내용 |
|------|-----------|
| 2025-12-16 | 최초 문서 작성, 테스트 현황 분석 |
