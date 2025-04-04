partial class Memory {
    public class Record: INameDesc {
        public string Name = ""; //short desc
        public DateTime Time = DateTime.Now;
        public string Text = "";
        public string ToString(bool Long = true) {
            var T = $"{Time}: [{Name}] \n";
            if (Long)
                T += $"{Text}\n" +
                $"====[End:{Name}]====";
            return T;
        }
    }
}