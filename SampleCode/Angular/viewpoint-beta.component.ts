import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-viewpoint-beta',
  templateUrl: './viewpoint-beta.component.html',
  styleUrls: ['./viewpoint-beta.component.scss']
})
export class ViewPointBetaComponent implements OnInit {

  constructor() { }

  dropdownItems = [1, 2, 3, 4, 5, 6];
  documentItems = [
    '/assets/img/esrld.png',
    '/assets/img/cbocs.png',    
    '/assets/img/padorg.png', 
    '/assets/img/padusr.png', 
    '/assets/img/padrol.png', 
    '/assets/img/csrovr.png'
  ];

  documentItemsHold = [1, 2, 3, 4, 5, 6];
  videoItemsHold = [1, 2, 3];
  
  videoItems = [{ 
    source: '/assets/video/ViewPoint Customer Search.mp4', image: '/assets/img/video1.PNG',
    headline: 'VideoPoint W001',
    description: 'Customer Search by Name'
    },{ 
      source: '/assets/video/ViewPoint Customer Search.mp4', image: '/assets/img/video2.PNG',
      headline: 'VideoPoint W001',
      description: 'Customer Search by Name'
    },{
      source: '/assets/video/ViewPoint Customer Search.mp4', image: '/assets/img/video3.PNG',
      headline: 'VideoPoint W001',
      description: 'Customer Search by Name'
    }
  ];

  showModal: boolean = false;

  stages = ["Committed", "Install Assigned", "Install Scheduled"]

  districtItems = ['District 1', 'District 2', 'District 3', 'District 4'];
  yesnoItems = ['Yes', 'No'];
  rateItems = ['EA1A-STD', 'EA2B-STD', 'EA2B-BC'];
  reasonItems = ['Later', 'Business Closing'];
  procedureItems = ['sp_proc1', 'sp_proc2', 'sp_proc3'];
  worksheetItems = ['worksheet-1', 'worksheet-2', 'worksheet-3'];
  customerStatusItems = ['Eligible', 'Ineligible', 'Closed'];
  customerStageItems = ['Unsurveyed', 'Prospect', 'Non-prospect'];
  stateItems = ['CA', 'DE', 'UT', 'WA'];
  cityItems = ['Los Angeles', 'San Diego', 'San Fransisco'];
  roleItems = ['CBO', 'ESR', 'Auditor'];
  orgItems = ['Acme Electric Company', 'Willdan Lighting', 'Org Name'];
  leadsourceItems = ['CBO List', 'Marketing campaign', 'Proposed-not-sold campaign', 'Other lead sources TBD'];
  invoicecodeItems = ['AEC', 'ABC', 'DOE'];
  opentimes = [8.00, 8.30, 9.00, 9.30, 10.00];
  closetimes = [5.00, 5.30, 6.00, 6.30, 7.00];
  textareaMaxlength = 300;
  textareaTitle = 'Add Comment';
  textareaContent = '';

  headerStartContent = {
    icon: {
      type: 'fas',
      name: 'shield-alt'
    },
    label: 'ViewPoint',
    link: '/'
  };

  headerEndContent = [
    {
      icon: {
        type: 'fas',
        name: 'search'
      },
      link: '/#'
    },
    {
      icon: {
        type: 'fas',
        name: 'cog'
      },
      link: '/#'
    },
    {
      icon: {
        type: 'fas',
        name: 'bell'
      },
      link: '/#'
    }
  ];

  headerBodyContent = [
    {
      label: 'Projects',
      link: '/#'
    },
    {
      label: 'People',
      link: '/#'
    },
    {
      label: 'Teams',
      link: '/#'
    }
  ];


  actionItems = [
    {
      id: 0,
      label: 'Action',
      isActive: true
    },
    {
      id: 1,
      label: 'Meta',
      isActive: false
    },
    {
      id: 2,
      label: 'Notes',
      isActive: false
    }
  ];

 accountItems = [
    {
      id: 0,
      label: 'Account',
      isActive: true
    },
    {
      id: 1,
      label: 'Project',
      isActive: false
    },
    {
      id: 2,
      label: 'Work Order',
      isActive: false
    },
    {
      id: 3,
      label: 'Audit',
      isActive: false
    },
    {
      id: 4,
      label: 'Overview',
      isActive: false
    },
    {
      id: 5,
      label: 'Analysis',
      isActive: false
    }
  ];

