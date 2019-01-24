import { Component} from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { HttpService } from "./http.service";

@Component({
    selector: 'app-root',
    template: `<div>
                    <input type="button" (click)="Clicked()" value = "Show Online Designer"/>
                    <div *ngIf="flag" [innerHTML]="html | safeHtml"></div>
               </div>`,
    styleUrls: ['./app.component.css'],
    providers: [HttpService]
})
export class AppComponent {
    html: string;
    flag: boolean;
    constructor(private httpService: HttpService) { }

    Clicked() {
        this.flag = false;
        this.httpService.getData().subscribe((data: string) => { this.html = data; this.flag = true });
    }
}
