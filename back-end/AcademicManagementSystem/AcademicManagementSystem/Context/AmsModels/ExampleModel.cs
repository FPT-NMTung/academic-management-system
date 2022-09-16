using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademicManagementSystem.Context.AmsModels;

public class ExampleModel
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name"), MinLength(20), MaxLength(50)]
    public string Name { get; set; } = null!;
}