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
using static System.Collections.Specialized.BitVector32;

namespace SNICKERS.Server.Controllers.Schools
{
    public class EnrollmentController : Controller
    {
        protected readonly SNICKERSOracleContext _context;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly OraTransMsgs _OraTranslateMsgs;

        public EnrollmentController(SNICKERSOracleContext context,
            IHttpContextAccessor httpContextAccessor,
             OraTransMsgs OraTranslateMsgs)
        {
            this._context = context;
            this._httpContextAccessor = httpContextAccessor;
            this._OraTranslateMsgs = OraTranslateMsgs;
        }

        [HttpGet]
        [Route("GetEnrollments")]
        public async Task<IActionResult> GetEnrollments()
        {
            try
            {
                List<EnrollmentDTO> lstEnrollments = await _context.Enrollments.OrderBy(x => x.StudentId)
                   .Select(sp => new EnrollmentDTO
                   {
                       StudentId = sp.StudentId,
                       SectionId = sp.SectionId,
                       EnrollDate = sp.CreatedDate,
                       FinalGrade = sp.FinalGrade,
                       CreatedBy = sp.CreatedBy,
                       CreatedDate = sp.CreatedDate,
                       ModifiedBy = sp.ModifiedBy,
                       ModifiedDate = sp.ModifiedDate,
                       SchoolId = sp.SchoolId
                   }).ToListAsync();

                return Ok(lstEnrollments);
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
        [Route("GetEnrollments/{pStudentId}")]
        public async Task<IActionResult> GetEnrollments(int pStudentId)
        {
            try
            {

                EnrollmentDTO itmEnrollment = await _context.Enrollments
                    .Where(x => x.StudentId == pStudentId)
                    .OrderBy(x => x.StudentId)
                   .Select(sp => new EnrollmentDTO
                   {
                       StudentId = sp.StudentId,
                       SectionId = sp.SectionId,
                       EnrollDate = sp.CreatedDate,
                       FinalGrade = sp.FinalGrade,
                       CreatedBy = sp.CreatedBy,
                       CreatedDate = sp.CreatedDate,
                       ModifiedBy = sp.ModifiedBy,
                       ModifiedDate = sp.ModifiedDate,
                       SchoolId = sp.SchoolId
                   }).FirstOrDefaultAsync();

                return Ok(itmEnrollment);
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
        [Route("PostEnrollment")]
        public async Task<IActionResult> PostEnrollment([FromBody] string _EnrollmentDTO_String)
        {

            try
            {
                EnrollmentDTO _EnrollmentDTO = JsonSerializer.Deserialize<EnrollmentDTO>(_EnrollmentDTO_String);
                await this.PostEnrollment(_EnrollmentDTO);
            }
            catch (Exception)
            {
                throw;
            }

            return Ok();
        }




        [HttpPost]
        public async Task<IActionResult> PostEnrollment([FromBody] EnrollmentDTO _EnrollmentDTO)
        {
            try
            {
                var trans = await _context.Database.BeginTransactionAsync();
                Enrollment c = new Enrollment
                {
                    StudentId = _EnrollmentDTO.StudentId,
                    SectionId = _EnrollmentDTO.SectionId,
                    EnrollDate = _EnrollmentDTO.CreatedDate,
                    FinalGrade = _EnrollmentDTO.FinalGrade,
                    SchoolId = _EnrollmentDTO.SchoolId
                };
                _context.Enrollments.Add(c);
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
        public async Task<IActionResult> PutEnrollment(EnrollmentDTO _EnrollmentDTO)
        {

            try
            {
                var trans = await _context.Database.BeginTransactionAsync();
                Enrollment c = await _context.Enrollments.Where(x => x.StudentId.Equals(_EnrollmentDTO.StudentId)).FirstOrDefaultAsync();

                if (c != null)
                {
                    c.StudentId = _EnrollmentDTO.StudentId;
                    c.SectionId = _EnrollmentDTO.SectionId;
                    c.EnrollDate = _EnrollmentDTO.CreatedDate;
                    c.FinalGrade = _EnrollmentDTO.FinalGrade;
                    c.SchoolId = _EnrollmentDTO.SchoolId;

                    _context.Enrollments.Update(c);
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
        [Route("DeleteEnrollment/{pStudentId}")]
        public async Task<IActionResult> DeleteEnrollment(int pStudentId)
        {

            try
            {


                var trans = await _context.Database.BeginTransactionAsync();
                Enrollment c = await _context.Enrollments.Where(x => x.StudentId.Equals(pStudentId)).FirstOrDefaultAsync();
                _context.Enrollments.Remove(c);

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
        [Route("GetEnrollments")]
        public async Task<DataEnvelope<EnrollmentDTO>> GetEnrollmentsPost([FromBody] DataSourceRequest gridRequest)
        {
            DataEnvelope<EnrollmentDTO> dataToReturn = null;
            IQueryable<EnrollmentDTO> queriableStates = _context.Enrollments
                    .Select(sp => new EnrollmentDTO
                    {
                        StudentId = sp.StudentId,
                        SectionId = sp.SectionId,
                        EnrollDate = sp.CreatedDate,
                        FinalGrade = sp.FinalGrade,
                        CreatedBy = sp.CreatedBy,
                        CreatedDate = sp.CreatedDate,
                        ModifiedBy = sp.ModifiedBy,
                        ModifiedDate = sp.ModifiedDate,
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
                    dataToReturn = new DataEnvelope<EnrollmentDTO>
                    {
                        GroupedData = processedData.Data.Cast<AggregateFunctionsGroup>().ToList(),
                        TotalItemCount = processedData.Total
                    };
                }
                else
                {
                    // When there is no grouping, the simplistic approach of 
                    // just serializing and deserializing the flat data is enough
                    dataToReturn = new DataEnvelope<EnrollmentDTO>
                    {
                        CurrentPageData = processedData.Data.Cast<EnrollmentDTO>().ToList(),
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
