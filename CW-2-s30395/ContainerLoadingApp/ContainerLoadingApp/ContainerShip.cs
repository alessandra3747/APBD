namespace ContainerLoadingApp;

public class ContainerShip (double maxSpeed, int maxCapacity, double maxWeight)
{
    public double MaxSpeed { get; set; } = maxSpeed;
    
    public double MaxWeight { get; set; } = maxWeight;
    
    private double _currentWeight;
    
    public List<Container> ContainersList = new List<Container>(maxCapacity);

    
    public void AddContainers(params Container[] containers)
    {
        foreach (var container in containers)
        {
            if (_currentWeight + container.Weight + container.CargoMass < MaxWeight)
            {
                ContainersList.Add(container);
                _currentWeight += container.Weight + container.CargoMass;
            }
            else
            {
                throw new OverfillException($"Containers ship cargo exceeded allowed limit of {MaxWeight}kg.");
            }
        }
    }

    public void RemoveContainers(params Container[] containers)
    {
        containers.ToList().ForEach(container => ContainersList.Remove(container));
    }

    public void ReplaceContainers(string serialNumberToReplace, Container newContainer)
    {
        for (int i = 0; i < ContainersList.Count; i++)
        {
            if (ContainersList[i].SerialNumber == serialNumberToReplace)
            {
                ContainersList[i] = newContainer;
                return;
            }
        }
    }

    public void MoveContainerToOtherShip(string serialNumberToMove, ContainerShip otherShip)
    {
        for (int i = 0; i < ContainersList.Count; i++)
        {
            if (ContainersList[i].SerialNumber == serialNumberToMove)
            {
                otherShip.AddContainers(ContainersList[i]);
                ContainersList.Remove(ContainersList[i]);
                return;
            }
        }
    }
    
    
    public override string ToString()
    {
        string containerInfo = "";
        foreach (Container container in ContainersList)
            containerInfo += container.SerialNumber + " ";
        
        return $"""
                Container Ship info:
                    MaxSpeed: {MaxSpeed}
                    MaxWeight: {MaxWeight}
                    Containers: {containerInfo}
                """;
    }
    
}