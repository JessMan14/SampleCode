import { Component, OnInit } from '@angular/core';
import * as am4core from '@amcharts/amcharts4/core';
import { ItemRenderComponent } from '../item-render/item-render.component';
import am4geodata_usaLow from  '@amcharts/amcharts4-geodata/usaLow';

@Component({
  selector: 'app-program-dashboard',
  templateUrl: './program-dashboard.component.html',
  styleUrls: ['./program-dashboard.component.scss']
})
export class ProgramDashboardComponent implements OnInit {

  constructor() { }

   series = [{
    type: 'PieSeries',
    dataFields: {
      value: 'litres',
      category: 'country'
    }
  }];

    // Line charts config
  yellowLineColor = '#F2A900';
  lineDiv1 = 'lineDiv1';
  lineConfig1 = this.getSingleLineConfig(this.yellowLineColor);

  purpleLineColor = '#0077c8';
  lineDiv2 = 'lineDiv2';
  lineConfig2 = this.getSingleLineConfig(this.purpleLineColor);
  // radius charts config
  radarText1 = '[font-size: 30px]$234M\n [font-size: 15px]Total Sales';
  radarDivId1 = 'radarDivId1';
  radarColor1 = '#0077c8';
  radarConfig1 = this.getRadarConfig(this.radarColor1, this.radarText1, 80);

  radarText2 = '[font-size: 30px]$123M\n [font-size: 15px]Total Sales';
  radarDivId2 = 'radarDivId2';
  radarColor2 = '#26c1c9';
  radarConfig2 = this.getRadarConfig(this.radarColor2, this.radarText2, 40);

  // Internal Reports
  CUSTOMER_DATA = [
    {
      reportId: 1,
      reportName: 'Billing Report',
      reportDesc: 'Engergy usage of City of Anaheim',
      reportPurpose: 'Total Cost Evaluation',
      frequency: '1',
      reportOwner: 'Peter Brown',
      dateRan: '9/01/2020',
    },
    {
      reportId: 2,
      reportName: 'LADWP Pipeline by Council District',
      reportDesc: 'Names of Council Members',
      reportPurpose: 'Prospects',
      frequency: '4',
      reportOwner: 'Peter Brown',
      dateRan: '10/01/2020',
    },
    {
      reportId: 3,
      reportName: 'LADWP_MeasureDetail ',
      reportDesc: 'SBDI Measure Detail',
      reportPurpose: 'Details of measures',
      frequency: '12',
      reportOwner: 'Peter Brown',
      dateRan: '11/01/2020',
    }
  ];
  displayedColumns = [
    {
      field: 'status',
      headerName: 'STATUS',
      width: 120,
      cellRenderer: 'IconRender',
      cellRendererParams: {
        clicked(field: any): void {
          console.log('Here');
        }
      }
    },
    {
      field: 'reportName',
      width: 300,
      headerName: 'REPORT NAME',
      sortable: true,
      filter: true
    },
    {
      field: 'reportDesc',
      width: 400,
      headerName: 'REPORT DESCRIPTION',
      sortable: true,
      filter: true,
      // cellStyle: {color: '#0093ee'}
    },
    {
      field: 'reportPurpose',
      width: 300,
      headerName: 'REPORT PURPOSE',
      sortable: true,
      filter: true
    },
    {
      field: 'frequency',
      headerName: 'FREQUENCY',
      sortable: true,
      filter: true
    },
    {
      field: 'reportOwner',
      headerName: 'REPORT OWNER',
      sortable: true,
      filter: true
    },
    {
      field: 'dateRan',
      headerName: 'DATE RAN',
      sortable: true,
      filter: true
    },
  ];

  frameworkComponents = {
    IconRender: ItemRenderComponent
  };

  elipsesData = {
    actionIcon: 'ellipsis-v',
    actions: [
      {
        label: 'Run',
        iconName: 'running',
        click: (event) => {
          console.log('update event', event);
          window.alert('Update Clicked');
        }
      },
      {
        label: 'History',
        iconName: 'history',
        click: (event) => {
          console.log('Preview event', event);
          window.alert('Preview Clicked');
        }
      },
      {
        label: 'Definition',
        iconName: 'spell-check',
        click: (event) => {
          console.log('generate event', event);
          window.alert('Generate Clicked');
        }
      }
    ]
  };

  // side bar
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

  // icon
  menuData = [
    {
      menuId: 1,
      parentId: null,
      description: '',
      menuIcon: 'fas-chart-bar',
      iconColor: 'white',
      backgroundColor: 'purple',
      page: {
        component: null,
        route: null,
      },
    },
    {
      menuId: 2,
      parentId: null,
      description: '',
      menuIcon: 'fas-comments-alt',
      iconColor: 'white',
      backgroundColor: 'accent-3-60',
      page: {
        component: null,
        route: null,
      },
    },
    {
      menuId: 3,
      parentId: null,
      description: '',
      menuIcon: 'fas-id-badge',
      iconColor: 'white',
      backgroundColor: 'accent-2-60',
      page: {
        component: null,
        route: null,
      },
    },
    {
      menuId: 4,
      parentId: null,
      description: '',
      menuIcon: 'fas-question-circle',
      iconColor: 'white',
      backgroundColor: 'secondary-100',
      page: {
        component: null,
        route: null,
      },
    }
  ];

  // Action panel
  segments = ['Revenue', 'Sales', 'Operations', 'KPI Analysis', 'Table Builder'];
  kpiSelectors = ['Work Order', 'Additional Revenue', 'Copay', 'Incentive', 'Total Sales', 'Savings kWh',
    'Net Revenue', 'Close Rate', 'Average Project Price'];
  regions = ['East', 'Mid-West', 'Northeast', 'Southeast', 'West'];
  utilities = ['Coned', 'PSE', 'PGE', 'SDGE'];
  timePeriods = ['All', 'Current Week', 'Last 4 Weeks', 'Last Month', 'Last Quarter', 'Last Year', 'Month to Date'];

  // pie charts config
  pieDiv1 = 'piechartdiv1';
  pieDiv2 = 'piechartdiv2';

