
[Entity]
public class Student
{
    [Column(primary: true, generated: true)]
    public int Id { get; set; }
    [Column]
    public string name { get; set; }
    [Column]
    public int age { get; set; }
}
