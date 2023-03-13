import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { SafeUrlPipe } from './safeUrl.pipe';
import { AppComponent } from './app.component';
import { WebReportComponent } from './web-report/web-report.component';

@
NgModule({
  declarations: [
    AppComponent,
    WebReportComponent,
    SafeUrlPipe
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', component: WebReportComponent}
    ])
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