  pieChartConfig1 = {
    data: [{
      'category': '[font-size: 10px]Social\nnetworks',
      'value': Math.floor(Math.random() * (1300 - 50 + 1) + 50),
      'color': am4core.color('#0077c8')
      }, {
      'category': '[font-size: 10px]Following\na link',
      'value': Math.floor(Math.random() * (1300 - 50 + 1) + 50),
      'color': am4core.color('#f9cc11')
      }, {
      'category': '[font-size: 10px]From\nSearch',
       'value': Math.floor(Math.random() * (1300 - 50 + 1) + 50),
       'color': am4core.color('#83c343')
      }, {
      'category': '[font-size: 10px]From\nbookmarks',
      'value': Math.floor(Math.random() * (1300 - 50 + 1) + 50),
      'color': am4core.color('#2fb5c4')
    }],
    innerRadius: am4core.percent(60),
    chartContainer: {
      children: [{
        type: 'Container',
        children: [{
          type: 'Label',
          //fontFamily: 'Heebo',
          text: '[font-size: 30px]154678\n [font-size: 15px]Visits',
          horizontalCenter: 'middle',
          verticalCenter: 'middle',
          fontSize: 40
        }]
      }]
    },
    legend: {
      type: 'Legend',
      position: 'bottom',
      contentAlign: 'center',
      valueLabels: {
        template : {
          text: '{category.category}'
        }
      }
    },
    series: [{
      type: 'PieSeries',
      dataFields: {
      value: 'value',
        category: 'category'
      },
      labels: {
        template: {
          disabled: true
        }
      },
      ticks: {
        template: {
          tooltipText: '',
          propertyFields: {
          fill: 'color'
        }
      }
    },
      slices: {
        template: {
          propertyFields: {
            fill: 'color'
          }
        }
      },
    }],
  };

  pieChartConfig2 = {
    data: [{
      'category': '[font-size: 10px]Social\nnetworks',
      'value': Math.floor(Math.random() * (1300 - 50 + 1) + 50),
      'color': am4core.color('#0077c8')
      }, {
      'category': '[font-size: 10px]Following\na link',
      'value': Math.floor(Math.random() * (1300 - 50 + 1) + 50),
      'color': am4core.color('#f9cc11')
    }, {
      'category': '[font-size: 10px]From\nSearch',
      'value': Math.floor(Math.random() * (1300 - 50 + 1) + 50),
      'color': am4core.color('#83c343')
    }, {
      'category': '[font-size: 10px]From\nbookmarks',
      'value': Math.floor(Math.random() * (1300 - 50 + 1) + 50),
      'color': am4core.color('#2fb5c4')
    }],
    innerRadius: am4core.percent(60),
    chartContainer: {
      children: [{
        type: 'Container',
        children: [{
          type: 'Label',
          //fontFamily: 'Heebo',
          text: '[font-size: 30px]$154K\n [font-size: 15px]Total Sales',
          horizontalCenter: 'middle',
          verticalCenter: 'middle',
          fontSize: 40
        }]
      }]
    },
    series: [{
      type: 'PieSeries',
      dataFields: {
          value: 'value',
          category: 'category'
      },
      labels: {
        template: {
          disabled: true
        }
      },
      ticks: {
        template: {
          tooltipText: '',
          propertyFields: {
            fill: 'color'
          }
        }
      },
      slices: {
        template: {
          propertyFields: {
            fill: 'color'
          }
        }
      },
    }],
  };

  // Gauge charts config
  gaugeDivId1 = 'gaugeDivId1';
  gaugeDivId2 = 'gaugeDivId2';
  gaugeChartConfig1 = {
    innerRadius: -35,
    //fontFamily: 'heboo',

    xAxes: [{
      type: 'ValueAxis',
      min: 0,
      max: 100,
      strictMinMax: true,
      fontSize: 10,
      renderer: {
        grid: {
          template: {
            stroke: new am4core.InterfaceColorSet().getFor('background'),
            strokeOpacity: 0.3,
          }
        },
        inside: true,
        labels: {
          template: {
            radius: 60,
          }
        }
      },

      axisRanges: [{
        value: 0,
        endValue: 28,
        axisFill: {
          fillOpacity: 1,
          fill: am4core.color('#67b7dc'),
          zIndex: -1,
        }
      }, {
        value: 28,
        endValue: 100,
        axisFill: {
          fillOpacity: 1,
          fill: am4core.color('#6771dc'),
          zIndex: -1,
        }
      }],


    }],
    radarContainer: {
      children: [{
        type: 'Container',
        children: [{
          type: 'Label',
          fontFamily: 'Heebo',
          isMeasured: false,
          text: '28%',
          horizontalCenter: 'middle',
          verticalCenter: 'bottom',
          fontSize: 25,
        }]
      }]
    },

    hands: [{
      type: 'ClockHand',
      innerRadius: am4core.percent(35),
      startWidth: 10,
      pin: {
        disabled: true,
      },
      value: 28,
    }]
  };

  gaugeChartConfig2 = {
    innerRadius: -35,
    //fontFamily: 'heboo',

    xAxes: [{
      type: 'ValueAxis',
      min: 0,
      max: 100,
      strictMinMax: true,
      fontSize: 10,
      renderer: {
        grid: {
          template: {
            stroke: new am4core.InterfaceColorSet().getFor('background'),
            strokeOpacity: 0,
          }
        },
        inside: true,
        labels: {
          template: {
            radius: 60,
            bent: true,
          }
        }
      },

      axisRanges: [{
        value: 0,
        endValue: 28,
        axisFill: {
          fillOpacity: 1,
          fill: am4core.color('#fdae19'),
          zIndex: -1,
        },
        label: {
          inside: true,
          text: 'Foundational',
          location: 0.5,
          radius: am4core.percent(10),
          paddingBottom: -5,
          fontSize: 10,
        },
      }, {
        value: 28,
        endValue: 48,
        axisFill: {
          fillOpacity: 1,
          fill: am4core.color('#f3eb0c'),
          zIndex: -1,
        },
        label: {
          inside: true,
          text: 'Developing',
          location: 0.5,
          radius: am4core.percent(10),
          paddingBottom: -5,
          fontSize: 10,
        }
      },{
        value: 48,
        endValue: 68,
        axisFill: {
          fillOpacity: 1,
          fill: am4core.color('#b0d136'),
          zIndex: -1,
        },
        label: {
          inside: true,
          text: 'Maturing',
          location: 0.5,
          radius: am4core.percent(10),
          paddingBottom: -5,
          fontSize: 10,
        }
      },  {
        value: 68,
        endValue: 100,
        axisFill: {
          fillOpacity: 1,
          fill: am4core.color('#0f9747'),
          zIndex: -1,
        },
        label: {
          inside: true,
          text: 'High Performing',
          location: 0.5,
          radius: am4core.percent(10),
          paddingBottom: -5,
          fontSize: 10,
        }
      }],


    }],
    radarContainer: {
      children: [{
        type: 'Container',
        children: [{
          type: 'Label',
          isMeasured: false,
          text: '70',
          horizontalCenter: 'middle',
          verticalCenter: 'bottom',
          fontSize: 20,
          paddingBottom: 15,
          fill: am4core.color('#0f9747')
        }, {
          type: 'Label',
          isMeasured: false,
          text: 'High Performance',
          horizontalCenter: 'middle',
          verticalCenter: 'bottom',
          fontSize: 10,
          fill: am4core.color('#0f9747')
        }],
      }]
    },

    hands: [{
      type: 'ClockHand',
      innerRadius: am4core.percent(35),
      startWidth: 10,
      pin: {
        disabled: true,
      },
      value: 70,
    }]
  };

