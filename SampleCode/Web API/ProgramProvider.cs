using EnergyEfficiency;
using EnergyEfficiency.Models;
using log4net;
using Microsoft.AspNetCore.Mvc.Formatters;
using OnlinePortalWebServiceCore.Models;
using SQLService;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using static EnergyEfficiency.Enums;
using static SystemCore.Enums;

namespace ProgramProvider_File
{
    public class ProgramProvider
    {
        private ISQL_Wrapper _sql_Wrapper;
        private string _connectionString = string.Empty;
        private string _prefix = string.Empty;
        private ILog _log = LogManager.GetLogger(typeof(ProgramProvider));

        public ProgramProvider()
        {

        }

        public ProgramProvider(ISQL_Wrapper sql_Wrapper, string connectionString, string prefix = null)
        {
            _sql_Wrapper = sql_Wrapper;
            _connectionString = connectionString;
            _prefix = prefix;
        }

        public async Task<List<ProgramModel>> GetAllAsync()
        {
            List<ProgramModel> programModels = new List<ProgramModel>();

            List<Program> programs = await _sql_Wrapper.GetItemsAsync<Program>(_connectionString, "spGetPrograms");
            foreach (var program in programs)
            {
                ProgramType type;
                Enum.TryParse<ProgramType>(program.type, out type);

                ProgramModel programModel = new ProgramModel
                {
                    Id = program.programID,
                    Name = program.programName,
                    Sponsor = program.utilityName,
                    Type = type,
                    ParentID = program.parentID
                };

                DateRange customerDates = new DateRange(program.programStartDate, program.programEndDate);
                programModel.Dates = customerDates;

                Geography geography = new Geography();
                geography.Lat = program.geoLat;
                geography.Long = program.geoLong;
                geography.Zoom = program.geoZoom;
                programModel.Geography = geography;

                programModel.Children = GetChildrenPrograms(program.programID, programs, false);

                programModels.Add(programModel);
            }

            return programModels;
        }

        private List<ProgramModel> GetChildrenPrograms(int programID, List<Program> programs, bool showChildren)
        {
            List<ProgramModel> children = new List<ProgramModel>();
            if (programID != 0)
            {
                List<Program> subPrograms = programs.Where(y => y.parentID == programID).ToList();

                foreach (Program program in subPrograms)
                {
                    ProgramType type;
                    Enum.TryParse<ProgramType>(program.type, out type);

                    ProgramModel programModel = new ProgramModel
                    {
                        Id = program.programID,
                        Name = program.programName,
                        Sponsor = program.utilityName,
                        Type = type,
                        ParentID = program.parentID
                    };

                    DateRange customerDates = new DateRange(program.programStartDate, program.programEndDate);
                    programModel.Dates = customerDates;

                    Geography geography = new Geography();
                    geography.Lat = program.geoLat;
                    geography.Long = program.geoLong;
                    geography.Zoom = program.geoZoom;
                    programModel.Geography = geography;

                    if (showChildren)
                    {
                        programModel.Children = GetChildrenPrograms(program.programID, programs, showChildren);
                    }

                    children.Add(programModel);
                }
            }

            return children;
        }

        private async Task<List<ProgramModel>> GetChildrenProgramsAsync(int programID, bool showChildren)
        {
            List<Program> programs = await _sql_Wrapper.GetItemsAsync<Program>(_connectionString, "spGetPrograms");
            return GetChildrenPrograms(programID, programs, showChildren);
        }

        public async Task<ProgramModel> GetAsync(int id, ProgramFilter filter)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            SqlParameter param = new SqlParameter
            {
                ParameterName = "@ProgramId",
                Value = id
            };
            parameters.Add(param);

            List<Program> programs = await _sql_Wrapper.GetItemsAsync<Program>(_connectionString, "spGetProgram", parameters);
            if (programs.Count == 0)
            {
                string message = "spGetProgram: no programs found";
                _log.Error(message);
                return null;
            }

            var program = programs.FirstOrDefault<Program>();
            ProgramType type;
            Enum.TryParse<ProgramType>(program.type, out type);

            ProgramModel programModel = new ProgramModel
            {
                Id = program.programID,
                Name = program.programName,
                Sponsor = program.utilityName,
                Type = type,
                ParentID = program.parentID
            };

            DateRange customerDates = new DateRange(program.programStartDate, program.programEndDate);
            programModel.Dates = customerDates;

            Geography geography = new Geography();
            geography.Lat = program.geoLat;
            geography.Long = program.geoLong;
            geography.Zoom = program.geoZoom;
            programModel.Geography = geography;

            if (!filter.showMetrics.HasValue || (filter.showMetrics.HasValue && filter.showMetrics == true))
            {
                int year = DateTime.Now.Year;

                if (filter.metricsYear.HasValue && filter.metricsYear.Value > 0)
                {
                    year = filter.metricsYear.Value;
                }


                Metrics metrics = new Metrics();
                List<Year> years = new List<Year>();

                List<SqlParameter> paramMetric = new List<SqlParameter>();
                paramMetric.Add(new SqlParameter { ParameterName = "@ProgramId", Value = id });
                paramMetric.Add(new SqlParameter { ParameterName = "@YearId", Value = year });
                List<ProgramMetric> programMeterics = await _sql_Wrapper.GetItemsAsync<ProgramMetric>(_connectionString, "spGetProgramMetrics", paramMetric);

                Year yearMetrics = new Year();
                yearMetrics.MetricYear = year;

                int quarterCnt = 0;
                foreach (var programMetric in programMeterics)
                {
                    Quarter quarter = new Quarter();
                    PowerMetric kw = new PowerMetric();
                    kw.Goal = programMetric.kWGoal;
                    kw.Complete = programMetric.kWComplete;
                    kw.InProgress = programMetric.kWInProgress;
                    quarter.kw = kw;

                    PowerMetric kwh = new PowerMetric();
                    kwh.Goal = programMetric.kWhGoal;
                    kwh.Complete = programMetric.kWhComplete;
                    kwh.InProgress = programMetric.kWhInProgress;
                    quarter.kwh = kwh;

                    PowerMetric therm = new PowerMetric();
                    therm.Goal = programMetric.thermGoal;
                    therm.Complete = programMetric.thermComplete;
                    therm.InProgress = programMetric.thermInProgress;
                    quarter.therms = therm;

                    PowerMetric incentives = new PowerMetric();
                    incentives.Goal = programMetric.incentiveGoal;
                    incentives.Complete = programMetric.incentivesComplete;
                    incentives.InProgress = programMetric.incentivesInProgress;
                    quarter.incentives = incentives;

                    PowerMetric costs = new PowerMetric();
                    costs.Goal = programMetric.costGoal;
                    costs.Complete = programMetric.costComplete;
                    costs.InProgress = programMetric.costInProgress;
                    quarter.cost = costs;

                    PowerMetric copay = new PowerMetric();
                    copay.Goal = programMetric.copayGoal;
                    copay.Complete = programMetric.copayComplete;
                    copay.InProgress = programMetric.copayInProgress;
                    quarter.copay = copay;

                    if (quarterCnt == 0)
                    {
                        yearMetrics.Q1 = quarter;
                    }
                    else if (quarterCnt == 1)
                    {
                        yearMetrics.Q2 = quarter;
                    }
                    else if (quarterCnt == 2)
                    {
                        yearMetrics.Q3 = quarter;
                    }
                    else if (quarterCnt == 3)
                    {
                        yearMetrics.Q4 = quarter;
                    }
                    quarterCnt++;


                }
                years.Add(yearMetrics);
                metrics.Years = years;

                programModel.Metrics = metrics;
            }


            if (filter.showchildren.HasValue && filter.showchildren.Value == true)
            {
                programModel.Children = await GetChildrenProgramsAsync(id, filter.showchildren.Value);
            }

            return programModel;
        }

