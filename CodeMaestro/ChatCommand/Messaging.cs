partial class Messaging(ContextStack CurrentContext) {
    Setting? EndPointSettings;
    public ContextStack MessagingContext = CurrentContext;
    public Stack<AgentContext> Stack = new();
    public string Response = string.Empty;
    public Compilation? Compilation;
    public void Request(string Text) {
        if (Compilation is not null)
            Compilation.CurrentData.Request = Text;
    }
    async void ExecuteNextCommand() {
        if (MessagingContext.Connection is null) {
            Stop = true;
            return;
        }

        var NextStep = InstructionsBlock.Steps[Instruction];
        if (NextStep.NeedExecute()) {
            #pragma warning disable CS8604
            Execute(NextStep.SubNodes);
            #pragma warning restore CS8604
            return;
        }
        var NextCommand = NextStep.Command;
        Response = await MessagingContext
            .Connection
            .Send(
                NextCommand.Text,
                EndPointSettings
            );

        var Command = NextCommand.Receiver(Response);
        CommandRun(Command);
    }
    public void UpdateSettings() {
        var Copy = new AgentContext[Stack.Count];
        Stack.CopyTo(Copy, 0);
        foreach (var context in Copy) {
            var ContextSetting = context.Instructions.Setting;
            if (ContextSetting is not null)
                if (EndPointSettings is null) EndPointSettings = ContextSetting.Value;
                else EndPointSettings?.Set(ContextSetting.Value);
        }
    }

    void CommandRun(ControlFlow Command) {
        var Control = Command.Control;
        bool Jump = false;
        switch (Control) {
            case ControlCommand.RepeatBlock:
                Instruction = 0;
                break;
            case ControlCommand.Continue:
                Instruction++;
                break;
            case ControlCommand.Return:
                Return();
                return;
            case ControlCommand.Call:
                Jump = false;//Call(Command.Link, false);
                break;
            case ControlCommand.Jump:
                Jump = true; // Call(Command.Link, true);
                break;
        }
        if (Command.Link is null) throw new InvalidOperationException();

        var NextBlock = Command.Link;
        if (Command.NewContext)
            MessagingContext.Call(NextBlock, !Command.ClearContext, Jump);
        else
            Call(NextBlock, Jump);
    }
}