  // bar charts config
  barChartDivId1 = 'barChartDivId1';
  barChartConfig1 = {
    data: [{
      date: new Date(2020,3, 10),
      value: Math.floor(Math.random() * (500 - 200 + 1) + 200),
    }, {
      date: new Date(2020,3, 11),
      value: Math.floor(Math.random() * (500 - 200 + 1) + 200),
    }, {
      date: new Date(2020,3, 12),
      value: Math.floor(Math.random() * (500 - 200 + 1) + 200),
    }, {
      date: new Date(2020,3, 13),
      value: Math.floor(Math.random() * (500 - 200 + 1) + 200),
    }, {
      date: new Date(2020,3, 14),
      value: Math.floor(Math.random() * (500 - 200 + 1) + 200),
    }, {
      date: new Date(2020,3, 15),
      value: Math.floor(Math.random() * (500 - 200 + 1) + 200),
    }, {
      date: new Date(2020,3, 16),
      value: Math.floor(Math.random() * (500 - 200 + 1) + 200),
    }, {
      date: new Date(2020,3, 17),
      value: Math.floor(Math.random() * (500 - 200 + 1) + 200),
    }, {
      date: new Date(2020,3, 18),
      value: Math.floor(Math.random() * (500 - 200 + 1) + 200),
    }],
    xAxes: [{
      type: 'DateAxis',
      dataFields: {
        category: 'date'
      },
      renderer: {
        labels: {
          template: {
            disabled: true
          }
        },
        grid: {
          template: {
            strokeWidth: 0
          }
        }
      }
    }],
    yAxes: [{
      type: 'ValueAxis',
      renderer: {
        labels: {
          template: {
            disabled: true
          }
        },
        grid: {
          template: {
            strokeWidth: 0
          }
        }
      },
    }],
    series: [{
      type: 'ColumnSeries',
      dataFields: {
        valueY: 'value',
        dateX: 'date'
      },
      name: 'Values',
      columns: {
        tooltipText: 'Series: {name}\nDate: {dateX}\nValue: {valueY}',
        template: {
          width: am4core.percent(40)
        }
      },
      fill: am4core.color('#26c1c9'),
    }],
  };

