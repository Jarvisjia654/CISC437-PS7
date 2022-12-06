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
using Microsoft.AspNetCore.Mvc;

namespace SNICKERS.Server.Controllers.Schools
{
    public class ZipcodeController: Controller
    {
        protected readonly SNICKERSOracleContext _context;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly OraTransMsgs _OraTranslateMsgs;

        public ZipcodeController(SNICKERSOracleContext context,
            IHttpContextAccessor httpContextAccessor,
             OraTransMsgs OraTranslateMsgs)
        {
            this._context = context;
            this._httpContextAccessor = httpContextAccessor;
            this._OraTranslateMsgs = OraTranslateMsgs;
        }

        [HttpGet]
        [Route("GetZipcodes")]
        public async Task<IActionResult> GetZipcodes()
        {
            try
            {
                List<ZipcodeDTO> lstZipcodes = await _context.Zipcodes.OrderBy(x => x.Zip)
                   .Select(sp => new ZipcodeDTO
                   {
                       Zip = sp.Zip,
                       City = sp.City,
                       State = sp.State,
                       CreatedBy = sp.CreatedBy,
                       CreatedDate = sp.CreatedDate,
                       ModifiedBy = sp.ModifiedBy,
                       ModifiedDate = sp.ModifiedDate
                   }).ToListAsync();

                return Ok(lstZipcodes);
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
        [Route("GetZipcodes/{pZipcode}")]
        public async Task<IActionResult> GetZipcodes(string pZipcode)
        {
            try
            {

                ZipcodeDTO itmZipcode = await _context.Zipcodes
                    .Where(x => x.Zip == pZipcode)
                    .OrderBy(x => x.Zip)
                   .Select(sp => new ZipcodeDTO
                   {
                       Zip = sp.Zip,
                       City = sp.City,
                       State = sp.State,
                       CreatedBy = sp.CreatedBy,
                       CreatedDate = sp.CreatedDate,
                       ModifiedBy = sp.ModifiedBy,
                       ModifiedDate = sp.ModifiedDate
                   }).FirstOrDefaultAsync();

                return Ok(itmZipcode);
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
        [Route("PostZipcode")]
        public async Task<IActionResult> PostZipcode([FromBody] string _ZipcodeDTO_String)
        {

            try
            {
                ZipcodeDTO _ZipcodeDTO = JsonSerializer.Deserialize<ZipcodeDTO>(_ZipcodeDTO_String);
                await this.PostZipcode(_ZipcodeDTO);
            }
            catch (Exception)
            {
                throw;
            }

            return Ok();
        }




        [HttpPost]
        public async Task<IActionResult> PostZipcode([FromBody] ZipcodeDTO _ZipcodeDTO)
        {
            try
            {
                var trans = await _context.Database.BeginTransactionAsync();
                Zipcode c = new Zipcode
                {
                    Zip = _ZipcodeDTO.Zip,
                    City = _ZipcodeDTO.City,
                    State = _ZipcodeDTO.State,
                    CreatedBy = _ZipcodeDTO.CreatedBy,
                    CreatedDate = _ZipcodeDTO.CreatedDate,
                    ModifiedBy = _ZipcodeDTO.ModifiedBy,
                    ModifiedDate = _ZipcodeDTO.ModifiedDate
                };
                _context.Zipcodes.Add(c);
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
        public async Task<IActionResult> PutZipcode(ZipcodeDTO _ZipcodeDTO)
        {

            try
            {
                var trans = await _context.Database.BeginTransactionAsync();
                Zipcode c = await _context.Zipcodes.Where(x => x.Zip.Equals(_ZipcodeDTO.Zip)).FirstOrDefaultAsync();

                if (c != null)
                {
                    c.Zip = _ZipcodeDTO.Zip;
                    c.City = _ZipcodeDTO.City;
                    c.State = _ZipcodeDTO.State;
                      

                    _context.Zipcodes.Update(c);
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
        [Route("DeleteZipcode/{pZip}")]
        public async Task<IActionResult> DeleteZipcode(int pZip)
        {

            try
            {


                var trans = await _context.Database.BeginTransactionAsync();
                Zipcode c = await _context.Zipcodes.Where(x => x.Zip.Equals(pZip)).FirstOrDefaultAsync();
                _context.Zipcodes.Remove(c);

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
        [Route("GetZipcodes")]
        public async Task<DataEnvelope<ZipcodeDTO>> GetZipcodesPost([FromBody] DataSourceRequest gridRequest)
        {
            DataEnvelope<ZipcodeDTO> dataToReturn = null;
            IQueryable<ZipcodeDTO> queriableStates = _context.Zipcodes
                    .Select(sp => new ZipcodeDTO
                    {
                        Zip = sp.Zip,
                        City = sp.City,
                        State = sp.State,
                        CreatedBy = sp.CreatedBy,
                        CreatedDate = sp.CreatedDate,
                        ModifiedBy = sp.ModifiedBy,
                        ModifiedDate = sp.ModifiedDate
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
                    dataToReturn = new DataEnvelope<ZipcodeDTO>
                    {
                        GroupedData = processedData.Data.Cast<AggregateFunctionsGroup>().ToList(),
                        TotalItemCount = processedData.Total
                    };
                }
                else
                {
                    // When there is no grouping, the simplistic approach of 
                    // just serializing and deserializing the flat data is enough
                    dataToReturn = new DataEnvelope<ZipcodeDTO>
                    {
                        CurrentPageData = processedData.Data.Cast<ZipcodeDTO>().ToList(),
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