        public async Task<List<ProgramIssuesModel>> GetProgramIssuesAsync(int programId, ProgramResolveFilter programResolveFilter)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter { ParameterName = "@ProgramId", Value = programId });
            if (programResolveFilter.contractorId.HasValue)
            {
                parameters.Add(new SqlParameter { ParameterName = "@inContractorID", Value = "'" + programResolveFilter.contractorId + "'" });
            }
            else
            {
                parameters.Add(new SqlParameter { ParameterName = "@inContractorID", Value = "NULL" });
            }
            if (programResolveFilter.customerId.HasValue)
            {
                parameters.Add(new SqlParameter { ParameterName = "@inCustomerID", Value = "'" + programResolveFilter.customerId + "'" });
            }
            else
            {
                parameters.Add(new SqlParameter { ParameterName = "@inCustomerID", Value = "NULL" });
            }
            if (programResolveFilter.resolved.HasValue)
            {
                parameters.Add(new SqlParameter { ParameterName = "@resolved", Value = "'" + programResolveFilter.resolved + "'" });
            }
            else
            {
                //Temp put back if you see this
                const bool defaultValue = false;
                parameters.Add(new SqlParameter { ParameterName = "@resolved", Value = "NULL" });
            }

            List<ProgramIssues> programIssues = await _sql_Wrapper.GetItemsAsync<ProgramIssues>(_connectionString, "spGetProgramIssues", parameters);

            var query = programIssues.Where(x => x.programID == programId);

            List<ProgramIssuesModel> programIssuesModels = new List<ProgramIssuesModel>();

            foreach (var q in query)
            {
                ProgramIssuesModel programIssuesModel = new ProgramIssuesModel();
                programIssuesModel.programID = q.programID;
                programIssuesModel.customerID = q.customerID;
                programIssuesModel.issueID = q.issueID;
                programIssuesModel.issueTitle = q.issueTitle;
                programIssuesModel.issueDescription = q.issueDescription;
                programIssuesModel.issueType = q.issueType;
                programIssuesModel.issueSubmitted = FormatDate(q.issueSubmitted);
                programIssuesModel.issueResolved = FormatDate(q.issueResolved);
                programIssuesModel.projectID = q.projectID;
                programIssuesModel.customerName = q.customerName;
                programIssuesModel.customerAddress1 = q.customerAddress1;
                programIssuesModel.customerAddress2 = q.customerAddress2;
                programIssuesModel.customerCity = q.customerCity;
                programIssuesModel.programID = q.programID;
                programIssuesModel.customerState = q.customerState;
                programIssuesModel.customerZip = q.customerZip;
                programIssuesModel.projectStatus = q.projectStatus;
                programIssuesModel.SegmentType = q.SegmentType;

                programIssuesModels.Add(programIssuesModel);
            }
            return programIssuesModels;
        }

        public async Task<List<IssueModel>> GetProgramIssuesAsync(int entityId, string entityName, UserType userType)
        {

             List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter { ParameterName = "@inEntityType", Value = "'" + entityName + "'" });
            parameters.Add(new SqlParameter { ParameterName = "@inEntityID", Value = entityId });
            parameters.Add(new SqlParameter { ParameterName = "@inUserType", Value = "'" + userType.ToString() + "'" });

            List<Issue> programIssues = await _sql_Wrapper.GetItemsAsync<Issue>(_connectionString, "sp_get_issues", parameters);

            List<IssueModel> programIssuesModels = new List<IssueModel>();

            foreach (var q in programIssues)
            {
                programIssuesModels.Add(
                    new IssueModel()
                    {
                        Id = q.id,
                        EntityName = q.entityType,
                        EntityId = q.entityId,
                        SubjectLabel = q.subjectLabel,
                        Description = q.description,
                        Title = q.title,
                        Priority = q.priority,
                        SubmittedDate = q.submittedDate,
                        ResolvedDate = q.resolvedDate
                    });
            }
            return programIssuesModels;
        }

        public async Task<List<ProgramInvoiceModel>> GetProgramInvoicesAsync(int programId, ProgramInvoiceFilter programInvoiceFilter)
        {
            List<ProgramInvoiceModel> programInvoiceModels = new List<ProgramInvoiceModel>();

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter { ParameterName = "@ProgramId", Value = programId });
            if (programInvoiceFilter.contractorId.HasValue)
            {
                parameters.Add(new SqlParameter { ParameterName = "@inContractorID", Value = "'" + programInvoiceFilter.contractorId + "'" });
            }
            else
            {
                parameters.Add(new SqlParameter { ParameterName = "@inContractorID", Value = "NULL" });
            }
            if (programInvoiceFilter.customerId.HasValue)
            {
                parameters.Add(new SqlParameter { ParameterName = "@inCustomerID", Value = "'" + programInvoiceFilter.customerId + "'" });
            }
            else
            {
                parameters.Add(new SqlParameter { ParameterName = "@inCustomerID", Value = "NULL" });
            }
            if (programInvoiceFilter.pastDue.HasValue)
            {
                parameters.Add(new SqlParameter { ParameterName = "@inCustomerID", Value = "'" + programInvoiceFilter.pastDue + "'" });
            }
            else
            {
                const bool defaultValue = false;
                parameters.Add(new SqlParameter { ParameterName = "@inCustomerID", Value = defaultValue });
            }

            List<ProgramInvoices> programInvoices = await _sql_Wrapper.GetItemsAsync<ProgramInvoices>(_connectionString, "spGetProgramInvoices", parameters);

            var query = (IEnumerable<ProgramInvoices>)programInvoices;

            foreach (var q in query)
            {
                ProgramInvoiceModel programInvoiceModel = new ProgramInvoiceModel();
                programInvoiceModel.Id = q.invoiceID;

                BaseProjectModel projectModel = new BaseProjectModel();
                projectModel.id = q.projectID;

                CustomerModel customerModel = new CustomerModel();
                customerModel.Id = q.customerID;
                customerModel.Name = q.customerName;
                customerModel.Address1 = q.customerAddress1;
                customerModel.Address2 = q.customerAddress2;
                customerModel.City = q.customerCity;
                customerModel.State = q.customerState;
                customerModel.ZipCode = q.customerZip;
                projectModel.Customer = customerModel;
                projectModel.Status = q.projectStatus;

                //TODO:
                // ContractorModel contractorModel = new ContractorModel();
                //projectModel.Contractor = contractorModel;

                //CrewMemberModel crewMember = new CrewMemberModel();

                //projectModel.AssignedCrew = crewMember;

                programInvoiceModel.Status = q.invoiceStatus;
                programInvoiceModel.Paid = q.invoicePaid;
                programInvoiceModel.Amount = string.Format("{0:0.00}", q.invoiceAmount);
                programInvoiceModel.Document = q.documentID;
                programInvoiceModel.ContractorPaymentDate = FormatDate(q.contractorPaymentDate);
                programInvoiceModel.DueDate = FormatDate(q.dueDate);

                programInvoiceModel.SegmentType = q.SegmentType;

                programInvoiceModel.Project = projectModel;
                programInvoiceModels.Add(programInvoiceModel);
            }
            return programInvoiceModels;
        }

        public async Task<List<MessageModel>> GetProgramMessagesAsync(int programId, ProgramMinSetFilter programFilter)
        {
            List<MessageModel> messageModels = new List<MessageModel>();

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter { ParameterName = "@ProgramId", Value = programId });
            if (programFilter.ContractorID.HasValue)
            {
                parameters.Add(new SqlParameter { ParameterName = "@inContractorID", Value = "'" + programFilter.ContractorID + "'" });
            }
            else
            {
                parameters.Add(new SqlParameter { ParameterName = "@inContractorID", Value = "NULL" });
            }
            if (programFilter.CustomerID.HasValue)
            {
                parameters.Add(new SqlParameter { ParameterName = "@inCustomerID", Value = "'" + programFilter.CustomerID + "'" });
            }
            else
            {
                parameters.Add(new SqlParameter { ParameterName = "@inCustomerID", Value = "NULL" });
            }

            List<ProgramMessages> programMessages = await _sql_Wrapper.GetItemsAsync<ProgramMessages>(_connectionString, "spGetProgramMessages", parameters);


            foreach (var programMessage in programMessages)
            {
                MessageModel messageModel = new MessageModel();
                messageModel.id = programMessage.customerMessageID;
                messageModel.Subject = programMessage.messageSubject;
                messageModel.Body = programMessage.messageBody;
                messageModel.From = programMessage.messageFrom;
                messageModel.Submitted = FormatDate(programMessage.messageSubmitted);
                messageModel.To = programMessage.messageTo;
                messageModel.Status = programMessage.messageStatus;

                CustomerModel customerModel = new CustomerModel();
                customerModel.Id = programMessage.customerID;
                customerModel.Name = programMessage.customerName;
                customerModel.Address1 = programMessage.customerAddress1;
                customerModel.Address2 = programMessage.customerAddress2;
                customerModel.City = programMessage.customerCity;
                customerModel.State = programMessage.customerState;
                customerModel.ZipCode = programMessage.customerZip;
                messageModel.Customer = customerModel;

                ContractorModel contractorModel = new ContractorModel();
                contractorModel.Id = programMessage.contractorID;
                contractorModel.Name = programMessage.contractorName;
                messageModel.Contractor = contractorModel;
                messageModels.Add(messageModel);
            }
            return messageModels;
        }

        public async Task<List<PipelineNewModel>> GetProgramPipelineAsync(int programId, ProgramPipelineFilter programPipelineFilter)
        {
            List<PipelineNewModel> pipelineModels = new List<PipelineNewModel>();

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter { ParameterName = "@ProgramId", Value = programId });
            if (programPipelineFilter.ContractorId.HasValue)
            {
                parameters.Add(new SqlParameter { ParameterName = "@inContractorID", Value = programPipelineFilter.ContractorId });
            }
            else
            {
                parameters.Add(new SqlParameter { ParameterName = "@inAuditorID", Value = "NULL" });
            }
            if (programPipelineFilter.AuditorId.HasValue)
            {
                parameters.Add(new SqlParameter { ParameterName = "@inAuditorID", Value = programPipelineFilter.AuditorId });
            }
            else
            {
                parameters.Add(new SqlParameter { ParameterName = "@inAuditorID", Value = "NULL" });
            }
            if (programPipelineFilter.CampaignId.HasValue)
            {
                parameters.Add(new SqlParameter { ParameterName = "@inCampaignID", Value = programPipelineFilter.CampaignId });
            }
            else
            {
                parameters.Add(new SqlParameter { ParameterName = "@inCampaignID", Value = "NULL" });
            }

            List<PipelineElement> programPipelines = await _sql_Wrapper.GetItemsAsync<PipelineElement>(_connectionString + ";default command timeout=0;", "spGenerateDynamicPipeline", parameters);

            foreach (var programPipeline in programPipelines)
            {
                PipelineNewModel pipelineModel = new PipelineNewModel();
                pipelineModel.Name = programPipeline.name;
                pipelineModel.Label = programPipeline.label;
                pipelineModel.ChecklistItem = programPipeline.checklist_item;
                pipelineModel.Count = programPipeline.count;
                pipelineModel.kW = programPipeline.kW;
                pipelineModel.kWh = programPipeline.kWh;
                pipelineModel.therms = programPipeline.therms;
                pipelineModel.Incentives = programPipeline.incentives;
                pipelineModel.Savings = programPipeline.savings;
                pipelineModel.Label = programPipeline.label;
                pipelineModel.GHG = programPipeline.ghg;
                pipelineModel.NOx = programPipeline.nox;

                pipelineModels.Add(pipelineModel);
            }
            return pipelineModels;
        }

        public async Task<List<KPIModel>> GetProgramKpiAsync(int programId, ProgramFilter programInvoiceFilter)
        {
            List<KPIModel> kpiModels = new List<KPIModel>();

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter { ParameterName = "@ProgramId", Value = programId });

            List<ProgramKpi> programKpis = await _sql_Wrapper.GetItemsAsync<ProgramKpi>(_connectionString, "spGetProgramKPIs", parameters);

            var query = (IEnumerable<ProgramKpi>)programKpis;

            if (string.IsNullOrEmpty(programInvoiceFilter.measures) == false)
            {
                string[] _measures = ParseString(programInvoiceFilter.measures, ',');
                query = query.Where(q => _measures.Contains(q.measures));
            }
            if (string.IsNullOrEmpty(programInvoiceFilter.customer_type) == false)
            {
                string[] subSegments = ParseString(programInvoiceFilter.customer_type, ',');
                query = query.Where(q => subSegments.Contains(q.SegmentType));
            }
            if (string.IsNullOrEmpty(programInvoiceFilter.geography) == false)
            {
                string[] zipCodes = ParseString(programInvoiceFilter.geography, ',');
                query = query.Where(q => zipCodes.Contains(q.zip.Substring(0, 5)));
            }

            foreach (var q in query)
            {
                KPIModel kpiModel = new KPIModel();
                kpiModel.Id = q.programID;
                kpiModel.Label = q.KPILabel;
                kpiModel.KPIType = q.KPIType;
                kpiModel.Order = q.KPIOrder;

                List<RubricModel> rubicModels = new List<RubricModel>();

                RubricModel rubricModel = new RubricModel();
                ScaleModel scaleModelLow = new ScaleModel();
                scaleModelLow.Lower = q.rubricLowLower;
                scaleModelLow.Upper = q.rubricLowUpper;
                rubricModel.Low = scaleModelLow;

                ScaleModel scaleModelMedium = new ScaleModel();
                scaleModelMedium.Lower = q.rubricMediumLower;
                scaleModelMedium.Upper = q.rubricMediumUpper;
                rubricModel.Medium = scaleModelMedium;

                ScaleModel scaleModelHigh = new ScaleModel();
                scaleModelHigh.Lower = q.rubricHighLower;
                scaleModelHigh.Upper = q.rubricHighUpper;
                rubricModel.High = scaleModelHigh;

                rubicModels.Add(rubricModel);

                kpiModel.Rubic = rubicModels;

                //TODO: Current Value and Details. Not in the SP

                kpiModels.Add(kpiModel);
            }
            return kpiModels;
        }

        public async Task<List<ProgramMeasuresModel>> GetProgramMeasuresAsync(int programId, ProgramMeasuresFilter programMeasuresFilter)
        {
            List<ProgramMeasuresModel> programMeasuresModels = new List<ProgramMeasuresModel>();

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter { ParameterName = "@ProgramId", Value = programId });

            List<ProgramMeasure> programMeasures = await _sql_Wrapper.GetItemsAsync<ProgramMeasure>(_connectionString, "spGetProgramMeasures", parameters);
            foreach (var programMeasure in programMeasures)
            {
                ProgramMeasuresModel programMeasuresModel = new ProgramMeasuresModel();
                programMeasuresModel.Id = programMeasure.measureID;
                programMeasuresModel.SolutionCode = programMeasure.solutionCode;
                programMeasuresModel.Name = programMeasure.name;
                programMeasuresModel.Type = programMeasure.measureType;
                programMeasuresModel.Cost = programMeasure.measureCost;
                programMeasuresModel.Incentive = programMeasure.measureIncentive;
                programMeasuresModel.Copay = programMeasure.measureCopay;

                GeneralSavingModel programMeasuresSaving = new GeneralSavingModel();
                programMeasuresSaving.kW = programMeasure.measurekWSavings;
                programMeasuresSaving.kWh = programMeasure.measurekWhSavings;
                programMeasuresSaving.therms = programMeasure.measureThermSavings;
                programMeasuresModel.Savings = programMeasuresSaving;

                programMeasuresModel.Image = programMeasure.Image;
                programMeasuresModel.Papers = programMeasure.Papers;
                programMeasuresModel.Active = programMeasure.IsActive;

                programMeasuresModels.Add(programMeasuresModel);
            }

            if (programMeasuresFilter.showInactive.HasValue && programMeasuresFilter.showInactive == false)
            {
                programMeasuresModels = programMeasuresModels.Where(q => q.Active) as List<ProgramMeasuresModel>;
            }

            return programMeasuresModels;
        }

        public async Task<List<GeneralSavingModel>> GetProgramSavingAsync(int programId, ProgramMinSetFilter programFilter)
        {
            List<GeneralSavingModel> programSavings = null;

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter { ParameterName = "@ProgramId", Value = programId });

            if (programFilter.ContractorID.HasValue)
            {
                parameters.Add(new SqlParameter { ParameterName = "@inContractorID", Value = "'" + programFilter.ContractorID + "'" });
            }
            else
            {
                parameters.Add(new SqlParameter { ParameterName = "@inContractorID", Value = "NULL" });
            }
            if (programFilter.CustomerID.HasValue)
            {
                parameters.Add(new SqlParameter { ParameterName = "@inCustomerID", Value = "'" + programFilter.CustomerID + "'" });
            }
            else
            {
                parameters.Add(new SqlParameter { ParameterName = "@inCustomerID", Value = "NULL" });
            }

            List<Saving> savings = await _sql_Wrapper.GetItemsAsync<Saving>(_connectionString, "spGetProgramSavings", parameters);

            programSavings = new List<GeneralSavingModel>();
            foreach (Saving saving in savings)
            {
                GeneralSavingModel programSavingModel = new GeneralSavingModel
                {
                    kW = saving.kW,
                    kWh = saving.kWh,
                    therms = saving.therms,
                    ghg = saving.ghg,
                    nox = saving.nox,
                    year = saving.year
                };

                programSavings.Add(programSavingModel);
            }

            return programSavings;
        }

        public async Task<ProgramModel> PatchProgram(int id, ProgramModel programModel)
        {
            await PatchProgram(id, programModel.Name, programModel.Type);
            await PatchProgramGeography(id, programModel.Geography);
            await PatchProgramCustomerDates(id, programModel.Dates);

            return await GetAsync(id, new ProgramFilter { showMetrics = false, showchildren = false });
        }

        private async Task PatchProgram(int id, string name, ProgramType type)
        {
            var set = new System.Text.StringBuilder();

            if (string.IsNullOrEmpty(name) == false)
            {
                set.Append(string.Format("programName = '{0}'", name));
            }

            if (set.Length > 0) set.Append(", ");
            set.Append(string.Format("type = '{0}'", type.ToString()));


            if (set.Length > 0)
            {
                string updateCommand = string.Format("UPDATE {0}.Program SET {1} WHERE programID = {2};", _prefix, set, id);
                await _sql_Wrapper.PerformCommandAsync(_connectionString, updateCommand);
            }
        }

        private async Task PatchProgramGeography(int id, Geography geography)
        {
            if (geography != null)
            {
                var set = new StringBuilder();
                bool init = false;
                if (string.IsNullOrEmpty(geography.Lat) == false)
                {
                    set.Append(string.Format("geoLat = '{0}'", geography.Lat));
                    init = true;
                }
                if (string.IsNullOrEmpty(geography.Long) == false)
                {
                    if (init)
                    {
                        set.Append(",");
                    }
                    else
                    {
                        init = true;
                    }
                    set.Append(string.Format("geoLong = '{0}'", geography.Long));
                }
                if (string.IsNullOrEmpty(geography.Zoom) == false)
                {
                    if (init)
                    {
                        set.Append(",");
                    }
                    else
                    {
                        init = true;
                    }
                    set.Append(string.Format("geoZoom = '{0}'", geography.Zoom));
                }
                if (init)
                {
                    string updateCommand = string.Format("UPDATE {0}.Program SET {1} WHERE programID = {2};", _prefix, set, id);
                    await _sql_Wrapper.PerformCommandAsync(_connectionString, updateCommand);
                }
            }
        }

        private async Task PatchProgramCustomerDates(int id, DateRange customerDates)
        {
            if (customerDates != null)
            {
                var set = new System.Text.StringBuilder();
                bool init = false;
                if (customerDates.StartDate != null)
                {
                    set.Append(string.Format("startDate = '{0}'", customerDates.StartDate.ToString("yyyy-MM-dd")));
                    init = true;
                }
                if (customerDates.EndDate != null)
                {
                    if (init)
                    {
                        set.Append(",");
                    }
                    else
                    {
                        init = true;
                    }
                    set.Append(string.Format("endDate = '{0}'", customerDates.EndDate.ToString("yyyy-MM-dd")));
                }
                if (init)
                {
                    string updateCommand = string.Format("UPDATE {0}.Program SET {1} WHERE programID = {2};", _prefix, set, id);
                    await _sql_Wrapper.PerformCommandAsync(_connectionString, updateCommand);
                }
            }
        }

        public async Task<ProgramMeasuresModel> PatchProgramMeasure(int programId, int measuresId, ProgramMeasuresModel incomingProgramMeasures)
        {
            await ExecutePatchProgramMeasure(programId, measuresId, incomingProgramMeasures);
            return await GetProgramMeasure(programId, measuresId);
        }

        public async Task<ProgramMeasuresModel> PostProgramMeasure(int programId, ProgramMeasuresModel incomingProgramMeasures)
        {
            long measureId = await ExecutePostProgramMeasure(programId, incomingProgramMeasures);
            return await GetProgramMeasure(programId, (int)measureId);
        }

        public async Task<List<DivisionModel>> GetDivisions(int programId)
        {
            string command = string.Format("select * from {0}.ProgramDivisions where programID = {1}", _prefix, programId);
            List<DivisionDataModel> divisionModels = await _sql_Wrapper.GetItemsByCommandAsync<DivisionDataModel>(_connectionString, command);

            List<DivisionModel> divisions = new List<DivisionModel>();

            if (divisionModels.Count > 0)
            {
                foreach (var division in divisionModels)
                {
                    divisions.Add(
                        new DivisionModel()
                        {
                            Id = division.divisionCode,
                            Name = division.divisionName
                        }
                    );
                }

                return divisions;
            }
            else return null;
        }

        public async Task<DivisionModel> GetDivision(int programId, string id)
        {
            string command = string.Format("select * from {0}.ProgramDivisions where programID = {1} and divisionCode = '{2}'", _prefix, programId, id);
            List<DivisionDataModel> divisionModels = await _sql_Wrapper.GetItemsByCommandAsync<DivisionDataModel>(_connectionString, command);

            List<DivisionModel> divisions = new List<DivisionModel>();

            if (divisionModels.Count > 0)
            {
                DivisionDataModel division = divisionModels.First();
                return new DivisionModel()
                {
                    Id = division.divisionCode,
                    Name = division.divisionName
                };
            }
            else return null;
        }

        public async Task<DivisionModel> AddDivision(int programId, DivisionModel division)
        {
            string commandInsert = string.Format("insert into {0}.ProgramDivisions (divisionCode, divisionName,programId) VALUES ('{1}', '{2}', {3})", _prefix, division.Id, division.Name, programId);
            await _sql_Wrapper.PerformCommandAsync(_connectionString, commandInsert);

            return await GetDivision(programId, division.Id);
        }

        public async Task<DivisionModel> UpdateDivision(int programId, DivisionModel division)
        {
            string commandUpdate = string.Format("UPDATE {0}.ProgramDivisions SET divisionName = '{1}' WHERE programId = '{2}' AND divisionCode='{3}'", _prefix, division.Name, programId, division.Id);
            await _sql_Wrapper.PerformCommandAsync(_connectionString, commandUpdate);

            return await GetDivision(programId, division.Id);
        }

        public async Task DeleteDivision(int programId, string divisionId)
        {
            string commandUpdate = string.Format("DELETE FROM {0}.ProgramDivisions WHERE divisionCode = '{1}' AND programId = '{2}'", _prefix, divisionId, programId);
            await _sql_Wrapper.PerformCommandAsync(_connectionString, commandUpdate);
        }

        public async Task<List<string>> GetProgramZipCodesAsync(int programId)
        {
            string command = string.Format("select zipCode from {0}.ProgramZipCodes where programID = {1}", _prefix, programId);
            List<ZipCode> zipCodes = await _sql_Wrapper.GetItemsByCommandAsync<ZipCode>(_connectionString, command);

            List<string> zipCodesList = new List<string>();
            foreach (var zipCode in zipCodes)
            {
                zipCodesList.Add(zipCode.zipCode);
            }
            return zipCodesList;
        }

        public async Task<List<string>> PostProgramZipCodesAsync(int programId, List<string> inZipCodes)
        {
            var values = new StringBuilder();
            values.Append("VALUES ");
            for (int i = 0; i < inZipCodes.Count; i++)
            {
                string value = string.Format("('{0}', '{1}'),", programId, inZipCodes[i]);
                values.Append(value);
            }
            string outValues = values.ToString().Trim(',');

            string commandInsert = string.Format("insert into {0}.ProgramZipCodes (programID, ZipCode) {1};", _prefix, outValues);
            await _sql_Wrapper.PerformCommandAsync(_connectionString, commandInsert);

            return await GetProgramZipCodesAsync(programId);
        }

        public async Task<List<string>> DeleteProgramZipCodesAsync(int programId, string zipcode)
        {
            string commandUpdate = string.Format("DELETE FROM {0}.ProgramZipCodes WHERE zipCode = '{1}' AND programId = {2}", _prefix, zipcode, programId);
            await _sql_Wrapper.PerformCommandAsync(_connectionString, commandUpdate);

            return await GetProgramZipCodesAsync(programId);
        }

        public async Task<List<MeasureSavings>> GetMeasuresCategory(int programId)
        {
            List<MeasureSavings> types = new List<MeasureSavings>();

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter { ParameterName = "@ProgramId", Value = programId });

            List<MeasuresByType> measuresByTypes = await _sql_Wrapper.GetItemsAsync<MeasuresByType>(_connectionString, "spGetProgramMeasuresByType", parameters);
            foreach (var measuresByType in measuresByTypes)
            {
                MeasureSavings measureSaving = new MeasureSavings();
                measureSaving.subsegment = measuresByType.measureType;
                measureSaving.savingsKWH = measuresByType.kWhSavings.ToString();
                measureSaving.savingsDollars = measuresByType.savingDollars.ToString();
                types.Add(measureSaving);
            }

            return types;
        }

        public async Task<List<MeasureSavings>> GetMeasuresSubsegment(int programId)
        {
            List<MeasureSavings> subsegments = new List<MeasureSavings>();

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter { ParameterName = "@ProgramId", Value = programId });

            List<MeasuresBySubsegment> subSegments = await _sql_Wrapper.GetItemsAsync<MeasuresBySubsegment>(_connectionString, "spGetProgramMeasuresBySubsegment", parameters);
            foreach (var subSegment in subSegments)
            {
                MeasureSavings saving = new MeasureSavings();
                saving.subsegment = subSegment.subSegment;
                saving.savingsKWH = subSegment.kWhSavings.ToString();
                saving.savingsDollars = subSegment.savingDollars.ToString();
                subsegments.Add(saving);
            }

            //TODO: test
            if (subsegments.Count > 10)
            {
                return subsegments.Take(10).ToList();
            }

            return subsegments;
        }

        public async Task<ProgramPotentialSavings> GetMeasuresPotential(int programId)
        {
            ProgramPotentialSavings programPotentialSavings = new ProgramPotentialSavings();

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter { ParameterName = "@ProgramId", Value = programId });

            List<PotentialSavings> potentialSavings = await _sql_Wrapper.GetItemsAsync<PotentialSavings>(_connectionString, "spGetProgramSavingsPotential", parameters);
            var potentialSaving = potentialSavings.FirstOrDefault();
            if (potentialSaving != null)
            {
                programPotentialSavings.energyConsumption = potentialSaving.totalEnergy.ToString();
                programPotentialSavings.energySavings = potentialSaving.kWhSavings.ToString();
            }

            return programPotentialSavings;
        }

        private string[] ParseString(string source, char character)
        {
            return source.Split(character);
        }

        private string FormatDate(DateTime date)
        {
            if (date == null || date == DateTime.MinValue)
            {
                return null;
            }
            else
            {
                return date.ToString("yyyy-MM-dd");
            }
        }

        private async Task ExecutePatchProgramMeasure(int programId, int measureId, ProgramMeasuresModel incomingProgramMeasures)
        {
            var set = new StringBuilder();
            bool init = false;

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter { ParameterName = "@Program", Value = programId });
            parameters.Add(new SqlParameter { ParameterName = "@Measure", Value = measureId });

            parameters.Add(DBUtils.GetSubSet(ref init, "solutionCode", incomingProgramMeasures.SolutionCode, ref set));
            parameters.Add(DBUtils.GetSubSet(ref init, "measureDescription", incomingProgramMeasures.Name, ref set));

            if (!string.IsNullOrEmpty(incomingProgramMeasures.Type))
            {
                Enums.MeasureType measureType;
                string type = incomingProgramMeasures.Type.Replace(" ", "_");
                if (!Enum.TryParse(type, out measureType)) measureType = Enums.MeasureType.Additional_Measures;

                parameters.Add(DBUtils.GetSubSet(ref init, "measureTypeID", (int) measureType, ref set));
            }

            parameters.Add(DBUtils.GetSubSet(ref init, "measureCost", incomingProgramMeasures.Cost, ref set));
            parameters.Add(DBUtils.GetSubSet(ref init, "measureIncentive", incomingProgramMeasures.Incentive, ref set));
            parameters.Add(DBUtils.GetSubSet(ref init, "measureCopay", incomingProgramMeasures.Copay, ref set));

            if (incomingProgramMeasures.Savings != null)
            {
                parameters.Add(DBUtils.GetSubSet(ref init, "measurekWSavings", incomingProgramMeasures.Savings.kW, ref set));
                parameters.Add(DBUtils.GetSubSet(ref init, "measurekWhSavings", incomingProgramMeasures.Savings.kWh, ref set));
                parameters.Add(DBUtils.GetSubSet(ref init, "measureThermSavings", incomingProgramMeasures.Savings.therms, ref set));
            }

            parameters.Add(DBUtils.GetSubSet(ref init, "Image", incomingProgramMeasures.Image, ref set));
            parameters.Add(DBUtils.GetSubSet(ref init, "Papers", incomingProgramMeasures.Papers, ref set));
            parameters.Add(DBUtils.GetSubSet(ref init, "IsActive", incomingProgramMeasures.Active, ref set));

            if (init)
            {
                string setCommand = string.Format("UPDATE {0}.ProgramMeasures SET {1} WHERE programID = @Program and measureID = @Measure;", _prefix, set);
                await _sql_Wrapper.PerformCommandAsync(_connectionString, setCommand, parameters.FindAll(q => q != null));
            }

            if (incomingProgramMeasures.Benefits != null && incomingProgramMeasures.Benefits.Count > 0)
            {
                await DeleteAllMeasureBenefits(measureId);
                await AddMeasureBenefits(measureId, incomingProgramMeasures.Benefits);
            }
        }

        private async Task<long> ExecutePostProgramMeasure(int programId, ProgramMeasuresModel incomingProgramMeasures)
        {
            Enums.MeasureType measureType;
            string type = incomingProgramMeasures.Type.Replace(" ", "_");
            if (!Enum.TryParse(type, out measureType)) measureType = Enums.MeasureType.Additional_Measures;

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter { ParameterName = "@SolutionCode", Value = incomingProgramMeasures.SolutionCode });
            parameters.Add(new SqlParameter { ParameterName = "@Name", Value = incomingProgramMeasures.Name });
            parameters.Add(new SqlParameter { ParameterName = "@Cost", Value = incomingProgramMeasures.Cost });
            parameters.Add(new SqlParameter { ParameterName = "@Incentive", Value = incomingProgramMeasures.Incentive });
            parameters.Add(new SqlParameter { ParameterName = "@Copay", Value = incomingProgramMeasures.Copay });
            parameters.Add(new SqlParameter { ParameterName = "@SavingsKW", Value = incomingProgramMeasures.Savings.kW });
            parameters.Add(new SqlParameter { ParameterName = "@SavingsKWh", Value = incomingProgramMeasures.Savings.kWh });
            parameters.Add(new SqlParameter { ParameterName = "@SavingsTherms", Value = incomingProgramMeasures.Savings.therms });
            parameters.Add(new SqlParameter { ParameterName = "@Type", Value = (int) measureType });
            parameters.Add(new SqlParameter { ParameterName = "@Image", Value = incomingProgramMeasures.Image });
            parameters.Add(new SqlParameter { ParameterName = "@Papers", Value = incomingProgramMeasures.Papers });
            parameters.Add(new SqlParameter { ParameterName = "@ProgramId", Value = programId });

            string commandInsert = string.Format("Insert into {0}.ProgramMeasures (" +
                "solutionCode, measureDescription, measureCost, measureIncentive, measureCopay, " +
                "measurekWSavings, measurekWhSavings, measureThermSavings, " +
                "measureTypeID, Image, Papers, programID) " +
                "VALUES (@SolutionCode, @Name, @Cost, @Incentive, @Copay, @SavingsKW,  @SavingsKWh, @SavingsTherms, @Type, @Image, @Papers, @ProgramId)",
                _prefix);

            return await _sql_Wrapper.PerformCommandWithAutoincrementedId(_connectionString, commandInsert, parameters);
        }

        private async Task DeleteAllMeasureBenefits(int measureId)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter { ParameterName = "@Measure", Value = measureId });

            string setCommand = string.Format("DELETE FROM {0}.MeasureBenefit WHERE measureID = @Measure;", _prefix);
            await _sql_Wrapper.PerformCommandAsync(_connectionString, setCommand, parameters);
        }

        private async Task AddMeasureBenefits(int measureId, List<string> benefits)
        {
            string setCommand = string.Format("INSERT INTO {0}.MeasureBenefit (measureID, benefit) VALUES (@Measure, @Benefit);", _prefix);

            foreach (var benefit in benefits)
            {
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter { ParameterName = "@Measure", Value = measureId });
                parameters.Add(new SqlParameter { ParameterName = "@Benefit", Value = benefit });
                await _sql_Wrapper.PerformCommandAsync(_connectionString, setCommand, parameters);
            }
        }

        public async Task<ProgramMeasuresModel> GetProgramMeasure(int programId, int measuresId)
        {
            ProgramMeasuresModel programMeasuresModel = new ProgramMeasuresModel();

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter { ParameterName = "@ProgramId", Value = programId });
            parameters.Add(new SqlParameter { ParameterName = "@inMeasureID", Value = measuresId });
            List<ProgramMeasure> programMeasures = await _sql_Wrapper.GetItemsAsync<ProgramMeasure>(_connectionString, "spGetProgramMeasure", parameters);
            var programMeasure = programMeasures.FirstOrDefault();
            if (programMeasure != null)
            {
                programMeasuresModel.Id = programMeasure.measureID;
                programMeasuresModel.SolutionCode = programMeasure.solutionCode;
                programMeasuresModel.Name = programMeasure.name;
                programMeasuresModel.Type = programMeasure.measureType;
                programMeasuresModel.Cost = programMeasure.measureCost;
                programMeasuresModel.Incentive = programMeasure.measureIncentive;
                programMeasuresModel.Copay = programMeasure.measureCopay;

                GeneralSavingModel programMeasuresSaving = new GeneralSavingModel();
                programMeasuresSaving.kW = programMeasure.measurekWSavings;
                programMeasuresSaving.kWh = programMeasure.measurekWhSavings;
                programMeasuresSaving.therms = programMeasure.measureThermSavings;
                programMeasuresModel.Savings = programMeasuresSaving;

                programMeasuresModel.Image = programMeasure.Image;
                programMeasuresModel.Papers = programMeasure.Papers;
                programMeasuresModel.Active = programMeasure.IsActive;

                programMeasuresModel.Benefits = await GetMeasureBenefits(measuresId);


            }

            return programMeasuresModel;
        }

        public async Task<bool> AddChildProgramAsync(int programId, int subProgramId)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter { ParameterName = "@ProgramId", Value = programId });
            parameters.Add(new SqlParameter { ParameterName = "@SubProgramId", Value = subProgramId });

            string command = string.Format("UPDATE {0}.Program SET parentID = @ProgramId WHERE programID = @SubProgramId", _prefix);
            await _sql_Wrapper.PerformCommandAsync(_connectionString, command, parameters);

            return true;
        }

        public async Task<List<PipelineNewModel>> UpdatePipelineAsync(int programId, List<PipelineNewModel> programPipelineModels)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter { ParameterName = "@ProgramId", Value = programId });


            //Delete all elements from the two tables
            string command = string.Format("DELETE FROM {0}.ProgramPipelineElement WHERE programPipelineID IN (SELECT programPipelineID FROM {0}.ProgramPipeline WHERE programID=@programId)", _prefix);
            await _sql_Wrapper.PerformCommandAsync(_connectionString, command, parameters);

            command = string.Format("DELETE FROM {0}.ProgramPipeline WHERE programID = @ProgramId;", _prefix);
            await _sql_Wrapper.PerformCommandAsync(_connectionString, command, parameters);

            int count = 1;
            foreach (var element in programPipelineModels)
            {
                PipelineElementName name;
                if (Enum.TryParse(element.Name, out name))
                {
                    parameters = new List<SqlParameter>();
                    parameters.Add(new SqlParameter { ParameterName = "@ProgramId", Value = programId });
                    parameters.Add(new SqlParameter { ParameterName = "@NameId", Value = name });
                    parameters.Add(new SqlParameter { ParameterName = "@Label", Value = element.Label == null ? element.Name : element.Label });
                    parameters.Add(new SqlParameter { ParameterName = "@ChecklistItem", Value = element.ChecklistItem });
                    parameters.Add(new SqlParameter { ParameterName = "@Order", Value = count++ });

                    //Insert new elements from ProgramPipeline and Element
                    string command1 = string.Format("INSERT INTO {0}.ProgramPipeline (pipelineNameID, programID) VALUES (@NameId, @ProgramId)", _prefix);
                    long programPipelineId = await _sql_Wrapper.PerformCommandWithAutoincrementedId(_connectionString, command1, parameters);

                    parameters.Add(new SqlParameter { ParameterName = "@PipelineID", Value = programPipelineId });
                    string command2 = string.Format("INSERT INTO {0}.ProgramPipelineElement (programPipelineID, label, pipelineOrder, checklist_item_master_id) VALUES (@PipelineID, @Label, @Order, @ChecklistItem)", _prefix);
                    await _sql_Wrapper.PerformCommandAsync(_connectionString, command2, parameters);
                }
            }

            return await GetProgramPipelineAsync(programId, new ProgramPipelineFilter());
        }

        public async Task<bool> DeleteChildProgramAsync(int programId, int subProgramId)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter { ParameterName = "@SubProgramId", Value = subProgramId });

            string command = string.Format("UPDATE {0}.Program SET parentID = 0 WHERE programID = @SubProgramId", _prefix);
            await _sql_Wrapper.PerformCommandAsync(_connectionString, command, parameters);

            return true;
        }

        private async Task<List<String>> GetMeasureBenefits(int measuresId)
        {
            List<string> benefits = new List<string>();

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter { ParameterName = "@inMeasureID", Value = measuresId });

            List<Benefit> models = await _sql_Wrapper.GetItemsAsync<Benefit>(_connectionString, "spGetMeasureBenefits", parameters);
            foreach (var model in models)
            {
                benefits.Add(model.benefit);
            }
            return benefits;

        }

        public async Task<List<int>> GetAllDashboardsByProgramAsync(int programId)
        {
            List<int> dashboards = new List<int>();
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter { ParameterName = "@ProgramId", Value = programId });

            string command = string.Format("SELECT * FROM {0}.ProgramDashboards WHERE programID  = @ProgramId", _prefix);
            List<ProgramDashboard> dashboardIds = await _sql_Wrapper.GetItemsByCommandAsync<ProgramDashboard>(_connectionString, command, parameters);

            foreach (ProgramDashboard programDashboard in dashboardIds)
            {
                dashboards.Add(programDashboard.dashboardID);
            }

            return dashboards;
        }

        public async Task<List<int>> AddDashboardToProgram(int programId, int dashboardId)
        {
            List<int> dashboards = new List<int>();

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter { ParameterName = "@ProgramId", Value = programId });
            parameters.Add(new SqlParameter { ParameterName = "@DashboardId", Value = dashboardId });

            string command = string.Format("INSERT INTO {0}.ProgramDashboards (programID, dashboardID) VALUES (@ProgramId, @DashboardId)", _prefix);
            await _sql_Wrapper.PerformCommandAsync(_connectionString, command, parameters);


            return await GetAllDashboardsByProgramAsync(programId);
        }

        public async Task PatchDashboardDefaultToProgram(int programId, string userType, int dashboardId)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter { ParameterName = "@ProgramId", Value = programId });
            parameters.Add(new SqlParameter { ParameterName = "@UserType", Value = userType });
            parameters.Add(new SqlParameter { ParameterName = "@DashboardDefaultID", Value = dashboardId });

            string command = string.Format("DELETE FROM {0}.ProgramDashboardsDefault WHERE programID  = @ProgramID and userType = @UserType", _prefix);
            await _sql_Wrapper.PerformCommandAsync(_connectionString, command, parameters);

            string command1 = string.Format("INSERT INTO {0}.ProgramDashboardsDefault(programID, userType, dashboardDefaultID) VALUES (@ProgramId, @UserType, @DashboardDefaultID) ", _prefix);
            await _sql_Wrapper.PerformCommandAsync(_connectionString, command1, parameters);
        }

        public async Task<bool> IsDashboardDefaultIdValid(int dashboardId)
        {
            string validCommand = string.Format("select * FROM {0}.ProgramDashboards where dashboardID = {1}", _prefix, dashboardId);
            return await _sql_Wrapper.ValidateCommandAsync<ProgramDashboard>(_connectionString, validCommand);
        }

        public async Task<List<ProgramDashboardDefault>> GetDashboardDefaultToProgram(int programId)
        {
            List<ProgramDashboardDefault> dashboardDefault = new List<ProgramDashboardDefault>();

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter { ParameterName = "@ProgramId", Value = programId });
            string command = string.Format("SELECT * FROM {0}.ProgramDashboardsDefault WHERE programID  = @ProgramId", _prefix);
            List<ProgramDashboardDefault> dashboardIds = await _sql_Wrapper.GetItemsByCommandAsync<ProgramDashboardDefault>(_connectionString, command, parameters);

            foreach (var programDashboard in dashboardIds)
            {
                ProgramDashboardDefault programDashboardDefault = new ProgramDashboardDefault();
                programDashboardDefault.programID = programId;
                programDashboardDefault.userType = programDashboard.userType;
                programDashboardDefault.dashboardDefaultID = programDashboard.dashboardDefaultID;

                dashboardDefault.Add(programDashboardDefault);
            }

            return dashboardDefault;
        }

        public async Task<List<int>> RemoveDashboardFromProgram(int programId, int dashboardId)
        {
            List<int> dashboards = new List<int>();

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter { ParameterName = "@ProgramId", Value = programId });
            parameters.Add(new SqlParameter { ParameterName = "@DashboardId", Value = dashboardId });

            string command = string.Format("DELETE FROM {0}.ProgramDashboards WHERE programID=@ProgramId AND dashboardID=@DashboardId", _prefix);
            await _sql_Wrapper.PerformCommandAsync(_connectionString, command, parameters);

            return await GetAllDashboardsByProgramAsync(programId);
        }

        public async Task<List<MarketingMaterialModel>> AddMarketingMaterialAsync(int programId, IncomingMarketingMaterial material)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter { ParameterName = "@Name", Value = material.Name });
            parameters.Add(new SqlParameter { ParameterName = "@URL", Value = material.Url });
            parameters.Add(new SqlParameter { ParameterName = "@AdditonalInfo", Value = material.AdditionalInfo });

            string command = string.Format("INSERT INTO {0}.MarketingMaterial(name, url, additionalInfo) VALUES (@Name, @URL, @AdditonalInfo)", _prefix);
            long materialId = await _sql_Wrapper.PerformCommandWithAutoincrementedId(_connectionString, command, parameters);

            parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter { ParameterName = "@ProgramId", Value = programId });
            parameters.Add(new SqlParameter { ParameterName = "@MaterialId", Value = materialId });

            command = string.Format("INSERT INTO {0}.MarketingMaterialProgram(programID, materialID) VALUES (@ProgramId, @MaterialId)", _prefix);
            await _sql_Wrapper.PerformCommandAsync(_connectionString, command, parameters);

            List<MarketingMaterialModel> marketingMaterials = await GetProgramMarketingMaterialsAsync(programId);

            return marketingMaterials;
        }

        public async Task<List<MarketingMaterialModel>> DeleteMarketingMaterialAsync(int programId, int documentId)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter { ParameterName = "@MaterialId", Value = documentId });

            string command = string.Format("SELECT * FROM {0}.MarketingMaterial WHERE materialID = @MaterialId", _prefix);
            MarketingMaterial material = await _sql_Wrapper.GetItemByCommandAsync<MarketingMaterial>(_connectionString, command, parameters);

            if (material != null)
            {
                command = string.Format("DELETE FROM {0}.MarketingMaterial WHERE materialID = @MaterialId", _prefix);
                await _sql_Wrapper.PerformCommandAsync(_connectionString, command, parameters);

                parameters.Add(new SqlParameter { ParameterName = "@ProgramId", Value = programId });

                command = string.Format("DELETE FROM {0}.MarketingMaterialProgram WHERE materialID = @MaterialId AND programID = @ProgramId", _prefix);
                await _sql_Wrapper.PerformCommandAsync(_connectionString, command, parameters);
            }

            return await GetProgramMarketingMaterialsAsync(programId);
        }

        public async Task<MarketingMaterialModel> GetMarketingMaterialAsync(int programId, int documentId)
        {
            MarketingMaterialModel model = null;
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter { ParameterName = "@MaterialId", Value = documentId });

            string command = string.Format("SELECT * FROM {0}.MarketingMaterial WHERE materialID = @MaterialId", _prefix);
            MarketingMaterial marketingMaterial = await _sql_Wrapper.GetItemByCommandAsync<MarketingMaterial>(_connectionString, command, parameters);

            if (marketingMaterial != null)
            {
                model = new MarketingMaterialModel()
                {
                    Id = marketingMaterial.materialID,
                    Name = marketingMaterial.name,
                    URL = marketingMaterial.url,
                    AdditionalInfo = marketingMaterial.additionalInfo
                };
            }

            return model;
        }

        public async Task<List<MarketingMaterialModel>> GetProgramMarketingMaterialsAsync(int programId)
        {
            List<MarketingMaterialModel> programMarketingMaterials = new List<MarketingMaterialModel>();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter { ParameterName = "@ProgramId", Value = programId });

            string command = string.Format("SELECT * FROM {0}.MarketingMaterial WHERE materialID IN (SELECT materialID FROM {0}.MarketingMaterialProgram WHERE programID = @ProgramId)", _prefix);
            List<MarketingMaterial> marketingMaterials = await _sql_Wrapper.GetItemsByCommandAsync<MarketingMaterial>(_connectionString, command, parameters);

            foreach (MarketingMaterial material in marketingMaterials)
            {
                MarketingMaterialModel model = new MarketingMaterialModel()
                {
                    Id = material.materialID,
                    Name = material.name,
                    URL = material.url
                };

                programMarketingMaterials.Add(model);
            }

            return programMarketingMaterials;
        }

        public async Task AddProgramDocumentAsync(int programId, IncomingDocument incomingDocument)
        {
            long ProgramDocumenttoreId = await ExecuteAddProgramDocumenttoreAsync(programId, incomingDocument);
            await ExecuteAddProgramDocumentAsync(programId, incomingDocument, ProgramDocumenttoreId);
        }

        private async Task<long> ExecuteAddProgramDocumenttoreAsync(int programId, IncomingDocument incomingDocument)
        {
            List<SqlParameter> parameters;
            long ProgramDocumenttoreId = 0;
            long documentLink = 0;

            string command;
            if (incomingDocument.url != null || !string.IsNullOrEmpty(incomingDocument.url))
            {
                parameters = new List<SqlParameter>
                {
                    new SqlParameter { ParameterName = "@LocationId", Value = DocumentStorageType.BOX },
                    new SqlParameter { ParameterName = "@Url", Value = incomingDocument.url },
                    new SqlParameter { ParameterName = "@FileId", Value = incomingDocument.fileSourceId },
                    new SqlParameter { ParameterName = "@ProgramDocumentId", Value = incomingDocument.id > 0 ? incomingDocument.id : 0 }
                };

                command = string.Format("INSERT INTO {0}.document_hub(document_storage_id, document_url, file_source_id, program_document_id) VALUES (@LocationId, @URL, @FileId, @ProgramDocumentId)", _prefix);
                ProgramDocumenttoreId = await _sql_Wrapper.PerformCommandWithAutoincrementedId(_connectionString, command, parameters);

                if (ProgramDocumenttoreId > 0)
                {
                    const int programEntityId = 4;
                    parameters = new List<SqlParameter>
                    {
                        new SqlParameter { ParameterName = "@ProgramDocumentId", Value = ProgramDocumenttoreId },
                        new SqlParameter { ParameterName = "@EntityId", Value = programEntityId },
                        new SqlParameter { ParameterName = "@DocumentEntityId", Value = programId }
                    };

                    command = string.Format("INSERT INTO {0}.document_link(document_id, entity_id, document_entity_id) " +
                "VALUES (@ProgramDocumentId, @EntityId, @DocumentEntityId)", _prefix);
                    documentLink = await _sql_Wrapper.PerformCommandWithAutoincrementedId(_connectionString, command, parameters);
                }
                
            }

            return ProgramDocumenttoreId;
        }

        private async Task ExecuteAddProgramDocumentAsync(int programId, IncomingDocument incomingDocument, long ProgramDocumenttoreId)
        {
            List<SqlParameter> parameters;

            string command;
            parameters = new List<SqlParameter>
            {
                new SqlParameter { ParameterName = "@ProgramID", Value = programId },
                new SqlParameter { ParameterName = "@Name", Value = incomingDocument.name },
                new SqlParameter { ParameterName = "@LocationId", Value = DocumentStorageType.BOX },
                new SqlParameter { ParameterName = "@StoreId", Value = ProgramDocumenttoreId },
                new SqlParameter { ParameterName = "@Required", Value = incomingDocument.isRequired }
            };

            command = string.Format("INSERT INTO {0}.program_document(programID, name, document_storage_id, document_id, is_required) VALUES (@ProgramID, @Name, @LocationId, @StoreId, @Required)", _prefix);
            long programDocumentId = await _sql_Wrapper.PerformCommandWithAutoincrementedId(_connectionString, command, parameters);

            parameters = new List<SqlParameter>
            {
                new SqlParameter { ParameterName = "@ProgramDocId", Value = programDocumentId },
                new SqlParameter { ParameterName = "@StoreId", Value = ProgramDocumenttoreId }
            };

            command = string.Format("UPDATE {0}.document_hub SET program_document_id = @ProgramDocId WHERE document_id = @StoreId", _prefix);
            await _sql_Wrapper.PerformCommandAsync(_connectionString, command, parameters);
        }

        public async Task<List<ProgramDocumentModel>> GetAllProgramDocumentsAsync(int programId)
        {
            List<ProgramDocumentModel> programProgramDocument = null;

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter { ParameterName = "@ProgramId", Value = programId });

            string command = string.Format("SELECT * FROM {0}.program_document WHERE programID = @ProgramId", _prefix);
            List<ProgramDocument> ProgramDocument = await _sql_Wrapper.GetItemsByCommandAsync<ProgramDocument>(_connectionString, command, parameters);

            if (ProgramDocument != null && ProgramDocument.Count > 0)
            {
                programProgramDocument = new List<ProgramDocumentModel>();

                foreach (ProgramDocument document in ProgramDocument)
                {
                    ProgramDocumentModel programDocument = new ProgramDocumentModel
                    {
                        Id = document.program_document_id,
                        Name = document.name,
                        Template = document.document_id,
                        IsRequired = Convert.ToBoolean(document.is_required)
                    };

                    programProgramDocument.Add(programDocument);
                }
            }

            return programProgramDocument;
        }

        public async Task<List<ProgramDocumentModel>> DeleteProgramDocumentAsync(int programId, int programDocumentId)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter { ParameterName = "@ProgramDocId", Value = programDocumentId });

            DocumentHub document = await GetDocumentStoreAsync(programDocumentId);
            if (document != null)
            {
                await DeleteProgramDocumenttoreAsync(document.document_id);
            }

            string command = string.Format("DELETE FROM {0}.program_document WHERE program_document_id = @ProgramDocId", _prefix);
            await _sql_Wrapper.PerformCommandAsync(_connectionString, command, parameters);

            const int programEntityId = 4;
            parameters = new List<SqlParameter>
            {
                new SqlParameter { ParameterName= "@ProgramId", Value = programId},
                new SqlParameter { ParameterName= "@EntityId", Value = programEntityId}
            };

            command = string.Format("DELETE FROM {0}.document_link WHERE document_entity_id = @ProgramId AND entity_id = @EntityId", _prefix);
            await _sql_Wrapper.PerformCommandAsync(_connectionString, command, parameters);

            return await GetAllProgramDocumentsAsync(programId);
        }

        private async Task DeleteProgramDocumenttoreAsync(int ProgramDocumenttoreID)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter { ParameterName = "@StoreId", Value = ProgramDocumenttoreID });

            string command = string.Format("DELETE FROM {0}.document_hub WHERE document_id = @StoreId", _prefix);
            await _sql_Wrapper.PerformCommandAsync(_connectionString, command, parameters);
        }

        public async Task<DocumentHub> GetDocumentStoreAsync(int programDocumentId)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter { ParameterName = "@ProgramDocId", Value = programDocumentId });

            string command = string.Format("SELECT * FROM {0}.document_hub WHERE program_document_id = @ProgramDocId", _prefix);
            DocumentHub ProgramDocumenttore = await _sql_Wrapper.GetItemByCommandAsync<DocumentHub>(_connectionString, command, parameters);

            return ProgramDocumenttore;
        }

        public async Task<ProgramDocumentModel> GetProgramDocumentAsync(int programDocumentId)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter { ParameterName = "@ProgramDocId", Value = programDocumentId });

            string command = string.Format("SELECT * FROM {0}.program_document WHERE program_document_id = @ProgramDocId", _prefix);
            ProgramDocument programDocument = await _sql_Wrapper.GetItemByCommandAsync<ProgramDocument>(_connectionString, command, parameters);

            ProgramDocumentModel updatedModel = null;

            if (programDocument != null)
            {
                updatedModel = new ProgramDocumentModel()
                {
                    Id = programDocument.program_document_id,
                    Name = programDocument.name,
                    Template = programDocument.document_id,
                    IsRequired = Convert.ToBoolean(programDocument.is_required)
                };
            }

            return updatedModel;
        }

        public async Task<ProgramDocumentModel> UpdateProgramDocumentAsync(int programId, int programDocumentId, IncomingDocument incomingDocument)
        {


            long ProgramDocumenttoreId = await ExecuteUpdateProgramDocumenttoreAsync(programId, programDocumentId, incomingDocument);

            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter { ParameterName = "@Name", Value = incomingDocument.name },
                new SqlParameter { ParameterName = "@Required", Value = incomingDocument.isRequired },
                new SqlParameter { ParameterName = "@DocStoreId", Value = ProgramDocumenttoreId },
                new SqlParameter { ParameterName = "@ProgramDocId", Value = programDocumentId }
            };

            string command = string.Format("UPDATE {0}.program_document set name = @Name, is_required = @Required, document_id = @DocStoreId" +
                " WHERE program_document_id = @ProgramDocId", _prefix);
            await _sql_Wrapper.PerformCommandAsync(_connectionString, command, parameters);

            return await GetProgramDocumentAsync(programDocumentId);
        }

        public async Task<List<SubsegmentTypeModel>> GetMeasuresSubsegmentTypes(int programId)
        {
            List<SubsegmentTypeModel> subsegmentTypeModels = new List<SubsegmentTypeModel>();

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter { ParameterName = "@ProgramId", Value = programId });
            List<MeasureSubsegmentType> measureSubsegmentTypes = await _sql_Wrapper.GetItemsAsync<MeasureSubsegmentType>(_connectionString, "spGetProgramTypesMeasures", parameters);

            var subSegments = measureSubsegmentTypes.Select(x => x.businessType).Distinct();
            foreach (var subSegment in subSegments)
            {
                var types = measureSubsegmentTypes.Where(x => x.businessType == subSegment).ToList();
                SubsegmentTypeModel measureSubsegmentTypeModel = new SubsegmentTypeModel();
                measureSubsegmentTypeModel.Subsegment = subSegment;
                TypeModel typeModel = new TypeModel();
                foreach (var type in types)
                {
                    if (type.measureType == "Lighting")
                    {
                        typeModel.Lighting = type.kWh_Savings.ToString();
                    }
                    else if (type.measureType == "HVAC")
                    {
                        typeModel.HVAC = type.kWh_Savings.ToString();
                    }
                    else if (type.measureType == "Refrigeration")
                    {
                        typeModel.Refrigeration = type.kWh_Savings.ToString();
                    }
                    else if (type.measureType == "Other")
                    {
                        typeModel.Other = type.kWh_Savings.ToString();
                    }
                }
                measureSubsegmentTypeModel.Types = typeModel;

                subsegmentTypeModels.Add(measureSubsegmentTypeModel);
            }

            return subsegmentTypeModels;
        }

        public async Task<List<SubsegmentTypeModel>> GetAllMeasuresSubsegmentTypes(int programId)
        {
            List<SubsegmentTypeModel> subsegmentTypeModels = new List<SubsegmentTypeModel>();

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter { ParameterName = "@ProgramId", Value = programId });
            List<MeasureSubsegmentType> measureSubsegmentTypes = await _sql_Wrapper.GetItemsAsync<MeasureSubsegmentType>(_connectionString, "spGetProgramTypesMeasures", parameters);

            var subSegments = measureSubsegmentTypes.Select(x => x.businessType).Distinct();
            foreach (var subSegment in subSegments)
            {
                var types = measureSubsegmentTypes.Where(x => x.businessType == subSegment).ToList();
                SubsegmentTypeModel measureSubsegmentTypeModel = new SubsegmentTypeModel();
                measureSubsegmentTypeModel.Subsegment = subSegment;

                Dictionary<string, string> typesList = new Dictionary<string, string>();
                foreach (var type in types)
                {
                    typesList.Add(type.measureType, type.kWh_Savings.ToString());
                }
                measureSubsegmentTypeModel.TypesList = typesList;

                subsegmentTypeModels.Add(measureSubsegmentTypeModel);
            }

            return subsegmentTypeModels;
        }

        private object GetValidationInput(string checklistItemType, int value)
        {
            ChecklistItemType itemType;
            if (Enum.TryParse(checklistItemType, out itemType))
            {
                switch (itemType)
                {
                    case ChecklistItemType.CHECKLIST:
                    case ChecklistItemType.DOCUMENT:
                        return value;
                    case ChecklistItemType.MANUAL:
                    case ChecklistItemType.CONTACT_LOG:
                    case ChecklistItemType.NOTE:
                        return value == 1;
                    case ChecklistItemType.SCHEDULE:
                        return ((ScheduleType)value).ToString();
                    case ChecklistItemType.ASSIGN:
                        return ((AssignType)value).ToString();
                    default:
                        return value;
                }
            }
            else return null;
        }

        private int GetInputAsInt(string checklistItemType, string value)
        {
            ChecklistItemType itemType;
            if (Enum.TryParse(checklistItemType, out itemType))
            {
                switch (itemType)
                {
                    case ChecklistItemType.CHECKLIST:
                    case ChecklistItemType.DOCUMENT:
                        return Int16.Parse(value);
                    case ChecklistItemType.MANUAL:
                    case ChecklistItemType.CONTACT_LOG:
                    case ChecklistItemType.NOTE:
                        return 1;
                    case ChecklistItemType.SCHEDULE:
                        ScheduleType scheduleType;
                        if (Enum.TryParse(value, out scheduleType))
                        {
                            return (int)scheduleType;
                        }
                        return 0;
                    case ChecklistItemType.ASSIGN:
                        AssignType assignmentType;
                        if (Enum.TryParse(value, out assignmentType))
                        {
                            return (int)assignmentType;
                        }
                        return 0;
                    default:
                        return Int16.Parse(value.ToString());
                }
            }
            else return 0;
        }

        public async Task<List<ChecklistItemModel>> GetProgramChecklistAsync(int programId, string context)
        {
            List<ChecklistItemModel> programChecklistModels = new List<ChecklistItemModel>();

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter { ParameterName = "@inProgramID", Value = programId });

            var inContext = string.Format("'{0}'", context);

            parameters.Add(new SqlParameter { ParameterName = "@inContext", Value = inContext });
            List<ProgramChecklist> programChecklists = await _sql_Wrapper.GetItemsAsync<ProgramChecklist>(_connectionString, "spGetProgramChecklist", parameters);

            foreach (var programChecklist in programChecklists)
            {
                ChecklistItemModel programChecklistModel = new ChecklistItemModel();
                programChecklistModel.Name = programChecklist.name;
                programChecklistModel.Id = programChecklist.id;
                programChecklistModel.Target = programChecklist.target;
                programChecklistModel.Subject = programChecklist.subject;
                programChecklistModel.Order = programChecklist.order;
                programChecklistModel.ActionDays = programChecklist.response_time;

                ChecklistValidationModel programValidationModel = new ChecklistValidationModel();
                programValidationModel.Type = programChecklist.validation_type;
                programValidationModel.Input = GetValidationInput(programChecklist.validation_type, programChecklist.validation_input);
                programChecklistModel.Validation = programValidationModel;

                programChecklistModels.Add(programChecklistModel);
            }
            return programChecklistModels;
        }

        public async Task<ChecklistItemModel> GetProgramChecklistItemAsync(int programId, int checklistItemId)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter { ParameterName = "@ProgramId", Value = programId });
            parameters.Add(new SqlParameter { ParameterName = "@ChecklistId", Value = checklistItemId });
            string command = string.Format("SELECT * FROM {0}.checklist_item_master_data WHERE programID  = @ProgramId AND checklist_item_master_id = @ChecklistId", _prefix);
            List<ProgramChecklist> programChecklists = await _sql_Wrapper.GetItemsByCommandAsync<ProgramChecklist>(_connectionString, command, parameters);

            var programCheckList = programChecklists.FirstOrDefault();
            ChecklistItemModel programChecklistModel = new ChecklistItemModel();
            if (programCheckList != null)
            {
                programChecklistModel.Name = programCheckList.name;
                programChecklistModel.Id = programCheckList.id;
                programChecklistModel.Target = programCheckList.target;
                programChecklistModel.Subject = programCheckList.subject;
                programChecklistModel.ActionDays = programCheckList.response_time;

                ChecklistValidationModel programValidationModel = new ChecklistValidationModel();
                programValidationModel.Type = programCheckList.validation_type;
                programValidationModel.Input = GetValidationInput(programCheckList.validation_type, programCheckList.validation_input);
                programChecklistModel.Validation = programValidationModel;
            }
            return programChecklistModel;
        }
        public async Task<bool> GetProgramChecklistItemProgressAsync(int programId, int entityId, int checklistItemId)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter { ParameterName = "@EntityId", Value = entityId });
            parameters.Add(new SqlParameter { ParameterName = "@ChecklistId", Value = checklistItemId });
            string command = string.Format("SELECT * FROM {0}.checklist_progress WHERE checklist_item_master_id=@ChecklistId AND checklist_entity_id=@EntityId", _prefix);
            List<ChecklistProgress> progress = await _sql_Wrapper.GetItemsByCommandAsync<ChecklistProgress>(_connectionString, command, parameters);
            if (progress == null) return false;
            var checklistItemProgress = progress.FirstOrDefault();
            return checklistItemProgress == null || checklistItemProgress.checklist_value == null ? false : true;
        }


        public async Task<ChecklistItemModel> PatchProgramChecklistAsync(int programId, int itemId, ChecklistItemModel checklistItem)
        {
            if (checklistItem != null)
            {
                var set = new StringBuilder();
                bool init = false;
                if (string.IsNullOrEmpty(checklistItem.Name) == false)
                {
                    set.Append(string.Format("checklist_item_name = '{0}'", checklistItem.Name));
                    init = true;
                }
                if (string.IsNullOrEmpty(checklistItem.Target) == false)
                {
                    if (init)
                    {
                        set.Append(",");
                    }
                    else
                    {
                        init = true;
                    }
                    UserType userType;
                    if (Enum.TryParse(checklistItem.Target, out userType) == false)
                    {
                        throw new Exception("invalidate Checklist userType");
                    }
                    set.Append(string.Format("user_type_id = '{0}'", (int)userType));
                }
                if (string.IsNullOrEmpty(checklistItem.Subject) == false)
                {
                    if (init)
                    {
                        set.Append(",");
                    }
                    else
                    {
                        init = true;
                    }
                    ContextName context_Name;
                    if (Enum.TryParse(checklistItem.Subject, out context_Name) == false)
                    {
                        throw new Exception("invalidate Checklist contextName");
                    }
                    set.Append(string.Format("context_id = '{0}'", (int)context_Name));
                }
                if (string.IsNullOrEmpty(checklistItem.Validation.Type) == false)
                {
                    if (init)
                    {
                        set.Append(",");
                    }
                    else
                    {
                        init = true;
                    }
                    ChecklistItemType checklistItems;
                    if (Enum.TryParse(checklistItem.Validation.Type, out checklistItems) == false)
                    {
                        throw new Exception("invalidate Checklist itemTypeName");
                    }
                    set.Append(string.Format("checklist_item_type_id = '{0}'", (int)checklistItems));
                }
                if (checklistItem.Validation.Input != null)
                {
                    if (init)
                    {
                        set.Append(",");
                    }
                    else
                    {
                        init = true;
                    }
                    set.Append(string.Format("checklist_input_id = '{0}'", GetInputAsInt(checklistItem.Validation.Type, checklistItem.Validation.Input.ToString())));
                }
                if (checklistItem.Order > 0)
                {
                    if (init)
                    {
                        set.Append(",");
                    }
                    else
                    {
                        init = true;
                    }
                    set.Append(string.Format("checklist_sort_order = '{0}'", checklistItem.Order));
                }
                if (checklistItem.ActionDays >= 0)
                {
                    if (init)
                    {
                        set.Append(",");
                    }
                    else
                    {
                        init = true;
                    }
                    set.Append(string.Format("response_time = '{0}'", checklistItem.ActionDays));
                }
                if (init)
                {
                    List<SqlParameter> parameters = new List<SqlParameter>();
                    parameters.Add(new SqlParameter { ParameterName = "@ProgramId", Value = programId });
                    parameters.Add(new SqlParameter { ParameterName = "@ChecklistItemMasterId", Value = itemId });

                    string updateCommand = string.Format("UPDATE {0}.checklist_item_master_data SET {1} WHERE program_id = @ProgramId and checklist_item_master_id = @ChecklistItemMasterId;", _prefix, set);
                    await _sql_Wrapper.PerformCommandAsync(_connectionString, updateCommand, parameters);

                    List<ChecklistItemModel> checkListModels = await GetProgramChecklistAsync(programId, checklistItem.Subject);
                    return checkListModels.Find(x => x.Id == itemId);
                }
            }
            return null;
        }

        public async Task<List<ChecklistItemModel>> PostProgramChecklistAsync(int programId, ChecklistItemModel checklistItem)
        {

            UserType checklistUserType;
            if (Enum.TryParse(checklistItem.Target, out checklistUserType) == false)
            {
                throw new Exception("invalid Checklist userType");
            }

            ContextName context_Name;
            if (Enum.TryParse(checklistItem.Subject, out context_Name) == false)
            {
                throw new Exception("invalid Checklist contextName");
            }

            ChecklistItemType checklistItems;
            if (Enum.TryParse(checklistItem.Validation.Type, out checklistItems) == false)
            {
                throw new Exception("invalid Checklist itemTypeName");
            }
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter { ParameterName = "@in_new_checklist_item_name", Value = checklistItem.Name });
            parameters.Add(new SqlParameter { ParameterName = "@in_user_type_name", Value = checklistItem.Target });
            parameters.Add(new SqlParameter { ParameterName = "@in_context_name", Value = checklistItem.Subject });
            parameters.Add(new SqlParameter { ParameterName = "@in_response_time", Value = checklistItem.ActionDays });
            
            if (checklistItem.Validation == null)
            {
                parameters.Add(new SqlParameter { ParameterName = "@in_item_type_name", Value = null});
                parameters.Add(new SqlParameter { ParameterName = "@in_input_number", Value = 0 });

            }
            else
            {
                parameters.Add(new SqlParameter { ParameterName = "@in_item_type_name", Value = checklistItem.Validation.Type });
                if (checklistItem.Validation.Input == null)
                {
                    parameters.Add(new SqlParameter { ParameterName = "@in_input_number", Value = 0 });
                } else
                {
                    parameters.Add(new SqlParameter { ParameterName = "@in_input_number", Value = GetInputAsInt(checklistItem.Validation.Type, checklistItem.Validation.Input.ToString()) });
                }
            }
            parameters.Add(new SqlParameter { ParameterName = "@in_program_id", Value = programId });

            await _sql_Wrapper.PerformSPCommandAsync(_connectionString, "sp_add_checklist_item", parameters);

            List<ChecklistItemModel> programChecklists = await GetProgramChecklistAsync(programId, context_Name.ToString());

            await AddChecklistItemTriggerAsync(checklistItem.Triggers, programChecklists.OrderByDescending(x => x.Id).FirstOrDefault().Id);

            return programChecklists;
        }

        private async Task AddChecklistItemTriggerAsync(ChecklistItemTrigger trigger, int checklistId)
        {
            if (checklistId <= 0 || trigger == null)
            {
                return;
            }

            List<SqlParameter> parameters = null;

            if (trigger.EmailTrigger != null)
            {
                parameters = new List<SqlParameter>
                {
                    new SqlParameter { ParameterName = "@in_email_list", Value = "'" + String.Join(",", trigger.EmailTrigger.Recipients) + "'" }
                };

                TriggerHubId triggerHubId = await _sql_Wrapper.GetItemAsync<TriggerHubId>(_connectionString, "sp_add_email_trigger", parameters);

                if (triggerHubId.trigger_hub_id > 0)
                {
                    parameters = new List<SqlParameter>
                    {
                        new SqlParameter { ParameterName = "@TriggerHubId", Value = triggerHubId.trigger_hub_id },
                        new SqlParameter { ParameterName = "@ChecklistId", Value = checklistId }
                    };

                    string commandInsert = string.Format("INSERT INTO {0}.app_trigger_checklist_link (app_trigger_hub_id, checklist_hub_id) VALUES (@TriggerHubId, @ChecklistId)", _prefix);
                    await _sql_Wrapper.PerformCommandWithAutoincrementedId(_connectionString, commandInsert, parameters);
                }
            }

            if (trigger.IssueTrigger != null)
            {
                parameters = new List<SqlParameter>
                {
                    new SqlParameter { ParameterName = "@in_issue_type_name", Value = string.Format("'{0}'", trigger.IssueTrigger.IssueType.ToString()) }
                };

                TriggerHubId triggerHubId = await _sql_Wrapper.GetItemAsync<TriggerHubId>(_connectionString, "sp_add_issue_trigger", parameters);

                if (triggerHubId.trigger_hub_id > 0)
                {
                    parameters = new List<SqlParameter>
                    {
                        new SqlParameter { ParameterName = "@TriggerHubId", Value = triggerHubId.trigger_hub_id },
                        new SqlParameter { ParameterName = "@ChecklistHubId", Value = checklistId }
                    };

                    string commandInsert = string.Format("INSERT INTO {0}.app_trigger_checklist_link (app_trigger_hub_id, checklist_hub_id) VALUES (@TriggerHubId, @ChecklistHubId)", _prefix);
                    await _sql_Wrapper.PerformCommandWithAutoincrementedId(_connectionString, commandInsert, parameters);
                }
            }
        }

        public async Task<List<ChecklistItemModel>> DeleteProgramChecklist(int programId, int itemId)
        {
            string commandUpdate = string.Format("DELETE FROM {0}.checklist_item_master_data WHERE checklist_item_master_id = '{1}' AND program_id = '{2}'", _prefix, itemId, programId);
            await _sql_Wrapper.PerformCommandAsync(_connectionString, commandUpdate);

            return await GetProgramChecklistAsync(programId, "CUSTOMER"); //Todo
        }

        public async Task<List<ZipcodeInstallModel>> GetZipcodeInstallation(int programId)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter { ParameterName = "@ProgramId", Value = programId });
            List<ZipcodeInstallModel> zipcodeInstallModels = await _sql_Wrapper.GetItemsAsync<ZipcodeInstallModel>(_connectionString, "spGetZipcodeInstallations", parameters);

            zipcodeInstallModels.Reverse();

            return zipcodeInstallModels;
        }

        public async Task<List<TrendingMeasuresModel>> GetTrendingMeasures(int programId)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter { ParameterName = "@ProgramId", Value = programId });
            List<TrendingMeasuresModel> trendingMeasuresModels = await _sql_Wrapper.GetItemsAsync<TrendingMeasuresModel>(_connectionString, "spGetTopMeasures", parameters);

            trendingMeasuresModels.Reverse();

            return trendingMeasuresModels;
        }


        private async Task<long> ExecuteUpdateProgramDocumenttoreAsync(int programId, int programDocumentId, IncomingDocument updateDocument)
        {
            DocumentHub document = await GetDocumentStoreAsync(programDocumentId);
            long ProgramDocumenttoreId;

            if (document == null)
            {
                updateDocument.id = programDocumentId;
                ProgramDocumenttoreId = await ExecuteAddProgramDocumenttoreAsync(programId, updateDocument);
            }
            else
            {
                ProgramDocumenttoreId = document.document_id;

                if (updateDocument.url == null || string.IsNullOrEmpty(updateDocument.url))
                {
                    await DeleteProgramDocumenttoreAsync(Convert.ToInt32(ProgramDocumenttoreId));
                    ProgramDocumenttoreId = 0;
                }
                else
                {
                    List<SqlParameter> parameters = new List<SqlParameter>()
                    {
                       new SqlParameter { ParameterName = "@Url", Value = updateDocument.url },
                       new SqlParameter { ParameterName = "@FileId", Value = updateDocument.fileSourceId },
                       new SqlParameter { ParameterName = "@DocStoreId", Value = ProgramDocumenttoreId },
                       new SqlParameter { ParameterName = "@ProgramDocId", Value = programDocumentId }
                    };

                    string command = string.Format("UPDATE {0}.document_hub SET document_url = @Url, file_source_id = @FileId WHERE " +
                        "document_id = @DocStoreId AND program_document_id = @ProgramDocId", _prefix);
                    await _sql_Wrapper.PerformCommandAsync(_connectionString, command, parameters);
                }
            }

            return ProgramDocumenttoreId;
        }

        public async Task<List<ProgramGoal>> GetProgramGoals(int programId)
        {
            List<ProgramGoal> programGoals = null;

            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                new SqlParameter { ParameterName = "@programID", Value = programId }
            };

            List<Goal> goals = await _sql_Wrapper.GetItemsAsync<Goal>(_connectionString, "spGetProgramGoals", parameters);

            if (goals != null && goals.Count > 0)
                programGoals = new List<ProgramGoal>();

            foreach (Goal goal in goals)
            {
                ProgramGoal programGoal = new ProgramGoal
                {
                    Year = goal.year,
                    Quarter = goal.quarter,
                    Month = goal.month,
                    kWGoal = goal.kWGoal,
                    kWhGoal = goal.kWhGoal,
                    ThermsGoal = goal.thermsGoal,
                    GHGGoal = goal.ghgGoal,
                    NOXGoal = goal.noxGoal,
                    IncentivesGoal = goal.incentivesGoal,
                    CopayGoal = goal.copayGoal,
                    CostGoal = goal.costGoal
                };

                programGoals.Add(programGoal);
            }

            return programGoals;
        }

        public async Task<List<ProgramGoal>> PostProgramGoalAsync(int programId, ProgramGoal goal)
        {
            List<ProgramGoal> goals = null;

            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                new SqlParameter { ParameterName = "@in_programID", Value = programId },
                new SqlParameter { ParameterName = "@in_programYear", Value = goal.Year },
                new SqlParameter { ParameterName = "@in_programMonth", Value = goal.Month },
                new SqlParameter { ParameterName = "@in_kWGoal", Value = goal.kWGoal },
                new SqlParameter { ParameterName = "@in_kWhGoal", Value = goal.kWhGoal },
                new SqlParameter { ParameterName = "@in_thermGoal", Value = goal.ThermsGoal },
                new SqlParameter { ParameterName = "@in_incentiveGoal", Value = goal.IncentivesGoal },
                new SqlParameter { ParameterName = "@in_copayGoal", Value = goal.CopayGoal },
                new SqlParameter { ParameterName = "@in_costGoal", Value = goal.CostGoal }
            };

            await _sql_Wrapper.GetItemsAsync<Goal>(_connectionString, "spAddProgramGoal", parameters);

            goals = await GetProgramGoals(programId);

            return goals;
        }

        public async Task<List<ProgramGoal>> PutProgramGoalAsync(int programId, ProgramGoal goal)
        {
            List<ProgramGoal> goals = null;

            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                new SqlParameter { ParameterName = "@in_program_id", Value = programId },
                new SqlParameter { ParameterName = "@in_program_year", Value = goal.Year },
                new SqlParameter { ParameterName = "@in_program_month", Value = goal.Month },
                new SqlParameter { ParameterName = "@in_kWGoal_value", Value = goal.kWGoal },
                new SqlParameter { ParameterName = "@in_kWhGoal_value", Value = goal.kWhGoal },
                new SqlParameter { ParameterName = "@in_thermGoal_value", Value = goal.ThermsGoal },
                new SqlParameter { ParameterName = "@in_incentiveGoal_value", Value = goal.IncentivesGoal },
                new SqlParameter { ParameterName = "@in_performancePaymentGoal_value", Value = 0 },
                new SqlParameter { ParameterName = "@in_copayGoal_value", Value = goal.CopayGoal },
                new SqlParameter { ParameterName = "@in_budgetGoal_value", Value = goal.CostGoal }
            };

            await _sql_Wrapper.GetItemsAsync<Goal>(_connectionString, "spUpdateProgramGoal", parameters);

            goals = await GetProgramGoals(programId);

            return goals;
        }

        public async Task<List<ProgramGoal>> DeleteProgramGoalAsync(int programId, int year, int month)
        {
            List<ProgramGoal> goals = null;

            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                new SqlParameter { ParameterName = "@in_programID", Value = programId },
                new SqlParameter { ParameterName = "@in_programYear", Value = year },
                new SqlParameter { ParameterName = "@in_programMonth", Value = month }
            };

            await _sql_Wrapper.GetItemsAsync<Goal>(_connectionString, "spDelProgramGoal", parameters);

            goals = await GetProgramGoals(programId);

            return goals;
        }

        public async Task<List<ForecastModel>> GetForecastGoalsAsync(int programId, ForecastGoalType goalType)
        {
            List<ForecastModel> forecastGoals = null;

            List<string> columnNames = ConvertGoalTypeToColumnNames(goalType);

            if (columnNames != null && columnNames.Count == 2)
            {
                forecastGoals = new List<ForecastModel>();

                List<SqlParameter> parameters = new List<SqlParameter>()
                {
                    new SqlParameter { ParameterName = "@inProgramID", Value = programId },
                    new SqlParameter { ParameterName = "@inProgramGoalType", Value = "\"" + columnNames.First() + "\"" },
                    new SqlParameter { ParameterName = "@inMeasureGoalType", Value = "\"" + columnNames.Last() + "\"" }
                };

                List<Forecast> goals = await _sql_Wrapper.GetItemsAsync<Forecast>(_connectionString, "spGetForecastGoals", parameters);

                foreach (Forecast goal in goals)
                {
                    ForecastModel model = new ForecastModel
                    {
                        Year = goal.year,
                        Month = goal.month,
                        Forecasted = goal.forecasted,
                        Actual = goal.actual
                    };

                    forecastGoals.Add(model);
                }
            }

            return forecastGoals;
        }

        private List<string> ConvertGoalTypeToColumnNames(ForecastGoalType goalType)
        {
            List<string> columnNames = new List<string>();

            string programGoalType = null;
            string measureGoalType = null;

            switch (goalType)
            {
                case ForecastGoalType.KW:
                    programGoalType = "kWGoal";
                    measureGoalType = "totalkWSavings";
                    break;

                case ForecastGoalType.KWH:
                    programGoalType = "kWhGoal";
                    measureGoalType = "totalkWhSavings";
                    break;

                case ForecastGoalType.THERMS:
                    programGoalType = "thermGoal";
                    measureGoalType = "totalThermSavings";
                    break;

                case ForecastGoalType.INCENTIVES:
                    programGoalType = "incentiveGoal";
                    measureGoalType = "totalIncentives";
                    break;

                case ForecastGoalType.GHG:
                    programGoalType = "ghgGoal";
                    measureGoalType = "totalGHGSavings";
                    break;

                case ForecastGoalType.NOX:
                    programGoalType = "noxGoal";
                    measureGoalType = "totalNOXSavings";
                    break;

                case ForecastGoalType.COST:
                    programGoalType = "budgetGoal";
                    measureGoalType = "totalCost";
                    break;

                case ForecastGoalType.COPAY:
                    programGoalType = "copayGoal";
                    measureGoalType = "totalCopay";
                    break;

                default:
                    _log.Debug("Invalid forecast goal type!!");
                    break;
            }

            columnNames.Add(programGoalType);
            columnNames.Add(measureGoalType);

            return columnNames;
        }

        public async Task<DocumentModel> GetDocumentByProgramDocumentIdAsync(string contextName, int contextId, int programDocumentId)
        {
            DocumentModel model = null;

            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                new SqlParameter { ParameterName = "@inContext", Value = "\"" + contextName + "\"" },
                new SqlParameter { ParameterName = "@inContextID", Value = contextId },
                new SqlParameter { ParameterName = "@inProgramDocumentID", Value = programDocumentId }
            };

            Document document = await _sql_Wrapper.GetItemAsync<Document>(_connectionString, "spGetDocumentByProgramDocumentID", parameters);

            if (document != null)
            {
                model = new DocumentModel
                {
                    DocumentId = document.document_id,
                    ProgramDocumentId = document.program_document_id,
                    Name = document.name,
                    StorageType = ((DocumentStorageType)document.document_storage_id).ToString(),
                    DocumentURL = document.document_url,
                    FileSourceID = document.file_source_id,
                    Entity = document.document_entity,
                    EntityID = document.entity_id
                };
            }

            return model;
        }

        public async Task<ForecastModel> GetForecastGoalForValidationAsync(int programId, ForecastGoalType goalType)
        {
            ForecastModel forecastGoalModel = null;

            List<string> columnNames = ConvertGoalTypeToColumnNames(goalType);

            if (columnNames != null && columnNames.Count == 2)
            {
                List<SqlParameter> parameters = new List<SqlParameter>()
                {
                    new SqlParameter { ParameterName = "@inProgramID", Value = programId },
                    new SqlParameter { ParameterName = "@inProgramGoalType", Value = "\"" + columnNames.First() + "\"" },
                    new SqlParameter { ParameterName = "@inMeasureGoalType", Value = "\"" + columnNames.Last() + "\"" }
                };

                Forecast goal = await _sql_Wrapper.GetItemAsync<Forecast>(_connectionString, "spGetForecastGoalForValidation", parameters);

                if (goal != null)
                {
                    forecastGoalModel = new ForecastModel
                    {
                        Year = goal.year,
                        Month = goal.month,
                        Forecasted = goal.forecasted,
                        Actual = goal.actual
                    };
                }
            }

            return forecastGoalModel;
        }

        public async Task<List<ChecklistItemModel>> GetUserChecklist(int programId, ToDoQuery query)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter { ParameterName = "@inProgramID", Value = programId });

            if (query.contractorId.HasValue)
            {
                parameters.Add(new SqlParameter { ParameterName = "@inContractorID", Value = query.contractorId.Value });
            } else parameters.Add(new SqlParameter { ParameterName = "@inContractorID", Value = "NULL" });

            if (query.auditorId.HasValue)
            {
                parameters.Add(new SqlParameter { ParameterName = "@inAuditorID", Value = query.auditorId.Value });
            }
            else parameters.Add(new SqlParameter { ParameterName = "@inAuditorID", Value = "NULL" });

            List<ToDoListItem> checklists = await _sql_Wrapper.GetItemsAsync<ToDoListItem>(_connectionString, "sp_get_todo_list", parameters);

            List<ChecklistItemModel> checklistModels = new List<ChecklistItemModel>();

            foreach (var checklist in checklists)
            {
                ChecklistItemModel model = new ChecklistItemModel()
                {
                    Id = checklist.checklistItemID,
                    Name = checklist.customerName + ": " + checklist.checklistItemName,
                    Target = checklist.target,
                    Subject = checklist.subject,

                    Validation = new ChecklistValidationModel()
                    {
                        Type = checklist.validation_type,
                        Input = checklist.validation_input
                    }
                };

                if (checklist.projectID == 0)
                {
                    model.EntityId = checklist.customerID;
                    model.ChecklistId = checklist.subject + "-1";
                }
                else
                {
                    model.EntityId = checklist.projectID;
                    model.ChecklistId = checklist.customerID + "";
                    model.Name += " (" + checklist.workOrder + ")";
                }

                checklistModels.Add(model);
            }
            return checklistModels;
        }

        public async Task<List<BaseProjectModel>> GetAppointments(int programId, CalendarQuery query)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();

            if (query.contractorId > 0)
            {
                parameters.Add(new SqlParameter { ParameterName = "@inContractorID", Value = query.contractorId });
            }
            else parameters.Add(new SqlParameter { ParameterName = "@inContractorID", Value = "NULL" });

            if (query.auditorId > 0)
            {
                parameters.Add(new SqlParameter { ParameterName = "@inAuditorID", Value = query.auditorId });
            }
            else parameters.Add(new SqlParameter { ParameterName = "@inAuditorID", Value = "NULL" });

            parameters.Add(new SqlParameter { ParameterName = "@inStartDate", Value = "'" + FormatDate(query.startDate) + "'" });
            parameters.Add(new SqlParameter { ParameterName = "@inEndDate", Value = "'" + FormatDate(query.endDate) + "'" });

            List<CalendarItem> appointments = await _sql_Wrapper.GetItemsAsync<CalendarItem>(_connectionString, "sp_get_appointments", parameters);

            List<BaseProjectModel> projectModels = new List<BaseProjectModel>();

            foreach (var app in appointments)
            {
                BaseProjectModel model = new BaseProjectModel()
                {
                    id = app.projectID,
                    WorkOrder = app.projectNo,
                    //ProjectType = app.projectType,
                    ScheduledInstallationDate = app.scheduledInstallationDate,
                    ActualInstallationDate = app.actualInstallationDate,
                    AuditDate = app.auditDate,
                    PreInspectionDate = app.preInspectionDate,
                    PostInspectionDate = app.postInspectionDate,
                    Customer = new CustomerModel()
                    {
                        Id = app.customerID,
                        Name = app.name,
                        Address1 = app.address1,
                        Address2 = app.address2,
                        City = app.city,
                        State = app.state,
                        ZipCode = app.zip
                    }
                };

                projectModels.Add(model);
            }
            return projectModels;
        }

        public async Task<List<EntityNotesModel>> GetNotesAsync(int programId, string entityType, int entityId, IncomingNotes incomingNotes)
        {
            List<EntityNotesModel> entityNotesModels = new List<EntityNotesModel>();

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter { ParameterName = "@inEntityType", Value = string.Format("'{0}'", entityType) });
            parameters.Add(new SqlParameter { ParameterName = "@inEntityid", Value = entityId });

            if (string.IsNullOrEmpty(incomingNotes.userid) == false)
            {
                parameters.Add(new SqlParameter { ParameterName = "@inFilterUserId", Value = string.Format("'{0}'", incomingNotes.userid) });
            }
            else
            {
                parameters.Add(new SqlParameter { ParameterName = "@inFilterUserId", Value = "NULL" });
            }

            if (string.IsNullOrEmpty(incomingNotes.noteContext) == false)
            {
                parameters.Add(new SqlParameter { ParameterName = "@inFilterContext", Value = string.Format("'{0}'", incomingNotes.noteContext) });
            }
            else
            {
                parameters.Add(new SqlParameter { ParameterName = "@inFilterContext", Value = "NULL" });
            }

            if (string.IsNullOrEmpty(incomingNotes.noteType) == false)
            {
                parameters.Add(new SqlParameter { ParameterName = "@inFilterNoteType", Value = string.Format("'{0}'", incomingNotes.noteType) });
            }
            else
            {
                parameters.Add(new SqlParameter { ParameterName = "@inFilterNoteType", Value = "NULL" });
            }

            List<GetNotes> notes = await _sql_Wrapper.GetItemsAsync<GetNotes>(_connectionString, "sp_get_notes", parameters);
            foreach(var note in notes)
            {
                EntityNotesModel entityNotesModel = new EntityNotesModel();

                entityNotesModel.userId = note.authorUserId;
                entityNotesModel.entityId = note.entityId;
                entityNotesModel.text = note.noteText;
                entityNotesModel.entityType = note.entityType;

                Author author = new Author();
                author.userId = note.authorUserId;
                entityNotesModel.author = author;

                entityNotesModel.date = FormatDate(note.noteDate);

                NoteContext noteContext = new NoteContext();
                //TODO
                entityNotesModel.noteContext = noteContext;

                entityNotesModel.noteType = note.noteType;

                entityNotesModels.Add(entityNotesModel);
            }

            return entityNotesModels;
        }

        public async Task<EntityNotesModel> GetNoteByIdAsync(int noteId, string entityType, int entityId)
        {
            EntityNotesModel entityNotesModel = new EntityNotesModel();

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter { ParameterName = "@inEntityType", Value = string.Format("'{0}'", entityType) });
            parameters.Add(new SqlParameter { ParameterName = "@inEntityid", Value = entityId });
            parameters.Add(new SqlParameter { ParameterName = "@inFilterUserId", Value = "NULL" });
            parameters.Add(new SqlParameter { ParameterName = "@inFilterContext", Value = "NULL" });
            parameters.Add(new SqlParameter { ParameterName = "@inFilterNoteType", Value = "NULL" });
            List<GetNotes> notes = await _sql_Wrapper.GetItemsAsync<GetNotes>(_connectionString, "sp_get_notes", parameters);
            
            GetNotes note = notes.Where(x => x.id == noteId).FirstOrDefault();
            if (note != null)
            {
                entityNotesModel.userId = note.authorUserId;
                entityNotesModel.entityId = note.entityId;
                entityNotesModel.text = note.noteText;
                entityNotesModel.entityType = note.entityType;

                Author author = new Author();
                author.userId = note.authorUserId;
                entityNotesModel.author = author;

                entityNotesModel.date = FormatDate(note.noteDate);

                NoteContext noteContext = new NoteContext();
                //TODO
                entityNotesModel.noteContext = noteContext;

                entityNotesModel.noteType = note.noteType;
            }
            
            return entityNotesModel;
        }

        public async Task<EntityNotesModel> PostNotesAsync(int programId, IncomingNotes incomingNotes)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();

            ContextName contextName;
            if (Enum.TryParse<ContextName>(incomingNotes.entityType, out contextName) == false)
            {
                throw new Exception("Invalid entityType");
            }
            parameters.Add(new SqlParameter { ParameterName = "@in_context_name", Value = string.Format("'{0}'", contextName) });
            parameters.Add(new SqlParameter { ParameterName = "@in_author_name", Value = string.Format("'{0}'", incomingNotes.userid) });


            ChecklistItemType checklistItemType;
            if (Enum.TryParse<ChecklistItemType>(incomingNotes.noteContext, out checklistItemType) == false)
            {
                throw new Exception("Invalid entityType");
            }
            parameters.Add(new SqlParameter { ParameterName = "@in_note_context_type_name", Value = string.Format("'{0}'", checklistItemType.ToString()) });
            parameters.Add(new SqlParameter { ParameterName = "@in_entity_id", Value = incomingNotes.entityId.ToString() });
            parameters.Add(new SqlParameter { ParameterName = "@in_note_text", Value = string.Format("'{0}'", incomingNotes.text) });

            NoteType noteType;
            if (Enum.TryParse<NoteType>(incomingNotes.noteType, out noteType) == false)
            {
                throw new Exception("Invalid noteType");
            }
            parameters.Add(new SqlParameter { ParameterName = "@in_note_type_name", Value = string.Format("'{0}'", noteType.ToString()) });
            parameters.Add(new SqlParameter { ParameterName = "@in_program_id", Value = programId });

            List<AddNotes> addNotes = await _sql_Wrapper.GetItemsAsync<AddNotes>(_connectionString, "sp_add_note", parameters);
            var addNote = addNotes.FirstOrDefault();
            if (addNote != null)
            {
                return await GetNoteByIdAsync(addNote.note_hub_id, incomingNotes.entityType, incomingNotes.entityId);
            }
            else
            {
                return new EntityNotesModel();
            }
        }

        public async Task<List<PhotoModel>> GetPhotos (string contextName, int entityId)
        {
            List<PhotoModel> models = new List<PhotoModel>();

            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                new SqlParameter { ParameterName = "@inEntityType", Value = "'" + contextName + "'" },
                new SqlParameter { ParameterName = "@inEntityID", Value = entityId }
            };

            List<Photo> photos = await _sql_Wrapper.GetItemsAsync<Photo>(_connectionString, "sp_get_photos", parameters);

            foreach (var photo in photos)
            {
                ContextName context;
                DocumentStorageType location;

                if (Enum.TryParse(photo.entityType, out context) &&
                    Enum.TryParse(photo.fileLocation, out location))
                {
                    models.Add(new PhotoModel()
                    {
                        Id = photo.id,
                        Context = context,
                        EntityId = photo.entityId,
                        PhotoName = photo.photoName,
                        DocumentURL = photo.documentURL,
                        FileSourceId = photo.fileStorageId,
                        StorageLocation = location,
                        PhotoDate = photo.photoDate
                    });
                }

            }

            return models;
        }

        public async Task<PhotoModel> AddPhoto(PhotoModel photo)
        {
            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                new SqlParameter { ParameterName = "@inEntityType", Value = "'" + photo.Context.ToString() + "'" },
                new SqlParameter { ParameterName = "@inPhotoName", Value = "'" + photo.PhotoName  + "'"},
                new SqlParameter { ParameterName = "@inEntityID", Value = photo.EntityId },
                new SqlParameter { ParameterName = "@inDocumentURL", Value = "'" + photo.DocumentURL + "'" },
                new SqlParameter { ParameterName = "@inFileSourceId", Value = "'" + photo.FileSourceId  + "'"},
                new SqlParameter { ParameterName = "@inDocumentStorageType", Value = "'" + photo.StorageLocation.ToString()  + "'"},
            };

            List<AddPhoto> photoId = await _sql_Wrapper.GetItemsAsync<AddPhoto>(_connectionString, "sp_add_photo", parameters);

            if (photoId != null)
            {
                return await GetPhotoById(photoId.FirstOrDefault().photo_hub_id);
            }
            else return null;
            
        }

        public async Task<PhotoModel> GetPhotoById(int photoId)
        {
            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                new SqlParameter { ParameterName = "@inPhotoID", Value = photoId }
            };

            Photo photo = await _sql_Wrapper.GetItemAsync<Photo>(_connectionString, "sp_get_photo", parameters);

            ContextName context;
            DocumentStorageType location;

            if (Enum.TryParse(photo.entityType, out context) &&
                Enum.TryParse(photo.fileLocation, out location))
            {
                return new PhotoModel()
                {
                    Id = photo.id,
                    Context = context,
                    EntityId = photo.entityId,
                    PhotoName = photo.photoName,
                    DocumentURL = photo.documentURL,
                    FileSourceId = photo.fileStorageId,
                    StorageLocation = location,
                    PhotoDate = photo.photoDate
                };
            }

            return null;
        }

        public async Task DeletePhoto(int photoId)
        {

            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                new SqlParameter { ParameterName = "@in_photo_id", Value = photoId }
            };

            await _sql_Wrapper.PerformSPCommandAsync(_connectionString, "sp_delete_photo", parameters);
        }

        public async Task<List<ReportModel>> GetAllGeneratedReports(int programId, int contractorId, int auditorId)
        {
            List<ReportModel> models = new List<ReportModel>();

            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                new SqlParameter { ParameterName = "@inProgramID", Value = programId },
                new SqlParameter { ParameterName = "@inContractorID", Value = contractorId },
                new SqlParameter { ParameterName = "@inAuditorID", Value = auditorId }
            };

            List<Report> reports = await _sql_Wrapper.GetItemsAsync<Report>(_connectionString, "sp_get_all_generated_reports", parameters);

            foreach (var report in reports)
            {
                ContextName context;
                DocumentStorageType location;

                if (Enum.TryParse(report.entityType, out context) &&
                    Enum.TryParse(report.fileLocation, out location))
                {
                    models.Add(new ReportModel()
                    {
                        Id = report.id,
                        Context = context,
                        EntityId = report.entityId,
                        ReportName = report.reportName,
                        DocumentURL = report.documentURL,
                        FileSourceId = report.fileStorageId,
                        StorageLocation = location,
                        DocumentId = report.documentId,
                        EntityName = report.entityName,
                        GeneratedDate = report.reportDate
                    });
                }

            }

            return models;
        }

        public async Task<List<ReportModel>> GetGeneratedReports(string contextName, int entityId)
        {
            List<ReportModel> models = new List<ReportModel>();

            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                new SqlParameter { ParameterName = "@inEntityType", Value = "'" + contextName + "'" },
                new SqlParameter { ParameterName = "@inEntityID", Value = entityId }
            };

            List<Report> reports = await _sql_Wrapper.GetItemsAsync<Report>(_connectionString, "sp_get_generated_reports", parameters);

            foreach (var report in reports)
            {
                ContextName context;
                DocumentStorageType location;

                if (Enum.TryParse(report.entityType, out context) &&
                    Enum.TryParse(report.fileLocation, out location))
                {
                    models.Add(new ReportModel()
                    {
                        Id = report.id,
                        Context = context,
                        EntityId = report.entityId,
                        ReportName = report.reportName,
                        DocumentURL = report.documentURL,
                        FileSourceId = report.fileStorageId,
                        StorageLocation = location,
                        DocumentId = report.documentId,
                        EntityName = report.entityName,
                        GeneratedDate = report.reportDate
                    });
                }

            }

            return models;
        }

        public async Task<ReportModel> AddGeneratedReport(ReportModel report)
        {
            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                new SqlParameter { ParameterName = "@inEntityType", Value = "'" + report.Context.ToString() + "'" },
                new SqlParameter { ParameterName = "@inPhotoName", Value = "'" + report.ReportName  + "'"},
                new SqlParameter { ParameterName = "@inEntityID", Value = report.EntityId },
                new SqlParameter { ParameterName = "@inDocumentURL", Value = "'" + report.DocumentURL + "'" },
                new SqlParameter { ParameterName = "@inFileSourceId", Value = "'" + report.FileSourceId  + "'"},
                new SqlParameter { ParameterName = "@inDocumentStorageType", Value = "'" + report.StorageLocation.ToString()  + "'"},
            };

            List<AddReport> reportId = await _sql_Wrapper.GetItemsAsync<AddReport>(_connectionString, "sp_add_generated_report", parameters);

            if (reportId != null)
            {
                return await GetGeneratedReportById(reportId.FirstOrDefault().generated_report_id);
            }
            else return null;

        }

        public async Task<ReportModel> GetGeneratedReportById(int reportId)
        {
            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                new SqlParameter { ParameterName = "@inReportId", Value = reportId }
            };

            Report report = await _sql_Wrapper.GetItemAsync<Report>(_connectionString, "sp_get_generated_report", parameters);

            ContextName context;
            DocumentStorageType location;

            if (Enum.TryParse(report.entityType, out context) &&
                Enum.TryParse(report.fileLocation, out location))
            {
                return new ReportModel()
                {
                    Id = report.id,
                    Context = context,
                    EntityId = report.entityId,
                    ReportName = report.reportName,
                    DocumentURL = report.documentURL,
                    FileSourceId = report.fileStorageId,
                    StorageLocation = location,
                    DocumentId = report.documentId,
                    EntityName = report.entityName,
                    GeneratedDate = report.reportDate
                };
            }

            return null;
        }

        public async Task DeleteGeneratedReport(int reportId)
        {

            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                new SqlParameter { ParameterName = "@in_report_id", Value = reportId }
            };

            await _sql_Wrapper.PerformSPCommandAsync(_connectionString, "sp_delete_generated_report", parameters);
        }

        public async Task<List<WeekValueModel>> GetWeeklyTrendData(int programId, MeasureFilter query)
        {

            List<WeekValueModel> weekValueModels = new List<WeekValueModel>();

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter { ParameterName = "@InProgramID", Value = programId });

            if (query.start_date.HasValue)
            {
                parameters.Add(new SqlParameter { ParameterName = "@InStartDate", Value = string.Format("'{0}'", query.start_date?.ToString("yyyy-MM-dd")) });
            }
            else
            {
                parameters.Add(new SqlParameter { ParameterName = "@InStartDate", Value = "null" });
            }

            if (query.end_date.HasValue)
            {
                parameters.Add(new SqlParameter { ParameterName = "@InEndDate", Value = string.Format("'{0}'", query.end_date?.ToString("yyyy-MM-dd")) });
            }
            else
            {
                parameters.Add(new SqlParameter { ParameterName = "@InEndDate", Value = "null" });
            }

            List<WeeklyKWh> weeklyKWhs = await _sql_Wrapper.GetItemsAsync<WeeklyKWh>(_connectionString, "sp_get_program_trends", parameters);
            foreach(var weeklyKWh in weeklyKWhs)
            {
                WeekValueModel weekValueModel = new WeekValueModel();
                weekValueModel.date = FormatDate(weeklyKWh.Start_of_Week);
                weekValueModel.kWh = weeklyKWh.kWh;
                weekValueModels.Add(weekValueModel);
            }

            return weekValueModels;
        }

        public async Task<List<string>> GetProgramSubSegmentsAsync (int programId)
        {
            List<string> subSegments = null;

            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter { ParameterName = "@InProgramID", Value = programId }
            };

            List<SubSegment> subSegmentList = await _sql_Wrapper.GetItemsAsync<SubSegment>(_connectionString, "sp_get_program_subsegments", parameters);

            if (subSegmentList != null && subSegmentList.Count > 0)
            {
                subSegments = subSegmentList.Select(item => item.businessType).ToList();
            }

            return subSegments;
        }
    }
}
