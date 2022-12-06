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
using System.Xml.Linq;
using static System.Collections.Specialized.BitVector32;

namespace SNICKERS.Server.Controllers.Schools
{
    public class GradeController : Controller
    {
        protected readonly SNICKERSOracleContext _context;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly OraTransMsgs _OraTranslateMsgs;

        public GradeController(SNICKERSOracleContext context,
            IHttpContextAccessor httpContextAccessor,
             OraTransMsgs OraTranslateMsgs)
        {
            this._context = context;
            this._httpContextAccessor = httpContextAccessor;
            this._OraTranslateMsgs = OraTranslateMsgs;
        }

        [HttpGet]
        [Route("GetGrades")]
        public async Task<IActionResult> GetGrades()
        {
            try
            {
                List<GradeDTO> lstGrades = await _context.Grades.OrderBy(x => x.StudentId)
                   .Select(sp => new GradeDTO
                   {
                       StudentId = sp.StudentId,
                       SectionId = sp.SectionId,
                       GradeTypeCode = sp.GradeTypeCode,
                       GradeCodeOccurrence = sp.GradeCodeOccurrence,
                       NumericGrade = sp.NumericGrade,
                       Comments = sp.Comments,
                       CreatedBy = sp.CreatedBy,
                       CreatedDate = sp.CreatedDate,
                       ModifiedBy = sp.ModifiedBy,
                       ModifiedDate = sp.ModifiedDate,
                       SchoolId = sp.SchoolId

                   }).ToListAsync();

                return Ok(lstGrades);
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
        [Route("GetGrades/{pStudentId}")]
        public async Task<IActionResult> GetGrades(int pStudentId)
        {
            try
            {

                GradeDTO itmGrade = await _context.Grades
                    .Where(x => x.StudentId == pStudentId)
                    .OrderBy(x => x.StudentId)
                   .Select(sp => new GradeDTO
                   {
                       StudentId = sp.StudentId,
                       SectionId = sp.SectionId,
                       GradeTypeCode = sp.GradeTypeCode,
                       GradeCodeOccurrence = sp.GradeCodeOccurrence,
                       NumericGrade = sp.NumericGrade,
                       Comments = sp.Comments,
                       CreatedBy = sp.CreatedBy,
                       CreatedDate = sp.CreatedDate,
                       ModifiedBy = sp.ModifiedBy,
                       ModifiedDate = sp.ModifiedDate,
                       SchoolId = sp.SchoolId
                   }).FirstOrDefaultAsync();

                return Ok(itmGrade);
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
        [Route("PostGrade")]
        public async Task<IActionResult> PostGrade([FromBody] string _GradeDTO_String)
        {

            try
            {
                GradeDTO _GradeDTO = JsonSerializer.Deserialize<GradeDTO>(_GradeDTO_String);
                await this.PostGrade(_GradeDTO);
            }
            catch (Exception)
            {
                throw;
            }

            return Ok();
        }




        [HttpPost]
        public async Task<IActionResult> PostGrade([FromBody] GradeDTO _GradeDTO)
        {
            try
            {
                var trans = await _context.Database.BeginTransactionAsync();
                Grade c = new Grade
                {
                    StudentId = _GradeDTO.StudentId,
                    SectionId = _GradeDTO.SectionId,
                    GradeTypeCode = _GradeDTO.GradeTypeCode,
                    GradeCodeOccurrence = _GradeDTO.GradeCodeOccurrence,
                    NumericGrade = _GradeDTO.NumericGrade,
                    Comments = _GradeDTO.Comments,
                    SchoolId = _GradeDTO.SchoolId
                };
                _context.Grades.Add(c);
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
        public async Task<IActionResult> PutGrade(GradeDTO _GradeDTO)
        {

            try
            {
                var trans = await _context.Database.BeginTransactionAsync();
                Grade c = await _context.Grades.Where(x => x.StudentId.Equals(_GradeDTO.StudentId)).FirstOrDefaultAsync();

                if (c != null)
                {
                    c.StudentId = _GradeDTO.StudentId;
                    c.SectionId = _GradeDTO.SectionId;
                    c.GradeTypeCode = _GradeDTO.GradeTypeCode;
                    c.GradeCodeOccurrence = _GradeDTO.GradeCodeOccurrence;
                    c.NumericGrade = _GradeDTO.NumericGrade;
                    c.Comments = _GradeDTO.Comments;
                    c.SchoolId = _GradeDTO.SchoolId;

                    _context.Grades.Update(c);
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
        [Route("DeleteGrade/{pStudentId}")]
        public async Task<IActionResult> DeleteGrade(int pStudentId)
        {

            try
            {


                var trans = await _context.Database.BeginTransactionAsync();
                Grade c = await _context.Grades.Where(x => x.StudentId.Equals(pStudentId)).FirstOrDefaultAsync();
                _context.Grades.Remove(c);

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
        [Route("GetGrades")]
        public async Task<DataEnvelope<GradeDTO>> GetGradesPost([FromBody] DataSourceRequest gridRequest)
        {
            DataEnvelope<GradeDTO> dataToReturn = null;
            IQueryable<GradeDTO> queriableStates = _context.Grades
                    .Select(sp => new GradeDTO
                    {
                        StudentId = sp.StudentId,
                        SectionId = sp.SectionId,
                        GradeTypeCode = sp.GradeTypeCode,
                        GradeCodeOccurrence = sp.GradeCodeOccurrence,
                        NumericGrade = sp.NumericGrade,
                        Comments = sp.Comments,
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
                    dataToReturn = new DataEnvelope<GradeDTO>
                    {
                        GroupedData = processedData.Data.Cast<AggregateFunctionsGroup>().ToList(),
                        TotalItemCount = processedData.Total
                    };
                }
                else
                {
                    // When there is no grouping, the simplistic approach of 
                    // just serializing and deserializing the flat data is enough
                    dataToReturn = new DataEnvelope<GradeDTO>
                    {
                        CurrentPageData = processedData.Data.Cast<GradeDTO>().ToList(),
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