  projectItems = [
    {
      id: 0,
      label: 'Account',
      isActive: false
    },
    {
      id: 1,
      label: 'Project',
      isActive: true
    },
    {
      id: 2,
      label: 'Work Order',
      isActive: false
    },
    {
      id: 3,
      label: 'Audit',
      isActive: false
    },
    {
      id: 4,
      label: 'Overview',
      isActive: false
    },
    {
      id: 5,
      label: 'Analysis',
      isActive: false
    }
  ];

  workOrderItems = [
    {
      id: 0,
      label: 'Account',
      isActive: false
    },
    {
      id: 1,
      label: 'Project',
      isActive: false
    },
    {
      id: 2,
      label: 'Work Order',
      isActive: true
    },
    {
      id: 3,
      label: 'Audit',
      isActive: false
    },
    {
      id: 4,
      label: 'Overview',
      isActive: false
    },
    {
      id: 5,
      label: 'Analysis',
      isActive: false
    }
  ];

  tabItems = [
    {
      id: 0,
      label: 'Account',
      isActive: true
    },
    {
      id: 1,
      label: 'Project',
      isActive: false
    },
    {
      id: 2,
      label: 'Work Order',
      isActive: false
    },
    {
      id: 3,
      label: 'Audit',
      isActive: false
    },
    {
      id: 4,
      label: 'Overview',
      isActive: false
    },
    {
      id: 5,
      label: 'Analysis',
      isActive: false
    }
  ];

  notes = [
    {
      name: "Lee D Person",
      date: "10/4/2020",
      phase: "Lead Phase",
      note: "This is how the notes input will look. Information here will be scrollable and easy to get to. If more information is added to the header of if the name is long there will be scrolling to the right as well.",
    },
    {
      name: "Iona Lyte",
      date: "10/14/2020",
      phase: "Material Issue",
      note: "There was an issue with materials. Information here will be scrollable and easy to get to. ",
    },
    {
      name: "Les Watts",
      date: "10/24/2020",
      phase: "Construction Issue",
      note: "There was a construction issue. Information here will be scrollable and easy to get to. ",
    },
    {
      name: "Paige Turner",
      date: "10/24/2020",
      phase: "Post-Inspection",
      note: "This was a post-inspection note. Information here will be scrollable and easy to get to. ",
    },
  ];

  CUSTOMER_DATA = [
    {
      accountNo: '*********9888',
      customerName: 'Architecture Firm LLC',
      address: '1234 W Main St',
      city: 'Los Angeles',
      zip: '90007',
      kw: 150,
      status: 'Assigned',
    },
    {
      accountNo: '*********3223',
      customerName: 'Downtown Laundry',
      address: '2766 E West St',
      city: 'Los Angeles',
      zip: '90006',
      kw: 110,
      status: 'Assigned',
    },
    {
      accountNo: '*********8669',
      customerName: 'Hobby Town',
      address: '2112 W Main St',
      city: 'Los Angeles',
      zip: '90007',
      kw: 120,
      status: 'Pending',
    },
    {
      accountNo: '*********3311',
      customerName: 'All Day Donuts',
      address: '2330 W Main St',
      city: 'Los Angeles',
      zip: '90007',
      kw: 200,
      status: 'Pending',
    },
    {
      accountNo: '*********3431',
      customerName: 'Mr. Bean Coffee',
      address: '3322 W Main St',
      city: 'Los Angeles',
      zip: '90007',
      kw: 145,
      status: 'Installed',
    },
    {
      accountNo: '*********2323',
      customerName: 'The Shoe Store',
      address: '2891 W Main St',
      city: 'Los Angeles',
      zip: '90007',
      kw: 145,
      status: 'Pending',
    },
    {
      accountNo: '*********7532',
      customerName: 'Startup Firm LLC',
      address: '5452 W Main St',
      city: 'Los Angeles',
      zip: '90007',
      kw: 150,
      status: 'Assigned',
    },
    {
      accountNo: '*********6343',
      customerName: 'Chicken Town',
      address: '5432 E Main St',
      city: 'Los Angeles',
      zip: '90007',
      kw: 175,
      status: 'Assigned',
    },
    {
      accountNo: '*********1207',
      customerName: 'Ann\'s Bakery',
      address: '8901 W Main St',
      city: 'Los Angeles',
      zip: '90007',
      kw: 145,
      status: 'Assigned',
    },
    {
      accountNo: '*********9888',
      customerName: 'Energy Firm LLC',
      address: '4223 W Center St',
      city: 'Los Angeles',
      zip: '90005',
      kw: 150,
      status: 'Pending',
    },
    {
      accountNo: '*********2224',
      customerName: 'Downtown Office Building',
      address: '2322 W Main St',
      city: 'Los Angeles',
      zip: '90007',
      kw: 150,
      status: 'Assigned',
    },
    {
      accountNo: '*********5353',
      customerName: 'Waste Management LLC',
      address: '6656 W Main St',
      city: 'Los Angeles',
      zip: '90007',
      kw: 250,
      status: 'Assigned',
    },
    {
      accountNo: '*********6343',
      customerName: 'Best Burgers in Town',
      address: '4567 W Main St',
      city: 'Los Angeles',
      zip: '90007',
      kw: 75,
      status: 'Pending'
    },
  ];

