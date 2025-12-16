using HwpLib.CompoundFile;
using HwpLib.Object.BodyText.Paragraph.Text;

namespace HwpLib.Reader.BodyText.Paragraph;

/// <summary>
/// 문단 텍스트 레코드를 읽기 위한 객체
/// </summary>
public static class ForParaText
{
    /// <summary>
    /// 문단 텍스트 레코드를 읽는다.
    /// </summary>
    /// <param name="p">문단</param>
    /// <param name="sr">스트림 리더</param>
    public static void Read(Object.BodyText.Paragraph.Paragraph p, CompoundStreamReader sr)
    {
        if (p.Text == null)
        {
            p.CreateText();
        }

        var pt = p.Text!;

        // 레코드 크기 / 2 = 문자 수 (UTF-16LE)
        long charCount = sr.CurrentRecordHeader!.Size / 2;

        for (long i = 0; i < charCount; i++)
        {
            ushort code = sr.ReadUInt2();

            if (code <= 0x0020)
            {
                // 제어 문자
                switch (code)
                {
                    case 0x0000: // 사용 안 함
                        break;
                    case 0x0001: // 예약 (문자 컨트롤)
                    case 0x0002: // 구역/단 정의 (확장 컨트롤)
                    case 0x0003: // 필드 시작 (확장 컨트롤)
                        {
                            var extendChar = pt.AddNewExtendControlChar();
                            extendChar.Code = (short)code;
                            byte[] addition = sr.ReadBytes(12);
                            extendChar.SetAddition(addition);
                            sr.ReadUInt2(); // 종료 코드 읽기
                            i += 7; // 추가 12바이트 = 6 문자 + 1 문자 (코드 자체) - 이미 1 더해짐
                        }
                        break;
                    case 0x0004: // 필드 끝 (인라인 컨트롤)
                        {
                            var inlineChar = pt.AddNewInlineControlChar();
                            inlineChar.Code = (short)code;
                            byte[] addition = sr.ReadBytes(12);
                            inlineChar.SetAddition(addition);
                            sr.ReadUInt2(); // 종료 코드 읽기
                            i += 7;
                        }
                        break;
                    case 0x0005: // 예약
                    case 0x0006: // 예약
                    case 0x0007: // 예약
                    case 0x0008: // 제목 차례 (문자 컨트롤)
                    case 0x0009: // 탭 (문자 컨트롤)
                    case 0x000A: // 줄 바꿈 (문자 컨트롤)
                        {
                            var controlChar = pt.AddNewCharControlChar();
                            controlChar.Code = (short)code;
                        }
                        break;
                    case 0x000B: // 그리기 개체/표 (확장 컨트롤)
                        {
                            var extendChar = pt.AddNewExtendControlChar();
                            extendChar.Code = (short)code;
                            byte[] addition = sr.ReadBytes(12);
                            extendChar.SetAddition(addition);
                            sr.ReadUInt2(); // 종료 코드 읽기
                            i += 7;
                        }
                        break;
                    case 0x000C: // 예약
                        break;
                    case 0x000D: // 문단 끝 (문자 컨트롤)
                        {
                            var normalChar = pt.AddNewNormalChar();
                            normalChar.Code = (short)code;
                        }
                        break;
                    case 0x000E: // 예약
                    case 0x000F: // 하이픈 (문자 컨트롤)
                        {
                            var controlChar = pt.AddNewCharControlChar();
                            controlChar.Code = (short)code;
                        }
                        break;
                    case 0x0010: // 머리말/꼬리말/각주/미주 등 (확장 컨트롤)
                    case 0x0011: // 자동 번호 (확장 컨트롤)
                        {
                            var extendChar = pt.AddNewExtendControlChar();
                            extendChar.Code = (short)code;
                            byte[] addition = sr.ReadBytes(12);
                            extendChar.SetAddition(addition);
                            sr.ReadUInt2(); // 종료 코드 읽기
                            i += 7;
                        }
                        break;
                    case 0x0012: // 예약
                    case 0x0013: // 예약
                    case 0x0014: // 예약
                    case 0x0015: // 숨은 설명 (확장 컨트롤)
                        {
                            var extendChar = pt.AddNewExtendControlChar();
                            extendChar.Code = (short)code;
                            byte[] addition = sr.ReadBytes(12);
                            extendChar.SetAddition(addition);
                            sr.ReadUInt2(); // 종료 코드 읽기
                            i += 7;
                        }
                        break;
                    case 0x0016: // 예약
                    case 0x0017: // 예약
                    case 0x0018: // 글자 겹침 (인라인 컨트롤)
                        {
                            var inlineChar = pt.AddNewInlineControlChar();
                            inlineChar.Code = (short)code;
                            byte[] addition = sr.ReadBytes(12);
                            inlineChar.SetAddition(addition);
                            sr.ReadUInt2(); // 종료 코드 읽기
                            i += 7;
                        }
                        break;
                    case 0x0019: // 예약
                    case 0x001A: // 예약
                    case 0x001B: // 예약
                    case 0x001C: // 예약
                    case 0x001D: // 예약
                    case 0x001E: // 연결/보조 문자 위치 (문자 컨트롤)
                    case 0x001F: // 하이픈 (문자 컨트롤)
                    case 0x0020: // 공백 (일반 문자)
                        {
                            var normalChar = pt.AddNewNormalChar();
                            normalChar.Code = (short)code;
                        }
                        break;
                    default:
                        {
                            var normalChar = pt.AddNewNormalChar();
                            normalChar.Code = (short)code;
                        }
                        break;
                }
            }
            else
            {
                // 일반 문자
                var normalChar = pt.AddNewNormalChar();
                normalChar.Code = (short)code;
            }
        }
    }
}