  barChartDivId2 = 'barChartDivId2';
  barChartConfig2 = {
    data: [{
      date: new Date(2020,0, 1),
      audits: Math.floor(Math.random() * (4000 - 1000 + 1) + 1000),
      installations : Math.floor(Math.random() * (3000 - 2000 + 1) + 2000),
      revenue: Math.floor(Math.random() * (10000 - 3000 + 1) + 3000),
    }, {
      date: new Date(2020,1, 1),
      audits: Math.floor(Math.random() * (4000 - 1000 + 1) + 1000),
      installations: Math.floor(Math.random() * (3000 - 2000 + 1) + 2000),
      revenue: Math.floor(Math.random() * (10000 - 3000 + 1) + 3000),
    }, {
      date: new Date(2020,2, 1),
      audits: Math.floor(Math.random() * (4000 - 1000 + 1) + 1000),
      installations: Math.floor(Math.random() * (3000 - 2000 + 1) + 2000),
      revenue: Math.floor(Math.random() * (10000 - 3000 + 1) + 3000),
    }, {
      date: new Date(2020,3, 1),
      audits: Math.floor(Math.random() * (4000 - 1000 + 1) + 1000),
      installations: Math.floor(Math.random() * (3000 - 2000 + 1) + 2000),
      revenue: Math.floor(Math.random() * (10000 - 3000 + 1) + 3000),
    }, {
      date: new Date(2020,4, 1),
      audits: Math.floor(Math.random() * (4000 - 1000 + 1) + 1000),
      installations: Math.floor(Math.random() * (3000 - 2000 + 1) + 2000),
      revenue: Math.floor(Math.random() * (10000 - 3000 + 1) + 3000),
    }, {
      date: new Date(2020,5, 1),
      audits: Math.floor(Math.random() * (4000 - 1000 + 1) + 1000),
      installations: Math.floor(Math.random() * (3000 - 2000 + 1) + 2000),
      revenue: Math.floor(Math.random() * (10000 - 3000 + 1) + 3000),
    }, {
      date: new Date(2020,6, 1),
      audits: Math.floor(Math.random() * (4000 - 1000 + 1) + 1000),
      installations: Math.floor(Math.random() * (3000 - 2000 + 1) + 2000),
      revenue: Math.floor(Math.random() * (10000 - 3000 + 1) + 3000),
    }, {
      date: new Date(2020,7, 1),
      audits: Math.floor(Math.random() * (4000 - 1000 + 1) + 1000),
      installations: Math.floor(Math.random() * (3000 - 2000 + 1) + 2000),
      revenue: Math.floor(Math.random() * (4000 - 1000 + 1) + 1000),
    }, {
      date: new Date(2020,8, 1),
      audits: Math.floor(Math.random() * (4000 - 1000 + 1) + 1000),
      installations: Math.floor(Math.random() * (3000 - 2000 + 1) + 2000),
      revenue: Math.floor(Math.random() * (10000 - 3000 + 1) + 3000),
    }, {
      date: new Date(2020,9, 1),
      audits: Math.floor(Math.random() * (4000 - 1000 + 1) + 1000),
      installations: Math.floor(Math.random() * (3000 - 2000 + 1) + 2000),
      revenue: Math.floor(Math.random() * (10000 - 3000 + 1) + 3000),
    }, {
      date: new Date(2020,10, 1),
      audits: Math.floor(Math.random() * (4000 - 1000 + 1) + 1000),
      installations: Math.floor(Math.random() * (3000 - 2000 + 1) + 2000),
      revenue: Math.floor(Math.random() * (10000 - 3000 + 1) + 3000),
    }, {
      date: new Date(2020,11, 1),
      audits: Math.floor(Math.random() * (4000 - 1000 + 1) + 1000),
      installations: Math.floor(Math.random() * (3000 - 2000 + 1) + 2000),
      revenue: Math.floor(Math.random() * (10000 - 3000 + 1) + 3000),
    }],
    //fontFamily: 'heboo',
    numberFormatter: {
      numberFormat: '#a',
      bigNumberPrefixes: [{
        number: 1e+3,
        suffix: 'K',
      }],
    },
    xAxes: [{
      type: 'DateAxis',
      dateFormats: {
        month: "MMM"
      },
      periodChangeDateFormats: {
        month: "MMM"
      },
      dataFields: {
        category: 'date'
      },
      renderer: {
        labels: {
          template: {
            disabled: false
          }
        },
        grid: {
          template: {
            strokeWidth: 1,
            location: 0,
          }
        },
        minGridDistance: 20,
      }
    }],
    yAxes: [{
      type: 'ValueAxis',
      renderer: {
        labels: {
          template: {
            disabled: false
          }
        },
        grid: {
          template: {
            strokeWidth: 1
          }
        }
      },
    }],
    series: [{
      type: 'ColumnSeries',
      dataFields: {
        valueY: 'audits',
        dateX: 'date'
      },
      name: 'Audits',
      stacked: true,
      columns: {
        //tooltipText: 'Series: {name}\nDate: {dateX}\Sales: {valueY}',
        template: {
          width: am4core.percent(60)
        }
      },
      fill: am4core.color('#2cbfc8'),
      legendSettings: {
        labelText: '[font-size: 25px]2,385\n [font-size: 15px][grey]{name}', //'[bold]{name}[/]',
        valueText: '{valueY}',
      },
    }, {
      type: 'ColumnSeries',
      dataFields: {
        valueY: 'installations',
        dateX: 'date'
      },
      name: 'Installations',
      stacked: true,
      columns: {
        //tooltipText: 'Series: {name}\nDate: {dateX}\Sales: {valueY}',
        template: {
          width: am4core.percent(60)
        }
      },
      fill: am4core.color('#83c341'),
      legendSettings: {
        labelText: '[font-size: 25px]3,015\n [font-size: 15px][grey]{name}', //'[bold]{name}[/]',
        valueText: '{valueY}',
      }
    }, {
      type: 'LineSeries',
      strokeWidth: 3,
      stroke:  am4core.color('#fbdb67'),
      //tensionX: 0.3,
      dataFields: {
        valueY: 'revenue',
        dateX: 'date'
      },
      name: 'Revenue',
      legendSettings: {
        labelText: '[font-size: 25px]$185,415\n [font-size: 15px][grey]{name}', //'[bold]{name}[/]',
        valueText: '{valueY}',
      }
    }],
    legend: {
      position: 'top',
      //useDefaultMarker: true,
      markers: {
        children: [{
          // hide regular marker
          disabled: true
        },
      ],
      }
    },
  };

  cities = [{
    "latitude": 32.715,
    "longitude": -117.1625,
    "title": "San Diego\nSDG&E",
    "installed": 200,
    "id": "US-CA"
  },{
    "latitude": 34.0522,
    "longitude": -118.2437,
    "title": "Los Angeles\nSoCalGas",
    "installed": 200,
    "id": "US-CA"
  }, {
    "latitude": 40.712775,
    "longitude": -74.005973,
    "title": "New York\nConEdison",
    "installed": 200,
    "id": "US-NY"
  },{
    "latitude": 40.7947,
    "longitude": -74.2162569,
    "title": "New Jersey\nClean Energy Program",
    "installed": 200,
    "id": "US-NY"
  },{
    "latitude": 39.6006523,
    "longitude": -77.8205508,
    "title": "Maryland\nPotomac Edison",
    "installed": 200,
    "id": "US-MA"
  },{
    "latitude": 37.773972,
    "longitude": -122.431297,
    "title": "San Francisco\nPG&E",
    "installed": 200,
    "id": "US-CA"
  },{
    "latitude": 40.758701,
    "longitude": -111.876183,
    "title": "Salt Lake City\nRocky Mountain Power",
    "installed": 200,
    "id": "UT-CA"
  },{
    "latitude": 42.8401364,
    "longitude": -106.32184,
    "title": "Casper\nRocky Mountain Power",
    "installed": 200,
    "id": "US-WY"
  },{
    "latitude": 39.7621623,
    "longitude": -104.87607,
    "title": "Denver\nRocky Mountain Power",
    "installed": 200,
    "id": "US-CO"
  },{
    "latitude": 41.8368082,
    "longitude": -87.684548,
    "title": "Chicago\nComEd",
    "installed": 200,
    "id": "US-IL"
  },{
    "latitude": 47.596573,
    "longitude": -122.15358,
    "title": "Bellevue\nPuget Sound Energy",
    "installed": 200,
    "id": "US-WA"
  }];

  primaryGreenColor = '#83c341';
  countryColor = "#3b3b3b";
  countryStrokeColor = "#000000";
  countryHoverColor = "#1b1b1b";
  activeCountryColor = "#0f0f0f";

