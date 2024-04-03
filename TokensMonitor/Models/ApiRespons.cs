namespace TokensMonitor.Models;

public class ApiResponse<TData>(TData data)
{
    public TData Data { get; set; } = data;
}