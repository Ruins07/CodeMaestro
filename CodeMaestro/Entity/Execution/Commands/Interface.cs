interface ICommandSet {
    ICommandData[] Read(string JSON);
    ICommandHandler Handler();
}
interface ICommandData {
    ICommandSet Set();
    string Name();
}
interface ICommandHandler {
    public struct Result {
        public string Text;
        public bool Log;
    }
    public struct Output {
        public string[] Results, Log;
    }
    /// <summary>
    /// Execute external commands and compile text result for next instructions for AI
    /// </summary>
    /// <param name="Command">sequence of command for execution</param>
    /// <returns>return output for next instruction for AI info/execution</returns>
    Output Execute(ICommandData[] Commands);
    Result Execute(ICommandData Command);
}