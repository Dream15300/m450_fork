using Moq;

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




    // TODO: Grenzfall testen
    [TestMethod]
    public void Reserve_WithExactlyEnoughSeats_Succeeds()
    {
        // Arrange: Stub mit genau 2 freien Plätzen.
        var availabilityStub = new SeatAvailabilityStub(2);
        var repositoryStub = new ReservationRepositoryStub();
        var confirmationSenderSpy = new ConfirmationSenderSpy();
        var reservationService = new ReservationService(availabilityStub, repositoryStub, confirmationSenderSpy);

        // Act:     2 Plätze reservieren.
        var result = reservationService.Reserve("ivan@test.ch", "EVENT123", 2);

        // Assert:  Ergebnis ist true.
        Assert.IsTrue(result, "Reservation sollte gelingen, wenn genau genügend Plätze verfügbar sind.");
    }





    // TODO: Platzanzahl 0 testen
    [TestMethod]
    public void Reserve_WithZeroSeats_ThrowsBeforeCallingDependencies()
    {
        // Arrange: Doubles, deren Aufrufe sich prüfen lassen.
        var availabilityStub = new SeatAvailabilityStub(5);
        var repositoryStub = new ReservationRepositoryStub();
        var confirmationSenderSpy = new ConfirmationSenderSpy();
        var reservationService = new ReservationService(availabilityStub, repositoryStub, confirmationSenderSpy);

        // Act + Assert: 0 Plätze reservieren. ArgumentOutOfRangeException; keine Abhängigkeit wurde aufgerufen.
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => reservationService.Reserve("ivan@test.ch", "EVENT123", 0));
    }





    // TODO: negative Platzanzahl testen
    [TestMethod]
    public void Reserve_WithNegativeSeats_ThrowsBeforeCallingDependencies()
    {
        // Arrange: Doubles, deren Aufrufe sich prüfen lassen.
        var availabilityStub = new SeatAvailabilityStub(5);
        var repositoryStub = new ReservationRepositoryStub();
        var confirmationSenderSpy = new ConfirmationSenderSpy();
        var reservationService = new ReservationService(availabilityStub, repositoryStub, confirmationSenderSpy);

        // Act + Assert: -1 Plätze reservieren. ArgumentOutOfRangeException; keine Abhängigkeit wurde aufgerufen.
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => reservationService.Reserve("ivan@test.ch", "EVENT123", -1));
    }





    // TODO: leeren Veranstaltungscode testen
    [TestMethod]
    public void Reserve_WithEmptyEventCode_ThrowsBeforeCallingDependencies()
    {
        // Arrange: Doubles, deren Aufrufe sich prüfen lassen.
        var availabilityStub = new SeatAvailabilityStub(5);
        var repositoryStub = new ReservationRepositoryStub();
        var confirmationSenderSpy = new ConfirmationSenderSpy();
        var reservationService = new ReservationService(availabilityStub, repositoryStub, confirmationSenderSpy);

        // Act + Assert: Mit leerem Veranstaltungscode reservieren. ArgumentException; keine Abhängigkeit wurde aufgerufen.
        var exception = Assert.Throws<ArgumentException>(() => reservationService.Reserve("ivan@test.ch", "", 2));
    }





    // TODO: E-Mail-Adresse aus Leerzeichen testen
    [TestMethod]
    public void Reserve_WithWhitespaceEmail_ThrowsBeforeCallingDependencies()
    {
        // Arrange: Handgeschriebene Doubles oder Moq gezielt auswählen.
        var availabilityStub = new SeatAvailabilityStub(5);
        var repositoryStub = new ReservationRepositoryStub();
        var confirmationSenderSpy = new ConfirmationSenderSpy();
        var reservationService = new ReservationService(availabilityStub, repositoryStub, confirmationSenderSpy);

        // Act + Assert: Mit einer E-Mail-Adresse aus Leerzeichen reservieren. ArgumentException; keine Abhängigkeit wurde aufgerufen.
        var exception = Assert.Throws<ArgumentException>(() => reservationService.Reserve(" ", "EVENT123", 2));
    }





    // TODO: fehlgeschlagenes Speichern testen
    [TestMethod]
    public void Reserve_WhenSavingFails_DoesNotSendConfirmation()
    {
        // Arrange: Stub mit genügend Plätzen, Repository liefert false, eigener Spy.
        var availabilityStub = new SeatAvailabilityStub(5);
        var repositoryStub = new FailingRepositoryStub();
        var confirmationSenderSpy = new ConfirmationSenderSpy();
        var reservationService = new ReservationService(availabilityStub, repositoryStub, confirmationSenderSpy);

        // Act: Reservierung versuchen. 
        var result = reservationService.Reserve("ivan@test.ch", "EVENT123", 2);

        // Assert: Ergebnis ist false und der Spy hat keinen Aufruf aufgezeichnet.
        Assert.IsFalse(result, "Reservation sollte fehlschlagen, wenn das Speichern fehlschlägt.");
        Assert.IsFalse(confirmationSenderSpy.WasCalled, "Bestätigung sollte nicht gesendet werden, wenn das Speichern fehlschlägt.");
    }





    // TODO: Moq für fehlenden Speicherversuch verwenden
    [TestMethod]
    public void Reserve_WithTooFewSeats_DoesNotCallRepository()
    {
        // Arrange: Stub mit zu wenigen Plätzen und Mock<IReservationRepository>.
        var availabilityStub = new SeatAvailabilityStub(1);
        var repositoryMock = new Mock<IReservationRepository>(); // Moq erstellt ein Testobjekt für das Repository, benötigt keine eigene Klasse
        var confirmationSenderSpy = new ConfirmationSenderSpy();
        var reservationService = new ReservationService(availabilityStub, repositoryMock.Object, confirmationSenderSpy);

        // Act:     Reservierung versuchen.
        var result = reservationService.Reserve("ivan@test.ch", "EVENT123", 2);

        // Assert:  Verify(..., Times.Never()) für TrySave.
        repositoryMock.Verify(r => r.TrySave(It.IsAny<Reservation>()), Times.Never()); // .Verify prüft Methode, TrySave zu überprüfende Methode, It.IsAny<Reservation>() akzeptiert jedes Argument vom Typ Reservation, Times.Never() erwartet, dass die Methode nie aufgerufen wurde.
    }






    // Fortgeschrittene: ReserveBatch zuerst implementieren, erweitert Reservation mit Gruppenreservierungen.
    [TestMethod]
    public void ReserveBatch_WhenCombinedSeatsExceedCapacity_DoesNotSaveOrSend()
    {
        // Arrange: Zwei Reservierungen für dieselbe Veranstaltung, die einzeln passen.
        var availabilityStub = new SeatAvailabilityStub(3);
        var repositoryStub = new ReservationRepositoryStub();
        var confirmationSenderSpy = new ConfirmationSenderSpy();
        var reservationService = new ReservationService(availabilityStub, repositoryStub, confirmationSenderSpy);
        Reservation[] requests = 
            [
                new("ivan@test.ch", "EVENT123", 2),
                new("ilai@test.ch", "EVENT123", 2)
            ];

        // Act: Nach Implementierung des Batch-Verfahrens beide zusammen reservieren.
        var result = reservationService.ReserveBatch(requests); // ReserveBatch versucht beide Reservierungen zusammen zu speichern, wenn die kombinierte Anzahl der Plätze die Kapazität überschreitet, sollte es fehlschlagen.


        // Assert:  Ergebnis ist false; kein Batch-Speichern und keine Bestätigung.
        Assert.IsFalse(result, "Batch-Reservation sollte fehlschlagen, wenn die kombinierte Anzahl der Plätze die Kapazität überschreitet.");
        Assert.IsFalse(confirmationSenderSpy.WasCalled, "Bestätigung sollte nicht gesendet werden, wenn die Batch-Reservation fehlschlägt.");
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
    // Für Aufgabe "fehlgeschlagenes Speichern"
    private sealed class FailingRepositoryStub : IReservationRepository
    {
        public bool TrySave(Reservation reservation)
        {
            return false;
        }
    }

}
