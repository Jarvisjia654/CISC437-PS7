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

namespace SNICKERS.Server.Controllers.Schools
{
    public class GradeTypeController : Controller
    {
        protected readonly SNICKERSOracleContext _context;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly OraTransMsgs _OraTranslateMsgs;

        public GradeTypeController(SNICKERSOracleContext context,
            IHttpContextAccessor httpContextAccessor,
             OraTransMsgs OraTranslateMsgs)
        {
            this._context = context;
            this._httpContextAccessor = httpContextAccessor;
            this._OraTranslateMsgs = OraTranslateMsgs;
        }

        [HttpGet]
        [Route("GetGradeTypes")]
        public async Task<IActionResult> GetGradeTypes()
        {
            try
            {
                List<GradeTypeDTO> lstGradeTypes = await _context.GradeTypes.OrderBy(x => x.SchoolId)
                   .Select(sp => new GradeTypeDTO
                   {
                       GradeTypeCode = sp.GradeTypeCode,
                       Description = sp.Description,
                       CreatedBy = sp.CreatedBy,
                       CreatedDate = sp.CreatedDate,
                       ModifiedBy = sp.ModifiedBy,
                       ModifiedDate = sp.ModifiedDate,
                       SchoolId = sp.SchoolId

                   }).ToListAsync();

                return Ok(lstGradeTypes);
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
        [Route("GetGradeTypes/{pGradeTypeNo}")]
        public async Task<IActionResult> GetGradeTypes(int pSchoolId)
        {
            try
            {

                GradeTypeDTO itmGradeType = await _context.GradeTypes
                    .Where(x => x.SchoolId == pSchoolId)
                    .OrderBy(x => x.SchoolId)
                   .Select(sp => new GradeTypeDTO
                   {
                       GradeTypeCode = sp.GradeTypeCode,
                       Description = sp.Description,
                       CreatedBy = sp.CreatedBy,
                       CreatedDate = sp.CreatedDate,
                       ModifiedBy = sp.ModifiedBy,
                       ModifiedDate = sp.ModifiedDate,
                       SchoolId = sp.SchoolId

                   }).FirstOrDefaultAsync();

                return Ok(itmGradeType);
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
        [Route("PostGradeType")]
        public async Task<IActionResult> PostGradeType([FromBody] string _GradeTypeDTO_String)
        {

            try
            {
                GradeTypeDTO _GradeTypeDTO = JsonSerializer.Deserialize<GradeTypeDTO>(_GradeTypeDTO_String);
                await this.PostGradeType(_GradeTypeDTO);
            }
            catch (Exception)
            {
                throw;
            }

            return Ok();
        }




        [HttpPost]
        public async Task<IActionResult> PostGradeType([FromBody] GradeTypeDTO _GradeTypeDTO)
        {
            try
            {
                var trans = await _context.Database.BeginTransactionAsync();
                GradeType c = new GradeType
                {
                    GradeTypeCode = _GradeTypeDTO.GradeTypeCode,
                    Description = _GradeTypeDTO.Description,
                    SchoolId = _GradeTypeDTO.SchoolId
                };
                _context.GradeTypes.Add(c);
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
        public async Task<IActionResult> PutGradeType(GradeTypeDTO _GradeTypeDTO)
        {

            try
            {
                var trans = await _context.Database.BeginTransactionAsync();
                GradeType c = await _context.GradeTypes.Where(x => x.SchoolId.Equals(_GradeTypeDTO.SchoolId)).FirstOrDefaultAsync();

                if (c != null)
                {
                    c.GradeTypeCode = _GradeTypeDTO.GradeTypeCode;
                    c.Description = _GradeTypeDTO.Description;
                    c.SchoolId = _GradeTypeDTO.SchoolId;

                    _context.GradeTypes.Update(c);
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
        [Route("DeleteGradeType/{pSchoolId}")]
        public async Task<IActionResult> DeleteGradeType(int pSchoolId)
        {

            try
            {


                var trans = await _context.Database.BeginTransactionAsync();
                GradeType c = await _context.GradeTypes.Where(x => x.SchoolId.Equals(pSchoolId)).FirstOrDefaultAsync();
                _context.GradeTypes.Remove(c);

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
        [Route("GetGradeTypes")]
        public async Task<DataEnvelope<GradeTypeDTO>> GetGradeTypesPost([FromBody] DataSourceRequest gridRequest)
        {
            DataEnvelope<GradeTypeDTO> dataToReturn = null;
            IQueryable<GradeTypeDTO> queriableStates = _context.GradeTypes
                    .Select(sp => new GradeTypeDTO
                    {
                        GradeTypeCode = sp.GradeTypeCode,
                        Description = sp.Description,
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
                    dataToReturn = new DataEnvelope<GradeTypeDTO>
                    {
                        GroupedData = processedData.Data.Cast<AggregateFunctionsGroup>().ToList(),
                        TotalItemCount = processedData.Total
                    };
                }
                else
                {
                    // When there is no grouping, the simplistic approach of 
                    // just serializing and deserializing the flat data is enough
                    dataToReturn = new DataEnvelope<GradeTypeDTO>
                    {
                        CurrentPageData = processedData.Data.Cast<GradeTypeDTO>().ToList(),
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
