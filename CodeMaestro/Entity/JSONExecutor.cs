using CodeMaestro;
using System.Text;
using System.Text.RegularExpressions;

internal static class JsonCommandExecutor {
    /// <summary>
    /// Асинхронно выполняет список JSON-команд с использованием заданного экземпляра Sync.
    /// </summary>
    /// <param name="commands">Список JSON-команд.</param>
    /// <param name="sync">Экземпляр класса Sync для работы с проектом.</param>
    /// <returns>Строка с результатами выполнения команд.</returns>
    public static async Task<string> ExecuteJsonCommandsAsync(List<JsonCommand> commands, Sync sync, Project project) {
        StringBuilder output = new StringBuilder();
        
        foreach (var jsonCommand in commands) {

            switch (jsonCommand) {
                case ListCommand: {
                        var Cmd = (ListCommand)jsonCommand;
                        // Ожидается: [<relativePath>]

                        string listPath = Cmd.Arguments.RelativePath;
                        string result = sync.ListAllFiles(listPath);
                        output.AppendLine(result);

                        break;
                    }
                case ReadCommand: {
                        // Ожидается: [<filepath>, <start>, <end>]

                        var Cmd = (ReadCommand)jsonCommand;
                        var fileReq = new Sync.FileRequest {
                            Name = Cmd.Arguments.FilePath,
                            Views = [new Range(Cmd.Arguments.Start, Cmd.Arguments.End)]
                        };
                        string result = sync.ReadFile(fileReq);
                        output.AppendLine(result);

                        break;

                    }
                case WriteCommand: {
                        // Ожидается: [<filepath>, <text to write>]

                        var Cmd = (WriteCommand)jsonCommand;
                        string filePath = Cmd.Arguments.FilePath;
                        // Объединяем оставшиеся аргументы и преобразуем escape-последовательности
                        string rawText = string.Join(" ", Cmd.Arguments.Content.Skip(1));
                        string text = Regex.Unescape(rawText);
                        string result = sync.WriteFile(filePath, text);
                        output.AppendLine(result);
                        break;
                    }
                case RunCodeCommand: {
                        // Ожидается: [<C# code>]
                        var Cmd = (RunCodeCommand)jsonCommand;
                        string code = string.Join(" ", Cmd.Arguments.Code);
                        string result = await sync.RunCodeAsync(code);
                        output.AppendLine(result);

                        break;
                    }
                case TryCompileCommand: {
                        // Нет аргументов
                        string compileResult = project.TryCompileProject();
                        output.AppendLine(compileResult);
                        break;
                    }
                default: throw new InvalidOperationException("Not found command: " + jsonCommand.Name);
            }
        }
        return output.ToString();
    }
}