  REPORT_DATA = [
    { reportName: 'Revenue Report', reportDescription: 'Description of the Revenue Report', excelTemplate: 'Template 1', templateWorksheet: 'Worksheet 1', procedureName: 'Proc-1', lastRun: '10/12/2020 - 12:00 AM EST'},
    { reportName: 'Program Status Report', reportDescription: 'Description of the Program Status Report', excelTemplate: 'Template 2', templateWorksheet: 'Worksheet 1', procedureName: 'Proc-2', lastRun: '10/2/2020 - 12:00 AM EST'},
    { reportName: 'Installation Report', reportDescription: 'Description of the Installation Report', excelTemplate: 'Template 3', templateWorksheet: 'Worksheet 3', procedureName: 'Proc-3', lastRun: '10/13/2020 - 2:00 AM EST'},
    { reportName: 'Work Order Report', reportDescription: 'Description of the Work Order Report', excelTemplate: 'Template 4', templateWorksheet: 'Worksheet 1', procedureName: 'Proc-4', lastRun: '10/1/2020 - 3:00 AM EST'},
    { reportName: 'Crew Install Report', reportDescription: 'Description of the Crew Install Report', excelTemplate: 'Template 5', templateWorksheet: 'Worksheet 2', procedureName: 'Proc-5', lastRun: '10/20/2020 - 12:00 AM EST'},
    { reportName: 'Additional Work Report', reportDescription: 'Description of the Additional Work Report', excelTemplate: 'Template 6', templateWorksheet: 'Worksheet 1', procedureName: 'Proc-6', lastRun: '10/22/2020 - 12:00 AM EST'},
    { reportName: 'Regional Sales Report', reportDescription: 'Description of the Regional Sales Report', excelTemplate: 'Template 7', templateWorksheet: 'Worksheet 2', procedureName: 'Proc-7', lastRun: '10/12/2020 - 12:00 AM EST'},
    { reportName: 'Program Manager Report', reportDescription: 'Description of the Program Manager Report', excelTemplate: 'Template 8', templateWorksheet: 'Worksheet 1', procedureName: 'Proc-8', lastRun: '10/20/2020 - 12:00 AM EST'},
  ];
  ORG_USER_DATA = [
    { userLogin: 'John-1', userName: 'John Smith', userRole: 'CBO', userPhone: '(555) 555-1212', userEmail: 'john@acmeelectric.com', lastAccessed: '10/12/2020 - 12:00 AM EST' },
    { userLogin: 'Edward-1', userName: 'Edward Watts', userRole: 'ESR', userPhone: '(555) 555-1212', userEmail: 'ed@acmeelectric.com', lastAccessed: '10/2/2020 - 12:00 AM EST' },
    { userLogin: 'Jane-1', userName: 'Jane Smith', userRole: 'ESR', userPhone: '(555) 555-1212', userEmail: 'jane@acmeelectric.com', lastAccessed: '10/13/2020 - 2:00 AM EST' },
    { userLogin: 'Shelly-1', userName: 'Shelly Jones', userRole: 'ESR', userPhone: '(555) 555-1212', userEmail: 'shelly@acmeelectric.com', lastAccessed: '10/1/2020 - 3:00 AM EST' },
    { userLogin: 'Fred-1', userName: 'Fred Fredricks', userRole: 'Installer', userPhone: '(555) 555-1212', userEmail: 'fred@acmeelectric.com', lastAccessed: '10/20/2020 - 12:00 AM EST' },
    { userLogin: 'Johnny-1', userName: 'Johnny Frisco', userRole: 'Installer', userPhone: '(555) 555-1212', userEmail: 'johnny@acmeelectric.com', lastAccessed: '10/22/2020 - 12:00 AM EST' },
    { userLogin: 'Sue-1', userName: 'Sue Susan', userRole: 'Inspector', userPhone: '(555) 555-1212', userEmail: 'sue@acmeelectric.com', lastAccessed: '10/12/2020 - 12:00 AM EST' },
    { userLogin: 'Zaiohai-1', userName: 'Zaiohai Sun', userRole: 'Inspector', userPhone: '(555) 555-1212', userEmail: 'zsun@acmeelectric.com', lastAccessed: '10/20/2020 - 12:00 AM EST' },
  ];
  displayedColumns = [
    { field: 'accountNo', sortable: true, filter: true },
    { field: 'customerName', sortable: true, filter: true },
    { field: 'address', sortable: true, filter: true },
    { field: 'city', sortable: true, filter: true },
    { field: 'zip', sortable: true, filter: true },
    { field: 'kw', sortable: true, filter: true },
    { field: 'status', sortable: true, filter: true }
  ];
  reportsColumns = [
    { field: 'reportName', sortable: true, filter: true },
    { field: 'reportDescription', sortable: true, filter: true },
    { field: 'excelTemplate', sortable: true, filter: true },
    { field: 'templateWorksheet', sortable: true, filter: true },
    { field: 'procedureName', sortable: true, filter: true },
    { field: 'lastRun', sortable: true, filter: true },
  ];
  orgUserColumns = [
    { field: 'userLogin', sortable: true, filter: true },
    { field: 'userName', sortable: true, filter: true },
    { field: 'userRole', sortable: true, filter: true },
    { field: 'userPhone', sortable: true, filter: true },
    { field: 'userEmail', sortable: true, filter: true },
    { field: 'lastAccessed', sortable: true, filter: true },
  ];
  timeTrackColumns = [
    { field: 'userLogin', sortable: true, filter: true },
    { field: 'userName', sortable: true, filter: true },
    { field: 'userRole', sortable: true, filter: true },
    { field: 'userPhone', sortable: true, filter: true },
    { field: 'userEmail', sortable: true, filter: true },
    { field: 'lastAccessed', sortable: true, filter: true },
  ];

