import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { RemoteFilteringSampleComponent } from './ig-grid/remote-filtering-sample.component';
import { RemotePagingGridSample } from './remote-paging-sample/remote-paging-sample.component';

const routes: Routes = [
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  { path: 'home', component: HomeComponent, data: { text: 'Home' } },
  { path: 'ig-grid', component: RemoteFilteringSampleComponent, data: { text: 'ig-grid' } },
  { path: 'ig-grid-paging', component: RemotePagingGridSample, data: { text: 'ig-grid-paging' } }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {
}
