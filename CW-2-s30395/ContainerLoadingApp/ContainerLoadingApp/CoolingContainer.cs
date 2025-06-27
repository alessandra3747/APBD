
namespace ContainerLoadingApp;

public class CoolingContainer : Container
{
    public string ProductType { get; set; }
    public double Temperature { get; set; }
    
    
    public CoolingContainer(double height, double weight, double depth, double maxCapacity, string productType, double temperature)
        : base(height, weight, depth, maxCapacity, "C")  
    {
        this.ProductType = productType;
        this.Temperature = temperature;
        
        if(!IsTemperatureValid())
            throw new ArgumentException("Invalid product type. The temperature is not suitable for this product.");
    }
    
    
    public override void LoadCargo(Product product)
    {
        if (product.Weight + CargoMass > MaxCapacity)
        {
            throw new OverfillException();
        }
        else if (product.ProductType != ProductType)
        {
            throw new ArgumentException($"Product must be of type: {ProductType}");
        }
        
        CargoMass += product.Weight;
    }

    private bool IsTemperatureValid()
    {
        switch (ProductType)
        {
            case "Bananas":
                return Temperature >= 13.3;
            case "Chocolate":
                return Temperature >= 18;
            case "Fish":    
                return Temperature >= 2;
            case "Meat":
                return Temperature >= -15;
            case "Ice cream":
                return Temperature >= -18;
            case "Frozen pizza":
                return Temperature >= -30;
            case "Cheese":
                return Temperature >= 7.2;
            case "Sausages":
                return Temperature >= 5;
            case "Butter":
                return Temperature >= 20.5;
            case "Eggs":
                return Temperature >= 19;
            default:
                return false;
            
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
                   Temperature: {Temperature}
                   ProductType: {ProductType}
                   Current CargoMass: {CargoMass}
                """;
    }
}