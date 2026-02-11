using Moq;
using Settlr.Common.Messages;
using Settlr.Data.IRepositories;
using Settlr.Models.Dtos.RequestDtos;
using Settlr.Models.Entities;
using Settlr.Services.Services;
using Xunit;

namespace Settlr.Tests;

public class GroupServiceTests
{
    private readonly Mock<IGroupRepository> _groupRepositoryMock;
    private readonly Mock<IGroupMemberRepository> _groupMemberRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly GroupService _groupService;

    public GroupServiceTests()
    {
        _groupRepositoryMock = new Mock<IGroupRepository>();
        _groupMemberRepositoryMock = new Mock<IGroupMemberRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _groupService = new GroupService(
            _groupRepositoryMock.Object,
            _groupMemberRepositoryMock.Object,
            _userRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateGroupAsync_ShouldReturnSuccess()
    {
        // Arrange
        int userId = 1;
        var request = new CreateGroupRequestDto { Name = "Test Group" };
        var group = new Group { Id = 1, Name = request.Name, CreatedById = userId, CreatedBy = new User { Name = "Creator" } };
        group.Members = new List<GroupMember> { new GroupMember { GroupId = 1, UserId = userId, User = group.CreatedBy } };

        _groupRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Group>()))
            .Callback<Group>(g => g.Id = 1)
            .Returns(Task.CompletedTask);
        _groupRepositoryMock.Setup(repo => repo.GetGroupWithMembersAsync(1))
            .ReturnsAsync(group);

        // Act
        var result = await _groupService.CreateGroupAsync(userId, request);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(Messages.GroupCreatedSuccessfully, result.Message);
        Assert.Equal(request.Name, result.Data.Name);
        _groupRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Group>()), Times.Once);
        _groupMemberRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<GroupMember>()), Times.Once);
    }

    [Fact]
    public async Task AddMemberAsync_ShouldReturnSuccess_WhenAllConditionsMet()
    {
        // Arrange
        int currentUserId = 1;
        var request = new AddGroupMemberRequestDto { GroupId = 1, UserEmail = "new@example.com" };
        var group = new Group { Id = 1, Name = "Test Group", CreatedBy = new User { Name = "Creator" } };
        var userToAdd = new User { Id = 2, Email = request.UserEmail, Name = "New User" };
        group.Members = new List<GroupMember>();

        _groupRepositoryMock.Setup(repo => repo.GetGroupWithMembersAsync(request.GroupId))
            .ReturnsAsync(group);
        _groupMemberRepositoryMock.Setup(repo => repo.IsUserMemberOfGroupAsync(currentUserId, request.GroupId))
            .ReturnsAsync(true);
        _userRepositoryMock.Setup(repo => repo.GetByEmailAsync(request.UserEmail))
            .ReturnsAsync(userToAdd);
        _groupMemberRepositoryMock.Setup(repo => repo.IsUserMemberOfGroupAsync(userToAdd.Id, request.GroupId))
            .ReturnsAsync(false);

        // Act
        var result = await _groupService.AddMemberAsync(currentUserId, request);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(Messages.MemberAddedSuccessfully, result.Message);
        _groupMemberRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<GroupMember>()), Times.Once);
    }

    [Fact]
    public async Task AddMemberAsync_ShouldReturnFail_WhenGroupNotFound()
    {
        // Arrange
        _groupRepositoryMock.Setup(repo => repo.GetGroupWithMembersAsync(It.IsAny<int>()))
            .ReturnsAsync((Group?)null);

        // Act
        var result = await _groupService.AddMemberAsync(1, new AddGroupMemberRequestDto { GroupId = 99 });

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(Messages.GroupNotFound, result.Message);
    }

    [Fact]
    public async Task AddMemberAsync_ShouldReturnFail_WhenCallerIsNotMember()
    {
        // Arrange
        var group = new Group { Id = 1 };
        _groupRepositoryMock.Setup(repo => repo.GetGroupWithMembersAsync(1))
            .ReturnsAsync(group);
        _groupMemberRepositoryMock.Setup(repo => repo.IsUserMemberOfGroupAsync(1, 1))
            .ReturnsAsync(false);

        // Act
        var result = await _groupService.AddMemberAsync(1, new AddGroupMemberRequestDto { GroupId = 1 });

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(Messages.UserNotMemberOfGroup, result.Message);
    }
}