  // map charts config
  heatMapDivId = 'heatMapDivId';
  heatMapConfig = {
    geodata: am4geodata_usaLow,
    projection: 'AlbersUsa',
    children: [{
      series: 's1',
      type: 'HeatLegend',
      forceCreate: true,
      id: 'heatLegend',
      align: 'right',
      valign: 'bottom',
      width: am4core.percent(20),
      marginRight: am4core.percent(4),
      minValue: 0,
      maxValue: 40000000,
      valueAxis: {
        axisRanges: [{
          value: 0,
          label: {
            text: 'min saving',
            fontSize: 10,
          }
        }, {
          value: 40000000,
          label: {
            text: 'max saving',
            fontSize: '10'
          }
        }],
        renderer: {
          labels: {
            adapter: {
              text: function(labelText) {
                return '';
              }
            }
          }
        },
      }
    }],

    series: [{
      id: 's1',
      type: 'MapPolygonSeries',
      property: 'fill',
      heatRules: [{
        target: 'mapPolygons.template',
        property: 'fill',
        min: am4core.color('#25529a').brighten(1),
        max: am4core.color('#25529a').brighten(-0.3)
      }],
      useGeodata: true,
      data: [{
        id: 'US-AL',
        value: Math.floor(Math.random() * (200 - 100 + 1) + 100)
      },
      {
        id: 'US-AK',
        value: Math.floor(Math.random() * (200 - 100 + 1) + 100)
      },
      {
        id: 'US-AZ',
        value: Math.floor(Math.random() * (200 - 100 + 1) + 100)
      },
      {
        id: 'US-AR',
        value: Math.floor(Math.random() * (200 - 100 + 1) + 100)
      },
      {
        id: 'US-CA',
        value: 500
      },
      {
        id: 'US-CO',
        value: 496
      },
      {
        id: 'US-CT',
        value: Math.floor(Math.random() * (200 - 100 + 1) + 100)
      },
      {
        id: 'US-DE',
        value: Math.floor(Math.random() * (200 - 100 + 1) + 100)
      },
      {
        id: 'US-FL',
        value: Math.floor(Math.random() * (200 - 100 + 1) + 100)
      },
      {
        id: 'US-GA',
        value: Math.floor(Math.random() * (200 - 100 + 1) + 100)
      },
      {
        id: 'US-HI',
        value: Math.floor(Math.random() * (200 - 100 + 1) + 100)
      },
      {
        id: 'US-ID',
        value: Math.floor(Math.random() * (200 - 100 + 1) + 100)
      },
      {
        id: 'US-IL',
        value: 494
      },
      {
        id: 'US-IN',
        value: Math.floor(Math.random() * (200 - 100 + 1) + 100)
      },
      {
        id: 'US-IA',
        value: Math.floor(Math.random() * (200 - 100 + 1) + 100)
      },
      {
        id: 'US-KS',
        value: Math.floor(Math.random() * (200 - 100 + 1) + 100)
      },
      {
        id: 'US-KY',
        value: Math.floor(Math.random() * (200 - 100 + 1) + 100)
      },
      {
        id: 'US-LA',
        value: Math.floor(Math.random() * (200 - 100 + 1) + 100)
      },
      {
        id: 'US-ME',
        value: Math.floor(Math.random() * (200 - 100 + 1) + 100)
      },
      {
        id: 'US-MD',
        value: 499
      },
      {
        id: 'US-MA',
        value: Math.floor(Math.random() * (491 - 100 + 1) + 100)
      },
      {
        id: 'US-MI',
        value: Math.floor(Math.random() * (200 - 100 + 1) + 100)
      },
      {
        id: 'US-MN',
        value: Math.floor(Math.random() * (200 - 100 + 1) + 100)
      },
      {
        id: 'US-MS',
        value: Math.floor(Math.random() * (200 - 100 + 1) + 100)
      },
      {
        id: 'US-MO',
        value: Math.floor(Math.random() * (200 - 100 + 1) + 100)
      },
      {
        id: 'US-MT',
        value: Math.floor(Math.random() * (200 - 100 + 1) + 100)
      },
      {
        id: 'US-NE',
        value: Math.floor(Math.random() * (200 - 100 + 1) + 100)
      },
      {
        id: 'US-NV',
        value: Math.floor(Math.random() * (200 - 100 + 1) + 100)
      },
      {
        id: 'US-NH',
        value: Math.floor(Math.random() * (200 - 100 + 1) + 100)
      },
      {
        id: 'US-NJ',
        value: 501
      },
      {
        id: 'US-NM',
        value: Math.floor(Math.random() * (200 - 100 + 1) + 100)
      },
      {
        id: 'US-NY',
        value: 502
      },
      {
        id: 'US-NC',
        value: Math.floor(Math.random() * (200 - 100 + 1) + 100)
      },
      {
        id: 'US-ND',
        value: Math.floor(Math.random() * (200 - 100 + 1) + 100)
      },
      {
        id: 'US-OH',
        value: Math.floor(Math.random() * (200 - 100 + 1) + 100)
      },
      {
        id: 'US-OK',
        value: Math.floor(Math.random() * (200 - 100 + 1) + 100)
      },
      {
        id: 'US-OR',
        value: Math.floor(Math.random() * (200 - 100 + 1) + 100)
      },
      {
        id: 'US-PA',
        value: Math.floor(Math.random() * (200 - 100 + 1) + 100)
      },
      {
        id: 'US-RI',
        value: Math.floor(Math.random() * (200 - 100 + 1) + 100)
      },
      {
        id: 'US-SC',
        value: Math.floor(Math.random() * (200 - 100 + 1) + 100)
      },
      {
        id: 'US-SD',
        value: Math.floor(Math.random() * (200 - 100 + 1) + 100)
      },
      {
        id: 'US-TN',
        value: Math.floor(Math.random() * (200 - 100 + 1) + 100)
      },
      {
        id: 'US-TX',
        value: Math.floor(Math.random() * (200 - 100 + 1) + 100)
      },
      {
        id: 'US-UT',
        value: 498
      },
      {
        id: 'US-VT',
        value: Math.floor(Math.random() * (200 - 100 + 1) + 100)
      },
      {
        id: 'US-VA',
        value: Math.floor(Math.random() * (200 - 100 + 1) + 100)
      },
      {
        id: 'US-WA',
        value: 497
      },
      {
        id: 'US-WV',
        value: Math.floor(Math.random() * (200 - 100 + 1) + 100)
      },
      {
        id: 'US-WI',
        value: Math.floor(Math.random() * (200 - 100 + 1) + 100)
      },
      {
        id: 'US-WY',
        value: Math.floor(Math.random() * (495 - 100 + 1) + 100)
      }],
    },{
      type: 'MapImageSeries',
      data: this.cities,
      mapImages: {
        propertyFields: {
          latitude: 'latitude',
          longitude: 'longitude'
        },
        children: [{
          type: 'Circle',
          radius: 5,
          fill : am4core.color(this.primaryGreenColor),
          stroke: am4core.color("#FFFFFF"),
          strokeWidth: 2,
          nonScaling: true,
          tooltipText: '{title}'
        }]
      }
    }]
  };

