namespace ContainerLoadingApp;

public class LiquidContainer(double height, double weight, double depth, double maxCapacity, bool isHazardous) 
    : Container(height, weight, depth, maxCapacity, "L") , IIHazardNotifier
{

    public bool IsHazardous { get; set; } = isHazardous;
    
    public void NotifyAboutDanger()
    {
        Console.WriteLine(
            $"Warning! Dangerous situation involving a liquid container happened! " +
            $"Container number:{SerialNumber}");
    }
    public override void LoadCargo(Product product)
    {
        double limit = IsHazardous ? MaxCapacity * 0.5 : MaxCapacity * 0.9;
        if (product.Weight > limit)
            throw new OverfillException($"Liquid cargo exceeded allowed limit of {limit}kg.");
        else
        {
            CargoMass += product.Weight;
        }
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
                   IsHazardous: {isHazardous}
                   Current CargoMass: {CargoMass}
                """;
    }
    
}