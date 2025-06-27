namespace ContainerLoadingApp;

public class Product (string productType, double weight)
{
    public string ProductType { get; set; } = productType;
    public double Weight { get; set; } = weight;
}