import {AfterViewInit, Component, NgZone, OnInit, Input} from '@angular/core';
import * as am4core from '@amcharts/amcharts4/core';
import * as am4charts from '@amcharts/amcharts4/charts';
import am4themes_animated from '@amcharts/amcharts4/themes/animated';

@Component({
  selector: 'app-bar-chart',
  templateUrl: './bar-chart.component.html',
  styleUrls: ['./bar-chart.component.scss']
})
export class BarChartComponent implements AfterViewInit {
  @Input() data = new Array();
  @Input() divId: string;
  @Input() height: string;

  constructor(private zone: NgZone) { }

  ngAfterViewInit(): void {
    this.zone.runOutsideAngular(() => {
      am4core.useTheme(am4themes_animated);
      const areaChart = am4core.createFromConfig(this.getChartConfig(), this.divId, am4charts.XYChart );
    });
  }

  getChartConfig(): any{

    return this.data;
  }
}
