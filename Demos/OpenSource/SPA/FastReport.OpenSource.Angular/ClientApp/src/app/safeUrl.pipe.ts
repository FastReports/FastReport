import { Pipe, PipeTransform } from '@angular/core';
import { DomSanitizer, SafeHtml, SafeResourceUrl } from '@angular/platform-browser';

@Pipe({
  name: 'safeUrl'
})
export class SafeUrlPipe implements PipeTransform {

  constructor(private domSanitizer: DomSanitizer) {}

  public transform(value: string, type: string): SafeHtml | SafeResourceUrl {
    switch (type) {
    case 'resourceUrl':
      return this.domSanitizer.bypassSecurityTrustResourceUrl(value);
    default:
      return this.domSanitizer.bypassSecurityTrustHtml(value);
    }
  }
}
