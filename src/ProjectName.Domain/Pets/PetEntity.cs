using ProjectName.Domain.Common;

namespace ProjectName.Domain.Pets;

public class PetEntity : EntityBase
{
    public required string Name { get; set; }
    public required DateTimeOffset BirthDate { get; set; }
}