partial class Messaging {
    AgentContext Context() => Stack.Peek();
    ChatData CurrentData() => Context().Instructions;
    bool Stop {
        get => Context().Stop;
        set => Context().Stop = value;
    }
    ushort Instruction {
        get => Context().CurrentInstruction;
        set => Context().CurrentInstruction = value;
    }
    Block InstructionsBlock {
        get => CurrentData().InstructionsBlock;
        set => CurrentData().InstructionsBlock = value;
    }
    public void Set(Setting setting) {
        EndPointSettings?.Set(setting);
    }
}