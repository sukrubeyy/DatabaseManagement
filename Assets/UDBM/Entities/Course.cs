
[Entity]
public class Course
{
    [Column(primary: true, generated: true)]
    public int Id { get; set; }
    [Column]
    public string name { get; set; }
}
