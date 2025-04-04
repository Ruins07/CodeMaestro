using System.Text;
partial class Compilation {
    public Memory SystemMemory = new();
    public Log OperationLog = new();

    public MessageData CurrentData = new();
    public bool ExplicitVector = true;

    public struct Message { public string Role, Text; }
    public Message[] Compile() {
        var Text = new StringBuilder();
        var MemoryCategory = SystemMemory["GLOBAL"];
        var Memory = MemoryCategory?.ToString();
        var Log = OperationLog.ToString();

        var List = new List<Message>();
        if (Memory is not null) {
            var FirstMessage = new Message() {
                Role = Role(Section.PersistentMemory),
                Text = Splitters[Section.PersistentMemory] + Memory
            };
            List.Add(FirstMessage);
        }
        var Messages = CurrentData
            .PreviousMessages
            .Select(
                Message =>
                    new Message() {
                        Role = Role(Message.sectiontype),
                        Text = Message.message
                    }
                );
        List.AddRange(Messages);
        if(Log is not null) {
            var LogMessage = new Message() {
                Role = Role(Section.Log),
                Text = Splitters[Section.Log] + Log
            };
            List.Add(LogMessage);
        }
        var Request = new Message() {
            Role = Role(Section.USER),
            Text = CurrentData.Request
        };
        List.Add(Request);

        return [.. List];
    }
}