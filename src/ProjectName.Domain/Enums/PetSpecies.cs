using Ardalis.SmartEnum;

namespace ProjectName.Domain.Enums;

/// <summary>
/// Represents the species of a pet in the domain layer, providing a set of predefined species such as Dog, Cat, Rabbit, Hamster, and GuineaPig.
/// </summary>
public sealed class PetSpecies : SmartEnum<PetSpecies>
{
    public static readonly PetSpecies Dog = new(nameof(Dog), 1);
    public static readonly PetSpecies Cat = new(nameof(Cat), 2);
    public static readonly PetSpecies Rabbit = new(nameof(Rabbit), 3);
    public static readonly PetSpecies Hamster = new(nameof(Hamster), 4);
    public static readonly PetSpecies GuineaPig = new(nameof(GuineaPig), 5);

    public static IEnumerable<PetSpecies> All => [Dog, Cat, Rabbit, Hamster, GuineaPig];

    private PetSpecies(string name, int value) : base(name, value)
    {
    }
}
