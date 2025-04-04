partial class Compilation {
    public struct MessageData {
        public List<(Section sectiontype, string message)> PreviousMessages;
        public string[] Files;
        public string Request;
    }
}