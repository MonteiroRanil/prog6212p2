using System.IO;
using System.Security.Claims;
using System.Threading;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using WebApplication1.Controllers;
using WebApplication1.Data;
using WebApplication1.Models;
using Xunit;

namespace WebApplication1.Tests
{
    public class ClaimsControllerTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        private ClaimsController GetController(ApplicationDbContext context, string userId)
        {
            var mockUserManager = new Mock<UserManager<IdentityUser>>(
                Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);
            var mockEnv = new Mock<IWebHostEnvironment>();
            mockEnv.Setup(e => e.WebRootPath).Returns(Path.GetTempPath());

            var controller = new ClaimsController(context, mockUserManager.Object, mockEnv.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            return controller;
        }

        [Fact]
        public async Task CreateClaim_SetsTotalAmountCorrectly()
        {
            var context = GetInMemoryDbContext();
            var controller = GetController(context, "user1");

            await controller.Create(5, 100, "Test note", null);

            var claim = await context.LecturerClaims.FirstOrDefaultAsync();
            Assert.NotNull(claim);
            Assert.Equal(500, claim.TotalAmount);
        }

        [Fact]
        public async Task CreateClaim_SavesAdditionalNotes()
        {
            var context = GetInMemoryDbContext();
            var controller = GetController(context, "user1");

            await controller.Create(3, 50, "This is an additional note", null);

            var claim = await context.LecturerClaims.FirstOrDefaultAsync();
            Assert.NotNull(claim);
            Assert.Equal("This is an additional note", claim.Notes);
        }

        [Fact]
        public async Task CreateClaim_SavesSupportingDocument()
        {
            var context = GetInMemoryDbContext();
            var controller = GetController(context, "user1");

            var fileMock = new Mock<IFormFile>();
            var content = "Dummy file content";
            var fileName = "test.pdf";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;

            fileMock.Setup(f => f.OpenReadStream()).Returns(ms);
            fileMock.Setup(f => f.FileName).Returns(fileName);
            fileMock.Setup(f => f.Length).Returns(ms.Length);
            fileMock.Setup(f => f.ContentType).Returns("application/pdf");

            await controller.Create(2, 100, "With document", fileMock.Object);

            var document = await context.ClaimDocuments.FirstOrDefaultAsync();
            Assert.NotNull(document);
            Assert.Equal(fileName, document.FileName);
            Assert.Equal("application/pdf", document.ContentType);
        }
    }
}
