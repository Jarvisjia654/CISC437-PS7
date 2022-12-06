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
    public class GradeConversionController : Controller
    {
        protected readonly SNICKERSOracleContext _context;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly OraTransMsgs _OraTranslateMsgs;

        public GradeConversionController(SNICKERSOracleContext context,
            IHttpContextAccessor httpContextAccessor,
             OraTransMsgs OraTranslateMsgs)
        {
            this._context = context;
            this._httpContextAccessor = httpContextAccessor;
            this._OraTranslateMsgs = OraTranslateMsgs;
        }

        [HttpGet]
        [Route("GetGradeConversions")]
        public async Task<IActionResult> GetGradeConversions()
        {
            try
            {
                List<GradeConversionDTO> lstGradeConversions = await _context.GradeConversions.OrderBy(x => x.SchoolId)
                   .Select(sp => new GradeConversionDTO
                   {
                       LetterGrade = sp.LetterGrade,
                       GradePoint = sp.GradePoint,
                       MaxGrade = sp.MaxGrade,
                       MinGrade = sp.MinGrade,
                       CreatedBy = sp.CreatedBy,
                       CreatedDate = sp.CreatedDate,
                       ModifiedBy = sp.ModifiedBy,
                       ModifiedDate = sp.ModifiedDate,
                       SchoolId = sp.SchoolId
                   }).ToListAsync();

                return Ok(lstGradeConversions);
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
        [Route("GetGradeConversions/{pSchoolId}")]
        public async Task<IActionResult> GetGradeConversions(int pSchoolId)
        {
            try
            {

                GradeConversionDTO itmGradeConversion = await _context.GradeConversions
                    .Where(x => x.SchoolId == pSchoolId)
                    .OrderBy(x => x.SchoolId)
                   .Select(sp => new GradeConversionDTO
                   {
                       LetterGrade = sp.LetterGrade,
                       GradePoint = sp.GradePoint,
                       MaxGrade = sp.MaxGrade,
                       MinGrade = sp.MinGrade,
                       CreatedBy = sp.CreatedBy,
                       CreatedDate = sp.CreatedDate,
                       ModifiedBy = sp.ModifiedBy,
                       ModifiedDate = sp.ModifiedDate,
                       SchoolId = sp.SchoolId
                   }).FirstOrDefaultAsync();

                return Ok(itmGradeConversion);
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
        [Route("PostGradeConversion")]
        public async Task<IActionResult> PostGradeConversion([FromBody] string _GradeConversionDTO_String)
        {

            try
            {
                GradeConversionDTO _GradeConversionDTO = JsonSerializer.Deserialize<GradeConversionDTO>(_GradeConversionDTO_String);
                await this.PostGradeConversion(_GradeConversionDTO);
            }
            catch (Exception)
            {
                throw;
            }

            return Ok();
        }




        [HttpPost]
        public async Task<IActionResult> PostGradeConversion([FromBody] GradeConversionDTO _GradeConversionDTO)
        {
            try
            {
                var trans = await _context.Database.BeginTransactionAsync();
                GradeConversion c = new GradeConversion
                {
                    LetterGrade = _GradeConversionDTO.LetterGrade,
                    GradePoint = _GradeConversionDTO.GradePoint,
                    MaxGrade = _GradeConversionDTO.MaxGrade,
                    MinGrade = _GradeConversionDTO.MinGrade,
                    SchoolId = _GradeConversionDTO.SchoolId
                };
                _context.GradeConversions.Add(c);
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
        public async Task<IActionResult> PutGradeConversion(GradeConversionDTO _GradeConversionDTO)
        {

            try
            {
                var trans = await _context.Database.BeginTransactionAsync();
                GradeConversion c = await _context.GradeConversions.Where(x => x.SchoolId.Equals(_GradeConversionDTO.SchoolId)).FirstOrDefaultAsync();

                if (c != null)
                {
                    c.LetterGrade = _GradeConversionDTO.LetterGrade;
                    c.GradePoint = _GradeConversionDTO.GradePoint;
                    c.MaxGrade = _GradeConversionDTO.MaxGrade;
                    c.MinGrade = _GradeConversionDTO.MinGrade;
                    c.SchoolId = _GradeConversionDTO.SchoolId;

                    _context.GradeConversions.Update(c);
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
        [Route("DeleteGradeConversion/{pSchoolId}")]
        public async Task<IActionResult> DeleteGradeConversion(int pSchoolId)
        {

            try
            {


                var trans = await _context.Database.BeginTransactionAsync();
                GradeConversion c = await _context.GradeConversions.Where(x => x.SchoolId.Equals(pSchoolId)).FirstOrDefaultAsync();
                _context.GradeConversions.Remove(c);

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
        [Route("GetGradeConversions")]
        public async Task<DataEnvelope<GradeConversionDTO>> GetGradeConversionsPost([FromBody] DataSourceRequest gridRequest)
        {
            DataEnvelope<GradeConversionDTO> dataToReturn = null;
            IQueryable<GradeConversionDTO> queriableStates = _context.GradeConversions
                    .Select(sp => new GradeConversionDTO
                    {
                        LetterGrade = sp.LetterGrade,
                        GradePoint = sp.GradePoint,
                        MaxGrade = sp.MaxGrade,
                        MinGrade = sp.MinGrade,
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
                    dataToReturn = new DataEnvelope<GradeConversionDTO>
                    {
                        GroupedData = processedData.Data.Cast<AggregateFunctionsGroup>().ToList(),
                        TotalItemCount = processedData.Total
                    };
                }
                else
                {
                    // When there is no grouping, the simplistic approach of 
                    // just serializing and deserializing the flat data is enough
                    dataToReturn = new DataEnvelope<GradeConversionDTO>
                    {
                        CurrentPageData = processedData.Data.Cast<GradeConversionDTO>().ToList(),
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
