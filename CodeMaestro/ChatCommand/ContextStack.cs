class ContextStack(IConnection Connection) {
    public IConnection Connection = Connection;
    public Stack<Messaging> Context = new();
    public string Result = string.Empty;
    bool Continue = true;
    public void Execute() {
        while (Continue && Context.Count > 0)
            Context.Peek().Execute();
    }
    public void Start(string Text, Block Instructions) {
        Call(Instructions, false, false);
        Context.Peek().Request(Text);
        Continue = true;
        Execute();
    }
    public void Call(Messaging MessageContext, bool CopyStory = true, bool Jump = false) {
        if (Context.Count > 0) {
            if (CopyStory) {
                var Src = Context.Peek()
                    .Compilation?.DeepCopy();
                MessageContext.Compilation = Src;
            }
            if (Jump) Return();
        }
        Context.Push(MessageContext);
    }
    public void Call(Block Instructions, bool CopyStory = true, bool Jump = false) {
        var MessagingContext = new Messaging(this);
        MessagingContext.Call(Instructions);
        Call(MessagingContext, CopyStory, Jump);
    }
    public void Return() {
        Result = Context.Peek().Response;
        Context.Pop();
    }
    public void Stop() => Continue = false;
}