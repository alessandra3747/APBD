
using ContainerLoadingApp;


LiquidContainer cL1 = new LiquidContainer(5, 50, 20, 50, true);
LiquidContainer cL2 = new LiquidContainer(5, 50, 20, 50, false);

// cL1.LoadCargo(new Product("Chemicals", 50)); OverfillException: Liquid cargo exceeded allowed limit of 25kg.

cL1.LoadCargo(new Product("Chemicals", 25));
Console.WriteLine($"\nAdding 25kg.\n\n{cL1}");

cL2.LoadCargo(new Product("Water", 45));
Console.WriteLine($"\nAdding 45kg.\n\n{cL2}");

cL2.UnloadCargo();
Console.WriteLine($"\nUnloading the cargo.\n\n{cL2}");



GasContainer cG1 = new GasContainer(3, 10, 50, 15, 30, true);
GasContainer cG2 = new GasContainer(5, 10, 50, 15, 60, false);

cG1.LoadCargo(new Product("Phosphine", 10));
Console.WriteLine($"\nAdding 10kg.\n\n{cG1}");

cG1.NotifyAboutDanger();

//cG2.LoadCargo(new Product("Oxygen", 20)); OverfillException: Liquid cargo exceeded allowed limit of 15kg.



CoolingContainer cC1 = new CoolingContainer(10, 70, 50, 200, "Bananas", 15);

//CoolingContainer cC2 = new CoolingContainer(10, 70, 50, 200, "Ice cream", 5); System.ArgumentException: Invalid product type. The temperature is not suitable for this product.

//cC1.LoadCargo(new Product("Butter", 190)); System.ArgumentException: Product must be of type: Bananas

cC1.LoadCargo(new Product("Bananas", 190));
Console.WriteLine($"\nAdding 190kg.\n\n{cC1}\n");



//ContainerShip containerShip1 = new ContainerShip(120, 9, 100);
//after adding containers: ContainerLoadingApp.OverfillException: Containers ship cargo exceeded allowed limit of 100kg.


ContainerShip containerShip1 = new ContainerShip(120, 9, 1500);
Console.WriteLine($"Created: {containerShip1}\n");

containerShip1.AddContainers(cL1);
Console.WriteLine($"ContainerShip1 after adding container:\n {containerShip1}\n");

containerShip1.AddContainers(cL2, cG1, cC1);
Console.WriteLine($"ContainerShip1 after adding containers:\n {containerShip1}\n");


containerShip1.RemoveContainers(cL1);
Console.WriteLine($"ContainerShip1 after removing container:\n {containerShip1}\n");

containerShip1.ReplaceContainers("KON-L-2", cL1);
Console.WriteLine($"ContainerShip1 after replacing containers:\n {containerShip1}\n");


ContainerShip containerShip2 = new ContainerShip(180, 10, 1300);

containerShip1.MoveContainerToOtherShip("KON-L-1", containerShip2);
Console.WriteLine($"ContainerShip1 after moving container:\n {containerShip1}\n");
Console.WriteLine($"ContainerShip2 after moving container:\n {containerShip2}\n");