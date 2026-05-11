

namespace Myshop.Core.Dtos{
public class BulkStatusDto
{
    public List<int> Ids    { get; set; } = new();
    public string    Status { get; set; } = "Active";
}
}