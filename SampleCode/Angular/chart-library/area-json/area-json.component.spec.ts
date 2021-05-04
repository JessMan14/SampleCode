import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AreaJsonComponent } from './area-json.component';

describe('AreaJsonComponent', () => {
  let component: AreaJsonComponent;
  let fixture: ComponentFixture<AreaJsonComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AreaJsonComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AreaJsonComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