  locationMapDivId = 'locationMapDivId';
  locationMapConfig = {
    height: am4core.percent(90),
    zoomControl: {
      align: 'right',
      marginRight: 15,
      valign: 'middle',
    },
    geodata: am4geodata_usaLow,
    projection: 'AlbersUsa',
    panBehavior: 'move',
    deltaLongitude: -10,

    series: [{
      id: 's1',
      type: 'MapPolygonSeries',
      dataFields: {
        id: 'id',
        value: 'confirmedPC',
      },
      interpolationDuration: 0,
      useGeodata: true,
      nonScalingStroke: true,
      strokeWidth: 0.5,
      calculateVisualCenter: true,
      data: this.cities,
      mapPolygons: {
        template: {
          fill: am4core.color(this.countryColor),
          fillOpacity: 1,
          stroke: am4core.color(this.countryStrokeColor),
          strokeOpacity: 0.15,
          setStateOnChildren: true,
          tooltipPosition: 'fixed',
        }
      },
      heatRules: [{
        target: 'mapPolygons.template',
        property: 'fill',
        min: am4core.color(this.countryColor),
        max: am4core.color(this.countryColor)
      }],
      property: 'fill',
    },{
      type: 'MapImageSeries',
      dataFields: {
        value: 'installed',
        id: 'id',
      },
      data: this.cities,
      tooltip: {
        background: {
          stroke: am4core.color(this.primaryGreenColor),
          strokeWidth: 2,
        },
      },
      mapImages: {
        children: [{
          type: 'Circle',
          fill : am4core.color(this.primaryGreenColor),
          hiddenState: {
            properties: {
              scale: 0.0001
            },
            transitionDuration: 2000,
          },
          defaultState: {
            transitionDuration: 2000,
            transitionEasing: am4core.ease.elasticOut,
          },
          applyOnClones: true
        }],
        nonScaling: true,
        strokeOpacity: 0,
        fillOpacity: 0.55,
        tooltipText: '{title}',
        applyOnClones: true,
        propertyFields: {
          latitude: 'latitude',
          longitude: 'longitude'
        },
      },
      heatRules: [{
        target: 'mapImages.template.children[0]',
        property: 'radius',
        min: 3,
        max: 30,
        dataField: 'value'
      }],
    }]
  };

  // area chart
  areaChartDivId = 'areaChartDivId';
  installColor = '#00A9E0';
  projectColor = '#43B02A';
  auditColor = '#F2A900';
  areaChartConfig = this.getAreaChartConfig(this.installColor, this.projectColor, this.auditColor);

  // kWh Pipeline
  kWhPipelineDivId = 'kWhPipelineDivId';
  kWPipelineDivId = 'kWPipelineDivId';
  thermPipelineDivId = 'thermPipelineDivId';
  install_color = '#43B02A';
  installing_color = '#70ad47';
  iou_color = '#a1c490';
  audited_color = '#c3d8bb';
  pipelineNeeded_color = '#ffffff'
  kWhdata =  [{
    pipeline: 'kWh',
    installed: 1,
    installing: 2,
    iouApproved: 4,
    audited: 5,
    pipelineNeeded: 9
  }]
  kWhChartConfig = this.getPipelineChartConfig(this.kWhdata, this.install_color, this.installing_color, this.iou_color, this.audited_color, this.pipelineNeeded_color, 'Millions', 35);

  kWdata =  [{
    pipeline: 'kW',
    installed: 200,
    installing: 200,
    iouApproved: 400,
    audited: 500,
    pipelineNeeded: 2327
  }]
  kWChartConfig = this.getPipelineChartConfig(this.kWdata, this.install_color, this.installing_color, this.iou_color, this.audited_color, this.pipelineNeeded_color, '', 50);

  thermdata =  [{
    pipeline: 'therm',
    installed: 10,
    installing: 20,
    iouApproved: 40,
    audited: 50,
    pipelineNeeded: 51
  }]
  thermChartConfig = this.getPipelineChartConfig(this.thermdata, this.install_color, this.installing_color, this.iou_color, this.audited_color, this.pipelineNeeded_color, 'Thousands', 70);

  ngOnInit(): void {
  }

  getSingleLineConfig(lineColor: string): any {
    let max = 480;
    let min = 200;
    const chartConfig = {
      data: [{
        date: new Date(2020,3, 10),
        value: Math.floor(Math.random() * (max - min + 1) + min),
      }, {
        date: new Date(2020,3, 11),
        value: Math.floor(Math.random() * (max - min + 1) + min),
      }, {
        date: new Date(2020,3, 12),
        value: Math.floor(Math.random() * (max - min + 1) + min),
      }, {
        date: new Date(2020,3, 13),
        value: Math.floor(Math.random() * (max - min + 1) + min),
      }, {
        date: new Date(2020,3, 14),
        value: Math.floor(Math.random() * (max - min + 1) + min),
      }, {
        date: new Date(2020,3, 15),
        value: Math.floor(Math.random() * (max - min + 1) + min),
      }, {
        date: new Date(2020,3, 16),
        value: Math.floor(Math.random() * (max - min + 1) + min),
      }, {
        date: new Date(2020,3, 17),
        value: Math.floor(Math.random() * (max - min + 1) + min),
      }, {
        date: new Date(2020,3, 18),
        value: Math.floor(Math.random() * (max - min + 1) + min),
      }],
      maskBullets: false,
      xAxes: [{
        type: 'DateAxis',
        dataFields: {
          category: 'date'
        },
        renderer: {
          labels: {
            template: {
              disabled: true
            }
          },
          grid: {
            template: {
              strokeWidth: 0
            }
          }
        }
      }],
      yAxes: [{
        type: 'ValueAxis',
        renderer: {
          labels: {
            template: {
              disabled: true
            }
          },
          grid: {
            template: {
              strokeWidth: 0,
              location: 0,
            }
          },
        },
      }],
      series: [{
        type: 'LineSeries',
        dataFields: {
          valueY: 'value',
          dateX: 'date'
        },
        name: 'Sales',
        columns: {
          tooltipText: 'Series: {name}\nDate: {dateX}\nValue: {valueY}',
        },
        minBulletDistance: 10,
        stroke : am4core.color(lineColor),
        strokeWidth: 3,
        bullets: [{
          type: 'CircleBullet',
          //radius: 3,
          fill: am4core.color("#fff"),
          strokeWidth: 3,
        }]
      }],
    };
    return chartConfig;
  }

