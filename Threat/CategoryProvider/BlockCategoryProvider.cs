namespace MESHelper.Threat.CategoryProvider
{
    public interface BlockCategoryProvider
    {
        string Name { get; }
        string GetCategory(object obj);
    }
}
