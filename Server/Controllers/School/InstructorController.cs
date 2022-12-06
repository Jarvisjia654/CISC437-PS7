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
using static Duende.IdentityServer.Models.IdentityResources;

namespace SNICKERS.Server.Controllers.Schools
{
    public class InstructorController : Controller
    {
        protected readonly SNICKERSOracleContext _context;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly OraTransMsgs _OraTranslateMsgs;

        public InstructorController(SNICKERSOracleContext context,
            IHttpContextAccessor httpContextAccessor,
             OraTransMsgs OraTranslateMsgs)
        {
            this._context = context;
            this._httpContextAccessor = httpContextAccessor;
            this._OraTranslateMsgs = OraTranslateMsgs;
        }

        [HttpGet]
        [Route("GetInstructors")]
        public async Task<IActionResult> GetInstructors()
        {
            try
            {
                List<InstructorDTO> lstInstructors = await _context.Instructors.OrderBy(x => x.InstructorId)
                   .Select(sp => new InstructorDTO
                   {
                       InstructorId = sp.InstructorId,
                       Salutation = sp.Salutation,
                       FirstName = sp.FirstName,
                       LastName = sp.LastName,
                       StreetAddress = sp.StreetAddress,
                       Zip = sp.Zip,
                       Phone = sp.Phone,
                       CreatedBy = sp.CreatedBy,
                       CreatedDate = sp.CreatedDate,
                       ModifiedBy = sp.ModifiedBy,
                       ModifiedDate = sp.ModifiedDate,
                       SchoolId = sp.SchoolId
                   }).ToListAsync();

                return Ok(lstInstructors);
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
        [Route("GetInstructors/{pInstructorId}")]
        public async Task<IActionResult> GetInstructors(int pInstructorId)
        {
            try
            {

                InstructorDTO itmInstructor = await _context.Instructors
                    .Where(x => x.InstructorId == pInstructorId)
                    .OrderBy(x => x.InstructorId)
                   .Select(sp => new InstructorDTO
                   {
                       InstructorId = sp.InstructorId,
                       Salutation = sp.Salutation,
                       FirstName = sp.FirstName,
                       LastName = sp.LastName,
                       StreetAddress = sp.StreetAddress,
                       Zip = sp.Zip,
                       Phone = sp.Phone,
                       CreatedBy = sp.CreatedBy,
                       CreatedDate = sp.CreatedDate,
                       ModifiedBy = sp.ModifiedBy,
                       ModifiedDate = sp.ModifiedDate,
                       SchoolId = sp.SchoolId
                   }).FirstOrDefaultAsync();

                return Ok(itmInstructor);
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
        [Route("PostInstructor")]
        public async Task<IActionResult> PostInstructor([FromBody] string _InstructorDTO_String)
        {

            try
            {
                InstructorDTO _InstructorDTO = JsonSerializer.Deserialize<InstructorDTO>(_InstructorDTO_String);
                await this.PostInstructor(_InstructorDTO);
            }
            catch (Exception)
            {
                throw;
            }

            return Ok();
        }




        [HttpPost]
        public async Task<IActionResult> PostInstructor([FromBody] InstructorDTO _InstructorDTO)
        {
            try
            {
                var trans = await _context.Database.BeginTransactionAsync();
                Instructor c = new Instructor
                {
                    InstructorId = _InstructorDTO.InstructorId,
                    Salutation = _InstructorDTO.Salutation,
                    FirstName = _InstructorDTO.FirstName,
                    LastName = _InstructorDTO.LastName,
                    StreetAddress = _InstructorDTO.StreetAddress,
                    Zip = _InstructorDTO.Zip,
                    Phone = _InstructorDTO.Phone,
                    CreatedBy = _InstructorDTO.CreatedBy,
                    CreatedDate = _InstructorDTO.CreatedDate,
                    ModifiedBy = _InstructorDTO.ModifiedBy,
                    ModifiedDate = _InstructorDTO.ModifiedDate,
                    SchoolId = _InstructorDTO.SchoolId
                };
                _context.Instructors.Add(c);
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
        public async Task<IActionResult> PutInstructor(InstructorDTO _InstructorDTO)
        {

            try
            {
                var trans = await _context.Database.BeginTransactionAsync();
                Instructor c = await _context.Instructors.Where(x => x.InstructorId.Equals(_InstructorDTO.InstructorId)).FirstOrDefaultAsync();

                if (c != null)
                {
                    c.InstructorId = _InstructorDTO.InstructorId;
                    c.Salutation = _InstructorDTO.Salutation;
                    c.FirstName = _InstructorDTO.FirstName;
                    c.LastName = _InstructorDTO.LastName;
                    c.StreetAddress = _InstructorDTO.StreetAddress;
                    c.Zip = _InstructorDTO.Zip;
                    c.Phone = _InstructorDTO.Phone;
                    c.SchoolId = _InstructorDTO.SchoolId;

                    _context.Instructors.Update(c);
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
        [Route("DeleteInstructor/{pInstructorId}")]
        public async Task<IActionResult> DeleteInstructor(int pInstructorId)
        {

            try
            {


                var trans = await _context.Database.BeginTransactionAsync();
                Instructor c = await _context.Instructors.Where(x => x.InstructorId.Equals(pInstructorId)).FirstOrDefaultAsync();
                _context.Instructors.Remove(c);

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
        [Route("GetInstructors")]
        public async Task<DataEnvelope<InstructorDTO>> GetInstructorsPost([FromBody] DataSourceRequest gridRequest)
        {
            DataEnvelope<InstructorDTO> dataToReturn = null;
            IQueryable<InstructorDTO> queriableStates = _context.Instructors
                    .Select(sp => new InstructorDTO
                    {
                        InstructorId = sp.InstructorId,
                        Salutation = sp.Salutation,
                        FirstName = sp.FirstName,
                        LastName = sp.LastName,
                        StreetAddress = sp.StreetAddress,
                        Zip = sp.Zip,
                        Phone = sp.Phone,
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
                    dataToReturn = new DataEnvelope<InstructorDTO>
                    {
                        GroupedData = processedData.Data.Cast<AggregateFunctionsGroup>().ToList(),
                        TotalItemCount = processedData.Total
                    };
                }
                else
                {
                    // When there is no grouping, the simplistic approach of 
                    // just serializing and deserializing the flat data is enough
                    dataToReturn = new DataEnvelope<InstructorDTO>
                    {
                        CurrentPageData = processedData.Data.Cast<InstructorDTO>().ToList(),
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
