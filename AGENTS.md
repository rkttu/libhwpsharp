# AGENTS.md - 코딩 에이전트 가이드

> 이 문서는 AI 코딩 에이전트가 이 프로젝트에서 작업할 때 참고해야 하는 핵심 정보를 담고 있습니다.

## 📌 프로젝트 개요

**libhwpsharp**는 한글(HWP) 문서 파일을 읽고 쓰기 위한 .NET 라이브러리입니다.
Java로 작성된 [neolord0/hwplib](https://github.com/neolord0/hwplib)를 C#으로 포팅한 프로젝트입니다.

---

## 📚 필수 참고 문서

작업 전 반드시 다음 문서들을 확인하세요:

| 문서 | 경로 | 설명 |
|------|------|------|
| **테스트 현황** | [docs/TEST_STATUS.md](docs/TEST_STATUS.md) | 현재 테스트 통과/실패 현황 및 알려진 이슈 |
| **README** | [README.md](README.md) | 프로젝트 소개 및 사용법 |
| **Java 원본** | [hwplib/](hwplib/) | 원본 Java 라이브러리 (서브모듈) |

---

## 🏗️ 프로젝트 구조

```
libhwpsharp/
├── src/
│   ├── hwplibsharp/           # 메인 라이브러리
│   │   ├── CompoundFile/      # HWP 컴파운드 파일 처리
│   │   ├── Reader/            # HWP 파일 읽기
│   │   ├── Writer/            # HWP 파일 쓰기
│   │   └── ...
│   └── hwplibsharp.test/      # 테스트 프로젝트
├── sample_hwp/                # 테스트용 HWP 파일들
│   ├── basic/                 # 원본 테스트 파일
│   └── result/                # 쓰기 테스트 결과
├── hwplib/                    # Java 원본 (서브모듈)
└── docs/                      # 문서
```

---

## 🎯 현재 작업 목표

### 최우선 과제: 테스트 통과율 향상

현재 상태: **148개 테스트 중 70개 성공 (47.3%)**

주요 수정 영역:
1. **Compound File Writer 수정** - FAT 테이블 구조 오류 해결
2. **HWP 버전 호환성** - minor version 59+ 지원
3. **BodyText Section 파싱** - 새 버전 형식 처리

자세한 내용은 [docs/TEST_STATUS.md](docs/TEST_STATUS.md) 참조

---

## 🔧 개발 환경

- **.NET 버전**: .NET 8.0+
- **테스트 프레임워크**: MSTest
- **주요 의존성**: OpenMcdf (Compound File 처리)

### 빌드 및 테스트 명령어

```bash
# 빌드
cd src && dotnet build

# 테스트 실행
cd src && dotnet test

# 특정 테스트만 실행
cd src && dotnet test --filter "FullyQualifiedName~TestMethodName"
```

---

## 📋 코딩 규칙

1. **Java 원본 참조**: 기능 구현 시 `hwplib/` 디렉토리의 Java 코드 참조
2. **네이밍**: C# 컨벤션 사용 (PascalCase for public, camelCase for private)
3. **테스트 작성**: 새 기능 추가 시 반드시 테스트 코드 포함
4. **문서 업데이트**: 주요 변경 시 관련 문서 업데이트

---

## ⚠️ 알려진 이슈

### 1. OpenMcdf FAT 오류
- **증상**: `Invalid FAT sector ID: 4294967295`
- **위치**: `CompoundFile/Wrappers.cs`
- **상태**: 해결 필요

### 2. HWP 버전 호환성
- **증상**: `Unexpected minor version: 59`
- **영향**: 최신 HWP 파일 일부 기능 미지원
- **상태**: 해결 필요

### 3. result 폴더 파일 손상
- **원인**: Writer가 생성한 파일 구조 오류
- **영향**: RewriteFile 테스트 실패
- **상태**: 해결 필요

---

## 🔗 참조 링크

- [한글과컴퓨터 HWP 파일 형식](https://www.hancom.com/)
- [OpenMcdf GitHub](https://github.com/ironfede/openmcdf)
- [원본 hwplib (Java)](https://github.com/neolord0/hwplib)

---

## 📝 문서 업데이트 규칙

테스트 현황이 변경될 때마다 다음을 수행하세요:

1. `dotnet test` 실행 후 결과 확인
2. [docs/TEST_STATUS.md](docs/TEST_STATUS.md)의 테스트 결과 요약 업데이트
3. 새로운 이슈 발견 시 해당 문서에 추가
4. 이슈 해결 시 해당 항목 업데이트 또는 제거
