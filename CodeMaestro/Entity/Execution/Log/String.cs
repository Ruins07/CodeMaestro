using System.Text;
public partial class Log {
    public override string? ToString() => ToString();
    public string? ToString(short Size = -1) {
        if (Records.Count == 0) return null;

        if (Size == -1)
            Size = LogSize < New ? LogSize : (short)New;

        var Builder = new StringBuilder();
        var List = Records.TakeLast(Size);
        foreach (var Record in List)
            Builder.AppendLine(Formatter(Record));

        New = 0;
        return Builder.ToString();
    }
}