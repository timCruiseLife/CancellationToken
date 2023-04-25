using Example;
using Example.Controllers;
using Example.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TestUtilities;

namespace CIT.Example
{
    [TestClass]
    public class MessageBoardsControllerTests
    {
        private static ClusterSetupHelper? clusterSetupHelper;
        private IServiceProvider? serviceProvider;

        [ClassInitialize]
        public static void Init(TestContext testContext)
        {
            clusterSetupHelper = new ClusterSetupHelper(
                ExampleConstants.Tests.DbServer,
                ExampleConstants.Tests.User,
                ExampleConstants.Tests.Password);
            clusterSetupHelper.Setup();
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            Assert.IsNotNull(clusterSetupHelper);
            clusterSetupHelper.TearDown();
        }

        [TestInitialize]
        public void Init()
        {
            Assert.IsNotNull(clusterSetupHelper);
            this.serviceProvider = clusterSetupHelper.CreateServiceProvider(services =>
            {
            });
        }

        [TestMethod]
        public async Task TestCRUD()
        {
            Assert.IsNotNull(this.serviceProvider);
            CallContext callContext = CallContext.Default;
            var controller = ActivatorUtilities.CreateInstance<MessageBoardsController>(this.serviceProvider);
            controller.SetupMocks();
            var dbContext = this.serviceProvider?.GetService<ExamplesContext>();
            Assert.IsNotNull(dbContext);

            var badRequest = new MessageBoard
            {
                Name = string.Empty,
                Content = string.Empty
            };

            var post = await controller.Post(request: badRequest);
            post.AssertBadRequest();
            Assert.AreEqual(ExampleConstants.WebApiErrors.InvalidData, post.GetErrorResult().Error);

            badRequest = new MessageBoard
            {
                Name = "name1",
                Content = string.Empty
            };

            post = await controller.Post(request: badRequest);
            post.AssertBadRequest();
            Assert.AreEqual(ExampleConstants.WebApiErrors.InvalidData, post.GetErrorResult().Error);

            badRequest = new MessageBoard
            {
                Name = string.Empty,
                Content = "content1"
            };

            post = await controller.Post(request: badRequest);
            post.AssertBadRequest();
            Assert.AreEqual(ExampleConstants.WebApiErrors.InvalidData, post.GetErrorResult().Error);

            var messageBoard = new MessageBoard
            {
                Name = "name1",
                Content = "content1"
            };

            post = await controller.Post(request: messageBoard);
            post.AssertOk();

            var checkMessageBoards = await dbContext.MessageBoards.ToListAsync(callContext.CancellationToken);
            Assert.IsNotNull(checkMessageBoards);
            Assert.AreEqual(1, checkMessageBoards.Count);
            Assert.AreEqual(messageBoard.Name, checkMessageBoards.First().Name);
            Assert.AreEqual(messageBoard.Content, checkMessageBoards.First().Content);
            Assert.IsTrue(checkMessageBoards.First().CreateTime > 0);

            var get = await controller.Get();
            get.AssertOk();

            var getResults = get.GetResult().Result;
            Assert.IsNotNull(getResults);
            Assert.AreEqual(1, getResults.Count);
            Assert.AreEqual(getResults.First().Name, checkMessageBoards.First().Name);
            Assert.AreEqual(getResults.First().Content, checkMessageBoards.First().Content);

            var boardId_1 = checkMessageBoards.First().Id;
            var fakeId = boardId_1 - 1;

            messageBoard.Content = "content2";
            var put = await controller.Put(id: fakeId, request: messageBoard);
            put.AssertNotFound();
            Assert.AreEqual(ExampleConstants.WebApiErrors.ObjectNotFound, put.GetErrorResult().Error);

            put = await controller.Put(id: boardId_1, request: messageBoard);
            put.AssertOk();

            checkMessageBoards = await dbContext.MessageBoards.ToListAsync(callContext.CancellationToken);
            Assert.IsNotNull(checkMessageBoards);
            Assert.AreEqual(1, checkMessageBoards.Count);
            Assert.AreEqual(messageBoard.Name, checkMessageBoards.First().Name);
            Assert.AreEqual(messageBoard.Content, checkMessageBoards.First().Content);
            Assert.IsTrue(checkMessageBoards.First().CreateTime > 0);
            Assert.IsTrue(checkMessageBoards.First().UpdateTime > 0);

            var getOne = await controller.GetOne(id: fakeId);
            getOne.AssertNotFound();
            Assert.AreEqual(ExampleConstants.WebApiErrors.ObjectNotFound, getOne.GetErrorResult().Error);

            getOne = await controller.GetOne(id: boardId_1);
            getOne.AssertOk();
            var getOneResult = getOne.GetResult().Result;
            Assert.IsNotNull(getOneResult);
            Assert.AreEqual(messageBoard.Name, getOneResult.Name);
            Assert.AreEqual(messageBoard.Content, getOneResult.Content);

            messageBoard = new MessageBoard
            {
                Name = "name1",
                Content = "content2"
            };

            post = await controller.Post(request: messageBoard);
            post.AssertConflict();
            Assert.AreEqual(ExampleConstants.WebApiErrors.ObjectConflict, post.GetErrorResult().Error);

            messageBoard = new MessageBoard
            {
                Name = "name2",
                Content = "content3"
            };

            post = await controller.Post(request: messageBoard);
            post.AssertOk();

            checkMessageBoards = await dbContext.MessageBoards.ToListAsync(callContext.CancellationToken);
            Assert.IsNotNull(checkMessageBoards);
            Assert.AreEqual(2, checkMessageBoards.Count);

            var boardId_2 = checkMessageBoards.First(x => x.Name == messageBoard.Name).Id;
            get = await controller.Get(name: messageBoard.Name);
            get.AssertOk();

            getResults = get.GetResult().Result;
            Assert.IsNotNull(getResults);
            Assert.AreEqual(1, getResults.Count);
            Assert.AreEqual(boardId_2, getResults.First().Id);

            var delete = await controller.Delete(id: fakeId);
            delete.AssertNotFound();
            Assert.AreEqual(ExampleConstants.WebApiErrors.ObjectNotFound, delete.GetErrorResult().Error);

            delete = await controller.Delete(id: boardId_1);
            delete.AssertNoContent();

            checkMessageBoards = await dbContext.MessageBoards.ToListAsync(callContext.CancellationToken);
            Assert.IsNotNull(checkMessageBoards);
            Assert.AreEqual(1, checkMessageBoards.Count);
            Assert.AreNotEqual(boardId_1, checkMessageBoards.First().Id);

            getOne = await controller.GetOne(id: boardId_1);
            getOne.AssertNotFound();
            Assert.AreEqual(ExampleConstants.WebApiErrors.ObjectNotFound, getOne.GetErrorResult().Error);
        }
    }
}