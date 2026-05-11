namespace Myshop.Core.Dtos{
    public class BulkPriceDto
{
    public List<int> Ids     { get; set; } = new();
    public string    Mode    { get; set; } = "set";    // "set" | "increase" | "decrease"
    public decimal   Value   { get; set; } = 0m;


}
}