using Dpa.Library.Services;
using Moq;

namespace DailyPoetryA.UnitTest.Services;

public class JinrishiciServiceTest
{
    [Fact(Skip = "依赖远程服务")]
    public async Task GetTokenAsync_ReturnIsNotNullOrWhiteSpace()
    {
        // Arrange
        var alertServiceMock = new Mock<IAlertService>();
        var mockAlertService = alertServiceMock.Object;

        var preferenceServiceMock = new Mock<IPreferenceStorage>();
        var mockPreferenceService = preferenceServiceMock.Object;
        
        var poetryStorageMock = new Mock<IPoetryStorage>();
        var mockPoetryStorage = poetryStorageMock.Object;

        var jinrishiciService = new JinrishiciService(mockAlertService, 
            mockPreferenceService, 
            mockPoetryStorage);

        // Act
        var data = await jinrishiciService.GetTokenAsync();

        // Assert 
        Assert.False(string.IsNullOrWhiteSpace(data));
        alertServiceMock.Verify(p=>p.AlertAsync(
            It.IsAny<string>(), It.IsAny<string>()), 
            Times.Never);
        preferenceServiceMock.Verify(p=>p.Get(
            JinrishiciService.JinrishiciTokenKey, string.Empty), 
            Times.Once);
        preferenceServiceMock.Verify(p=>p.Set(
            JinrishiciService.JinrishiciTokenKey, data), 
            Times.Once);
    }

    [Fact]
    public async Task GetTokenAsync_NetworkError()
    {
        // Arrange
        var alertServiceMock = new Mock<IAlertService>();
        var mockAlertService = alertServiceMock.Object;
        var preferenceServiceMock = new Mock<IPreferenceStorage>();
        var mockPreferenceService = preferenceServiceMock.Object;
        var poetryStorageMock = new Mock<IPoetryStorage>();
        var mockPoetryStorage = poetryStorageMock.Object;
        var jinrishiciService = new JinrishiciService(mockAlertService, 
            mockPreferenceService, 
            mockPoetryStorage,
            "https://www.baidu.com");

        // Act
        var data = await jinrishiciService.GetTokenAsync();

        // Assert 
        Assert.True(string.IsNullOrWhiteSpace(data));
        alertServiceMock.Verify(p => p.AlertAsync(
                It.IsAny<string>(), It.IsAny<string>()),
            Times.Once);
        preferenceServiceMock.Verify(
            p => p.Get(JinrishiciService.JinrishiciTokenKey, string.Empty),
            Times.Once);
        preferenceServiceMock.Verify(
            p => p.Set(It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact(Skip = "依赖远程服务")]
    public async Task GetTodayPoetryAsync_ReturnFromJinrishici()
    {
        // Arrange
        var alertServiceMock = new Mock<IAlertService>();
        var mockAlertService = alertServiceMock.Object;
        var preferenceServiceMock = new Mock<IPreferenceStorage>();
        var mockPreferenceService = preferenceServiceMock.Object;
        var poetryStorageMock = new Mock<IPoetryStorage>();
        var mockPoetryStorage = poetryStorageMock.Object;
        var jinrishiciService = new JinrishiciService(mockAlertService,
            mockPreferenceService,
            mockPoetryStorage);

        // Act
        var todayPoetry = await jinrishiciService.GetTodayPoetryAsync();

        // Assert 
        Assert.Equal(TodayPoetrySources.Jinrishici, todayPoetry.Source);
        Assert.False(string.IsNullOrWhiteSpace(todayPoetry.Snippet));
        alertServiceMock.Verify(
            p=>p.AlertAsync(It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
        preferenceServiceMock.Verify(
            p=>p.Get(JinrishiciService.JinrishiciTokenKey, string.Empty),
            Times.Once);
        preferenceServiceMock.Verify(
            p => p.Set(JinrishiciService.JinrishiciTokenKey, It.IsAny<string>()),
            Times.Once);
    }
}