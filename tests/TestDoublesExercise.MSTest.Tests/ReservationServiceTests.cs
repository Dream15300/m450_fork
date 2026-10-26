namespace TestDoublesExercise.MSTest.Tests;

[TestClass]
public sealed class ReservationServiceTests
{
    // Entfernt Ignore jeweils erst, wenn ihr den Test implementiert habt.
    // Die Beispiele für Stub, Spy und Moq stehen in TestDoubleExamples.cs.
    // Aufgabe 1:
    [TestMethod]
    public void Reserve_WithEnoughSeats_ReturnsTrue()
    {
        // Arrange: Stub mit 3 freien Plätzen, Repository mit erfolgreichem Speichern,
        //          eigener Sender-Spy und ReservationService.
        var availabilityStub = new SeatAvailabilityStub(3);
        var repositoryStub = new ReservationRepositoryStub();
        var confirmationSenderSpy = new ConfirmationSenderSpy();
        var reservationService = new ReservationService(availabilityStub, repositoryStub, confirmationSenderSpy); // Constructor Dependency Injection

        // Act:     2 Plätze reservieren.
        var result = reservationService.Reserve("ivan@test.ch", "EVENT123", 2);

        // Assert:  Ergebnis ist true.
        Assert.IsTrue(result, "Reservation sollte gelingen, wenn genügend Plätze verfügbar sind.");
    }







    [TestMethod]
    public void Reserve_WithTooFewSeats_DoesNotSaveOrSend()
    {
        // Arrange: Stub mit 1 freiem Platz und Doubles für Repository und Sender.
        var availabilityStub = new SeatAvailabilityStub(1);
        var repositoryStub = new ReservationRepositoryStub();
        var confirmationSenderSpy = new ConfirmationSenderSpy();
        var reservationService = new ReservationService(availabilityStub, repositoryStub, confirmationSenderSpy);

        // Act:     2 Plätze reservieren.
        var result = reservationService.Reserve("ivan@test.ch", "EVENT123", 2);

        // Assert:  Ergebnis ist false; weder Speichern noch Senden wurde aufgerufen.
        Assert.IsFalse(result, "Reservation sollte fehlschlagen, wenn nicht genügend Plätze verfügbar sind.");
        Assert.AreEqual(0, repositoryStub.SaveCalls, "Repository sollte nicht aufgerufen werden.");
        Assert.IsFalse(confirmationSenderSpy.WasCalled, "Sender sollte nicht aufgerufen werden.");
    }





    [TestMethod]
    [Ignore("TODO: Grenzfall testen")]
    public void Reserve_WithExactlyEnoughSeats_Succeeds()
    {
        // Arrange: Stub mit genau 2 freien Plätzen.
        // Act:     2 Plätze reservieren.
        // Assert:  Ergebnis ist true.
        throw new NotImplementedException();
    }






    [TestMethod]
    [Ignore("TODO: Platzanzahl 0 testen")]
    public void Reserve_WithZeroSeats_ThrowsBeforeCallingDependencies()
    {
        // Arrange: Doubles, deren Aufrufe sich prüfen lassen.
        // Act:     0 Plätze reservieren.
        // Assert:  ArgumentOutOfRangeException; keine Abhängigkeit wurde aufgerufen.
        throw new NotImplementedException();
    }






    [TestMethod]
    [Ignore("TODO: negative Platzanzahl testen")]
    public void Reserve_WithNegativeSeats_ThrowsBeforeCallingDependencies()
    {
        // Arrange: Doubles, deren Aufrufe sich prüfen lassen.
        // Act:     -1 Plätze reservieren.
        // Assert:  ArgumentOutOfRangeException; keine Abhängigkeit wurde aufgerufen.
        throw new NotImplementedException();
    }






    [TestMethod]
    [Ignore("TODO: leeren Veranstaltungscode testen")]
    public void Reserve_WithEmptyEventCode_ThrowsBeforeCallingDependencies()
    {
        // Arrange: Doubles, deren Aufrufe sich prüfen lassen.
        // Act:     Mit leerem Veranstaltungscode reservieren.
        // Assert:  ArgumentException; keine Abhängigkeit wurde aufgerufen.
        throw new NotImplementedException();
    }






    [TestMethod]
    [Ignore("TODO: E-Mail-Adresse aus Leerzeichen testen")]
    public void Reserve_WithWhitespaceEmail_ThrowsBeforeCallingDependencies()
    {
        // Arrange: Handgeschriebene Doubles oder Moq gezielt auswählen.
        // Act:     Mit einer E-Mail-Adresse aus Leerzeichen reservieren.
        // Assert:  ArgumentException; keine Abhängigkeit wurde aufgerufen.
        throw new NotImplementedException();
    }






    [TestMethod]
    [Ignore("TODO: fehlgeschlagenes Speichern testen")]
    public void Reserve_WhenSavingFails_DoesNotSendConfirmation()
    {
        // Arrange: Stub mit genügend Plätzen, Repository liefert false, eigener Spy.
        // Act:     Reservierung versuchen.
        // Assert:  Ergebnis ist false und der Spy hat keinen Aufruf aufgezeichnet.
        throw new NotImplementedException();
    }






    [TestMethod]
    [Ignore("TODO: Moq für fehlenden Speicherversuch verwenden")]
    public void Reserve_WithTooFewSeats_DoesNotCallRepository()
    {
        // Arrange: Stub mit zu wenigen Plätzen und Mock<IReservationRepository>.
        // Act:     Reservierung versuchen.
        // Assert:  Verify(..., Times.Never()) für TrySave.
        throw new NotImplementedException();
    }







    [TestMethod]
    [Ignore("Fortgeschrittene: ReserveBatch zuerst implementieren")]
    public void ReserveBatch_WhenCombinedSeatsExceedCapacity_DoesNotSaveOrSend()
    {
        // Arrange: Zwei Reservierungen für dieselbe Veranstaltung, die einzeln passen.
        // Act:     Nach Implementierung des Batch-Verfahrens beide zusammen reservieren.
        // Assert:  Ergebnis ist false; kein Batch-Speichern und keine Bestätigung.
        throw new NotImplementedException();
    }







    private sealed class SeatAvailabilityStub : ISeatAvailability
    {
        private readonly int _availableSeats; // speichert Anzahl der verfügbaren Plätze, die der Stub zurückgeben soll
        public SeatAvailabilityStub(int availableSeats)
        {
            _availableSeats = availableSeats;
        }
        public int GetAvailableSeats(string eventCode)
        {
            return _availableSeats;
        }
    }

    private sealed class ReservationRepositoryStub : IReservationRepository
    {
        public int SaveCalls { get; private set; }

        public bool TrySave(Reservation res)
        {
            SaveCalls++;
            return true; // Immer erfolgreich speichern.
        }
    }
    private sealed class ConfirmationSenderSpy : IConfirmationSender
    {
        public bool WasCalled { get; private set; } = false;
        public void Send(Reservation res)
        {
            WasCalled = true; // Aufruf aufzeichnen.
        }
    }
}
