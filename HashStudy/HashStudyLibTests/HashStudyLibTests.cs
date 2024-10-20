using Microsoft.VisualStudio.TestTools.UnitTesting;
using HashStudyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Metadata;
using System.Diagnostics;
using System.Globalization;
using System.Security.Cryptography;

namespace HashStudyLib.Tests
{
    [TestClass()]
    public class HashStudyLibTests
    {
        [TestMethod()]
        public void processTest()
        {
            string logFilePath = @"C:\repos\eugene\c-sharp\HashStudy\logDataSample.log";
            DateTime startTime = new DateTime(2024, 10, 19, 0, 1, 0, 0); // 시작 시간
            DateTime endTime = new DateTime(2024, 10, 19, 15, 0, 0, 0);   // 종료 시간

            foreach (var logLine in ReadLogSection(logFilePath, startTime, endTime))
            {
                Debug.WriteLine(logLine);
            }
        }


        [TestMethod()]
        public void procTest2()
        {
            string logFilePath = @"C:\repos\eugene\c-sharp\HashStudy\logDataSample.log";
            int linesPerTimestamp = 3; // 각 시간대당 3줄씩만 읽기

            foreach (var logLine in ReadLogByTimeAndLimitLines(logFilePath, linesPerTimestamp))
            {
                Debug.WriteLine(logLine);
            }
        }


        public static IEnumerable<string> ReadLogSection(string filePath, DateTime startTime, DateTime endTime)
        {
            string timestampFormat = "yyyy-MM-dd HH:mm:ss.fff";

            foreach (var line in File.ReadLines(filePath))
            {
                //회사꺼는 23개 0, 23
                if (DateTime.TryParseExact(line.Substring(0, 23), timestampFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime logTime))
                {
                    if (logTime >= startTime && logTime <= endTime)
                    {
                        yield return line;
                    }
                }

            }
        }



        public static IEnumerable<string> ReadLogByTimeAndLimitLines(string filePath, int linesPerTimestamp)
        {
            string timestampFormat = "yyyy-MM-dd HH:mm:ss.fff"; // 타임스탬프 형식
            DateTime? lastHour = null; // 마지막으로 처리한 시간
            int lineCount = 0; // 시간별로 읽은 줄 수

            foreach (var line in File.ReadLines(filePath))
            {
                // 타임스탬프 추출
                if (DateTime.TryParseExact(line.Substring(0, 23), timestampFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime logTime))
                {
                    // 시간을 시간 단위로만 비교 (HH)
                    DateTime currentHour = new DateTime(logTime.Year, logTime.Month, logTime.Day, logTime.Hour, 0, 0);

                    // 시간이 바뀌면 lineCount 초기화
                    if (lastHour == null || currentHour != lastHour)
                    {
                        lastHour = currentHour;
                        lineCount = 0;
                    }

                    // 각 시간별로 3줄씩만 읽음
                    if (lineCount < linesPerTimestamp)
                    {
                        yield return line;
                        lineCount++;
                    }
                }
            }
        }


        [TestMethod()]
        public void procTest3()
        {
            string logFilePath = @"C:\repos\eugene\c-sharp\HashStudy\logDataSample.log";
            int linesPerTimestamp = 3; // 각 시간대당 3줄씩 처리

            foreach (var hash in ReadLog(logFilePath, linesPerTimestamp))
            {
                Debug.WriteLine(hash); // 각 3줄의 해시 출력
            }
        }


        public static IEnumerable<string> ReadLog(string filePath, int linesPerTimestamp)
        {
            string timestampFormat = "yyyy-MM-dd HH:mm:ss.fff"; // 타임스탬프 형식
            DateTime? lastHour = null; // 마지막으로 처리한 시간
            int lineCount = 0; // 시간별로 읽은 줄 수
            List<string> currentLines = new List<string>(); // 현재 시간대의 3줄 저장

            foreach (var line in File.ReadLines(filePath))
            {
                // 타임스탬프 추출
                if (DateTime.TryParseExact(line.Substring(0, 23), timestampFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime logTime))
                {
                    // 시간을 시간 단위로만 비교 (HH)
                    DateTime currentHour = new DateTime(logTime.Year, logTime.Month, logTime.Day, logTime.Hour, 0, 0);

                    // 시간이 바뀌면 해시 생성
                    if (lastHour == null || currentHour != lastHour)
                    {
                        if (currentLines.Count > 0)
                        {
                            yield return GenerateHash(currentLines); // 이전 시간대의 3줄 해시 반환
                        }

                        // 새로운 시간대 시작
                        lastHour = currentHour;
                        lineCount = 0;
                        currentLines.Clear();
                    }

                    // 각 시간대별로 3줄씩 읽어서 리스트에 저장
                    if (lineCount < linesPerTimestamp)
                    {
                        currentLines.Add(line);
                        lineCount++;
                    }
                }
            }

            // 마지막 남은 시간대의 3줄 해시 반환
            if (currentLines.Count > 0)
            {
                yield return GenerateHash(currentLines);
            }
        }

        // 3줄의 로그를 결합하고 SHA256 해시 생성
        private static string GenerateHash(List<string> lines)
        {
            string combinedLines = string.Join("\n", lines); // 3줄을 하나의 문자열로 결합
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combinedLines));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower(); // 해시를 16진수 문자열로 반환
            }
        }




    }
}