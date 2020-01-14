import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { BrowserModule } from "@angular/platform-browser";
import { AppRoutingModule } from './app-routing.module';
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { AppComponent } from "./app.component";
import { RemoteFilteringSampleComponent } from "./ig-grid/remote-filtering-sample.component";
import { HomeComponent } from "./home/home.component";
import { IgxGridModule, IgxBadgeModule, IgxToastModule } from "igniteui-angular";
import { HttpClientModule } from "@angular/common/http";
import { RemoteFilteringService } from "./services/remoteFilteringService";
import { RemotePagingGridSample } from './remote-paging-sample/remote-paging-sample.component';
import { ContextmenuComponent } from "./remote-paging-sample/contextmenu/contextmenu.component";

@NgModule({
  bootstrap: [AppComponent],
  declarations: [
    AppComponent,
    HomeComponent,
    RemoteFilteringSampleComponent,
    RemotePagingGridSample,
    ContextmenuComponent
  ],
  imports: [
    AppRoutingModule,
    BrowserModule,
    BrowserAnimationsModule,
    FormsModule,
    IgxGridModule,
    IgxBadgeModule,
    HttpClientModule,
    IgxToastModule,
    ReactiveFormsModule
  ],
  providers: [RemoteFilteringService],
  entryComponents: []
})
export class AppModule {
}
