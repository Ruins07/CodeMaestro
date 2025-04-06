/// <summary>
/// Sparse data represented
/// </summary>
partial class Memory: ICloneable {
    public List<Category> Categories = [];
    public Category? this[string Name] => Categories
        .Where(C => C.Name == Name)
        .SingleOrDefault();

    public object Clone() {
        var Src = (Memory)MemberwiseClone();
        Src.Categories = [.. Categories];
        return Src;
    }
}