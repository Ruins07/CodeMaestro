public partial class Log {
    public Func<Record, string> Formatter = Rec =>
    $"Completed {Rec.HandlerName}" + ' ' +
    NamedOrEmpty("object", Rec.ObjectName) + ' ' +
    NamedOrEmpty("operation", Rec.OperationName) + ' ' +
    $":{Rec.Size}";

    static string NamedOrEmpty(string Name, string Value) =>
        Value == string.Empty ?
            string.Empty :
            $"{Name}: {Value}";
}