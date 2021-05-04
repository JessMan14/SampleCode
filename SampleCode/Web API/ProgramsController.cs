using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoxConnector;
using ContractorProvider_SQL;
using DashboardCore.Models;
using DashboardProvider_SQL;
using EnergyEfficiency.Models;
using FileProvider_SQL;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using OnlinePortalWebService.Models;
using OnlinePortalWebServiceCore.Helpers;
using OnlinePortalWebServiceCore.Models;
using ProgramProvider_File;
using SQLService;
using TinyCsvParser;
using ValidationCore.IncomingModels;
using ValidationProvider_SQL;
using static EnergyEfficiency.Enums;
using static SystemCore.Enums;

namespace OnlinePortalWebServiceCore.Controllers
{
    public class ProgramsController : ConfiguredController
    {
        private ILog _log = LogManager.GetLogger(typeof(ProgramsController));

        public ProgramsController(IConfiguration config)
        {
            _sqlFactory = SQL_Factory.getInstance(config);
            this.Configuration = config;
        }

        [HttpGet()]
        [Route("api/Programs")]
        public async Task<IActionResult> GetAsync()
        {
            _log.Debug("Get all programs");

            ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetConnectionString());
            try
            {
                List<ProgramModel> programs = await programProvider.GetAllAsync();
                return Ok(new { results = programs });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("GetAllAsync", ex, _log);
            }
        }

