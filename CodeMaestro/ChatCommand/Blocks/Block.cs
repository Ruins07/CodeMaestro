class Block(Entry[] entries, Block? Parent = null) {
    public Block? BaseBlock = Parent;
    public string Name = string.Empty;
    public Entry[] Steps = entries;
}