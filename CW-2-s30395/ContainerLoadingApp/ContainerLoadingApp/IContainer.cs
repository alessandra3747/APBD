namespace ContainerLoadingApp;

public interface IContainer
{
    double CargoMass { get; }
    double Height { get; }
    double Weight { get; }
    double Depth { get; }
    double MaxCapacity { get; }
    string SerialNumber { get; }
    
    void LoadCargo(Product product);
    void UnloadCargo();
}