
[Entity]
public class Student
{
    [Column(primary: true, generated: true)]
    public string Id { get; set; }
    public string name { get; set; }
    public int age { get; set; }
}
