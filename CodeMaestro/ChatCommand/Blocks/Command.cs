/// <summary>
/// Class control messages execution
/// messages send one by one until cancellation token
/// each message session can be setting with prompt, p and varieties(temperature)
/// also has JSON serializer for responses
/// and interface to call custom manager for every command list, which resolve execution
/// can cancel or change settings and analyze response
/// </summary>

public enum ControlCommand { Continue, RepeatBlock, RepeatInstruction, Return, Call, Jump }
class BasicCommand {    
    public Setting? Setting = null;
    public string Text = string.Empty;
    public Func<string, ControlFlow> Receiver = S => new();
}
struct ControlFlow(ControlCommand ResultCommand = ControlCommand.Continue) {
    public string Request = string.Empty;
    public ControlCommand Control = ResultCommand;
    public Block? Link;
    public bool NewContext, ClearContext;
}