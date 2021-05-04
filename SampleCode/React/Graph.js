import React, { Component } from 'react'
import { CircleSpinner } from 'react-spinners-kit'

import "./HistoricalData.css";
import * as Constants from '../../../constants/constants.js';
import { getAPIHeaders, stringToDate } from '../../../utilities';

import * as am4core from "@amcharts/amcharts4/core";
import * as am4charts from "@amcharts/amcharts4/charts";
import am4themes_animated from "@amcharts/amcharts4/themes/animated";

import {am4themes_dataViewTheme} from "../../../assets/ChartThemes/DataViewTheme";

am4core.useTheme(am4themes_animated);
am4core.useTheme(am4themes_dataViewTheme);

const axios = require('axios');

export default class Graph extends Component {
    constructor(props) {
        super(props);
        this.state = {
            context: props.context,
            data: null,
            loaded: false
        }
    }
  
    componentDidMount() {
          let data = []; //Month -> Values

          axios.get(Constants.BASE_URL + '/programs/' + this.props.context.program.id + '/customers/' + this.props.context.customer + "/billing/all", { headers: getAPIHeaders() })
          .then (
              response => { 
                  const results = response.data.results;
                  results.forEach(period => {
                    let dateRange = period.serviceDates;
                    let start = stringToDate(dateRange.startDate,'yyyy-mm-dd','-');
                    let end = stringToDate(dateRange.endDate,'yyyy-mm-dd','-');

                    let mapDate = (start.getDay() > 15) ? end : start;
                    mapDate.setDate(1);

                    data.push({
                        date: mapDate,
                        kWh: period.usage,
                        kWatt: period.demand,
                        HistPay: period.balanceCurrent - period.balanceForward
                    });
                  });

                  this.setState({ data: data, chart: this.buildChart(data), loaded: true })
              })
          .catch(
              error => { console.log("ERROR: " + error) }
          );
      }
    

      buildChart = (data) => {
        let chart = am4core.create("historical-chart", am4charts.XYChart);
        chart.colors.step = 2;
        chart.maskBullets = false;
        chart.data = data;

        // Create axes
        let dateAxis = chart.xAxes.push(new am4charts.DateAxis());
        dateAxis.renderer.grid.template.location = 0;
        dateAxis.renderer.minGridDistance = 50;
        dateAxis.renderer.grid.template.disabled = true;
        dateAxis.renderer.fullWidthTooltip = true;

        let kWhAxis = chart.yAxes.push(new am4charts.ValueAxis());
        kWhAxis.title.text = "kWh Usage";

        let kWattAxis = chart.yAxes.push(new am4charts.ValueAxis());
        kWattAxis.title.text = "kW Demand";
        kWattAxis.renderer.opposite = true;
        kWattAxis.syncWithAxis = kWhAxis;

        let HistPayAxis = chart.yAxes.push(new am4charts.ValueAxis());
        HistPayAxis.renderer.grid.template.disabled = true;
        HistPayAxis.renderer.labels.template.disabled = true;
        HistPayAxis.syncWithAxis = kWhAxis;

        // Create kWh Line Series
        let kWhSeries = chart.series.push(new am4charts.LineSeries());
            kWhSeries.id = "kWh";
            kWhSeries.dataFields.valueY = "kWh";
            kWhSeries.dataFields.dateX = "date";
            kWhSeries.yAxis = kWhAxis;
            kWhSeries.tooltipText = "{valueY} kWh Usage";
            kWhSeries.name = "kWh";
            kWhSeries.strokeWidth = 2;
            kWhSeries.propertyFields.strokeDasharray = "dashLength";
            kWhSeries.showOnInit = true;
      
        let kWhBullet = kWhSeries.bullets.push(new am4charts.CircleBullet());
            kWhBullet.circle.strokeWidth = 2;

        // Create kWatt Line Series
        let kWattSeries = chart.series.push(new am4charts.LineSeries());
            kWattSeries.dataFields.valueY = "kWatt";
            kWattSeries.dataFields.dateX = "date";
            kWattSeries.yAxis = kWattAxis;
            kWattSeries.name = "kW";
            kWattSeries.strokeWidth = 2;
            kWattSeries.propertyFields.strokeDasharray = "dashLength";
            kWattSeries.tooltipText = "{valueY} kW Demand";
            kWattSeries.showOnInit = true;

        let kWattBullet = kWattSeries.bullets.push(new am4charts.Bullet());

        let kWattRectangle = kWattBullet.createChild(am4core.Rectangle);
            kWattBullet.horizontalCenter = "middle";
            kWattBullet.verticalCenter = "middle";
            kWattBullet.width = 7;
            kWattBullet.height = 7;
            kWattRectangle.width = 7;
            kWattRectangle.height = 7;

        let kWattState = kWattBullet.states.create("hover");
            kWattState.properties.scale = 1.2;
            kWattSeries.tooltip.getFillFromObject = false;
            kWattSeries.tooltip.background.fill = am4core.color("#003764");

        //HistPay Column 
        let HistPaySeries = chart.series.push(new am4charts.ColumnSeries());
            HistPaySeries.dataFields.valueY = "HistPay";
            HistPaySeries.dataFields.dateX = "date";
            HistPaySeries.yAxis = HistPayAxis;
            HistPaySeries.tooltipText = "Charges: ${valueY}";
            HistPaySeries.name = "Charges";
            HistPaySeries.columns.template.fillOpacity = 0.7;
            HistPaySeries.columns.template.propertyFields.strokeDasharray = "dashLength";
            HistPaySeries.columns.template.propertyFields.fillOpacity = "alpha";
            HistPaySeries.showOnInit = true;

          let HistPayState = HistPaySeries.columns.template.states.create("hover");
              HistPayState.properties.fillOpacity = 0.9;

          // Add legend
          chart.legend = new am4charts.Legend();

          // Add cursor
          chart.cursor = new am4charts.XYCursor();
          chart.cursor.fullWidthLineX = true;
          chart.cursor.xAxis = dateAxis;
          chart.cursor.lineX.strokeOpacity = 0;
          chart.cursor.lineX.fill = am4core.color("#000");
          chart.cursor.lineX.fillOpacity = 0.1;
          
          return chart;
    }

    componentWillUnmount() {
      if (this.state.chart) {
        this.state.chart.dispose();
      }
    }

    render() {
            return (
              <div id="historical-chart" className="historical-chart" />
            );
        }
    }
    
    
