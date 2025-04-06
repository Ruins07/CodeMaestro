using System.Text;
partial class Memory {
    public class Category: INameDesc {
        public string Name = "GLOBAL";
        public List<Record> Records = [];
        public string ToString(bool Long = true) {
            var Title = $"Category: {Name}";

            var Recs = new StringBuilder(Title);
            if (Long) foreach (var record in Records)
                    Recs.AppendLine(record.ToString());
            return Recs.ToString();
        }
    }
}