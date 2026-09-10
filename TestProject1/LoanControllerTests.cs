using Library;
using NSubstitute;
namespace LibraryTests
{
    [TestFixture]
    internal class LoanControllerTests
    {
        [Test]
        public void MakeLoan_ShouldDelegateToLibraryManager()
        {
            var mockManager = Substitute.For<LibraryManager>();
            var controller = new LoanController(mockManager);

            controller.MakeLoan(bookId: 10, memberId: 5);

            mockManager.Received(1).MakeLoan(10, 5);
        }
    }
}
