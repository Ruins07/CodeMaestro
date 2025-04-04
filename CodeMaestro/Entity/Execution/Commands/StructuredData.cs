class StructuredData {
    public string Name = "";
    public ICommandSet? Set;
    public ICommandData[] Commands;
    public ICommandHandler Handler;

    public StructuredData(ICommandData[] Commands, ICommandSet Set) {
        this.Commands = Commands;
        this.Set = Set;
        this.Handler = Set.Handler();
    }
    public StructuredData(ICommandData[] Commands, ICommandHandler Handler) {
        this.Commands = Commands;
        this.Handler = Handler;
    }
    public ICommandHandler.Output Execute() {
        return Handler.Execute(Commands);
    }
}