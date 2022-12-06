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
//using static System.Collections.Specialized.BitVector32;
//using SNICKERS.Client.Pages.School;

namespace SNICKERS.Server.Controllers.Schools
{
    public class SectionController : Controller
    {
        protected readonly SNICKERSOracleContext _context;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly OraTransMsgs _OraTranslateMsgs;

        public SectionController(SNICKERSOracleContext context,
            IHttpContextAccessor httpContextAccessor,
             OraTransMsgs OraTranslateMsgs)
        {
            this._context = context;
            this._httpContextAccessor = httpContextAccessor;
            this._OraTranslateMsgs = OraTranslateMsgs;
        }

        [HttpGet]
        [Route("GetSections")]
        public async Task<IActionResult> GetSections()
        {
            try
            {
                List<SectionDTO> lstSections = await _context.Sections.OrderBy(x => x.SectionId)
                   .Select(sp => new SectionDTO
                   {
                       SectionId = sp.SectionId,
                       CourseNo = sp.CourseNo,
                       SectionNo = sp.SectionNo,
                       StartDateTime = sp.StartDateTime,
                       Location = sp.Location,
                       InstructorId = sp.InstructorId,
                       Capacity = sp.Capacity,
                       CreatedBy = sp.CreatedBy,
                       CreatedDate = sp.CreatedDate,
                       ModifiedBy = sp.ModifiedBy,
                       ModifiedDate = sp.ModifiedDate,
                       SchoolId = sp.SchoolId,
                   }).ToListAsync();

                return Ok(lstSections);
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
        [Route("GetSections/{pSectionId}")]
        public async Task<IActionResult> GetSections(int pSectionId)
        {
            try
            {

                SectionDTO itmSection = await _context.Sections
                    .Where(x => x.SectionId == pSectionId)
                    .OrderBy(x => x.SectionId)
                   .Select(sp => new SectionDTO
                   {
                       SectionId = sp.SectionId,
                       CourseNo = sp.CourseNo,
                       SectionNo = sp.SectionNo,
                       StartDateTime = sp.StartDateTime,
                       Location = sp.Location,
                       InstructorId = sp.InstructorId,
                       Capacity = sp.Capacity,
                       CreatedBy = sp.CreatedBy,
                       CreatedDate = sp.CreatedDate,
                       ModifiedBy = sp.ModifiedBy,
                       ModifiedDate = sp.ModifiedDate,
                       SchoolId = sp.SchoolId,
                   }).FirstOrDefaultAsync();

                return Ok(itmSection);
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
        [Route("PostSection")]
        public async Task<IActionResult> PostSection([FromBody] string _SectionDTO_String)
        {

            try
            {
                SectionDTO _SectionDTO = JsonSerializer.Deserialize<SectionDTO>(_SectionDTO_String);
                await this.PostSection(_SectionDTO);
            }
            catch (Exception)
            {
                throw;
            }

            return Ok();
        }




        [HttpPost]
        public async Task<IActionResult> PostSection([FromBody] SectionDTO _SectionDTO)
        {
            try
            {
                var trans = await _context.Database.BeginTransactionAsync();
                Section c = new Section
                {
                    SectionId = _SectionDTO.SectionId,
                    CourseNo = _SectionDTO.CourseNo,
                    SectionNo = _SectionDTO.SectionNo,
                    StartDateTime = _SectionDTO.StartDateTime,
                    Location = _SectionDTO.Location,
                    InstructorId = _SectionDTO.InstructorId,
                    Capacity = _SectionDTO.Capacity,
                    SchoolId = _SectionDTO.SchoolId
                };
                _context.Sections.Add(c);
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
        public async Task<IActionResult> PutSection(SectionDTO _SectionDTO)
        {

            try
            {
                var trans = await _context.Database.BeginTransactionAsync();
                Section c = await _context.Sections.Where(x => x.SectionId.Equals(_SectionDTO.SectionId)).FirstOrDefaultAsync();

                if (c != null)
                {
                    c.SectionId = _SectionDTO.SectionId;
                    c.CourseNo = _SectionDTO.CourseNo;
                    c.SectionNo = _SectionDTO.SectionNo;
                    c.StartDateTime = _SectionDTO.StartDateTime;
                    c.Location = _SectionDTO.Location;
                    c.InstructorId = _SectionDTO.InstructorId;
                    c.Capacity = _SectionDTO.Capacity;
                    c.SchoolId = _SectionDTO.SchoolId;

                    _context.Sections.Update(c);
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
        [Route("DeleteSection/{pSectionId}")]
        public async Task<IActionResult> DeleteSection(int pSectionId)
        {

            try
            {


                var trans = await _context.Database.BeginTransactionAsync();
                Section c = await _context.Sections.Where(x => x.SectionId.Equals(pSectionId)).FirstOrDefaultAsync();
                _context.Sections.Remove(c);

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
        [Route("GetSections")]
        public async Task<DataEnvelope<SectionDTO>> GetSectionsPost([FromBody] DataSourceRequest gridRequest)
        {
            DataEnvelope<SectionDTO> dataToReturn = null;
            IQueryable<SectionDTO> queriableStates = _context.Sections
                    .Select(sp => new SectionDTO
                    {
                        SectionId = sp.SectionId,
                        CourseNo = sp.CourseNo,
                        SectionNo = sp.SectionNo,
                        StartDateTime = sp.StartDateTime,
                        Location = sp.Location,
                        InstructorId = sp.InstructorId,
                        Capacity = sp.Capacity,
                        CreatedBy = sp.CreatedBy,
                        CreatedDate = sp.CreatedDate,
                        ModifiedBy = sp.ModifiedBy,
                        ModifiedDate = sp.ModifiedDate,
                        SchoolId = sp.SchoolId,
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
                    dataToReturn = new DataEnvelope<SectionDTO>
                    {
                        GroupedData = processedData.Data.Cast<AggregateFunctionsGroup>().ToList(),
                        TotalItemCount = processedData.Total
                    };
                }
                else
                {
                    // When there is no grouping, the simplistic approach of 
                    // just serializing and deserializing the flat data is enough
                    dataToReturn = new DataEnvelope<SectionDTO>
                    {
                        CurrentPageData = processedData.Data.Cast<SectionDTO>().ToList(),
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
