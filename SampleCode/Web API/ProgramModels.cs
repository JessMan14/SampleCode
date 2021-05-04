using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace ProgramProvider_File
{


    public class Program
    {
        [Key]
        public int utilityID { get; set; }
        public string utilityName { get; set; }
        public int programID { get; set; }
        public string programName { get; set; }
        public string programStartDate { get; set; }
        public string programEndDate { get; set; }
        public string geoLat { get; set; }
        public string geoLong { get; set; }
        public string geoZoom { get; set; }
        public string type { get; set; }
        public int parentID { get; set; }
    }

    public class ProgramMetric
    {
        [Key]
        public int programID { get; set; }
        public int programYear { get; set; }
        public int programQtr { get; set; }
        public int kWGoal { get; set; }
        public int kWhGoal { get; set; }
        public int thermGoal { get; set; }
        public int incentiveGoal { get; set; }
        public int costGoal { get; set; }
        public int copayGoal { get; set; }
        public double kWComplete { get; set; }
        public double kWhComplete { get; set; }
        public double thermComplete { get; set; }
        public double incentivesComplete { get; set; }
        public double costComplete { get; set; }
        public double copayComplete { get; set; }
        public double kWInProgress { get; set; }
        public double kWhInProgress { get; set; }
        public double thermInProgress { get; set; }
        public double incentivesInProgress { get; set; }
        public double costInProgress { get; set; }
        public double copayInProgress { get; set; }
    }


    public class ProgramIssues
    {
        [Key]
        public int programID { get; set; }
        public int customerID { get; set; }
        public string SegmentType { get; set; }
        public int issueID { get; set; }
        public string issueTitle { get; set; }
        public string issueDescription { get; set; }
        public string issueType { get; set; }
        public DateTime issueSubmitted { get; set; }
        public DateTime issueResolved { get; set; }
        public int projectID { get; set; }
        public string customerName { get; set; }
        public string customerAddress1 { get; set; }
        public string customerAddress2 { get; set; }
        public string customerCity { get; set; }
        public string customerState { get; set; }
        public string customerZip { get; set; }
        public string projectStatus { get; set; }
    }

    public class ProgramInvoices
    {
        [Key]
        public int invoiceID { get; set; }
        public int projectID { get; set; }
        public int customerID { get; set; }
        public string customerName { get; set; }
        public string customerAddress1 { get; set; }
        public string customerAddress2 { get; set; }
        public string customerCity { get; set; }
        public string customerState { get; set; }
        public string customerZip { get; set; }
        public string projectStatus { get; set; }
        public DateTime invoiceSubmitted { get; set; }
        public string invoiceStatus { get; set; }
        public DateTime contractorPaymentDate { get; set; }
        public string invoicePaid { get; set; }
        public DateTime dueDate { get; set; }
        public double invoiceAmount { get; set; }
        public int documentID { get; set; }
        public string SegmentType { get; set; }
    }

    public class ProgramMessages
    {
        [Key]
        public int customerMessageID { get; set; }
        public string messageSubject { get; set; }
        public string messageFrom { get; set; }
        public string messageTo { get; set; }
        public string messageBody { get; set; }
        public DateTime messageSubmitted { get; set; }
        public string messageStatus { get; set; }
        public int customerID { get; set; }
        public string customerName { get; set; }
        public string customerAddress1 { get; set; }
        public string customerAddress2 { get; set; }
        public string customerCity { get; set; }
        public string customerState { get; set; }
        public string customerZip { get; set; }
        public int contractorID { get; set; }
        public string contractorName { get; set; }
    }
    public class ProgramPipeline
    {
        [Key]
        public int programID { get; set; }
        public int pipelineNameID { get; set; }
        public string pipelineName { get; set; }
        public string label { get; set; }
        public int pipelineOrder { get; set; }
    }

    public class PipelineElement
    {
        [Key]
        public string name { get; set; }
        public int count { get; set; }
        public double kW { get; set; }
        public double kWh { get; set; }
        public double therms { get; set; }
        public double ghg { get; set; }
        public double nox { get; set; }
        public double incentives { get; set; }
        public double savings { get; set; }
        public double expenditures { get; set; }
        public string label { get; set; }
        public int checklist_item { get; set; }
    }

    public class ProgramKpi
    {
        [Key]
        public int programID { get; set; }
        public string KPILabel { get; set; }
        public string KPIType { get; set; }
        public string rubricLow { get; set; }
        public string rubricLowLower { get; set; }
        public string rubricLowUpper { get; set; }
        public string rubricMedium { get; set; }
        public string rubricMediumLower { get; set; }
        public string rubricMediumUpper { get; set; }
        public string rubricHigh { get; set; }
        public string rubricHighLower { get; set; }
        public string rubricHighUpper { get; set; }
        public int KPIOrder { get; set; }
        public string zip { get; set; }
        public string SegmentType { get; set; }
        public string measures { get; set; }
    }


    public class Saving
    {
        [Key]
        public int year { get; set; }
        public double kW { get; set; }
        public double kWh { get; set; }
        public double therms { get; set; }
        public double ghg { get; set; }
        public double nox { get; set; }
    }

    public class DivisionDataModel
    {
        [Key]
        public string divisionCode { get; set; }
        public string divisionName { get; set; }
        public int programID { get; set; }
    }

    public class ProgramMeasure
    {
        [Key]
        public int measureID { get; set; }
        public string solutionCode { get; set; }
        public string name { get; set; }
        public string measureType { get; set; }
        public double measureCost { get; set; }
        public double measureIncentive { get; set; }
        public double measureCopay { get; set; }
        public double measurekWSavings { get; set; }
        public double measurekWhSavings { get; set; }
        public double measureThermSavings { get; set; }
        public string Image { get; set; }
        public string Papers { get; set; }
        public bool IsActive { get; set; }
    }

    public class Benefit
    {
        [Key]
        public string benefit { get; set; }
    }

    public class ZipCode
    {
        [Key]
        public string zipCode { get; set; }
    }

    [DataContract()]
    public class PipelineElementModel
    {
        [DataMember()]
        public string Element { get; set; }
        [DataMember()]
        public string Label { get; set; }
    }

    public class ProgramDashboard
    {
        [Key]
        public int programID { get; set; }
        public int dashboardID { get; set; }
    }

    public class IncomingMarketingMaterial
    {
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("URL")]
        public string Url { get; set; }
        [JsonProperty("AdditionalInfo")]
        public string AdditionalInfo { get; set; }
    }

    public class MarketingMaterial
    {
        [Key]
        public int materialID { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string additionalInfo { get; set; }
    }

    public class ProgramDashboardDefault
    {
        [Key]
        public int programID { get; set; }
        public string userType { get; set; }
        public int dashboardDefaultID { get; set; }
    }

    [DataContract()]
    public class MeasureSavings
    {
        [DataMember()]
        public string subsegment { get; set; }
        [DataMember()]
        public string savingsKWH { get; set; }
        [DataMember()]
        public string savingsDollars { get; set; }
    }


    [DataContract()]
    public class ProgramPotentialSavings
    {
        [DataMember()]
        public string energyConsumption { get; set; }
        [DataMember()]
        public string energySavings { get; set; }
    }

    public class PotentialSavings
    {
        [Key]
        public double kWhSavings { get; set; }
        public double totalEnergy { get; set; }
    }

    [DataContract()]
    public class MeasureType
    {
        [DataMember()]
        public string type { get; set; }
        [DataMember()]
        public string savingsKWH { get; set; }
        [DataMember()]
        public string savingsDollars { get; set; }
    }

    public class MeasuresByType
    {
        [Key]
        public string measureType { get; set; }
        public double kWhSavings { get; set; }
        public double savingDollars { get; set; }
    }

    public class MeasuresBySubsegment
    {
        [Key]
        public string subSegment { get; set; }
        public double kWhSavings { get; set; }
        public double savingDollars { get; set; }
    }

    public class ProgramDocument
    {
        [Key]
        public int program_document_id { get; set; }
        public int programID { get; set; }
        public string name { get; set; }
        public int document_storage_id { get; set; }
        public int document_id { get; set; }
        public int is_required { get; set; }
        public DateTime recorded_created_date { get; set; }
    }

    public class DocumentHub
    {
        [Key]
        public int document_id { get; set; }
        public int document_storage_id { get; set; }
        public string document_url { get; set; }
        public string file_source_id { get; set; }
        public int program_document_id { get; set; }
        public DateTime record_created_date { get; set; }
    }

    public class DocumentLink
    {
        [Key]
        public int document_link_id { get; set; }
        public int document_id { get; set; }
        public int entity_id { get; set; }
        public int document_entity_id { get; set; }
        public DateTime record_created_date { get; set; }
    }

    public class IncomingDocument
    {
        public int id { get; set; }
        public string name { get; set; }
        public bool isRequired { get; set; }
        public string fileSourceId { get; set; }
        public string url { get; set; }
    }

    [DataContract]
    public class Document
    {
        public int document_id { get; set; }
        public int program_document_id { get; set; }
        public string name { get; set; }
        public int document_storage_id { get; set; }
        public string document_url { get; set; }
        public string file_source_id { get; set; }
        public string document_entity { get; set; }
        public int entity_id { get; set; }
    }

    [DataContract]
    public class SubsegmentTypeModel
    {
        [DataMember()]
        public string Subsegment { get; set; }
        [DataMember()]
        public TypeModel Types { get; set; }
        [DataMember()]
        public Dictionary<string, string> TypesList { get; set; }
    }

    [DataContract]
    public class TypeModel
    {
        [DataMember()]
        public string Lighting { get; set; }
        [DataMember()]
        public string Refrigeration { get; set; }
        [DataMember()]
        public string HVAC { get; set; }
        [DataMember()]
        public string Other { get; set; }
    }

    public class MeasureSubsegmentType
    {
        [Key]
        public string businessType { get; set; }
        public string measureType { get; set; }
        public double kWh_Savings { get; set; }
    }

    public class ProgramChecklist
    {
        [Key]
        public int id { get; set; }
        public string name { get; set; }
        public string target { get; set; }
        public string subject { get; set; }
        public string validation_type { get; set; }
        public int validation_input { get; set; }
        public int order { get; set; }
        public int response_time { get; set; }
    }

    public class ChecklistProgress
    {
        [Key]
        public int checklist_progress_id { get; set; }
        public int checklist_item_master_id { get; set; }
        public int checklist_entity_id { get; set; }
        public string checklist_value { get; set; }
    }

    public class ProgramPostChecklist
    {
        [Key]
        public int checklist_item_master_id { get; set; }
        public string checklist_item_name { get; set; }
        public int user_type_id { get; set; }
        public int context_id { get; set; }
        public int checklist_item_type_id { get; set; }
        public int checklist_input_id { get; set; }
        public int program_id { get; set; }
        public int checklist_sort_order { get; set; }
    }

    [DataContract]
    public class ZipcodeInstallModel
    {
        [DataMember()]
        public string zipcode { get; set; }
        [DataMember()]
        public string installations { get; set; }
    }

    public class ZipcodeInstall
    {
        [Key]
        public string zipcode { get; set; }
        public int installations { get; set; }
    }

    [DataContract]
    public class TrendingMeasuresModel
    {
        [DataMember()]
        public string measure { get; set; }
        [DataMember()]
        public string saving { get; set; }
    }

    public class MeasuresSaving
    {
        [Key]
        public string measures { get; set; }
        public double saving { get; set; }
    }

    public class Goal
    {
        [Key]
        public int year { get; set; }
        public int quarter { get; set; }
        public int month { get; set; }
        public double kWGoal { get; set; }
        public double kWhGoal { get; set; }
        public double thermsGoal { get; set; }
        public double ghgGoal { get; set; }
        public double noxGoal { get; set; }
        public double incentivesGoal { get; set; }
        public double copayGoal { get; set; }
        public double costGoal { get; set; }
    }

    public class Forecast
    {
        [Key]
        public int year { get; set; }
        public int month { get; set; }
        public double forecasted { get; set; }
        public double actual { get; set; }
    }

    public class ToDoListItem
    {
        [Key]
        public int customerID { get; set; }
        public int projectID { get; set; }
        public string customerName { get; set; }
        public string workOrder { get; set; }
        public int contractorID { get; set; }
        public int auditorID { get; set; }
        public int checklistItemID { get; set; }
        public string checklistItemName { get; set; }
        public string subject { get; set; }
        public string target { get; set; }
        public string validation_type { get; set; }
        public int validation_input { get; set; }
    }

    public class CalendarItem
    {
        [Key]
        public int projectID { get; set; }
        public string projectNo { get; set; }
        //public string projectType { get; set; }

        public int customerID { get; set; }
        public string name { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }

        public DateTime auditDate { get; set; }
        public DateTime scheduledInstallationDate { get; set; }
        public DateTime actualInstallationDate { get; set; }
        public DateTime preInspectionDate { get; set; }
        public DateTime postInspectionDate { get; set; }
    }

    public class AddNotes
    {
        [Key]
        public int note_hub_id { get; set; }
    }

    public class GetNotes
    {
        [Key]
        public int id { get; set; }
        public string entityType { get; set; }
        public int entityId { get; set; }
        public string noteText { get; set; }
        public string authorUserId { get; set; }
        public string authorFullName { get; set; }
        public DateTime noteDate { get; set; }
        public string noteContext { get; set; }
        public string noteType { get; set; }
    }

    public class Photo
    {
        [Key]
        public int id { get; set; }

        public string entityType { get; set; } 

        public int entityId { get; set; }

        public string photoName { get; set; }

        public string documentURL { get; set; }

        public string fileStorageId { get; set; }

        public string fileLocation { get; set; }

        public DateTime photoDate { get; set; }
    }

    public class AddPhoto
    {
        [Key]
        public int photo_hub_id { get; set; }
    }

    public class Report
    {
        [Key]
        public int id { get; set; }

        public string entityType { get; set; }

        public int entityId { get; set; }

        public string reportName { get; set; }

        public string documentURL { get; set; }

        public string fileStorageId { get; set; }

        public string fileLocation { get; set; }

        public DateTime reportDate { get; set; }

        public int documentId { get; set; }

        public string entityName { get; set; }
    }

    public class AddReport
    {
        [Key]
        public int generated_report_id { get; set; }
    }

    public class WeeklyKWh
    {
        [Key]
        public int programID { get; set; }
        public DateTime Start_of_Week { get; set; }
        public double kWh { get; set; }
    }

    public class SubSegment
    {
        public string businessType { get; set; }
    }


    public class Issue
    {
        public int id { get; set; }
        public string entityType { get; set; }

        public int entityId { get; set; }

        public string subjectLabel { get; set; }

        public string description { get; set; }

        public string title { get; set; }

        public string issueType { get; set; }

        public int priority { get; set; }

        public DateTime submittedDate { get; set; }

        public DateTime? resolvedDate { get; set; }
    }

    public class TriggerHubId
    {
        [Key]
        public int trigger_hub_id { get; set; }
    }
}
