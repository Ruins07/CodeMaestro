partial class Messaging(Context CurrentContext) {
    Setting? EndPointSettings;
    public Context MessagingContext = CurrentContext;
    public Stack<AgentContext> Stack = new();
    public string Response = string.Empty;
    public Compilation Compilation = new();
    public async void ExecuteNextCommand() {
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
        var Control = Command.Control;

        switch (Control) {
            case BasicCommand.ControlCommand.RepeatBlock:
                Instruction = 0;
                break;
            case BasicCommand.ControlCommand.Continue:
                Instruction++;
                break;
            case BasicCommand.ControlCommand.Return:
                Return();
                return;
            #pragma warning disable CS8604
            case BasicCommand.ControlCommand.Call:
                Call(Command.Link, false);
                return;
            case BasicCommand.ControlCommand.Jump:
                Call(Command.Link, true);
                return;
            #pragma warning restore CS8604
        }
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
}