        [HttpGet()]
        [Route("api/Programs/{id}")]
        public async Task<IActionResult> GetAsync(int id, [FromQuery] ProgramFilter programFilter)
        {
            _log.Debug("Get a program");
            ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(id));
            try
            {
                ProgramModel program = await programProvider.GetAsync(id, programFilter);
                return Ok(new { results = program });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("GetAsync(id)", ex, _log);
            }
        }

        [HttpGet()]
        [Route("api/Programs/{id}/KPIS")]
        public async Task<IActionResult> Get_KPIS(int id, [FromQuery] ProgramFilter programFilter)
        {
            _log.Debug("Get program KPIS");

            ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(id), _sqlFactory.GetDatabasePrefix(id));
            try
            {
                var programKPIs = await programProvider.GetProgramKpiAsync(id, programFilter);
                return Ok(new { results = programKPIs });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("GetProgramKpiAsync", ex, _log);
            }
        }

        [HttpGet()]
        [Route("api/Programs/{id}/Pipeline")]
        [Authorize]
        public async Task<IActionResult> Get_Pipeline(int id, [FromQuery] ProgramPipelineFilter programFilter)
        {
            _log.Debug("Get program pipeline");

            ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(id), _sqlFactory.GetDatabasePrefix(id));
            if (_sqlFactory.GetProgramConnectionString(id) == null) return Ok(new { results = "The program connection string is null" });
            if (_sqlFactory.GetDatabasePrefix(id) == null) return Ok(new { results = "The database prefix string is null" });

            try
            {
                UserModel user = this.GetUser();
                if (user.UserType == UserType.CONTRACTOR)
                {
                    programFilter.ContractorId = user.Affiliation.Value;
                }
                else if (user.UserType == UserType.AUDITOR)
                {
                    programFilter.AuditorId = user.Affiliation.Value;
                }
                List<PipelineNewModel> programPipeline = await programProvider.GetProgramPipelineAsync(id, programFilter);
                return Ok(new { results = programPipeline });
            }
            catch (Exception ex)
            {
                //return Ok(new { results = ex.Message + " on " + _sqlFactory.GetProgramConnectionString(id) });
                return ErrorHandler_Helper.ErrorMessage("GetProgramPipelineAsync", ex, _log);
            }
        }

        [HttpGet()]
        [Route("api/Programs/{id}/Issues")]
        [Authorize]
        public async Task<IActionResult> Get_ProgramIssues(int id, [FromQuery] ProgramResolveFilter programCustomerFilter)
        {
            _log.Debug("Get program issues");
            ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(id), _sqlFactory.GetDatabasePrefix(id));

            UserModel user = this.GetUser();

            if (user.UserType == UserType.CONTRACTOR)
            {
                programCustomerFilter.contractorId = user.Affiliation.Value;
            }

            try
            {
                List<ProgramIssuesModel> programsIssues = await programProvider.GetProgramIssuesAsync(id, programCustomerFilter);
                return Ok(new { results = programsIssues });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("GetProgramIssuesAsync", ex, _log);
            }
        }

        [HttpGet()]
        [Route("api/Programs/{id}/Issues/{entityName}/{entityId}")]
        public async Task<IActionResult> Get_ProgramIssuesFromHub(int id, string entityName, int entityId)
        {
            _log.Debug("Get program issues from issues hub");
            ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(id), _sqlFactory.GetDatabasePrefix(id));

            UserModel user = this.GetUser();
            try
            {
                List<IssueModel> programsIssues = await programProvider.GetProgramIssuesAsync(entityId, entityName, user.UserType);
                return Ok(new { results = programsIssues });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("Get_ProgramIssuesFromHub", ex, _log);
            }
        }

        [HttpGet()]
        [Route("api/Programs/{id}/Invoices")]
        [Authorize]
        public async Task<IActionResult> Get_Invoices(int id, [FromQuery] ProgramInvoiceFilter programFilter)
        {
            _log.Debug("Get program Invoices");
            ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(id), _sqlFactory.GetDatabasePrefix(id));

            UserModel user = this.GetUser();

            if (user.UserType == UserType.CONTRACTOR)
            {
                programFilter.contractorId = user.Affiliation.Value;
            }

            List<ProgramInvoiceModel> programsInvoices;
            try
            {
                programsInvoices = await programProvider.GetProgramInvoicesAsync(id, programFilter);
                return Ok(new { results = programsInvoices });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("GetProgramInvoicesAsync", ex, _log);
            }
        }

        [HttpGet()]
        [Route("api/Programs/{programId}/Zipcodes")]
        public async Task<IActionResult> Get_Zipcodes(int programId)
        {
            _log.Debug("Get Program zipcodes");

            ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(programId), _sqlFactory.GetDatabasePrefix(programId));
            try
            {
                var zipCodes = await programProvider.GetProgramZipCodesAsync(programId);
                return Ok(new { results = zipCodes });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("Get_Zipcodes", ex, _log);
            }
        }

        [HttpGet()]
        [Route("api/Programs/{programId}/Subsegments")]
        public async Task<IActionResult> GetSubsegmentsAsync(int programId)
        {
            _log.Debug("Get Program subsegments");

            ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(programId), _sqlFactory.GetDatabasePrefix(programId));
            try
            {
                List<string> subsegments = await programProvider.GetProgramSubSegmentsAsync(programId);
                return Ok(new { results = subsegments });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("GetSubsegments", ex, _log);
            }
        }

        [HttpGet()]
        [Route("api/Programs/{id}/Measures")]
        public async Task<IActionResult> Get_Measures(int id, [FromQuery] ProgramMeasuresFilter programMeasuresFilter)
        {
            _log.Debug("Get Program Measures");

            ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(id), _sqlFactory.GetDatabasePrefix(id));
            try
            {
                var programKPIs = await programProvider.GetProgramMeasuresAsync(id, programMeasuresFilter);
                return Ok(new { results = programKPIs });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("Get_Measures", ex, _log);
            }
        }

        [HttpPost()]
        [Route("api/Programs/{id}/KPIS")]
        public async Task<IActionResult> Post_KPIS(int id, [FromQuery] ProgramFilter programFilter)
        {
            string message = "Post KPIS";
            _log.Debug(message);
            return Ok(new { results = message });
        }

        [HttpPost()]
        [Route("api/Programs/{id}/KPIS/{KPIS_Id}")]
        public async Task<IActionResult> Post_KPIS(int id, int KPIS_Id)
        {
            string message = "KPIS_Id";
            _log.Debug(message);
            return Ok(new { results = message });
        }

        [HttpPatch()]
        [Route("api/Programs/{id}")]
        public async Task<IActionResult> Patch(int id, [FromBody] ProgramModel incomingProgramModel)
        {
            _log.Debug("Patch a program");

            ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(id), _sqlFactory.GetDatabasePrefix(id));
            ProgramCache programCache = ProgramCache.getInstance(this.Configuration);

            try
            {
                var updatedProgram = await programProvider.PatchProgram(id, incomingProgramModel);

                programCache.IsDirty(id.ToString());

                return Ok(new { results = updatedProgram });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("PatchProgram", ex, _log);
            }
        }

        [HttpGet()]
        [Route("api/Programs/{id}/GridResources")]
        public async Task<IActionResult> Get_GridResources(int id)
        {
            string message = "GridResources";
            _log.Debug(message);
            return Ok(new { results = message });
        }

        [HttpGet()]
        [Route("api/Programs/{id}/Savings")]
        [Authorize]
        public async Task<IActionResult> Get_Savings(int id, [FromQuery] ProgramMinSetFilter programFilter)
        {
            _log.Debug("Get program Savings");

            UserModel user = this.GetUser();

            if (user.UserType == UserType.CONTRACTOR)
            {
                programFilter.ContractorID = user.Affiliation.Value;
            }

            ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(id), _sqlFactory.GetDatabasePrefix(id));
            try
            {
                var programSavings = await programProvider.GetProgramSavingAsync(id, programFilter);
                return Ok(new { results = programSavings });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("Get_Savings", ex, _log);
            }
        }

        [HttpGet()]
        [Route("api/Programs/{id}/Messages")]
        public async Task<IActionResult> Get_Messages(int id, [FromQuery] ProgramMinSetFilter programFilter)
        {
            _log.Debug("Get program Invoices");
            ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(id), _sqlFactory.GetDatabasePrefix(id));
            try
            {
                var programMessages = await programProvider.GetProgramMessagesAsync(id, programFilter);
                return Ok(new { results = programMessages });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("Get_Messages", ex, _log);
            }
        }

        [HttpGet()]
        [Route("api/Programs/{id}/Feeders")]
        public async Task<IActionResult> Get_Feeders(int id)
        {
            string message = "Feeders";
            _log.Debug(message);
            return Ok(new { results = message });
        }

        [HttpGet()]
        [Route("api/Programs/{id}/GridContraints")]
        public async Task<IActionResult> Get_GridContraints(int id)
        {
            string message = "GridContraints";
            _log.Debug(message);
            return Ok(new { results = message });
        }

        [HttpPatch()]
        [Route("api/Programs/{id}/Issues/{issueId}")]
        public async Task<IActionResult> Patch_Issues(int id, int issueId)
        {
            string message = "Issues-PATCH";
            _log.Debug(message);
            return Ok(new { results = message });
        }

        [HttpPost()]
        [Route("api/Programs/{id}/Messages")]
        public async Task<IActionResult> Post_Messages(int id)
        {
            string message = "Messages-POST";
            _log.Debug(message);
            return Ok(new { results = message });
        }

        [HttpGet()]
        [Route("api/Programs/{id}/Invoices/{invoiceId}")]
        public async Task<IActionResult> Get_Invoices(int id, int invoiceId)
        {
            string message = "Invoices";
            _log.Debug(message);
            return Ok(new { results = message });
        }

        [HttpPatch()]
        [Route("api/Programs/{id}/Invoices/{invoiceId}")]
        public async Task<IActionResult> Patch_Invoices(int id, int invoiceId)
        {
            string message = "Invoices-PATCH";
            _log.Debug(message);
            return Ok(new { results = message });
        }

        [HttpGet]
        [Route("api/Programs/{id}/Divisions")]
        public async Task<IActionResult> Get_Divisions(int id)
        {
            string message = "Get Divisions";
            _log.Debug(message);

            ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(id), _sqlFactory.GetDatabasePrefix(id));
            try
            {
                var divisions = await programProvider.GetDivisions(id);
                return Ok(new { results = divisions });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("GetDivisions", ex, _log);
            }
        }

        [HttpGet]
        [Route("api/Programs/{id}/Divisions/{divisionId}")]
        public async Task<IActionResult> Get_Divisions(int id, string divisionId)
        {
            string message = "Get Division";
            _log.Debug(message);

            ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(id), _sqlFactory.GetDatabasePrefix(id));
            try
            {
                var division = await programProvider.GetDivision(id, divisionId);
                return Ok(new { results = division });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("GetDivision", ex, _log);
            }
        }

        [HttpPost]
        [Route("api/Programs/{id}/Divisions")]
        public async Task<IActionResult> Add_Division(int id, DivisionModel model)
        {
            string message = "Add Division";
            _log.Debug(message);

            ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(id), _sqlFactory.GetDatabasePrefix(id));
            try
            {
                var division = await programProvider.AddDivision(id, model);
                return Ok(new { results = division });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("AddDivision", ex, _log);
            }
        }

        [HttpPatch]
        [Route("api/Programs/{id}/Divisions/{divisionId}")]
        public async Task<IActionResult> Update_Division(int id, string divisionId, DivisionModel model)
        {
            string message = "Update Division";
            _log.Debug(message);

            model.Id = divisionId;
            ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(id), _sqlFactory.GetDatabasePrefix(id));
            try
            {
                var division = await programProvider.UpdateDivision(id, model);
                return Ok(new { results = division });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("UpdateDivision", ex, _log);
            }
        }

        [HttpDelete]
        [Route("api/Programs/{id}/Divisions/{divisionId}")]
        public async Task<IActionResult> Delete_Divisions(int id, string divisionId)
        {
            string message = "Delete Division";
            _log.Debug(message);

            ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(id), _sqlFactory.GetDatabasePrefix(id));
            try
            {
                await programProvider.DeleteDivision(id, divisionId);
                return Ok(new { results = "Division deleted successfully" });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("DeleteDivision", ex, _log);
            }
        }

        [HttpPatch()]
        [Route("api/Programs/{programId}/measures/{measuresId}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> PatchAsyncProgramMeasure(int programId, int measuresId, [FromForm] IncomingMeasuresDataModel incomingMeasuresDataModel)
        {
            string message = "Patch Program Measure";
            _log.Debug(message);

            ProgramMeasuresModel incomingProgramMeasures = new ProgramMeasuresModel();
            AWS_Helper aws_Helper = new AWS_Helper(this.Configuration);
            if (incomingMeasuresDataModel.data != null)
            {
                incomingProgramMeasures = JsonConvert.DeserializeObject<ProgramMeasuresModel>(incomingMeasuresDataModel.data);
            }
            if (incomingMeasuresDataModel.image != null)
            {
                incomingProgramMeasures.Image = await aws_Helper.SaveContentAsync(programId, incomingMeasuresDataModel.image, "images");
            }
            if (incomingMeasuresDataModel.papers != null)
            {
                incomingProgramMeasures.Papers = await aws_Helper.SaveContentAsync(programId, incomingMeasuresDataModel.papers, "papers");
            }

            ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(programId), _sqlFactory.GetDatabasePrefix(programId));
            try
            {
                var program = await programProvider.PatchProgramMeasure(programId, measuresId, incomingProgramMeasures);
                return Ok(new { results = program });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("PatchAsyncProgramMeasure", ex, _log);
            }
        }

        [HttpPost()]
        [Route("api/Programs/{programId}/measures")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> PostAsyncProgramMeasure(int programId, [FromForm] IncomingMeasuresDataModel incomingMeasuresDataModel)
        {
            try
            {
                AWS_Helper aws_Helper = new AWS_Helper(this.Configuration);
                ProgramMeasuresModel incomingProgramMeasures = new ProgramMeasuresModel();
                if (incomingMeasuresDataModel.data != null)
                {
                    incomingProgramMeasures = JsonConvert.DeserializeObject<ProgramMeasuresModel>(incomingMeasuresDataModel.data);
                }
                if (incomingMeasuresDataModel.image != null)
                {
                    incomingProgramMeasures.Image = await aws_Helper.SaveContentAsync(programId, incomingMeasuresDataModel.image, "images");
                }
                if (incomingMeasuresDataModel.papers != null)
                {
                    incomingProgramMeasures.Papers = await aws_Helper.SaveContentAsync(programId, incomingMeasuresDataModel.papers, "papers");
                }

                ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(programId), _sqlFactory.GetDatabasePrefix(programId));
                var program = await programProvider.PostProgramMeasure(programId, incomingProgramMeasures);
                return Ok(new { results = program });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("PostAsyncProgramMeasure", ex, _log);
            }
        }

        [HttpPost()]
        [Route("api/Programs/{programId}/measures/bulk")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> PostAsyncProgramMeasures(int programId, [FromForm] IncomingBulkDataFile data)
        {
            try
            {

                CsvParserOptions csvParserOptions = new CsvParserOptions(true, ',');
                MeasureUploadCSVMapper csvMapper = new MeasureUploadCSVMapper();
                CsvParser<ProgramMeasureRow> csvParser = new CsvParser<ProgramMeasureRow>(csvParserOptions, csvMapper);

                var result = csvParser
                    .ReadFromStream(data.data.OpenReadStream(), Encoding.ASCII)
                    .ToList();

                ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(programId), _sqlFactory.GetDatabasePrefix(programId));

                List<ProgramMeasuresModel> models = new List<ProgramMeasuresModel>();
                foreach (var measure in result)
                {
                    if (string.IsNullOrEmpty(measure.Result.SolutionCode)) continue;

                    ProgramMeasuresModel model = measure.Result.ToProgramMeasure();
                    if (model.Id > 0)
                    {
                        var programMeasure = await programProvider.PatchProgramMeasure(programId, model.Id, model);
                        models.Add(programMeasure);
                    } 
                    else
                    {
                        var programMeasure = await programProvider.PostProgramMeasure(programId, model);
                        models.Add(programMeasure);
                    }
                }
                
                return Ok(new { results = models });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("PostAsyncProgramMeasure", ex, _log);
            }
        }

        [HttpPost]
        [Route("api/Programs/{programId}/zipcodes")]
        public async Task<IActionResult> PostProgramZipCode(int programId, [FromBody] List<string> inZipCodes)
        {
            string message = "Add Division";
            _log.Debug(message);

            ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(programId), _sqlFactory.GetDatabasePrefix(programId));
            try
            {
                var zipCodes = await programProvider.PostProgramZipCodesAsync(programId, inZipCodes);
                return Ok(new { results = zipCodes });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("PostProgramZipCode", ex, _log);
            }
        }

        [HttpDelete]
        [Route("api/Programs/{programId}/zipcodes/{zipcode}")]
        public async Task<IActionResult> DeleteProgramZipCode(int programId, [FromQuery] string zipcode)
        {
            _log.Debug("Delete Program Zipcodes");

            ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(programId), _sqlFactory.GetDatabasePrefix(programId));
            try
            {
                var zipCodes = await programProvider.DeleteProgramZipCodesAsync(programId, zipcode);
                return Ok(new { results = zipCodes });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("Delete_Zipcodes", ex, _log);
            }
        }

        [HttpPost()]
        [Route("api/Programs/{programId}/children")]
        public async Task<IActionResult> PostChildProgram(int programId, [FromBody] int subProgramId)
        {
            _log.Debug("Add a sub program to the current program");
            ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetConnectionString());
            ProgramCache programCache = ProgramCache.getInstance(this.Configuration);
            try
            {
                var isAdded = await programProvider.AddChildProgramAsync(programId, subProgramId);

                programCache.IsDirty(programId.ToString());

                return Ok(new { results = await programProvider.GetAsync(programId, new ProgramFilter() { showchildren = false, showMetrics = false } ) });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("PostProgramChildren", ex, _log);
            }
        }
        [HttpDelete()]
        [Route("api/Programs/{programId}/children/{subProgramId}")]
        public async Task<IActionResult> DeleteChildProgram(int programId, int subProgramId)
        {
            _log.Debug("Deletes a sub program from the parent program");
            ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetConnectionString());
            ProgramCache programCache = ProgramCache.getInstance(this.Configuration);
            try
            {
                var isDeleted = await programProvider.DeleteChildProgramAsync(programId, subProgramId);

                programCache.IsDirty(programId.ToString());

                return Ok(new { results = await programProvider.GetAsync(programId, new ProgramFilter() { showchildren = false, showMetrics = false }) });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("DeleteChildProgram", ex, _log);
            }
        }
        [HttpPut]
        [Route("api/programs/{programId}/pipeline")]
        public async Task<IActionResult> UpdatePutPipelineAsync(int programId, [FromBody] List<PipelineNewModel> pipelineModels)
        {
            _log.Debug("UPDATE a pipeline");
            ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(programId), _sqlFactory.GetDatabasePrefix(programId));
            try
            {
                var pipeline = await programProvider.UpdatePipelineAsync(programId, pipelineModels);
                return Ok(new { results = pipeline });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("UpdatePutPipelineAsync", ex, _log);
            }
        }
        [HttpGet()]
        [Route("api/programs/{programId}/dashboards")]
        public async Task<IActionResult> GetAllDashboardsByProgramAsync(int programId)
        {
            _log.Debug("GET all dashboards by Program");
            ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(programId), _sqlFactory.GetDatabasePrefix(programId));
            DashboardCache dashboardCache = DashboardCache.getInstance(this.Configuration);
            try
            {
                var dashboards = await programProvider.GetAllDashboardsByProgramAsync(programId);
                List<DashboardModel> models = new List<DashboardModel>();
                foreach (var dashboardId in dashboards)
                {
                    DashboardModel model = dashboardCache.Get(dashboardId.ToString());
                    models.Add(model);
                }
                return Ok(new { results = models });
            }
            catch (Exception ex)
            {
                _log.Error("Error in getting dashboards by program", ex);
                return NotFound();
            }
        }
        [HttpPost()]
        [Route("api/programs/{programId}/dashboards")]
        public async Task<IActionResult> AddDashboardToProgram(int programId, [FromBody] IncomingDashboard incomingDashboard)
        {
            _log.Debug("ADD dashboard to Program");
            ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(programId), _sqlFactory.GetDatabasePrefix(programId));
            DashboardCache dashboardCache = DashboardCache.getInstance(this.Configuration);
            ProgramCache programCache = ProgramCache.getInstance(this.Configuration);
            try
            {
                var dashboards = await programProvider.AddDashboardToProgram(programId, incomingDashboard.Id);
                List<DashboardModel> models = new List<DashboardModel>();
                foreach (var dashboardId in dashboards)
                {
                    DashboardModel model = dashboardCache.Get(dashboardId.ToString());
                    models.Add(model);
                }
                programCache.IsDirty(programId.ToString());
                return Ok(new { results = models });
            }
            catch (Exception ex)
            {
                _log.Error("Error in adding dashboard to program", ex);
                return NotFound();
            }
        }
        [HttpDelete()]
        [Route("api/programs/{programId}/dashboards/{dashboardId}")]
        public async Task<IActionResult> RemoveDashboardFromProgram(int programId, int dashboardId)
        {
            _log.Debug("DELETE dashboards from Program");
            ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(programId), _sqlFactory.GetDatabasePrefix(programId));
            DashboardCache dashboardCache = DashboardCache.getInstance(this.Configuration);
            ProgramCache programCache = ProgramCache.getInstance(this.Configuration);
            try
            {
                var dashboards = await programProvider.RemoveDashboardFromProgram(programId, dashboardId);
                List<DashboardModel> models = new List<DashboardModel>();
                foreach (var id in dashboards)
                {
                    DashboardModel model = dashboardCache.Get(id.ToString());
                    models.Add(model);
                }
                programCache.IsDirty(programId.ToString());
                return Ok(new { results = models });
            }
            catch (Exception ex)
            {
                _log.Error("Error in removing dashboard from program", ex);
                return NotFound();
            }
        }
        [HttpPatch()]
        [Route("api/programs/{programId}/dashboards/default/{ut}")]
        public async Task<IActionResult> PatchDashboardDefaultToProgram(int programId, string ut, [FromBody] IncomingDashboard incomingDashboard)
        {
            _log.Debug("Patch dashboard default to Program");
            ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(programId), _sqlFactory.GetDatabasePrefix(programId));
            DashboardCache dashboardCache = DashboardCache.getInstance(this.Configuration);
            ProgramCache programCache = ProgramCache.getInstance(this.Configuration);

            UserType userType;
            try
            {
                userType = (UserType)Enum.Parse(typeof(UserType), ut);
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("PatchDashboardDefaultToProgram: Invalid UserType", ex, _log);
            }

            bool isValid = await programProvider.IsDashboardDefaultIdValid(incomingDashboard.Id);
            if (isValid == false)
            {
                const string message = "PatchDashboardDefaultToProgram: Invalid dashboardID";
                Exception ex = new Exception(message);
                return ErrorHandler_Helper.ErrorMessage(message, ex, _log);
            }

            try
            {
                await programProvider.PatchDashboardDefaultToProgram(programId, ut, incomingDashboard.Id);

                List<DashboardDefault> dashboardDefaults = new List<DashboardDefault>();
                var dashboards = await programProvider.GetDashboardDefaultToProgram(programId);
                foreach (var dashboard in dashboards)
                {
                    DashboardDefault dashboardDefault = new DashboardDefault();
                    dashboardDefault.UserType = dashboard.userType;
                    dashboardDefault.DashboardModel = dashboardCache.Get(dashboard.dashboardDefaultID.ToString());
                    dashboardDefaults.Add(dashboardDefault);
                }

                programCache.IsDirty(programId.ToString());

                return Ok(new { results = dashboardDefaults });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("Error in patch dashboard default to program", ex, _log);
            }
        }

        [HttpGet()]
        [Route("api/programs/{programId}/dashboards/default")]
        public async Task<IActionResult> GetDashboardDefaultToProgram(int programId)
        {
            _log.Debug("Get dashboard default to Program");
            ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(programId), _sqlFactory.GetDatabasePrefix(programId));
            DashboardCache dashboardCache = DashboardCache.getInstance(this.Configuration);
            try
            {
                List<DashboardDefault> dashboardDefaults = new List<DashboardDefault>();
                var dashboards = await programProvider.GetDashboardDefaultToProgram(programId);
                foreach (var dashboard in dashboards)
                {
                    DashboardDefault dashboardDefault = new DashboardDefault();
                    dashboardDefault.UserType = dashboard.userType;
                    dashboardDefault.DashboardModel = dashboardCache.Get(dashboard.dashboardDefaultID.ToString());
                    dashboardDefaults.Add(dashboardDefault);
                }
                return Ok(new { results = dashboardDefaults });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("Error in patch dashboard default to program", ex, _log);
            }
        }

        [HttpPost()]
        [Consumes("multipart/form-data")]
        [Route("api/programs/{programId}/marketing")]
        public async Task<IActionResult> PostMarketingMaterialAsync(int programId, [FromForm] UploadFileModel model)
        {
            try
            {
                string message = "Post a new piece of marketing material";
                _log.Debug(message);

                if (model == null || model.fileToUpload == null || model.fileToUpload.Length == 0)
                {
                    return new StatusCodeResult(StatusCodes.Status415UnsupportedMediaType);
                }

                UploadDocument_Helper helper = new UploadDocument_Helper(this.Configuration);
                FileModel fileModel = await helper.UploadToBox(this.Configuration["MaterialBoxFolderId"], model.name + Path.GetExtension(model.fileToUpload.FileName), model.fileToUpload);

                IncomingMarketingMaterial material = new IncomingMarketingMaterial
                {
                    Name = model.name,
                    AdditionalInfo = fileModel.AdditionalInfo,
                    Url = fileModel.Url
                };

                ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(programId), _sqlFactory.GetDatabasePrefix(programId));
                List<MarketingMaterialModel> marketingMaterials = await programProvider.AddMarketingMaterialAsync(programId, material);
                return Ok(new { results = marketingMaterials });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("PostMarketingMaterialAsync", ex, _log);
            }
        }
        [HttpGet()]
        [Route("api/programs/{programId}/marketing")]
        public async Task<IActionResult> GetProgramMarketingMaterialsAsync(int programId)
        {
            _log.Debug("Get all program marketing materials");
            ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(programId), _sqlFactory.GetDatabasePrefix(programId));
            try
            {
                List<MarketingMaterialModel> materials = await programProvider.GetProgramMarketingMaterialsAsync(programId);
                return Ok(new { results = materials });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("GetProgramMarketingMaterialsAsync", ex, _log);
            }
        }

        [HttpGet()]
        [Route("api/programs/{programId}/marketing/{documentId}/download")]
        public async Task<IActionResult> DownloadMarketingMaterialAsync(int programId, int documentId)
        {
            _log.Debug("Download marketing material");
            try
            {
                ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(programId), _sqlFactory.GetDatabasePrefix(programId));
                MarketingMaterialModel material = await programProvider.GetMarketingMaterialAsync(programId, documentId);
                if (material != null)
                {
                    BoxEngine_Helper helper = new BoxEngine_Helper(this.Configuration);
                    StoredItem file = await helper.GetFileAsync(material.AdditionalInfo);
                    if (file != null)
                    {
                        string mediaType = null;
                        var fileContent = new MemoryStream(file.Contents);
                        switch (file.Extension.ToUpper())
                        {
                            case "PDF":
                                mediaType = "application/pdf";
                                break;
                            case "PNG":
                                mediaType = "image/png";
                                break;
                            case "JPG":
                            case "JPEG":
                                mediaType = "image/jpeg";
                                break;
                            default:
                                _log.Debug("Invalid media type format!!");
                                break;
                        }

                        return File(fileContent, mediaType);
                    }
                    else
                    {
                        return new StatusCodeResult(StatusCodes.Status404NotFound);
                    }
                }
                else
                {
                    return new StatusCodeResult(StatusCodes.Status404NotFound);
                }
            }
            catch (Exception ex)
            {
                _log.Debug("DownloadMarketingMaterialAsync(): " + ex.Message);
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
        }

        [HttpDelete()]
        [Route("api/programs/{programId}/marketing/{documentId}")]
        public async Task<IActionResult> DeleteMarketingMaterialAsync(int programId, int documentId)
        {
            _log.Debug("Delete a marketing material");
            List<MarketingMaterialModel> materials = null;
            ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(programId), _sqlFactory.GetDatabasePrefix(programId));
            try
            {
                MarketingMaterialModel material = await programProvider.GetMarketingMaterialAsync(programId, documentId);
                if (material != null)
                {
                    BoxEngine_Helper helper = new BoxEngine_Helper(this.Configuration);
                    bool isDeleted = await helper.DeleteFileAsync(material.AdditionalInfo);
                    if (isDeleted)
                    {
                        materials = await programProvider.DeleteMarketingMaterialAsync(programId, documentId);
                    }
                }
                return Ok(new { results = materials });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("DeleteMarketingMaterialAsync", ex, _log);
            }
        }

        [HttpGet()]
        [Route("api/programs/{programId}/measures/category")]
        public async Task<IActionResult> GetProgramMeasureCategoryAsync(int programId)
        {
            _log.Debug("Get all program measures category");
            ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(programId), _sqlFactory.GetDatabasePrefix(programId));
            try
            {
                var measureSavings = await programProvider.GetMeasuresCategory(programId);
                return Ok(new { results = measureSavings });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("GetProgramMeasureCategoryAsync", ex, _log);
            }
        }

        [HttpGet()]
        [Route("api/programs/{programId}/measures/subsegment")]
        public async Task<IActionResult> GetProgramMeasureSubsegmentAsync(int programId)
        {
            _log.Debug("Get all program measures subsegment");
            ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(programId), _sqlFactory.GetDatabasePrefix(programId));
            try
            {
                List<MeasureSavings> measureSavings = await programProvider.GetMeasuresSubsegment(programId);
                return Ok(new { results = measureSavings });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("GetProgramMeasureSubsegmentAsync", ex, _log);
            }
        }
        
        [HttpGet()]
        [Route("api/programs/{programId}/measures/potential")]
        public async Task<IActionResult> GetProgramMeasurePotential(int programId)
        {
            _log.Debug("Get all program measures potential");
            ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(programId), _sqlFactory.GetDatabasePrefix(programId));
            try
            {
                var potentialSavings = await programProvider.GetMeasuresPotential(programId);
                return Ok(new { results = potentialSavings });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("GetProgramMeasurePotential", ex, _log);
            }
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        [Route("api/programs/{programId}/documents")]
        public async Task<IActionResult> PostProgramDocumentAsync(int programId, [FromForm] UploadFileModel programDocument)
        {
            try
            {
                string message = "Post a program document";
                _log.Debug(message);

                ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(programId), _sqlFactory.GetDatabasePrefix(programId));
                UploadDocument_Helper helper = new UploadDocument_Helper(this.Configuration);

                ProgramCache programCache = ProgramCache.getInstance(this.Configuration);
                ProgramModel program = programCache.Get(programId.ToString());

                List<int> programIds = new List<int>();
                programIds.Add(programId);
                
                if (program.Children != null && program.Children.Count > 0)
                {
                    programIds.AddRange(program.Children.Select(x => x.Id).ToList());
                }

                foreach(int i in programIds)
                {
                    IncomingDocument document = new IncomingDocument
                    {
                        name = programDocument.name,
                        isRequired = programDocument.isRequired                        
                    };

                    if (programDocument.fileToUpload != null && programDocument.fileToUpload.Length > 0)
                    {
                        FileModel fileModel = await helper.UploadToBox(this.Configuration["DocumentsBoxFolderId"], programDocument.name + "_" + i + Path.GetExtension(programDocument.fileToUpload.FileName), programDocument.fileToUpload);

                        document.fileSourceId = fileModel.AdditionalInfo;
                        document.url = fileModel.Url;
                    }

                    await programProvider.AddProgramDocumentAsync(i, document);
                }

                List<ProgramDocumentModel> documents = await programProvider.GetAllProgramDocumentsAsync(programId);
                ProgramDocumentModel programRecentDocument = documents.OrderByDescending(i => i.Id).FirstOrDefault();

                return Ok(new { results = programRecentDocument });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("PostProgramDocumentAsync", ex, _log);
            }
        }

        [HttpGet]
        [Route("api/programs/{programId}/documents")]
        public async Task<IActionResult> GetAllProgramDocumentsAsync(int programId)
        {
            _log.Debug("Get all program documents");
            ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(programId), _sqlFactory.GetDatabasePrefix(programId));
            try
            {
                List<ProgramDocumentModel> documents = await programProvider.GetAllProgramDocumentsAsync(programId);
                return Ok(new { results = documents });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("GetAllProgramDocumentsAsync", ex, _log);
            }
        }

        [HttpGet]
        [Route("api/programs/{programId}/documents/{documentId}")]
        public async Task<IActionResult> GetProgramDocumentAsync(int programId, int documentId)
        {
            _log.Debug("Get all program documents");
            ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(programId), _sqlFactory.GetDatabasePrefix(programId));
            try
            {
                ProgramDocumentModel documents = await programProvider.GetProgramDocumentAsync(documentId);
                return Ok(new { results = documents });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("GetProgramDocumentAsync", ex, _log);
            }
        }

        [HttpDelete()]
        [Route("api/programs/{programId}/documents/{programDocumentId}")]
        public async Task<IActionResult> DeleteProgramDocumentAsync(int programId, int programDocumentId)
        {
            _log.Debug("Delete a program document");
            List<ProgramDocumentModel> documents = null;
            ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(programId), _sqlFactory.GetDatabasePrefix(programId));
            try
            {
                ProgramProvider_File.DocumentHub document = await programProvider.GetDocumentStoreAsync(programDocumentId);
                if (document != null)
                {
                    BoxEngine_Helper helper = new BoxEngine_Helper(this.Configuration);
                    await helper.DeleteFileAsync(document.file_source_id);
                }
                documents = await programProvider.DeleteProgramDocumentAsync(programId, programDocumentId);

                return Ok(new { results = documents });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("DeleteProgramDocumentAsync", ex, _log);
            }
        }

        [HttpPut]
        [Consumes("multipart/form-data")]
        [Route("api/programs/{programId}/documents/{programDocumentId}")]
        public async Task<IActionResult> UpdateProgramDocumentAsync(int programId, int programDocumentId, [FromForm] UploadFileModel programDocument)
        {
            try
            {
                string message = "Update a program document";
                _log.Debug(message);

                ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(programId), _sqlFactory.GetDatabasePrefix(programId));
                ProgramProvider_File.DocumentHub document = await programProvider.GetDocumentStoreAsync(programDocumentId);

                UploadDocument_Helper helper = new UploadDocument_Helper(this.Configuration);
                BoxEngine_Helper boxHelper = new BoxEngine_Helper(this.Configuration);

                if (document != null)
                {
                    await boxHelper.DeleteFileAsync(document.file_source_id);
                }

                IncomingDocument updateDocument = new IncomingDocument
                {
                    name = programDocument.name,
                    isRequired = programDocument.isRequired,
                };

                if (programDocument.fileToUpload != null && programDocument.fileToUpload.Length > 0)
                {
                    FileModel fileModel = await helper.UploadToBox(this.Configuration["DocumentsBoxFolderId"], programDocument.name + "_" + programId + Path.GetExtension(programDocument.fileToUpload.FileName), programDocument.fileToUpload);

                    updateDocument.fileSourceId = fileModel.AdditionalInfo;
                    updateDocument.url = fileModel.Url;
                }

                ProgramDocumentModel model = await programProvider.UpdateProgramDocumentAsync(programId, programDocumentId, updateDocument);
                return Ok(new { results = model });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("PatchProgramDocumentAsync", ex, _log);
            }
        }

        [HttpGet]
        [Route("api/programs/{programId}/measures/subsegment/type")]
        public async Task<IActionResult> GetMeasuresSubsegmentTypeAsync(int programId)
        {
            _log.Debug("Get all program documents");
            ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(programId), _sqlFactory.GetDatabasePrefix(programId));
            try
            {
                List<SubsegmentTypeModel> subsegmentTypeModels = await programProvider.GetAllMeasuresSubsegmentTypes(programId);
                return Ok(new { results = subsegmentTypeModels });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("GetMeasuresSubsegmentTypeAsync", ex, _log);
            }
        }

        [HttpGet]
        [Route("api/programs/{programId}/checklist/{context}")]
        public async Task<IActionResult> GetProgramAllChecklistAsync(int programId, string context)
        {
            _log.Debug("Get all program checklist");
            ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(programId), _sqlFactory.GetDatabasePrefix(programId));
            try
            {
                var checklists = await programProvider.GetProgramChecklistAsync(programId, context);
                return Ok(new { results = checklists });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("GetProgramAllChecklistAsync", ex, _log);
            }
        }

        [HttpPatch()]
        [Route("api/Programs/{id}/checklist/{itemId}")]
        public async Task<IActionResult> PatchProgramChecklistAsync(int id, int itemId, [FromBody] ChecklistItemModel checklistItem)
        {
            _log.Debug("Patch program checklist");

            ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(id), _sqlFactory.GetDatabasePrefix(id));
            try
            {
                var checklist = await programProvider.PatchProgramChecklistAsync(id, itemId, checklistItem);
                return Ok(new { results = checklist });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("PatchProgramChecklistAsync", ex, _log);
            }
        }


        [HttpPost]
        [Route("api/Programs/{programId}/checklist")]
        public async Task<IActionResult> PostProgramChecklistAsync(int programId, [FromBody] ChecklistItemModel checklistItem)
        {
            string message = "Add program checklist";
            _log.Debug(message);

            ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(programId), _sqlFactory.GetDatabasePrefix(programId));
            try
            {
                var checklist = await programProvider.PostProgramChecklistAsync(programId, checklistItem);
                return Ok(new { results = checklist });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("PostProgramChecklistAsync", ex, _log);
            }
        }

        [HttpDelete]
        [Route("api/Programs/{programId}/checklist/{itemId}")]
        public async Task<IActionResult> DeleteProgramChecklist(int programId, int itemId)
        {
            string message = "Delete program checklist";
            _log.Debug(message);

            ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(programId), _sqlFactory.GetDatabasePrefix(programId));
            try
            {
                var checklists = await programProvider.DeleteProgramChecklist(programId, itemId);
                return Ok(new { results = checklists });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("DeleteProgramChecklist", ex, _log);
            }
        }

        // for demo
        [HttpGet]
        [Route("api/programs/{programId}/zipcodeinstallation")]
        public async Task<IActionResult> GetProgramZipcodeInstalltionAsync(int programId)
        {
            _log.Debug("Get all program zipcodeinstallation");
            ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(programId), _sqlFactory.GetDatabasePrefix(programId));
            try
            {
                var zipInstalls = await programProvider.GetZipcodeInstallation(programId);
                return Ok(new { results = zipInstalls });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("GetProgramZipcodeInstalltionAsync", ex, _log);
            }
        }

        [HttpGet]
        [Route("api/programs/{programId}/treadingmeasures")]
        public async Task<IActionResult> GetTrendingMeasuresAsync(int programId)
        {
            _log.Debug("Get all program Trending Measures");
            ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(programId), _sqlFactory.GetDatabasePrefix(programId));
            try
            {
                var trends = await programProvider.GetTrendingMeasures(programId);
                return Ok(new { results = trends });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("GetTrendingMeasuresAsync", ex, _log);
            }
        }

        [HttpGet]
        [Route("api/programs/{programId}/goals")]
        public async Task<IActionResult> GetProgramGoalsAsync(int programId)
        {
            _log.Debug("Get goals for each month of the program");

            ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(programId), _sqlFactory.GetDatabasePrefix(programId));

            try
            {
                var goals = await programProvider.GetProgramGoals(programId);
                return Ok(new { results = goals });
            }
            catch(Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("GetProgramGoalsAsync", ex, _log);
            }
        }

        [HttpPost]
        [Route("api/programs/{programId}/goals")]
        public async Task<IActionResult> AddProgramGoalAsync(int programId, [FromBody] ProgramGoal incomingGoal)
        {
            _log.Debug("Add a goal for a specific month");

            ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(programId), _sqlFactory.GetDatabasePrefix(programId));

            try
            {
                var goals = await programProvider.PostProgramGoalAsync(programId, incomingGoal);
                return Ok(new { results = goals });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("AddProgramGoalAsync", ex, _log);
            }
        }

        [HttpPut]
        [Route("api/programs/{programId}/goals")]
        public async Task<IActionResult> UpdateProgramGoalAsync(int programId, [FromBody] ProgramGoal incomingGoal)
        {
            _log.Debug("Update a goal for a specific month");

            ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(programId), _sqlFactory.GetDatabasePrefix(programId));

            try
            {
                var goals = await programProvider.PutProgramGoalAsync(programId, incomingGoal);
                return Ok(new { results = goals });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("UpdateProgramGoalAsync", ex, _log);
            }
        }

        [HttpDelete]
        [Route("api/programs/{programId}/goals/{year}/{month}")]
        public async Task<IActionResult> DeleteProgramGoalAsync(int programId, int year, int month)
        {
            _log.Debug("Delete a goal for a specific month");

            ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(programId), _sqlFactory.GetDatabasePrefix(programId));

            try
            {
                var goals = await programProvider.DeleteProgramGoalAsync(programId, year, month);
                return Ok(new { results = goals });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("DeleteProgramGoalAsync", ex, _log);
            }
        }

        [HttpGet]
        [Route("api/programs/{programId}/goals/actuals/{goaltype}")]
        public async Task<IActionResult> GetForecastGoalsAsync(int programId, string goaltype)
        {
            _log.Debug("get forecast goals by goal type");

            ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(programId), _sqlFactory.GetDatabasePrefix(programId));

            try
            {
                ForecastGoalType forecastGoalType = (ForecastGoalType) Enum.Parse(typeof(ForecastGoalType), goaltype.ToUpper());
                var forecastGoals = await programProvider.GetForecastGoalsAsync(programId, forecastGoalType);
                return Ok(new { results = forecastGoals });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("GetForecastGoalsAsync", ex, _log);
            }
        }

        [HttpGet]
        [Route("api/programs/{programId}/KPI/Forecasting")]
        public async Task<IActionResult> GetKPIForecastAccuracysync(int programId)
        {
            _log.Debug("get forecast goals by goal type");

            ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(programId), _sqlFactory.GetDatabasePrefix(programId));

            try
            {
                KPIForecasting forecastingAccuracy = new KPIForecasting()
                {
                    KWData = await programProvider.GetForecastGoalsAsync(programId, ForecastGoalType.KW),
                    KWhData = await programProvider.GetForecastGoalsAsync(programId, ForecastGoalType.KWH),
                    ThermData = await programProvider.GetForecastGoalsAsync(programId, ForecastGoalType.THERMS),
                    IncentiveData = await programProvider.GetForecastGoalsAsync(programId, ForecastGoalType.INCENTIVES)
                };

                return Ok(new { results = forecastingAccuracy });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("GetForecastGoalsAsync", ex, _log);
            }
        }

        [HttpGet]
        [Route("api/programs/{programId}/KPI/Forecasting/{unit}")]
        public async Task<IActionResult> GetKPIForecastAccuracyByUnit(int programId, string unit)
        {
            _log.Debug("get forecast goals by goal type");

            ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(programId), _sqlFactory.GetDatabasePrefix(programId));

            try
            {
                if (unit.Equals("kW")) return Ok(new
                {
                    results = await programProvider.GetForecastGoalsAsync(programId, ForecastGoalType.KW),
                });
                else if (unit.Equals("kWh")) return Ok(new
                {
                    results = await programProvider.GetForecastGoalsAsync(programId, ForecastGoalType.KWH),
                });
                else if (unit.Equals("therms")) return Ok(new
                {
                    results = await programProvider.GetForecastGoalsAsync(programId, ForecastGoalType.THERMS),
                });
                else if (unit.Equals("incentives")) return Ok(new
                {
                    results = await programProvider.GetForecastGoalsAsync(programId, ForecastGoalType.INCENTIVES)
                });

                return BadRequest(new { results = "Invalid unit" });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("GetForecastGoalsAsync", ex, _log);
            }
        }

        [HttpGet]
        [Route("api/programs/{programId}/checklist/user")]
        [Authorize]
        public async Task<IActionResult> GetChecklistForUser(int programId, [FromQuery] ToDoQuery query)
        {
            _log.Debug("get to do list by user type");

            ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(programId), _sqlFactory.GetDatabasePrefix(programId));

            UserModel user = this.GetUser();
            if (user.UserType == UserType.CONTRACTOR)
            {
                query.contractorId = user.Affiliation.Value;
            }
            else if (user.UserType == UserType.AUDITOR )
            {
                query.auditorId = user.Affiliation.Value;
            }

            try
            {
                List<ChecklistItemModel> toDoList = await programProvider.GetUserChecklist(programId, query);
                return Ok(new { results = toDoList });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("GetChecklistForUser", ex, _log);
            }
        }

        [HttpGet]
        [Route("api/programs/{programId}/calendar")]
        [Authorize]
        public async Task<IActionResult> GetAppointmentsForUser(int programId, [FromQuery] CalendarQuery query)
        {

            _log.Debug("get calendar items");

            ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(programId), _sqlFactory.GetDatabasePrefix(programId));

            UserModel user = this.GetUser();
            if (user.UserType == UserType.CONTRACTOR)
            {
                query.contractorId = user.Affiliation.Value;
            }
            else if (user.UserType == UserType.AUDITOR)
            {
                query.auditorId = user.Affiliation.Value;
            }

            try
            {
                List<BaseProjectModel> projects = await programProvider.GetAppointments(programId, query);
                
                return Ok(new { results = projects });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("GetAppointmentsForUser", ex, _log);
            }
        }

        [HttpGet]
        [Route("api/programs/{programId}/notes/{entityType}/{entityId}")]
        [Authorize]
        public async Task<IActionResult> GetProgramNotes(int programId, string entityType, int entityId, [FromQuery] IncomingNotes incomingNotes)
        {
            _log.Debug("get to notes");

            ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(programId), _sqlFactory.GetDatabasePrefix(programId));
            UserCache userCache = UserCache.getInstance(this.Configuration);
            try
            {
                List<EntityNotesModel> entityNotes = await programProvider.GetNotesAsync(programId, entityType, entityId, incomingNotes);
                foreach (var note in entityNotes)
                {
                    var user = userCache.Get(note.author.userId);
                    if (user != null)
                    {
                        note.author.firstName = user.FirstName;
                        note.author.lastName = user.LastName;
                        if(user.Affiliation != null)
                        {
                            note.author.affiliationCompany =  await GetAffiliationName(programId, user.Affiliation.Value);
                        }
                    }
                }   
                
                return Ok(new { results = entityNotes });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("GetProgramNotes", ex, _log);
            }
        }

        [HttpPost]
        [Route("api/Programs/{programId}/notes")]
        [Authorize]
        public async Task<IActionResult> PostProgramNotes(int programId, [FromBody] IncomingNotes incomingNotes)
        {
            string message = "Add Notes";
            _log.Debug(message);

            ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(programId), _sqlFactory.GetDatabasePrefix(programId));
            try
            {
                UserModel userModel = this.GetUser();
                incomingNotes.userid = userModel.UserId;
                var note = await programProvider.PostNotesAsync(programId, incomingNotes);

                UserCache userCache = UserCache.getInstance(this.Configuration);
                var user = userCache.Get(note.author.userId);
                if (user != null)
                {
                    note.author.firstName = userModel.FirstName;
                    note.author.lastName = userModel.LastName;
                }
                return Ok(new { results = note });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("PostProgramNotes", ex, _log);
            }
        }

        [HttpGet]
        [Route("api/Programs/{programId}/validations")]
        public async Task<IActionResult> GetProgramValidationsAsync(int programId)
        {
            string message = "Get Program Validations";
            _log.Debug(message);

            ValidationProvider validationProvider = new ValidationProvider(this.Configuration, _sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(programId), _sqlFactory.GetDatabasePrefix(programId));
            try
            {
                var validations = await validationProvider.GetAllValidationsAsync(programId);
                return Ok(new { results = validations });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("GetProgramValidationsAsync", ex, _log);
            }
        }

        [HttpPut]
        [Route("api/Programs/{programId}/validations/{validationId}")]
        public async Task<IActionResult> PutProgramValidationAsync(int programId, int validationId, [FromBody] IncomingValidation validation)
        {
            string message = "Update a Program Validation";
            _log.Debug(message);

            ValidationProvider validationProvider = new ValidationProvider(this.Configuration, _sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(programId), _sqlFactory.GetDatabasePrefix(programId));
            try
            {
                var validations = await validationProvider.UpdateValidationAsync(programId, validation);
                return Ok(new { results = validations });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("PutProgramValidationAsync", ex, _log);
            }
        }

        [HttpDelete]
        [Route("api/Programs/{programId}/validations/{validationId}")]
        public async Task<IActionResult> DeleteProgramValidationAsync(int programId, int validationId)
        {
            string message = "Delete a Program Validation";
            _log.Debug(message);

            ValidationProvider validationProvider = new ValidationProvider(this.Configuration, _sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(programId), _sqlFactory.GetDatabasePrefix(programId));
            try
            {
                await validationProvider.DeleteValidationAsync(validationId);
                return Ok(new { results = "SUCCESS" });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("DeleteProgramValidationAsync", ex, _log);
            }
        }

        [HttpPost]
        [Route("api/Programs/{programId}/validations")]
        public async Task<IActionResult> AddProgramValidationAsync(int programId, [FromBody] IncomingValidation validation)
        {
            string message = "Add a Program Validation";
            _log.Debug(message);

            ValidationProvider validationProvider = new ValidationProvider(this.Configuration, _sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(programId), _sqlFactory.GetDatabasePrefix(programId));
            try
            {
                var newValidation = await validationProvider.AddValidationAsync(programId, validation);
                return Ok(new { results = newValidation });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("PutProgramValidationAsync", ex, _log);
            }
        }

        [HttpGet]
        [Route("api/Programs/{programId}/photos/{entityName}/{entityId}")]
        public async Task<IActionResult> GetPhotos(int programId, string entityName, int entityId)
        {
            string message = "Add Photo";
            _log.Debug(message);

            ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(programId), _sqlFactory.GetDatabasePrefix(programId));
            try
            {
                var photos = await programProvider.GetPhotos(entityName, entityId);
                return Ok(new { results = photos });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("GetPhotos", ex, _log);
            }
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        [Route("api/Programs/{programId}/photos/{entityName}/{entityId}")]
        public async Task<IActionResult> AddPhoto(int programId, string entityName, int entityId, [FromForm] UploadFileModel uploadFileModel)
        {
            string message = "Add Photo";
            _log.Debug(message);

            //We are only going to use AWS for photos so we can access them directly from UI
            //until we are directed otherwise
            AWS_Helper aws_Helper = new AWS_Helper(this.Configuration);
            string name = entityName + "_" + entityId + "_" + uploadFileModel.name;
            string fileURL = await aws_Helper.SaveContentAsync(programId, uploadFileModel.image, name);
            ContextName context;
            if (Enum.TryParse(entityName, out context))
            {
                PhotoModel model = new PhotoModel()
                {
                    PhotoName = uploadFileModel.name,
                    DocumentURL = fileURL,
                    StorageLocation = DocumentStorageType.AWS,
                    EntityId = entityId,
                    Context = context
                };

                ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(programId), _sqlFactory.GetDatabasePrefix(programId));
                try
                {
                    var photo = await programProvider.AddPhoto(model);
                    return Ok(new { results = photo });
                }
                catch (Exception ex)
                {
                    return ErrorHandler_Helper.ErrorMessage("AddPhoto", ex, _log);
                }
            } else
            {
                return ErrorHandler_Helper.ErrorMessage("Context invalid: " + entityName, null, _log);
            }
        }

        [HttpDelete]
        [Route("api/Programs/{programId}/photos/{photoId}")]
        public async Task<IActionResult> DeletePhoto(int programId, int photoId)
        {
            string message = "Delete Photo";
            _log.Debug(message);

            ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(programId), _sqlFactory.GetDatabasePrefix(programId));
            try
            {
                await programProvider.DeletePhoto(photoId);
                return Ok(new { results = "Deleted" });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("DeletePhoto", ex, _log);
            }
        }

        [HttpGet]
        [Route("api/Programs/{programId}/Reports/Generated/{entityName}/{entityId}")]
        public async Task<IActionResult> GetGeneratedReports(int programId, string entityName, int entityId)
        {
            string message = "Get Reports";
            _log.Debug(message);

            ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(programId), _sqlFactory.GetDatabasePrefix(programId));
            try
            {
                var reports = await programProvider.GetGeneratedReports(entityName, entityId);
                return Ok(new { results = reports });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("GetReports", ex, _log);
            }
        }

        [HttpGet]
        [Route("api/Programs/{programId}/Reports/Generated")]
        public async Task<IActionResult> GetAllGeneratedReports(int programId, [FromQuery] ReportQuery query)
        {
            string message = "Get Reports";
            _log.Debug(message);

            ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(programId), _sqlFactory.GetDatabasePrefix(programId));
            try
            {
                var reports = await programProvider.GetAllGeneratedReports(programId, query.contractorId, query.auditorId);
                return Ok(new { results = reports });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("GetReports", ex, _log);
            }
        }


        [HttpPost]
        [Consumes("multipart/form-data")]
        [Route("api/Programs/{programId}/Reports/Generated/{entityName}/{entityId}")]
        public async Task<IActionResult> AddGeneratedReport(int programId, string entityName, int entityId, [FromForm] UploadFileModel uploadFileModel)
        {
            string message = "Add Generated Report";
            _log.Debug(message);

            BoxEngine_Helper box_Helper = new BoxEngine_Helper(this.Configuration);
            string name = entityName + "_" + entityId + "_" + uploadFileModel.name;

            UploadDocument_Helper helper = new UploadDocument_Helper(this.Configuration);
            FileModel fileModel = await helper.UploadToBox(this.Configuration["DocumentsBoxFolderId"], name, uploadFileModel.fileToUpload);

            ContextName context;
            if (Enum.TryParse(entityName, out context))
            {
                ReportModel model = new ReportModel()
                {
                    ReportName = uploadFileModel.name,
                    DocumentURL = fileModel.Url,
                    StorageLocation = DocumentStorageType.BOX,
                    FileSourceId = fileModel.AdditionalInfo,
                    EntityId = entityId,
                    Context = context
                };

                ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(programId), _sqlFactory.GetDatabasePrefix(programId));
                try
                {
                    var report = await programProvider.AddGeneratedReport(model);
                    return Ok(new { results = report });
                }
                catch (Exception ex)
                {
                    return ErrorHandler_Helper.ErrorMessage("AddReport", ex, _log);
                }
            }
            else
            {
                return ErrorHandler_Helper.ErrorMessage("Context invalid: " + entityName, null, _log);
            }
        }

        [HttpDelete]
        [Route("api/Programs/{programId}/reports/generated/{reportId}")]
        public async Task<IActionResult> DeleteGeneratedReport(int programId, int reportId)
        {
            string message = "Delete Report";
            _log.Debug(message);

            ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(programId), _sqlFactory.GetDatabasePrefix(programId));
            try
            {
                await programProvider.DeleteGeneratedReport(reportId);
                return Ok(new { results = "Deleted" });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("Delete Report", ex, _log);
            }
        }

        [HttpGet]
        [Route("api/Programs/{programId}/trends")]
        public async Task<IActionResult> GeWeekly_kWh(int programId, [FromQuery] MeasureFilter query)
        {
            string message = "Get weekly kwh";
            _log.Debug(message);

            ProgramProvider programProvider = new ProgramProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(programId), _sqlFactory.GetDatabasePrefix(programId));
            try
            {
                var weekValueModels = await programProvider.GetWeeklyTrendData(programId, query);
                return Ok(new { results = weekValueModels });
            }
            catch (Exception ex)
            {
                return ErrorHandler_Helper.ErrorMessage("GeWeekly_kWh", ex, _log);
            }
        }

        private async Task<string> GetAffiliationName(int programId, int affiliationValue)
        {
            ContractorProvider contractorProvider = new ContractorProvider(_sqlFactory.GetWrapper(), _sqlFactory.GetProgramConnectionString(programId), _sqlFactory.GetDatabasePrefix(programId));
            var contractor = await contractorProvider.GetContractorAsync(programId, affiliationValue);
            return contractor.Name;
        }
    }
}