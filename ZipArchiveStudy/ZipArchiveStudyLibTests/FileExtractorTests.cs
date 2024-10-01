using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZipArchiveStudyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Globalization;

namespace ZipArchiveStudyLib.Tests
{
    [TestClass()]
    public class FileExtractorTests
    {
        /*[TestMethod()]
        public void FindFilesInZipTest()
        {
            FileExtractor fileExtractor = new FileExtractor();
            var path = @"D:\sampleZipFile.zip";
            fileExtractor.FindFilesInZip(path);
            System.Diagnostics.Debug.WriteLine(path);
        }*/

        [TestMethod()]
        public void ZipFileTest()
        {
            FileExtractor fileExtractor = new FileExtractor();

            var zipPath = @"D:\sampleZipFile.zip";
            var pattern = @"\.log$";


            void ProcessZipEntry(ZipArchive archive, string currentPath)
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    string fullPath = Path.Combine(currentPath, entry.FullName);

                    if (Regex.IsMatch(entry.FullName, pattern))
                    {
                        fileExtractor.ExtractedFilePaths.Add(fullPath);
                        Debug.WriteLine($"Found file: {entry.FullName} fullPath: {fullPath}");
                    }
                    else if (entry.FullName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                    {
                        using (var innerStream = entry.Open())
                        using (var innerArchive = new ZipArchive(innerStream, ZipArchiveMode.Read))
                        {
                            ProcessZipEntry(innerArchive, fullPath);
                        }
                    }
                }
            }
            using (var fileStream = new FileStream(zipPath, FileMode.Open, FileAccess.Read))
            using (var archive = new ZipArchive(fileStream, ZipArchiveMode.Read))
            {
                ProcessZipEntry(archive, "");  // 초기 호출
            }
        }

        [TestMethod()]
        public void ExtractAndPrepareFilesTest()
        {
            FileExtractor fileExtractor = new FileExtractor();

            var zipPath = @"D:\sampleZipFile.zip";

            using (ZipArchive archive = ZipFile.OpenRead(zipPath))
            {
                foreach (var filePath in fileExtractor.ExtractedFilePaths)
                {
                    if (string.IsNullOrEmpty(filePath))
                    {
                        continue;
                    }
                    var entry = archive.GetEntry(filePath);
                    if (entry != null)
                    {
                        // 임시 디렉토리에 파일을 추출
                        string destinationPath = Path.Combine(Path.GetTempPath(), entry.FullName);

                        // 디렉토리가 존재하지 않으면 생성
                        string directoryPath = Path.GetDirectoryName(destinationPath);
                        if (!Directory.Exists(directoryPath))
                        {
                            Directory.CreateDirectory(directoryPath);
                        }

                        try
                        {
                            // 파일을 지정된 경로에 추출, 파일이 이미 존재하면 덮어쓰기
                            entry.ExtractToFile(destinationPath, overwrite: true);
                            Debug.WriteLine($"Extracted to: {destinationPath}");
                        }
                        catch (IOException ex)
                        {
                            // 파일 추출 중 에러 처리, 필요한 에러 로깅 또는 사용자 통지 수행
                            Debug.WriteLine($"Error extracting file '{entry.FullName}': {ex.Message}");
                        }

                        // 추출된 파일을 사용하여 추가 작업 수행, 예: 파일 업로드
                        // UploadFile(destinationPath);  // 예시로 업로드 함수 호출
                    }
                    else
                    {
                        // 파일 엔트리를 찾을 수 없음
                        Debug.WriteLine($"File not found in archive: {filePath}");
                    }
                }
            }

        }
    }
}