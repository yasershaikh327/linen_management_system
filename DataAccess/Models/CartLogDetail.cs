namespace LinenManagementAPI.Models;

public class CartLogDetail
{
    public int CartLogDetailId { get; set; }
    public int CartLogId { get; set; }
    public int LinenId { get; set; }
    public int Count { get; set; }
}