using College_App.Data;
using College_App.Model;
using College_App.MyLogger;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace College_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   
    public class CollegeApp : ControllerBase
    {
        private readonly IMyLogger _mylogger;
        private readonly CollegeDBContext _dbcontext;

        public CollegeApp(IMyLogger Mylogger, CollegeDBContext dbcontext)
        {
            _mylogger = Mylogger;
            _dbcontext = dbcontext;
        }

/*
        //[HttpGet]
        ////Route("All")]
        //public IEnumerable<Student> getstudents()
        //{
        //    return collegeRepository.students;
        //}
        [HttpPatch]
        [Route("{id:int}/UpdatePartial")]

        //public ActionResult UpdateStudentPartial(int id, [FromBody] JsonPatchDocument<studentDTO> patchDocument)
        //{
        //    if(patchDocument == null || id <= 0)
        //    {
        //        return BadRequest();
        //    }
        //    var exsistingStudent = collegeRepository.students.Where(s => s.studentId == id);
        //}
        [HttpGet]
        [Route("All")]
        public ActionResult<IEnumerable<studentDTO>> getstudents()
        {
            var students = collegeRepository.students.Select(n => new studentDTO()
            {
                studentId = n.studentId,
                name = n.name,
                age = n.age,
                email = n.email,
                password = "",
                confirmPassword = "",
            });
            return Ok(students);
        }

        [HttpPut]

        public ActionResult updateStudent([FromBody] studentDTO Model)
        {
            if (Model == null)
                return BadRequest();

            var existingStudent = collegeRepository.students.Where(s => s.studentId == Model.studentId)
                .FirstOrDefault();

            if(existingStudent == null)
            {
                return NotFound($"The student with id {Model.studentId} not found");
            }

            existingStudent.name = Model.name;
            existingStudent.age = Model.age;
            existingStudent.email = Model.email;

            return Ok(existingStudent);
        }
        //[HttpGet("{id:Int}", Name = "getstudentsbyid")] //This is an Attribute routing

        //public Student getstudentbyid(int id)
        //{
        //    return collegeRepository.students.
        //        Where(n => n.studentId == id).
        //        FirstOrDefault();
        //}

        //[HttpGet] //"[HttpGet({Name:Alpha}", Name = "getstudentsbyname")] we can use this also for attribute routing
        //[Route("getstudentsbyname")] //Nrml routing

        //public Student getstudentsbyname(string Name)
        //{
        //    return collegeRepository.students.
        //        Where(n => n.name == Name).
        //        FirstOrDefault();
        //}

        //[HttpDelete]
        //public bool deletestudent(int id)
        //{
        //    var deleting = collegeRepository.students.
        //        Where(n => n.studentId == id)
        //        .FirstOrDefault();
        //    collegeRepository.students.Remove(deleting);
        //    return true;
        //}

        [HttpGet("{id:Int}", Name = "getstudentsbyid")]
        [ProducesResponseType(200)]
        public ActionResult<studentDTO> getstudentbyidaction(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var student = collegeRepository.students.
                Where(n => n.studentId == id).
                FirstOrDefault();
            var studentDTO = new studentDTO()
            {
                studentId = student.studentId,
                name = student.name,
                age = student.age,
                email = student.email,
                password = "",
                confirmPassword = ""
            };
            if (id == null)
            {
                return NotFound($"Not found{id}");
            }
            return Ok(student);
        }
        [HttpPost("Create")]
        public ActionResult<studentDTO> CreateStudent([FromBody] studentDTO Model)
        {
            if (Model == null)
            {
                return BadRequest(Model);
            }
            int newid = collegeRepository.students.LastOrDefault().studentId+1;
            
            Student studentnew = new Student()
            {
                studentId = newid,
                name = Model.name,
                age = Model.age,
                email = Model.email,
                password = Model.password,
                confirmPassword = Model.confirmPassword,
            };
            collegeRepository.students.Add(studentnew);
            return Ok(Model);
        }


        [HttpGet("{Name:Alpha}", Name = "getstudentsbyname")]
        public ActionResult<Student> getstudentsbyname(string Name)
        {
            if(String.IsNullOrEmpty(Name))
            {
                return BadRequest();
            }
            var student = collegeRepository.students.
                Where(n => n.name == Name).
                FirstOrDefault();

            if(student == null)
            {
                return NotFound($"Not found {Name}");
            }
            return Ok(student);
        }*/
    }
}
