namespace ContainerLoadingApp;

public abstract class Container (double height, double weight, double depth, double maxCapacity, string type) : IContainer
{

    public string Type { get; } = type;
    public double CargoMass { get; protected set; }

    public double Height { get; } = height;
    
    public double Weight { get; } = weight;
    
    public double Depth { get; } = depth;
    public double MaxCapacity { get; } = maxCapacity;

    
    private static int _idCounter = 1;

    public string SerialNumber { get; } = $"KON-{type}-{_idCounter++}";


    public virtual void UnloadCargo()
    {
        CargoMass = 0;
    }

    public virtual void LoadCargo(Product product)
    {
        if (CargoMass + product.Weight > MaxCapacity)
            throw new OverfillException();
        else 
            CargoMass += product.Weight;
    }

    public override string ToString()
    {
        return $"""
                 Container {SerialNumber}
                    Height: {Height}
                    Weight: {Weight}
                    Depth: {Depth}
                    MaxCapacity: {MaxCapacity}
                    Type: {Type}
                    Current CargoMass: {CargoMass}
                 """;
    }
}
