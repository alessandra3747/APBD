namespace ContainerLoadingApp;

public class OverfillException : Exception
{
    public OverfillException() : base("Cargo mass exceeds max capacity") { }
    
    public OverfillException(string message) : base(message) { }
}