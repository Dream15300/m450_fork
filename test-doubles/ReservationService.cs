namespace TestDoublesExercise;

public sealed record Reservation(string CustomerEmail, string EventCode, int SeatCount);

// Für diese Schnittstelle existiert in der Übung noch keine produktive Implementierung.
public interface ISeatAvailability
{
    int GetAvailableSeats(string eventCode);
}

public interface IReservationRepository
{
    // Gibt false zurück, wenn die Reservierung nicht gespeichert werden konnte.
    bool TrySave(Reservation reservation);
    bool TrySaveBatch(IReadOnlyList<Reservation> reservations)
    {
        throw new NotSupportedException(
            "Batch-Speicherung wird nicht unterstützt."); // erhält eine Standardimplementierung, damit die Schnittstelle nicht geändert werden muss, wenn eine Implementierung diese Methode nicht unterstützt.
    }
}

public interface IConfirmationSender
{
    void Send(Reservation reservation);
}

public sealed class ReservationService
{
    private readonly ISeatAvailability _availability;
    private readonly IReservationRepository _repository;
    private readonly IConfirmationSender _sender;

    public ReservationService(
        ISeatAvailability availability,
        IReservationRepository repository,
        IConfirmationSender sender)
    {
        _availability = availability ?? throw new ArgumentNullException(nameof(availability));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));
    }

    public bool Reserve(string customerEmail, string eventCode, int seatCount)
    {
        if (string.IsNullOrWhiteSpace(customerEmail))
        {
            throw new ArgumentException("Eine E-Mail-Adresse ist erforderlich.", nameof(customerEmail));
        }

        if (string.IsNullOrWhiteSpace(eventCode))
        {
            throw new ArgumentException("Ein Veranstaltungscode ist erforderlich.", nameof(eventCode));
        }

        if (seatCount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(seatCount));
        }

        if (_availability.GetAvailableSeats(eventCode) < seatCount)
        {
            return false;
        }

        var reservation = new Reservation(customerEmail, eventCode, seatCount);
        if (!_repository.TrySave(reservation))
        {
            return false;
        }

        _sender.Send(reservation);
        return true;
    }

    // Von KI machen lassen, hab sie nicht selber geschafft
    public bool ReserveBatch(IReadOnlyList<Reservation> requests)
    {
        // 1. Eingaben überprüfen
        ArgumentNullException.ThrowIfNull(requests);

        if (requests.Count == 0)
            throw new ArgumentException("Keine Reservierungen vorhanden.");

        var totals = new Dictionary<string, int>();

        foreach (var request in requests)
        {
            if (string.IsNullOrWhiteSpace(request.CustomerEmail))
                throw new ArgumentException("Ungültige E-Mail-Adresse.");

            if (string.IsNullOrWhiteSpace(request.EventCode))
                throw new ArgumentException("Ungültiger Veranstaltungscode.");

            if (request.SeatCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(requests));

            // Plätze pro Veranstaltung zusammenzählen
            if (!totals.ContainsKey(request.EventCode))
                totals[request.EventCode] = 0;

            totals[request.EventCode] = checked(
                totals[request.EventCode] + request.SeatCount);
        }

        // 2. Verfügbarkeit prüfen
        foreach (var entry in totals)
        {
            int available = _availability.GetAvailableSeats(entry.Key);

            if (available < entry.Value)
                return false;
        }

        // 3. Alle Reservierungen gemeinsam speichern
        if (!_repository.TrySaveBatch(requests))
            return false;

        // 4. Bestätigungen versenden
        foreach (var request in requests)
        {
            _sender.Send(request);
        }

        return true;
    }

}
