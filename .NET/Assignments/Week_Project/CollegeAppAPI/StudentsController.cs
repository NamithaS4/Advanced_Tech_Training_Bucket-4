internal class StudentsController
{
    private IStudentRepository @object;

    // Change return type from Task to Task<ActionResult<Student>>
    internal async Task<ActionResult<Student>> GetStudentById(int id)
    {
        var student = await @object.GetByIdAsync(id);
        if (student == null)
        {
            return new NotFoundResult();
        }
        return new ActionResult<Student>(student);
    }
}