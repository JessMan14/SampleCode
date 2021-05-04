import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RadiusChartComponent } from './radius-chart.component';

describe('RadiusChartComponent', () => {
  let component: RadiusChartComponent;
  let fixture: ComponentFixture<RadiusChartComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RadiusChartComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RadiusChartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
