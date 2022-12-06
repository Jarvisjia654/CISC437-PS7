using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SNICKERS.EF;
using SNICKERS.EF.Models;
using SNICKERS.Server.Models;
using SNICKERS.Shared;
using SNICKERS.Shared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Telerik.DataSource;
using Telerik.DataSource.Extensions;
using SNICKERS.EF.Data;
using SNICKERS.Shared.Utils;
using SNICKERS.Shared.Errors;
using System.Text.Json;
//using static Duende.IdentityServer.Models.IdentityResources;

namespace SNICKERS.Server.Controllers.Schools
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController: Controller
    {
        protected readonly SNICKERSOracleContext _context;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly OraTransMsgs _OraTranslateMsgs;

        public StudentController(SNICKERSOracleContext context,
            IHttpContextAccessor httpContextAccessor,
             OraTransMsgs OraTranslateMsgs)
        {
            this._context = context;
            this._httpContextAccessor = httpContextAccessor;
            this._OraTranslateMsgs = OraTranslateMsgs;
        }

        [HttpGet]
        [Route("GetStudents")]
        public async Task<IActionResult> GetStudents()
        {
            try
            {
                List<StudentDTO> lstStudents = await _context.Students.OrderBy(x => x.LastName)
                   .Select(sp => new StudentDTO
                   {
                       StudentId = sp.StudentId,
                       Salutation = sp.Salutation,
                       FirstName = sp.FirstName,
                       LastName = sp.LastName,
                       StreetAddress = sp.StreetAddress,
                       Zip = sp.Zip,
                       Phone = sp.Phone,
                       Employer = sp.Employer,
                       RegistrationDate = sp.RegistrationDate,
                       ModifiedBy = sp.ModifiedBy,
                       ModifiedDate = sp.ModifiedDate,
                       CreatedBy = sp.CreatedBy,
                       CreatedDate = sp.CreatedDate,
                       SchoolId = sp.SchoolId
                   }).ToListAsync();

                return Ok(lstStudents);
            }
            catch (DbUpdateException Dex)
            {
                List<OraError> DBErrors = ErrorHandling.TryDecodeDbUpdateException(Dex, _OraTranslateMsgs);
                return StatusCode(StatusCodes.Status417ExpectationFailed, Newtonsoft.Json.JsonConvert.SerializeObject(DBErrors));
            }
            catch (Exception ex)
            {
                _context.Database.RollbackTransaction();
                List<OraError> errors = new List<OraError>();
                errors.Add(new OraError(1, ex.Message.ToString()));
                string ex_ser = Newtonsoft.Json.JsonConvert.SerializeObject(errors);
                return StatusCode(StatusCodes.Status417ExpectationFailed, ex_ser);
            }
        }

        [HttpGet]
        [Route("GetStudents/{pStudentId}")]
        public async Task<IActionResult> GetStudents(int pStudentId)
        {
            try
            {

                StudentDTO itmStudent = await _context.Students
                    .Where(x => x.StudentId == pStudentId)
                    .OrderBy(x => x.StudentId)
                   .Select(sp => new StudentDTO
                   {
                       StudentId = sp.StudentId,
                       Salutation = sp.Salutation,
                       FirstName = sp.FirstName,
                       LastName = sp.LastName,
                       StreetAddress = sp.StreetAddress,
                       Zip = sp.Zip,
                       Phone = sp.Phone,
                       Employer = sp.Employer,
                       RegistrationDate = sp.RegistrationDate,
                       ModifiedBy = sp.ModifiedBy,
                       ModifiedDate = sp.ModifiedDate,
                       CreatedBy = sp.CreatedBy,
                       CreatedDate = sp.CreatedDate,
                       SchoolId = sp.SchoolId
                   }).FirstOrDefaultAsync();

                return Ok(itmStudent);
            }
            catch (DbUpdateException Dex)
            {
                List<OraError> DBErrors = ErrorHandling.TryDecodeDbUpdateException(Dex, _OraTranslateMsgs);
                return StatusCode(StatusCodes.Status417ExpectationFailed, Newtonsoft.Json.JsonConvert.SerializeObject(DBErrors));
            }
            catch (Exception ex)
            {
                _context.Database.RollbackTransaction();
                List<OraError> errors = new List<OraError>();
                errors.Add(new OraError(1, ex.Message.ToString()));
                string ex_ser = Newtonsoft.Json.JsonConvert.SerializeObject(errors);
                return StatusCode(StatusCodes.Status417ExpectationFailed, ex_ser);
            }
        }


        [HttpPost]
        [Route("PostStudent")]
        public async Task<IActionResult> PostStudent([FromBody] string _StudentDTO_String)
        {

            try
            {
                StudentDTO _StudentDTO = JsonSerializer.Deserialize<StudentDTO>(_StudentDTO_String);
                await this.PostStudent(_StudentDTO);
            }
            catch (Exception)
            {
                throw;
            }

            return Ok();
        }




        [HttpPost]
        public async Task<IActionResult> PostStudent([FromBody] StudentDTO _StudentDTO)
        {
            try
            {
                var trans = await _context.Database.BeginTransactionAsync();
                Student c = new Student
                {
                    StudentId = _StudentDTO.StudentId,
                    Salutation = _StudentDTO.Salutation,
                    FirstName = _StudentDTO.FirstName,
                    LastName = _StudentDTO.LastName,
                    StreetAddress = _StudentDTO.StreetAddress,
                    Zip = _StudentDTO.Zip,
                    Phone = _StudentDTO.Phone,
                    Employer = _StudentDTO.Employer,
                    RegistrationDate = _StudentDTO.RegistrationDate,
                    SchoolId = _StudentDTO.SchoolId
                };
                _context.Students.Add(c);
                await _context.SaveChangesAsync();
                await _context.Database.CommitTransactionAsync();
                return Ok();

            }
            catch (DbUpdateException Dex)
            {
                List<OraError> DBErrors = ErrorHandling.TryDecodeDbUpdateException(Dex, _OraTranslateMsgs);
                return StatusCode(StatusCodes.Status417ExpectationFailed, Newtonsoft.Json.JsonConvert.SerializeObject(DBErrors));
            }
            catch (Exception ex)
            {
                _context.Database.RollbackTransaction();
                List<OraError> errors = new List<OraError>();
                errors.Add(new OraError(1, ex.Message.ToString()));
                string ex_ser = Newtonsoft.Json.JsonConvert.SerializeObject(errors);
                return StatusCode(StatusCodes.Status417ExpectationFailed, ex_ser);
            }
        }

        [HttpPut]
        public async Task<IActionResult> PutStudent(StudentDTO _StudentDTO)
        {

            try
            {
                var trans = await _context.Database.BeginTransactionAsync();
                Student c = await _context.Students.Where(x => x.StudentId.Equals(_StudentDTO.StudentId)).FirstOrDefaultAsync();

                if (c != null)
                {
                    c.StudentId = _StudentDTO.StudentId;
                    c.Salutation = _StudentDTO.Salutation;
                    c.FirstName = _StudentDTO.FirstName;
                    c.LastName = _StudentDTO.LastName;
                    c.StreetAddress = _StudentDTO.StreetAddress;
                    c.Zip = _StudentDTO.Zip;
                    c.Phone = _StudentDTO.Phone;
                    c.Employer = _StudentDTO.Employer;
                    c.RegistrationDate = _StudentDTO.RegistrationDate;
                    c.SchoolId = _StudentDTO.SchoolId;

                    _context.Students.Update(c);
                    await _context.SaveChangesAsync();
                    await _context.Database.CommitTransactionAsync();
                }
            }


            catch (DbUpdateException Dex)
            {
                List<OraError> DBErrors = ErrorHandling.TryDecodeDbUpdateException(Dex, _OraTranslateMsgs);
                return StatusCode(StatusCodes.Status417ExpectationFailed, Newtonsoft.Json.JsonConvert.SerializeObject(DBErrors));
            }
            catch (Exception ex)
            {
                _context.Database.RollbackTransaction();
                List<OraError> errors = new List<OraError>();
                errors.Add(new OraError(1, ex.Message.ToString()));
                string ex_ser = Newtonsoft.Json.JsonConvert.SerializeObject(errors);
                return StatusCode(StatusCodes.Status417ExpectationFailed, ex_ser);
            }



            return Ok();
        }

        [HttpDelete]
        [Route("DeleteStudent/{pStudentId}")]
        public async Task<IActionResult> DeleteStudent(int pStudentId)
        {

            try
            {


                var trans = await _context.Database.BeginTransactionAsync();
                Student c = await _context.Students.Where(x => x.StudentId.Equals(pStudentId)).FirstOrDefaultAsync();
                _context.Students.Remove(c);

                await _context.SaveChangesAsync();
                await _context.Database.CommitTransactionAsync();
            }
            catch (DbUpdateException Dex)
            {
                List<OraError> DBErrors = ErrorHandling.TryDecodeDbUpdateException(Dex, _OraTranslateMsgs);
                return StatusCode(StatusCodes.Status417ExpectationFailed, Newtonsoft.Json.JsonConvert.SerializeObject(DBErrors));
            }
            catch (Exception ex)
            {
                _context.Database.RollbackTransaction();
                List<OraError> errors = new List<OraError>();
                errors.Add(new OraError(1, ex.Message.ToString()));
                string ex_ser = Newtonsoft.Json.JsonConvert.SerializeObject(errors);
                return StatusCode(StatusCodes.Status417ExpectationFailed, ex_ser);
            }
            return Ok();
        }

        [HttpPost]
        [Route("GetStudents")]
        public async Task<DataEnvelope<StudentDTO>> GetStudentsPost([FromBody] DataSourceRequest gridRequest)
        {
            DataEnvelope<StudentDTO> dataToReturn = null;
            IQueryable<StudentDTO> queriableStates = _context.Students
                    .Select(sp => new StudentDTO
                    {
                        StudentId = sp.StudentId,
                        Salutation = sp.Salutation,
                        FirstName = sp.FirstName,
                        LastName = sp.LastName,
                        StreetAddress = sp.StreetAddress,
                        Zip = sp.Zip,
                        Phone = sp.Phone,
                        Employer = sp.Employer,
                        RegistrationDate = sp.RegistrationDate,
                        ModifiedBy = sp.ModifiedBy,
                        ModifiedDate = sp.ModifiedDate,
                        CreatedBy = sp.CreatedBy,
                        CreatedDate = sp.CreatedDate,
                        SchoolId = sp.SchoolId
                    });

            // use the Telerik DataSource Extensions to perform the query on the data
            // the Telerik extension methods can also work on "regular" collections like List<T> and IQueriable<T>
            try
            {

                DataSourceResult processedData = await queriableStates.ToDataSourceResultAsync(gridRequest);

                if (gridRequest.Groups.Count > 0)
                {
                    // If there is grouping, use the field for grouped data
                    // The app must be able to serialize and deserialize it
                    // Example helper methods for this are available in this project
                    // See the GroupDataHelper.DeserializeGroups and JsonExtensions.Deserialize methods
                    dataToReturn = new DataEnvelope<StudentDTO>
                    {
                        GroupedData = processedData.Data.Cast<AggregateFunctionsGroup>().ToList(),
                        TotalItemCount = processedData.Total
                    };
                }
                else
                {
                    // When there is no grouping, the simplistic approach of 
                    // just serializing and deserializing the flat data is enough
                    dataToReturn = new DataEnvelope<StudentDTO>
                    {
                        CurrentPageData = processedData.Data.Cast<StudentDTO>().ToList(),
                        TotalItemCount = processedData.Total
                    };
                }
            }
            catch (Exception e)
            {
                //fixme add decent exception handling
            }
            return dataToReturn;
        }
    }
}

