import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { Routes } from '@angular/router';
import { RegisterComponent } from './register/register.component';
import { HomeComponent } from '../home/home.component';
import { provideHttpClient } from '@angular/common/http';
import { LoginComponent } from '../login/login.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { HeaderComponent } from './header/header.component';
import { authGuard } from './Guard/auth.guard';
import { DoctorDashboardComponent } from '../doctor-dashboard/doctor-dashboard.component';
import { PatientDashboardComponent } from '../patient-dashboard/patient-dashboard.component';
import { AppointmentsComponent } from '../appointments/appointments.component';
import { MedicalRecordsComponent } from '../medical-records/medical-records.component';
import { PrescriptionComponent } from '../prescription/prescription.component';
import { ForgotPasswordComponent } from '../forgot-password/forgot-password/forgot-password.component';


const routes : Routes =[
  {
    path: 'app-register',
    component: RegisterComponent
  },
  {
    path: 'app-login',
    component: LoginComponent
  },
  {
    path: 'app-forgot-password',
    component: ForgotPasswordComponent
  },
  {
    path:'app-dashboard',
    component: DashboardComponent,
    canActivate:[authGuard]
  },
  {
    path:'app-patient-dashboard',
    component: PatientDashboardComponent,
    canActivate:[authGuard]
  },
  { path: 'app-appointments', 
    component: AppointmentsComponent,
    canActivate:[authGuard]
   },
  { path: 'app-medical-records',
     component: MedicalRecordsComponent,
     canActivate:[authGuard]
 },
  { path: 'app-prescription', 
    component: PrescriptionComponent,
    canActivate:[authGuard]
 },
  {
    path:'app-doctor-dashboard',
    component: DoctorDashboardComponent,
    canActivate:[authGuard]
  },
  

  {
    path: 'app-header',
    component: HeaderComponent,
    children:[
      {
        path:'app-patient-dashboard',
        component: PatientDashboardComponent,
        canActivate:[authGuard]
      },
      {
        path:'app-doctor-dashboard',
        component: DoctorDashboardComponent,
        canActivate:[authGuard]
      }
    ]
  },
  {
    path: 'app-home',
    component: HomeComponent,
  },
  { path: '', redirectTo: 'app-home', pathMatch: 'full' }
]

export const appConfig: ApplicationConfig = {
  providers: [provideHttpClient(),provideZoneChangeDetection({ eventCoalescing: true }), provideRouter(routes)]
};
