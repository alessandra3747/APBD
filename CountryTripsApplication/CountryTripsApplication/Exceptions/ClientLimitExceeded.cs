namespace CountryTripsApplication.Exceptions;

public class ClientLimitExceeded(string message) : Exception(message)
{
    
}