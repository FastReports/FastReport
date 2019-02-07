import { Component } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { SafeHtmlPipe } from "./safehtml.pipe";
import { HttpService } from "./http.service";


@Component({
  selector: 'app-root',
  template: `<div>
                    <input type="button" (click)="Clicked()" value = "Show Report"/>
                   <div *ngIf="show">
                    <iframe  id="report" height="1000" width="1000" [src]="url | safeUrl"></iframe>
                    </div>
               </div>`
})
export class AppComponent {
  show: boolean = false;
  url: string;

  Clicked() {
    this.show = true;
    this.url = "api/SampleData/ShowReport";
  }
}
