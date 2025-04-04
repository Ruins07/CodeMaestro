class Context(IConnection Connection) {
    public IConnection Connection = Connection;
    public Stack<Messaging> Stack = new();
    public void Call(Messaging MessageContext, bool Jump = false) {
        if (Jump) Stack.TryPop(out _);
        Stack.Push(MessageContext);
    }
    public void Return() => Stack.Pop();
}