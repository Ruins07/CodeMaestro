using System.Text;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
internal class Sync {
    public string ProjectPath;
    public string RefList;
    public Sync(string projectPath) {
        ProjectPath = projectPath;
    }

    public string ListAllFiles(string relativePath, int offset = 0) {
        // Формирование полного пути с использованием Path.Combine
        string fullPath = Path.Combine(ProjectPath, relativePath);
        if (!Directory.Exists(fullPath))
            return $"Directory '{fullPath}' does not exist.";

        StringBuilder builder = new StringBuilder();
        string offsetStr = new ('\t', offset);

        // Обходим файлы в текущей директории
        foreach (var file in Directory.GetFiles(fullPath)) {
            string relativeFilePath = Path.Combine(relativePath, Path.GetFileName(file));
            builder.AppendLine($"{offsetStr}{relativeFilePath}");
        }

        // Рекурсивно обходим директории
        foreach (var dir in Directory.GetDirectories(fullPath)) {
            string relativeDirPath = Path.Combine(relativePath, Path.GetFileName(dir));
            builder.AppendLine($"{offsetStr}{relativeDirPath}/");
            builder.Append(ListAllFiles(relativeDirPath, offset + 1));
        }
        return builder.ToString();
    }

    public struct FileRequest {
        public string Name; // Относительный путь к файлу
        public Range[] Views; // Диапазоны для чтения (например, части файла)
    }
    public struct FileWriteRequest {
        public struct WriteRangeData {
            public int Offset;
            public string Data;
        }
        public string Name;
        public WriteRangeData[] Ranges;

        public static void OverWrite(Span<Range> Src) {
            var Accumulator = 0;
            for(int I = 0; I < Src.Length; I++) {
                var CurrentRange = Src[I];
                Src[I] = new Range(CurrentRange.Start.Value + Accumulator, CurrentRange.End.Value);
                Accumulator += CurrentRange.End.Value - CurrentRange.Start.Value;
            }
        }
    }
    public string ReadFile(FileRequest request) {
        string fullFilePath = Path.Combine(ProjectPath, request.Name);
        if (!File.Exists(fullFilePath))
            return $"File '{fullFilePath}' does not exist.";

        var fileData = File.ReadAllText(fullFilePath);
        var views = new string[request.Views.Length];
        for (int i = 0; i < request.Views.Length; i++) {
            var currentView = request.Views[i];
            int start = currentView.Start.Value;
            int end = currentView.End.Value;
            int length = end - start;
            if (start < 0 || end > fileData.Length || length < 0) {
                views[i] = $"Invalid range {start}-{end} for file '{request.Name}'.";
            }
            else {
                string splitter =
                    "=============================== " +
                    $"Region [{start}:{length}:{end}]" +
                    " ===============================";
                views[i] = splitter + Environment.NewLine +
                           fileData.Substring(start, length);
            }
        }
        string fileHeader = $"=========== File: {request.Name} ===========";
        return fileHeader + Environment.NewLine + string.Join(Environment.NewLine, views);
    }

    public string ReadFiles(FileRequest[] requests) {
        StringBuilder builder = new StringBuilder();
        foreach (var req in requests) {
            builder.AppendLine(ReadFile(req));
        }
        return builder.ToString();
    }

    public string WriteFile(string request, string content) {
        string fullFilePath = Path.Combine(ProjectPath, request);
        // Создаем директорию, если ее нет
        Directory.CreateDirectory(Path.GetDirectoryName(fullFilePath));
        File.WriteAllText(fullFilePath, content);
        return $"File '{ request }' written successfully.";
    }
    public void WriteFile(FileWriteRequest Request) {
        var FullPath = Path.Combine(ProjectPath, Request.Name);
        var Text = File.ReadAllText(FullPath);
        foreach (var CurrentRange in Request.Ranges)
            Text = Text.Insert(CurrentRange.Offset, CurrentRange.Data);
        File.WriteAllText(Request.Name, Text);
    }
    public async Task<string> RunCodeAsync(string code) {
        try {
            var result = await CSharpScript.EvaluateAsync(code, ScriptOptions.Default);
            return result?.ToString() ?? "null";
        }
        catch (Exception ex) {
            return $"Error executing code: {ex.Message}";
        }
    }
}