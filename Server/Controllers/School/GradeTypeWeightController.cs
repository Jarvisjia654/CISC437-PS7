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
    public class GradeTypeWeightController : Controller
    {
        protected readonly SNICKERSOracleContext _context;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly OraTransMsgs _OraTranslateMsgs;

        public GradeTypeWeightController(SNICKERSOracleContext context,
            IHttpContextAccessor httpContextAccessor,
             OraTransMsgs OraTranslateMsgs)
        {
            this._context = context;
            this._httpContextAccessor = httpContextAccessor;
            this._OraTranslateMsgs = OraTranslateMsgs;
        }

        [HttpGet]
        [Route("GetGradeTypeWeights")]
        public async Task<IActionResult> GetGradeTypeWeights()
        {
            try
            {
                List<GradeTypeWeightDTO> lstGradeTypeWeights = await _context.GradeTypeWeights.OrderBy(x => x.SectionId)
                   .Select(sp => new GradeTypeWeightDTO
                   {
                       SectionId = sp.SectionId,
                       GradeTypeCode = sp.GradeTypeCode,
                       NumberPerSection = sp.NumberPerSection,
                       PercentOfFinalGrade = sp.PercentOfFinalGrade,
                       DropLowest = sp.DropLowest,
                       CreatedBy = sp.CreatedBy,
                       CreatedDate = sp.CreatedDate,
                       ModifiedBy = sp.ModifiedBy,
                       ModifiedDate = sp.ModifiedDate,
                       SchoolId = sp.SchoolId
                       
                   }).ToListAsync();

                return Ok(lstGradeTypeWeights);
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
        [Route("GetGradeTypeWeights/{pSectionId}")]
        public async Task<IActionResult> GetGradeTypeWeights(int pSectionId)
        {
            try
            {

                GradeTypeWeightDTO itmGradeTypeWeight = await _context.GradeTypeWeights
                    .Where(x => x.SectionId == pSectionId)
                    .OrderBy(x => x.SectionId)
                   .Select(sp => new GradeTypeWeightDTO
                   {
                       SectionId = sp.SectionId,
                       GradeTypeCode = sp.GradeTypeCode,
                       NumberPerSection = sp.NumberPerSection,
                       PercentOfFinalGrade = sp.PercentOfFinalGrade,
                       DropLowest = sp.DropLowest,
                       CreatedBy = sp.CreatedBy,
                       CreatedDate = sp.CreatedDate,
                       ModifiedBy = sp.ModifiedBy,
                       ModifiedDate = sp.ModifiedDate,
                       SchoolId = sp.SchoolId

                   }).FirstOrDefaultAsync();

                return Ok(itmGradeTypeWeight);
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
        [Route("PostGradeTypeWeight")]
        public async Task<IActionResult> PostGradeTypeWeight([FromBody] string _GradeTypeWeightDTO_String)
        {

            try
            {
                GradeTypeWeightDTO _GradeTypeWeightDTO = JsonSerializer.Deserialize<GradeTypeWeightDTO>(_GradeTypeWeightDTO_String);
                await this.PostGradeTypeWeight(_GradeTypeWeightDTO);
            }
            catch (Exception)
            {
                throw;
            }

            return Ok();
        }




        [HttpPost]
        public async Task<IActionResult> PostGradeTypeWeight([FromBody] GradeTypeWeightDTO _GradeTypeWeightDTO)
        {
            try
            {
                var trans = await _context.Database.BeginTransactionAsync();
                GradeTypeWeight c = new GradeTypeWeight
                {
                    SectionId = _GradeTypeWeightDTO.SectionId,
                    GradeTypeCode = _GradeTypeWeightDTO.GradeTypeCode,
                    NumberPerSection = _GradeTypeWeightDTO.NumberPerSection,
                    PercentOfFinalGrade = _GradeTypeWeightDTO.PercentOfFinalGrade,
                    DropLowest = _GradeTypeWeightDTO.DropLowest,
                    SchoolId = _GradeTypeWeightDTO.SchoolId
                };
                _context.GradeTypeWeights.Add(c);
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
        public async Task<IActionResult> PutGradeTypeWeight(GradeTypeWeightDTO _GradeTypeWeightDTO)
        {

            try
            {
                var trans = await _context.Database.BeginTransactionAsync();
                GradeTypeWeight c = await _context.GradeTypeWeights.Where(x => x.SectionId.Equals(_GradeTypeWeightDTO.SectionId)).FirstOrDefaultAsync();

                if (c != null)
                {
                    c.SectionId = _GradeTypeWeightDTO.SectionId;
                    c.GradeTypeCode = _GradeTypeWeightDTO.GradeTypeCode;
                    c.NumberPerSection = _GradeTypeWeightDTO.NumberPerSection;
                    c.PercentOfFinalGrade = _GradeTypeWeightDTO.PercentOfFinalGrade;
                    c.DropLowest = _GradeTypeWeightDTO.DropLowest;
                    c.SchoolId = _GradeTypeWeightDTO.SchoolId;

                    _context.GradeTypeWeights.Update(c);
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
        [Route("DeleteGradeTypeWeight/{pSectionId}")]
        public async Task<IActionResult> DeleteGradeTypeWeight(int pSectionId)
        {

            try
            {


                var trans = await _context.Database.BeginTransactionAsync();
                GradeTypeWeight c = await _context.GradeTypeWeights.Where(x => x.SectionId.Equals(pSectionId)).FirstOrDefaultAsync();
                _context.GradeTypeWeights.Remove(c);

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
        [Route("GetGradeTypeWeights")]
        public async Task<DataEnvelope<GradeTypeWeightDTO>> GetGradeTypeWeightsPost([FromBody] DataSourceRequest gridRequest)
        {
            DataEnvelope<GradeTypeWeightDTO> dataToReturn = null;
            IQueryable<GradeTypeWeightDTO> queriableStates = _context.GradeTypeWeights
                    .Select(sp => new GradeTypeWeightDTO
                    {
                        SectionId = sp.SectionId,
                        GradeTypeCode = sp.GradeTypeCode,
                        NumberPerSection = sp.NumberPerSection,
                        PercentOfFinalGrade = sp.PercentOfFinalGrade,
                        DropLowest = sp.DropLowest,
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
                    dataToReturn = new DataEnvelope<GradeTypeWeightDTO>
                    {
                        GroupedData = processedData.Data.Cast<AggregateFunctionsGroup>().ToList(),
                        TotalItemCount = processedData.Total
                    };
                }
                else
                {
                    // When there is no grouping, the simplistic approach of 
                    // just serializing and deserializing the flat data is enough
                    dataToReturn = new DataEnvelope<GradeTypeWeightDTO>
                    {
                        CurrentPageData = processedData.Data.Cast<GradeTypeWeightDTO>().ToList(),
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
