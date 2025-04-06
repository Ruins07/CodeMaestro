partial class Compilation {
    public struct MessageData: ICloneable {
        public List<(Section sectiontype, string message)> PreviousMessages;
        public string[] Files;
        public string Request;

        public object Clone() {
            var Src = (MessageData)MemberwiseClone();
            Src.PreviousMessages = [.. PreviousMessages];
            Src.Files = [.. Files];
            return Src;
        }

        public void Acquire(string Response){
            PreviousMessages.Add((Section.USER, Request));
            PreviousMessages.Add((Section.ASSISTENT, Response));
            Request = string.Empty;
        }
    }
}