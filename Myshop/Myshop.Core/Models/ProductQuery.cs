public class ProductQuery
{

    public string? Search { get; set; }
    public string? Category { get; set; }
    public string? Brand { get; set; }
    public decimal? Min { get; set; }
    public decimal? Max { get; set; }
    public string? Sort { get; set; } = "new";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}