namespace HPTourist.Services.Medication;

public abstract class MedicationException(string message) : Exception(message);

public class MedicationNotFoundException(string message = "Medicatie kon niet worden gevonden.") : MedicationException(message);

public class MedicationBusinessException(string message) : MedicationException(message);
