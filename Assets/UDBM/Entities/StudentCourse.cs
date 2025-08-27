using UnityEngine;

[Entity]
public class StudentCourse
{
    [Column(primary: true, generated: true)]
    public int Id { get; set; }

    [Column(false, false)]
    [Relation("many-to-one", "Student", "studentId")]
    public int studentId { get; set; }

    [Relation("many-to-one", "Course", "courseId")]
    [Column(false, false)]
    public int courseId { get; set; }
}

