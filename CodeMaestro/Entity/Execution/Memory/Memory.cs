/// <summary>
/// Sparse data represented
/// </summary>
partial class Memory {
    public List<Category> Categories = [];
    public Category? this[string Name] => Categories
        .Where(C => C.Name == Name)
        .SingleOrDefault();
}