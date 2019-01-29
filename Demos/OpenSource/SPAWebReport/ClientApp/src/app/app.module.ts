import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { AppComponent } from './app.component';
import { SafeHtmlPipe } from "./safehtml.pipe";
import { SafeUrlPipe } from "./safeUrl.pipe";

@NgModule({
  declarations: [
    AppComponent,
    SafeHtmlPipe,
    SafeUrlPipe
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }

