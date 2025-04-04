using CodeMaestro;
using SpongeEngine;
using SpongeEngine.LMStudioSharp;
using SpongeEngine.LMStudioSharp.Models.Chat;
using SpongeEngine.LMStudioSharp.Models.Completion;
using SpongeEngine.LMStudioSharp.Models.Model;
using System.Text;
using System.Windows.Markup;

var MainTask = "UserTask: Think First, Напиши вывод Hello World C# через консоль в файл Program.cs(only commands response/Think is not response)(lastError: Source code not present as commands to write file)";
var ProjectPath = "TestProject";
var ProgramFile = Path.Combine(ProjectPath, "Program.cs");

Console.WriteLine("Wait Connect");

Directory.CreateDirectory(ProjectPath);
if (!File.Exists(ProgramFile)) File.Create(ProgramFile, 512);

IConnection Interface = new MainAPI();
Sync MainSync = new(ProjectPath);
Project MainProject = new Project();

Console.WriteLine("Request: " + MainTask);

/*PerStep[] Steps = [];

foreach (var Step in Steps) {IndexOutOfRangeException: "Index was outside the 
    var Res = Interface.Send("Hi, its system test, just say: Status OK");
    Console.WriteLine(Res);
}*/

while (true) {
    var CurrentRequest = MainTask;
    var Res = "Response: " + Interface.Send(CurrentRequest, null).Result;
    //.Split("</think>")[1];
    Console.WriteLine(Res);

    var Cmds = JsonCommandParser.ParseCommands(Res);
    try {
        var Exec = JsonCommandExecutor
            .ExecuteJsonCommandsAsync(Cmds, MainSync, MainProject)
            .Result;

        Console.WriteLine(Exec);
        break;
    }
    catch (Exception ex) {
        CurrentRequest = ex.Message;
    }
}
Console.ReadLine();

interface iAI {
    bool Connect();
    Task<string> Send(string Text);
    bool Disconnect();

}
struct MainAPI: IConnection {
    LMStudioClientOptions Options;
    LMStudioSharpClient Client;
    string Model;

    public MainAPI() {
        Options = new LMStudioClientOptions {
            HttpClient = new HttpClient {
                BaseAddress = new Uri("http://localhost:1234")
            }
        };
    }
    public bool Connect() {
        try {
            Client = new(Options);
            var models = Client.ListModelsAsync().Result;
            var modelId = models.Data[0].Id;
            Model = modelId;

            return true;
        }
        catch {
            return false;
        }
    }
    public async Task<string> Send(string Text, Setting? setting) {
        var completionRequest = new CompletionRequest {
            Prompt = Text,
            Temperature = (float)setting?.Varieties
        };
        var completionResponse = await Client.CompleteAsync(completionRequest);
        return completionResponse.Choices[0].GetText();
    }
    public bool Disconnect() {
        return true;
    }
}
interface PerStep {
    const string AllStages =
        "1. Сначала мы будем обрабатывать только аналитический" +
        "2. следующий шаг подразумевает анализ с технической стороны" +
        "3. последний шаг будет синтез обоих анализов" +
        "4. переход к следующему этапу - полному техническому анализу";
    const string CurrentStepBegin =
        "На данный момент рассматривается только стадия ";

    public string Title();
    public string SmallDesc();
    public string FullText();
    public PerStep[] SubNodes();

    public string Me() {
        var Me =
$@"###{Title}###
__________________________________________
Desc: {SmallDesc}

------------------------------------------
FullTask: {FullText}

------------------------------------------
------------------------------------------";
        return Me;
    }
    public string Text() {
        var M = Me();
        foreach (var Node in SubNodes()) {
            var SubText = Node.Text()
                .Split('\n');
            var Builder = new StringBuilder();
            foreach (var CurrentString in SubText)
                Builder.AppendLine("\t\t" + CurrentString);
            M += Builder.ToString();
        }
        return M;
    }
}

internal class Abstract: PerStep {
    public string FullText() => $"{PerStep.AllStages} \n {PerStep.CurrentStepBegin} 1";
    public string SmallDesc() => 
        "Разбей задачу на основные аналитические стадии, ограничимся 7 основными последовательными стадиями." +
        "Опиши каждую в кратце не углубляясь в подробности.";
    public PerStep[] SubNodes() => Array.Empty<PerStep>();
    public string Title() => "Абстрактная обработка";
}
internal class AbstractSubTask: PerStep {
    public int Stage = 1;
    public string StageSplit = "";
    public string FullText() => "";
    public string SmallDesc() =>
        "Подробный анализ стадии";
    public PerStep[] SubNodes() => Array.Empty<PerStep>();
    public string Title() => "Абстрактная обработка, Стадия " + Stage + ":";
}

public static class ClassOverride {
    public static T Exec<T>(this T obj, Action<T> func) {
        func(obj);
        return obj;
    }
}