using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ZipArchiveStudyLib;

namespace ZipArchiveStudy
{
    public class MainViewModel
    {
        private readonly FileExtractor fileExtractor = new FileExtractor();
        public ObservableCollection<string> Files { get; private set; } = new ObservableCollection<string>();
        public ICommand ShowFilesCommand { get; }
        public ICommand FileDropCommand { get; }
        public ICommand FindFilesCommand { get; }
        public MainViewModel()
        {
            ShowFilesCommand = new RelayCommand(OpenFilesWindow);
            FindFilesCommand = new RelayCommand(() => HandleFileDrop());
        }

        private void HandleFileDrop()
        {
            // 예시 경로, 실제 환경에서는 동적으로 설정 가능
            string zipPath = "path/to/your/zipfile.zip";
            fileExtractor.FindFilesInZip(zipPath);

            // UI 리스트를 업데이트하기 위해 현재 리스트를 클리어하고 새 경로들을 추가
            Files.Clear();
            foreach (var file in fileExtractor.ExtractedFilePaths)
            {
                Files.Add(file);
            }
        }

        private void OpenFilesWindow()
        {
            FilesWindow filesWindow = new FilesWindow(Files);
            filesWindow.Show();
        }
    }
}
