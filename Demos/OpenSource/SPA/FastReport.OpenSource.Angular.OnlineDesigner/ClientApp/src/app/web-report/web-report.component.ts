import { Component, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { SafeUrlPipe } from '../safeUrl.pipe';
import { HttpService } from '../http.service';

@Component({
  selector: 'app-web-report',
  templateUrl: './web-report.component.html',
  providers: [HttpService]
})

export class WebReportComponent {

  html: string = '';
  flag: boolean = false;
  constructor(private httpService: HttpService) { }

  Clicked() {
    this.flag = true;
    this.httpService.getData().subscribe((data: string) => { this.html = data; this.flag = true });
  }
}
