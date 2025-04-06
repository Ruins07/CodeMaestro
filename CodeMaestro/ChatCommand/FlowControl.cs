partial class Messaging {
    public void Execute(string Text = "") {
        if(Compilation is null) Compilation = new();
        if(Text != string.Empty) Request(Text);
        while (
            !(Stop |=
                Instruction >= InstructionsBlock
                    .Steps
                    .Length)
                )

            ExecuteNextCommand();
    }
    public void Execute(Block InstructionBlock) {
        Stack.Push(new(new(InstructionBlock)));
        Execute();
    }
    public void Return() {
        Stack.Pop();
    }
    public void Call(Block block, bool Jump = false) {
        if (Jump) Stack.TryPop(out _);
        Stack.Push(new(new(block)));
    }
}