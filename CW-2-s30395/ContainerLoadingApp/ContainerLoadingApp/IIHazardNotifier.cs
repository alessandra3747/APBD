namespace ContainerLoadingApp;

public interface IIHazardNotifier
{
    void NotifyAboutDanger();
    public bool IsHazardous { get; set; }
}