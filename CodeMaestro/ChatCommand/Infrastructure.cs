interface IConnection {
    Task<string> Send(string Message, 
        Setting? Setting = null, 
        Compilation.Message[]? messages = null);
}
class ChatData(Block InstructionsBlock) {
    public Setting? Setting;
    public Block InstructionsBlock = InstructionsBlock;
}
class AgentContext(ChatData Data) {
    public ChatData Instructions = Data;
    public ushort CurrentInstruction;
    public bool Stop;
}