namespace ContainerLoadingApp;

public class GasContainer(double height, double weight, double depth, double maxCapacity, double pressure, bool isHazardous) 
    : Container(height, weight, depth, maxCapacity, "G") , IIHazardNotifier
{
    
    public double Pressure { get; set; } = pressure;
    public bool IsHazardous { get; set;  } = isHazardous;

    public void NotifyAboutDanger()
    {
        Console.WriteLine(
            $"Warning! Dangerous situation involving a gas container happened! " +
            $"Container number:{SerialNumber}"
            );
    }
    
    public override void UnloadCargo()
    {
        CargoMass = CargoMass * 0.05;
    }

    public override void LoadCargo(Product product)
    {
        if (CargoMass + product.Weight > MaxCapacity)
            throw new OverfillException($"Liquid cargo exceeded allowed limit of {MaxCapacity}kg.");
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
                   Pressure: {Pressure}
                   IsHazardous: {isHazardous}
                   Current CargoMass: {CargoMass}
                """;
    }
    
}