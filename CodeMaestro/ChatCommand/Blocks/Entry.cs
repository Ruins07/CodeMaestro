class Entry(string TextOnly = "") {
    public BasicCommand Command = new BasicCommand()
        .Exec(cmd =>
            cmd.Text = TextOnly);
    public Block? SubNodes;
    public bool NeedExecute() => SubNodes is not null;
    public override string ToString() {
        return Command.Text;
    }
    public static Entry[] Cast(params string[] args) {
        var Result = new List<Entry>(args.Length);
        foreach (var Arg in args)
            Result.Add(new(Arg));
        return [.. Result];
    }
}