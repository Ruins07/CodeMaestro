partial class Log {
    List<Record> Records = [];
    private ushort New = 0;

    public byte LogSize = 16;
    //public bool OnlyNewMod = true;
    public void Reg(params Record[] recs) {
        Records.AddRange(recs);
        New += (ushort)recs.Length;
    }
    public void Reg(string HandlerName, string ObjectName = "", string OperationName = "", int Size = 1) {
        Reg(new Record() {
            HandlerName = HandlerName,
            ObjectName = ObjectName,
            OperationName = OperationName,
            Size = Size
        });
    }
    public void Clear(int Size = -1) {
        if (Size == -1) Records.Clear();
        else if (Size > 0)
            Records = [.. Records.TakeLast(Size)];
    }
}