  getRadarConfig(color: string, text: string, value: number): any {
    const radarChartConfig = {
      data: [{
        category: 'Progress',
        value: value,
        full: 100
      }],
      startAngle: -90,
      endAngle: 270,
      innerRadius: am4core.percent(80),
      rotation: 360,
      radarContainer: {
        children: [{
          type: 'Container',
          children: [{
            type: 'Label',
            text: text,
            horizontalCenter: 'middle',
            verticalCenter: 'middle',
          }]
        }]
      },
      yAxes: [{
        type: 'CategoryAxis', //'AxisRendererRadial',
        dataFields: {
          category: 'category'
        },
        renderer: {
          minGridDistance: 10,
          labels: {
            template: {
              disabled: true
            }
          },
          grid: {
            template: {
              location: 0,
              strokeOpacity: 0
            }
          }
        }
      }],
      xAxes: [{
        type: 'ValueAxis',
        min: 0,
        max: 100,
        strictMinMax: true,
        renderer: {
          minGridDistance: 10,
          labels: {
            template: {
              disabled: true
            }
          },
          grid: {
            template: {
              strokeOpacity: 0
            }
          }
        }
      }],
      series: [{
        type: 'RadarColumnSeries',
        clustered: false,
        dataFields: {
          valueX: 'full',
          categoryY: 'category'
        },
        columns: {
          template: {
            fill: am4core.color('#FFFFFF'),
            fillOpacity: 0.08,
            radarColumn: {
              cornerRadius: 20
            }
          },
        },
      }, {
        type: 'RadarColumnSeries',
        clustered: false,
        dataFields: {
          valueX: 'value',
          categoryY: 'category'
        },
        columns: {
          template: {
            strokeWidth: 0,
            tooltipText: '{category}: [bold]{value}[/]',
            radarColumn: {
              cornerRadius: 20
            },
            fill: am4core.color(color),
          },
        },
      }],
    };
    return radarChartConfig;
  }

  getAreaChartConfig(color1: string, color2: string, color3: string): any {
      let max = 5000;
      let min = 1000;

      const areaChartConfig = {
        data: [{
        month: new Date(2020, 0, 1),
        installs: Math.floor(Math.random() * (max - min + 1) + min),
        projects: Math.floor(Math.random() * (max - min + 1) + min),
        audits: Math.floor(Math.random() * (max - min + 1) + min)
      }, {
        month: new Date(2020, 1, 1),
        installs: Math.floor(Math.random() * (max - min + 1) + min),
        projects: Math.floor(Math.random() * (max - min + 1) + min),
        audits: Math.floor(Math.random() * (max - min + 1) + min)
      }, {
        month: new Date(2020, 2, 1),
        installs: Math.floor(Math.random() * (max - min + 1) + min),
        projects: Math.floor(Math.random() * (max - min + 1) + min),
        audits: Math.floor(Math.random() * (max - min + 1) + min)
      }, {
        month: new Date(2020, 3, 1),
        installs: Math.floor(Math.random() * (max - min + 1) + min),
        projects: Math.floor(Math.random() * (max - min + 1) + min),
        audits: Math.floor(Math.random() * (max - min + 1) + min)
      }, {
        month: new Date(2020, 4, 1),
        installs: Math.floor(Math.random() * (max - min + 1) + min),
        projects: Math.floor(Math.random() * (max - min + 1) + min),
        audits: Math.floor(Math.random() * (max - min + 1) + min)
      }, {
        month: new Date(2020, 5, 1),
        installs: Math.floor(Math.random() * (max - min + 1) + min),
        projects: Math.floor(Math.random() * (max - min + 1) + min),
        audits: Math.floor(Math.random() * (max - min + 1) + min)
      }, {
        month: new Date(2020, 6, 1),
        installs: Math.floor(Math.random() * (max - min + 1) + min),
        projects: Math.floor(Math.random() * (max - min + 1) + min),
        audits: Math.floor(Math.random() * (max - min + 1) + min)
      }, {
        month: new Date(2020, 7, 1),
        installs: Math.floor(Math.random() * (max - min + 1) + min),
        projects: Math.floor(Math.random() * (max - min + 1) + min),
        audits: Math.floor(Math.random() * (max - min + 1) + min)
      }, {
        month: new Date(2020, 8, 1),
        installs: Math.floor(Math.random() * (max - min + 1) + min),
        projects: Math.floor(Math.random() * (max - min + 1) + min),
        audits: Math.floor(Math.random() * (max - min + 1) + min)
      }, {
        month: new Date(2020, 9, 1),
        installs: Math.floor(Math.random() * (max - min + 1) + min),
        projects: Math.floor(Math.random() * (max - min + 1) + min),
        audits: Math.floor(Math.random() * (max - min + 1) + min)
      }, {
        month: new Date(2020, 10, 1),
        installs: Math.floor(Math.random() * (max - min + 1) + min),
        projects: Math.floor(Math.random() * (max - min + 1) + min),
        audits: Math.floor(Math.random() * (max - min + 1) + min)
      }, {
        month: new Date(2020, 11, 1),
        installs: Math.floor(Math.random() * (max - min + 1) + min),
        projects: Math.floor(Math.random() * (max - min + 1) + min),
        audits: Math.floor(Math.random() * (max - min + 1) + min)
      }],

      xAxes: [{
        type: 'DateAxis',
        dateFormats: {
          month: "MMM"
        },
        periodChangeDateFormats: {
          month: "MMM"
        },
        renderer: {
          minGridDistance: 50,
        },
        startLocation: 0.5,
        endLocation: 0.5,
        baseInterval: {
          timeUnit: 'month',
          count: 1
        }, axisRanges: [{
          grid: {
            stroke: '#dc67ab',
            strokeOpacity: 0.6,
            strokeDasharray: '5,2'
          },
        }]
      }],
      yAxes: [{
        type: 'ValueAxis',
        tooltip: {
          disabled: true
        },
        renderer: {
          labels: {
            template: {
              disabled: true
            }
          },
          grid: {
            template: {
              strokeWidth: 0
            }
          }
        },
      }],

      series: [{
        type: 'LineSeries',
        dataFields: {
          dateX: 'month',
          valueY: 'installs'
        },
        name: 'Installs',
        tooltipHTML: '<i class=\'fas fa-shopping-basket text-black\' style=\'vertical-align:bottom; margin-right: 10px; width:28px; height:21px;\'></i><span style=\'font-size:14px; color:#000000;\'><b>{valueY.value}</b></span>',
        tooltip: {
          background: {
            fill: '#FFF',
            strokeWidth: 3
          },
          getStrokeFromObject: true,
          getFillFromObject: false
        },
        fill: am4core.color(color1),
        fillOpacity: 0.6,
        stroke: am4core.color(color1),
        strokeWidth: 2,
        stacked: true
      }, {
        type: 'LineSeries',
        dataFields: {
          dateX: 'month',
          valueY: 'projects'
        },
        name: 'Projects',
        tooltipHTML: '<i class=\'fas fa-sack-dollar text-black\' style=\'vertical-align:bottom; margin-right: 10px; width:28px; height:21px;\'></i><span style=\'font-size:14px; color:#000000;\'><b>{valueY.value}</b></span>',
        tooltip: {
          background: {
            fill: '#FFF',
            strokeWidth: 3
          },
          getStrokeFromObject: true,
          getFillFromObject: false
        },
        fill: am4core.color(color2),
        fillOpacity: 0.6,
        stroke: am4core.color(color2),
        strokeWidth: 2,
        stacked: true
      }, {
        type: 'LineSeries',
        dataFields: {
          dateX: 'month',
          valueY: 'audits'
        },
        name: 'Audits',
        tooltipHTML: '<i class=\'fas fa-project-diagram text-black\' style=\'vertical-align:bottom; margin-right: 10px; width:28px; height:21px;\'></i><span style=\'font-size:14px; color:#000000;\'><b>{valueY.value}</b></span>',
        tooltip: {
          background: {
            fill: '#FFF',
            strokeWidth: 3
          },
          getStrokeFromObject: true,
          getFillFromObject: false
        },
        fill: am4core.color(color3),
        fillOpacity: 0.6,
        stroke: am4core.color(color3),
        strokeWidth: 2,
        stacked: true
      }],
      cursor: {
        type: 'XYCursor'
      },
      legend: {
        position: 'top',
      },
    };
    return areaChartConfig;
  }

