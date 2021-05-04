import { Component, AfterViewInit, OnInit, NgZone, Input } from '@angular/core';
import * as am4core from '@amcharts/amcharts4/core';
import * as am4maps from "@amcharts/amcharts4/maps";
import * as am4charts from "@amcharts/amcharts4/charts";
import am4themes_animated from '@amcharts/amcharts4/themes/animated';
import am4geodata_usaLow from  '@amcharts/amcharts4-geodata/usaLow';

@Component({
  selector: 'app-map-chart',
  templateUrl: './map-chart.component.html',
  styleUrls: ['./map-chart.component.scss']
})
export class MapChartComponent implements AfterViewInit {
  @Input() data = new Array();
  @Input() divId: string;

  primaryGreenColor: string = "#83c341";

  constructor(private zone: NgZone) { }

  ngAfterViewInit(): void {
    this.zone.runOutsideAngular(() => {
      am4core.useTheme(am4themes_animated);
      const areaChart = am4core.createFromConfig(this.getChartConfig(), this.divId, am4maps.MapChart );
    });
  }

  getChartConfig(): any{
    return this.data;
  }
} 

