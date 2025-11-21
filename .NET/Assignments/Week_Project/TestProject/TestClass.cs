using CollegeAppAPI.Models;
using CollegeAppAPI.Repositories;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject
{
    [TestClass]
    public class TestClass
    {
        

        [TestMethod]

        public async Task TestgetstudentsbyidAsync()
        {
            var student = new Student
            {
                StudentId = 1,
                Name = "Namii S",
                Email = "nami@gmail.com",
                Gender = "Female"
            };
            var studentRepo = new Mock<IStudentRepository>();
            studentRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(student);

            var controller = new CollegeAppAPI.Controllers.StudentsController(studentRepo.Object);
            var getstudentbyid = await controller.GetStudentById(1); 

            Assert.IsNotNull(getstudentbyid);
            var resultstudent = getstudentbyid.Value;
        }
    }
}