  getPipelineChartConfig(configData: any[],color1: string, color2: string, color3: string, color4: string, color5: string, label: string, distance: number): any {
    let columnWidth = 20;

    const kWhPipelineChartConfig = {
    data: configData,
    chartContainer: {
      children: [{
        type: 'Container',
        children: [{
          type: 'Label',
          text: label,
          align: 'right',
          isMeasured: false,
          x: 350,
          y: 235,
          fontSize: 10
        }]
      }]
    },
    xAxes: [{
      type: 'ValueAxis',
      min: 0,
      fontSize: 10,
      renderer: {
        // grid: {
        //   template: {
        //     opacity : 0,
        //   },
        // },
        ticks: {
          template: {
            strokeOpacity: 0.5,
            stroke: am4core.color('#495C43'),
            length: 10,
          }
        },
        line: {
          strokeOpacity: 0.5
        },
        // baseGrid: {
        //   disabled : true
        // },
        minGridDistance: distance,
      }
    }],
    yAxes: [{
      type: 'CategoryAxis',
      fontSize: 10,
      dataFields: {
        category: 'pipeline'
      },
      renderer: {
        grid: {
          template: {
            opacity: 0
          }
        },
        cellStartLocation: 0.3,
        cellEndLocation: 0.7
      }
    }],
    series: [{
      type: 'ColumnSeries',
      columns: {
        template :{
          width: am4core.percent(columnWidth),
        },
      },
      dataFields: {
        valueX: 'installed',
        categoryY : 'pipeline'
      },
      name: 'Installed',
      fill: am4core.color(color1),
      stroke: am4core.color(color1),
      stacked: true,
      bullets: [{
        type: 'LabelBullet',
        fontSize: 10,
        locationX: 0.5,
        label: {
          text: '{valueX}'
        }
      }]
    }, {
      type: 'ColumnSeries',
      columns: {
        template :{
          width: am4core.percent(columnWidth)
        }
      },
      dataFields: {
        valueX: 'installing',
        categoryY : 'pipeline'
      },
      name: 'Installing',
      fill: am4core.color(color2),
      stroke: am4core.color(color2),
      stacked: true,
      bullets: [{
        type: 'LabelBullet',
        fontSize: 10,
        locationX: 0.5,
        label: {
          text: '{valueX}'
        }
      }]
    }, {
      type: 'ColumnSeries',
      columns: {
        template :{
          width: am4core.percent(columnWidth)
        }
      },
      dataFields: {
        valueX: 'iouApproved',
        categoryY : 'pipeline'
      },
      name: 'IOU Approved',
      fill: am4core.color(color3),
      stroke: am4core.color(color3),
      stacked: true,
      bullets: [{
        type: 'LabelBullet',
        fontSize: 10,
        locationX: 0.5,
        label: {
          text: '{valueX}'
        }
      }]
    }, {
      type: 'ColumnSeries',
      columns: {
        template :{
          width: am4core.percent(columnWidth)
        }
      },
      dataFields: {
        valueX: 'audited',
        categoryY : 'pipeline'
      },
      name: 'Audited',
      fill: am4core.color(color4),
      stroke: am4core.color(color4),
      stacked: true,
      bullets: [{
        type: 'LabelBullet',
        fontSize: 10,
        locationX: 0.5,
        label: {
          text: '{valueX}'
        }
      }]
    }, {
      type: 'ColumnSeries',
      columns: {
        template :{
          width: am4core.percent(columnWidth)
        }
      },
      dataFields: {
        valueX: 'pipelineNeeded',
        categoryY : 'pipeline'
      },
      name: 'Pipelined Needed',
      fill: am4core.color(color5),
      stroke: am4core.color('#000000'),
      stacked: true,
      bullets: [{
        type: 'LabelBullet',
        fontSize: 10,
        locationX: 0.5,
        label: {
          text: '{valueX}'
        }
      }]
    }],
    legend: {
      position: 'bottom',
      fontSize: 9,
      markers: {
        width: 10,
        height: 10
      }
    },
  };
  return kWhPipelineChartConfig;
  }
}
