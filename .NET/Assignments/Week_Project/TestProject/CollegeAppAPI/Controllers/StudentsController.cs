using CollegeAppAPI.Repositories;

namespace CollegeAppAPI.Controllers
{
    internal class StudentsController
    {
        private IStudentRepository @object;

        public StudentsController(IStudentRepository @object)
        {
            this.@object = @object;
        }

        internal async Task GetStudentById(int v)
        {
            throw new NotImplementedException();
        }
    }
}