partial class Compilation {
    public enum Section {
        PersistentMemory,
        TaskMemory,
        Log,

        USER, ASSISTENT
    }
    string Role(Section section) => section == Section.ASSISTENT ? "assistant" : "user ";

    Dictionary<Section, string> Splitters = new() {
        { Section.Log, "[API:LOG:LATEST]" },
        { Section.PersistentMemory, "[API:MEMORY:GLOBAL]" }
    };
}