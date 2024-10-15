namespace SampleApi.Models;

public class Contact : ContactBase
{
    public int Id { get; set; }
    public DateTime CreatedTime { get; set; }
    public DateTime UpdateTime { get; set; }
}
