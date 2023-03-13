import { Component } from '@angular/core';

@Component({
  selector: 'app-web-report',
  templateUrl: './web-report.component.html'
})
export class WebReportComponent {

  showReport: boolean = false;
  url = "";

  public renderWebReport() {
    this.showReport = true;
    this.url = "webreport/get";
  }
}
