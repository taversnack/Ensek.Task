using Application.Models;
using Application.Models.DTO_s;
using Application.Repositories.Interfaces;
using Application.Services;
using Moq;

namespace Application.Tests
{
    public class MeterReadingTest
    {
        private Mock<ICsvRepository> _csvServiceMock;
        private Mock<IMeterReadingRepository> _repositoryMock;
        private MeterReadingService _service;

        [SetUp] // NUnit Setup Method
        public void SetUp()
        {
            _csvServiceMock = new Mock<ICsvRepository>();
            _repositoryMock = new Mock<IMeterReadingRepository>();
            _service = new MeterReadingService(_csvServiceMock.Object, _repositoryMock.Object);
        }


        [Test] // NUnit Test Method
        public async Task ProcessMeterReadingsAsync_AllValid_ReadingsProcessedSuccessfully()
        {
            // Arrange
            var csvStream = new MemoryStream(); // Mock or prepare a CSV stream with valid data
            var readings = new List<MeterReadingDto>
        {
            new MeterReadingDto { AccountId = "1", MeterReadingDateTime = "22/04/2019 09:24", MeterReadValue = "12345" }
        };

            _csvServiceMock.Setup(x => x.ParseMeterReadingsAsync(csvStream)).ReturnsAsync(readings);
            _repositoryMock.Setup(x => x.AccountExistsAsync(It.IsAny<string>())).ReturnsAsync(true);
            _repositoryMock.Setup(x => x.IsDuplicateReadingAsync(It.IsAny<MeterReadingDto>())).ReturnsAsync(false);
            _repositoryMock.Setup(x => x.IsLatestReading(It.IsAny<string>(), It.IsAny<DateTime>())).ReturnsAsync(true);
            _repositoryMock.Setup(x => x.AddMeterReadingAsync(It.IsAny<MeterReading>())).Returns(Task.CompletedTask);
            _repositoryMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _service.ProcessMeterReadingsAsync(csvStream);

            // Assert
            Assert.AreEqual(1, result.SuccessfulReadings);
            Assert.AreEqual(0, result.FailedReadings); // No failures
            Assert.AreEqual(0, result.InvalidReadings);
            Assert.AreEqual(0, result.AccountDoesNotExists);
            Assert.AreEqual(0, result.DuplicateEntry);
        }

        [Test]
        public async Task ProcessMeterReadingsAsync_RejectsOlderReadings()
        {
            // Arrange
            var csvStream = new MemoryStream();
            var readings = new List<MeterReadingDto>
            {
                new MeterReadingDto { AccountId = "1", MeterReadingDateTime = "22/04/2019 09:24", MeterReadValue = "12345" }
            };

            var csvServiceMock = _service;
            _csvServiceMock.Setup(x => x.ParseMeterReadingsAsync(csvStream)).ReturnsAsync(readings);

            var repositoryMock = new Mock<IMeterReadingRepository>();
            repositoryMock.Setup(x => x.AccountExistsAsync(It.IsAny<string>())).ReturnsAsync(true);
            repositoryMock.Setup(x => x.IsDuplicateReadingAsync(It.IsAny<MeterReadingDto>())).ReturnsAsync(false);
            repositoryMock.Setup(x => x.IsLatestReading(It.IsAny<string>(), It.IsAny<DateTime>())).ReturnsAsync(false); // Latest date is newer

            var service = _service;

            // Act
            var result = await service.ProcessMeterReadingsAsync(csvStream);

            // Assert
            Assert.AreEqual(0, result.SuccessfulReadings);
            Assert.AreEqual(1, result.FailedReadings);
        }


        [Test]
        public async Task ProcessMeterReadingsAsync_DuplicateReading_RejectsDuplicate()
        {
            // Arrange
            var csvStream = new MemoryStream();
            var readings = new List<MeterReadingDto>
    {
        new MeterReadingDto { AccountId = "1", MeterReadingDateTime = "22/04/2019 09:24", MeterReadValue = "12345",  }
    };

            var csvServiceMock = _csvServiceMock;
            csvServiceMock.Setup(x => x.ParseMeterReadingsAsync(csvStream)).ReturnsAsync(readings);

            var repositoryMock = new Mock<IMeterReadingRepository>();
            repositoryMock.Setup(x => x.AccountExistsAsync(It.IsAny<string>())).ReturnsAsync(true);
            repositoryMock.Setup(x => x.IsDuplicateReadingAsync(It.IsAny<MeterReadingDto>())).ReturnsAsync(true); // Mock duplicate

            var service = new MeterReadingService(csvServiceMock.Object, repositoryMock.Object);

            // Act
            var result = await service.ProcessMeterReadingsAsync(csvStream);

            // Assert
            Assert.AreEqual(0, result.SuccessfulReadings);
            Assert.AreEqual(1, result.DuplicateEntry);
            Assert.AreEqual(1, result.FailedReadings); // Includes duplicate
        }

        [Test]
        public async Task ProcessMeterReadingsAsync_InvalidReadingFormat_ThrowsValidationError()
        {
            // Arrange
            var csvStream = new MemoryStream();
            var readings = new List<MeterReadingDto>
            {
                new MeterReadingDto { AccountId = "1", MeterReadingDateTime = "22/04/2019 09:24", MeterReadValue = "12A45" }
            };

            var csvServiceMock = _csvServiceMock;
            csvServiceMock.Setup(x => x.ParseMeterReadingsAsync(csvStream)).ReturnsAsync(readings);

            var service = _service;

            // Act & Assert
            //Assert.ThrowsAsync<ValidationException>(async () => await service.ProcessMeterReadingsAsync(csvStream));
            var result = await service.ProcessMeterReadingsAsync(csvStream);

            Assert.AreEqual(0, result.SuccessfulReadings);
            Assert.AreEqual(1, result.FailedReadings);
            Assert.AreEqual(1, result.InvalidReadings);
        }


        [Test]
        public async Task ProcessMeterReadingsAsync_Duplicate()
        {
            // Arrange
            var csvStream = new MemoryStream();
            _repositoryMock.Setup(x => x.AccountExistsAsync(It.IsAny<string>())).ReturnsAsync(true);
            _repositoryMock.Setup(x => x.IsDuplicateReadingAsync(It.IsAny<MeterReadingDto>())).ReturnsAsync(true);
            var readings = new List<MeterReadingDto>
            {
                new MeterReadingDto { AccountId = "1237", MeterReadingDateTime = "22/04/2019 09:24", MeterReadValue = "12345" }
            };

            var csvServiceMock = _csvServiceMock;
            csvServiceMock.Setup(x => x.ParseMeterReadingsAsync(csvStream)).ReturnsAsync(readings);

            var service = _service;

            // Act & Assert
            //Assert.ThrowsAsync<ValidationException>(async () => await service.ProcessMeterReadingsAsync(csvStream));
            var result = await service.ProcessMeterReadingsAsync(csvStream);

            Assert.AreEqual(0, result.SuccessfulReadings);
            Assert.AreEqual(1, result.FailedReadings);
            Assert.AreEqual(1, result.DuplicateEntry);
        }


    }

}
