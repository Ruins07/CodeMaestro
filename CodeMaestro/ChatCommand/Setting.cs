struct Setting() {
    public string Prompt = string.Empty;
    public double P = double.NaN, Varieties = double.NaN;
    public string Schema = string.Empty; //JSON
    public void Set(Setting setting) {
        if (setting.Prompt != string.Empty) Prompt = setting.Prompt;
        if (setting.P != double.NaN) P = setting.P;
        if (setting.Varieties != double.NaN) Varieties = setting.Varieties;
        if (setting.Schema != string.Empty) Schema = setting.Schema;
    }
}