  elipsesData = {
    actionIcon: 'ellipsis-v',
    actions: [
      {
        label: 'Update',
        iconName: 'edit',
        click: (event) => {
          console.log('update event', event);
          window.alert('Update Clicked');
        }
      },
      {
        label: 'Preview',
        iconName: 'file-pdf',
        click: (event) => {
          console.log('Preview event', event);
          window.alert('Preview Clicked');
        }
      },
      {
        label: 'Generate',
        iconName: 'calendar',
        click: (event) => {
          console.log('generate event', event);
          window.alert('Generate Clicked');
        }
      }
    ]
  };

  pdfSource =  '';
  videoSource = '';
  imageSource = '';
  
  ngOnInit(): void {
  }

  onFirstDataRendered(params) {
    params.api.sizeColumnsToFit();
  }

  onTextareaContentChange(value): void {
    console.log('textarea content: ', value);
  }

  onTabClick(id): void {
    this.tabItems.map(item => {
      item.isActive = item.id === id ? true : false;
    });
  }

  onActionClick(id): void {
    this.actionItems.map(item => {
      item.isActive = item.id === id ? true : false;
    });
  }

  onAccountClick(id): void {
    this.accountItems.map(item => {
      item.isActive = item.id === id ? true : false;
    });
  }

  onProjectClick(id): void {
    this.projectItems.map(item => {
      item.isActive = item.id === id ? true : false;
    });
  }

  onWorkOrderClick(id): void {
    this.workOrderItems.map(item => {
      item.isActive = item.id === id ? true : false;
    });
  }

  onClickPdfDisplay(source): void {
    this.videoSource = '';
    this.imageSource = '';

    this.pdfSource = source; 
    this.showModal = true; 
  }

  onClickPlayVideo(item): void {
    this.pdfSource = '';
    this.imageSource = '';

    this.videoSource = item;
    this.showModal = true;
  }

  onClickImageDisplay(item): void {
    this.pdfSource = '';
    this.videoSource = '';

    this.imageSource = item;
    this.showModal = true;
  }
}
